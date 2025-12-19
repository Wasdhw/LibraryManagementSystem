namespace LibraryManagementSystem
{
    partial class BookInfoForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Title = new System.Windows.Forms.Label();
            this.Author = new System.Windows.Forms.Label();
            this.Quantity = new System.Windows.Forms.Label();
            this.Published = new System.Windows.Forms.Label();
            this.Availability = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.buttonPanel = new System.Windows.Forms.Panel();
            this.reserveBtn = new System.Windows.Forms.Button();
            this.cancelReserveBtn = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.buttonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // Title
            // 
            this.Title.AutoSize = true;
            this.Title.Dock = System.Windows.Forms.DockStyle.Top;
            this.Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Title.Location = new System.Drawing.Point(0, 80);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(61, 24);
            this.Title.TabIndex = 0;
            this.Title.Text = "TITLE";
            this.Title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Title.Click += new System.EventHandler(this.label1_Click);
            // 
            // Author
            // 
            this.Author.AutoSize = true;
            this.Author.Dock = System.Windows.Forms.DockStyle.Top;
            this.Author.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Author.Location = new System.Drawing.Point(0, 104);
            this.Author.Name = "Author";
            this.Author.Size = new System.Drawing.Size(57, 20);
            this.Author.TabIndex = 0;
            this.Author.Text = "Author";
            this.Author.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Author.Click += new System.EventHandler(this.label1_Click);
            // 
            // Quantity
            // 
            this.Quantity.AutoSize = true;
            this.Quantity.Dock = System.Windows.Forms.DockStyle.Top;
            this.Quantity.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Quantity.Location = new System.Drawing.Point(0, 0);
            this.Quantity.Name = "Quantity";
            this.Quantity.Size = new System.Drawing.Size(68, 20);
            this.Quantity.TabIndex = 0;
            this.Quantity.Text = "Quantity";
            this.Quantity.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Quantity.Click += new System.EventHandler(this.label1_Click);
            // 
            // Published
            // 
            this.Published.AutoSize = true;
            this.Published.Dock = System.Windows.Forms.DockStyle.Top;
            this.Published.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Published.Location = new System.Drawing.Point(0, 20);
            this.Published.Name = "Published";
            this.Published.Size = new System.Drawing.Size(78, 20);
            this.Published.TabIndex = 0;
            this.Published.Text = "Published";
            this.Published.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Published.Click += new System.EventHandler(this.label1_Click);
            // 
            // Availability
            // 
            this.Availability.AutoSize = true;
            this.Availability.Dock = System.Windows.Forms.DockStyle.Top;
            this.Availability.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Availability.Location = new System.Drawing.Point(0, 40);
            this.Availability.Name = "Availability";
            this.Availability.Size = new System.Drawing.Size(81, 20);
            this.Availability.TabIndex = 0;
            this.Availability.Text = "Availability";
            this.Availability.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Availability.Click += new System.EventHandler(this.label1_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.Availability);
            this.panel1.Controls.Add(this.Published);
            this.panel1.Controls.Add(this.Quantity);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(477, 80);
            this.panel1.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label1.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(440, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "X";
            this.label1.Click += new System.EventHandler(this.label1_Click_1);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.pictureBox1.Location = new System.Drawing.Point(0, 124);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(477, 310);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // buttonPanel
            // 
            this.buttonPanel.Controls.Add(this.reserveBtn);
            this.buttonPanel.Controls.Add(this.cancelReserveBtn);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonPanel.Location = new System.Drawing.Point(0, 586);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(477, 50);
            this.buttonPanel.TabIndex = 4;
            // 
            // reserveBtn
            // 
            this.reserveBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.reserveBtn.FlatAppearance.BorderSize = 0;
            this.reserveBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.reserveBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.reserveBtn.ForeColor = System.Drawing.Color.White;
            this.reserveBtn.Location = new System.Drawing.Point(100, 10);
            this.reserveBtn.Name = "reserveBtn";
            this.reserveBtn.Size = new System.Drawing.Size(120, 35);
            this.reserveBtn.TabIndex = 0;
            this.reserveBtn.Text = "Reserve Book";
            this.reserveBtn.UseVisualStyleBackColor = false;
            this.reserveBtn.Visible = false;
            this.reserveBtn.Click += new System.EventHandler(this.ReserveBtn_Click);
            // 
            // cancelReserveBtn
            // 
            this.cancelReserveBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(67)))), ((int)(((byte)(54)))));
            this.cancelReserveBtn.FlatAppearance.BorderSize = 0;
            this.cancelReserveBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancelReserveBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelReserveBtn.ForeColor = System.Drawing.Color.White;
            this.cancelReserveBtn.Location = new System.Drawing.Point(100, 10);
            this.cancelReserveBtn.Name = "cancelReserveBtn";
            this.cancelReserveBtn.Size = new System.Drawing.Size(150, 35);
            this.cancelReserveBtn.TabIndex = 1;
            this.cancelReserveBtn.Text = "Cancel Reservation";
            this.cancelReserveBtn.UseVisualStyleBackColor = false;
            this.cancelReserveBtn.Visible = false;
            this.cancelReserveBtn.Click += new System.EventHandler(this.CancelReserveBtn_Click);
            // 
            // BookInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(477, 636);
            this.Controls.Add(this.buttonPanel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.Author);
            this.Controls.Add(this.Title);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "BookInfoForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form2";
            this.Load += new System.EventHandler(this.BookInfoForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.buttonPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Title;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label Author;
        private System.Windows.Forms.Label Quantity;
        private System.Windows.Forms.Label Published;
        private System.Windows.Forms.Label Availability;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel buttonPanel;
        private System.Windows.Forms.Button reserveBtn;
        private System.Windows.Forms.Button cancelReserveBtn;
    }
}