using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibraryManagementSystem.Utils;

namespace LibraryManagementSystem.studentUser
{
    public partial class StReturnBooks : UserControl
    {
        SqlConnection connect = Database.GetConnection();
        private int currentUserId = 0;
        private static DateTime lastStudentNotificationTime = DateTime.MinValue;
        private const int StudentNotificationCooldownMinutes = 5; // Only show notification once every 5 minutes

        public StReturnBooks()
        {
            InitializeComponent();
            // Clear placeholder panels at startup
            flowAvailableBooks.Controls.Clear();
            this.Resize += StReturnBooks_Resize;
            this.Load += StReturnBooks_Load;
        }

        private void StReturnBooks_Load(object sender, EventArgs e)
        {
            // Load books when control becomes visible
            if (currentUserId == 0)
            {
                currentUserId = SessionManager.CurrentUserId > 0 ? SessionManager.CurrentUserId : 1;
            }
            LoadBorrowedBooksAsync();
        }

        public void LoadUserBorrowedBooks(int userId = 0)
        {
            currentUserId = userId > 0 ? userId : (SessionManager.CurrentUserId > 0 ? SessionManager.CurrentUserId : 1);
            LoadBorrowedBooksAsync();
        }

        public void refreshData()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)refreshData);
                return;
            }
            LoadBorrowedBooksAsync();
        }

        private void StReturnBooks_Resize(object sender, EventArgs e)
        {
            // Optionally, adjust child control sizes or spacing
        }

        private async void LoadBorrowedBooksAsync()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => { flowAvailableBooks.Controls.Clear(); }));
                }
                else
                {
                    flowAvailableBooks.Controls.Clear();
                }

                if (connect.State == ConnectionState.Closed)
                {
                    connect.Open();
                }

                string query = @"
                    SELECT 
                        i.id,
                        i.issue_id,
                        b.id as book_id,
                        b.book_title,
                        b.author,
                        b.image,
                        i.issue_date,
                        i.return_date,
                        i.status,
                        DATEDIFF(day, i.return_date, GETDATE()) as days_overdue
                    FROM issues i
                    INNER JOIN books b ON i.book_id = b.id
                    WHERE i.user_id = @userId 
                    AND i.status = 'Not Return' 
                    AND i.date_delete IS NULL
                    ORDER BY i.issue_date DESC";

                var borrowedBooks = new List<BorrowedBookInfo>();

                using (SqlCommand cmd = new SqlCommand(query, connect))
                {
                    cmd.Parameters.AddWithValue("@userId", currentUserId);
                    
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var bookInfo = new BorrowedBookInfo
                            {
                                IssueId = reader["issue_id"].ToString(),
                                BookId = Convert.ToInt32(reader["book_id"]),
                                Title = reader["book_title"].ToString(),
                                Author = reader["author"].ToString(),
                                BlobName = reader["image"]?.ToString(),
                                IssueDate = Convert.ToDateTime(reader["issue_date"]),
                                ReturnDate = Convert.ToDateTime(reader["return_date"]),
                                Status = reader["status"].ToString(),
                                DaysOverdue = reader["days_overdue"] != DBNull.Value ? Convert.ToInt32(reader["days_overdue"]) : 0
                            };
                            borrowedBooks.Add(bookInfo);
                        }
                    }
                }

                // Check for overdue books and notify student
                CheckAndNotifyStudentOverdueBooks(borrowedBooks);

                // Create UI for each borrowed book
                // Since LoadUserBorrowedBooks is called from UI thread, we should be on UI thread
                foreach (var book in borrowedBooks)
                {
                    await CreateBookPanel(book);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading borrowed books: " + ex.Message, "Error Message", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (connect.State == ConnectionState.Open)
                {
                    connect.Close();
                }
            }
        }

        private async Task CreateBookPanel(BorrowedBookInfo book)
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
                System.Diagnostics.Debug.WriteLine($"Failed to load image for book {book.BookId}: {ex.Message}");
            }

            PictureBox pb = new PictureBox();
            pb.Size = new Size(145, 176);
            pb.SizeMode = PictureBoxSizeMode.Zoom;
            pb.Image = img;
            pb.Cursor = Cursors.Hand;
            pb.Margin = new Padding(10);
            pb.TabStop = false;
            pb.Dock = DockStyle.Top;

            // Store book info in Tag
            pb.Tag = new BorrowedBookTag
            {
                IssueId = book.IssueId,
                BookId = book.BookId,
                Title = book.Title,
                Author = book.Author,
                ImagePath = book.BlobName,
                DaysOverdue = book.DaysOverdue,
                IsOverdue = book.DaysOverdue > 3
            };

            pb.Click += Pb_Click;

            Panel pnl = new Panel();
            pnl.Width = pb.Width + 25;
            pnl.Height = pb.Height + 80; // Extra height for additional info
            pnl.BackColor = book.DaysOverdue > 3 ? Color.LightCoral : SystemColors.Control;
            pnl.Padding = new Padding(5, 5, 5, 5);
            pnl.BorderStyle = book.DaysOverdue > 3 ? BorderStyle.FixedSingle : BorderStyle.None;

            // Create title label
            Label lblTitle = new Label();
            lblTitle.Text = book.Title;
            lblTitle.AutoSize = false;
            lblTitle.Dock = DockStyle.Bottom;
            lblTitle.Height = 44;
            lblTitle.Font = new Font("Arial", 10, FontStyle.Bold);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.ForeColor = book.DaysOverdue > 3 ? Color.DarkRed : Color.Black;
            lblTitle.BackColor = pnl.BackColor;
            pnl.Controls.Add(lblTitle);

            // Create due date label if overdue
            if (book.DaysOverdue > 3)
            {
                Label lblDueDate = new Label();
                lblDueDate.Text = $"Due: {book.ReturnDate:MM/dd/yyyy}\n{book.DaysOverdue} days overdue";
                lblDueDate.AutoSize = false;
                lblDueDate.Dock = DockStyle.Bottom;
                lblDueDate.Height = 36;
                lblDueDate.Font = new Font("Arial", 8, FontStyle.Bold);
                lblDueDate.TextAlign = ContentAlignment.MiddleCenter;
                lblDueDate.ForeColor = Color.DarkRed;
                lblDueDate.BackColor = pnl.BackColor;
                pnl.Controls.Add(lblDueDate);
            }

            // Add image after labels so it docks at the top
            pnl.Controls.Add(pb);

            flowAvailableBooks.Controls.Add(pnl);
        }

        private void CheckAndNotifyStudentOverdueBooks(List<BorrowedBookInfo> borrowedBooks)
        {
            try
            {
                // Only show notification if enough time has passed since last notification
                if ((DateTime.Now - lastStudentNotificationTime).TotalMinutes < StudentNotificationCooldownMinutes)
                {
                    return; // Skip notification if shown recently
                }

                var overdueBooks = new List<string>();
                
                foreach (var book in borrowedBooks)
                {
                    if (book.DaysOverdue > 3) // More than 3 days overdue
                    {
                        overdueBooks.Add($"• {book.Title} (Issue ID: {book.IssueId}) - {book.DaysOverdue} days overdue");
                    }
                }
                
                if (overdueBooks.Count > 0)
                {
                    string message = $"⚠️ OVERDUE BOOK NOTIFICATION ⚠️\n\n";
                    message += $"STUDENT ALERT: You have {overdueBooks.Count} overdue book(s) that are 3+ days past their return date.\n\n";
                    message += "Please return these books as soon as possible to avoid penalties!\n\n";
                    
                    var notificationForm = new OverdueNotificationForm(
                        "Overdue Books Alert - Student",
                        message,
                        overdueBooks
                    );
                    notificationForm.ShowDialog();
                    
                    // Update last notification time
                    lastStudentNotificationTime = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking student overdue books: {ex.Message}");
            }
        }

        private void Pb_Click(object sender, EventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            if (pb != null && pb.Tag is BorrowedBookTag tag)
            {
                // Show book details and return option
                ShowBorrowedBookInfo(tag);
            }
        }

        private void ShowBorrowedBookInfo(BorrowedBookTag tag)
        {
            try
            {
                string message = $"Book Information:\n\n" +
                    $"Title: {tag.Title}\n" +
                    $"Author: {tag.Author}\n" +
                    $"Issue ID: {tag.IssueId}\n";
                
                if (tag.IsOverdue)
                {
                    message += $"\n⚠️ WARNING: This book is {tag.DaysOverdue} days overdue!\n";
                    message += "Please return it as soon as possible.";
                }

                var result = MessageBox.Show(
                    message + "\n\nWould you like to return this book now?",
                    "Borrowed Book Details",
                    MessageBoxButtons.YesNo,
                    tag.IsOverdue ? MessageBoxIcon.Warning : MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                {
                    ProcessBookReturn(tag.IssueId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error displaying book info: " + ex.Message, "Error Message",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ProcessBookReturn(string issueId)
        {
            SqlTransaction transaction = null;
            try
            {
                if (connect.State == ConnectionState.Closed)
                {
                    connect.Open();
                }

                transaction = connect.BeginTransaction();

                // Get book_id from the issue record
                string getBookIdQuery = "SELECT book_id FROM issues WHERE issue_id = @issue_id";
                int bookId = 0;
                
                using (SqlCommand getCmd = new SqlCommand(getBookIdQuery, connect, transaction))
                {
                    getCmd.Parameters.AddWithValue("@issue_id", issueId);
                    object result = getCmd.ExecuteScalar();
                    if (result != null)
                    {
                        bookId = Convert.ToInt32(result);
                    }
                    else
                    {
                        throw new Exception("Issue record not found.");
                    }
                }

                // Call stored procedure to update issue status
                using (SqlCommand cmd = new SqlCommand("sp_ReturnBook", connect, transaction))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@issue_id", issueId);

                    cmd.ExecuteNonQuery();
                }

                // Update actual_return_date and condition if stored procedure doesn't handle it
                string updateReturnDateQuery = @"
                    UPDATE issues 
                    SET actual_return_date = @actualReturnDate,
                        condition = @condition
                    WHERE issue_id = @issue_id AND actual_return_date IS NULL";
                using (SqlCommand updateCmd = new SqlCommand(updateReturnDateQuery, connect, transaction))
                {
                    updateCmd.Parameters.AddWithValue("@issue_id", issueId);
                    updateCmd.Parameters.AddWithValue("@actualReturnDate", DateTime.Today);
                    updateCmd.Parameters.AddWithValue("@condition", "Good"); // Default condition for student returns
                    updateCmd.ExecuteNonQuery();
                }

                // Increment book quantity
                if (bookId > 0)
                {
                    string updateQuantityQuery = "UPDATE books SET quantity = quantity + 1 WHERE id = @book_id";
                    using (SqlCommand updateCmd = new SqlCommand(updateQuantityQuery, connect, transaction))
                    {
                        updateCmd.Parameters.AddWithValue("@book_id", bookId);
                        updateCmd.ExecuteNonQuery();
                    }

                    // Update book status to Available if it was Not Available
                    string updateStatusQuery = "UPDATE books SET status = 'Available' WHERE id = @book_id AND status = 'Not Available'";
                    using (SqlCommand statusCmd = new SqlCommand(updateStatusQuery, connect, transaction))
                    {
                        statusCmd.Parameters.AddWithValue("@book_id", bookId);
                        statusCmd.ExecuteNonQuery();
                    }

                    // Check for reservations when book becomes available
                    ReservationManager.CheckAndNotifyAvailableReservations(bookId);
                }

                transaction.Commit();

                // Log audit action
                AuditLogger.LogBookReturn(issueId, bookId, currentUserId);

                    MessageBox.Show("Book returned successfully!", "Success Message", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Refresh the borrowed books list
                    LoadBorrowedBooksAsync();
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                MessageBox.Show("Error processing book return: " + ex.Message, "Error Message", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (connect.State == ConnectionState.Open)
                {
                    connect.Close();
                }
            }
        }

        public void ShowReturnInfo(string issueId)
        {
            try
            {
                if (connect.State == ConnectionState.Closed)
                {
                    connect.Open();
                }

                string query = @"
                    SELECT 
                        i.issue_id,
                        b.book_title,
                        b.author,
                        i.issue_date,
                        i.return_date,
                        i.actual_return_date,
                        i.status,
                        DATEDIFF(day, i.issue_date, ISNULL(i.actual_return_date, GETDATE())) as days_borrowed,
                        DATEDIFF(day, i.return_date, GETDATE()) as days_overdue
                    FROM issues i
                    INNER JOIN books b ON i.book_id = b.id
                    WHERE i.issue_id = @issueId AND i.user_id = @userId AND i.date_delete IS NULL";

                using (SqlCommand cmd = new SqlCommand(query, connect))
                {
                    cmd.Parameters.AddWithValue("@issueId", issueId);
                    cmd.Parameters.AddWithValue("@userId", currentUserId);
                    
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Display return information
                            ShowReturnDetails(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading return info: " + ex.Message, "Error Message", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (connect.State == ConnectionState.Open)
                {
                    connect.Close();
                }
            }
        }

        private void ShowReturnDetails(SqlDataReader reader)
        {
            try
            {
                string message = $"Return Information:\n\n" +
                    $"Issue ID: {reader["issue_id"]}\n" +
                    $"Book: {reader["book_title"]}\n" +
                    $"Author: {reader["author"]}\n" +
                    $"Issue Date: {Convert.ToDateTime(reader["issue_date"]):yyyy-MM-dd}\n" +
                    $"Due Date: {Convert.ToDateTime(reader["return_date"]):yyyy-MM-dd}\n" +
                    $"Status: {reader["status"]}\n" +
                    $"Days Borrowed: {reader["days_borrowed"]}\n";

                if (Convert.ToInt32(reader["days_overdue"]) > 0)
                {
                    message += $"Days Overdue: {reader["days_overdue"]}\n";
                }

                MessageBox.Show(message, "Return Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error displaying return details: " + ex.Message, "Error Message", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void RefreshBorrowedBooks()
        {
            LoadBorrowedBooksAsync();
        }

        // Helper classes
        private class BorrowedBookInfo
        {
            public string IssueId { get; set; }
            public int BookId { get; set; }
            public string Title { get; set; }
            public string Author { get; set; }
            public string BlobName { get; set; }
            public DateTime IssueDate { get; set; }
            public DateTime ReturnDate { get; set; }
            public string Status { get; set; }
            public int DaysOverdue { get; set; }
        }

        private class BorrowedBookTag
        {
            public string IssueId { get; set; }
            public int BookId { get; set; }
            public string Title { get; set; }
            public string Author { get; set; }
            public string ImagePath { get; set; }
            public int DaysOverdue { get; set; }
            public bool IsOverdue { get; set; }
        }
    }
}
