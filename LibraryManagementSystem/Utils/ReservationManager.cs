using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using LibraryManagementSystem.Utils;

namespace LibraryManagementSystem.Utils
{
    public static class ReservationManager
    {
        private const int ReservationExpiryDays = 7;

        /// <summary>
        /// Creates a reservation for a book
        /// </summary>
        public static bool CreateReservation(int userId, int bookId)
        {
            try
            {
                using (var con = Database.GetConnection())
                {
                    con.Open();
                    
                    // Check if reservations table exists
                    string checkTableQuery = @"
                        SELECT COUNT(*) 
                        FROM INFORMATION_SCHEMA.TABLES 
                        WHERE TABLE_NAME = 'reservations'";
                    
                    using (SqlCommand checkTableCmd = new SqlCommand(checkTableQuery, con))
                    {
                        int tableExists = Convert.ToInt32(checkTableCmd.ExecuteScalar());
                        if (tableExists == 0)
                        {
                            System.Diagnostics.Debug.WriteLine("Reservations table does not exist. Skipping reservation creation.");
                            return false;
                        }
                    }
                    
                    // Check if user already has a reservation for this book
                    string checkQuery = @"
                        SELECT COUNT(*) 
                        FROM reservations 
                        WHERE user_id = @user_id AND book_id = @book_id 
                        AND status = 'Pending' AND date_expires > GETDATE()";

                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                    {
                        checkCmd.Parameters.AddWithValue("@user_id", userId);
                        checkCmd.Parameters.AddWithValue("@book_id", bookId);
                        int existingReservations = Convert.ToInt32(checkCmd.ExecuteScalar());
                        
                        if (existingReservations > 0)
                        {
                            return false; // Already has a reservation
                        }
                    }

                    // Create reservation
                    string insertQuery = @"
                        INSERT INTO reservations (user_id, book_id, status, date_reserved, date_expires)
                        VALUES (@user_id, @book_id, 'Pending', @date_reserved, @date_expires)";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@user_id", userId);
                        cmd.Parameters.AddWithValue("@book_id", bookId);
                        cmd.Parameters.AddWithValue("@date_reserved", DateTime.Today);
                        cmd.Parameters.AddWithValue("@date_expires", DateTime.Today.AddDays(ReservationExpiryDays));

                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating reservation: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets pending reservations for a user
        /// </summary>
        public static List<ReservationInfo> GetUserReservations(int userId)
        {
            var reservations = new List<ReservationInfo>();
            try
            {
                using (var con = Database.GetConnection())
                {
                    con.Open();
                    string query = @"
                        SELECT 
                            r.id,
                            r.book_id,
                            b.book_title,
                            b.author,
                            b.image,
                            r.date_reserved,
                            r.date_expires,
                            r.status
                        FROM reservations r
                        INNER JOIN books b ON r.book_id = b.id
                        WHERE r.user_id = @user_id 
                        AND r.status = 'Pending' 
                        AND r.date_expires > GETDATE()
                        ORDER BY r.date_reserved ASC";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@user_id", userId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                reservations.Add(new ReservationInfo
                                {
                                    Id = Convert.ToInt32(reader["id"]),
                                    BookId = Convert.ToInt32(reader["book_id"]),
                                    Title = reader["book_title"].ToString(),
                                    Author = reader["author"].ToString(),
                                    BlobName = reader["image"]?.ToString(),
                                    DateReserved = Convert.ToDateTime(reader["date_reserved"]),
                                    DateExpires = Convert.ToDateTime(reader["date_expires"]),
                                    Status = reader["status"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting reservations: {ex.Message}");
            }
            return reservations;
        }

        /// <summary>
        /// Checks if a book becomes available and notifies first in queue
        /// </summary>
        public static void CheckAndNotifyAvailableReservations(int bookId)
        {
            try
            {
                using (var con = Database.GetConnection())
                {
                    con.Open();
                    
                    // Get first pending reservation for this book
                    string query = @"
                        SELECT TOP 1 id, user_id 
                        FROM reservations 
                        WHERE book_id = @book_id 
                        AND status = 'Pending' 
                        AND date_expires > GETDATE()
                        ORDER BY date_reserved ASC";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@book_id", bookId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int reservationId = Convert.ToInt32(reader["id"]);
                                int userId = Convert.ToInt32(reader["user_id"]);
                                
                                // Mark as notified
                                string updateQuery = @"
                                    UPDATE reservations 
                                    SET status = 'Notified', date_notified = @date_notified 
                                    WHERE id = @reservation_id";

                                using (SqlCommand updateCmd = new SqlCommand(updateQuery, con))
                                {
                                    updateCmd.Parameters.AddWithValue("@reservation_id", reservationId);
                                    updateCmd.Parameters.AddWithValue("@date_notified", DateTime.Today);
                                    updateCmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking reservations: {ex.Message}");
            }
        }

        /// <summary>
        /// Cancels a reservation
        /// </summary>
        public static bool CancelReservation(int reservationId)
        {
            try
            {
                using (var con = Database.GetConnection())
                {
                    con.Open();
                    string updateQuery = "UPDATE reservations SET status = 'Cancelled' WHERE id = @id";
                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@id", reservationId);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cancelling reservation: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Cancels a reservation with user validation
        /// </summary>
        public static bool CancelReservation(int reservationId, int userId)
        {
            try
            {
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
                        if (tableExists == 0) return false;
                    }
                    
                    // Verify the reservation belongs to the user
                    string verifyQuery = "SELECT COUNT(*) FROM reservations WHERE id = @id AND user_id = @user_id";
                    using (SqlCommand verifyCmd = new SqlCommand(verifyQuery, con))
                    {
                        verifyCmd.Parameters.AddWithValue("@id", reservationId);
                        verifyCmd.Parameters.AddWithValue("@user_id", userId);
                        int count = Convert.ToInt32(verifyCmd.ExecuteScalar());
                        if (count == 0) return false; // Reservation doesn't belong to user
                    }
                    
                    string updateQuery = "UPDATE reservations SET status = 'Cancelled' WHERE id = @id AND user_id = @user_id";
                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@id", reservationId);
                        cmd.Parameters.AddWithValue("@user_id", userId);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cancelling reservation: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets reservation count for a book
        /// </summary>
        public static int GetReservationCount(int bookId)
        {
            try
            {
                using (var con = Database.GetConnection())
                {
                    con.Open();
                    string query = @"
                        SELECT COUNT(*) 
                        FROM reservations 
                        WHERE book_id = @book_id 
                        AND status IN ('Pending', 'Notified') 
                        AND date_expires > GETDATE()";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@book_id", bookId);
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        public class ReservationInfo
        {
            public int Id { get; set; }
            public int BookId { get; set; }
            public string Title { get; set; }
            public string Author { get; set; }
            public string BlobName { get; set; }
            public DateTime DateReserved { get; set; }
            public DateTime DateExpires { get; set; }
            public string Status { get; set; }
        }
    }
}

