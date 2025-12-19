using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LibraryManagementSystem.Utils;

namespace LibraryManagementSystem.MainformsUser
{
    public partial class Reservations : UserControl
    {
        SqlConnection connect = Database.GetConnection();

        public Reservations()
        {
            InitializeComponent();
            ThemeManager.StyleDataGridView(dataGridView1);
            LoadReservations();
        }

        public void refreshData()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)refreshData);
                return;
            }
            LoadReservations();
        }

        private void LoadReservations()
        {
            try
            {
                if (connect.State == ConnectionState.Closed)
                {
                    connect.Open();
                }

                // Check if reservations table exists
                string checkTableQuery = @"
                    SELECT COUNT(*) 
                    FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_NAME = 'reservations'";
                
                using (SqlCommand checkCmd = new SqlCommand(checkTableQuery, connect))
                {
                    int tableExists = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (tableExists == 0)
                    {
                        MessageBox.Show("The 'reservations' table does not exist in the database. Please run the Database_Migration_Script.sql to create the required tables.", 
                            "Database Setup Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                string query = @"
                    SELECT 
                        r.id,
                        r.book_id,
                        b.book_title,
                        b.author,
                        u.name as student_name,
                        u.idcode as student_id,
                        r.status,
                        r.date_reserved,
                        r.date_notified,
                        r.date_expires
                    FROM reservations r
                    INNER JOIN books b ON r.book_id = b.id
                    INNER JOIN users u ON r.user_id = u.id
                    WHERE r.date_delete IS NULL
                    ORDER BY r.date_reserved DESC";

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
                if (ex.Message.Contains("Invalid object name 'reservations'"))
                {
                    MessageBox.Show("The 'reservations' table does not exist in the database. Please run the Database_Migration_Script.sql file to create the required tables.", 
                        "Database Setup Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Error loading reservations: " + ex.Message, "Error Message", 
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

                    if (dataGridView1.Columns["book_id"] != null)
                        dataGridView1.Columns["book_id"].Visible = false;

                    if (dataGridView1.Columns["date_reserved"] != null)
                        dataGridView1.Columns["date_reserved"].HeaderText = "Date Reserved";

                    if (dataGridView1.Columns["date_expires"] != null)
                        dataGridView1.Columns["date_expires"].HeaderText = "Expires";

                    if (dataGridView1.Columns["date_notified"] != null)
                        dataGridView1.Columns["date_notified"].HeaderText = "Date Notified";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error formatting DataGridView: {ex.Message}");
            }
        }

        private void refreshBtn_Click(object sender, EventArgs e)
        {
            LoadReservations();
        }
    }
}

