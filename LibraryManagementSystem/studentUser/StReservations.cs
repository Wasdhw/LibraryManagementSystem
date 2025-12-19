using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibraryManagementSystem.Utils;

namespace LibraryManagementSystem.studentUser
{
    public partial class StReservations : UserControl
    {
        SqlConnection connect = Database.GetConnection();
        private int currentUserId = 0;

        public StReservations()
        {
            InitializeComponent();
            if (currentUserId == 0)
            {
                currentUserId = SessionManager.CurrentUserId > 0 ? SessionManager.CurrentUserId : 1;
            }
            LoadReservations();
        }

        public void refreshData()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)refreshData);
                return;
            }
            LoadReservations();
        }

        private void LoadReservations()
        {
            try
            {
                var reservations = ReservationManager.GetUserReservations(currentUserId);
                DisplayReservations(reservations);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading reservations: " + ex.Message, "Error Message", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void DisplayReservations(List<ReservationManager.ReservationInfo> reservations)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => DisplayReservations(reservations)));
                return;
            }

            flowReservations.Controls.Clear();

            foreach (var reservation in reservations)
            {
                await CreateReservationPanel(reservation);
            }
        }

        private async Task CreateReservationPanel(ReservationManager.ReservationInfo reservation)
        {
            // Download image
            Image img = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(reservation.BlobName))
                {
                    img = await BlobCovers.DownloadAsync(reservation.BlobName);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load image: {ex.Message}");
            }

            Panel pnl = new Panel();
            pnl.Width = 200;
            pnl.Height = 300;
            pnl.BackColor = SystemColors.Control;
            pnl.Padding = new Padding(5);
            pnl.Margin = new Padding(10);

            PictureBox pb = new PictureBox();
            pb.Size = new Size(145, 176);
            pb.SizeMode = PictureBoxSizeMode.Zoom;
            pb.Image = img;
            pb.Dock = DockStyle.Top;

            Label lblTitle = new Label();
            lblTitle.Text = reservation.Title;
            lblTitle.AutoSize = false;
            lblTitle.Dock = DockStyle.Bottom;
            lblTitle.Height = 44;
            lblTitle.Font = new Font("Arial", 10, FontStyle.Bold);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            Label lblStatus = new Label();
            lblStatus.Text = $"Status: {reservation.Status}\nExpires: {reservation.DateExpires:MM/dd/yyyy}";
            lblStatus.AutoSize = false;
            lblStatus.Dock = DockStyle.Bottom;
            lblStatus.Height = 40;
            lblStatus.Font = new Font("Arial", 8);
            lblStatus.TextAlign = ContentAlignment.MiddleCenter;

            Button btnCancel = new Button();
            btnCancel.Text = "CANCEL";
            btnCancel.Dock = DockStyle.Bottom;
            btnCancel.Height = 35;
            btnCancel.BackColor = Color.FromArgb(220, 53, 69);
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Tag = reservation.Id;
            btnCancel.Click += BtnCancel_Click;

            pnl.Controls.Add(btnCancel);
            pnl.Controls.Add(lblStatus);
            pnl.Controls.Add(lblTitle);
            pnl.Controls.Add(pb);

            flowReservations.Controls.Add(pnl);
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag != null)
            {
                int reservationId = Convert.ToInt32(btn.Tag);
                CancelReservation(reservationId);
            }
        }

        private void CancelReservation(int reservationId)
        {
            DialogResult check = MessageBox.Show("Cancel this reservation?", "Confirmation", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (check == DialogResult.Yes)
            {
                if (ReservationManager.CancelReservation(reservationId))
                {
                    // Log audit action
                    int userId = SessionManager.CurrentUserId > 0 ? SessionManager.CurrentUserId : 1;
                    AuditLogger.LogReservationCancel(reservationId, userId);

                    MessageBox.Show("Reservation cancelled successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadReservations();
                }
                else
                {
                    MessageBox.Show("Error cancelling reservation.", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}

