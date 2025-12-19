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
            InitializeAutoRefresh();
            InitializeKeyboardShortcuts();
            InitializeTooltips();
            
            // Check for overdue books when student form loads
            CheckStudentOverdueBooksOnLoad();
        }

        private void InitializeKeyboardShortcuts()
        {
            this.KeyPreview = true;
            this.KeyDown += StudentForm_KeyDown;
        }

        private void StudentForm_KeyDown(object sender, KeyEventArgs e)
        {
            // Esc to logout
            if (e.KeyCode == Keys.Escape)
            {
                DialogResult result = MessageBox.Show("Do you want to logout?", "Confirm", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    logout_btn_Click(sender, e);
                }
            }
        }

        private void InitializeTooltips()
        {
            TooltipHelper.SetTooltip(dashboard_btn, "View your dashboard", "Dashboard");
            TooltipHelper.SetTooltip(avail_btn, "Browse available books", "Books");
            if (this.Controls.Find("renew_btn", true).Length > 0)
            {
                TooltipHelper.SetTooltip(this.Controls.Find("renew_btn", true)[0], "Renew borrowed books", "Renew");
            }
            if (this.Controls.Find("borrow_btn", true).Length > 0)
            {
                TooltipHelper.SetTooltip(this.Controls.Find("borrow_btn", true)[0], "View borrowed books", "Borrowed");
            }
            TooltipHelper.SetTooltip(history_btn, "View borrowing history", "History");
            TooltipHelper.SetTooltip(logout_btn, "Logout from the system", "Logout");
        }

        private void CheckStudentOverdueBooksOnLoad()
        {
            try
            {
                int userId = SessionManager.CurrentUserId;
                if (userId > 0)
                {
                    // Load borrowed books to trigger overdue check
                    var returnBooks = stReturnBooks1 as studentUser.StReturnBooks;
                    if (returnBooks != null)
                    {
                        returnBooks.LoadUserBorrowedBooks(userId);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking overdue books on load: {ex.Message}");
            }
        }

        private void InitializeAutoRefresh()
        {
            // Register refresh callbacks for student views
            RefreshServiceManager.RegisterRefresh("book_covers", () =>
            {
                var availBooks = stAvailbooks1 as studentUser.StAvailbooks;
                if (availBooks != null && availBooks.Visible)
                {
                    availBooks.refreshData();
                }
            });

            RefreshServiceManager.RegisterRefresh("issued_books", () =>
            {
                var returnBooks = stReturnBooks1 as studentUser.StReturnBooks;
                if (returnBooks != null && returnBooks.Visible)
                {
                    returnBooks.LoadUserBorrowedBooks();
                }
            });

            RefreshServiceManager.RegisterRefresh("returned_books", () =>
            {
                var history = history1 as studentUser.History;
                if (history != null && history.Visible)
                {
                    history.LoadUserHistory();
                }
            });

            // Start the auto-refresh service
            RefreshServiceManager.Start();
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
            stRenewBooks1.Visible = false;

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
            stRenewBooks1.Visible = false;

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
            stRenewBooks1.Visible = false;

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
            stRenewBooks1.Visible = false;

            // Show history
            history1.Visible = true;
            history1.BringToFront();

            // Load user's history
            history1.LoadUserHistory();

            // Update button styles
            UpdateButtonStyles("history");
        }

        private void ShowRenew()
        {
            // Hide all user controls
            stDashboard1.Visible = false;
            stAvailbooks1.Visible = false;
            stReturnBooks1.Visible = false;
            history1.Visible = false;
            stRenewBooks1.Visible = false;

            // Show renew books
            stRenewBooks1.Visible = true;
            stRenewBooks1.BringToFront();

            // Refresh renew books
            stRenewBooks1.refreshData();

            // Update button styles
            UpdateButtonStyles("renew");
        }

        private void UpdateButtonStyles(string activeButton)
        {
            // Reset all button styles
            dashboard_btn.BackColor = Color.FromArgb(14, 128, 87);
            avail_btn.BackColor = Color.FromArgb(14, 128, 87);
            borrow_btn.BackColor = Color.FromArgb(14, 128, 87);
            history_btn.BackColor = Color.FromArgb(14, 128, 87);
            renew_btn.BackColor = Color.FromArgb(14, 128, 87);

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
                case "renew":
                    renew_btn.BackColor = Color.FromArgb(20, 150, 100);
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

        private void renew_btn_Click(object sender, EventArgs e)
        {
            ShowRenew();
        }

        private void logout_btn_Click(object sender, EventArgs e)
        {
            // Clear session
            SessionManager.ClearSession();

            // Stop auto-refresh when logging out
            RefreshServiceManager.Stop();

            // Show login form
            LoginForm loginForm = new LoginForm();
            loginForm.Show();

            // Close current form
            this.Close();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            // Stop auto-refresh when form closes
            RefreshServiceManager.Stop();
        }
    }
}
