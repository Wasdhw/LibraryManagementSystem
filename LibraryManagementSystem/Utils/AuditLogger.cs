using System;
using System.Data.SqlClient;
using LibraryManagementSystem.Utils;

namespace LibraryManagementSystem.Utils
{
    public static class AuditLogger
    {
        /// <summary>
        /// Logs an action to the audit log
        /// </summary>
        public static void LogAction(string action, string entityType, int? entityId = null, string details = "")
        {
            try
            {
                int userId = SessionManager.CurrentUserId > 0 ? SessionManager.CurrentUserId : 0;
                LogAction(userId, action, entityType, entityId, details);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error logging action: {ex.Message}");
            }
        }

        /// <summary>
        /// Logs an action to the audit log with specific user ID
        /// </summary>
        public static void LogAction(int userId, string action, string entityType, int? entityId = null, string details = "")
        {
            try
            {
                using (var con = Database.GetConnection())
                {
                    con.Open();
                    
                    // Check if audit_logs table exists
                    string checkTableQuery = @"
                        SELECT COUNT(*) 
                        FROM INFORMATION_SCHEMA.TABLES 
                        WHERE TABLE_NAME = 'audit_logs'";
                    
                    using (SqlCommand checkCmd = new SqlCommand(checkTableQuery, con))
                    {
                        int tableExists = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (tableExists == 0)
                        {
                            System.Diagnostics.Debug.WriteLine("Audit_logs table does not exist. Skipping audit log.");
                            return;
                        }
                    }
                    
                    string query = @"
                        INSERT INTO audit_logs (user_id, action, entity_type, entity_id, details, timestamp)
                        VALUES (@user_id, @action, @entity_type, @entity_id, @details, @timestamp)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@user_id", userId);
                        cmd.Parameters.AddWithValue("@action", action);
                        cmd.Parameters.AddWithValue("@entity_type", entityType);
                        cmd.Parameters.AddWithValue("@entity_id", entityId.HasValue ? (object)entityId.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@details", details ?? "");
                        cmd.Parameters.AddWithValue("@timestamp", DateTime.Now);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error logging audit action: {ex.Message}");
            }
        }

        // Convenience methods for common actions
        public static void LogBookIssue(string issueId, int bookId, int userId) =>
            LogAction(userId, "Issue Book", "Issue", null, $"Issue ID: {issueId}, Book ID: {bookId}");

        public static void LogBookReturn(string issueId, int bookId, int userId) =>
            LogAction(userId, "Return Book", "Issue", null, $"Issue ID: {issueId}, Book ID: {bookId}");

        public static void LogBookAdd(int bookId) =>
            LogAction("Add Book", "Book", bookId);

        public static void LogBookUpdate(int bookId) =>
            LogAction("Update Book", "Book", bookId);

        public static void LogBookDelete(int bookId) =>
            LogAction("Delete Book", "Book", bookId);

        public static void LogUserAdd(int userId) =>
            LogAction("Add User", "User", userId);

        public static void LogUserUpdate(int userId) =>
            LogAction("Update User", "User", userId);

        public static void LogUserDelete(int userId) =>
            LogAction("Delete User", "User", userId);

        public static void LogFinePaid(int fineId, int userId) =>
            LogAction(userId, "Pay Fine", "Fine", fineId);

        public static void LogBookRenewal(string issueId, int bookId, int userId) =>
            LogAction(userId, "Renew Book", "Issue", null, $"Issue ID: {issueId}, Book ID: {bookId}");

        public static void LogReservationCreate(int reservationId, int bookId, int userId) =>
            LogAction(userId, "Create Reservation", "Reservation", reservationId, $"Book ID: {bookId}");

        public static void LogReservationCancel(int reservationId, int userId) =>
            LogAction(userId, "Cancel Reservation", "Reservation", reservationId);
    }
}

