namespace LibraryManagementSystem
{
    partial class StudentForm
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
            this.label4 = new System.Windows.Forms.Label();
            this.greet_label = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.stDashboard1 = new LibraryManagementSystem.MainformsUser.StDashboard();
            this.stReturnBooks1 = new LibraryManagementSystem.studentUser.StReturnBooks();
            this.history1 = new LibraryManagementSystem.studentUser.History();
            this.stAvailbooks1 = new LibraryManagementSystem.studentUser.StAvailbooks();
            this.logout_btn = new System.Windows.Forms.Button();
            this.history_btn = new System.Windows.Forms.Button();
            this.avail_btn = new System.Windows.Forms.Button();
            this.borrow_btn = new System.Windows.Forms.Button();
            this.dashboard_btn = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(55, 639);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 16);
            this.label4.TabIndex = 7;
            this.label4.Text = "Log out";
            // 
            // greet_label
            // 
            this.greet_label.AutoSize = true;
            this.greet_label.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.greet_label.ForeColor = System.Drawing.Color.White;
            this.greet_label.Location = new System.Drawing.Point(42, 137);
            this.greet_label.Name = "greet_label";
            this.greet_label.Size = new System.Drawing.Size(133, 19);
            this.greet_label.TabIndex = 1;
            this.greet_label.Text = "Welcome, Savian!";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(128)))), ((int)(((byte)(87)))));
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.logout_btn);
            this.panel2.Controls.Add(this.history_btn);
            this.panel2.Controls.Add(this.avail_btn);
            this.panel2.Controls.Add(this.borrow_btn);
            this.panel2.Controls.Add(this.dashboard_btn);
            this.panel2.Controls.Add(this.greet_label);
            this.panel2.Controls.Add(this.pictureBox1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 35);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(220, 675);
            this.panel2.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(13, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(262, 18);
            this.label2.TabIndex = 1;
            this.label2.Text = "Library Management System | Student";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.stAvailbooks1);
            this.panel3.Controls.Add(this.history1);
            this.panel3.Controls.Add(this.stReturnBooks1);
            this.panel3.Controls.Add(this.stDashboard1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 35);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1330, 675);
            this.panel3.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(128)))), ((int)(((byte)(87)))));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1330, 35);
            this.panel1.TabIndex = 3;
            // 
            // stDashboard1
            // 
            this.stDashboard1.Location = new System.Drawing.Point(220, -7);
            this.stDashboard1.Name = "stDashboard1";
            this.stDashboard1.Size = new System.Drawing.Size(1110, 682);
            this.stDashboard1.TabIndex = 0;
            // 
            // stReturnBooks1
            // 
            this.stReturnBooks1.Location = new System.Drawing.Point(220, 0);
            this.stReturnBooks1.Name = "stReturnBooks1";
            this.stReturnBooks1.Size = new System.Drawing.Size(1110, 682);
            this.stReturnBooks1.TabIndex = 1;
            this.stReturnBooks1.Visible = false;
            // 
            // history1
            // 
            this.history1.Location = new System.Drawing.Point(220, 0);
            this.history1.Name = "history1";
            this.history1.Size = new System.Drawing.Size(1110, 682);
            this.history1.TabIndex = 2;
            this.history1.Visible = false;
            // 
            // stAvailbooks1
            // 
            this.stAvailbooks1.Location = new System.Drawing.Point(220, 0);
            this.stAvailbooks1.Name = "stAvailbooks1";
            this.stAvailbooks1.Size = new System.Drawing.Size(1110, 675);
            this.stAvailbooks1.TabIndex = 3;
            this.stAvailbooks1.Visible = false;
            // 
            // logout_btn
            // 
            this.logout_btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.logout_btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkSeaGreen;
            this.logout_btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkSeaGreen;
            this.logout_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.logout_btn.ForeColor = System.Drawing.Color.White;
            this.logout_btn.Image = global::LibraryManagementSystem.Properties.Resources.icons8_logout_rounded_up_filled_20px;
            this.logout_btn.Location = new System.Drawing.Point(14, 630);
            this.logout_btn.Name = "logout_btn";
            this.logout_btn.Size = new System.Drawing.Size(35, 35);
            this.logout_btn.TabIndex = 6;
            this.logout_btn.UseVisualStyleBackColor = true;
            this.logout_btn.Click += new System.EventHandler(this.logout_btn_Click);
            // 
            // history_btn
            // 
            this.history_btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.history_btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkSeaGreen;
            this.history_btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkSeaGreen;
            this.history_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.history_btn.ForeColor = System.Drawing.Color.White;
            this.history_btn.Image = global::LibraryManagementSystem.Properties.Resources.clock_4_32;
            this.history_btn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.history_btn.Location = new System.Drawing.Point(11, 378);
            this.history_btn.Name = "history_btn";
            this.history_btn.Size = new System.Drawing.Size(200, 45);
            this.history_btn.TabIndex = 4;
            this.history_btn.Text = "HISTORY";
            this.history_btn.UseVisualStyleBackColor = true;
            this.history_btn.Click += new System.EventHandler(this.history_btn_Click);
            // 
            // avail_btn
            // 
            this.avail_btn.AccessibleDescription = "";
            this.avail_btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.avail_btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkSeaGreen;
            this.avail_btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkSeaGreen;
            this.avail_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.avail_btn.ForeColor = System.Drawing.Color.White;
            this.avail_btn.Image = global::LibraryManagementSystem.Properties.Resources.icons8_book_32px;
            this.avail_btn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.avail_btn.Location = new System.Drawing.Point(11, 276);
            this.avail_btn.Name = "avail_btn";
            this.avail_btn.Size = new System.Drawing.Size(200, 45);
            this.avail_btn.TabIndex = 3;
            this.avail_btn.Text = "BOOKS";
            this.avail_btn.UseVisualStyleBackColor = true;
            this.avail_btn.Click += new System.EventHandler(this.avail_btn_Click);
            // 
            // borrow_btn
            // 
            this.borrow_btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.borrow_btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkSeaGreen;
            this.borrow_btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkSeaGreen;
            this.borrow_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.borrow_btn.ForeColor = System.Drawing.Color.White;
            this.borrow_btn.Image = global::LibraryManagementSystem.Properties.Resources.book_2_32;
            this.borrow_btn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.borrow_btn.Location = new System.Drawing.Point(11, 327);
            this.borrow_btn.Name = "borrow_btn";
            this.borrow_btn.Size = new System.Drawing.Size(200, 45);
            this.borrow_btn.TabIndex = 3;
            this.borrow_btn.Text = "BORROWED";
            this.borrow_btn.UseVisualStyleBackColor = true;
            this.borrow_btn.Click += new System.EventHandler(this.borrow_btn_Click);
            // 
            // dashboard_btn
            // 
            this.dashboard_btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dashboard_btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkSeaGreen;
            this.dashboard_btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkSeaGreen;
            this.dashboard_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.dashboard_btn.ForeColor = System.Drawing.Color.White;
            this.dashboard_btn.Image = global::LibraryManagementSystem.Properties.Resources.icons8_dashboard_32px;
            this.dashboard_btn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.dashboard_btn.Location = new System.Drawing.Point(11, 222);
            this.dashboard_btn.Name = "dashboard_btn";
            this.dashboard_btn.Size = new System.Drawing.Size(200, 45);
            this.dashboard_btn.TabIndex = 2;
            this.dashboard_btn.Text = "DASHBOARD";
            this.dashboard_btn.UseVisualStyleBackColor = true;
            this.dashboard_btn.Click += new System.EventHandler(this.dashboard_btn_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::LibraryManagementSystem.Properties.Resources.sabyo_logo;
            this.pictureBox1.Location = new System.Drawing.Point(58, 15);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(100, 110);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // StudentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1330, 710);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Name = "StudentForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form2";
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button logout_btn;
        private System.Windows.Forms.Button history_btn;
        private System.Windows.Forms.Button avail_btn;
        private System.Windows.Forms.Button borrow_btn;
        private System.Windows.Forms.Button dashboard_btn;
        private System.Windows.Forms.Label greet_label;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel1;
        private studentUser.StReturnBooks stReturnBooks1;
        private MainformsUser.StDashboard stDashboard1;
        private studentUser.StAvailbooks stAvailbooks1;
        private studentUser.History history1;
    }
}