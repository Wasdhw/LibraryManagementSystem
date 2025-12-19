using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using LibraryManagementSystem.Utils;

namespace LibraryManagementSystem
{
    public partial class ReturnBooks : UserControl
    {
        SqlConnection connect = Database.GetConnection();
        private DateTime lastNotificationTime = DateTime.MinValue;
        private const int NotificationCooldownMinutes = 5; // Only show notification once every 5 minutes

        public ReturnBooks()
        {
            InitializeComponent();

            displayIssuedBooksData();
            
            // Set up cell formatting for overdue highlighting
            dataGridView1.CellFormatting += DataGridView1_CellFormatting;

            // Initialize condition ComboBox
            if (conditionComboBox != null)
            {
                conditionComboBox.SelectedIndex = 1; // Default to "Good"
            }
        }

        public void refreshData()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)refreshData);
                return;
            }

            displayIssuedBooksData();
        }

        private void returnBooks_returnBtn_Click(object sender, EventArgs e)
        {
            if(returnBooks_issueID.Text == ""
                || returnBooks_name.Text == ""
                || returnBooks_bookTitle.Text == ""
                || returnBooks_author.Text == ""
                || bookIssue_issueDate.Value == null){
                MessageBox.Show("Please select item first", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if(connect.State == ConnectionState.Closed)
                {
                    DialogResult check = MessageBox.Show("Are you sure that Issue ID: "
                        + returnBooks_issueID.Text.Trim()
                        + "is return already?", "Confirmation Message", MessageBoxButtons.YesNo
                        , MessageBoxIcon.Question);

                    if(check == DialogResult.Yes)
                    {
                        SqlTransaction transaction = null;
                        try
                        {
                            connect.Open();
                            transaction = connect.BeginTransaction();

                            string issueId = returnBooks_issueID.Text.Trim();

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

                            // Get user_id for fine calculation
                            string getUserIdQuery = "SELECT user_id FROM issues WHERE issue_id = @issue_id";
                            int userId = 0;
                            using (SqlCommand getUserIdCmd = new SqlCommand(getUserIdQuery, connect, transaction))
                            {
                                getUserIdCmd.Parameters.AddWithValue("@issue_id", issueId);
                                object userIdResult = getUserIdCmd.ExecuteScalar();
                                if (userIdResult != null)
                                {
                                    userId = Convert.ToInt32(userIdResult);
                                }
                            }

                            // Calculate fine before updating return status
                            var (fineAmount, daysOverdue) = FineCalculator.CalculateFineForIssue(issueId);

                            // Get condition from UI if available, default to "Good"
                            string condition = "Good"; // Default condition
                            if (conditionComboBox != null && conditionComboBox.SelectedItem != null)
                            {
                                condition = conditionComboBox.SelectedItem.ToString();
                            }

                            // Update the issue status to 'Return' and set actual_return_date (not return_date)
                            string updateQuery = @"
                                UPDATE issues 
                                SET status = 'Return', 
                                    actual_return_date = @actualReturnDate,
                                    condition = @condition
                                WHERE issue_id = @issue_id";
                            
                            using (SqlCommand cmd = new SqlCommand(updateQuery, connect, transaction))
                            {
                                cmd.Parameters.AddWithValue("@issue_id", issueId);
                                cmd.Parameters.AddWithValue("@actualReturnDate", DateTime.Today);
                                cmd.Parameters.AddWithValue("@condition", condition);

                                cmd.ExecuteNonQuery();
                            }

                            // Create fine record if overdue
                            if (fineAmount > 0 && userId > 0)
                            {
                                FineCalculator.CreateFineRecord(issueId, userId, fineAmount, daysOverdue);
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
                            AuditLogger.LogBookReturn(issueId, bookId, userId);

                                displayIssuedBooksData();
                                MessageBox.Show("Returned successfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                clearFields();
                        }
                        catch (Exception ex)
                        {
                            if (transaction != null)
                            {
                                transaction.Rollback();
                            }
                            MessageBox.Show("Error: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {
                            if (connect.State == ConnectionState.Open)
                        {
                            connect.Close();
                            }
                        }
                    }
                    
                }
            }
        }

        public void displayIssuedBooksData()
        {
            DataIssueBooks dib = new DataIssueBooks();
            List<DataIssueBooks> listData = dib.ReturnIssueBooksData();

            dataGridView1.DataSource = listData;
            
            // Format the DataGridView columns after binding
            FormatDataGridView();
            
            // Check for overdue books and show notifications
            CheckAndNotifyOverdueBooks(listData);
        }

        private void FormatDataGridView()
        {
            try
            {
                if (dataGridView1.Columns.Count > 0)
                {
                    // Set column headers
                    if (dataGridView1.Columns["ID"] != null)
                        dataGridView1.Columns["ID"].Visible = false; // Hide ID column
                    
                    if (dataGridView1.Columns["IssueID"] != null)
                        dataGridView1.Columns["IssueID"].HeaderText = "Issue ID";
                    
                    if (dataGridView1.Columns["Name"] != null)
                        dataGridView1.Columns["Name"].HeaderText = "Name";
                    
                    // Change Contact column header to Student ID
                    if (dataGridView1.Columns["Contact"] != null)
                        dataGridView1.Columns["Contact"].HeaderText = "Student ID";
                    
                    if (dataGridView1.Columns["BookTitle"] != null)
                        dataGridView1.Columns["BookTitle"].HeaderText = "Book Title";
                    
                    if (dataGridView1.Columns["Author"] != null)
                        dataGridView1.Columns["Author"].HeaderText = "Author";
                    
                    if (dataGridView1.Columns["DateIssue"] != null)
                        dataGridView1.Columns["DateIssue"].HeaderText = "Issue Date";
                    
                    if (dataGridView1.Columns["DateReturn"] != null)
                        dataGridView1.Columns["DateReturn"].HeaderText = "Return Date";
                    
                    // Add Overdue Days column
                    if (dataGridView1.Columns["OverdueDays"] != null)
                    {
                        dataGridView1.Columns["OverdueDays"].HeaderText = "Overdue Days";
                        dataGridView1.Columns["OverdueDays"].DisplayIndex = dataGridView1.Columns.Count - 1; // Move to end
                    }
                    
                    if (dataGridView1.Columns["Status"] != null)
                        dataGridView1.Columns["Status"].HeaderText = "Status";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error formatting DataGridView: {ex.Message}");
            }
        }

        private void DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && dataGridView1.Rows[e.RowIndex].DataBoundItem != null)
                {
                    DataIssueBooks item = dataGridView1.Rows[e.RowIndex].DataBoundItem as DataIssueBooks;
                    
                    if (item != null && item.IsOverdue)
                    {
                        // Highlight entire row in red for overdue books
                        e.CellStyle.BackColor = Color.LightCoral;
                        e.CellStyle.ForeColor = Color.DarkRed;
                        e.CellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
                    }
                }
            }
            catch
            {
                // Ignore errors in formatting
            }
        }

        private void CheckAndNotifyOverdueBooks(List<DataIssueBooks> listData)
        {
            try
            {
                // Only show notification if enough time has passed since last notification
                if ((DateTime.Now - lastNotificationTime).TotalMinutes < NotificationCooldownMinutes)
                {
                    return; // Skip notification if shown recently
                }

                var overdueBooks = listData.Where(b => b.IsOverdue).ToList();
                
                if (overdueBooks.Count > 0)
                {
                    string message = $"⚠️ OVERDUE BOOKS NOTIFICATION ⚠️\n\n";
                    message += $"ADMIN ALERT: You have {overdueBooks.Count} overdue book(s) that are 3+ days past their return date:\n\n";
                    
                    List<string> overdueList = new List<string>();
                    foreach (var book in overdueBooks)
                    {
                        string bookInfo = $"• {book.BookTitle} - Student: {book.Name} (ID: {book.Contact}) - {book.OverdueDays} days overdue";
                        overdueList.Add(bookInfo);
                        message += bookInfo + "\n";
                    }
                    
                    message += "\nPlease contact these students to return the books as soon as possible!";
                    
                    // Show notification pop-up for admin
                    var notificationForm = new OverdueNotificationForm(
                        "Overdue Books Alert - Admin",
                        message,
                        overdueList
                    );
                    notificationForm.ShowDialog();
                    
                    // Update last notification time
                    lastNotificationTime = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking overdue books: {ex.Message}");
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                returnBooks_issueID.Text = row.Cells[1].Value?.ToString() ?? "";
                returnBooks_name.Text = row.Cells[2].Value?.ToString() ?? "";
                // Note: Contact field (Cells[3]) is not displayed in this form
                returnBooks_bookTitle.Text = row.Cells[4].Value?.ToString() ?? "";
                returnBooks_author.Text = row.Cells[5].Value?.ToString() ?? "";
                
                // Handle date parsing
                try
                {
                    if (row.Cells[6].Value != null && DateTime.TryParse(row.Cells[6].Value.ToString(), out DateTime issueDate))
                    {
                        bookIssue_issueDate.Value = issueDate;
                    }
                    else
                    {
                        bookIssue_issueDate.Value = DateTime.Today;
                    }
                }
                catch
                {
                    bookIssue_issueDate.Value = DateTime.Today;
                }
            }
        }

        public void clearFields()
        {
            returnBooks_issueID.Text = "";
            returnBooks_name.Text = "";
            returnBooks_bookTitle.Text = "";
            returnBooks_author.Text = "";
            bookIssue_issueDate.Value = DateTime.Today;
            if (conditionComboBox != null)
            {
                conditionComboBox.SelectedIndex = 1; // Reset to "Good"
            }
        }

        private void returnBooks_clearBtn_Click(object sender, EventArgs e)
        {
            clearFields();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void returnBooks_issueID_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void ReturnBooks_Load(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
