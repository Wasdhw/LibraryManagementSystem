using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using LibraryManagementSystem.Utils;

namespace LibraryManagementSystem
{
    public partial class BookInfoForm : Form
    {
        private readonly int _bookId;
        private bool _isStudentForm = false;
        private int _currentUserId = 0;
        private bool _hasReservation = false;
        private int _reservationId = 0;

        public BookInfoForm()
        {
            InitializeComponent();
        }

        public BookInfoForm(int bookId, bool isStudentForm = false)
        {
            InitializeComponent();
            _bookId = bookId;
            _isStudentForm = isStudentForm;
            _currentUserId = SessionManager.CurrentUserId > 0 ? SessionManager.CurrentUserId : 0;
        }

        private async void BookInfoForm_Load(object sender, EventArgs e)
        {
            if (_bookId <= 0)
            {
                return;
            }

            // Show/hide reserve buttons based on form type
            if (_isStudentForm && _currentUserId > 0)
            {
                CheckReservationStatus();
            }
            else
            {
                if (reserveBtn != null) reserveBtn.Visible = false;
                if (cancelReserveBtn != null) cancelReserveBtn.Visible = false;
            }

            try
            {
                using (var con = Utils.Database.GetConnection())
                {
                    con.Open();
                    string sql = "SELECT book_title, author, published_date, quantity, status, image FROM books WHERE id = @id";
                    using (var cmd = new System.Data.SqlClient.SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@id", _bookId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var title = reader["book_title"].ToString();
                                var author = reader["author"].ToString();
                                var published = Convert.ToDateTime(reader["published_date"]).ToString("yyyy-MM-dd");
                                var quantity = Convert.ToInt32(reader["quantity"]);
                                var status = reader["status"].ToString();
                                var imagePath = reader["image"].ToString();

                                Title.Text = title;
                                Author.Text = author;
                                Published.Text = $"Published: {published}";
                                Quantity.Text = $"Quantity: {quantity}";
                                Availability.Text = $"Availability: {status}";

                                // Update button states based on availability
                                if (_isStudentForm && _currentUserId > 0)
                                {
                                    UpdateReservationButtons(status, quantity);
                                }

                                try
                                {
                                    if (!string.IsNullOrEmpty(imagePath))
                                    {
                                        // image column now stores the blob name; download from Azure Blob Storage
                                        var cover = await Utils.BlobCovers.DownloadAsync(imagePath);
                                        pictureBox1.Image = cover;
                                        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        pictureBox1.Image = null;
                                    }
                                }
                                catch
                                {
                                    pictureBox1.Image = null;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load book details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CheckReservationStatus()
        {
            try
            {
                if (_currentUserId <= 0 || _bookId <= 0) return;

                using (var con = Database.GetConnection())
                {
                    con.Open();
                    
                    // Check if reservations table exists
                    string checkTableQuery = @"
                        SELECT COUNT(*) 
                        FROM INFORMATION_SCHEMA.TABLES 
                        WHERE TABLE_NAME = 'reservations'";
                    
                    using (SqlCommand checkCmd = new SqlCommand(checkTableQuery, con))
                    {
                        int tableExists = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (tableExists == 0) return;
                    }

                    string query = @"
                        SELECT id, status 
                        FROM reservations 
                        WHERE user_id = @user_id AND book_id = @book_id 
                        AND status IN ('Pending', 'Notified') 
                        AND date_expires > GETDATE() 
                        AND date_delete IS NULL";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@user_id", _currentUserId);
                        cmd.Parameters.AddWithValue("@book_id", _bookId);
                        
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                _hasReservation = true;
                                _reservationId = Convert.ToInt32(reader["id"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking reservation status: {ex.Message}");
            }
        }

        private void UpdateReservationButtons(string status, int quantity)
        {
            if (reserveBtn == null || cancelReserveBtn == null) return;

            if (_hasReservation)
            {
                reserveBtn.Visible = false;
                cancelReserveBtn.Visible = true;
                cancelReserveBtn.Enabled = true;
            }
            else if (status == "Not Available" || quantity <= 0)
            {
                reserveBtn.Visible = true;
                reserveBtn.Enabled = true;
                cancelReserveBtn.Visible = false;
            }
            else
            {
                reserveBtn.Visible = false;
                cancelReserveBtn.Visible = false;
            }
        }

        private void ReserveBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (_currentUserId <= 0 || _bookId <= 0)
                {
                    MessageBox.Show("Invalid user or book information.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (ReservationManager.CreateReservation(_currentUserId, _bookId))
                {
                    // Get reservation ID for audit log
                    int reservationId = 0;
                    using (var con = Database.GetConnection())
                    {
                        con.Open();
                        string query = "SELECT TOP 1 id FROM reservations WHERE user_id = @user_id AND book_id = @book_id ORDER BY date_reserved DESC";
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@user_id", _currentUserId);
                            cmd.Parameters.AddWithValue("@book_id", _bookId);
                            object result = cmd.ExecuteScalar();
                            if (result != null)
                            {
                                reservationId = Convert.ToInt32(result);
                            }
                        }
                    }

                    AuditLogger.LogReservationCreate(reservationId, _bookId, _currentUserId);
                    MessageBox.Show("Book reserved successfully! You will be notified when it becomes available.", 
                        "Reservation Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    _hasReservation = true;
                    _reservationId = reservationId;
                    UpdateReservationButtons("Not Available", 0);
                }
                else
                {
                    MessageBox.Show("Unable to create reservation. You may already have a reservation for this book.", 
                        "Reservation Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating reservation: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CancelReserveBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (_reservationId <= 0 || _currentUserId <= 0)
                {
                    MessageBox.Show("Invalid reservation information.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (ReservationManager.CancelReservation(_reservationId, _currentUserId))
                {
                    AuditLogger.LogReservationCancel(_reservationId, _currentUserId);
                    MessageBox.Show("Reservation cancelled successfully.", 
                        "Cancellation Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    _hasReservation = false;
                    _reservationId = 0;
                    
                    // Refresh book status to update buttons
                    using (var con = Database.GetConnection())
                    {
                        con.Open();
                        string sql = "SELECT quantity, status FROM books WHERE id = @id";
                        using (SqlCommand cmd = new SqlCommand(sql, con))
                        {
                            cmd.Parameters.AddWithValue("@id", _bookId);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    int quantity = Convert.ToInt32(reader["quantity"]);
                                    string status = reader["status"].ToString();
                                    UpdateReservationButtons(status, quantity);
                                }
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Unable to cancel reservation.", 
                        "Cancellation Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cancelling reservation: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
