using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibraryManagementSystem.Utils;

namespace LibraryManagementSystem.studentUser
{
    public partial class StBookInfo : UserControl
    {
        SqlConnection connect = Database.GetConnection();
        private int currentBookId = 0;
        private int currentUserId = 0;

        public StBookInfo()
        {
            InitializeComponent();
        }

        public void LoadBookInfo(int bookId, int userId = 0)
        {
            currentBookId = bookId;
            currentUserId = userId > 0 ? userId : (SessionManager.CurrentUserId > 0 ? SessionManager.CurrentUserId : 1);
            LoadBookDetails();
            CheckBorrowingStatus();
        }

        private void LoadBookDetails()
        {
            try
            {
                if (connect.State == ConnectionState.Closed)
                {
                    connect.Open();
                }

                string query = @"
                    SELECT 
                        id, book_title, author, published_date, 
                        quantity, status, image, date_insert
                    FROM books 
                    WHERE id = @bookId AND date_delete IS NULL";

                using (SqlCommand cmd = new SqlCommand(query, connect))
                {
                    cmd.Parameters.AddWithValue("@bookId", currentBookId);
                    
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Update UI controls with book information
                            UpdateBookInfoUI(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading book details: " + ex.Message, "Error Message", 
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

        private void UpdateBookInfoUI(SqlDataReader reader)
        {
            try
            {
                // Update book title
                if (this.Controls.Find("Title", true).FirstOrDefault() is Label lblTitle)
                    lblTitle.Text = reader["book_title"].ToString();

                // Update author
                if (this.Controls.Find("Author", true).FirstOrDefault() is Label lblAuthor)
                    lblAuthor.Text = reader["author"].ToString();

                // Update published date
                if (this.Controls.Find("Published", true).FirstOrDefault() is Label lblDate)
                    lblDate.Text = Convert.ToDateTime(reader["published_date"]).ToString("yyyy-MM-dd");

                // Update quantity
                if (this.Controls.Find("Quantity", true).FirstOrDefault() is Label lblQuantity)
                    lblQuantity.Text = reader["quantity"].ToString();

                // Update status
                if (this.Controls.Find("Availability", true).FirstOrDefault() is Label lblStatus)
                    lblStatus.Text = reader["status"].ToString();

                // Update book image
                string imagePath = reader["image"].ToString();
                if (!string.IsNullOrEmpty(imagePath) && System.IO.File.Exists(imagePath))
                {
                    if (this.Controls.Find("pictureBox1", true).FirstOrDefault() is PictureBox pb)
                    {
                        pb.Image = Image.FromFile(imagePath);
                        pb.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                }

                // Update availability status
                UpdateAvailabilityStatus(Convert.ToInt32(reader["quantity"]), reader["status"].ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating UI: " + ex.Message, "Error Message", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateAvailabilityStatus(int quantity, string status)
        {
            try
            {
                bool isAvailable = quantity > 0 && status == "Available";
                
                if (this.Controls.Find("Availability", true).FirstOrDefault() is Label lblAvailability)
                {
                    lblAvailability.Text = isAvailable ? "Available" : "Not Available";
                    lblAvailability.ForeColor = isAvailable ? Color.Green : Color.Red;
                }

                // Enable/disable borrow button based on availability
                if (this.Controls.Find("bookIssue_addBtn", true).FirstOrDefault() is Button btnBorrow)
                {
                    btnBorrow.Enabled = isAvailable;
                    btnBorrow.Text = isAvailable ? "Borrow Book" : "Not Available";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating availability: " + ex.Message, "Error Message", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CheckBorrowingStatus()
        {
            try
            {
                if (connect.State == ConnectionState.Closed)
                {
                    connect.Open();
                }

                // Check if user has already borrowed this book
                string checkQuery = @"
                    SELECT COUNT(*) 
                    FROM issues 
                    WHERE user_id = @userId AND book_id = @bookId 
                    AND status = 'Not Return' AND date_delete IS NULL";

                using (SqlCommand cmd = new SqlCommand(checkQuery, connect))
                {
                    cmd.Parameters.AddWithValue("@userId", currentUserId);
                    cmd.Parameters.AddWithValue("@bookId", currentBookId);
                    
                    int alreadyBorrowed = Convert.ToInt32(cmd.ExecuteScalar());
                    
                    if (alreadyBorrowed > 0)
                    {
                        // User has already borrowed this book
                        if (this.Controls.Find("bookIssue_addBtn", true).FirstOrDefault() is Button btnBorrow)
                        {
                            btnBorrow.Enabled = false;
                            btnBorrow.Text = "Already Borrowed";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking borrowing status: " + ex.Message, "Error Message", 
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

        public void BorrowBook()
        {
            try
            {
                if (currentBookId == 0 || currentUserId == 0)
                {
                    MessageBox.Show("Invalid book or user information.", "Error Message", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Check if user can borrow more books (implement borrowing limits if needed)
                if (!CanUserBorrowBook())
                {
                    MessageBox.Show("You have reached the maximum borrowing limit.", "Error Message", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (connect.State == ConnectionState.Closed)
                {
                    connect.Open();
                }

                // Get user details for the issue record
                string userQuery = "SELECT name, idcode FROM users WHERE id = @userId";
                string userName = "";
                string userContact = "";

                using (SqlCommand cmd = new SqlCommand(userQuery, connect))
                {
                    cmd.Parameters.AddWithValue("@userId", currentUserId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            userName = reader["name"].ToString();
                            userContact = reader["idcode"].ToString();
                        }
                    }
                }

                // Create issue record using stored procedure
                using (SqlCommand cmd = new SqlCommand("sp_IssueBook", connect))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@user_id", currentUserId);
                    cmd.Parameters.AddWithValue("@book_id", currentBookId);
                    cmd.Parameters.AddWithValue("@full_name", userName);
                    cmd.Parameters.AddWithValue("@contact", userContact);
                    cmd.Parameters.AddWithValue("@issue_date", DateTime.Today);
                    cmd.Parameters.AddWithValue("@return_date", DateTime.Today.AddDays(14)); // 14 days borrowing period

                    SqlParameter outIssue = new SqlParameter("@issue_id", SqlDbType.NVarChar, 50)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outIssue);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Book borrowed successfully! Issue ID: " + outIssue.Value.ToString(), 
                        "Success Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Refresh book info
                    LoadBookDetails();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error borrowing book: " + ex.Message, "Error Message", 
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

        private bool CanUserBorrowBook()
        {
            try
            {
                // Check current borrowing count for user
                string countQuery = @"
                    SELECT COUNT(*) 
                    FROM issues 
                    WHERE user_id = @userId AND status = 'Not Return' AND date_delete IS NULL";

                using (SqlCommand cmd = new SqlCommand(countQuery, connect))
                {
                    cmd.Parameters.AddWithValue("@userId", currentUserId);
                    int currentBorrowed = Convert.ToInt32(cmd.ExecuteScalar());
                    
                    // Maximum 5 books per user (you can adjust this limit)
                    return currentBorrowed < 5;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking borrowing limit: " + ex.Message, "Error Message", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            // Handle book image click if needed
        }

        private void bookIssue_addBtn_Click(object sender, EventArgs e)
        {
            BorrowBook();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Handle additional button click if needed
        }
    }
}
