using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using LibraryManagementSystem.Utils;

namespace LibraryManagementSystem.Utils
{
    public static class ReportGenerator
    {
        public class MonthlyStats
        {
            public int Month { get; set; }
            public int Year { get; set; }
            public int BooksIssued { get; set; }
            public int BooksReturned { get; set; }
            public int OverdueBooks { get; set; }
            public decimal TotalFines { get; set; }
        }

        public class PopularBook
        {
            public int BookId { get; set; }
            public string Title { get; set; }
            public string Author { get; set; }
            public int IssueCount { get; set; }
        }

        /// <summary>
        /// Gets monthly statistics for a given year
        /// </summary>
        public static List<MonthlyStats> GetMonthlyStatistics(int year)
        {
            var stats = new List<MonthlyStats>();
            try
            {
                using (var con = Database.GetConnection())
                {
                    con.Open();
                    string query = @"
                        SELECT 
                            MONTH(issue_date) as Month,
                            YEAR(issue_date) as Year,
                            COUNT(*) as BooksIssued
                        FROM issues
                        WHERE YEAR(issue_date) = @year AND date_delete IS NULL
                        GROUP BY MONTH(issue_date), YEAR(issue_date)
                        ORDER BY Month";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@year", year);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int month = Convert.ToInt32(reader["Month"]);
                                stats.Add(new MonthlyStats
                                {
                                    Month = month,
                                    Year = year,
                                    BooksIssued = Convert.ToInt32(reader["BooksIssued"])
                                });
                            }
                        }
                    }

                    // Get returned books count
                    foreach (var stat in stats)
                    {
                        string returnQuery = @"
                            SELECT COUNT(*) 
                            FROM issues 
                            WHERE MONTH(actual_return_date) = @month 
                            AND YEAR(actual_return_date) = @year 
                            AND status = 'Return' 
                            AND date_delete IS NULL";

                        using (SqlCommand cmd = new SqlCommand(returnQuery, con))
                        {
                            cmd.Parameters.AddWithValue("@month", stat.Month);
                            cmd.Parameters.AddWithValue("@year", year);
                            stat.BooksReturned = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        // Get overdue count
                        string overdueQuery = @"
                            SELECT COUNT(*) 
                            FROM issues 
                            WHERE MONTH(return_date) = @month 
                            AND YEAR(return_date) = @year 
                            AND status = 'Not Return' 
                            AND return_date < GETDATE() 
                            AND date_delete IS NULL";

                        using (SqlCommand cmd = new SqlCommand(overdueQuery, con))
                        {
                            cmd.Parameters.AddWithValue("@month", stat.Month);
                            cmd.Parameters.AddWithValue("@year", year);
                            stat.OverdueBooks = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        // Get total fines
                        string finesQuery = @"
                            SELECT ISNULL(SUM(amount), 0) 
                            FROM fines f
                            INNER JOIN issues i ON f.issue_id = i.issue_id
                            WHERE MONTH(i.actual_return_date) = @month 
                            AND YEAR(i.actual_return_date) = @year 
                            AND f.date_delete IS NULL";

                        using (SqlCommand cmd = new SqlCommand(finesQuery, con))
                        {
                            cmd.Parameters.AddWithValue("@month", stat.Month);
                            cmd.Parameters.AddWithValue("@year", year);
                            object result = cmd.ExecuteScalar();
                            stat.TotalFines = result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error generating monthly stats: {ex.Message}");
            }
            return stats;
        }

        /// <summary>
        /// Gets most popular books by issue count
        /// </summary>
        public static List<PopularBook> GetPopularBooks(int limit = 10)
        {
            var books = new List<PopularBook>();
            try
            {
                using (var con = Database.GetConnection())
                {
                    con.Open();
                    string query = @"
                        SELECT TOP (@limit)
                            b.id,
                            b.book_title,
                            b.author,
                            COUNT(i.id) as IssueCount
                        FROM books b
                        LEFT JOIN issues i ON b.id = i.book_id AND i.date_delete IS NULL
                        WHERE b.date_delete IS NULL
                        GROUP BY b.id, b.book_title, b.author
                        ORDER BY COUNT(i.id) DESC";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@limit", limit);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                books.Add(new PopularBook
                                {
                                    BookId = Convert.ToInt32(reader["id"]),
                                    Title = reader["book_title"].ToString(),
                                    Author = reader["author"].ToString(),
                                    IssueCount = Convert.ToInt32(reader["IssueCount"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting popular books: {ex.Message}");
            }
            return books;
        }

        /// <summary>
        /// Gets user activity statistics
        /// </summary>
        public static Dictionary<string, int> GetUserActivityStats()
        {
            var stats = new Dictionary<string, int>();
            try
            {
                using (var con = Database.GetConnection())
                {
                    con.Open();
                    string query = @"
                        SELECT 
                            u.name,
                            COUNT(i.id) as IssueCount
                        FROM users u
                        LEFT JOIN issues i ON u.id = i.user_id AND i.date_delete IS NULL
                        WHERE u.role = 'student' AND u.date_delete IS NULL
                        GROUP BY u.id, u.name
                        ORDER BY COUNT(i.id) DESC";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                stats[reader["name"].ToString()] = Convert.ToInt32(reader["IssueCount"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting user activity: {ex.Message}");
            }
            return stats;
        }
    }
}

