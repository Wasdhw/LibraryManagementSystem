using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using LibraryManagementSystem.Utils;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryManagementSystem
{
    public partial class AvailBooks : UserControl
    {
        private List<BookData> allBooks = new List<BookData>();
        private bool isLoading = false;
        private string pendingSearchTerm = "";

        public AvailBooks()
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
            // Prevent concurrent loads
            if (isLoading)
            {
                pendingSearchTerm = searchTerm;
                return;
            }

            isLoading = true;
            pendingSearchTerm = "";

            try
            {
                // Ensure we're on the UI thread before clearing controls
                if (InvokeRequired)
                {
                    Invoke((MethodInvoker)(() => flowAvailableBooks.Controls.Clear()));
                }
                else
                {
                    flowAvailableBooks.Controls.Clear();
                }

                allBooks.Clear();

                // Include reservation count and category in query
                string query = @"
                    SELECT 
                        b.id, 
                        b.book_title, 
                        b.author, 
                        b.status, 
                        b.image,
                        c.name as category_name,
                        (SELECT COUNT(*) FROM reservations r WHERE r.book_id = b.id AND r.status IN ('Pending', 'Notified') AND r.date_expires > GETDATE()) as reservation_count
                    FROM books b 
                    LEFT JOIN categories c ON b.category_id = c.id
                    WHERE b.date_delete IS NULL AND b.status = 'Available'";

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

                            int reservationCount = reader["reservation_count"] != DBNull.Value ? Convert.ToInt32(reader["reservation_count"]) : 0;
                            string category = reader["category_name"] != DBNull.Value ? reader["category_name"].ToString() : "";
                            string status = reader["status"].ToString();

                            // Store book data
                            allBooks.Add(new BookData
                            {
                                Id = id,
                                Title = title,
                                Author = author,
                                BlobName = blobName,
                                ReservationCount = reservationCount,
                                Category = category,
                                Status = status
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

                // Sort books: Available first, then by title
                filteredBooks = filteredBooks.OrderByDescending(b => b.Status == "Available")
                    .ThenBy(b => b.Title).ToList();

                // Display filtered books
                await DisplayBooks(filteredBooks);
            }
            finally
            {
                isLoading = false;
                
                // Process any pending search that came in while we were loading
                if (!string.IsNullOrEmpty(pendingSearchTerm))
                {
                    LoadAvailableBooksAsync(pendingSearchTerm);
                }
            }
        }

        private async Task DisplayBooks(List<BookData> books)
        {
            // Create a list to hold all panels before adding them
            List<Panel> panelsToAdd = new List<Panel>();

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

                // Create panel on UI thread
                if (InvokeRequired)
                {
                    Panel panel = null;
                    Invoke((MethodInvoker)(() => panel = CreateBookPanel(book, img)));
                    if (panel != null)
                        panelsToAdd.Add(panel);
                }
                else
                {
                    panelsToAdd.Add(CreateBookPanel(book, img));
                }
            }

            // Add all panels at once to minimize collection modifications
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)(() =>
                {
                    foreach (var panel in panelsToAdd)
                    {
                        flowAvailableBooks.Controls.Add(panel);
                    }
                }));
            }
            else
            {
                foreach (var panel in panelsToAdd)
                {
                    flowAvailableBooks.Controls.Add(panel);
                }
            }
        }

        private Panel CreateBookPanel(BookData book, Image img)
        {
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
            // Create title label first so layout reserves space at the bottom
            Label lbl = new Label();
            string titleText = book.Title;
            if (book.ReservationCount > 0)
            {
                titleText += $"\n({book.ReservationCount} reservation(s))";
            }
            lbl.Text = titleText; // ensure we show DB title, not file name
            lbl.AutoSize = false;
            lbl.Dock = DockStyle.Bottom;
            lbl.Height = book.ReservationCount > 0 ? 60 : 44;
            lbl.Font = new Font("Arial", 11, FontStyle.Bold);
            lbl.TextAlign = ContentAlignment.MiddleCenter;
            lbl.ForeColor = Color.Black;
            lbl.BackColor = SystemColors.Control;
            pnl.Controls.Add(lbl);

            // Add image after so it docks at the top and doesn't overlap the label
            pnl.Controls.Add(pb);

            return pnl;
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
            // Open info form for the selected book id
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
            public int ReservationCount { get; set; }
            public string Category { get; set; }
            public string Status { get; set; }
        }

        private void flowAvailableBooks_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }
    }

}
