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
    public partial class History : UserControl
    {
        SqlConnection connect = Database.GetConnection();
        private int currentUserId = 0;

        public History()
        {
            InitializeComponent();
        }

        public void LoadUserHistory(int userId = 0)
        {
            currentUserId = userId > 0 ? userId : (SessionManager.CurrentUserId > 0 ? SessionManager.CurrentUserId : 1);
            LoadBorrowingHistory();
        }

        public void refreshData()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)refreshData);
                return;
            }
            LoadBorrowingHistory();
        }

        private void LoadBorrowingHistory()
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
                        i.issue_date,
                        i.return_date,
                        i.actual_return_date,
                        i.status,
                        DATEDIFF(day, i.issue_date, ISNULL(i.actual_return_date, GETDATE())) as days_borrowed,
                        CASE 
                            WHEN i.status = 'Return' AND i.actual_return_date > i.return_date 
                            THEN DATEDIFF(day, i.return_date, i.actual_return_date)
                            WHEN i.status = 'Not Return' AND i.return_date < GETDATE() 
                            THEN DATEDIFF(day, i.return_date, GETDATE())
                            ELSE 0 
                        END as days_overdue
                    FROM issues i
                    INNER JOIN books b ON i.book_id = b.id
                    WHERE i.user_id = @userId AND i.date_delete IS NULL
                    ORDER BY i.issue_date DESC";

                using (SqlCommand cmd = new SqlCommand(query, connect))
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
                            FormatHistoryDataGridView(dgv);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading borrowing history: " + ex.Message, "Error Message", 
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

        private void FormatHistoryDataGridView(DataGridView dgv)
        {
            try
            {
                if (dgv.Columns.Count > 0)
                {
                    // Hide ID column
                    if (dgv.Columns["id"] != null)
                        dgv.Columns["id"].Visible = false;
                    
                    // Set column headers
                    if (dgv.Columns["issue_id"] != null)
                        dgv.Columns["issue_id"].HeaderText = "Issue ID";
                    if (dgv.Columns["book_title"] != null)
                        dgv.Columns["book_title"].HeaderText = "Book Title";
                    if (dgv.Columns["author"] != null)
                        dgv.Columns["author"].HeaderText = "Author";
                    if (dgv.Columns["issue_date"] != null)
                        dgv.Columns["issue_date"].HeaderText = "Issue Date";
                    if (dgv.Columns["return_date"] != null)
                        dgv.Columns["return_date"].HeaderText = "Due Date";
                    if (dgv.Columns["actual_return_date"] != null)
                        dgv.Columns["actual_return_date"].HeaderText = "Return Date";
                    if (dgv.Columns["status"] != null)
                        dgv.Columns["status"].HeaderText = "Status";
                    if (dgv.Columns["days_borrowed"] != null)
                        dgv.Columns["days_borrowed"].HeaderText = "Days Borrowed";
                    if (dgv.Columns["days_overdue"] != null)
                        dgv.Columns["days_overdue"].HeaderText = "Days Overdue";
                    
                    // Format date columns
                    if (dgv.Columns["issue_date"] != null)
                        dgv.Columns["issue_date"].DefaultCellStyle.Format = "yyyy-MM-dd";
                    if (dgv.Columns["return_date"] != null)
                        dgv.Columns["return_date"].DefaultCellStyle.Format = "yyyy-MM-dd";
                    if (dgv.Columns["actual_return_date"] != null)
                        dgv.Columns["actual_return_date"].DefaultCellStyle.Format = "yyyy-MM-dd";
                    
                    // Add color coding for overdue books
                    dgv.CellFormatting += HistoryDgv_CellFormatting;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error formatting history DataGridView: " + ex.Message, "Error Message", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HistoryDgv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                DataGridView dgv = sender as DataGridView;
                if (dgv != null && e.RowIndex >= 0)
                {
                    // Color code based on status and overdue days
                    if (dgv.Columns["status"] != null && e.ColumnIndex == dgv.Columns["status"].Index)
                    {
                        string status = e.Value?.ToString();
                        if (status == "Return")
                        {
                            e.CellStyle.BackColor = Color.LightGreen;
                            e.CellStyle.ForeColor = Color.DarkGreen;
                        }
                        else if (status == "Not Return")
                        {
                            // Check if overdue
                            if (dgv.Columns["days_overdue"] != null)
                            {
                                var overdueCell = dgv.Rows[e.RowIndex].Cells["days_overdue"];
                                if (overdueCell.Value != null && int.TryParse(overdueCell.Value.ToString(), out int daysOverdue))
                                {
                                    if (daysOverdue > 0)
                                    {
                                        e.CellStyle.BackColor = Color.LightCoral;
                                        e.CellStyle.ForeColor = Color.DarkRed;
                                    }
                                    else
                                    {
                                        e.CellStyle.BackColor = Color.LightYellow;
                                        e.CellStyle.ForeColor = Color.DarkOrange;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Ignore formatting errors
            }
        }

        public void SearchHistory(string searchTerm)
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
                        i.issue_date,
                        i.return_date,
                        i.actual_return_date,
                        i.status,
                        DATEDIFF(day, i.issue_date, ISNULL(i.actual_return_date, GETDATE())) as days_borrowed,
                        CASE 
                            WHEN i.status = 'Return' AND i.actual_return_date > i.return_date 
                            THEN DATEDIFF(day, i.return_date, i.actual_return_date)
                            WHEN i.status = 'Not Return' AND i.return_date < GETDATE() 
                            THEN DATEDIFF(day, i.return_date, GETDATE())
                            ELSE 0 
                        END as days_overdue
                    FROM issues i
                    INNER JOIN books b ON i.book_id = b.id
                    WHERE i.user_id = @userId 
                    AND i.date_delete IS NULL
                    AND (b.book_title LIKE @search OR b.author LIKE @search OR i.issue_id LIKE @search)
                    ORDER BY i.issue_date DESC";

                using (SqlCommand cmd = new SqlCommand(query, connect))
                {
                    cmd.Parameters.AddWithValue("@userId", currentUserId);
                    cmd.Parameters.AddWithValue("@search", "%" + searchTerm + "%");
                    
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable table = new DataTable();
                        adapter.Fill(table);
                        
                        if (this.Controls.Find("dataGridView1", true).FirstOrDefault() is DataGridView dgv)
                        {
                            dgv.DataSource = table;
                            FormatHistoryDataGridView(dgv);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching history: " + ex.Message, "Error Message", 
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

        public void FilterByStatus(string status)
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
                        i.issue_date,
                        i.return_date,
                        i.actual_return_date,
                        i.status,
                        DATEDIFF(day, i.issue_date, ISNULL(i.actual_return_date, GETDATE())) as days_borrowed,
                        CASE 
                            WHEN i.status = 'Return' AND i.actual_return_date > i.return_date 
                            THEN DATEDIFF(day, i.return_date, i.actual_return_date)
                            WHEN i.status = 'Not Return' AND i.return_date < GETDATE() 
                            THEN DATEDIFF(day, i.return_date, GETDATE())
                            ELSE 0 
                        END as days_overdue
                    FROM issues i
                    INNER JOIN books b ON i.book_id = b.id
                    WHERE i.user_id = @userId 
                    AND i.status = @status
                    AND i.date_delete IS NULL
                    ORDER BY i.issue_date DESC";

                using (SqlCommand cmd = new SqlCommand(query, connect))
                {
                    cmd.Parameters.AddWithValue("@userId", currentUserId);
                    cmd.Parameters.AddWithValue("@status", status);
                    
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable table = new DataTable();
                        adapter.Fill(table);
                        
                        if (this.Controls.Find("dataGridView1", true).FirstOrDefault() is DataGridView dgv)
                        {
                            dgv.DataSource = table;
                            FormatHistoryDataGridView(dgv);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error filtering history: " + ex.Message, "Error Message", 
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

        public void GetBorrowingStatistics()
        {
            try
            {
                if (connect.State == ConnectionState.Closed)
                {
                    connect.Open();
                }

                // Get total books borrowed
                string totalQuery = "SELECT COUNT(*) FROM issues WHERE user_id = @userId AND date_delete IS NULL";
                int totalBorrowed = 0;
                using (SqlCommand cmd = new SqlCommand(totalQuery, connect))
                {
                    cmd.Parameters.AddWithValue("@userId", currentUserId);
                    totalBorrowed = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Get currently borrowed books
                string currentQuery = "SELECT COUNT(*) FROM issues WHERE user_id = @userId AND status = 'Not Return' AND date_delete IS NULL";
                int currentBorrowed = 0;
                using (SqlCommand cmd = new SqlCommand(currentQuery, connect))
                {
                    cmd.Parameters.AddWithValue("@userId", currentUserId);
                    currentBorrowed = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Get overdue books
                string overdueQuery = "SELECT COUNT(*) FROM issues WHERE user_id = @userId AND status = 'Not Return' AND return_date < GETDATE() AND date_delete IS NULL";
                int overdueBooks = 0;
                using (SqlCommand cmd = new SqlCommand(overdueQuery, connect))
                {
                    cmd.Parameters.AddWithValue("@userId", currentUserId);
                    overdueBooks = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Update statistics labels if they exist
                if (this.Controls.Find("lblTotalBorrowed", true).FirstOrDefault() is Label lblTotal)
                    lblTotal.Text = totalBorrowed.ToString();
                if (this.Controls.Find("lblCurrentBorrowed", true).FirstOrDefault() is Label lblCurrent)
                    lblCurrent.Text = currentBorrowed.ToString();
                if (this.Controls.Find("lblOverdueBooks", true).FirstOrDefault() is Label lblOverdue)
                    lblOverdue.Text = overdueBooks.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading statistics: " + ex.Message, "Error Message", 
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Handle search text change
            if (sender is TextBox textBox)
            {
                if (!string.IsNullOrEmpty(textBox.Text))
                {
                    SearchHistory(textBox.Text);
                }
                else
                {
                    LoadBorrowingHistory();
                }
            }
        }

        private void History_Load(object sender, EventArgs e)
        {
            if (currentUserId > 0)
            {
                LoadBorrowingHistory();
                GetBorrowingStatistics();
            }
        }
    }
}
