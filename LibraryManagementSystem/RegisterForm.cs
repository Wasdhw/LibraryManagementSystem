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
using System.Configuration;
using LibraryManagementSystem.Utils;


namespace LibraryManagementSystem
{
    public partial class RegisterForm : Form
    {
        SqlConnection connect = Database.GetConnection();



        public RegisterForm()
        {
            InitializeComponent();
        }

        private void signIn_btn_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void register_btn_Click(object sender, EventArgs e)
        {
            // Validate all required fields
            if (!ValidateRegistrationFields())
            {
                return;
            }

            // Check if passwords match
            if (!ValidatePasswordMatch())
            {
                return;
            }

            if(connect.State != ConnectionState.Open)
            {
                try
                {
                    connect.Open();

                    // Check if username already exists
                    if (IsUsernameExists())
                    {
                        MessageBox.Show(username_textbox.Text.Trim() 
                            + " is already taken", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Check if ID code already exists
                    if (IsIdCodeExists())
                    {
                        MessageBox.Show("ID Code " + idcode_textbox.Text.Trim() 
                            + " is already registered", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Insert new user data
                    if (InsertUserData())
                    {
                        MessageBox.Show("Register successfully!", "Information Message"
                            , MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LoginForm lForm = new LoginForm();
                        lForm.Show();
                        this.Hide();
                    }

                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error connecting Database: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connect.Close();
                }
            }
        }

        // Validate all registration fields
        private bool ValidateRegistrationFields()
        {
            if (string.IsNullOrWhiteSpace(name_teboxxt.Text))
            {
                MessageBox.Show("Please enter your name", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                name_teboxxt.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(idcode_textbox.Text))
            {
                MessageBox.Show("Please enter your ID Code", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                idcode_textbox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(username_textbox.Text))
            {
                MessageBox.Show("Please enter a username", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                username_textbox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(password_textbox.Text))
            {
                MessageBox.Show("Please enter a password", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                password_textbox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(confirmpassword_textbox.Text))
            {
                MessageBox.Show("Please confirm your password", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                confirmpassword_textbox.Focus();
                return false;
            }

            return true;
        }

        // Validate password match
        private bool ValidatePasswordMatch()
        {
            if (password_textbox.Text != confirmpassword_textbox.Text)
            {
                MessageBox.Show("Passwords do not match", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                confirmpassword_textbox.Focus();
                return false;
            }
            return true;
        }

        // Check if username already exists
        private bool IsUsernameExists()
        {
            String checkUsername = "SELECT COUNT(*) FROM users WHERE username = @username";
            using (SqlCommand checkCMD = new SqlCommand(checkUsername, connect))
            {
                checkCMD.Parameters.AddWithValue("@username", username_textbox.Text.Trim());
                int count = (int)checkCMD.ExecuteScalar();
                return count >= 1;
            }
        }

        // Check if ID code already exists
        private bool IsIdCodeExists()
        {
            String checkIdCode = "SELECT COUNT(*) FROM users WHERE idcode = @idcode";
            using (SqlCommand checkCMD = new SqlCommand(checkIdCode, connect))
            {
                checkCMD.Parameters.AddWithValue("@idcode", idcode_textbox.Text.Trim());
                int count = (int)checkCMD.ExecuteScalar();
                return count >= 1;
            }
        }

        // Insert user data into database
        private bool InsertUserData()
        {
            try
            {
                DateTime day = DateTime.Now;

                String insertData = "INSERT INTO users (name, idcode, username, password, role, date_register) " +
                    "VALUES(@name, @idcode, @username, @password, @role, @date)";

                using (SqlCommand insertCMD = new SqlCommand(insertData, connect))
                {
                    insertCMD.Parameters.AddWithValue("@name", name_teboxxt.Text.Trim());
                    insertCMD.Parameters.AddWithValue("@idcode", idcode_textbox.Text.Trim());
                    insertCMD.Parameters.AddWithValue("@username", username_textbox.Text.Trim());
                                    insertCMD.Parameters.AddWithValue("@password", Security.HashPassword(password_textbox.Text.Trim()));
                    insertCMD.Parameters.AddWithValue("@role", "student"); // Default role for new registrations
                    
                    // Use proper SqlDbType for DATETIME2
                    var dateParam = insertCMD.Parameters.Add("@date", SqlDbType.DateTime2);
                    dateParam.Value = day;

                    insertCMD.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inserting data: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void register_showPass_CheckedChanged(object sender, EventArgs e)
        {
            if (register_showPass.Checked)
            {
                password_textbox.PasswordChar = '\0';
                confirmpassword_textbox.PasswordChar = '\0';
            }
            else
            {
                password_textbox.PasswordChar = '*';
                confirmpassword_textbox.PasswordChar = '*';
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {
            LoginForm lForm = new LoginForm();
            lForm.Show();
            this.Hide();
        }


        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void RegisterForm_Load(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
