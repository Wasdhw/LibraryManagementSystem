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
    public partial class IssueBooks : UserControl
    {
        SqlConnection connect = Database.GetConnection();
        private string selectedIssueId = ""; // Store selected issue ID
        
        public IssueBooks()
        {
            InitializeComponent();

            displayBookIssueData();
            DataBookTitle();
            DataStudentName();
            
        }

        public void refreshData()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)refreshData);
                return;
            }

            displayBookIssueData();
            DataBookTitle();
            DataStudentName();
        }

        public void displayBookIssueData()
        {

            DataIssueBooks dib = new DataIssueBooks();
            List<DataIssueBooks> listData = dib.IssueBooksData();

            dataGridView1.DataSource = listData;
        }

        private void bookIssue_addBtn_Click(object sender, EventArgs e)
        {
            if (name_of_student.SelectedValue == null
                || bookIssue_bookTitle.SelectedValue == null
                || string.IsNullOrWhiteSpace(bookIssue_author.Text)
                || bookIssue_issueDate.Value == null
                || bookIssue_returnDate.Value == null
                || string.IsNullOrWhiteSpace(bookIssue_status.Text))
            {
                MessageBox.Show("Please fill all required fields", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (connect.State != ConnectionState.Open)
            {
                try
                {
                    connect.Open();

                    int selectedBookId = Convert.ToInt32(((DataRowView)bookIssue_bookTitle.SelectedItem)["id"]);
                    int userId = Convert.ToInt32(((DataRowView)name_of_student.SelectedItem)["id"]);
                    string userName = ((DataRowView)name_of_student.SelectedItem)["name"].ToString();
                    string userContact = ((DataRowView)name_of_student.SelectedItem)["idcode"].ToString();

                    // Generate unique issue ID
                    string issueId = GenerateUniqueIssueId();
                    
                    // Get book title and author
                    string bookTitle = ((DataRowView)bookIssue_bookTitle.SelectedItem)["book_title"].ToString();
                    string author = bookIssue_author.Text.Trim();
                    
                    // Insert new issue record
                    string insertQuery = "INSERT INTO issues (issue_id, user_id, book_id, book_title, author, full_name, contact, issue_date, return_date, status, date_insert) " +
                        "VALUES (@issue_id, @user_id, @book_id, @book_title, @author, @full_name, @contact, @issue_date, @return_date, 'Not Return', @date_insert)";
                    
                    using (SqlCommand cmd = new SqlCommand(insertQuery, connect))
                    {
                        cmd.Parameters.AddWithValue("@issue_id", issueId);
                        cmd.Parameters.AddWithValue("@user_id", userId);
                        cmd.Parameters.AddWithValue("@book_id", selectedBookId);
                        cmd.Parameters.AddWithValue("@book_title", bookTitle);
                        cmd.Parameters.AddWithValue("@author", author);
                        cmd.Parameters.AddWithValue("@full_name", userName);
                        cmd.Parameters.AddWithValue("@contact", userContact);
                        cmd.Parameters.AddWithValue("@issue_date", bookIssue_issueDate.Value.Date);
                        cmd.Parameters.AddWithValue("@return_date", bookIssue_returnDate.Value.Date);
                        cmd.Parameters.AddWithValue("@date_insert", DateTime.Today);

                        cmd.ExecuteNonQuery();

                        displayBookIssueData();
                        MessageBox.Show("Issued successfully! Issue ID: " + issueId, "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        clearFields();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connect.Close();
                }
            }
        }

        /// <summary>
        /// Generates a unique issue ID that doesn't exist in the database
        /// Note: Assumes database connection is already open
        /// </summary>
        private string GenerateUniqueIssueId()
        {
            string issueId = "";
            int attempts = 0;
            const int maxAttempts = 10;
            
            do
            {
                // Generate issue ID: ISS + YYYYMMDDHHmmss + random 3 digits
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string randomSuffix = new Random().Next(100, 999).ToString();
                issueId = "ISS" + timestamp + randomSuffix;
                
                // Check if this ID already exists (connection should be open when this is called)
                if (connect.State == ConnectionState.Open)
                {
                    string checkQuery = "SELECT COUNT(*) FROM issues WHERE issue_id = @issueId";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, connect))
                    {
                        checkCmd.Parameters.AddWithValue("@issueId", issueId);
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (count == 0)
                        {
                            return issueId; // Unique ID found
                        }
                    }
                }
                else
                {
                    // If connection is not open, just return the generated ID
                    // (uniqueness will be checked at database level if there's a unique constraint)
                    return issueId;
                }
                
                attempts++;
                if (attempts >= maxAttempts)
                {
                    // Fallback: use GUID if we can't generate a unique timestamp-based ID
                    issueId = "ISS" + Guid.NewGuid().ToString("N").Substring(0, 15).ToUpper();
                    break;
                }
                
                // Small delay to ensure different timestamp on next attempt
                System.Threading.Thread.Sleep(10);
            } while (attempts < maxAttempts);
            
            return issueId;
        }

        public void clearFields()
        {
            name_of_student.SelectedIndex = -1;
            bookIssue_bookTitle.SelectedIndex = -1;
            bookIssue_author.SelectedIndex = -1;
            bookIssue_status.SelectedIndex = -1;
            bookIssue_picture.Image = null;
            selectedIssueId = ""; // Clear stored issue ID
            dataGridView1.ClearSelection();
        }

        public void DataBookTitle()
        {
            if(connect.State == ConnectionState.Closed)
            {
                try
                {
                    connect.Open();
                    string selectData = "SELECT id, book_title FROM books WHERE status = 'Available' AND date_delete IS NULL";

                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        bookIssue_bookTitle.DataSource = table;
                        bookIssue_bookTitle.DisplayMember = "book_title";
                        bookIssue_bookTitle.ValueMember = "id";

                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
                finally
                {
                    connect.Close();
                }
            }
            
        }

        public void DataStudentName()
        {
            if(connect.State == ConnectionState.Closed)
            {
                try
                {
                    connect.Open();
                    string selectData = "SELECT id, name, idcode FROM users WHERE role = 'student' AND is_active = 1 AND date_delete IS NULL";

                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        name_of_student.DataSource = table;
                        name_of_student.DisplayMember = "name";
                        name_of_student.ValueMember = "id";

                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
                finally
                {
                    connect.Close();
                }
            }
        }

        private async void bookIssue_bookTitle_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(connect.State != ConnectionState.Open)
            {
                if (bookIssue_bookTitle.SelectedValue != null)
                {
                    DataRowView selectedRow = (DataRowView)bookIssue_bookTitle.SelectedItem;
                    int selectID = Convert.ToInt32(selectedRow["id"]);
                    try
                    {
                        connect.Open();

                        string selectData = "SELECT * FROM books WHERE id = @id";

                        using (SqlCommand cmd = new SqlCommand(selectData, connect))
                        {
                            cmd.Parameters.AddWithValue("@id", selectID);

                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            DataTable table = new DataTable();
                            adapter.Fill(table);

                            if (table.Rows.Count > 0)
                            {
                                bookIssue_author.Text = table.Rows[0]["author"].ToString();

                                string blobName = table.Rows[0]["image"]?.ToString();

                                // Download image from Azure Blob Storage
                                if (!string.IsNullOrWhiteSpace(blobName))
                                {
                                    try
                                    {
                                        var cover = await BlobCovers.DownloadAsync(blobName);
                                        bookIssue_picture.Image = cover;
                                    }
                                    catch (Exception imgEx)
                                    {
                                        // Silently fail - just show no image
                                        System.Diagnostics.Debug.WriteLine($"Failed to load image {blobName}: {imgEx.Message}");
                                        bookIssue_picture.Image = null;
                                    }
                                }
                                else
                                {
                                    bookIssue_picture.Image = null;
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        connect.Close();
                    }
                }
            }
            
        }

        private void dataGridView1_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                // Select the entire row when a cell is clicked
                dataGridView1.ClearSelection();
                dataGridView1.Rows[e.RowIndex].Selected = true;
                dataGridView1.CurrentCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                
                // Store the issue ID for update/delete operations
                selectedIssueId = row.Cells[1].Value?.ToString() ?? "";
                
                // Try to find and select the student by name
                string studentName = row.Cells[2].Value?.ToString();
                if (!string.IsNullOrWhiteSpace(studentName))
                {
                    for (int i = 0; i < name_of_student.Items.Count; i++)
                    {
                        DataRowView item = (DataRowView)name_of_student.Items[i];
                        if (item["name"].ToString() == studentName)
                        {
                            name_of_student.SelectedIndex = i;
                            break;
                        }
                    }
                }
                
                // Try to find and select the book by title
                string bookTitle = row.Cells[4].Value?.ToString();
                if (!string.IsNullOrWhiteSpace(bookTitle))
                {
                    for (int i = 0; i < bookIssue_bookTitle.Items.Count; i++)
                    {
                        DataRowView item = (DataRowView)bookIssue_bookTitle.Items[i];
                        if (item["book_title"].ToString() == bookTitle)
                        {
                            bookIssue_bookTitle.SelectedIndex = i;
                            break;
                        }
                    }
                }
                
                bookIssue_author.Text = row.Cells[5].Value?.ToString();
                
                // Handle date parsing with error handling
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

                try
                {
                    if (row.Cells[7].Value != null && DateTime.TryParse(row.Cells[7].Value.ToString(), out DateTime returnDate))
                    {
                        bookIssue_returnDate.Value = returnDate;
                    }
                    else
                    {
                        bookIssue_returnDate.Value = DateTime.Today.AddDays(7); // Default 7 days from today
                    }
                }
                catch
                {
                    bookIssue_returnDate.Value = DateTime.Today.AddDays(7);
                }
                
                bookIssue_status.Text = row.Cells[8].Value?.ToString();

            }
        }

        private void bookIssue_updateBtn_Click(object sender, EventArgs e)
        {
            // Get issue ID from stored value or selected row
            string issueId = "";
            if (!string.IsNullOrWhiteSpace(selectedIssueId))
            {
                issueId = selectedIssueId;
            }
            else if (dataGridView1.SelectedRows.Count > 0)
            {
                issueId = dataGridView1.SelectedRows[0].Cells[1].Value?.ToString() ?? "";
            }
            else if (dataGridView1.CurrentRow != null)
            {
                issueId = dataGridView1.CurrentRow.Cells[1].Value?.ToString() ?? "";
            }
            
            if (string.IsNullOrWhiteSpace(issueId))
            {
                MessageBox.Show("Please select an issue from the list first", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (name_of_student.SelectedValue == null
                || bookIssue_bookTitle.SelectedValue == null
                || string.IsNullOrWhiteSpace(bookIssue_author.Text)
                || bookIssue_issueDate.Value == null
                || bookIssue_returnDate.Value == null
                || string.IsNullOrWhiteSpace(bookIssue_status.Text))
            {
                MessageBox.Show("Please fill all required fields", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (connect.State != ConnectionState.Open)
            {

                    DialogResult check = MessageBox.Show("Are you sure you want to UPDATE Issue ID: "
                        + issueId + "?", "Confirmation Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (check == DialogResult.Yes)
                    {
                        try
                        {
                            connect.Open();
                            DateTime today = DateTime.Today;
                            
                            string userName = ((DataRowView)name_of_student.SelectedItem)["name"].ToString();
                            string userContact = ((DataRowView)name_of_student.SelectedItem)["idcode"].ToString();
                            string bookTitle = ((DataRowView)bookIssue_bookTitle.SelectedItem)["book_title"].ToString();
                            
                            string updateData = "UPDATE issues SET full_name = @fullName, contact = @contact" +
                                ", book_title = @bookTitle, author = @author, status = @status, issue_date = @issueDate" +
                                ", return_date = @returnDate, date_update = @dateUpdate WHERE issue_id = @issueID";

                            using (SqlCommand cmd = new SqlCommand(updateData, connect))
                            {
                                cmd.Parameters.AddWithValue("@fullName", userName);
                                cmd.Parameters.AddWithValue("@contact", userContact);
                                cmd.Parameters.AddWithValue("@bookTitle", bookTitle);
                                cmd.Parameters.AddWithValue("@author", bookIssue_author.Text.Trim());
                                cmd.Parameters.AddWithValue("@status", bookIssue_status.Text.Trim());
                                cmd.Parameters.AddWithValue("@issueDate", bookIssue_issueDate.Value);
                                cmd.Parameters.AddWithValue("@returnDate", bookIssue_returnDate.Value);
                                cmd.Parameters.AddWithValue("@dateUpdate", today);
                                cmd.Parameters.AddWithValue("@issueID", issueId);

                                cmd.ExecuteNonQuery();

                                displayBookIssueData();

                                MessageBox.Show("Updated successfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                clearFields();
                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {
                            connect.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Cancelled.", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
            }
        }

        private void bookIssue_deleteBtn_Click(object sender, EventArgs e)
        {
            // Get issue ID from stored value or selected row
            string issueId = "";
            if (!string.IsNullOrWhiteSpace(selectedIssueId))
            {
                issueId = selectedIssueId;
            }
            else if (dataGridView1.SelectedRows.Count > 0)
            {
                issueId = dataGridView1.SelectedRows[0].Cells[1].Value?.ToString() ?? "";
            }
            else if (dataGridView1.CurrentRow != null)
            {
                issueId = dataGridView1.CurrentRow.Cells[1].Value?.ToString() ?? "";
            }
            
            if (string.IsNullOrWhiteSpace(issueId))
            {
                MessageBox.Show("Please select an issue from the list first", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (connect.State != ConnectionState.Open)
            {
                DialogResult check = MessageBox.Show("Are you sure you want to DELETE Issue ID: "
                    + issueId + "?", "Confirmation Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (check == DialogResult.Yes)
                {
                    try
                    {
                        connect.Open();
                        DateTime today = DateTime.Today;
                        string updateData = "UPDATE issues SET date_delete = @dateDelete WHERE issue_id = @issueID";

                        using (SqlCommand cmd = new SqlCommand(updateData, connect))
                        {
                            cmd.Parameters.AddWithValue("@dateDelete", today);
                            cmd.Parameters.AddWithValue("@issueID", issueId);

                            cmd.ExecuteNonQuery();

                            displayBookIssueData();

                            MessageBox.Show("Deleted successfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            clearFields();
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        connect.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Cancelled.", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void bookIssue_clearBtn_Click(object sender, EventArgs e)
        {
            clearFields();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void bookIssue_picture_Click(object sender, EventArgs e)
        {

        }

        private void bookIssue_id_TextChanged(object sender, EventArgs e)
        {

        }

        private void bookIssue_status_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void bookIssue_author_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void bookIssue_returnDate_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void IssueBooks_Load(object sender, EventArgs e)
        {

        }

        private void name_of_student_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
