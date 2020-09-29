namespace DATS_Timesheets
{
    partial class ReviewTimesheetsList
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
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Tom", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Jeff", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Frank", System.Windows.Forms.HorizontalAlignment.Left);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReviewTimesheetsList));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.employeeComboBox = new System.Windows.Forms.ComboBox();
            this.lastPeriodButton = new System.Windows.Forms.Button();
            this.thisPeriodButton = new System.Windows.Forms.Button();
            this.monthCalendar1 = new System.Windows.Forms.MonthCalendar();
            this.loadingLabel = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.deselectAllButton = new System.Windows.Forms.Button();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.selectAllButton = new System.Windows.Forms.Button();
            this.nextButton = new System.Windows.Forms.Button();
            this.listView2 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.resultsList = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hoursCalendar1 = new DATS_Timesheets.HoursCalendar();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.timesheetsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.historyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.goBackToFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.approveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pendingApprovalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 36);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel1MinSize = 0;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listView2);
            this.splitContainer1.Panel2.Controls.Add(this.resultsList);
            this.splitContainer1.Panel2.Controls.Add(this.hoursCalendar1);
            this.splitContainer1.Panel2MinSize = 0;
            this.splitContainer1.Size = new System.Drawing.Size(1136, 685);
            this.splitContainer1.SplitterDistance = 376;
            this.splitContainer1.TabIndex = 64;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.employeeComboBox);
            this.groupBox1.Controls.Add(this.lastPeriodButton);
            this.groupBox1.Controls.Add(this.thisPeriodButton);
            this.groupBox1.Controls.Add(this.monthCalendar1);
            this.groupBox1.Controls.Add(this.loadingLabel);
            this.groupBox1.Controls.Add(this.listBox1);
            this.groupBox1.Controls.Add(this.deselectAllButton);
            this.groupBox1.Controls.Add(this.checkedListBox1);
            this.groupBox1.Controls.Add(this.selectAllButton);
            this.groupBox1.Controls.Add(this.nextButton);
            this.groupBox1.Font = new System.Drawing.Font("Myriad Pro", 20F);
            this.groupBox1.Location = new System.Drawing.Point(3, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(363, 660);
            this.groupBox1.TabIndex = 64;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Show me timesheets from...";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Myriad Pro", 15F);
            this.label4.Location = new System.Drawing.Point(6, 502);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 24);
            this.label4.TabIndex = 72;
            this.label4.Text = "Filters";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Myriad Pro", 15F);
            this.label3.Location = new System.Drawing.Point(6, 302);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 24);
            this.label3.TabIndex = 71;
            this.label3.Text = "Date Range";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Myriad Pro", 15F);
            this.label2.Location = new System.Drawing.Point(6, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 24);
            this.label2.TabIndex = 70;
            this.label2.Text = "Employees";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Myriad Pro", 15F);
            this.label1.Location = new System.Drawing.Point(6, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 24);
            this.label1.TabIndex = 69;
            this.label1.Text = "Department";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(126, 622);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(227, 23);
            this.progressBar1.TabIndex = 68;
            this.progressBar1.Visible = false;
            // 
            // employeeComboBox
            // 
            this.employeeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.employeeComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.employeeComboBox.FormattingEnabled = true;
            this.employeeComboBox.Location = new System.Drawing.Point(125, 41);
            this.employeeComboBox.Name = "employeeComboBox";
            this.employeeComboBox.Size = new System.Drawing.Size(228, 28);
            this.employeeComboBox.TabIndex = 67;
            this.employeeComboBox.SelectedIndexChanged += new System.EventHandler(this.employeeComboBox_SelectedIndexChanged);
            // 
            // lastPeriodButton
            // 
            this.lastPeriodButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lastPeriodButton.Location = new System.Drawing.Point(245, 467);
            this.lastPeriodButton.Name = "lastPeriodButton";
            this.lastPeriodButton.Size = new System.Drawing.Size(109, 23);
            this.lastPeriodButton.TabIndex = 66;
            this.lastPeriodButton.Text = "Last Period";
            this.lastPeriodButton.UseVisualStyleBackColor = true;
            this.lastPeriodButton.Click += new System.EventHandler(this.lastPeriodButton_Click);
            // 
            // thisPeriodButton
            // 
            this.thisPeriodButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.thisPeriodButton.Location = new System.Drawing.Point(126, 467);
            this.thisPeriodButton.Name = "thisPeriodButton";
            this.thisPeriodButton.Size = new System.Drawing.Size(113, 23);
            this.thisPeriodButton.TabIndex = 65;
            this.thisPeriodButton.Text = "This Period";
            this.thisPeriodButton.UseVisualStyleBackColor = true;
            this.thisPeriodButton.Click += new System.EventHandler(this.thisPeriodButton_Click);
            // 
            // monthCalendar1
            // 
            this.monthCalendar1.FirstDayOfWeek = System.Windows.Forms.Day.Monday;
            this.monthCalendar1.Location = new System.Drawing.Point(126, 302);
            this.monthCalendar1.MaxSelectionCount = 31;
            this.monthCalendar1.Name = "monthCalendar1";
            this.monthCalendar1.TabIndex = 63;
            // 
            // loadingLabel
            // 
            this.loadingLabel.BackColor = System.Drawing.Color.Transparent;
            this.loadingLabel.Font = new System.Drawing.Font("Myriad Pro", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loadingLabel.ForeColor = System.Drawing.Color.Red;
            this.loadingLabel.Location = new System.Drawing.Point(126, 557);
            this.loadingLabel.Name = "loadingLabel";
            this.loadingLabel.Size = new System.Drawing.Size(228, 59);
            this.loadingLabel.TabIndex = 62;
            this.loadingLabel.Text = "Loading results.\r\nPlease wait...";
            this.loadingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.loadingLabel.Visible = false;
            // 
            // listBox1
            // 
            this.listBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5"});
            this.listBox1.Location = new System.Drawing.Point(126, 75);
            this.listBox1.Name = "listBox1";
            this.listBox1.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.listBox1.Size = new System.Drawing.Size(228, 186);
            this.listBox1.TabIndex = 59;
            // 
            // deselectAllButton
            // 
            this.deselectAllButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.deselectAllButton.Location = new System.Drawing.Point(245, 265);
            this.deselectAllButton.Name = "deselectAllButton";
            this.deselectAllButton.Size = new System.Drawing.Size(109, 23);
            this.deselectAllButton.TabIndex = 61;
            this.deselectAllButton.Text = "Deselect All";
            this.deselectAllButton.UseVisualStyleBackColor = true;
            this.deselectAllButton.Click += new System.EventHandler(this.deselectAllButton_Click);
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.CheckOnClick = true;
            this.checkedListBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Items.AddRange(new object[] {
            "Hide \"Approved\"",
            "Hide \"Reviewed\"",
            "Hide \"Pending Approval\""});
            this.checkedListBox1.Location = new System.Drawing.Point(126, 502);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(228, 49);
            this.checkedListBox1.TabIndex = 43;
            // 
            // selectAllButton
            // 
            this.selectAllButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.selectAllButton.Location = new System.Drawing.Point(126, 265);
            this.selectAllButton.Name = "selectAllButton";
            this.selectAllButton.Size = new System.Drawing.Size(113, 23);
            this.selectAllButton.TabIndex = 60;
            this.selectAllButton.Text = "Select All";
            this.selectAllButton.UseVisualStyleBackColor = true;
            this.selectAllButton.Click += new System.EventHandler(this.selectAllButton_Click);
            // 
            // nextButton
            // 
            this.nextButton.Location = new System.Drawing.Point(126, 557);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(228, 59);
            this.nextButton.TabIndex = 64;
            this.nextButton.Text = "Next...";
            this.nextButton.UseVisualStyleBackColor = true;
            this.nextButton.Click += new System.EventHandler(this.loadResults);
            // 
            // listView2
            // 
            this.listView2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.listView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader12,
            this.columnHeader10,
            this.columnHeader11});
            this.listView2.Location = new System.Drawing.Point(457, 538);
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(299, 135);
            this.listView2.TabIndex = 58;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Pay Type";
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "Week 1";
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Week 2";
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "Total";
            // 
            // resultsList
            // 
            this.resultsList.Activation = System.Windows.Forms.ItemActivation.TwoClick;
            this.resultsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resultsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2,
            this.columnHeader8,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader7,
            this.columnHeader6,
            this.columnHeader9});
            this.resultsList.FullRowSelect = true;
            listViewGroup1.Header = "Tom";
            listViewGroup1.Name = "Tom";
            listViewGroup2.Header = "Jeff";
            listViewGroup2.Name = "Jeff";
            listViewGroup3.Header = "Frank";
            listViewGroup3.Name = "Frank";
            this.resultsList.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3});
            this.resultsList.HideSelection = false;
            this.resultsList.Location = new System.Drawing.Point(1, 1);
            this.resultsList.Name = "resultsList";
            this.resultsList.Size = new System.Drawing.Size(755, 538);
            this.resultsList.TabIndex = 59;
            this.resultsList.UseCompatibleStateImageBehavior = false;
            this.resultsList.View = System.Windows.Forms.View.Details;
            this.resultsList.SelectedIndexChanged += new System.EventHandler(this.resultsList_SelectedIndexChanged);
            this.resultsList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.resultsList_MouseDoubleClick);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Date";
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Time";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Pay Type";
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Hours";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Work Order";
            // 
            // columnHeader7
            // 
            this.columnHeader7.Tag = "";
            this.columnHeader7.Text = "Status";
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Equipment";
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Description";
            // 
            // hoursCalendar1
            // 
            this.hoursCalendar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hoursCalendar1.BackColor = System.Drawing.Color.Transparent;
            this.hoursCalendar1.Location = new System.Drawing.Point(1, 538);
            this.hoursCalendar1.Name = "hoursCalendar1";
            this.hoursCalendar1.Size = new System.Drawing.Size(457, 135);
            this.hoursCalendar1.TabIndex = 66;
            this.hoursCalendar1.DayClicked += new DATS_Timesheets.HoursCalendar.CalendarEventHandler(this.hoursCalendar1_DayClicked);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.White;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.timesheetsToolStripMenuItem,
            this.goBackToFilterToolStripMenuItem,
            this.approveToolStripMenuItem,
            this.reviewToolStripMenuItem,
            this.pendingApprovalToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1136, 36);
            this.menuStrip1.TabIndex = 65;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // timesheetsToolStripMenuItem
            // 
            this.timesheetsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.editToolStripMenuItem,
            this.historyToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.toolStripSeparator1,
            this.selectAllToolStripMenuItem});
            this.timesheetsToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 15F);
            this.timesheetsToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(173)))), ((int)(((byte)(250)))));
            this.timesheetsToolStripMenuItem.Name = "timesheetsToolStripMenuItem";
            this.timesheetsToolStripMenuItem.Size = new System.Drawing.Size(120, 32);
            this.timesheetsToolStripMenuItem.Text = "Timesheets";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(164, 32);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newButton_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(164, 32);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.Visible = false;
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editButton_Click);
            // 
            // historyToolStripMenuItem
            // 
            this.historyToolStripMenuItem.Name = "historyToolStripMenuItem";
            this.historyToolStripMenuItem.Size = new System.Drawing.Size(164, 32);
            this.historyToolStripMenuItem.Text = "History";
            this.historyToolStripMenuItem.Visible = false;
            this.historyToolStripMenuItem.Click += new System.EventHandler(this.historyToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(164, 32);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Visible = false;
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(161, 6);
            this.toolStripSeparator1.Visible = false;
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(164, 32);
            this.selectAllToolStripMenuItem.Text = "Select All";
            this.selectAllToolStripMenuItem.Visible = false;
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // goBackToFilterToolStripMenuItem
            // 
            this.goBackToFilterToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 15F);
            this.goBackToFilterToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(173)))), ((int)(((byte)(250)))));
            this.goBackToFilterToolStripMenuItem.Name = "goBackToFilterToolStripMenuItem";
            this.goBackToFilterToolStripMenuItem.Size = new System.Drawing.Size(168, 32);
            this.goBackToFilterToolStripMenuItem.Text = "Go Back to Filter";
            this.goBackToFilterToolStripMenuItem.Visible = false;
            this.goBackToFilterToolStripMenuItem.Click += new System.EventHandler(this.backToFilter);
            // 
            // approveToolStripMenuItem
            // 
            this.approveToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 15F);
            this.approveToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(173)))), ((int)(((byte)(250)))));
            this.approveToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("approveToolStripMenuItem.Image")));
            this.approveToolStripMenuItem.Name = "approveToolStripMenuItem";
            this.approveToolStripMenuItem.Size = new System.Drawing.Size(116, 32);
            this.approveToolStripMenuItem.Text = "Approve";
            this.approveToolStripMenuItem.Visible = false;
            this.approveToolStripMenuItem.Click += new System.EventHandler(this.approveButton_Click);
            // 
            // reviewToolStripMenuItem
            // 
            this.reviewToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 15F);
            this.reviewToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(173)))), ((int)(((byte)(250)))));
            this.reviewToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("reviewToolStripMenuItem.Image")));
            this.reviewToolStripMenuItem.Name = "reviewToolStripMenuItem";
            this.reviewToolStripMenuItem.Size = new System.Drawing.Size(100, 32);
            this.reviewToolStripMenuItem.Text = "Review";
            this.reviewToolStripMenuItem.Visible = false;
            this.reviewToolStripMenuItem.Click += new System.EventHandler(this.reviewButton_Click);
            // 
            // pendingApprovalToolStripMenuItem
            // 
            this.pendingApprovalToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 15F);
            this.pendingApprovalToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(173)))), ((int)(((byte)(250)))));
            this.pendingApprovalToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("pendingApprovalToolStripMenuItem.Image")));
            this.pendingApprovalToolStripMenuItem.Name = "pendingApprovalToolStripMenuItem";
            this.pendingApprovalToolStripMenuItem.Size = new System.Drawing.Size(197, 32);
            this.pendingApprovalToolStripMenuItem.Text = "Pending Approval";
            this.pendingApprovalToolStripMenuItem.Visible = false;
            this.pendingApprovalToolStripMenuItem.Click += new System.EventHandler(this.pendingApprovalButton_Click);
            // 
            // ReviewTimesheetsList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1136, 709);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ReviewTimesheetsList";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Review Timesheets";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ReviewTimesheets_KeyDown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ComboBox employeeComboBox;
        private System.Windows.Forms.Button lastPeriodButton;
        private System.Windows.Forms.Button thisPeriodButton;
        private System.Windows.Forms.MonthCalendar monthCalendar1;
        private System.Windows.Forms.Label loadingLabel;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button deselectAllButton;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.Button selectAllButton;
        private System.Windows.Forms.Button nextButton;
        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader12;
        private System.Windows.Forms.ListView resultsList;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private HoursCalendar hoursCalendar1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem timesheetsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem approveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reviewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pendingApprovalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem goBackToFilterToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ToolStripMenuItem historyToolStripMenuItem;



    }
}