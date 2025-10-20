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
        public IssueBooks()
        {
            InitializeComponent();

            displayBookIssueData();
            DataBookTitle();
            
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
        }

        public void displayBookIssueData()
        {

            DataIssueBooks dib = new DataIssueBooks();
            List<DataIssueBooks> listData = dib.IssueBooksData();

            dataGridView1.DataSource = listData;
        }

        private void bookIssue_addBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(bookIssue_name.Text)
                || string.IsNullOrWhiteSpace(bookIssue_contact.Text)
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

                    int userId = GetUserId(bookIssue_name.Text.Trim(), bookIssue_contact.Text.Trim());
                    if (userId == 0)
                    {
                        MessageBox.Show("User not found. Please register the user first or enter the exact registered name.",
                            "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    using (SqlCommand cmd = new SqlCommand("sp_IssueBook", connect))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@user_id", userId);
                        cmd.Parameters.AddWithValue("@book_id", selectedBookId);
                        cmd.Parameters.AddWithValue("@full_name", bookIssue_name.Text.Trim());
                        cmd.Parameters.AddWithValue("@contact", bookIssue_contact.Text.Trim());
                        cmd.Parameters.AddWithValue("@issue_date", bookIssue_issueDate.Value.Date);
                        cmd.Parameters.AddWithValue("@return_date", bookIssue_returnDate.Value.Date);

                        SqlParameter outIssue = new SqlParameter("@issue_id", SqlDbType.NVarChar, 50)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outIssue);

                        cmd.ExecuteNonQuery();

                        // Set generated issue id back to UI
                        if (outIssue.Value != DBNull.Value)
                        {
                            bookIssue_id.Text = outIssue.Value.ToString();
                        }

                        displayBookIssueData();
                        MessageBox.Show("Issued successfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private int GetUserId(string nameOrUsername, string idcode)
        {
            string sql = "SELECT TOP 1 id FROM users WHERE is_active = 1 AND date_delete IS NULL AND (name = @n OR username = @n OR idcode = @c)";
            using (SqlCommand cmd = new SqlCommand(sql, connect))
            {
                cmd.Parameters.AddWithValue("@n", nameOrUsername);
                cmd.Parameters.AddWithValue("@c", idcode);
                object result = cmd.ExecuteScalar();
                if (result == null || result == DBNull.Value) return 0;
                return Convert.ToInt32(result);
            }
        }

        public void clearFields()
        {
            bookIssue_id.Text = "";
            bookIssue_name.Text = "";
            bookIssue_contact.Text = "";
            bookIssue_bookTitle.SelectedIndex = -1;
            bookIssue_author.SelectedIndex = -1;
            bookIssue_status.SelectedIndex = -1;
            bookIssue_picture.Image = null;
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

        private void bookIssue_bookTitle_SelectedIndexChanged(object sender, EventArgs e)
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

                                string imagePath = table.Rows[0]["image"].ToString();

                                if (imagePath != null)
                                {
                                    bookIssue_picture.Image = Image.FromFile(imagePath);
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
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                bookIssue_id.Text = row.Cells[1].Value.ToString();
                bookIssue_name.Text = row.Cells[2].Value.ToString();
                bookIssue_contact.Text = row.Cells[3].Value.ToString();
                bookIssue_bookTitle.Text = row.Cells[5].Value.ToString();
                bookIssue_author.Text = row.Cells[6].Value.ToString();
                bookIssue_issueDate.Text = row.Cells[7].Value.ToString();
                bookIssue_returnDate.Text = row.Cells[8].Value.ToString();
                bookIssue_status.Text = row.Cells[9].Value.ToString();

            }
        }

        private void bookIssue_updateBtn_Click(object sender, EventArgs e)
        {
            if (bookIssue_id.Text == ""
                || bookIssue_name.Text == ""
                || bookIssue_contact.Text == ""
                || bookIssue_bookTitle.Text == ""
                || bookIssue_author.Text == ""
                || bookIssue_issueDate.Value == null
                || bookIssue_returnDate.Value == null
                || bookIssue_status.Text == ""
                || bookIssue_picture.Image == null)
            {
                MessageBox.Show("Please select item first", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (connect.State != ConnectionState.Open)
                {
                    DialogResult check = MessageBox.Show("Are you sure you want to UPDATE Issue ID:"
                        + bookIssue_id + "?", "Confirmation Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (check == DialogResult.Yes)
                    {
                        try
                        {
                            connect.Open();
                            DateTime today = DateTime.Today;
                            string updateData = "UPDATE issues SET full_name = @fullName, contact = @contact" +
                                ", book_title = @bookTitle, author = @author, status = @status, issue_date = @issueDate" +
                                ", return_date = @returnDate, date_update = @dateUpdate WHERE issue_id = @issueID";

                            using (SqlCommand cmd = new SqlCommand(updateData, connect))
                            {
                                cmd.Parameters.AddWithValue("@fullName", bookIssue_name.Text.Trim());
                                cmd.Parameters.AddWithValue("@contact", bookIssue_contact.Text.Trim());
                                cmd.Parameters.AddWithValue("@bookTitle", bookIssue_bookTitle.Text.Trim());
                                cmd.Parameters.AddWithValue("@author", bookIssue_author.Text.Trim());
                                cmd.Parameters.AddWithValue("@status", bookIssue_status.Text.Trim());
                                cmd.Parameters.AddWithValue("@issueDate", bookIssue_issueDate.Value);
                                cmd.Parameters.AddWithValue("@returnDate", bookIssue_returnDate.Value);
                                cmd.Parameters.AddWithValue("@dateUpdate", today);
                                cmd.Parameters.AddWithValue("@issueID", bookIssue_id.Text.Trim());

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
        }

        private void bookIssue_deleteBtn_Click(object sender, EventArgs e)
        {
            if (bookIssue_id.Text == ""
                || bookIssue_name.Text == ""
                || bookIssue_contact.Text == ""
                || bookIssue_bookTitle.Text == ""
                || bookIssue_author.Text == ""
                || bookIssue_issueDate.Value == null
                || bookIssue_returnDate.Value == null
                || bookIssue_status.Text == ""
                || bookIssue_picture.Image == null)
            {
                MessageBox.Show("Please select item first", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (connect.State != ConnectionState.Open)
                {
                    DialogResult check = MessageBox.Show("Are you sure you want to DELETE Issue ID:"
                        + bookIssue_id.Text.Trim() + "?", "Confirmation Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

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
                                cmd.Parameters.AddWithValue("@issueID", bookIssue_id.Text.Trim());

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
    }
}
