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

        private async void loginBtn_Click(object sender, EventArgs e)
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

                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            DataTable table = new DataTable();
                            adapter.Fill(table);

                            if (table.Rows.Count >= 1)
                            {
                                string stored = table.Rows[0]["password"].ToString();
                                string input = login_password.Text.Trim();

                                if (!string.Equals(stored, input, StringComparison.Ordinal))
                                {
                                    MessageBox.Show("Incorrect Username/Password", "Error Message"
                                        , MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }

                                string userRole = table.Rows[0]["role"].ToString();
                                string userName = table.Rows[0]["name"].ToString();
                                int userId = Convert.ToInt32(table.Rows[0]["id"]);

                                // Set user session
                                SessionManager.SetUserSession(userId, userName, userRole);

                                MessageBox.Show("Login Successfully! Welcome " + userName, "Information Message"
                                    , MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Check if there are new/updated book covers, then show progress dialog if needed
                                bool hasUpdates = await BlobCovers.HasNewOrUpdatedCoversAsync();
                                if (hasUpdates)
                                {
                                    await ShowImageDownloadProgressAsync();
                                }

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

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private async Task ShowImageDownloadProgressAsync()
        {
            // Create and show progress dialog (non-modal but topmost)
            var progressForm = new ImageDownloadProgressForm();
            progressForm.TopMost = true;
            progressForm.Show();
            progressForm.BringToFront();
            progressForm.Update();

            try
            {
                // Download new/updated book covers with progress updates
                await BlobCovers.DownloadAllBookCoversAsync((current, total, bookTitle) =>
                {
                    progressForm.UpdateProgress(current, total, bookTitle);
                });

                // Mark as complete (even if total was 0, this sets it to 100%)
                progressForm.SetComplete();
                
                // Wait a moment so user can see 100%
                await Task.Delay(500);
            }
            catch (Exception ex)
            {
                // Even if download fails, continue to main form
                System.Diagnostics.Debug.WriteLine($"Error downloading covers: {ex.Message}");
            }
            finally
            {
                // Close progress dialog
                if (progressForm.InvokeRequired)
                {
                    progressForm.Invoke(new Action(() => progressForm.Close()));
                }
                else
                {
                    progressForm.Close();
                }
            }
        }
    }
}
