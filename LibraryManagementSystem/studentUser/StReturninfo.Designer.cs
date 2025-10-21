namespace LibraryManagementSystem.studentUser
{
    partial class StReturninfo
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
            this.Author = new System.Windows.Forms.Label();
            this.Title = new System.Windows.Forms.Label();
            this.Quantity = new System.Windows.Forms.Label();
            this.Availability = new System.Windows.Forms.Label();
            this.Published = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.bookreturn_addBtn = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Author
            // 
            this.Author.AutoSize = true;
            this.Author.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Author.Location = new System.Drawing.Point(205, 370);
            this.Author.Name = "Author";
            this.Author.Size = new System.Drawing.Size(57, 20);
            this.Author.TabIndex = 19;
            this.Author.Text = "Author";
            // 
            // Title
            // 
            this.Title.AutoSize = true;
            this.Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Title.Location = new System.Drawing.Point(205, 334);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(61, 24);
            this.Title.TabIndex = 20;
            this.Title.Text = "TITLE";
            // 
            // Quantity
            // 
            this.Quantity.AutoSize = true;
            this.Quantity.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Quantity.Location = new System.Drawing.Point(181, 86);
            this.Quantity.Name = "Quantity";
            this.Quantity.Size = new System.Drawing.Size(68, 20);
            this.Quantity.TabIndex = 0;
            this.Quantity.Text = "Quantity";
            // 
            // Availability
            // 
            this.Availability.AutoSize = true;
            this.Availability.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Availability.Location = new System.Drawing.Point(178, 155);
            this.Availability.Name = "Availability";
            this.Availability.Size = new System.Drawing.Size(81, 20);
            this.Availability.TabIndex = 0;
            this.Availability.Text = "Availability";
            // 
            // Published
            // 
            this.Published.AutoSize = true;
            this.Published.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Published.Location = new System.Drawing.Point(181, 121);
            this.Published.Name = "Published";
            this.Published.Size = new System.Drawing.Size(78, 20);
            this.Published.TabIndex = 0;
            this.Published.Text = "Published";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(128)))), ((int)(((byte)(87)))));
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(305, 547);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 33);
            this.button1.TabIndex = 23;
            this.button1.Text = "CLOSE";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // bookreturn_addBtn
            // 
            this.bookreturn_addBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(128)))), ((int)(((byte)(87)))));
            this.bookreturn_addBtn.FlatAppearance.BorderSize = 0;
            this.bookreturn_addBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bookreturn_addBtn.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bookreturn_addBtn.ForeColor = System.Drawing.Color.White;
            this.bookreturn_addBtn.Location = new System.Drawing.Point(86, 547);
            this.bookreturn_addBtn.Name = "bookreturn_addBtn";
            this.bookreturn_addBtn.Size = new System.Drawing.Size(100, 33);
            this.bookreturn_addBtn.TabIndex = 24;
            this.bookreturn_addBtn.Text = "RETURN";
            this.bookreturn_addBtn.UseVisualStyleBackColor = false;
            this.bookreturn_addBtn.Click += new System.EventHandler(this.bookreturn_addBtn_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.pictureBox1.Location = new System.Drawing.Point(160, 56);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(149, 211);
            this.pictureBox1.TabIndex = 21;
            this.pictureBox1.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.Quantity);
            this.panel1.Controls.Add(this.Availability);
            this.panel1.Controls.Add(this.Published);
            this.panel1.Location = new System.Drawing.Point(17, 322);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(442, 191);
            this.panel1.TabIndex = 22;
            // 
            // StBorrowReturn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Author);
            this.Controls.Add(this.Title);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.bookreturn_addBtn);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.panel1);
            this.Name = "StBorrowReturn";
            this.Size = new System.Drawing.Size(477, 636);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Author;
        private System.Windows.Forms.Label Title;
        private System.Windows.Forms.Label Quantity;
        private System.Windows.Forms.Label Availability;
        private System.Windows.Forms.Label Published;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button bookreturn_addBtn;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel1;
    }
}
