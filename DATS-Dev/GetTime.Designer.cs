namespace DATS_Timesheets
{
    partial class GetTime
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
            this.label2 = new System.Windows.Forms.Label();
            this.startAMPM = new System.Windows.Forms.ComboBox();
            this.startH = new System.Windows.Forms.ComboBox();
            this.startM = new System.Windows.Forms.ComboBox();
            this.endM = new System.Windows.Forms.ComboBox();
            this.endH = new System.Windows.Forms.ComboBox();
            this.endAMPM = new System.Windows.Forms.ComboBox();
            this.continueButton = new System.Windows.Forms.Button();
            this.hourBox = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.hourBox)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(13, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(172, 25);
            this.label1.TabIndex = 51;
            this.label1.Text = "When did you start";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(13, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(171, 25);
            this.label2.TabIndex = 53;
            this.label2.Text = "When did you stop";
            // 
            // startAMPM
            // 
            this.startAMPM.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.startAMPM.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.startAMPM.FormattingEnabled = true;
            this.startAMPM.Items.AddRange(new object[] {
            "AM",
            "PM"});
            this.startAMPM.Location = new System.Drawing.Point(151, 37);
            this.startAMPM.Name = "startAMPM";
            this.startAMPM.Size = new System.Drawing.Size(76, 29);
            this.startAMPM.TabIndex = 57;
            this.startAMPM.SelectedIndexChanged += new System.EventHandler(this.updateVariables);
            // 
            // startH
            // 
            this.startH.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.startH.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.startH.FormattingEnabled = true;
            this.startH.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12"});
            this.startH.Location = new System.Drawing.Point(18, 37);
            this.startH.Name = "startH";
            this.startH.Size = new System.Drawing.Size(54, 29);
            this.startH.TabIndex = 58;
            this.startH.SelectedIndexChanged += new System.EventHandler(this.updateVariables);
            // 
            // startM
            // 
            this.startM.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.startM.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.startM.FormattingEnabled = true;
            this.startM.Items.AddRange(new object[] {
            ":00",
            ":15",
            ":30",
            ":45"});
            this.startM.Location = new System.Drawing.Point(78, 37);
            this.startM.Name = "startM";
            this.startM.Size = new System.Drawing.Size(67, 29);
            this.startM.TabIndex = 59;
            this.startM.SelectedIndexChanged += new System.EventHandler(this.updateVariables);
            // 
            // endM
            // 
            this.endM.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.endM.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.endM.FormattingEnabled = true;
            this.endM.Items.AddRange(new object[] {
            ":00",
            ":15",
            ":30",
            ":45"});
            this.endM.Location = new System.Drawing.Point(78, 116);
            this.endM.Name = "endM";
            this.endM.Size = new System.Drawing.Size(67, 29);
            this.endM.TabIndex = 62;
            this.endM.SelectedIndexChanged += new System.EventHandler(this.updateVariables);
            // 
            // endH
            // 
            this.endH.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.endH.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.endH.FormattingEnabled = true;
            this.endH.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12"});
            this.endH.Location = new System.Drawing.Point(18, 116);
            this.endH.Name = "endH";
            this.endH.Size = new System.Drawing.Size(54, 29);
            this.endH.TabIndex = 61;
            this.endH.SelectedIndexChanged += new System.EventHandler(this.updateVariables);
            // 
            // endAMPM
            // 
            this.endAMPM.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.endAMPM.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.endAMPM.FormattingEnabled = true;
            this.endAMPM.Items.AddRange(new object[] {
            "AM",
            "PM"});
            this.endAMPM.Location = new System.Drawing.Point(151, 116);
            this.endAMPM.Name = "endAMPM";
            this.endAMPM.Size = new System.Drawing.Size(76, 29);
            this.endAMPM.TabIndex = 60;
            this.endAMPM.SelectedIndexChanged += new System.EventHandler(this.updateVariables);
            // 
            // continueButton
            // 
            this.continueButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.continueButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(255)))));
            this.continueButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.continueButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.continueButton.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.continueButton.ForeColor = System.Drawing.Color.White;
            this.continueButton.Location = new System.Drawing.Point(115, 236);
            this.continueButton.Name = "continueButton";
            this.continueButton.Size = new System.Drawing.Size(116, 56);
            this.continueButton.TabIndex = 65;
            this.continueButton.Text = "Submit";
            this.continueButton.UseVisualStyleBackColor = false;
            this.continueButton.Click += new System.EventHandler(this.continueButton_Click);
            // 
            // hourBox
            // 
            this.hourBox.DecimalPlaces = 3;
            this.hourBox.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.hourBox.Increment = new decimal(new int[] {
            25,
            0,
            0,
            131072});
            this.hourBox.Location = new System.Drawing.Point(18, 193);
            this.hourBox.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.hourBox.Name = "hourBox";
            this.hourBox.Size = new System.Drawing.Size(91, 29);
            this.hourBox.TabIndex = 67;
            this.hourBox.Value = new decimal(new int[] {
            1250,
            0,
            0,
            131072});
            this.hourBox.ValueChanged += new System.EventHandler(this.hourBox_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(11, 165);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(216, 25);
            this.label3.TabIndex = 54;
            this.label3.Text = "How many hours is this?";
            // 
            // GetTime
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(243, 304);
            this.Controls.Add(this.hourBox);
            this.Controls.Add(this.continueButton);
            this.Controls.Add(this.endM);
            this.Controls.Add(this.endH);
            this.Controls.Add(this.endAMPM);
            this.Controls.Add(this.startM);
            this.Controls.Add(this.startH);
            this.Controls.Add(this.startAMPM);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GetTime";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Get Time";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GetTime_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.hourBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox startAMPM;
        private System.Windows.Forms.ComboBox startH;
        private System.Windows.Forms.ComboBox startM;
        private System.Windows.Forms.ComboBox endM;
        private System.Windows.Forms.ComboBox endH;
        private System.Windows.Forms.ComboBox endAMPM;
        private System.Windows.Forms.Button continueButton;
        private System.Windows.Forms.NumericUpDown hourBox;
        private System.Windows.Forms.Label label3;

    }
}