using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryManagementSystem
{
    public partial class AvailBooks : UserControl
    {
        public AvailBooks()
        {
            InitializeComponent();

          
            LoadAvailableBooks();

           
            this.Resize += AvailBooks_Resize;

        }

        public void refreshData()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)refreshData);
                return;
            }

           
            LoadAvailableBooks();

        }



        private void AvailBooks_Resize(object sender, EventArgs e)
        {
            // Optionally, adjust child control sizes or spacing
        }

        private void LoadAvailableBooks()
        {
            flowAvailableBooks.Controls.Clear();

            string query = "SELECT id, book_title, author, status, image FROM books WHERE date_delete IS NULL AND status = 'Available'";

            using (SqlConnection con = new SqlConnection(@"Server=tcp:sdsc-johnmenardmarcelo.database.windows.net,1433;Initial Catalog=LibrarySystemDB;Persist Security Info=False;User ID=app_user;Password=StrongP@ssw0rd!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = (int)reader["id"];
                        string title = reader["book_title"].ToString();
                        string author = reader["author"].ToString();
                        string imgPath = reader["image"].ToString();

                        // Check image file exists
                        Image img = null;
                        try
                        {
                            if (File.Exists(imgPath))
                                img = Image.FromFile(imgPath);
                        }
                        catch (Exception ex) {
                            MessageBox.Show("DB or query error: " + ex.Message);
                        }

                        PictureBox pb = new PictureBox();
                        pb.Size = new Size(145, 176);
                        pb.SizeMode = PictureBoxSizeMode.Zoom;
                        pb.Image = img;  
                        pb.Cursor = Cursors.Hand;
                        pb.Margin = new Padding(10);

                        // Store info 
                        pb.Tag = new BookTag
                        {
                            Id = id,
                            Title = title,
                            Author = author,
                            ImagePath = imgPath
                        };

                        pb.Click += Pb_Click;

                      
                        Panel pnl = new Panel();
                        pnl.Width = pb.Width + 25;
                        pnl.Height = pb.Height + 25;
                        pnl.Controls.Add(pb);

                        Label lbl = new Label();
                        lbl.Text = title;
                        lbl.Dock = DockStyle.Bottom;
                        lbl.Font = new Font("Arial", 12, FontStyle.Bold);
                        lbl.TextAlign = ContentAlignment.MiddleCenter;
                        pnl.Controls.Add(lbl);

                        flowAvailableBooks.Controls.Add(pnl);
                    }
                }
            }
        }

        private void Pb_Click(object sender, EventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            if (pb != null && pb.Tag is BookTag tag)
            {
                // Open or display the book info
                ShowBookInfo(tag);
            }
        }

        private void ShowBookInfo(BookTag tag)
        {
    
            BookInfoForm info = new BookInfoForm();
            info.ShowDialog();
        }

        private class BookTag
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Author { get; set; }
            public string ImagePath { get; set; }
        }

        private void flowAvailableBooks_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    }

}
