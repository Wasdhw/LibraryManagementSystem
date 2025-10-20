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
    public partial class Dashboard : UserControl
    {
        SqlConnection connect = Database.GetConnection();

        public Dashboard()
        {
            InitializeComponent();

            displayAB();
            displayIB();
            displayRB();
        }

        public void refreshData()
        {

            if (InvokeRequired)
            {
                Invoke((MethodInvoker)refreshData);
                return;
            }

            displayAB();
            displayIB();
            displayRB();
            displayExtraStats();
        }

        public void displayAB()
        {
            if(connect.State == ConnectionState.Closed)
            {
                try
                {
                    connect.Open();
                    string selectData = "SELECT COUNT(id) FROM books " +
                        "WHERE status = 'Available' AND date_delete IS NULL";

                    using(SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        int tempAB = 0;

                        if (reader.Read())
                        {
                            tempAB = Convert.ToInt32(reader[0]);

                            dashboard_AB.Text = tempAB.ToString();
                        }
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

        public void displayIB()
        {
            if (connect.State == ConnectionState.Closed)
            {
                try
                {
                    connect.Open();
                    string selectData = "SELECT COUNT(id) FROM issues " +
                        "WHERE date_delete IS NULL";

                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        int tempIB = 0;

                        if (reader.Read())
                        {
                            tempIB = Convert.ToInt32(reader[0]);

                            dashboard_IB.Text = tempIB.ToString();
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

        public void displayRB()
        {
            if (connect.State == ConnectionState.Closed)
            {
                try
                {
                    connect.Open();
                    string selectData = "SELECT COUNT(id) FROM issues " +
                        " WHERE status = 'Return' AND date_delete IS NULL";

                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        int tempRB = 0;

                        if (reader.Read())
                        {
                            tempRB = Convert.ToInt32(reader[0]);

                            dashboard_RB.Text = tempRB.ToString();
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

        private int GetTotalUsers()
        {
            if (connect.State == ConnectionState.Closed)
            {
                try
                {
                    connect.Open();
                    string sql = "SELECT COUNT(id) FROM users WHERE is_active = 1 AND date_delete IS NULL";
                    using (SqlCommand cmd = new SqlCommand(sql, connect))
                    {
                        object result = cmd.ExecuteScalar();
                        return Convert.ToInt32(result);
                    }
                }
                catch
                {
                    return 0;
                }
                finally
                {
                    connect.Close();
                }
            }
            return 0;
        }

        private int GetOverdueCount()
        {
            if (connect.State == ConnectionState.Closed)
            {
                try
                {
                    connect.Open();
                    string sql = "SELECT COUNT(id) FROM issues WHERE status = 'Not Return' AND return_date < GETDATE() AND date_delete IS NULL";
                    using (SqlCommand cmd = new SqlCommand(sql, connect))
                    {
                        object result = cmd.ExecuteScalar();
                        return Convert.ToInt32(result);
                    }
                }
                catch
                {
                    return 0;
                }
                finally
                {
                    connect.Close();
                }
            }
            return 0;
        }

        public void displayExtraStats()
        {
            int totalUsers = GetTotalUsers();
            int overdue = GetOverdueCount();

            // Update labels if present
            if (dashboard_TotalUsers != null)
            {
                dashboard_TotalUsers.Text = "Users: " + totalUsers;
            }
            if (dashboard_Overdue != null)
            {
                dashboard_Overdue.Text = "Overdue: " + overdue;
            }
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dashboard_AB_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void dashboard_IB_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Dashboard_Load(object sender, EventArgs e)
        {

        }
    }
}
