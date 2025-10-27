using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using LibraryManagementSystem.Utils;

namespace LibraryManagementSystem.studentUser
{
    public partial class PasswordChangeForm : Form
    {
        private SqlConnection connect = Database.GetConnection();
        private int currentUserId;

        public PasswordChangeForm(int userId)
        {
            InitializeComponent();
            currentUserId = userId;
        }

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.lblCurrentPassword = new Label();
            this.txtCurrentPassword = new TextBox();
            this.lblNewPassword = new Label();
            this.txtNewPassword = new TextBox();
            this.lblConfirmPassword = new Label();
            this.txtConfirmPassword = new TextBox();
            this.btnChangePassword = new Button();
            this.btnCancel = new Button();
            this.chkShowPasswords = new CheckBox();
            this.SuspendLayout();

            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(200, 23);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Change Password";
            // 
            // lblCurrentPassword
            // 
            this.lblCurrentPassword.AutoSize = true;
            this.lblCurrentPassword.Font = new System.Drawing.Font("Tahoma", 10F);
            this.lblCurrentPassword.Location = new System.Drawing.Point(20, 70);
            this.lblCurrentPassword.Name = "lblCurrentPassword";
            this.lblCurrentPassword.Size = new System.Drawing.Size(120, 17);
            this.lblCurrentPassword.TabIndex = 1;
            this.lblCurrentPassword.Text = "Current Password:";
            // 
            // txtCurrentPassword
            // 
            this.txtCurrentPassword.Font = new System.Drawing.Font("Tahoma", 10F);
            this.txtCurrentPassword.Location = new System.Drawing.Point(20, 90);
            this.txtCurrentPassword.Name = "txtCurrentPassword";
            this.txtCurrentPassword.PasswordChar = '*';
            this.txtCurrentPassword.Size = new System.Drawing.Size(300, 24);
            this.txtCurrentPassword.TabIndex = 2;
            // 
            // lblNewPassword
            // 
            this.lblNewPassword.AutoSize = true;
            this.lblNewPassword.Font = new System.Drawing.Font("Tahoma", 10F);
            this.lblNewPassword.Location = new System.Drawing.Point(20, 130);
            this.lblNewPassword.Name = "lblNewPassword";
            this.lblNewPassword.Size = new System.Drawing.Size(100, 17);
            this.lblNewPassword.TabIndex = 3;
            this.lblNewPassword.Text = "New Password:";
            // 
            // txtNewPassword
            // 
            this.txtNewPassword.Font = new System.Drawing.Font("Tahoma", 10F);
            this.txtNewPassword.Location = new System.Drawing.Point(20, 150);
            this.txtNewPassword.Name = "txtNewPassword";
            this.txtNewPassword.PasswordChar = '*';
            this.txtNewPassword.Size = new System.Drawing.Size(300, 24);
            this.txtNewPassword.TabIndex = 4;
            // 
            // lblConfirmPassword
            // 
            this.lblConfirmPassword.AutoSize = true;
            this.lblConfirmPassword.Font = new System.Drawing.Font("Tahoma", 10F);
            this.lblConfirmPassword.Location = new System.Drawing.Point(20, 190);
            this.lblConfirmPassword.Name = "lblConfirmPassword";
            this.lblConfirmPassword.Size = new System.Drawing.Size(120, 17);
            this.lblConfirmPassword.TabIndex = 5;
            this.lblConfirmPassword.Text = "Confirm Password:";
            // 
            // txtConfirmPassword
            // 
            this.txtConfirmPassword.Font = new System.Drawing.Font("Tahoma", 10F);
            this.txtConfirmPassword.Location = new System.Drawing.Point(20, 210);
            this.txtConfirmPassword.Name = "txtConfirmPassword";
            this.txtConfirmPassword.PasswordChar = '*';
            this.txtConfirmPassword.Size = new System.Drawing.Size(300, 24);
            this.txtConfirmPassword.TabIndex = 6;
            // 
            // chkShowPasswords
            // 
            this.chkShowPasswords.AutoSize = true;
            this.chkShowPasswords.Font = new System.Drawing.Font("Tahoma", 9F);
            this.chkShowPasswords.Location = new System.Drawing.Point(20, 250);
            this.chkShowPasswords.Name = "chkShowPasswords";
            this.chkShowPasswords.Size = new System.Drawing.Size(120, 19);
            this.chkShowPasswords.TabIndex = 7;
            this.chkShowPasswords.Text = "Show Passwords";
            this.chkShowPasswords.UseVisualStyleBackColor = true;
            this.chkShowPasswords.CheckedChanged += new EventHandler(this.chkShowPasswords_CheckedChanged);
            // 
            // btnChangePassword
            // 
            this.btnChangePassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(128)))), ((int)(((byte)(87)))));
            this.btnChangePassword.FlatAppearance.BorderSize = 0;
            this.btnChangePassword.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChangePassword.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.btnChangePassword.ForeColor = System.Drawing.Color.White;
            this.btnChangePassword.Location = new System.Drawing.Point(20, 290);
            this.btnChangePassword.Name = "btnChangePassword";
            this.btnChangePassword.Size = new System.Drawing.Size(140, 35);
            this.btnChangePassword.TabIndex = 8;
            this.btnChangePassword.Text = "Change Password";
            this.btnChangePassword.UseVisualStyleBackColor = false;
            this.btnChangePassword.Click += new EventHandler(this.btnChangePassword_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Tahoma", 10F);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(180, 290);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(140, 35);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
            // 
            // PasswordChangeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(350, 350);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnChangePassword);
            this.Controls.Add(this.chkShowPasswords);
            this.Controls.Add(this.txtConfirmPassword);
            this.Controls.Add(this.lblConfirmPassword);
            this.Controls.Add(this.txtNewPassword);
            this.Controls.Add(this.lblNewPassword);
            this.Controls.Add(this.txtCurrentPassword);
            this.Controls.Add(this.lblCurrentPassword);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PasswordChangeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Change Password";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private Label lblTitle;
        private Label lblCurrentPassword;
        private TextBox txtCurrentPassword;
        private Label lblNewPassword;
        private TextBox txtNewPassword;
        private Label lblConfirmPassword;
        private TextBox txtConfirmPassword;
        private CheckBox chkShowPasswords;
        private Button btnChangePassword;
        private Button btnCancel;

        private void chkShowPasswords_CheckedChanged(object sender, EventArgs e)
        {
            txtCurrentPassword.PasswordChar = chkShowPasswords.Checked ? '\0' : '*';
            txtNewPassword.PasswordChar = chkShowPasswords.Checked ? '\0' : '*';
            txtConfirmPassword.PasswordChar = chkShowPasswords.Checked ? '\0' : '*';
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(txtCurrentPassword.Text))
            {
                MessageBox.Show("Please enter your current password.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCurrentPassword.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNewPassword.Text))
            {
                MessageBox.Show("Please enter a new password.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNewPassword.Focus();
                return;
            }

            if (txtNewPassword.Text.Length < 6)
            {
                MessageBox.Show("New password must be at least 6 characters long.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNewPassword.Focus();
                return;
            }

            if (txtNewPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("New password and confirmation do not match.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtConfirmPassword.Focus();
                return;
            }

            if (txtCurrentPassword.Text == txtNewPassword.Text)
            {
                MessageBox.Show("New password must be different from current password.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNewPassword.Focus();
                return;
            }

            // Verify current password and update
            if (ChangePassword(txtCurrentPassword.Text, txtNewPassword.Text))
            {
                MessageBox.Show("Password changed successfully!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Failed to change password. Please check your current password.", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtCurrentPassword.Focus();
            }
        }

        private bool ChangePassword(string currentPassword, string newPassword)
        {
            try
            {
                if (connect.State == ConnectionState.Closed)
                {
                    connect.Open();
                }

                // First, verify the current password
                string verifyQuery = "SELECT password FROM users WHERE id = @userId AND is_active = 1 AND date_delete IS NULL";
                using (SqlCommand verifyCmd = new SqlCommand(verifyQuery, connect))
                {
                    verifyCmd.Parameters.AddWithValue("@userId", currentUserId);
                    string storedHash = verifyCmd.ExecuteScalar()?.ToString();

                    if (string.IsNullOrEmpty(storedHash))
                    {
                        return false;
                    }

                    // Verify current password
                    if (!Security.VerifyPassword(currentPassword, storedHash))
                    {
                        return false;
                    }
                }

                // Update password
                string updateQuery = "UPDATE users SET password = @newPassword, date_update = @dateUpdate WHERE id = @userId";
                using (SqlCommand updateCmd = new SqlCommand(updateQuery, connect))
                {
                    updateCmd.Parameters.AddWithValue("@newPassword", Security.HashPassword(newPassword));
                    updateCmd.Parameters.AddWithValue("@dateUpdate", DateTime.Now);
                    updateCmd.Parameters.AddWithValue("@userId", currentUserId);

                    int rowsAffected = updateCmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error changing password: " + ex.Message, "Database Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                if (connect.State == ConnectionState.Open)
                {
                    connect.Close();
                }
            }
        }
    }
}
