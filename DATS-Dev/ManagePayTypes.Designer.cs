namespace DATS_Timesheets
{
    partial class ManagePayTypes
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.PayType_Txt = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Description_txt = new System.Windows.Forms.TextBox();
            this.TypeActive = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.Edit = new System.Windows.Forms.DataGridViewLinkColumn();
            this.Delete = new System.Windows.Forms.DataGridViewLinkColumn();
            this.PayType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PTypeActive = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TransType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Rate_A = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Rate_R = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RegYN = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StdYN = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OTYN = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VacYN = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Rate_Exp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AbsYN = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Colour = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SAL = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ALL_PTC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FFE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HRLY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FAC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PTC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.XGRD = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PTH = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CON = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.O_SCL = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SEIU_C = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(234)))), ((int)(((byte)(240)))));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Edit,
            this.Delete,
            this.PayType,
            this.Description,
            this.PTypeActive,
            this.TransType,
            this.Rate_A,
            this.Rate_R,
            this.RegYN,
            this.StdYN,
            this.OTYN,
            this.VacYN,
            this.Rate_Exp,
            this.AbsYN,
            this.Colour,
            this.SAL,
            this.ALL_PTC,
            this.FFE,
            this.HRLY,
            this.FAC,
            this.PTC,
            this.XGRD,
            this.PTH,
            this.CON,
            this.O_SCL,
            this.SEIU_C});
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
            this.dataGridView1.Location = new System.Drawing.Point(47, 168);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 28;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView1.Size = new System.Drawing.Size(1094, 552);
            this.dataGridView1.TabIndex = 21;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            this.dataGridView1.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView1_DataError);
            // 
            // PayType_Txt
            // 
            this.PayType_Txt.Location = new System.Drawing.Point(243, 64);
            this.PayType_Txt.Name = "PayType_Txt";
            this.PayType_Txt.Size = new System.Drawing.Size(196, 26);
            this.PayType_Txt.TabIndex = 22;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(43, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 20);
            this.label1.TabIndex = 23;
            this.label1.Text = "Pay Type";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(43, 113);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(157, 20);
            this.label2.TabIndex = 24;
            this.label2.Text = "Pay Type Description";
            // 
            // Description_txt
            // 
            this.Description_txt.Location = new System.Drawing.Point(243, 113);
            this.Description_txt.Name = "Description_txt";
            this.Description_txt.Size = new System.Drawing.Size(196, 26);
            this.Description_txt.TabIndex = 25;
            // 
            // TypeActive
            // 
            this.TypeActive.AutoSize = true;
            this.TypeActive.Location = new System.Drawing.Point(468, 113);
            this.TypeActive.Name = "TypeActive";
            this.TypeActive.Size = new System.Drawing.Size(146, 24);
            this.TypeActive.TabIndex = 26;
            this.TypeActive.Text = "Pay Type Active";
            this.TypeActive.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(688, 102);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(141, 37);
            this.button1.TabIndex = 27;
            this.button1.Text = "Submit";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.White;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(9, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(1597, 51);
            this.menuStrip1.TabIndex = 68;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Font = new System.Drawing.Font("Segoe UI", 15F);
            this.toolStripMenuItem1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(255)))));
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(225, 45);
            this.toolStripMenuItem1.Text = "Export to Excel";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // Edit
            // 
            this.Edit.DataPropertyName = "lnkColumn";
            this.Edit.HeaderText = "Edit";
            this.Edit.Name = "Edit";
            this.Edit.Text = "Edit";
            this.Edit.UseColumnTextForLinkValue = true;
            this.Edit.Width = 43;
            // 
            // Delete
            // 
            this.Delete.DataPropertyName = "lnkColumn";
            this.Delete.HeaderText = "Delete";
            this.Delete.Name = "Delete";
            this.Delete.Text = "Delete";
            this.Delete.UseColumnTextForLinkValue = true;
            this.Delete.Width = 62;
            // 
            // PayType
            // 
            this.PayType.DataPropertyName = "PayType";
            this.PayType.HeaderText = "Pay Type";
            this.PayType.Name = "PayType";
            this.PayType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.PayType.Width = 79;
            // 
            // Description
            // 
            this.Description.DataPropertyName = "Description";
            this.Description.HeaderText = "Description";
            this.Description.Name = "Description";
            this.Description.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Description.Width = 95;
            // 
            // PTypeActive
            // 
            this.PTypeActive.DataPropertyName = "PayTypeActive";
            this.PTypeActive.HeaderText = "PayTypeActive";
            this.PTypeActive.Name = "PTypeActive";
            this.PTypeActive.Width = 148;
            // 
            // TransType
            // 
            this.TransType.DataPropertyName = "TransType";
            this.TransType.HeaderText = "TransType";
            this.TransType.Name = "TransType";
            this.TransType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.TransType.Width = 89;
            // 
            // Rate_A
            // 
            this.Rate_A.DataPropertyName = "Rate_A";
            this.Rate_A.HeaderText = "Rate A";
            this.Rate_A.Name = "Rate_A";
            this.Rate_A.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Rate_A.Width = 65;
            // 
            // Rate_R
            // 
            this.Rate_R.DataPropertyName = "Rate_R";
            this.Rate_R.HeaderText = "Rate R";
            this.Rate_R.Name = "Rate_R";
            this.Rate_R.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Rate_R.Width = 66;
            // 
            // RegYN
            // 
            this.RegYN.DataPropertyName = "RegYN";
            this.RegYN.HeaderText = "RegYN";
            this.RegYN.Name = "RegYN";
            this.RegYN.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.RegYN.Width = 67;
            // 
            // StdYN
            // 
            this.StdYN.DataPropertyName = "StdYN";
            this.StdYN.HeaderText = "StdYN";
            this.StdYN.Name = "StdYN";
            this.StdYN.ReadOnly = true;
            this.StdYN.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.StdYN.Width = 62;
            // 
            // OTYN
            // 
            this.OTYN.DataPropertyName = "OTYN";
            this.OTYN.HeaderText = "OTYN";
            this.OTYN.Name = "OTYN";
            this.OTYN.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.OTYN.Width = 58;
            // 
            // VacYN
            // 
            this.VacYN.DataPropertyName = "VacYN";
            this.VacYN.HeaderText = "VacYN";
            this.VacYN.Name = "VacYN";
            this.VacYN.ReadOnly = true;
            this.VacYN.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.VacYN.Width = 65;
            // 
            // Rate_Exp
            // 
            this.Rate_Exp.DataPropertyName = "Rate_Exp";
            this.Rate_Exp.HeaderText = "Rate Exp";
            this.Rate_Exp.Name = "Rate_Exp";
            this.Rate_Exp.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Rate_Exp.Width = 81;
            // 
            // AbsYN
            // 
            this.AbsYN.DataPropertyName = "AbsYN";
            this.AbsYN.HeaderText = "AbsYN";
            this.AbsYN.Name = "AbsYN";
            this.AbsYN.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.AbsYN.Width = 65;
            // 
            // Colour
            // 
            this.Colour.DataPropertyName = "Colour";
            this.Colour.HeaderText = "Colour";
            this.Colour.Name = "Colour";
            this.Colour.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Colour.Width = 61;
            // 
            // SAL
            // 
            this.SAL.DataPropertyName = "SAL";
            this.SAL.HeaderText = "SAL";
            this.SAL.Name = "SAL";
            this.SAL.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.SAL.Width = 46;
            // 
            // ALL_PTC
            // 
            this.ALL_PTC.DataPropertyName = "EXE";
            this.ALL_PTC.HeaderText = "EXE";
            this.ALL_PTC.Name = "EXE";
            this.ALL_PTC.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ALL_PTC.Width = 48;
            // 
            // FFE
            // 
            this.FFE.DataPropertyName = "FFE";
            this.FFE.HeaderText = "FFE";
            this.FFE.Name = "FFE";
            this.FFE.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.FFE.Width = 46;
            // 
            // HRLY
            // 
            this.HRLY.DataPropertyName = "HRLY";
            this.HRLY.HeaderText = "HRLY";
            this.HRLY.Name = "HRLY";
            this.HRLY.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.HRLY.Width = 59;
            // 
            // FAC
            // 
            this.FAC.DataPropertyName = "FAC";
            this.FAC.HeaderText = "FAC";
            this.FAC.Name = "FAC";
            this.FAC.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.FAC.Width = 47;
            // 
            // PTC
            // 
            this.PTC.DataPropertyName = "PTC";
            this.PTC.HeaderText = "PTC";
            this.PTC.Name = "PTC";
            this.PTC.ReadOnly = true;
            this.PTC.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.PTC.Width = 45;
            // 
            // XGRD
            // 
            this.XGRD.DataPropertyName = "XGRD";
            this.XGRD.HeaderText = "XGRD";
            this.XGRD.Name = "XGRD";
            this.XGRD.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.XGRD.Width = 63;
            // 
            // PTH
            // 
            this.PTH.DataPropertyName = "PTH";
            this.PTH.HeaderText = "PTH";
            this.PTH.Name = "PTH";
            this.PTH.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.PTH.Width = 46;
            // 
            // CON
            // 
            this.CON.DataPropertyName = "CON";
            this.CON.HeaderText = "CON";
            this.CON.Name = "CON";
            this.CON.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.CON.Width = 49;
            // 
            // O_SCL
            // 
            this.O_SCL.DataPropertyName = "O_SCL";
            this.O_SCL.HeaderText = "O_SCL";
            this.O_SCL.Name = "O_SCL";
            this.O_SCL.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.O_SCL.Width = 67;
            // 
            // SEIU_C
            // 
            this.SEIU_C.DataPropertyName = "SEIU_C";
            this.SEIU_C.HeaderText = "SEIU_C";
            this.SEIU_C.Name = "SEIU_C";
            this.SEIU_C.Width = 104;
            // 
            // ManagePayTypes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1597, 734);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.TypeActive);
            this.Controls.Add(this.Description_txt);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PayType_Txt);
            this.Controls.Add(this.dataGridView1);
            this.Name = "ManagePayTypes";
            this.Text = "ManagePayTypescs";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox PayType_Txt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Description_txt;
        private System.Windows.Forms.CheckBox TypeActive;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.DataGridViewLinkColumn Edit;
        private System.Windows.Forms.DataGridViewLinkColumn Delete;
        private System.Windows.Forms.DataGridViewTextBoxColumn PayType;
        private System.Windows.Forms.DataGridViewTextBoxColumn Description;
        private System.Windows.Forms.DataGridViewTextBoxColumn PTypeActive;
        private System.Windows.Forms.DataGridViewTextBoxColumn TransType;
        private System.Windows.Forms.DataGridViewTextBoxColumn Rate_A;
        private System.Windows.Forms.DataGridViewTextBoxColumn Rate_R;
        private System.Windows.Forms.DataGridViewTextBoxColumn RegYN;
        private System.Windows.Forms.DataGridViewTextBoxColumn StdYN;
        private System.Windows.Forms.DataGridViewTextBoxColumn OTYN;
        private System.Windows.Forms.DataGridViewTextBoxColumn VacYN;
        private System.Windows.Forms.DataGridViewTextBoxColumn Rate_Exp;
        private System.Windows.Forms.DataGridViewTextBoxColumn AbsYN;
        private System.Windows.Forms.DataGridViewTextBoxColumn Colour;
        private System.Windows.Forms.DataGridViewTextBoxColumn SAL;
        private System.Windows.Forms.DataGridViewTextBoxColumn ALL_PTC;
        private System.Windows.Forms.DataGridViewTextBoxColumn FFE;
        private System.Windows.Forms.DataGridViewTextBoxColumn HRLY;
        private System.Windows.Forms.DataGridViewTextBoxColumn FAC;
        private System.Windows.Forms.DataGridViewTextBoxColumn PTC;
        private System.Windows.Forms.DataGridViewTextBoxColumn XGRD;
        private System.Windows.Forms.DataGridViewTextBoxColumn PTH;
        private System.Windows.Forms.DataGridViewTextBoxColumn CON;
        private System.Windows.Forms.DataGridViewTextBoxColumn O_SCL;
        private System.Windows.Forms.DataGridViewTextBoxColumn SEIU_C;
    }
}