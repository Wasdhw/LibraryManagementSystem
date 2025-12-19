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

namespace LibraryManagementSystem.MainformsUser
{
    public partial class Fines : UserControl
    {
        SqlConnection connect = Database.GetConnection();

        public Fines()
        {
            InitializeComponent();
            ThemeManager.StyleDataGridView(dataGridView1);
            LoadFines();
        }

        public void refreshData()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)refreshData);
                return;
            }
            LoadFines();
        }

        private void LoadFines()
        {
            try
            {
                if (connect.State == ConnectionState.Closed)
                {
                    connect.Open();
                }

                // Check if fines table exists
                string checkTableQuery = @"
                    SELECT COUNT(*) 
                    FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_NAME = 'fines'";
                
                using (SqlCommand checkCmd = new SqlCommand(checkTableQuery, connect))
                {
                    int tableExists = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (tableExists == 0)
                    {
                        MessageBox.Show("The 'fines' table does not exist in the database. Please run the Database_Migration_Script.sql to create the required tables.", 
                            "Database Setup Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                string query = @"
                    SELECT 
                        f.id,
                        f.issue_id,
                        i.book_title,
                        u.name as student_name,
                        u.idcode as student_id,
                        f.amount,
                        f.days_overdue,
                        f.status,
                        f.date_created,
                        f.date_paid
                    FROM fines f
                    INNER JOIN issues i ON f.issue_id = i.issue_id
                    INNER JOIN users u ON f.user_id = u.id
                    WHERE f.date_delete IS NULL
                    ORDER BY f.date_created DESC";

                using (SqlCommand cmd = new SqlCommand(query, connect))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    dataGridView1.DataSource = table;
                    FormatDataGridView();
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Invalid object name 'fines'"))
                {
                    MessageBox.Show("The 'fines' table does not exist in the database. Please run the Database_Migration_Script.sql file to create the required tables.", 
                        "Database Setup Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Error loading fines: " + ex.Message, "Error Message", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                if (connect.State == ConnectionState.Open)
                {
                    connect.Close();
                }
            }
        }

        private void FormatDataGridView()
        {
            try
            {
                if (dataGridView1.Columns.Count > 0)
                {
                    if (dataGridView1.Columns["id"] != null)
                        dataGridView1.Columns["id"].Visible = false;

                    if (dataGridView1.Columns["amount"] != null)
                    {
                        dataGridView1.Columns["amount"].HeaderText = "Amount";
                        dataGridView1.Columns["amount"].DefaultCellStyle.Format = "C2";
                    }

                    if (dataGridView1.Columns["days_overdue"] != null)
                        dataGridView1.Columns["days_overdue"].HeaderText = "Days Overdue";

                    if (dataGridView1.Columns["status"] != null)
                        dataGridView1.Columns["status"].HeaderText = "Status";

                    if (dataGridView1.Columns["date_created"] != null)
                        dataGridView1.Columns["date_created"].HeaderText = "Date Created";

                    if (dataGridView1.Columns["date_paid"] != null)
                        dataGridView1.Columns["date_paid"].HeaderText = "Date Paid";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error formatting DataGridView: {ex.Message}");
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                if (row.Cells["id"] != null && row.Cells["id"].Value != null)
                {
                    fineIdLabel.Text = "Fine ID: " + row.Cells["id"].Value.ToString();
                    selectedFineId = Convert.ToInt32(row.Cells["id"].Value);
                }
            }
        }

        private int selectedFineId = 0;

        private void markPaidBtn_Click(object sender, EventArgs e)
        {
            if (selectedFineId == 0)
            {
                MessageBox.Show("Please select a fine from the list first", "Error Message", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult check = MessageBox.Show("Mark this fine as paid?", "Confirmation Message", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (check == DialogResult.Yes)
            {
                try
                {
                    if (FineCalculator.MarkFineAsPaid(selectedFineId))
                    {
                        // Log audit action
                        int userId = SessionManager.CurrentUserId > 0 ? SessionManager.CurrentUserId : 0;
                        AuditLogger.LogFinePaid(selectedFineId, userId);

                        MessageBox.Show("Fine marked as paid successfully!", "Information Message", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadFines();
                        selectedFineId = 0;
                        if (fineIdLabel != null)
                            fineIdLabel.Text = "Fine ID: -";
                    }
                    else
                    {
                        MessageBox.Show("Error marking fine as paid.", "Error Message", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error Message", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void refreshBtn_Click(object sender, EventArgs e)
        {
            LoadFines();
        }
    }
}

