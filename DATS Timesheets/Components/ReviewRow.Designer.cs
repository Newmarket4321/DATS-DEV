namespace DATS_Timesheets
{
    partial class ReviewRow
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
            this.dateWorkedBox = new System.Windows.Forms.DateTimePicker();
            this.payTypeBox = new System.Windows.Forms.ComboBox();
            this.hoursBox = new System.Windows.Forms.NumericUpDown();
            this.workOrderBox = new System.Windows.Forms.TextBox();
            this.workOrderLabel = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.hoursBox)).BeginInit();
            this.SuspendLayout();
            // 
            // dateWorkedBox
            // 
            this.dateWorkedBox.Location = new System.Drawing.Point(3, 3);
            this.dateWorkedBox.Name = "dateWorkedBox";
            this.dateWorkedBox.Size = new System.Drawing.Size(136, 20);
            this.dateWorkedBox.TabIndex = 1;
            // 
            // payTypeBox
            // 
            this.payTypeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.payTypeBox.FormattingEnabled = true;
            this.payTypeBox.Location = new System.Drawing.Point(198, 3);
            this.payTypeBox.Name = "payTypeBox";
            this.payTypeBox.Size = new System.Drawing.Size(121, 21);
            this.payTypeBox.TabIndex = 3;
            // 
            // hoursBox
            // 
            this.hoursBox.Location = new System.Drawing.Point(145, 3);
            this.hoursBox.Name = "hoursBox";
            this.hoursBox.Size = new System.Drawing.Size(47, 20);
            this.hoursBox.TabIndex = 5;
            // 
            // workOrderBox
            // 
            this.workOrderBox.Location = new System.Drawing.Point(325, 3);
            this.workOrderBox.Name = "workOrderBox";
            this.workOrderBox.Size = new System.Drawing.Size(44, 20);
            this.workOrderBox.TabIndex = 6;
            this.workOrderBox.Text = "123456";
            this.workOrderBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.workOrderBox_KeyUp);
            // 
            // workOrderLabel
            // 
            this.workOrderLabel.Location = new System.Drawing.Point(375, 6);
            this.workOrderLabel.Name = "workOrderLabel";
            this.workOrderLabel.Size = new System.Drawing.Size(125, 17);
            this.workOrderLabel.TabIndex = 7;
            this.workOrderLabel.Text = "label1";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(506, 2);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 8;
            this.comboBox1.DropDown += new System.EventHandler(this.comboBox1_DropDown);
            // 
            // comboBox2
            // 
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(633, 2);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(121, 21);
            this.comboBox2.TabIndex = 9;
            // 
            // ReviewRow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.workOrderLabel);
            this.Controls.Add(this.workOrderBox);
            this.Controls.Add(this.hoursBox);
            this.Controls.Add(this.payTypeBox);
            this.Controls.Add(this.dateWorkedBox);
            this.Name = "ReviewRow";
            this.Size = new System.Drawing.Size(908, 26);
            ((System.ComponentModel.ISupportInitialize)(this.hoursBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateWorkedBox;
        private System.Windows.Forms.ComboBox payTypeBox;
        private System.Windows.Forms.NumericUpDown hoursBox;
        private System.Windows.Forms.TextBox workOrderBox;
        private System.Windows.Forms.Label workOrderLabel;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ComboBox comboBox2;
    }
}
