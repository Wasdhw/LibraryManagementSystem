using System.Data.SqlClient;

namespace LibraryManagementSystem.Utils
{
    public static class Database
    {
        public static readonly string ConnectionString =
            @"Server=tcp:sdsc-johnmenardmarcelo.database.windows.net,1433;Initial Catalog=LibrarySystemDB;Persist Security Info=False;User ID=app_user;Password=StrongP@ssw0rd!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}

