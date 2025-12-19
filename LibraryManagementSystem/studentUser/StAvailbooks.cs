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
        private bool isLoading = false;
        private string pendingSearchTerm = "";
        private string currentSortField = "Title";
        private string currentSortOrder = "Ascending";

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

            // Initialize sorting controls
            InitializeSortingControls();
        }

        private void InitializeSortingControls()
        {
            if (sortFieldComboBox != null)
            {
                sortFieldComboBox.Items.AddRange(new string[] { "Title", "Author", "Status", "Quantity" });
                sortFieldComboBox.SelectedIndex = 0;
                sortFieldComboBox.SelectedIndexChanged += SortFieldComboBox_SelectedIndexChanged;
            }

            if (sortOrderComboBox != null)
            {
                sortOrderComboBox.Items.AddRange(new string[] { "Ascending", "Descending" });
                sortOrderComboBox.SelectedIndex = 0;
                sortOrderComboBox.SelectedIndexChanged += SortOrderComboBox_SelectedIndexChanged;
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

        private void SortFieldComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sortFieldComboBox != null && sortFieldComboBox.SelectedItem != null)
            {
                currentSortField = sortFieldComboBox.SelectedItem.ToString();
                ApplySorting();
            }
        }

        private void SortOrderComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sortOrderComboBox != null && sortOrderComboBox.SelectedItem != null)
            {
                currentSortOrder = sortOrderComboBox.SelectedItem.ToString();
                ApplySorting();
            }
        }

        private void ApplySorting()
        {
            if (allBooks == null || allBooks.Count == 0) return;

            try
            {
                IEnumerable<BookData> sortedBooks = allBooks;

                switch (currentSortField)
                {
                    case "Title":
                        sortedBooks = currentSortOrder == "Ascending" 
                            ? allBooks.OrderBy(b => b.Title) 
                            : allBooks.OrderByDescending(b => b.Title);
                        break;
                    case "Author":
                        sortedBooks = currentSortOrder == "Ascending" 
                            ? allBooks.OrderBy(b => b.Author) 
                            : allBooks.OrderByDescending(b => b.Author);
                        break;
                    case "Status":
                        sortedBooks = currentSortOrder == "Ascending" 
                            ? allBooks.OrderBy(b => b.Status) 
                            : allBooks.OrderByDescending(b => b.Status);
                        break;
                    case "Quantity":
                        sortedBooks = currentSortOrder == "Ascending" 
                            ? allBooks.OrderBy(b => b.Quantity) 
                            : allBooks.OrderByDescending(b => b.Quantity);
                        break;
                    default:
                        sortedBooks = allBooks.OrderBy(b => b.Title);
                        break;
                }

                DisplayBooks(sortedBooks.ToList());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error applying sorting: {ex.Message}");
            }
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

                // Load both available and unavailable books (for reservation)
                string query = "SELECT id, book_title, author, status, quantity, image FROM books WHERE date_delete IS NULL";

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

                            string status = reader["status"].ToString();
                            int quantity = reader["quantity"] != DBNull.Value ? Convert.ToInt32(reader["quantity"]) : 0;

                            // Store book data
                            allBooks.Add(new BookData
                            {
                                Id = id,
                                Title = title,
                                Author = author,
                                BlobName = blobName,
                                Status = status,
                                Quantity = quantity
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

                // Apply sorting
                if (currentSortField != null && currentSortOrder != null)
                {
                    switch (currentSortField)
                    {
                        case "Title":
                            filteredBooks = currentSortOrder == "Ascending" 
                                ? filteredBooks.OrderBy(b => b.Title).ToList() 
                                : filteredBooks.OrderByDescending(b => b.Title).ToList();
                            break;
                        case "Author":
                            filteredBooks = currentSortOrder == "Ascending" 
                                ? filteredBooks.OrderBy(b => b.Author).ToList() 
                                : filteredBooks.OrderByDescending(b => b.Author).ToList();
                            break;
                        case "Status":
                            filteredBooks = currentSortOrder == "Ascending" 
                                ? filteredBooks.OrderBy(b => b.Status).ToList() 
                                : filteredBooks.OrderByDescending(b => b.Status).ToList();
                            break;
                        case "Quantity":
                            filteredBooks = currentSortOrder == "Ascending" 
                                ? filteredBooks.OrderBy(b => b.Quantity).ToList() 
                                : filteredBooks.OrderByDescending(b => b.Quantity).ToList();
                            break;
                        default:
                            // Default: Available books first, then unavailable
                            filteredBooks = filteredBooks.OrderByDescending(b => b.Status == "Available" && b.Quantity > 0).ToList();
                            break;
                    }
                }
                else
                {
                    // Default: Available books first, then unavailable
                    filteredBooks = filteredBooks.OrderByDescending(b => b.Status == "Available" && b.Quantity > 0).ToList();
                }

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

            bool isAvailable = book.Status == "Available" && book.Quantity > 0;

            Panel pnl = new Panel();
            pnl.Width = pb.Width + 25;
            pnl.Height = pb.Height + (isAvailable ? 60 : 100); // Extra height for reserve button
            pnl.BackColor = SystemColors.Control;
            pnl.Padding = new Padding(5, 5, 5, 5);

            // Add reserve button for unavailable books
            if (!isAvailable)
            {
                Button btnReserve = new Button();
                btnReserve.Text = "RESERVE";
                btnReserve.Dock = DockStyle.Bottom;
                btnReserve.Height = 35;
                btnReserve.BackColor = Color.FromArgb(14, 128, 87);
                btnReserve.ForeColor = Color.White;
                btnReserve.FlatStyle = FlatStyle.Flat;
                btnReserve.FlatAppearance.BorderSize = 0;
                btnReserve.Tag = book.Id;
                btnReserve.Click += BtnReserve_Click;
                pnl.Controls.Add(btnReserve);
            }

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

            return pnl;
        }

        private void BtnReserve_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag != null)
            {
                int bookId = Convert.ToInt32(btn.Tag);
                int userId = SessionManager.CurrentUserId > 0 ? SessionManager.CurrentUserId : 1;
                
                int reservationId = 0;
                if (ReservationManager.CreateReservation(userId, bookId))
                {
                    // Get reservation ID for audit log
                    using (var con = Database.GetConnection())
                    {
                        con.Open();
                        string query = "SELECT TOP 1 id FROM reservations WHERE user_id = @user_id AND book_id = @book_id ORDER BY date_reserved DESC";
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@user_id", userId);
                            cmd.Parameters.AddWithValue("@book_id", bookId);
                            object result = cmd.ExecuteScalar();
                            if (result != null)
                            {
                                reservationId = Convert.ToInt32(result);
                            }
                        }
                    }

                    // Log audit action
                    AuditLogger.LogReservationCreate(reservationId, bookId, userId);

                    MessageBox.Show("Book reserved successfully! You will be notified when it becomes available.", 
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    refreshData();
                }
                else
                {
                    MessageBox.Show("Unable to create reservation. You may already have a reservation for this book.", 
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
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
            // Open BookInfoForm with student mode enabled
            BookInfoForm info = new BookInfoForm(tag.Id, isStudentForm: true);
            info.ShowDialog();
            // Refresh data after closing in case reservation was made/cancelled
            refreshData();
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
            public string Status { get; set; }
            public int Quantity { get; set; }
        }

    }
}
