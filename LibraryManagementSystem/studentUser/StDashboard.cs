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
                // Get current user ID (you'll need to pass this from the login)
                // For now, we'll use a placeholder - in real implementation, get from session
                int currentUserId = GetCurrentUserId();
                
                if (currentUserId == 0) return;

                // Total books borrowed
                string totalBorrowedQuery = @"
                    SELECT COUNT(*) 
                    FROM issues 
                    WHERE user_id = @userId AND date_delete IS NULL";

                using (SqlCommand cmd = new SqlCommand(totalBorrowedQuery, connect))
                {
                    cmd.Parameters.AddWithValue("@userId", currentUserId);
                    int totalBorrowed = Convert.ToInt32(cmd.ExecuteScalar());
                    
                    // Update UI - using actual control names
                    if (this.Controls.Find("dashboard_AB", true).FirstOrDefault() is Label lblTotal)
                        lblTotal.Text = totalBorrowed.ToString();
                }

                // Currently borrowed books
                string currentBorrowedQuery = @"
                    SELECT COUNT(*) 
                    FROM issues 
                    WHERE user_id = @userId AND status = 'Not Return' AND date_delete IS NULL";

                using (SqlCommand cmd = new SqlCommand(currentBorrowedQuery, connect))
                {
                    cmd.Parameters.AddWithValue("@userId", currentUserId);
                    int currentBorrowed = Convert.ToInt32(cmd.ExecuteScalar());
                    
                    if (this.Controls.Find("dashboard_IB", true).FirstOrDefault() is Label lblCurrent)
                        lblCurrent.Text = currentBorrowed.ToString();
                }

                // Overdue books
                string overdueQuery = @"
                    SELECT COUNT(*) 
                    FROM issues 
                    WHERE user_id = @userId AND status = 'Not Return' 
                    AND return_date < GETDATE() AND date_delete IS NULL";

                using (SqlCommand cmd = new SqlCommand(overdueQuery, connect))
                {
                    cmd.Parameters.AddWithValue("@userId", currentUserId);
                    int overdue = Convert.ToInt32(cmd.ExecuteScalar());
                    
                    if (this.Controls.Find("dashboard_RB", true).FirstOrDefault() is Label lblOverdue)
                        lblOverdue.Text = overdue.ToString();
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
            try
            {
                int currentUserId = GetCurrentUserId();
                if (currentUserId == 0) return;

                string recentActivityQuery = @"
                    SELECT TOP 5 
                        i.issue_id,
                        b.book_title,
                        b.author,
                        i.issue_date,
                        i.return_date,
                        i.status
                    FROM issues i
                    INNER JOIN books b ON i.book_id = b.id
                    WHERE i.user_id = @userId AND i.date_delete IS NULL
                    ORDER BY i.date_insert DESC";

                using (SqlCommand cmd = new SqlCommand(recentActivityQuery, connect))
                {
                    cmd.Parameters.AddWithValue("@userId", currentUserId);
                    
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable table = new DataTable();
                        adapter.Fill(table);
                        
                        // Update DataGridView if it exists
                        if (this.Controls.Find("dataGridView1", true).FirstOrDefault() is DataGridView dgv)
                        {
                            dgv.DataSource = table;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading recent activity: " + ex.Message, "Error Message", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int GetCurrentUserId()
        {
            return SessionManager.CurrentUserId > 0 ? SessionManager.CurrentUserId : 1; // Default to user ID 1 if not set
        }

        private void StDashboard_Load(object sender, EventArgs e)
        {
            LoadStudentDashboard();
        }
    }
}
