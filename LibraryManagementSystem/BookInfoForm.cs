using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryManagementSystem
{
    public partial class BookInfoForm : Form
    {
        private readonly int _bookId;

        public BookInfoForm()
        {
            InitializeComponent();
        }

        public BookInfoForm(int bookId)
        {
            InitializeComponent();
            _bookId = bookId;
        }

        private async void BookInfoForm_Load(object sender, EventArgs e)
        {
            if (_bookId <= 0)
            {
                return;
            }

            try
            {
                using (var con = Utils.Database.GetConnection())
                {
                    con.Open();
                    string sql = "SELECT book_title, author, published_date, quantity, status, image FROM books WHERE id = @id";
                    using (var cmd = new System.Data.SqlClient.SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@id", _bookId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var title = reader["book_title"].ToString();
                                var author = reader["author"].ToString();
                                var published = Convert.ToDateTime(reader["published_date"]).ToString("yyyy-MM-dd");
                                var quantity = Convert.ToInt32(reader["quantity"]);
                                var status = reader["status"].ToString();
                                var imagePath = reader["image"].ToString();

                                Title.Text = title;
                                Author.Text = author;
                                Published.Text = $"Published: {published}";
                                Quantity.Text = $"Quantity: {quantity}";
                                Availability.Text = $"Availability: {status}";

                                try
                                {
                                    if (!string.IsNullOrEmpty(imagePath))
                                    {
                                        // image column now stores the blob name; download from Azure Blob Storage
                                        var cover = await Utils.BlobCovers.DownloadAsync(imagePath);
                                        pictureBox1.Image = cover;
                                        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        pictureBox1.Image = null;
                                    }
                                }
                                catch
                                {
                                    pictureBox1.Image = null;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load book details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
