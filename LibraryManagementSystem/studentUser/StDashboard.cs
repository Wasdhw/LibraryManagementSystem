using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryManagementSystem.MainformsUser
{
    public partial class StDashboard : UserControl
    {
        SqlConnection connect = new SqlConnection(@"Server=tcp:sdsc-johnmenardmarcelo.database.windows.net,1433;Initial Catalog=LibrarySystemDB;Persist Security Info=False;User ID=app_user;Password=StrongP@ssw0rd!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
        public StDashboard()
        {
            InitializeComponent();
        }

        private void StDashboard_Load(object sender, EventArgs e)
        {

        }
    }
}
