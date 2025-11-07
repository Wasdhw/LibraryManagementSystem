using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LibraryManagementSystem.Utils;

namespace LibraryManagementSystem.MainformsUser
{
    public partial class StDashboard : UserControl
    {
        SqlConnection connect = Database.GetConnection();
        
        public StDashboard()
        {
            InitializeComponent();
            LoadStudentDashboard();
        }

        public void refreshData()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)refreshData);
                return;
            }
            LoadStudentDashboard();
        }

        private void LoadStudentDashboard()
        {
            try
            {
                if (connect.State == ConnectionState.Closed)
                {
                    connect.Open();
                }

                // Get student's borrowing statistics
                LoadBorrowingStats();
                LoadRecentActivity();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading dashboard: " + ex.Message, "Error Message", 
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

        private void LoadBorrowingStats()
        {
            try
            {
                int currentUserId = GetCurrentUserId();
                
                if (currentUserId == 0) return;

                // Available Books (Total available in library)
                string availableBooksQuery = @"
                    SELECT COUNT(*) 
                    FROM books 
                    WHERE status = 'Available' AND date_delete IS NULL";

                using (SqlCommand cmd = new SqlCommand(availableBooksQuery, connect))
                {
                    int availableBooks = Convert.ToInt32(cmd.ExecuteScalar());
                    
                    if (this.Controls.Find("dashboard_AB", true).FirstOrDefault() is Label lblAB)
                        lblAB.Text = availableBooks.ToString();
                }

                // Issued Books (Currently borrowed by student)
                string issuedBooksQuery = @"
                    SELECT COUNT(*) 
                    FROM issues 
                    WHERE user_id = @userId AND status = 'Not Return' AND date_delete IS NULL";

                using (SqlCommand cmd = new SqlCommand(issuedBooksQuery, connect))
                {
                    cmd.Parameters.AddWithValue("@userId", currentUserId);
                    int issuedBooks = Convert.ToInt32(cmd.ExecuteScalar());
                    
                    if (this.Controls.Find("dashboard_IB", true).FirstOrDefault() is Label lblIB)
                        lblIB.Text = issuedBooks.ToString();
                }

                // Returned Books (Total returned by student)
                string returnedBooksQuery = @"
                    SELECT COUNT(*) 
                    FROM issues 
                    WHERE user_id = @userId AND status = 'Return' AND date_delete IS NULL";

                using (SqlCommand cmd = new SqlCommand(returnedBooksQuery, connect))
                {
                    cmd.Parameters.AddWithValue("@userId", currentUserId);
                    int returnedBooks = Convert.ToInt32(cmd.ExecuteScalar());
                    
                    if (this.Controls.Find("dashboard_RB", true).FirstOrDefault() is Label lblRB)
                        lblRB.Text = returnedBooks.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading borrowing stats: " + ex.Message, "Error Message", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadRecentActivity()
        {
            // Removed - matching admin dashboard structure which doesn't have recent activity table
        }

        private int GetCurrentUserId()
        {
            return SessionManager.CurrentUserId > 0 ? SessionManager.CurrentUserId : 1; // Default to user ID 1 if not set
        }

        private void StDashboard_Load(object sender, EventArgs e)
        {
            LoadStudentDashboard();
        }

        // Password change feature removed
    }
}
