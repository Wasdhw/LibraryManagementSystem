using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using LibraryManagementSystem.Utils;

namespace LibraryManagementSystem.MainformsUser
{
    public partial class AuditLog : UserControl
    {
        SqlConnection connect = Database.GetConnection();

        public AuditLog()
        {
            InitializeComponent();
            ThemeManager.StyleDataGridView(dataGridView1);
            LoadAuditLogs();
        }

        public void refreshData()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)refreshData);
                return;
            }
            LoadAuditLogs();
        }

        private void LoadAuditLogs()
        {
            try
            {
                if (connect.State == ConnectionState.Closed)
                {
                    connect.Open();
                }

                // Check if audit_logs table exists
                string checkTableQuery = @"
                    SELECT COUNT(*) 
                    FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_NAME = 'audit_logs'";
                
                using (SqlCommand checkCmd = new SqlCommand(checkTableQuery, connect))
                {
                    int tableExists = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (tableExists == 0)
                    {
                        MessageBox.Show("The 'audit_logs' table does not exist in the database. Please run the Database_Migration_Script.sql to create the required tables.", 
                            "Database Setup Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                string query = @"
                    SELECT 
                        a.id,
                        u.name as user_name,
                        a.action,
                        a.entity_type,
                        a.entity_id,
                        a.details,
                        a.timestamp
                    FROM audit_logs a
                    LEFT JOIN users u ON a.user_id = u.id
                    ORDER BY a.timestamp DESC";

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
                if (ex.Message.Contains("Invalid object name 'audit_logs'"))
                {
                    MessageBox.Show("The 'audit_logs' table does not exist in the database. Please run the Database_Migration_Script.sql file to create the required tables.", 
                        "Database Setup Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Error loading audit logs: " + ex.Message, "Error Message", 
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

                    if (dataGridView1.Columns["timestamp"] != null)
                        dataGridView1.Columns["timestamp"].HeaderText = "Timestamp";

                    if (dataGridView1.Columns["user_name"] != null)
                        dataGridView1.Columns["user_name"].HeaderText = "User";

                    if (dataGridView1.Columns["action"] != null)
                        dataGridView1.Columns["action"].HeaderText = "Action";

                    if (dataGridView1.Columns["entity_type"] != null)
                        dataGridView1.Columns["entity_type"].HeaderText = "Entity Type";

                    if (dataGridView1.Columns["entity_id"] != null)
                        dataGridView1.Columns["entity_id"].HeaderText = "Entity ID";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error formatting DataGridView: {ex.Message}");
            }
        }

        private void refreshBtn_Click(object sender, EventArgs e)
        {
            LoadAuditLogs();
        }
    }
}

