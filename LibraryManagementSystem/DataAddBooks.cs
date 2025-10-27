using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace LibraryManagementSystem
{
    class DataAddBooks
    {
        SqlConnection connect = new SqlConnection(@"Server=tcp:sdsc-johnmenardmarcelo.database.windows.net,1433;Initial Catalog=LibrarySystemDB;Persist Security Info=False;User ID=app_user;Password=StrongP@ssw0rd!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
        public int ID { set; get; }
        public string BookTitle { set; get; }
        public string Author { set; get; }
        public string Pulished { set; get; }
        public int Quantity { set; get; }
        public string image { set; get; }
        public string Status { set; get; }

        public List<DataAddBooks> addBooksData()
        {
            List<DataAddBooks> listData = new List<DataAddBooks>();

            if(connect.State != ConnectionState.Open)
            {
                try
                {
                    connect.Open();

                    string selectData = "SELECT id, book_title, author, published_date, quantity, image, status FROM books WHERE date_delete IS NULL";

                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        

                        while (reader.Read())
                        {
                            DataAddBooks dab = new DataAddBooks();
                            dab.ID = (int)reader["id"];
                            dab.BookTitle = reader["book_title"].ToString();
                            dab.Author = reader["author"].ToString();
                            dab.Pulished = reader["published_date"].ToString();
                            dab.Quantity = (int)reader["quantity"];
                            dab.image = reader["image"].ToString();
                            dab.Status = reader["status"].ToString();

                            listData.Add(dab);
                        }

                        reader.Close();
                    }

                    }catch(Exception ex)
                {
                    Console.WriteLine("Error conenecting Database: " + ex);
                }
                finally
                {
                    connect.Close();
                }
            }
            return listData;
        }
    }
}
