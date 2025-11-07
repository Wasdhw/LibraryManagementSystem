using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibraryManagementSystem.Utils;

namespace LibraryManagementSystem.studentUser
{
    public partial class StAvailbooks : UserControl
    {
        private List<BookData> allBooks = new List<BookData>();

        public StAvailbooks()
        {
            InitializeComponent();

            LoadAvailableBooksAsync();

            this.Resize += AvailBooks_Resize;

            // Wire up search textbox event
            if (searchTextBox != null)
            {
                searchTextBox.TextChanged += SearchTextBox_TextChanged;
            }
        }

        public void refreshData()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)refreshData);
                return;
            }

            string searchTerm = searchTextBox != null ? searchTextBox.Text.Trim() : "";
            LoadAvailableBooksAsync(searchTerm);
        }



        private void AvailBooks_Resize(object sender, EventArgs e)
        {
            // Optionally, adjust child control sizes or spacing
        }

        private async void LoadAvailableBooksAsync(string searchTerm = "")
        {
            flowAvailableBooks.Controls.Clear();
            allBooks.Clear();

            string query = "SELECT id, book_title, author, status, image FROM books WHERE date_delete IS NULL AND status = 'Available'";

            using (SqlConnection con = Database.GetConnection())
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = (int)reader["id"];
                        string title = reader["book_title"].ToString();
                        string author = reader["author"].ToString();
                        string blobName = reader["image"]?.ToString();

                        // Store book data
                        allBooks.Add(new BookData
                        {
                            Id = id,
                            Title = title,
                            Author = author,
                            BlobName = blobName
                        });
                    }
                }
            }

            // Filter books based on search term
            var filteredBooks = allBooks;
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string searchLower = searchTerm.ToLower();
                filteredBooks = allBooks.Where(b => 
                    b.Title.ToLower().Contains(searchLower) || 
                    b.Author.ToLower().Contains(searchLower)).ToList();
            }

            // Display filtered books
            await DisplayBooks(filteredBooks);
        }

        private async Task DisplayBooks(List<BookData> books)
        {
            foreach (var book in books)
            {
                // Download image from Azure Blob Storage
                Image img = null;
                try
                {
                    if (!string.IsNullOrWhiteSpace(book.BlobName))
                    {
                        img = await BlobCovers.DownloadAsync(book.BlobName);
                    }
                }
                catch (Exception ex)
                {
                    // Silently fail - just show no image
                    System.Diagnostics.Debug.WriteLine($"Failed to load image for book {book.Id}: {ex.Message}");
                }

                PictureBox pb = new PictureBox();
                pb.Size = new Size(145, 176);
                pb.SizeMode = PictureBoxSizeMode.Zoom;
                pb.Image = img;
                pb.Cursor = Cursors.Hand;
                pb.Margin = new Padding(10);
                pb.TabStop = false;
                pb.Dock = DockStyle.Top;

                // Store info 
                pb.Tag = new BookTag
                {
                    Id = book.Id,
                    Title = book.Title,
                    Author = book.Author,
                    ImagePath = book.BlobName
                };

                pb.Click += Pb_Click;

                Panel pnl = new Panel();
                pnl.Width = pb.Width + 25;
                pnl.Height = pb.Height + 60;
                pnl.BackColor = SystemColors.Control;
                pnl.Padding = new Padding(5, 5, 5, 5);

                Label lbl = new Label();
                lbl.Text = book.Title; // show DB title
                lbl.AutoSize = false;
                lbl.Dock = DockStyle.Bottom;
                lbl.Height = 44;
                lbl.Font = new Font("Arial", 11, FontStyle.Bold);
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.ForeColor = Color.Black;
                lbl.BackColor = SystemColors.Control;
                pnl.Controls.Add(lbl);

                // Add image after so it docks at the top and doesn't overlap the label
                pnl.Controls.Add(pb);

                flowAvailableBooks.Controls.Add(pnl);
            }
        }

        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            if (sender is TextBox textBox)
            {
                string searchTerm = textBox.Text.Trim();
                LoadAvailableBooksAsync(searchTerm);
            }
        }

        private void Pb_Click(object sender, EventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            if (pb != null && pb.Tag is BookTag tag)
            {
                // Open or display the book info
                ShowBookInfo(tag);
            }
        }

        private void ShowBookInfo(BookTag tag)
        {
            // Open admin BookInfoForm with selected book id for consistency
            BookInfoForm info = new BookInfoForm(tag.Id);
            info.ShowDialog();
        }

        private class BookTag
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Author { get; set; }
            public string ImagePath { get; set; }
        }

        private class BookData
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Author { get; set; }
            public string BlobName { get; set; }
        }

    }
}
