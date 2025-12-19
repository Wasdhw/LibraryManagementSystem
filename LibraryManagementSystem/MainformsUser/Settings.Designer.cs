namespace LibraryManagementSystem.MainformsUser
{
    partial class Settings
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.resetBtn = new System.Windows.Forms.Button();
            this.saveBtn = new System.Windows.Forms.Button();
            this.renewalDaysNumeric = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.maxRenewalsNumeric = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.fineRateNumeric = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.overdueThresholdNumeric = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.maxBooksNumeric = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.borrowingPeriodNumeric = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.borrowingPeriodNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxBooksNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.overdueThresholdNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fineRateNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxRenewalsNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.renewalDaysNumeric)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1110, 50);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(20, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "SETTINGS";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.resetBtn);
            this.panel2.Controls.Add(this.saveBtn);
            this.panel2.Controls.Add(this.renewalDaysNumeric);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.maxRenewalsNumeric);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.fineRateNumeric);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.overdueThresholdNumeric);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.maxBooksNumeric);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.borrowingPeriodNumeric);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Location = new System.Drawing.Point(14, 60);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1080, 400);
            this.panel2.TabIndex = 1;
            // 
            // resetBtn
            // 
            this.resetBtn.BackColor = System.Drawing.Color.Gray;
            this.resetBtn.FlatAppearance.BorderSize = 0;
            this.resetBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.resetBtn.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.resetBtn.ForeColor = System.Drawing.Color.White;
            this.resetBtn.Location = new System.Drawing.Point(600, 350);
            this.resetBtn.Name = "resetBtn";
            this.resetBtn.Size = new System.Drawing.Size(100, 33);
            this.resetBtn.TabIndex = 13;
            this.resetBtn.Text = "RESET";
            this.resetBtn.UseVisualStyleBackColor = false;
            this.resetBtn.Click += new System.EventHandler(this.resetBtn_Click);
            // 
            // saveBtn
            // 
            this.saveBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(128)))), ((int)(((byte)(87)))));
            this.saveBtn.FlatAppearance.BorderSize = 0;
            this.saveBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveBtn.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveBtn.ForeColor = System.Drawing.Color.White;
            this.saveBtn.Location = new System.Drawing.Point(720, 350);
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(100, 33);
            this.saveBtn.TabIndex = 12;
            this.saveBtn.Text = "SAVE";
            this.saveBtn.UseVisualStyleBackColor = false;
            this.saveBtn.Click += new System.EventHandler(this.saveBtn_Click);
            // 
            // renewalDaysNumeric
            // 
            this.renewalDaysNumeric.Location = new System.Drawing.Point(300, 300);
            this.renewalDaysNumeric.Maximum = new decimal(new int[] {
            365,
            0,
            0,
            0});
            this.renewalDaysNumeric.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.renewalDaysNumeric.Name = "renewalDaysNumeric";
            this.renewalDaysNumeric.Size = new System.Drawing.Size(200, 20);
            this.renewalDaysNumeric.TabIndex = 11;
            this.renewalDaysNumeric.Value = new decimal(new int[] {
            14,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(20, 302);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(87, 16);
            this.label7.TabIndex = 10;
            this.label7.Text = "Renewal Days:";
            // 
            // maxRenewalsNumeric
            // 
            this.maxRenewalsNumeric.Location = new System.Drawing.Point(300, 250);
            this.maxRenewalsNumeric.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.maxRenewalsNumeric.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.maxRenewalsNumeric.Name = "maxRenewalsNumeric";
            this.maxRenewalsNumeric.Size = new System.Drawing.Size(200, 20);
            this.maxRenewalsNumeric.TabIndex = 9;
            this.maxRenewalsNumeric.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(20, 252);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(90, 16);
            this.label6.TabIndex = 8;
            this.label6.Text = "Max Renewals:";
            // 
            // fineRateNumeric
            // 
            this.fineRateNumeric.DecimalPlaces = 2;
            this.fineRateNumeric.Location = new System.Drawing.Point(300, 200);
            this.fineRateNumeric.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.fineRateNumeric.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.fineRateNumeric.Name = "fineRateNumeric";
            this.fineRateNumeric.Size = new System.Drawing.Size(200, 20);
            this.fineRateNumeric.TabIndex = 7;
            this.fineRateNumeric.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(20, 202);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(108, 16);
            this.label5.TabIndex = 6;
            this.label5.Text = "Fine Rate (per day):";
            // 
            // overdueThresholdNumeric
            // 
            this.overdueThresholdNumeric.Location = new System.Drawing.Point(300, 150);
            this.overdueThresholdNumeric.Maximum = new decimal(new int[] {
            365,
            0,
            0,
            0});
            this.overdueThresholdNumeric.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.overdueThresholdNumeric.Name = "overdueThresholdNumeric";
            this.overdueThresholdNumeric.Size = new System.Drawing.Size(200, 20);
            this.overdueThresholdNumeric.TabIndex = 5;
            this.overdueThresholdNumeric.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(20, 152);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(130, 16);
            this.label4.TabIndex = 4;
            this.label4.Text = "Overdue Threshold (days):";
            // 
            // maxBooksNumeric
            // 
            this.maxBooksNumeric.Location = new System.Drawing.Point(300, 100);
            this.maxBooksNumeric.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.maxBooksNumeric.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.maxBooksNumeric.Name = "maxBooksNumeric";
            this.maxBooksNumeric.Size = new System.Drawing.Size(200, 20);
            this.maxBooksNumeric.TabIndex = 3;
            this.maxBooksNumeric.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(20, 102);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(120, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "Max Books Per User:";
            // 
            // borrowingPeriodNumeric
            // 
            this.borrowingPeriodNumeric.Location = new System.Drawing.Point(300, 50);
            this.borrowingPeriodNumeric.Maximum = new decimal(new int[] {
            365,
            0,
            0,
            0});
            this.borrowingPeriodNumeric.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.borrowingPeriodNumeric.Name = "borrowingPeriodNumeric";
            this.borrowingPeriodNumeric.Size = new System.Drawing.Size(200, 20);
            this.borrowingPeriodNumeric.TabIndex = 1;
            this.borrowingPeriodNumeric.Value = new decimal(new int[] {
            14,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(20, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 16);
            this.label2.TabIndex = 0;
            this.label2.Text = "Borrowing Period (days):";
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "Settings";
            this.Size = new System.Drawing.Size(1110, 682);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.borrowingPeriodNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxBooksNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.overdueThresholdNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fineRateNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxRenewalsNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.renewalDaysNumeric)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown borrowingPeriodNumeric;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown maxBooksNumeric;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown overdueThresholdNumeric;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown fineRateNumeric;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown maxRenewalsNumeric;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown renewalDaysNumeric;
        private System.Windows.Forms.Button saveBtn;
        private System.Windows.Forms.Button resetBtn;
    }
}

