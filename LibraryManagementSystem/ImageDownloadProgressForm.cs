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
    public partial class ImageDownloadProgressForm : Form
    {
        private ProgressBar progressBar;
        private Label statusLabel;
        private Label percentageLabel;

        public ImageDownloadProgressForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties
            this.Text = "Loading Book Covers";
            this.Size = new Size(450, 180);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ControlBox = false; // Prevent closing during download
            this.BackColor = Color.White;

            // Status label
            statusLabel = new Label();
            statusLabel.Text = "Preparing to download book covers...";
            statusLabel.Location = new Point(20, 20);
            statusLabel.Size = new Size(400, 25);
            statusLabel.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            statusLabel.ForeColor = Color.Black;
            this.Controls.Add(statusLabel);

            // Progress bar
            progressBar = new ProgressBar();
            progressBar.Location = new Point(20, 60);
            progressBar.Size = new Size(400, 30);
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            progressBar.Value = 0;
            this.Controls.Add(progressBar);

            // Percentage label
            percentageLabel = new Label();
            percentageLabel.Text = "0%";
            percentageLabel.Location = new Point(20, 100);
            percentageLabel.Size = new Size(400, 25);
            percentageLabel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            percentageLabel.ForeColor = Color.FromArgb(0, 120, 215);
            percentageLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(percentageLabel);

            this.ResumeLayout(false);
        }

        public void UpdateProgress(int current, int total, string currentBook = "")
        {
            if (InvokeRequired)
            {
                Invoke(new Action<int, int, string>(UpdateProgress), current, total, currentBook);
                return;
            }

            int percentage = total > 0 ? (int)((current * 100.0) / total) : 0;
            progressBar.Value = Math.Min(percentage, 100);
            percentageLabel.Text = $"{percentage}%";
            
            if (!string.IsNullOrEmpty(currentBook))
            {
                statusLabel.Text = $"Downloading: {currentBook}...";
            }
            else
            {
                statusLabel.Text = $"Downloading book covers... ({current} of {total})";
            }

            Application.DoEvents(); // Update UI immediately
        }

        public void SetComplete()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(SetComplete));
                return;
            }

            progressBar.Value = 100;
            percentageLabel.Text = "100%";
            statusLabel.Text = "Download complete!";
            Application.DoEvents();
        }
    }
}

