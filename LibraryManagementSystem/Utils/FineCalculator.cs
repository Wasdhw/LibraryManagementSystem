using System;
using System.Data.SqlClient;
using LibraryManagementSystem.Utils;

namespace LibraryManagementSystem.Utils
{
    public static class FineCalculator
    {
        private const decimal DefaultFineRatePerDay = 5.00m; // Default fine rate: $5 per day

        /// <summary>
        /// Gets the fine rate per day from settings or returns default
        /// </summary>
        public static decimal GetFineRatePerDay()
        {
            try
            {
                using (var con = Database.GetConnection())
                {
                    con.Open();
                    string query = "SELECT value FROM settings WHERE key = 'fine_rate_per_day'";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null && decimal.TryParse(result.ToString(), out decimal rate))
                        {
                            return rate;
                        }
                    }
                }
            }
            catch
            {
                // If settings table doesn't exist or query fails, return default
            }
            return DefaultFineRatePerDay;
        }

        /// <summary>
        /// Calculates fine amount based on overdue days
        /// </summary>
        /// <param name="daysOverdue">Number of days overdue</param>
        /// <returns>Fine amount</returns>
        public static decimal CalculateFine(int daysOverdue)
        {
            if (daysOverdue <= 0)
                return 0;

            decimal rate = GetFineRatePerDay();
            return daysOverdue * rate;
        }

        /// <summary>
        /// Calculates fine for a specific issue
        /// </summary>
        /// <param name="issueId">Issue ID</param>
        /// <returns>Fine amount and days overdue</returns>
        public static (decimal amount, int daysOverdue) CalculateFineForIssue(string issueId)
        {
            try
            {
                using (var con = Database.GetConnection())
                {
                    con.Open();
                    string query = @"
                        SELECT 
                            CASE 
                                WHEN status = 'Return' AND actual_return_date > return_date 
                                THEN DATEDIFF(day, return_date, actual_return_date)
                                WHEN status = 'Not Return' AND return_date < GETDATE() 
                                THEN DATEDIFF(day, return_date, GETDATE())
                                ELSE 0 
                            END as days_overdue
                        FROM issues 
                        WHERE issue_id = @issue_id AND date_delete IS NULL";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@issue_id", issueId);
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            int daysOverdue = Convert.ToInt32(result);
                            decimal amount = CalculateFine(daysOverdue);
                            return (amount, daysOverdue);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error calculating fine: {ex.Message}");
            }
            return (0, 0);
        }

        /// <summary>
        /// Creates a fine record in the database
        /// </summary>
        public static bool CreateFineRecord(string issueId, int userId, decimal amount, int daysOverdue)
        {
            try
            {
                using (var con = Database.GetConnection())
                {
                    con.Open();
                    
                    // Check if fines table exists
                    string checkTableQuery = @"
                        SELECT COUNT(*) 
                        FROM INFORMATION_SCHEMA.TABLES 
                        WHERE TABLE_NAME = 'fines'";
                    
                    using (SqlCommand checkCmd = new SqlCommand(checkTableQuery, con))
                    {
                        int tableExists = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (tableExists == 0)
                        {
                            System.Diagnostics.Debug.WriteLine("Fines table does not exist. Skipping fine creation.");
                            return false;
                        }
                    }
                    
                    string insertQuery = @"
                        INSERT INTO fines (issue_id, user_id, amount, days_overdue, status, date_created)
                        VALUES (@issue_id, @user_id, @amount, @days_overdue, 'Unpaid', @date_created)";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@issue_id", issueId);
                        cmd.Parameters.AddWithValue("@user_id", userId);
                        cmd.Parameters.AddWithValue("@amount", amount);
                        cmd.Parameters.AddWithValue("@days_overdue", daysOverdue);
                        cmd.Parameters.AddWithValue("@date_created", DateTime.Today);

                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating fine record: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Marks a fine as paid
        /// </summary>
        public static bool MarkFineAsPaid(int fineId)
        {
            try
            {
                using (var con = Database.GetConnection())
                {
                    con.Open();
                    string updateQuery = @"
                        UPDATE fines 
                        SET status = 'Paid', date_paid = @date_paid 
                        WHERE id = @fine_id";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@fine_id", fineId);
                        cmd.Parameters.AddWithValue("@date_paid", DateTime.Today);

                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error marking fine as paid: {ex.Message}");
                return false;
            }
        }
    }
}

