using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace LibraryManagementSystem.Utils
{
	public static class BlobCovers
	{
		private static BlobContainerClient _container;
		private static readonly object _lock = new object();

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


