using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LibraryManagementSystem.Utils;

namespace LibraryManagementSystem.studentUser
{
    public partial class StRenewBooks : UserControl
    {
        SqlConnection connect = Database.GetConnection();
        private int currentUserId = 0;
        private const int DefaultRenewalDays = 14;
        private const int MaxRenewals = 2;

        public StRenewBooks()
        {
            InitializeComponent();
            if (currentUserId == 0)
            {
                currentUserId = SessionManager.CurrentUserId > 0 ? SessionManager.CurrentUserId : 1;
            }
            LoadRenewableBooks();
        }

        public void refreshData()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)refreshData);
                return;
            }
            LoadRenewableBooks();
        }

        private void LoadRenewableBooks()
        {
            try
            {
                if (connect.State == ConnectionState.Closed)
                {
                    connect.Open();
                }

                string query = @"
                    SELECT 
                        i.id,
                        i.issue_id,
                        b.book_title,
                        b.author,
                        b.image,
                        i.issue_date,
                        i.return_date,
                        i.renewal_count,
                        DATEDIFF(day, i.return_date, GETDATE()) as days_overdue
                    FROM issues i
                    INNER JOIN books b ON i.book_id = b.id
                    WHERE i.user_id = @userId 
                    AND i.status = 'Not Return' 
                    AND i.date_delete IS NULL
                    ORDER BY i.return_date ASC";

                var renewableBooks = new List<RenewableBookInfo>();

                using (SqlCommand cmd = new SqlCommand(query, connect))
                {
                    cmd.Parameters.AddWithValue("@userId", currentUserId);
                    
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int renewalCount = reader["renewal_count"] != DBNull.Value ? Convert.ToInt32(reader["renewal_count"]) : 0;
                            int daysOverdue = reader["days_overdue"] != DBNull.Value ? Convert.ToInt32(reader["days_overdue"]) : 0;
                            
                            // Only show books that can be renewed (not overdue beyond threshold and under max renewals)
                            if (renewalCount < MaxRenewals && daysOverdue <= 7)
                            {
                                renewableBooks.Add(new RenewableBookInfo
                                {
                                    IssueId = reader["issue_id"].ToString(),
                                    BookId = Convert.ToInt32(reader["id"]),
                                    Title = reader["book_title"].ToString(),
                                    Author = reader["author"].ToString(),
                                    BlobName = reader["image"]?.ToString(),
                                    ReturnDate = Convert.ToDateTime(reader["return_date"]),
                                    RenewalCount = renewalCount,
                                    DaysOverdue = daysOverdue
                                });
                            }
                        }
                    }
                }

                DisplayRenewableBooks(renewableBooks);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading renewable books: " + ex.Message, "Error Message", 
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

        private void DisplayRenewableBooks(List<RenewableBookInfo> books)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => DisplayRenewableBooks(books)));
                return;
            }

            flowRenewableBooks.Controls.Clear();

            foreach (var book in books)
            {
                CreateBookPanel(book);
            }
        }

        private async void CreateBookPanel(RenewableBookInfo book)
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
                System.Diagnostics.Debug.WriteLine($"Failed to load image: {ex.Message}");
            }

            Panel pnl = new Panel();
            pnl.Width = 200;
            pnl.Height = 300;
            pnl.BackColor = SystemColors.Control;
            pnl.Padding = new Padding(5);
            pnl.Margin = new Padding(10);

            PictureBox pb = new PictureBox();
            pb.Size = new Size(145, 176);
            pb.SizeMode = PictureBoxSizeMode.Zoom;
            pb.Image = img;
            pb.Dock = DockStyle.Top;
            pb.Tag = book.IssueId;

            Label lblTitle = new Label();
            lblTitle.Text = book.Title;
            lblTitle.AutoSize = false;
            lblTitle.Dock = DockStyle.Bottom;
            lblTitle.Height = 44;
            lblTitle.Font = new Font("Arial", 10, FontStyle.Bold);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            Label lblDueDate = new Label();
            lblDueDate.Text = $"Due: {book.ReturnDate:MM/dd/yyyy}\nRenewals: {book.RenewalCount}/{MaxRenewals}";
            lblDueDate.AutoSize = false;
            lblDueDate.Dock = DockStyle.Bottom;
            lblDueDate.Height = 40;
            lblDueDate.Font = new Font("Arial", 8);
            lblDueDate.TextAlign = ContentAlignment.MiddleCenter;

            Button btnRenew = new Button();
            btnRenew.Text = "RENEW";
            btnRenew.Dock = DockStyle.Bottom;
            btnRenew.Height = 35;
            btnRenew.BackColor = Color.FromArgb(14, 128, 87);
            btnRenew.ForeColor = Color.White;
            btnRenew.FlatStyle = FlatStyle.Flat;
            btnRenew.FlatAppearance.BorderSize = 0;
            btnRenew.Tag = book.IssueId;
            btnRenew.Click += BtnRenew_Click;

            pnl.Controls.Add(btnRenew);
            pnl.Controls.Add(lblDueDate);
            pnl.Controls.Add(lblTitle);
            pnl.Controls.Add(pb);

            flowRenewableBooks.Controls.Add(pnl);
        }

        private void BtnRenew_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag != null)
            {
                string issueId = btn.Tag.ToString();
                RenewBook(issueId);
            }
        }

        private void RenewBook(string issueId)
        {
            try
            {
                if (connect.State == ConnectionState.Closed)
                {
                    connect.Open();
                }

                // Check current renewal count
                string checkQuery = "SELECT renewal_count, return_date FROM issues WHERE issue_id = @issue_id";
                int currentRenewalCount = 0;
                DateTime currentReturnDate = DateTime.Today;

                using (SqlCommand checkCmd = new SqlCommand(checkQuery, connect))
                {
                    checkCmd.Parameters.AddWithValue("@issue_id", issueId);
                    using (SqlDataReader reader = checkCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            currentRenewalCount = reader["renewal_count"] != DBNull.Value ? Convert.ToInt32(reader["renewal_count"]) : 0;
                            currentReturnDate = Convert.ToDateTime(reader["return_date"]);
                        }
                        else
                        {
                            MessageBox.Show("Issue record not found.", "Error Message", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }

                if (currentRenewalCount >= MaxRenewals)
                {
                    MessageBox.Show($"Maximum renewals ({MaxRenewals}) reached for this book.", "Error Message", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Calculate new return date
                DateTime newReturnDate = currentReturnDate.AddDays(DefaultRenewalDays);

                // Update issue record
                string updateQuery = @"
                    UPDATE issues 
                    SET return_date = @newReturnDate, 
                        renewal_count = renewal_count + 1,
                        date_update = @dateUpdate
                    WHERE issue_id = @issue_id";

                using (SqlCommand cmd = new SqlCommand(updateQuery, connect))
                {
                    cmd.Parameters.AddWithValue("@newReturnDate", newReturnDate);
                    cmd.Parameters.AddWithValue("@issue_id", issueId);
                    cmd.Parameters.AddWithValue("@dateUpdate", DateTime.Today);

                    cmd.ExecuteNonQuery();
                }

                // Get book_id for audit log
                string getBookIdQuery = "SELECT book_id FROM issues WHERE issue_id = @issue_id";
                int bookId = 0;
                using (SqlCommand getBookCmd = new SqlCommand(getBookIdQuery, connect))
                {
                    getBookCmd.Parameters.AddWithValue("@issue_id", issueId);
                    object result = getBookCmd.ExecuteScalar();
                    if (result != null)
                    {
                        bookId = Convert.ToInt32(result);
                    }
                }

                // Log audit action
                int userId = SessionManager.CurrentUserId > 0 ? SessionManager.CurrentUserId : 1;
                AuditLogger.LogBookRenewal(issueId, bookId, userId);

                MessageBox.Show($"Book renewed successfully! New return date: {newReturnDate:MM/dd/yyyy}", 
                    "Success Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadRenewableBooks();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error renewing book: " + ex.Message, "Error Message", 
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

        private class RenewableBookInfo
        {
            public string IssueId { get; set; }
            public int BookId { get; set; }
            public string Title { get; set; }
            public string Author { get; set; }
            public string BlobName { get; set; }
            public DateTime ReturnDate { get; set; }
            public int RenewalCount { get; set; }
            public int DaysOverdue { get; set; }
        }
    }
}

