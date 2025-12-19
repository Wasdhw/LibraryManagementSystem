using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using LibraryManagementSystem.Utils;
using System.IO;

namespace LibraryManagementSystem
{
    public partial class AddBooks : UserControl
    {
        SqlConnection connect = Database.GetConnection();

        public AddBooks()
        {
            InitializeComponent();

            displayBooks();
            LoadCategories();
        }

        private void LoadCategories()
        {
            try
            {
                var categories = CategoryManager.GetAllCategories();
                if (this.Controls.Find("categoryComboBox", true).FirstOrDefault() is ComboBox categoryCombo)
                {
                    categoryCombo.DataSource = categories;
                    categoryCombo.DisplayMember = "Name";
                    categoryCombo.ValueMember = "Id";
                    categoryCombo.SelectedIndex = -1; // No selection by default
                }
            }
            catch
            {
                // Categories table may not exist yet
            }
        }


        public void refreshData()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)refreshData);
                return;
            }

            displayBooks();
        }

        private String imagePath;
        private void addBooks_importBtn_Click(object sender, EventArgs e)
        {
            
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Image Files (*.jpg; *.png)|*.jpg;*.png";

                if(dialog.ShowDialog() == DialogResult.OK)
                {
                    imagePath = dialog.FileName;
                    addBooks_picture.ImageLocation = imagePath;
                }
            }catch(Exception ex)
            {
                MessageBox.Show("Error: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void addBooks_addBtn_Click(object sender, EventArgs e)
        {
            if(addBooks_picture.Image == null
                || addBooks_bookTitle.Text == ""
                || addBooks_author.Text == ""
                || addBooks_published.Value == null
                || addBooks_quantity.Value == 0
                || addBooks_status.Text == ""
                || addBooks_picture.Image == null)
            {
                MessageBox.Show("Please fill all blank fields", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if(connect.State == ConnectionState.Closed)
                {
                    try
                    {
                        DateTime today = DateTime.Today;
                        connect.Open();
                        // Get category if available
                        int? categoryId = null;
                        if (this.Controls.Find("categoryComboBox", true).FirstOrDefault() is ComboBox categoryCombo && categoryCombo.SelectedValue != null)
                        {
                            categoryId = Convert.ToInt32(categoryCombo.SelectedValue);
                        }

                        string insertData = "INSERT INTO books " +
                            "(book_title, author, published_date, quantity, status, image, isbn, publisher, edition, language, pages, category_id, date_insert) " +
                            "VALUES(@bookTitle, @author, @published_date, @quantity, @status, @image, @isbn, @publisher, @edition, @language, @pages, @category_id, @dateInsert)";

                        // Upload cover to Azure Blob and store blob name in DB
                        string blobName = BlobCovers.GenerateBlobName(Guid.NewGuid().ToString("N"));
                        await BlobCovers.UploadAsync(addBooks_picture.Image, blobName);

                        using(SqlCommand cmd = new SqlCommand(insertData, connect))
                        {
                            // Duplicate check
                            if (IsDuplicateBook(addBooks_bookTitle.Text.Trim(), addBooks_author.Text.Trim()))
                            {
                                MessageBox.Show("Book already exists.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            cmd.Parameters.AddWithValue("@bookTitle", addBooks_bookTitle.Text.Trim());
                            cmd.Parameters.AddWithValue("@author", addBooks_author.Text.Trim());
                            cmd.Parameters.AddWithValue("@published_date", addBooks_published.Value);
                            cmd.Parameters.AddWithValue("@quantity", Convert.ToInt32(addBooks_quantity.Value));
                            cmd.Parameters.AddWithValue("@status", addBooks_status.Text.Trim());
                            cmd.Parameters.AddWithValue("@image", blobName);
                            
                            // Enhanced metadata (optional fields)
                            string isbn = this.Controls.Find("isbnTextBox", true).FirstOrDefault() is TextBox isbnTb ? isbnTb.Text.Trim() : "";
                            string publisher = this.Controls.Find("publisherTextBox", true).FirstOrDefault() is TextBox pubTb ? pubTb.Text.Trim() : "";
                            string edition = this.Controls.Find("editionTextBox", true).FirstOrDefault() is TextBox edTb ? edTb.Text.Trim() : "";
                            string language = this.Controls.Find("languageTextBox", true).FirstOrDefault() is TextBox langTb ? langTb.Text.Trim() : "";
                            int pages = this.Controls.Find("pagesNumeric", true).FirstOrDefault() is NumericUpDown pagesNum ? (int)pagesNum.Value : 0;
                            
                            cmd.Parameters.AddWithValue("@isbn", string.IsNullOrEmpty(isbn) ? DBNull.Value : (object)isbn);
                            cmd.Parameters.AddWithValue("@publisher", string.IsNullOrEmpty(publisher) ? DBNull.Value : (object)publisher);
                            cmd.Parameters.AddWithValue("@edition", string.IsNullOrEmpty(edition) ? DBNull.Value : (object)edition);
                            cmd.Parameters.AddWithValue("@language", string.IsNullOrEmpty(language) ? DBNull.Value : (object)language);
                            cmd.Parameters.AddWithValue("@pages", pages > 0 ? (object)pages : DBNull.Value);
                            cmd.Parameters.AddWithValue("@category_id", categoryId.HasValue ? (object)categoryId.Value : DBNull.Value);
                            cmd.Parameters.AddWithValue("@dateInsert", today);

                            cmd.ExecuteNonQuery();

                            displayBooks();

                            MessageBox.Show("Added successfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            clearFields();
                        }

                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("Error: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                    finally
                    {
                        connect.Close();
                    }
                }
            }
        }

        public void clearFields()
        {
            addBooks_bookTitle.Text = "";
            addBooks_author.Text = "";
            addBooks_quantity.Value = 0;
            addBooks_picture.Image = null;
            addBooks_status.SelectedIndex = -1;
        }

        private bool IsDuplicateBook(string title, string author)
        {
            if (connect.State == ConnectionState.Closed)
            {
                try
                {
                    connect.Open();
                    string sql = "SELECT COUNT(*) FROM books WHERE book_title = @title AND author = @author AND date_delete IS NULL";
                    using (SqlCommand cmd = new SqlCommand(sql, connect))
                    {
                        cmd.Parameters.AddWithValue("@title", title);
                        cmd.Parameters.AddWithValue("@author", author);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
                catch
                {
                    return false;
                }
                finally
                {
                    connect.Close();
                }
            }
            return false;
        }

        public void displayBooks()
        {
            DataAddBooks dab = new DataAddBooks();
            List<DataAddBooks> listData = dab.addBooksData();

            dataGridView1.DataSource = listData;

        }

        private int bookID = 0;
        private async void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex != -1)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                bookID = (int)row.Cells[0].Value;
                addBooks_bookTitle.Text = row.Cells[1].Value.ToString();
                addBooks_author.Text = row.Cells[2].Value.ToString();
                addBooks_published.Text = row.Cells[3].Value.ToString();
                addBooks_quantity.Value = Convert.ToInt32(row.Cells[4].Value);

                string imagePath = row.Cells[5].Value?.ToString();

                // imagePath column now stores the blob name
                if (!string.IsNullOrWhiteSpace(imagePath))
                {
                    try
                    {
                        addBooks_picture.Image = await BlobCovers.DownloadAsync(imagePath);
                    }
                    catch (Exception ex)
                    {
                        addBooks_picture.Image = null;
                        Console.WriteLine($"Failed to download image: {ex.Message}");
                    }
                }
                else
                {
                    addBooks_picture.Image = null;
                }
                addBooks_status.Text = row.Cells[6].Value.ToString();
            }
        }

        private void addBooks_clearBtn_Click(object sender, EventArgs e)
        {
            clearFields();
        }

        private async void addBooks_updateBtn_Click(object sender, EventArgs e)
        {
            if (addBooks_picture.Image == null
                || addBooks_bookTitle.Text == ""
                || addBooks_author.Text == ""
                || addBooks_published.Value == null
                || addBooks_quantity.Value == 0
                || addBooks_status.Text == ""
                || addBooks_picture.Image == null)
            {
                MessageBox.Show("Please select item first", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if(connect.State == ConnectionState.Closed)
                {
                    DialogResult check = MessageBox.Show("Are you sure you want to UPDATE Book ID:"
                        + bookID + "?", "Confirmation Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if(check == DialogResult.Yes)
                    {
                        try
                        {
                            connect.Open();
                            DateTime today = DateTime.Today;
                            string updateData = "UPDATE books SET book_title = @bookTitle" +
                                ", author = @author, published_date = @published" +
                                ", quantity = @quantity, status = @status, image = @image, date_update = @dateUpdate WHERE id = @id";

                            // Handle image update: upload to blob and store blob name
                            string imagePath = null;
                            if (addBooks_picture.Image != null)
                            {
                                string blobName = BlobCovers.GenerateBlobName(Guid.NewGuid().ToString("N"));
                                await BlobCovers.UploadAsync(addBooks_picture.Image, blobName);
                                imagePath = blobName;
                            }

                            using (SqlCommand cmd = new SqlCommand(updateData, connect))
                            {
                                cmd.Parameters.AddWithValue("@bookTitle", addBooks_bookTitle.Text.Trim());
                                cmd.Parameters.AddWithValue("@author", addBooks_author.Text.Trim());
                                cmd.Parameters.AddWithValue("@published", addBooks_published.Value);
                                cmd.Parameters.AddWithValue("@quantity", Convert.ToInt32(addBooks_quantity.Value));
                                cmd.Parameters.AddWithValue("@status", addBooks_status.Text.Trim());
                                cmd.Parameters.AddWithValue("@image", (object)imagePath ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@dateUpdate", today);
                                cmd.Parameters.AddWithValue("@id", bookID);

                                cmd.ExecuteNonQuery();

                                displayBooks();

                                MessageBox.Show("Updated successfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                clearFields();
                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {
                            connect.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Cancelled.", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    
                }
            }
        }

        private void addBooks_deleteBtn_Click(object sender, EventArgs e)
        {
            if (addBooks_picture.Image == null
                || addBooks_bookTitle.Text == ""
                || addBooks_author.Text == ""
                || addBooks_published.Value == null
                || addBooks_quantity.Value == 0
                || addBooks_status.Text == ""
                || addBooks_picture.Image == null)
            {
                MessageBox.Show("Please select item first", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (connect.State == ConnectionState.Closed)
                {
                    DialogResult check = MessageBox.Show("Are you sure you want to DELETE Book ID:"
                        + bookID + "?", "Confirmation Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (check == DialogResult.Yes)
                    {
                        try
                        {
                            connect.Open();
                            DateTime today = DateTime.Today;
                            string updateData = "UPDATE books SET date_delete = @dateDelete WHERE id = @id";

                            using (SqlCommand cmd = new SqlCommand(updateData, connect))
                            {
                                cmd.Parameters.AddWithValue("@dateDelete", today);
                                cmd.Parameters.AddWithValue("@id", bookID);

                                cmd.ExecuteNonQuery();

                                displayBooks();

                                MessageBox.Show("Deleted successfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                clearFields();
                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {
                            connect.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Cancelled.", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void addBooks_status_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void AddBooks_Load(object sender, EventArgs e)
        {

        }
    }
}
