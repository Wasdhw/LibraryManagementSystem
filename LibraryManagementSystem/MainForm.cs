using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LibraryManagementSystem.Utils;
using LibraryManagementSystem.MainformsUser;

namespace LibraryManagementSystem
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            InitializeAutoRefresh();
            InitializeKeyboardShortcuts();
            InitializeTooltips();
        }

        private void InitializeKeyboardShortcuts()
        {
            this.KeyPreview = true;
            this.KeyDown += MainForm_KeyDown;
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            // Ctrl+S for Settings (if Settings button exists)
            if (e.Control && e.KeyCode == Keys.S)
            {
                e.Handled = true;
                // Settings navigation would go here if Settings button is added
            }
            // Esc to close
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
            TooltipHelper.SetTooltip(dashboard_btn, "View dashboard statistics", "Dashboard");
            TooltipHelper.SetTooltip(addBooks_btn, "Add new books to the library", "Add Books");
            TooltipHelper.SetTooltip(avail_btn, "Browse available books", "Books");
            TooltipHelper.SetTooltip(issueBooks_btn, "Issue books to students", "Issue Books");
            TooltipHelper.SetTooltip(returnBooks_btn, "Process book returns", "Return Books");
            TooltipHelper.SetTooltip(button1, "Manage student accounts", "Accounts");
            if (this.Controls.Find("fines_btn", true).Length > 0)
            {
                TooltipHelper.SetTooltip(this.Controls.Find("fines_btn", true)[0], "View and manage fines", "Fines");
            }
            if (this.Controls.Find("reservations_btn", true).Length > 0)
            {
                TooltipHelper.SetTooltip(this.Controls.Find("reservations_btn", true)[0], "View book reservations", "Reservations");
            }
            TooltipHelper.SetTooltip(logout_btn, "Logout from the system", "Logout");
        }

        private void InitializeAutoRefresh()
        {
            // Register refresh callbacks for all DataGridViews
            RefreshServiceManager.RegisterRefresh("books", () =>
            {
                var addBooks = addBooks1 as AddBooks;
                if (addBooks != null && addBooks.Visible)
                {
                    addBooks.refreshData();
                }
            });

            RefreshServiceManager.RegisterRefresh("users", () =>
            {
                var accounts = accounts1 as Accounts;
                if (accounts != null && accounts.Visible)
                {
                    accounts.LoadUsers();
                }
            });

            RefreshServiceManager.RegisterRefresh("issued_books", () =>
            {
                var issueBooks = issueBooks1 as IssueBooks;
                if (issueBooks != null && issueBooks.Visible)
                {
                    issueBooks.refreshData();
                }
            });

            RefreshServiceManager.RegisterRefresh("returned_books", () =>
            {
                var returnBooks = returnBooks1 as ReturnBooks;
                if (returnBooks != null && returnBooks.Visible)
                {
                    returnBooks.refreshData();
                }
            });

            RefreshServiceManager.RegisterRefresh("book_covers", () =>
            {
                var availBooks = availBooks2 as AvailBooks;
                if (availBooks != null && availBooks.Visible)
                {
                    availBooks.refreshData();
                }
            });

            RefreshServiceManager.RegisterRefresh("dashboard", () =>
            {
                var dashboard = dashboard1 as Dashboard;
                if (dashboard != null && dashboard.Visible)
                {
                    dashboard.refreshData();
                }
            });

            RefreshServiceManager.RegisterRefresh("fines", () =>
            {
                var fines = fines1 as Fines;
                if (fines != null && fines.Visible)
                {
                    fines.refreshData();
                }
            });

            // Start the auto-refresh service
            RefreshServiceManager.Start();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void logout_btn_Click(object sender, EventArgs e)
        {
            DialogResult check = MessageBox.Show("Are you sure you want to logout?", "Confirmation Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if(check == DialogResult.Yes)
            {
                // Stop auto-refresh when logging out
                RefreshServiceManager.Stop();
                LoginForm lForm = new LoginForm();
                lForm.Show();
                this.Hide();
            }

        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            // Stop auto-refresh when form closes
            RefreshServiceManager.Stop();
        }

        private void dashboard_btn_Click(object sender, EventArgs e)
        {
            dashboard1.Visible = true;
            addBooks1.Visible = false;
            returnBooks1.Visible = false;
            issueBooks1.Visible = false;
            availBooks2.Visible = false;
            accounts1.Visible = false;
            fines1.Visible = false;
            reservations1.Visible = false;

            Dashboard dForm = dashboard1 as Dashboard;
            if (dForm != null)
            {
                dForm.refreshData();
            }
        }

        private void addBooks_btn_Click(object sender, EventArgs e)
        {
            dashboard1.Visible = false;
            addBooks1.Visible = true;
            returnBooks1.Visible = false;
            issueBooks1.Visible = false;
            availBooks2.Visible = false;
            accounts1.Visible = false;
            fines1.Visible = false;
            reservations1.Visible = false;

            AddBooks aForm = addBooks1 as AddBooks;
            if (aForm != null)
            {
                aForm.refreshData();
            }
        }

        private void issueBooks_btn_Click(object sender, EventArgs e)
        {
            dashboard1.Visible = false;
            addBooks1.Visible = false;
            returnBooks1.Visible = false;
            issueBooks1.Visible = true;
            availBooks2.Visible = false;
            accounts1.Visible = false;
            fines1.Visible = false;
            reservations1.Visible = false;

            IssueBooks iForm = issueBooks1 as IssueBooks;
            if (iForm != null)
            {
                iForm.refreshData();
            }
        }

        private void returnBooks_btn_Click(object sender, EventArgs e)
        {
            dashboard1.Visible = false;
            addBooks1.Visible = false;
            returnBooks1.Visible = true;
            issueBooks1.Visible = false;
            availBooks2.Visible = false;
            accounts1.Visible = false;
            fines1.Visible = false;
            reservations1.Visible = false;

            ReturnBooks rForm = returnBooks1 as ReturnBooks;
            if (rForm != null)
            {
                rForm.refreshData();
            }
        }

        private void accounts_btn_Click(object sender, EventArgs e)
        {
            dashboard1.Visible = false;
            addBooks1.Visible = false;
            returnBooks1.Visible = false;
            issueBooks1.Visible = false;
            availBooks2.Visible = false;
            accounts1.Visible = true;
            fines1.Visible = false;
            reservations1.Visible = false;

            // Trigger a refresh if Accounts exposes a method later
        }

        private void fines_btn_Click(object sender, EventArgs e)
        {
            dashboard1.Visible = false;
            addBooks1.Visible = false;
            returnBooks1.Visible = false;
            issueBooks1.Visible = false;
            availBooks2.Visible = false;
            accounts1.Visible = false;
            fines1.Visible = true;
            reservations1.Visible = false;

            Fines fForm = fines1 as Fines;
            if (fForm != null)
            {
                fForm.refreshData();
            }
        }

        private void reservations_btn_Click(object sender, EventArgs e)
        {
            dashboard1.Visible = false;
            addBooks1.Visible = false;
            returnBooks1.Visible = false;
            issueBooks1.Visible = false;
            availBooks2.Visible = false;
            accounts1.Visible = false;
            fines1.Visible = false;
            reservations1.Visible = true;

            Reservations rForm = reservations1 as Reservations;
            if (rForm != null)
            {
                rForm.refreshData();
            }
        }

        private void Avail_btn_Click(object sender, EventArgs e)
        {
            dashboard1.Visible = false;
            addBooks1.Visible = false;
            returnBooks1.Visible = false;
            issueBooks1.Visible = false;
            availBooks2.Visible = true;
            accounts1.Visible = false;
            fines1.Visible = false;
            reservations1.Visible = false;
            availBooks2.BringToFront();

           AvailBooks iForm = availBooks2 as AvailBooks;
            if (iForm != null)
            {
                iForm.refreshData();
            }
        }

        private void dashboard1_Load(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }


    }
}
