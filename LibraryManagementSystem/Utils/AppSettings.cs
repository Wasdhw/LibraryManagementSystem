using System;
using System.Data.SqlClient;
using LibraryManagementSystem.Utils;

namespace LibraryManagementSystem.Utils
{
    public static class AppSettings
    {
        // Default values
        private const int DefaultBorrowingPeriodDays = 14;
        private const int DefaultMaxBooksPerUser = 5;
        private const int DefaultOverdueThresholdDays = 3;
        private const decimal DefaultFineRatePerDay = 5.00m;
        private const int DefaultMaxRenewals = 2;
        private const int DefaultRenewalDays = 14;

        /// <summary>
        /// Gets a setting value or returns default
        /// </summary>
        public static T GetSetting<T>(string key, T defaultValue)
        {
            try
            {
                using (var con = Database.GetConnection())
                {
                    con.Open();
                    string query = "SELECT value FROM settings WHERE key = @key";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@key", key);
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            return (T)Convert.ChangeType(result, typeof(T));
                        }
                    }
                }
            }
            catch
            {
                // If settings table doesn't exist, return default
            }
            return defaultValue;
        }

        /// <summary>
        /// Sets a setting value
        /// </summary>
        public static bool SetSetting(string key, string value, string description = "")
        {
            try
            {
                using (var con = Database.GetConnection())
                {
                    con.Open();
                    string query = @"
                        IF EXISTS (SELECT 1 FROM settings WHERE key = @key)
                            UPDATE settings SET value = @value, description = @description WHERE key = @key
                        ELSE
                            INSERT INTO settings (key, value, description) VALUES (@key, @value, @description)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@key", key);
                        cmd.Parameters.AddWithValue("@value", value);
                        cmd.Parameters.AddWithValue("@description", description);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting setting: {ex.Message}");
                return false;
            }
        }

        // Convenience methods for common settings
        public static int GetBorrowingPeriodDays() => GetSetting("borrowing_period_days", DefaultBorrowingPeriodDays);
        public static bool SetBorrowingPeriodDays(int days) => SetSetting("borrowing_period_days", days.ToString(), "Number of days a book can be borrowed");

        public static int GetMaxBooksPerUser() => GetSetting("max_books_per_user", DefaultMaxBooksPerUser);
        public static bool SetMaxBooksPerUser(int max) => SetSetting("max_books_per_user", max.ToString(), "Maximum number of books a user can borrow at once");

        public static int GetOverdueThresholdDays() => GetSetting("overdue_threshold_days", DefaultOverdueThresholdDays);
        public static bool SetOverdueThresholdDays(int days) => SetSetting("overdue_threshold_days", days.ToString(), "Days after return date before book is considered overdue");

        public static decimal GetFineRatePerDay() => GetSetting("fine_rate_per_day", DefaultFineRatePerDay);
        public static bool SetFineRatePerDay(decimal rate) => SetSetting("fine_rate_per_day", rate.ToString(), "Fine amount per day for overdue books");

        public static int GetMaxRenewals() => GetSetting("max_renewals", DefaultMaxRenewals);
        public static bool SetMaxRenewals(int max) => SetSetting("max_renewals", max.ToString(), "Maximum number of times a book can be renewed");

        public static int GetRenewalDays() => GetSetting("renewal_days", DefaultRenewalDays);
        public static bool SetRenewalDays(int days) => SetSetting("renewal_days", days.ToString(), "Number of days added when a book is renewed");
    }
}

