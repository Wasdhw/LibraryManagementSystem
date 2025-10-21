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
    public partial class StReturnBooks : UserControl
    {
        SqlConnection connect = Database.GetConnection();
        private int currentUserId = 0;

        public StReturnBooks()
        {
            InitializeComponent();
        }

        public void LoadUserBorrowedBooks(int userId = 0)
        {
            currentUserId = userId > 0 ? userId : (SessionManager.CurrentUserId > 0 ? SessionManager.CurrentUserId : 1);
            LoadBorrowedBooks();
        }

        public void refreshData()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)refreshData);
                return;
            }
            LoadBorrowedBooks();
        }

        private void LoadBorrowedBooks()
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
                        i.status,
                        DATEDIFF(day, i.return_date, GETDATE()) as days_overdue
                    FROM issues i
                    INNER JOIN books b ON i.book_id = b.id
                    WHERE i.user_id = @userId 
                    AND i.status = 'Not Return' 
                    AND i.date_delete IS NULL
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
                            
                            // Format the DataGridView
                            FormatDataGridView(dgv);
                        }
                    }
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

        private void FormatDataGridView(DataGridView dgv)
        {
            try
            {
                // Set column headers
                if (dgv.Columns.Count > 0)
                {
                    dgv.Columns["id"].Visible = false; // Hide ID column
                    dgv.Columns["issue_id"].HeaderText = "Issue ID";
                    dgv.Columns["book_title"].HeaderText = "Book Title";
                    dgv.Columns["author"].HeaderText = "Author";
                    dgv.Columns["issue_date"].HeaderText = "Issue Date";
                    dgv.Columns["return_date"].HeaderText = "Due Date";
                    dgv.Columns["status"].HeaderText = "Status";
                    dgv.Columns["days_overdue"].HeaderText = "Days Overdue";
                    
                    // Format date columns
                    if (dgv.Columns["issue_date"] != null)
                        dgv.Columns["issue_date"].DefaultCellStyle.Format = "yyyy-MM-dd";
                    if (dgv.Columns["return_date"] != null)
                        dgv.Columns["return_date"].DefaultCellStyle.Format = "yyyy-MM-dd";
                    
                    // Color code overdue books
                    dgv.CellFormatting += Dgv_CellFormatting;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error formatting DataGridView: " + ex.Message, "Error Message", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Dgv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                DataGridView dgv = sender as DataGridView;
                if (dgv != null && e.RowIndex >= 0)
                {
                    // Check if the book is overdue
                    if (dgv.Columns["days_overdue"] != null && e.ColumnIndex == dgv.Columns["days_overdue"].Index)
                    {
                        if (e.Value != null && int.TryParse(e.Value.ToString(), out int daysOverdue))
                        {
                            if (daysOverdue > 0)
                            {
                                e.CellStyle.BackColor = Color.LightCoral;
                                e.CellStyle.ForeColor = Color.DarkRed;
                            }
                            else
                            {
                                e.CellStyle.BackColor = Color.LightGreen;
                                e.CellStyle.ForeColor = Color.DarkGreen;
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

        public void ReturnSelectedBook()
        {
            try
            {
                if (this.Controls.Find("dataGridView1", true).FirstOrDefault() is DataGridView dgv)
                {
                    if (dgv.CurrentRow == null)
                    {
                        MessageBox.Show("Please select a book to return.", "Error Message", 
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string issueId = dgv.CurrentRow.Cells["issue_id"].Value?.ToString();
                    if (string.IsNullOrEmpty(issueId))
                    {
                        MessageBox.Show("Invalid book selection.", "Error Message", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Confirm return
                    DialogResult result = MessageBox.Show(
                        "Are you sure you want to return this book?", 
                        "Confirm Return", 
                        MessageBoxButtons.YesNo, 
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        ProcessBookReturn(issueId);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error returning book: " + ex.Message, "Error Message", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ProcessBookReturn(string issueId)
        {
            try
            {
                if (connect.State == ConnectionState.Closed)
                {
                    connect.Open();
                }

                using (SqlCommand cmd = new SqlCommand("sp_ReturnBook", connect))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@issue_id", issueId);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Book returned successfully!", "Success Message", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Refresh the borrowed books list
                    LoadBorrowedBooks();
                }
            }
            catch (Exception ex)
            {
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
            LoadBorrowedBooks();
        }
    }
}
