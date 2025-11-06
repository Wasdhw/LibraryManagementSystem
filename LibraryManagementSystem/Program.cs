using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibraryManagementSystem.Utils;

namespace LibraryManagementSystem
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Initialize Azure Blob container for book covers
            BlobCovers.Initialize("DefaultEndpointsProtocol=https;AccountName=librarysomething;AccountKey=e98IaTRdQf0usuMiRqck02g7NJ2bIixwdHeglFF4K976Xd2Ba7pVVyPZobUZbqZ0Yb5yhzytaOp1+ASt+Onjzg==;EndpointSuffix=core.windows.net", "book-covers");
            Application.Run(new Form1());
        }
    }
}
