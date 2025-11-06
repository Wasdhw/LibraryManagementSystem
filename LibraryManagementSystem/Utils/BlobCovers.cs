using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace LibraryManagementSystem.Utils
{
	public static class BlobCovers
	{
		private static BlobContainerClient _container;
		private static readonly object _lock = new object();
		private static string CacheFilePath => Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
			"LibraryManagementSystem",
			"bookCoversCache.txt");

		public static void Initialize(string connectionString, string containerName = "book-covers")
		{
			if (_container != null) return;
			lock (_lock)
			{
				if (_container != null) return;
				var container = new BlobContainerClient(connectionString, containerName);
				container.CreateIfNotExists(PublicAccessType.None);
				_container = container;
			}
		}

		public static BlobContainerClient Container
		{
			get
			{
				if (_container == null) throw new InvalidOperationException("BlobCovers not initialized. Call Initialize once at startup.");
				return _container;
			}
		}

		public static string GenerateBlobName(string key)
		{
			if (string.IsNullOrWhiteSpace(key)) key = Guid.NewGuid().ToString("N");
			return key.Replace(" ", "_") + ".jpg";
		}

        public static async Task<string> UploadAsync(Image image, string blobName)
        {
            var processed = EnsureMaxSize(image, 800);
            try
            {
                using (var ms = new MemoryStream())
                {
                    processed.Save(ms, ImageFormat.Jpeg);
                    ms.Position = 0;

                    var blob = Container.GetBlobClient(blobName);
                    // Ensure overwrite semantics on older SDK overloads
                    await blob.DeleteIfExistsAsync();
                    var options = new BlobUploadOptions
                    {
                        HttpHeaders = new BlobHttpHeaders { ContentType = "image/jpeg" }
                    };
                    await blob.UploadAsync(ms, options);
                    
                    // Update cache with new upload timestamp
                    var cache = LoadCache();
                    var properties = await blob.GetPropertiesAsync();
                    cache[blobName] = properties.Value.LastModified.DateTime;
                    SaveCache(cache);
                    
                    return blob.Name;
                }
            }
            finally
            {
                if (processed != null) processed.Dispose();
            }
        }

        public static async Task<Image> DownloadAsync(string blobName)
        {
            if (string.IsNullOrWhiteSpace(blobName)) return null;
            var blob = Container.GetBlobClient(blobName);
            if (!await blob.ExistsAsync()) return null;
            using (var ms = new MemoryStream())
            {
                await blob.DownloadToAsync(ms);
                ms.Position = 0;
                return Image.FromStream(ms);
            }
        }

		public static async Task DeleteIfExistsAsync(string blobName)
		{
			if (string.IsNullOrWhiteSpace(blobName)) return;
			await Container.DeleteBlobIfExistsAsync(blobName);
		}

		/// <summary>
		/// Checks if there are any new or updated book covers in Azure Blob Storage
		/// </summary>
		/// <returns>True if there are new/updated covers, false otherwise</returns>
		public static async Task<bool> HasNewOrUpdatedCoversAsync()
		{
			var cachedCovers = LoadCache();
			var bookCovers = new List<BookCoverInfo>();
			
			// Get all book cover blob names from database
			using (var con = Database.GetConnection())
			{
				con.Open();
				string query = "SELECT id, book_title, image FROM books WHERE date_delete IS NULL AND image IS NOT NULL AND image != ''";
				using (var cmd = new SqlCommand(query, con))
				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						string blobName = reader["image"]?.ToString();
						if (!string.IsNullOrWhiteSpace(blobName))
						{
							bookCovers.Add(new BookCoverInfo { BlobName = blobName });
						}
					}
				}
			}

			// Check each blob for updates
			foreach (var bookInfo in bookCovers)
			{
				try
				{
					var blob = Container.GetBlobClient(bookInfo.BlobName);
					if (await blob.ExistsAsync())
					{
						var properties = await blob.GetPropertiesAsync();
						var lastModified = properties.Value.LastModified.DateTime;

						// Check if blob is new or has been updated (compare with 1 second tolerance)
						if (!cachedCovers.ContainsKey(bookInfo.BlobName))
						{
							return true; // New cover
						}
						var cachedDate = cachedCovers[bookInfo.BlobName];
						var timeDiff = Math.Abs((lastModified - cachedDate).TotalSeconds);
						if (timeDiff > 1.0) // More than 1 second difference means updated
						{
							return true; // Updated cover
						}
					}
				}
				catch
				{
					// If we can't check, assume there might be updates
					return true;
				}
			}

			return false; // No new or updated covers
		}

		/// <summary>
		/// Downloads new/updated book cover images from the database with progress reporting
		/// </summary>
		/// <param name="progressCallback">Callback function: (current, total, bookTitle) => void</param>
		public static async Task DownloadAllBookCoversAsync(Action<int, int, string> progressCallback = null)
		{
			var cachedCovers = LoadCache();
			var bookCovers = new List<BookCoverInfo>();
			var coversToDownload = new List<BookCoverInfo>();
			
			// Get all book cover blob names from database
			using (var con = Database.GetConnection())
			{
				con.Open();
				string query = "SELECT id, book_title, image FROM books WHERE date_delete IS NULL AND image IS NOT NULL AND image != ''";
				using (var cmd = new SqlCommand(query, con))
				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						int id = (int)reader["id"];
						string title = reader["book_title"]?.ToString() ?? "";
						string blobName = reader["image"]?.ToString();
						if (!string.IsNullOrWhiteSpace(blobName))
						{
							bookCovers.Add(new BookCoverInfo { Id = id, BlobName = blobName, Title = title });
						}
					}
				}
			}

			// Check which covers need to be downloaded (new or updated)
			foreach (var bookInfo in bookCovers)
			{
				try
				{
					var blob = Container.GetBlobClient(bookInfo.BlobName);
					if (await blob.ExistsAsync())
					{
						var properties = await blob.GetPropertiesAsync();
						var lastModified = properties.Value.LastModified.DateTime;

						// Add to download list if new or updated (compare with 1 second tolerance)
						bool needsDownload = false;
						if (!cachedCovers.ContainsKey(bookInfo.BlobName))
						{
							needsDownload = true; // New cover
						}
						else
						{
							var cachedDate = cachedCovers[bookInfo.BlobName];
							var timeDiff = Math.Abs((lastModified - cachedDate).TotalSeconds);
							if (timeDiff > 1.0) // More than 1 second difference means updated
							{
								needsDownload = true; // Updated cover
							}
						}

						if (needsDownload)
						{
							coversToDownload.Add(bookInfo);
							// Update cache with new timestamp
							cachedCovers[bookInfo.BlobName] = lastModified;
						}
					}
				}
				catch
				{
					// If check fails, download it anyway
					coversToDownload.Add(bookInfo);
				}
			}

			int total = coversToDownload.Count;
			int current = 0;

			// Download each cover that needs updating
			foreach (var bookInfo in coversToDownload)
			{
				try
				{
					await DownloadAsync(bookInfo.BlobName);
					current++;
					progressCallback?.Invoke(current, total, bookInfo.Title);
				}
				catch
				{
					// Continue even if one fails
					current++;
					progressCallback?.Invoke(current, total, bookInfo.Title);
				}
			}

			// Save updated cache
			SaveCache(cachedCovers);
		}

		private static Dictionary<string, DateTime> LoadCache()
		{
			var cache = new Dictionary<string, DateTime>();
			
			try
			{
				if (File.Exists(CacheFilePath))
				{
					var lines = File.ReadAllLines(CacheFilePath);
					foreach (var line in lines)
					{
						if (string.IsNullOrWhiteSpace(line)) continue;
						var parts = line.Split('|');
						if (parts.Length == 2 && DateTime.TryParse(parts[1], out DateTime date))
						{
							cache[parts[0]] = date;
						}
					}
				}
			}
			catch
			{
				// If cache file is corrupted, start fresh
			}

			return cache;
		}

		private static void SaveCache(Dictionary<string, DateTime> cache)
		{
			try
			{
				var directory = Path.GetDirectoryName(CacheFilePath);
				if (!Directory.Exists(directory))
				{
					Directory.CreateDirectory(directory);
				}

				var lines = cache.Select(kvp => $"{kvp.Key}|{kvp.Value:yyyy-MM-dd HH:mm:ss}");
				File.WriteAllLines(CacheFilePath, lines);
			}
			catch
			{
				// If we can't save cache, continue anyway
			}
		}

		private class BookCoverInfo
		{
			public int Id { get; set; }
			public string BlobName { get; set; }
			public string Title { get; set; }
		}

		private static Image EnsureMaxSize(Image source, int max)
		{
			if (source == null) return null;
			if (source.Width <= max && source.Height <= max) return (Image)source.Clone();
			float ratio = Math.Min((float)max / source.Width, (float)max / source.Height);
			int w = (int)(source.Width * ratio);
			int h = (int)(source.Height * ratio);
			var bmp = new Bitmap(w, h);
			using (var g = Graphics.FromImage(bmp))
			{
				g.SmoothingMode = SmoothingMode.HighQuality;
				g.InterpolationMode = InterpolationMode.HighQualityBicubic;
				g.PixelOffsetMode = PixelOffsetMode.HighQuality;
				g.DrawImage(source, 0, 0, w, h);
			}
			return bmp;
		}
	}
}


