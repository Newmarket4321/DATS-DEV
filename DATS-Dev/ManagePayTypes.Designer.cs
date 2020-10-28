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
            this.Edit = new System.Windows.Forms.DataGridViewLinkColumn();
            this.Delete = new System.Windows.Forms.DataGridViewLinkColumn();
            this.PayType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PayTypeActive = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.ALL_Salary = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ALL_PTC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ALL_PTH = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ALL_SEIU_Contract = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OpsOffice_HRLY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Roads_HRLY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Fleet_HRLY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Water_HRLY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Parks_HRLY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.F_Maintanence_Operations = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
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
            this.PayTypeActive,
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
            this.ALL_Salary,
            this.ALL_PTC,
            this.ALL_PTH,
            this.ALL_SEIU_Contract,
            this.OpsOffice_HRLY,
            this.Roads_HRLY,
            this.Fleet_HRLY,
            this.Water_HRLY,
            this.Parks_HRLY,
            this.F_Maintanence_Operations});
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
            this.dataGridView1.Location = new System.Drawing.Point(47, 159);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 28;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView1.Size = new System.Drawing.Size(1094, 552);
            this.dataGridView1.TabIndex = 21;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            // 
            // PayType_Txt
            // 
            this.PayType_Txt.Location = new System.Drawing.Point(209, 35);
            this.PayType_Txt.Name = "PayType_Txt";
            this.PayType_Txt.Size = new System.Drawing.Size(196, 26);
            this.PayType_Txt.TabIndex = 22;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 20);
            this.label1.TabIndex = 23;
            this.label1.Text = "Pay Type";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(157, 20);
            this.label2.TabIndex = 24;
            this.label2.Text = "Pay Type Description";
            // 
            // Description_txt
            // 
            this.Description_txt.Location = new System.Drawing.Point(209, 69);
            this.Description_txt.Name = "Description_txt";
            this.Description_txt.Size = new System.Drawing.Size(196, 26);
            this.Description_txt.TabIndex = 25;
            // 
            // TypeActive
            // 
            this.TypeActive.AutoSize = true;
            this.TypeActive.Location = new System.Drawing.Point(497, 71);
            this.TypeActive.Name = "TypeActive";
            this.TypeActive.Size = new System.Drawing.Size(146, 24);
            this.TypeActive.TabIndex = 26;
            this.TypeActive.Text = "Pay Type Active";
            this.TypeActive.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(708, 64);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(128, 37);
            this.button1.TabIndex = 27;
            this.button1.Text = "Submit";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
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
            this.PayType.Width = 109;
            // 
            // Description
            // 
            this.Description.DataPropertyName = "Description";
            this.Description.HeaderText = "Description";
            this.Description.Name = "Description";
            this.Description.Width = 125;
            // 
            // PayTypeActive
            // 
            this.PayTypeActive.DataPropertyName = "PayTypeActive";
            this.PayTypeActive.HeaderText = "PayType Active";
            this.PayTypeActive.Name = "PayTypeActive";
            this.PayTypeActive.Width = 152;
            // 
            // TransType
            // 
            this.TransType.DataPropertyName = "TransType";
            this.TransType.HeaderText = "TransType";
            this.TransType.Name = "TransType";
            this.TransType.Width = 119;
            // 
            // Rate_A
            // 
            this.Rate_A.DataPropertyName = "Rate_A";
            this.Rate_A.HeaderText = "Rate A";
            this.Rate_A.Name = "Rate_A";
            this.Rate_A.Width = 95;
            // 
            // Rate_R
            // 
            this.Rate_R.DataPropertyName = "Rate_R";
            this.Rate_R.HeaderText = "Rate R";
            this.Rate_R.Name = "Rate_R";
            this.Rate_R.Width = 96;
            // 
            // RegYN
            // 
            this.RegYN.DataPropertyName = "RegYN";
            this.RegYN.HeaderText = "RegYN";
            this.RegYN.Name = "RegYN";
            this.RegYN.Width = 97;
            // 
            // StdYN
            // 
            this.StdYN.DataPropertyName = "StdYN";
            this.StdYN.HeaderText = "StdYN";
            this.StdYN.Name = "StdYN";
            this.StdYN.ReadOnly = true;
            this.StdYN.Width = 92;
            // 
            // OTYN
            // 
            this.OTYN.DataPropertyName = "OTYN";
            this.OTYN.HeaderText = "OTYN";
            this.OTYN.Name = "OTYN";
            this.OTYN.Width = 88;
            // 
            // VacYN
            // 
            this.VacYN.DataPropertyName = "VacYN";
            this.VacYN.HeaderText = "VacYN";
            this.VacYN.Name = "VacYN";
            this.VacYN.ReadOnly = true;
            this.VacYN.Width = 95;
            // 
            // Rate_Exp
            // 
            this.Rate_Exp.DataPropertyName = "Rate_Exp";
            this.Rate_Exp.HeaderText = "Rate Exp";
            this.Rate_Exp.Name = "Rate_Exp";
            this.Rate_Exp.Width = 111;
            // 
            // AbsYN
            // 
            this.AbsYN.DataPropertyName = "AbsYN";
            this.AbsYN.HeaderText = "AbsYN";
            this.AbsYN.Name = "AbsYN";
            this.AbsYN.Width = 95;
            // 
            // Colour
            // 
            this.Colour.DataPropertyName = "Colour";
            this.Colour.HeaderText = "Colour";
            this.Colour.Name = "Colour";
            this.Colour.Width = 91;
            // 
            // ALL_Salary
            // 
            this.ALL_Salary.DataPropertyName = "ALL_Salary";
            this.ALL_Salary.HeaderText = "ALL Salary";
            this.ALL_Salary.Name = "ALL_Salary";
            this.ALL_Salary.Width = 122;
            // 
            // ALL_PTC
            // 
            this.ALL_PTC.DataPropertyName = "ALL_PTC";
            this.ALL_PTC.HeaderText = "ALL PTC";
            this.ALL_PTC.Name = "ALL_PTC";
            this.ALL_PTC.Width = 108;
            // 
            // ALL_PTH
            // 
            this.ALL_PTH.DataPropertyName = "ALL_PTH";
            this.ALL_PTH.HeaderText = "ALL PTH";
            this.ALL_PTH.Name = "ALL_PTH";
            this.ALL_PTH.Width = 109;
            // 
            // ALL_SEIU_Contract
            // 
            this.ALL_SEIU_Contract.DataPropertyName = "ALL_SEIU_Contract";
            this.ALL_SEIU_Contract.HeaderText = "ALL SEIU Contract";
            this.ALL_SEIU_Contract.Name = "ALL_SEIU_Contract";
            this.ALL_SEIU_Contract.Width = 167;
            // 
            // OpsOffice_HRLY
            // 
            this.OpsOffice_HRLY.DataPropertyName = "OpsOffice_HRLY";
            this.OpsOffice_HRLY.HeaderText = "OpsOffice HRLY";
            this.OpsOffice_HRLY.Name = "OpsOffice_HRLY";
            this.OpsOffice_HRLY.Width = 151;
            // 
            // Roads_HRLY
            // 
            this.Roads_HRLY.DataPropertyName = "Roads_HRLY";
            this.Roads_HRLY.HeaderText = "Roads HRLY";
            this.Roads_HRLY.Name = "Roads_HRLY";
            this.Roads_HRLY.ReadOnly = true;
            this.Roads_HRLY.Width = 129;
            // 
            // Fleet_HRLY
            // 
            this.Fleet_HRLY.DataPropertyName = "Fleet_HRLY";
            this.Fleet_HRLY.HeaderText = "Fleet HRLY";
            this.Fleet_HRLY.Name = "Fleet_HRLY";
            this.Fleet_HRLY.Width = 119;
            // 
            // Water_HRLY
            // 
            this.Water_HRLY.DataPropertyName = "Water_HRLY";
            this.Water_HRLY.HeaderText = "Water HRLY";
            this.Water_HRLY.Name = "Water_HRLY";
            this.Water_HRLY.Width = 125;
            // 
            // Parks_HRLY
            // 
            this.Parks_HRLY.DataPropertyName = "Parks_HRLY";
            this.Parks_HRLY.HeaderText = "Parks HRLY";
            this.Parks_HRLY.Name = "Parks_HRLY";
            this.Parks_HRLY.Width = 123;
            // 
            // F_Maintanence_Operations
            // 
            this.F_Maintanence_Operations.DataPropertyName = "F_Maintanence_Operations";
            this.F_Maintanence_Operations.HeaderText = "Facilities Maintanence/Operations";
            this.F_Maintanence_Operations.Name = "F_Maintanence_Operations";
            this.F_Maintanence_Operations.Width = 259;
            // 
            // ManagePayTypes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1597, 734);
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
        private System.Windows.Forms.DataGridViewLinkColumn Edit;
        private System.Windows.Forms.DataGridViewLinkColumn Delete;
        private System.Windows.Forms.DataGridViewTextBoxColumn PayType;
        private System.Windows.Forms.DataGridViewTextBoxColumn Description;
        private System.Windows.Forms.DataGridViewTextBoxColumn PayTypeActive;
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
        private System.Windows.Forms.DataGridViewTextBoxColumn ALL_Salary;
        private System.Windows.Forms.DataGridViewTextBoxColumn ALL_PTC;
        private System.Windows.Forms.DataGridViewTextBoxColumn ALL_PTH;
        private System.Windows.Forms.DataGridViewTextBoxColumn ALL_SEIU_Contract;
        private System.Windows.Forms.DataGridViewTextBoxColumn OpsOffice_HRLY;
        private System.Windows.Forms.DataGridViewTextBoxColumn Roads_HRLY;
        private System.Windows.Forms.DataGridViewTextBoxColumn Fleet_HRLY;
        private System.Windows.Forms.DataGridViewTextBoxColumn Water_HRLY;
        private System.Windows.Forms.DataGridViewTextBoxColumn Parks_HRLY;
        private System.Windows.Forms.DataGridViewTextBoxColumn F_Maintanence_Operations;
    }
}