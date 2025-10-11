namespace LibraryManagementSystem
{
    partial class AvailBooks
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.flowAvailableBooks = new System.Windows.Forms.FlowLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.flowAvailableBooks.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // flowAvailableBooks
            // 
            this.flowAvailableBooks.AutoScroll = true;
            this.flowAvailableBooks.Controls.Add(this.pictureBox1);
            this.flowAvailableBooks.Controls.Add(this.pictureBox2);
            this.flowAvailableBooks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowAvailableBooks.Location = new System.Drawing.Point(0, 0);
            this.flowAvailableBooks.Name = "flowAvailableBooks";
            this.flowAvailableBooks.Size = new System.Drawing.Size(1110, 682);
            this.flowAvailableBooks.TabIndex = 0;
            this.flowAvailableBooks.Paint += new System.Windows.Forms.PaintEventHandler(this.flowAvailableBooks_Paint);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(15, 15);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(15);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Padding = new System.Windows.Forms.Padding(3);
            this.pictureBox1.Size = new System.Drawing.Size(145, 176);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(187, 15);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(12, 15, 12, 15);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Padding = new System.Windows.Forms.Padding(3);
            this.pictureBox2.Size = new System.Drawing.Size(145, 176);
            this.pictureBox2.TabIndex = 1;
            this.pictureBox2.TabStop = false;
            // 
            // AvailBooks
            // 
            this.Controls.Add(this.flowAvailableBooks);
            this.Name = "AvailBooks";
            this.Size = new System.Drawing.Size(1110, 682);
            this.flowAvailableBooks.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowAvailableBooks;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}
