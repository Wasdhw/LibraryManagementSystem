using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LibraryManagementSystem.Utils;

namespace LibraryManagementSystem.studentUser
{
    public partial class StReturninfo : UserControl
    {
        SqlConnection connect = Database.GetConnection();
        private int currentUserId = 0;

        public StReturninfo()
        {
            InitializeComponent();
        }

        public void LoadReturnInfo(int userId, string issueId)
        {
            currentUserId = userId > 0 ? userId : (SessionManager.CurrentUserId > 0 ? SessionManager.CurrentUserId : 1);
            LoadReturnDetails(issueId);
        }

        private void LoadReturnDetails(string issueId)
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
                            DisplayReturnInfo(reader);
                        }
                        else
                        {
                            MessageBox.Show("Return information not found.", "Error Message", 
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading return information: " + ex.Message, "Error Message", 
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

        private void DisplayReturnInfo(SqlDataReader reader)
        {
            try
            {
                // Update UI controls with return information
                if (this.Controls.Find("Title", true).FirstOrDefault() is Label lblBookTitle)
                    lblBookTitle.Text = reader["book_title"].ToString();

                if (this.Controls.Find("Author", true).FirstOrDefault() is Label lblAuthor)
                    lblAuthor.Text = reader["author"].ToString();

                if (this.Controls.Find("Published", true).FirstOrDefault() is Label lblIssueDate)
                    lblIssueDate.Text = Convert.ToDateTime(reader["issue_date"]).ToString("yyyy-MM-dd");

                if (this.Controls.Find("Quantity", true).FirstOrDefault() is Label lblReturnDate)
                    lblReturnDate.Text = Convert.ToDateTime(reader["return_date"]).ToString("yyyy-MM-dd");

                if (this.Controls.Find("Availability", true).FirstOrDefault() is Label lblStatus)
                {
                    lblStatus.Text = reader["status"].ToString();
                    if (reader["status"].ToString() == "Return")
                    {
                        lblStatus.ForeColor = Color.Green;
                    }
                    else if (reader["status"].ToString() == "Not Return")
                    {
                        int daysOverdue = Convert.ToInt32(reader["days_overdue"]);
                        if (daysOverdue > 0)
                        {
                            lblStatus.ForeColor = Color.Red;
                            lblStatus.Text += " (Overdue)";
                        }
                        else
                        {
                            lblStatus.ForeColor = Color.Orange;
                        }
                    }
                }

                // Note: These controls may not exist in the current Designer
                // You may need to add them to the Designer file or use existing controls
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error displaying return information: " + ex.Message, "Error Message", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void bookreturn_addBtn_Click(object sender, EventArgs e)
        {
            // Handle return button click
            MessageBox.Show("Return functionality will be implemented here.", "Info", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Handle close button click
            this.Parent?.Parent?.Hide();
        }
    }
}
