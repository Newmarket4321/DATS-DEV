namespace DATS_Timesheets
{
    partial class Account
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button8 = new System.Windows.Forms.Button();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.InactiveUser = new System.Windows.Forms.RadioButton();
            this.EntersTimesheets = new System.Windows.Forms.RadioButton();
            this.Enter_Review_Timesheet = new System.Windows.Forms.RadioButton();
            this.ReviewsTimesheets = new System.Windows.Forms.RadioButton();
            this.ApprovesTimesheets = new System.Windows.Forms.RadioButton();
            this.Enter_Approve_Timesheets = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.Viewonlyuser = new System.Windows.Forms.RadioButton();
            this.Admin = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(13, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(196, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "Computer Logon";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(171, 8);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(257, 37);
            this.textBox1.TabIndex = 1;
            // 
            // textBox2
            // 
            this.textBox2.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.Location = new System.Drawing.Point(171, 82);
            this.textBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(257, 37);
            this.textBox2.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(13, 83);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(150, 32);
            this.label3.TabIndex = 41;
            this.label3.Text = "Employee ID";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button8
            // 
            this.button8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(255)))));
            this.button8.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button8.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.button8.ForeColor = System.Drawing.Color.White;
            this.button8.Location = new System.Drawing.Point(558, 374);
            this.button8.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(190, 38);
            this.button8.TabIndex = 7;
            this.button8.Text = "Save";
            this.button8.UseVisualStyleBackColor = false;
            this.button8.Click += new System.EventHandler(this.button2_Click);
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.CheckOnClick = true;
            this.checkedListBox1.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(558, 8);
            this.checkedListBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(190, 324);
            this.checkedListBox1.TabIndex = 57;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(431, 9);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(153, 32);
            this.label4.TabIndex = 58;
            this.label4.Text = "Departments";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBox3
            // 
            this.textBox3.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox3.Location = new System.Drawing.Point(171, 45);
            this.textBox3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(257, 37);
            this.textBox3.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(13, 46);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(163, 32);
            this.label5.TabIndex = 62;
            this.label5.Text = "Display Name";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // InactiveUser
            // 
            this.InactiveUser.AutoSize = true;
            this.InactiveUser.ForeColor = System.Drawing.Color.Red;
            this.InactiveUser.Location = new System.Drawing.Point(171, 153);
            this.InactiveUser.Name = "InactiveUser";
            this.InactiveUser.Size = new System.Drawing.Size(122, 36);
            this.InactiveUser.TabIndex = 67;
            this.InactiveUser.TabStop = true;
            this.InactiveUser.Text = "Inactive";
            this.InactiveUser.UseVisualStyleBackColor = true;
            // 
            // EntersTimesheets
            // 
            this.EntersTimesheets.AutoSize = true;
            this.EntersTimesheets.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.EntersTimesheets.Location = new System.Drawing.Point(171, 184);
            this.EntersTimesheets.Name = "EntersTimesheets";
            this.EntersTimesheets.Size = new System.Drawing.Size(228, 36);
            this.EntersTimesheets.TabIndex = 68;
            this.EntersTimesheets.TabStop = true;
            this.EntersTimesheets.Text = "Enters timesheets";
            this.EntersTimesheets.UseVisualStyleBackColor = true;
            // 
            // Enter_Review_Timesheet
            // 
            this.Enter_Review_Timesheet.AutoSize = true;
            this.Enter_Review_Timesheet.ForeColor = System.Drawing.Color.Blue;
            this.Enter_Review_Timesheet.Location = new System.Drawing.Point(171, 215);
            this.Enter_Review_Timesheet.Name = "Enter_Review_Timesheet";
            this.Enter_Review_Timesheet.Size = new System.Drawing.Size(361, 36);
            this.Enter_Review_Timesheet.TabIndex = 69;
            this.Enter_Review_Timesheet.TabStop = true;
            this.Enter_Review_Timesheet.Text = "Enters and reviews timesheets";
            this.Enter_Review_Timesheet.UseVisualStyleBackColor = true;
            // 
            // ReviewsTimesheets
            // 
            this.ReviewsTimesheets.AutoSize = true;
            this.ReviewsTimesheets.ForeColor = System.Drawing.Color.Blue;
            this.ReviewsTimesheets.Location = new System.Drawing.Point(171, 246);
            this.ReviewsTimesheets.Name = "ReviewsTimesheets";
            this.ReviewsTimesheets.Size = new System.Drawing.Size(247, 36);
            this.ReviewsTimesheets.TabIndex = 70;
            this.ReviewsTimesheets.TabStop = true;
            this.ReviewsTimesheets.Text = "Reviews timesheets";
            this.ReviewsTimesheets.UseVisualStyleBackColor = true;
            // 
            // ApprovesTimesheets
            // 
            this.ApprovesTimesheets.AutoSize = true;
            this.ApprovesTimesheets.ForeColor = System.Drawing.Color.Purple;
            this.ApprovesTimesheets.Location = new System.Drawing.Point(171, 308);
            this.ApprovesTimesheets.Name = "ApprovesTimesheets";
            this.ApprovesTimesheets.Size = new System.Drawing.Size(263, 36);
            this.ApprovesTimesheets.TabIndex = 72;
            this.ApprovesTimesheets.TabStop = true;
            this.ApprovesTimesheets.Text = "Approves timesheets";
            this.ApprovesTimesheets.UseVisualStyleBackColor = true;
            // 
            // Enter_Approve_Timesheets
            // 
            this.Enter_Approve_Timesheets.AutoSize = true;
            this.Enter_Approve_Timesheets.ForeColor = System.Drawing.Color.Purple;
            this.Enter_Approve_Timesheets.Location = new System.Drawing.Point(171, 277);
            this.Enter_Approve_Timesheets.Name = "Enter_Approve_Timesheets";
            this.Enter_Approve_Timesheets.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Enter_Approve_Timesheets.Size = new System.Drawing.Size(379, 36);
            this.Enter_Approve_Timesheets.TabIndex = 71;
            this.Enter_Approve_Timesheets.TabStop = true;
            this.Enter_Approve_Timesheets.Text = "Enters and approves timesheets";
            this.Enter_Approve_Timesheets.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(13, 155);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(139, 32);
            this.label2.TabIndex = 73;
            this.label2.Text = "Permissions";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(255)))));
            this.button1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(171, 115);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(187, 30);
            this.button1.TabIndex = 74;
            this.button1.Text = "View Entitlements";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(13, 120);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(148, 32);
            this.label6.TabIndex = 75;
            this.label6.Text = "Entitlements";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Location = new System.Drawing.Point(13, 343);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(30, 32);
            this.label7.TabIndex = 77;
            this.label7.Text = "...";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Viewonlyuser
            // 
            this.Viewonlyuser.AutoSize = true;
            this.Viewonlyuser.ForeColor = System.Drawing.Color.Fuchsia;
            this.Viewonlyuser.Location = new System.Drawing.Point(171, 339);
            this.Viewonlyuser.Name = "Viewonlyuser";
            this.Viewonlyuser.Size = new System.Drawing.Size(196, 36);
            this.Viewonlyuser.TabIndex = 78;
            this.Viewonlyuser.TabStop = true;
            this.Viewonlyuser.Text = "View only user";
            this.Viewonlyuser.UseVisualStyleBackColor = true;
            // 
            // Admin
            // 
            this.Admin.AutoSize = true;
            this.Admin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(149)))), ((int)(((byte)(0)))));
            this.Admin.Location = new System.Drawing.Point(19, 392);
            this.Admin.Name = "Admin";
            this.Admin.Size = new System.Drawing.Size(343, 36);
            this.Admin.TabIndex = 79;
            this.Admin.Text = "Permission for Administrator";
            this.Admin.UseVisualStyleBackColor = true;
            // 
            // Account
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(758, 440);
            this.Controls.Add(this.Admin);
            this.Controls.Add(this.Viewonlyuser);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ApprovesTimesheets);
            this.Controls.Add(this.Enter_Approve_Timesheets);
            this.Controls.Add(this.ReviewsTimesheets);
            this.Controls.Add(this.Enter_Review_Timesheet);
            this.Controls.Add(this.EntersTimesheets);
            this.Controls.Add(this.InactiveUser);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.checkedListBox1);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.Name = "Account";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "New Account";
            this.Load += new System.EventHandler(this.NewAccount_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.NewAccount_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton InactiveUser;
        private System.Windows.Forms.RadioButton EntersTimesheets;
        private System.Windows.Forms.RadioButton Enter_Review_Timesheet;
        private System.Windows.Forms.RadioButton ReviewsTimesheets;
        private System.Windows.Forms.RadioButton ApprovesTimesheets;
        private System.Windows.Forms.RadioButton Enter_Approve_Timesheets;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.RadioButton Viewonlyuser;
        private System.Windows.Forms.CheckBox Admin;
    }
}