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
    public partial class LoginForm : Form
    {
        SqlConnection connect = Database.GetConnection();
        public LoginForm()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            login_password.PasswordChar = login_showPass.Checked ? '\0' : '*';
        }

        private void loginBtn_Click(object sender, EventArgs e)
        {
            if (login_username.Text == "" || login_password.Text == "")
            {
                MessageBox.Show("Please fill all blank fields", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (connect.State != ConnectionState.Open)
                {
                    try
                    {
                        connect.Open();

                        String selectData
                            = "SELECT id, name, username, role, password FROM users WHERE username = @username AND is_active = 1 AND date_delete IS NULL";
                        using (SqlCommand cmd = new SqlCommand(selectData, connect))
                        {
                            cmd.Parameters.AddWithValue("@username", login_username.Text.Trim());
                            // don't send password, we'll verify hash in app

                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            DataTable table = new DataTable();
                            adapter.Fill(table);

                            if (table.Rows.Count >= 1)
                            {
                                string storedHash = table.Rows[0]["password"].ToString();
                                string input = login_password.Text.Trim();

                                bool ok = Security.VerifyPassword(input, storedHash) || string.Equals(storedHash, input, StringComparison.Ordinal);

                                if (!ok)
                                {
                                    MessageBox.Show("Incorrect Username/Password", "Error Message"
                                        , MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }

                                // If DB still stores plaintext, upgrade it to hash
                                if (string.Equals(storedHash, input, StringComparison.Ordinal))
                                {
                                    try
                                    {
                                        string up = "UPDATE users SET password=@p, date_update=GETDATE() WHERE id=@id";
                                        using (SqlCommand upCmd = new SqlCommand(up, connect))
                                        {
                                            upCmd.Parameters.AddWithValue("@p", Security.HashPassword(input));
                                            upCmd.Parameters.AddWithValue("@id", Convert.ToInt32(table.Rows[0]["id"]));
                                            upCmd.ExecuteNonQuery();
                                        }
                                    }
                                    catch { /* best-effort upgrade */ }
                                }

                                string userRole = table.Rows[0]["role"].ToString();
                                string userName = table.Rows[0]["name"].ToString();
                                int userId = Convert.ToInt32(table.Rows[0]["id"]);

                                // Set user session
                                SessionManager.SetUserSession(userId, userName, userRole);

                                MessageBox.Show("Login Successfully! Welcome " + userName, "Information Message"
                                    , MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Redirect based on user role
                                if (userRole.ToLower() == "admin")
                                {
                                    MainForm mForm = new MainForm();
                                    mForm.Show();
                                    this.Hide();
                                }
                                else if (userRole.ToLower() == "student")
                                {
                                    StudentForm sForm = new StudentForm();
                                    sForm.Show();
                                    this.Hide();
                                }
                                else
                                {
                                    MessageBox.Show("Invalid user role. Please contact administrator.", "Error Message"
                                        , MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Incorrect Username/Password", "Error Message"
                                    , MessageBoxButtons.OK, MessageBoxIcon.Error);

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error connecting Database: " + ex, "Error Message"
                            , MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        connect.Close();
                    }
                }
            }
        }




    }
}
