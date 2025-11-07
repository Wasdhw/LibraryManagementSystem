using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryManagementSystem
{
    public partial class OverdueNotificationForm : Form
    {
        private Label titleLabel;
        private Label messageLabel;
        private Button closeButton;
        private Panel contentPanel;
        private ListBox overdueListBox;

        public OverdueNotificationForm(string title, string message, List<string> overdueBooks = null)
        {
            InitializeComponent(title, message, overdueBooks);
        }

        private void InitializeComponent(string title, string message, List<string> overdueBooks)
        {
            this.SuspendLayout();

            // Form properties
            this.Text = title;
            this.Size = new Size(600, 400);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(255, 245, 245);
            this.TopMost = true;

            // Title label
            titleLabel = new Label();
            titleLabel.Text = title;
            titleLabel.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            titleLabel.ForeColor = Color.DarkRed;
            titleLabel.Location = new Point(20, 20);
            titleLabel.Size = new Size(560, 30);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(titleLabel);

            // Message label
            messageLabel = new Label();
            messageLabel.Text = message;
            messageLabel.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            messageLabel.ForeColor = Color.Black;
            messageLabel.Location = new Point(20, 60);
            messageLabel.Size = new Size(560, 40);
            messageLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(messageLabel);

            // Content panel for overdue books list
            contentPanel = new Panel();
            contentPanel.Location = new Point(20, 110);
            contentPanel.Size = new Size(560, 220);
            contentPanel.BackColor = Color.White;
            contentPanel.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(contentPanel);

            // Overdue books list
            if (overdueBooks != null && overdueBooks.Count > 0)
            {
                overdueListBox = new ListBox();
                overdueListBox.Location = new Point(10, 10);
                overdueListBox.Size = new Size(540, 200);
                overdueListBox.Font = new Font("Segoe UI", 9, FontStyle.Regular);
                overdueListBox.BackColor = Color.White;
                overdueListBox.ForeColor = Color.DarkRed;
                overdueListBox.BorderStyle = BorderStyle.None;
                
                foreach (string book in overdueBooks)
                {
                    overdueListBox.Items.Add(book);
                }
                
                contentPanel.Controls.Add(overdueListBox);
            }
            else
            {
                Label noBooksLabel = new Label();
                noBooksLabel.Text = "No overdue books at this time.";
                noBooksLabel.Font = new Font("Segoe UI", 10, FontStyle.Italic);
                noBooksLabel.ForeColor = Color.Gray;
                noBooksLabel.Location = new Point(10, 100);
                noBooksLabel.Size = new Size(540, 20);
                noBooksLabel.TextAlign = ContentAlignment.MiddleCenter;
                contentPanel.Controls.Add(noBooksLabel);
            }

            // Close button
            closeButton = new Button();
            closeButton.Text = "OK";
            closeButton.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            closeButton.BackColor = Color.FromArgb(220, 53, 69);
            closeButton.ForeColor = Color.White;
            closeButton.FlatStyle = FlatStyle.Flat;
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.Size = new Size(100, 35);
            closeButton.Location = new Point(250, 340);
            closeButton.Click += CloseButton_Click;
            this.Controls.Add(closeButton);

            this.ResumeLayout(false);
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

