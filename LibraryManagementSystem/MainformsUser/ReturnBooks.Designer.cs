
namespace LibraryManagementSystem
{
    partial class ReturnBooks
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.conditionComboBox = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.bookIssue_issueDate = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.returnBooks_clearBtn = new System.Windows.Forms.Button();
            this.returnBooks_returnBtn = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.returnBooks_author = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.returnBooks_bookTitle = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.returnBooks_name = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.returnBooks_issueID = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.conditionComboBox);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.bookIssue_issueDate);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.returnBooks_clearBtn);
            this.panel1.Controls.Add(this.returnBooks_returnBtn);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.returnBooks_author);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.returnBooks_bookTitle);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.returnBooks_name);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.returnBooks_issueID);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Location = new System.Drawing.Point(13, 18);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(278, 640);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // conditionComboBox
            // 
            this.conditionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.conditionComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.conditionComboBox.FormattingEnabled = true;
            this.conditionComboBox.Items.AddRange(new object[] {
            "New",
            "Good",
            "Fair",
            "Poor",
            "Damaged"});
            this.conditionComboBox.Location = new System.Drawing.Point(92, 360);
            this.conditionComboBox.Name = "conditionComboBox";
            this.conditionComboBox.Size = new System.Drawing.Size(169, 24);
            this.conditionComboBox.TabIndex = 21;
            this.conditionComboBox.SelectedIndex = 1;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(6, 363);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(67, 16);
            this.label9.TabIndex = 20;
            this.label9.Text = "Condition:";
            // 
            // bookIssue_issueDate
            // 
            this.bookIssue_issueDate.Location = new System.Drawing.Point(92, 311);
            this.bookIssue_issueDate.Name = "bookIssue_issueDate";
            this.bookIssue_issueDate.Size = new System.Drawing.Size(169, 20);
            this.bookIssue_issueDate.TabIndex = 19;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(29, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(146, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "All Return Books";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // returnBooks_clearBtn
            // 
            this.returnBooks_clearBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(128)))), ((int)(((byte)(87)))));
            this.returnBooks_clearBtn.FlatAppearance.BorderSize = 0;
            this.returnBooks_clearBtn.FlatAppearance.CheckedBackColor = System.Drawing.Color.DarkSeaGreen;
            this.returnBooks_clearBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkSeaGreen;
            this.returnBooks_clearBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.returnBooks_clearBtn.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.returnBooks_clearBtn.ForeColor = System.Drawing.Color.White;
            this.returnBooks_clearBtn.Location = new System.Drawing.Point(161, 502);
            this.returnBooks_clearBtn.Name = "returnBooks_clearBtn";
            this.returnBooks_clearBtn.Size = new System.Drawing.Size(99, 34);
            this.returnBooks_clearBtn.TabIndex = 16;
            this.returnBooks_clearBtn.Text = "CLEAR";
            this.returnBooks_clearBtn.UseVisualStyleBackColor = false;
            this.returnBooks_clearBtn.Click += new System.EventHandler(this.returnBooks_clearBtn_Click);
            // 
            // returnBooks_returnBtn
            // 
            this.returnBooks_returnBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(128)))), ((int)(((byte)(87)))));
            this.returnBooks_returnBtn.FlatAppearance.BorderSize = 0;
            this.returnBooks_returnBtn.FlatAppearance.CheckedBackColor = System.Drawing.Color.DarkSeaGreen;
            this.returnBooks_returnBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkSeaGreen;
            this.returnBooks_returnBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.returnBooks_returnBtn.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.returnBooks_returnBtn.ForeColor = System.Drawing.Color.White;
            this.returnBooks_returnBtn.Location = new System.Drawing.Point(26, 502);
            this.returnBooks_returnBtn.Name = "returnBooks_returnBtn";
            this.returnBooks_returnBtn.Size = new System.Drawing.Size(99, 34);
            this.returnBooks_returnBtn.TabIndex = 15;
            this.returnBooks_returnBtn.Text = "RETURN";
            this.returnBooks_returnBtn.UseVisualStyleBackColor = false;
            this.returnBooks_returnBtn.Click += new System.EventHandler(this.returnBooks_returnBtn_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(6, 315);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(80, 16);
            this.label8.TabIndex = 13;
            this.label8.Text = "Book Issued:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // returnBooks_author
            // 
            this.returnBooks_author.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.returnBooks_author.Location = new System.Drawing.Point(92, 261);
            this.returnBooks_author.Name = "returnBooks_author";
            this.returnBooks_author.Size = new System.Drawing.Size(168, 22);
            this.returnBooks_author.TabIndex = 12;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(35, 264);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(50, 16);
            this.label7.TabIndex = 11;
            this.label7.Text = "Author:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // returnBooks_bookTitle
            // 
            this.returnBooks_bookTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.returnBooks_bookTitle.Location = new System.Drawing.Point(92, 213);
            this.returnBooks_bookTitle.Name = "returnBooks_bookTitle";
            this.returnBooks_bookTitle.Size = new System.Drawing.Size(168, 22);
            this.returnBooks_bookTitle.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(20, 216);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(69, 16);
            this.label6.TabIndex = 9;
            this.label6.Text = "Book TItle:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // returnBooks_name
            // 
            this.returnBooks_name.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.returnBooks_name.Location = new System.Drawing.Point(91, 170);
            this.returnBooks_name.Name = "returnBooks_name";
            this.returnBooks_name.Size = new System.Drawing.Size(168, 22);
            this.returnBooks_name.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(43, 174);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "Name:";
            // 
            // returnBooks_issueID
            // 
            this.returnBooks_issueID.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.returnBooks_issueID.Location = new System.Drawing.Point(91, 123);
            this.returnBooks_issueID.Name = "returnBooks_issueID";
            this.returnBooks_issueID.Size = new System.Drawing.Size(168, 22);
            this.returnBooks_issueID.TabIndex = 1;
            this.returnBooks_issueID.TextChanged += new System.EventHandler(this.returnBooks_issueID_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(30, 127);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 16);
            this.label2.TabIndex = 0;
            this.label2.Text = "Issue ID:";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.dataGridView1);
            this.panel2.Location = new System.Drawing.Point(312, 18);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(784, 640);
            this.panel2.TabIndex = 1;
            this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(128)))), ((int)(((byte)(87)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Arial Rounded MT Bold", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView1.EnableHeadersVisualStyles = false;
            this.dataGridView1.Location = new System.Drawing.Point(19, 60);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.ControlLight;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.Size = new System.Drawing.Size(750, 543);
            this.dataGridView1.TabIndex = 3;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            // 
            // ReturnBooks
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "ReturnBooks";
            this.Size = new System.Drawing.Size(1110, 682);
            this.Load += new System.EventHandler(this.ReturnBooks_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox returnBooks_name;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox returnBooks_issueID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox returnBooks_author;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox returnBooks_bookTitle;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button returnBooks_clearBtn;
        private System.Windows.Forms.Button returnBooks_returnBtn;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DateTimePicker bookIssue_issueDate;
        private System.Windows.Forms.ComboBox conditionComboBox;
        private System.Windows.Forms.Label label9;
    }
}
