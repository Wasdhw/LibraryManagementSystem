using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using LibraryManagementSystem.Utils;

namespace LibraryManagementSystem.Utils
{
    public static class CategoryManager
    {
        /// <summary>
        /// Gets all categories
        /// </summary>
        public static List<CategoryInfo> GetAllCategories()
        {
            var categories = new List<CategoryInfo>();
            try
            {
                using (var con = Database.GetConnection())
                {
                    con.Open();
                    string query = "SELECT id, name, description FROM categories WHERE date_delete IS NULL ORDER BY name";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                categories.Add(new CategoryInfo
                                {
                                    Id = Convert.ToInt32(reader["id"]),
                                    Name = reader["name"].ToString(),
                                    Description = reader["description"] != DBNull.Value ? reader["description"].ToString() : ""
                                });
                            }
                        }
                    }
                }
            }
            catch
            {
                // If categories table doesn't exist, return empty list
            }
            return categories;
        }

        /// <summary>
        /// Creates a new category
        /// </summary>
        public static bool CreateCategory(string name, string description = "")
        {
            try
            {
                using (var con = Database.GetConnection())
                {
                    con.Open();
                    string query = "INSERT INTO categories (name, description, date_insert) VALUES (@name, @description, @date_insert)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@description", description);
                        cmd.Parameters.AddWithValue("@date_insert", DateTime.Today);

                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets category for a book
        /// </summary>
        public static int? GetBookCategory(int bookId)
        {
            try
            {
                using (var con = Database.GetConnection())
                {
                    con.Open();
                    string query = "SELECT category_id FROM books WHERE id = @book_id";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@book_id", bookId);
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            return Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch
            {
            }
            return null;
        }

        public class CategoryInfo
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
        }
    }
}

