using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LibraryManagementSystem.Utils;

namespace LibraryManagementSystem
{
    public partial class StudentForm : Form
    {
        public StudentForm()
        {
            InitializeComponent();
            InitializeNavigation();
        }

        private void InitializeNavigation()
        {
            // Set initial view to dashboard
            ShowDashboard();
        }

        private void ShowDashboard()
        {
            // Hide all user controls
            stDashboard1.Visible = false;
            stAvailbooks1.Visible = false;
            stReturnBooks1.Visible = false;
            history1.Visible = false;

            // Show dashboard
            stDashboard1.Visible = true;
            stDashboard1.BringToFront();

            // Update button styles
            UpdateButtonStyles("dashboard");
        }

        private void ShowBooks()
        {
            // Hide all user controls
            stDashboard1.Visible = false;
            stAvailbooks1.Visible = false;
            stReturnBooks1.Visible = false;
            history1.Visible = false;

            // Show available books
            stAvailbooks1.Visible = true;
            stAvailbooks1.BringToFront();

            // Update button styles
            UpdateButtonStyles("books");
        }

        private void ShowBorrowed()
        {
            // Hide all user controls
            stDashboard1.Visible = false;
            stAvailbooks1.Visible = false;
            stReturnBooks1.Visible = false;
            history1.Visible = false;

            // Show borrowed books
            stReturnBooks1.Visible = true;
            stReturnBooks1.BringToFront();

            // Load user's borrowed books
            stReturnBooks1.LoadUserBorrowedBooks();

            // Update button styles
            UpdateButtonStyles("borrowed");
        }

        private void ShowHistory()
        {
            // Hide all user controls
            stDashboard1.Visible = false;
            stAvailbooks1.Visible = false;
            stReturnBooks1.Visible = false;
            history1.Visible = false;

            // Show history
            history1.Visible = true;
            history1.BringToFront();

            // Load user's history
            history1.LoadUserHistory();

            // Update button styles
            UpdateButtonStyles("history");
        }

        private void UpdateButtonStyles(string activeButton)
        {
            // Reset all button styles
            dashboard_btn.BackColor = Color.FromArgb(14, 128, 87);
            avail_btn.BackColor = Color.FromArgb(14, 128, 87);
            borrow_btn.BackColor = Color.FromArgb(14, 128, 87);
            history_btn.BackColor = Color.FromArgb(14, 128, 87);

            // Highlight active button
            switch (activeButton)
            {
                case "dashboard":
                    dashboard_btn.BackColor = Color.FromArgb(20, 150, 100);
                    break;
                case "books":
                    avail_btn.BackColor = Color.FromArgb(20, 150, 100);
                    break;
                case "borrowed":
                    borrow_btn.BackColor = Color.FromArgb(20, 150, 100);
                    break;
                case "history":
                    history_btn.BackColor = Color.FromArgb(20, 150, 100);
                    break;
            }
        }

        // Navigation button click events
        private void dashboard_btn_Click(object sender, EventArgs e)
        {
            ShowDashboard();
        }

        private void avail_btn_Click(object sender, EventArgs e)
        {
            ShowBooks();
        }

        private void borrow_btn_Click(object sender, EventArgs e)
        {
            ShowBorrowed();
        }

        private void history_btn_Click(object sender, EventArgs e)
        {
            ShowHistory();
        }

        private void logout_btn_Click(object sender, EventArgs e)
        {
            // Clear session
            SessionManager.ClearSession();

            // Show login form
            LoginForm loginForm = new LoginForm();
            loginForm.Show();

            // Close current form
            this.Close();
        }
    }
}
