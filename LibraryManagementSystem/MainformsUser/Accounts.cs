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
    public partial class Accounts : UserControl
    {
        SqlConnection connect = Database.GetConnection();

        public Accounts()
        {
            InitializeComponent();

            // Wire events (designer doesn't attach handlers)
            bookIssue_addBtn.Click += bookIssue_addBtn_Click;
            button1.Click += button1_Edit_Click;
            button2.Click += button2_Delete_Click;
            button3.Click += button3_Refresh_Click;
            dataGridView1.CellClick += dataGridView1_CellClick;

            LoadUsers();
        }
        public void refreshData()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)refreshData);
                return;
            }
            LoadUsers();
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void LoadUsers()
        {
            if (connect.State == ConnectionState.Closed)
            {
                try
                {
                    connect.Open();
                    string sql = "SELECT id, name, idcode, username, role, grade_course, date_register, is_active FROM users WHERE date_delete IS NULL";
                    using (SqlCommand cmd = new SqlCommand(sql, connect))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable table = new DataTable();
                        adapter.Fill(table);
                        dataGridView1.DataSource = table;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading users: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connect.Close();
                }
            }
        }

        private void ClearFields()
        {
            // Designer control names are reused from other forms; keep them
            bookIssue_id.Text = "";           // maps to idcode
            bookIssue_name.Text = "";         // maps to name
            bookIssue_contact.Text = "";      // unused (no DB column), optional
            email.Text = "";               // maps to username (email)
            grade_course.Text = "";         // maps to grade_course
            textBox2.Text = "";               // shows date_register (read-only usage)
        }

        private bool UsernameExists(string username)
        {
            string sql = "SELECT COUNT(*) FROM users WHERE username = @u AND date_delete IS NULL";
            using (SqlCommand cmd = new SqlCommand(sql, connect))
            {
                cmd.Parameters.AddWithValue("@u", username);
                object result = cmd.ExecuteScalar();
                return Convert.ToInt32(result) > 0;
            }
        }

        private bool IdCodeExists(string idcode)
        {
            string sql = "SELECT COUNT(*) FROM users WHERE idcode = @c AND date_delete IS NULL";
            using (SqlCommand cmd = new SqlCommand(sql, connect))
            {
                cmd.Parameters.AddWithValue("@c", idcode);
                object result = cmd.ExecuteScalar();
                return Convert.ToInt32(result) > 0;
            }
        }

        private bool UsernameExistsForOther(int id, string username)
        {
            string sql = "SELECT COUNT(*) FROM users WHERE username=@u AND id<>@id AND date_delete IS NULL";
            using (SqlCommand cmd = new SqlCommand(sql, connect))
            {
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@id", id);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private bool IdCodeExistsForOther(int id, string idcode)
        {
            string sql = "SELECT COUNT(*) FROM users WHERE idcode=@c AND id<>@id AND date_delete IS NULL";
            using (SqlCommand cmd = new SqlCommand(sql, connect))
            {
                cmd.Parameters.AddWithValue("@c", idcode);
                cmd.Parameters.AddWithValue("@id", id);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private void bookIssue_addBtn_Click(object sender, EventArgs e)
        {
            string name = bookIssue_name.Text.Trim();
            string idcode = bookIssue_id.Text.Trim();
            string username = email.Text.Trim(); // treat as username/email
            string grade = grade_course.Text.Trim();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(idcode) || string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Please enter Student ID, Name, and Username (Email)", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (connect.State == ConnectionState.Closed)
            {
                try
                {
                    connect.Open();
                    if (IdCodeExists(idcode))
                    {
                        MessageBox.Show("ID Code already exists.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (UsernameExists(username))
                    {
                        MessageBox.Show("Username already exists.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string insert = "INSERT INTO users (name, idcode, username, password, role, grade_course, date_register) VALUES (@n, @c, @u, @p, @r, @g, @d)";
                    using (SqlCommand cmd = new SqlCommand(insert, connect))
                    {
                        cmd.Parameters.AddWithValue("@n", name);
                        cmd.Parameters.AddWithValue("@c", idcode);
                        cmd.Parameters.AddWithValue("@u", username);
                        string temp = Security.GenerateTemporaryPassword();
                        cmd.Parameters.AddWithValue("@p", Security.HashPassword(temp));
                        cmd.Parameters.AddWithValue("@r", "student");
                        if (string.IsNullOrWhiteSpace(grade))
                        {
                            cmd.Parameters.AddWithValue("@g", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@g", grade);
                        }
                        cmd.Parameters.AddWithValue("@d", DateTime.Now);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Student account added. Temporary password set; please reset on first login.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadUsers();
                    ClearFields();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error adding user: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connect.Close();
                }
            }
        }

        private void button1_Edit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Please select a user to edit.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int id = Convert.ToInt32((dataGridView1.CurrentRow.Cells["id"].Value));
            string name = bookIssue_name.Text.Trim();
            string idcode = bookIssue_id.Text.Trim();
            string username = email.Text.Trim();
            string grade = grade_course.Text.Trim();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(idcode) || string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Please enter Student ID, Name, and Username (Email)", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (connect.State == ConnectionState.Closed)
            {
                try
                {
                    connect.Open();

                    if (UsernameExistsForOther(id, username))
                    {
                        MessageBox.Show("Username already exists.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (IdCodeExistsForOther(id, idcode))
                    {
                        MessageBox.Show("ID Code already exists.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string update = "UPDATE users SET name=@n, idcode=@c, username=@u, grade_course=@g, date_update=@du WHERE id=@id";
                    using (SqlCommand cmd = new SqlCommand(update, connect))
                    {
                        cmd.Parameters.AddWithValue("@n", name);
                        cmd.Parameters.AddWithValue("@c", idcode);
                        cmd.Parameters.AddWithValue("@u", username);
                        if (string.IsNullOrWhiteSpace(grade))
                        {
                            cmd.Parameters.AddWithValue("@g", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@g", grade);
                        }
                        cmd.Parameters.AddWithValue("@du", DateTime.Now);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Student account updated.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadUsers();
                    ClearFields();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating user: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connect.Close();
                }
            }
        }

        private void button2_Delete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Please select a user to delete.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int id = Convert.ToInt32((dataGridView1.CurrentRow.Cells["id"].Value));
            DialogResult confirm = MessageBox.Show("Are you sure you want to delete this user?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            if (connect.State == ConnectionState.Closed)
            {
                try
                {
                    connect.Open();
                    string softDelete = "UPDATE users SET is_active=0, date_delete=@dd WHERE id=@id";
                    using (SqlCommand cmd = new SqlCommand(softDelete, connect))
                    {
                        cmd.Parameters.AddWithValue("@dd", DateTime.Now);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Student account deleted.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadUsers();
                    ClearFields();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting user: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connect.Close();
                }
            }
        }

        private void button3_Refresh_Click(object sender, EventArgs e)
        {
            LoadUsers();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

            // Map DB fields to UI inputs
            bookIssue_id.Text = row.Cells["idcode"].Value?.ToString();
            bookIssue_name.Text = row.Cells["name"].Value?.ToString();
            email.Text = row.Cells["username"].Value?.ToString();
            if (row.Cells["grade_course"].Value != null)
            {
                grade_course.Text = row.Cells["grade_course"].Value?.ToString();
            }
            textBox2.Text = row.Cells["date_register"].Value?.ToString();
        }
    }
}
