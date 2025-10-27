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

        public ReturnBooks()
        {
            InitializeComponent();

            displayIssuedBooksData();
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
                || returnBooks_contact.Text == ""
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
                        try
                        {
                            connect.Open();

                            // Update the issue status to 'Return' and set return date
                            string updateQuery = "UPDATE issues SET status = 'Return', return_date = @returnDate WHERE issue_id = @issue_id";
                            
                            using (SqlCommand cmd = new SqlCommand(updateQuery, connect))
                            {
                                cmd.Parameters.AddWithValue("@issue_id", returnBooks_issueID.Text.Trim());
                                cmd.Parameters.AddWithValue("@returnDate", DateTime.Today);

                                cmd.ExecuteNonQuery();

                                displayIssuedBooksData();
                                MessageBox.Show("Returned successfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    
                }
            }
        }

        public void displayIssuedBooksData()
        {
            DataIssueBooks dib = new DataIssueBooks();
            List<DataIssueBooks> listData = dib.ReturnIssueBooksData();

            dataGridView1.DataSource = listData;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                returnBooks_issueID.Text = row.Cells[1].Value.ToString();
                returnBooks_name.Text = row.Cells[2].Value.ToString();
                returnBooks_contact.Text = row.Cells[3].Value.ToString();
                returnBooks_bookTitle.Text = row.Cells[5].Value.ToString();
                returnBooks_author.Text = row.Cells[6].Value.ToString();
                bookIssue_issueDate.Text = row.Cells[7].Value.ToString();
            }
        }

        public void clearFields()
        {
            returnBooks_issueID.Text = "";
            returnBooks_name.Text = "";
            returnBooks_contact.Text = "";
            returnBooks_bookTitle.Text = "";
            returnBooks_author.Text = "";
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
    }
}
