using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DATS_Timesheets
{
    public partial class ReviewTimesheetsList : Form
    {
        bool showApproved;
        bool showReviewed;
        bool showPending;

        string username = "";
        string calendarLookingAt = "";

        string APPROVED_TEXT = "Approved";
        string REVIEWED_TEXT = "Reviewed";
        string PENDING_TEXT = "Pending Approval";
        Color APPROVED_COLOR = Color.Green;
        Color REVIEWED_COLOR = Color.Orange;
        Color PENDING_COLOR = Color.Red;

        public ReviewTimesheetsList()
        {
            InitializeComponent();

            SQL sql = new SQL("select [from] from periods where [from]<=@DATE and [to]>=@DATE");
            sql.AddParameter("@DATE", DateTime.Today);

            DateTime startingMonday = DateTime.Parse(sql.Run().Rows[0][0].ToString());
            monthCalendar1.SelectionStart = startingMonday;
            monthCalendar1.SelectionEnd = startingMonday.AddDays(13);
            monthCalendar1.Select();

            checkedListBox1.SetItemCheckState(0, CheckState.Unchecked);
            checkedListBox1.SetItemCheckState(1, CheckState.Unchecked);
            checkedListBox1.SetItemCheckState(2, CheckState.Unchecked);

            sql = new SQL("select d.department from department d, departmentassociations da, users u where d.departmentid = da.departmentid and da.userid = u.userid and u.username = @USERNAME order by d.department asc");
            sql.AddParameter("@USERNAME", Core.getUsername());
            DataTable dt = sql.Run();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                employeeComboBox.Items.Add(dt.Rows[i][0].ToString());
            }

            employeeComboBox.SelectedIndex = 0;

            checkedListBox1.ItemCheck += new ItemCheckEventHandler(checkedListBox1_ItemCheck);

            showApproved = checkedListBox1.GetItemCheckState(0) != CheckState.Checked;
            showReviewed = checkedListBox1.GetItemCheckState(1) != CheckState.Checked;
            showPending = checkedListBox1.GetItemCheckState(2) != CheckState.Checked;

            for (int i = 0; i < resultsList.Columns.Count; i++)
                resultsList.Columns[i].Name = resultsList.Columns[i].Text;

            if (!Core.showUserWorkOrders())
                resultsList.Columns.RemoveByKey("Work Order");

            if (!Core.showUserEquipment())
                resultsList.Columns.RemoveByKey("Equipment");

            if (!Core.canApprove(Core.getUsername()))
            {
                approveToolStripMenuItem.Visible = false;
            }

            hoursCalendar1.blackText();

            splitContainer1.Panel2Collapsed = true;
            Width = 400;
            CenterToScreen();
            //int r = refresh();
        }

        public void loadResults(object sender, EventArgs e2)
        {
            loadingLabel.Visible = true;
            loadingLabel.Update();
            nextButton.Visible = false;
            nextButton.Update();
            progressBar1.Visible = true;
            progressBar1.Update();

            string showString = "";
            
            if(showApproved && showReviewed && showPending)
                showString = "";
            else if(!showApproved && showReviewed && showPending)
                showString = "AND t.recordlocked = 'False'";
            else if(showApproved && !showReviewed && showPending)
                showString = "AND (t.recordlocked = 'True' OR (t.recordlocked = 'False' AND t.reviewed = 'False'))";
            else if(!showApproved && !showReviewed && showPending)
                showString = "AND t.recordlocked = 'False' AND t.reviewed = 'False'";
            else if(showApproved && showReviewed && !showPending)
                showString = "AND (t.recordlocked = 'True' OR t.reviewed = 'True')";
            else if(!showApproved && showReviewed && !showPending)
                showString = "AND t.recordlocked = 'False' AND t.reviewed = 'True'";
            else if(showApproved && !showReviewed && !showPending)
                showString = "AND t.recordlocked = 'True'";
            else if(!showApproved && !showReviewed && !showPending)
                showString = "AND t.recordlocked = 'False' AND t.recordlocked = 'True'";

            resultsList.Items.Clear();
            resultsList.Groups.Clear();

            progressBar1.Maximum = listBox1.SelectedItems.Count;
            progressBar1.Value = 0;

            for (int e = 0; e < listBox1.SelectedItems.Count; e++)
            {
                progressBar1.Value = e+1;
                progressBar1.Update();

                SQL sql = new SQL(
    @"SELECT
t.[TIMECARDDETAILID] as ID,
u.username as Username,
t.dateworked AS DateWorked,
t.starttime + ' - ' + t.finishtime as [TimeRange],
pt.[DESCRIPTION] as PayType,
t.[HOURS] as Hours,
t.workorder as WorkOrder,
'$'+convert(varchar,convert(int, t.[LUMPSUM])) as LSum,
DATENAME(MM, t.DATEENTERED) + RIGHT(CONVERT(VARCHAR(12), t.DATEENTERED, 107), 9) as DateEntered,
t.[REVIEWED] as Reviewed,
t.[RecordLocked] as Processed,
t.[DESCRIPTION] as [Description],
t.DATEWORKED as FullDateWorked

FROM [Timesheets] t

INNER JOIN Users u ON t.employeeid = u.employeeid
INNER JOIN PayCodes pt ON t.paytype = pt.paytype
LEFT OUTER JOIN Mirror_WorkOrders wo ON t.workorder = wo.workorder

WHERE
"/*emp.Username=@DEPARTMENTUSERNAME*/ + @"
u.username = @USERNAME
AND t.DATEWORKED >= @DATEWORKED
AND t.DATEWORKED <= @DATECUTOFF
" + showString + @"

ORDER BY u.username, t.DATEWORKED ASC");
//                sql.AddParameter("@DEPARTMENTUSERNAME", Core.getUsername()); //Username of current user to determine current department
                sql.AddParameter("@USERNAME", listBox1.SelectedItems[e].ToString());
                sql.AddParameter("@DATEWORKED", monthCalendar1.SelectionStart);
                sql.AddParameter("@DATECUTOFF", monthCalendar1.SelectionEnd);
                DataTable dt = sql.Run();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string ID = dt.Rows[i]["ID"].ToString();
                    string username = dt.Rows[i]["Username"].ToString();
                    string dateWorked = DateTime.Parse(dt.Rows[i]["DateWorked"].ToString()).ToString("MMM dd ddd");
                    string fullDateWorked = dt.Rows[i]["FullDateWorked"].ToString();
                    string time = dt.Rows[i]["TimeRange"].ToString();
                    string payType = dt.Rows[i]["PayType"].ToString();
                    string hours = dt.Rows[i]["Hours"].ToString();
                    string workOrder = dt.Rows[i]["WorkOrder"].ToString().Trim();

                    Oracle ora = new Oracle("select WADL01, WAVR01 from " + Core.getSchema(Core.getEnvironment()) + ".F4801 where WADOCO = :WO");
                    ora.AddParameter("WO", workOrder);
                    DataTable wodt = ora.Run();

                    if(wodt.Rows.Count > 0)
                    {
                        workOrder += " - " + wodt.Rows[0]["WADL01"].ToString().Trim();

                        if (wodt.Rows[0]["WAVR01"].ToString().Trim() != "")
                            workOrder += " (" + wodt.Rows[0]["WAVR01"].ToString().Trim() + ")";
                    }

                    string lumpSum = dt.Rows[i]["LSum"].ToString();
                    string equipment = "";

                    sql = new SQL("select equipid from equipmenttimeentry where timesheetid = @TIMESHEETID order by equipid");
                    sql.AddParameter("@TIMESHEETID", ID);
                    DataTable eqdt = sql.Run();

                    for(int j = 0; j < eqdt.Rows.Count; j++)
                    {
                        if (equipment == "")
                            equipment = eqdt.Rows[j]["equipid"].ToString().Trim();
                        else
                            equipment += ", " + eqdt.Rows[j]["equipid"].ToString().Trim();
                    }

                    string description = dt.Rows[i]["Description"].ToString();
                    bool reviewed = bool.Parse(dt.Rows[i]["Reviewed"].ToString());
                    bool recordLocked = bool.Parse(dt.Rows[i]["Processed"].ToString());
                    string status;
                    Color statusColor;



                    resultsList.Groups.Add(new ListViewGroup(username, username));

                    if (recordLocked)
                    {
                        status = APPROVED_TEXT;
                        statusColor = APPROVED_COLOR;
                    }
                    else
                    {
                        if (reviewed)
                        {
                            status = REVIEWED_TEXT;
                            statusColor = REVIEWED_COLOR;
                        }
                        else
                        {
                            status = PENDING_TEXT;
                            statusColor = PENDING_COLOR;
                        }
                    }

                    ListViewItem x;

                    if (!Core.showUserWorkOrders() && !Core.showUserEquipment())
                        x = new ListViewItem(new[] { dateWorked, time, payType, hours, status, description });
                    else if (!Core.showUserWorkOrders() && Core.showUserEquipment())
                        x = new ListViewItem(new[] { dateWorked, time, payType, hours, status, equipment, description });
                    else if (Core.showUserWorkOrders() && !Core.showUserEquipment())
                        x = new ListViewItem(new[] { dateWorked, time, payType, hours, workOrder, status, description });
                    else
                        x = new ListViewItem(new[] { dateWorked, time, payType, hours, workOrder, status, equipment, description });

                    x.Tag = ID;
                    x.SubItems[resultsList.Columns["Status"].Index].ForeColor = statusColor;
                    x.UseItemStyleForSubItems = false;
                    x.Group = resultsList.Groups[username];
                    resultsList.Items.Add(x);
                }
            }

            for (int i = 0; i < resultsList.Columns.Count; i++)
                resultsList.Columns[i].Width = -2;

            splitContainer1.Panel1Collapsed = true;
            splitContainer1.Panel2Collapsed = false;
            Width = 1152;
            CenterToScreen();

            if (Core.canApprove(Core.getUsername()))
            {
                approveToolStripMenuItem.Visible = true;
            }
            reviewToolStripMenuItem.Visible = true;
            pendingApprovalToolStripMenuItem.Visible = true;
            editToolStripMenuItem.Visible = true;
            deleteToolStripMenuItem.Visible = true;
            goBackToFilterToolStripMenuItem.Visible = true;
            selectAllToolStripMenuItem.Visible = true;
            toolStripSeparator1.Visible = true;
            historyToolStripMenuItem.Visible = true;

            loadingLabel.Visible = false;
            loadingLabel.Update();
            nextButton.Visible = true;
            nextButton.Update();
            progressBar1.Visible = false;
            progressBar1.Update();
        }

        private void approveButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("By approving these time entry records, you are also approving equipment time entry (if applicable). Do you agree?", "Are you sure?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                for (int i = 0; i < resultsList.SelectedItems.Count; i++)
                {
                    SQL sql = new SQL("update Timesheets set recordlocked='True' where timecarddetailid=@ID");
                    sql.AddParameter("@ID", resultsList.SelectedItems[i].Tag.ToString());
                    sql.Run();

                    resultsList.SelectedItems[i].SubItems[resultsList.Columns["Status"].Index].Text = APPROVED_TEXT;
                    resultsList.SelectedItems[i].SubItems[resultsList.Columns["Status"].Index].ForeColor = APPROVED_COLOR;

                    Core.logHistory("Set timesheet to Approved", "Timesheet ID# " + resultsList.SelectedItems[i].Tag.ToString(), "");
                    Core.logTimesheetHistory("Timesheet approved", int.Parse(resultsList.SelectedItems[i].Tag.ToString()));
                }

                resultsList.Columns["Status"].Width = -2;
            }
        }

        private void reviewButton_Click(object sender, EventArgs e)
        {
            bool showMessage = false;

            for (int i = 0; i < resultsList.SelectedItems.Count; i++)
            {
                if (!Core.isAdmin(Core.getUsername()) && resultsList.SelectedItems[i].SubItems[resultsList.Columns["Status"].Index].Text == APPROVED_TEXT && resultsList.SelectedItems.Count == 1)
                {
                    MessageBox.Show("You don't have permission to change the status on an approved timesheet.");
                }
                else if (!Core.isAdmin(Core.getUsername()) && resultsList.SelectedItems[i].SubItems[resultsList.Columns["Status"].Index].Text == APPROVED_TEXT && resultsList.SelectedItems.Count != 1)
                {
                    showMessage = true;
                }
                else
                {
                    SQL sql = new SQL("update Timesheets set reviewed='True', recordlocked='False' where timecarddetailid=@ID");
                    sql.AddParameter("@ID", resultsList.SelectedItems[i].Tag.ToString());
                    sql.Run();

                    resultsList.SelectedItems[i].SubItems[resultsList.Columns["Status"].Index].Text = REVIEWED_TEXT;
                    resultsList.SelectedItems[i].SubItems[resultsList.Columns["Status"].Index].ForeColor = REVIEWED_COLOR;

                    Core.logHistory("Set timesheet to Reviewed", "Timesheet ID# " + resultsList.SelectedItems[i].Tag.ToString(), "");
                    Core.logTimesheetHistory("Timesheet reviewed", int.Parse(resultsList.SelectedItems[i].Tag.ToString()));
                }
            }

            resultsList.Columns["Status"].Width = -2;

            if (showMessage)
                MessageBox.Show("Some records were not updated because you don't have permission to change approved records.");
        }

        private void pendingApprovalButton_Click(object sender, EventArgs e)
        {
            bool showMessage = false;

            for (int i = 0; i < resultsList.SelectedItems.Count; i++)
            {
                if (!Core.isAdmin(Core.getUsername()) && resultsList.SelectedItems[i].SubItems[resultsList.Columns["Status"].Index].Text == APPROVED_TEXT && resultsList.SelectedItems.Count == 1)
                {
                    MessageBox.Show("You don't have permission to change the status on an approved timesheet.");
                }
                else if (!Core.isAdmin(Core.getUsername()) && resultsList.SelectedItems[i].SubItems[resultsList.Columns["Status"].Index].Text == APPROVED_TEXT && resultsList.SelectedItems.Count != 1)
                {
                    showMessage = true;
                }
                else
                {
                    SQL sql = new SQL("update Timesheets set reviewed='False', recordlocked='False' where timecarddetailid=@ID");
                    sql.AddParameter("@ID", resultsList.SelectedItems[i].Tag.ToString());
                    sql.Run();

                    resultsList.SelectedItems[i].SubItems[resultsList.Columns["Status"].Index].Text = PENDING_TEXT;
                    resultsList.SelectedItems[i].SubItems[resultsList.Columns["Status"].Index].ForeColor = PENDING_COLOR;

                    Core.logHistory("Set timesheet to Pending Approval", "Timesheet ID# " + resultsList.SelectedItems[i].Tag.ToString(), "");
                    Core.logTimesheetHistory("Timesheet pending approval", int.Parse(resultsList.SelectedItems[i].Tag.ToString()));
                }
            }

            resultsList.Columns["Status"].Width = -2;

            if (showMessage)
                MessageBox.Show("Some records were not updated because you don't have permission to change approved records.");
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.Index == 0)
            {
                showApproved = e.NewValue != CheckState.Checked;

                //if(e.NewValue == CheckState.Unchecked)
                //{
                //    for (int i = listView1.Items.Count - 1; i >= 0; i--)
                //        if (listView1.Items[i].SubItems[listView1.Columns["Status"].Index].Text == APPROVED_TEXT)
                //            listView1.Items.RemoveAt(i);
                //}
            }
            else if (e.Index == 1)
            {
                showReviewed = e.NewValue != CheckState.Checked;

                //if (e.NewValue == CheckState.Unchecked)
                //{
                //    for (int i = listView1.Items.Count - 1; i >= 0; i--)
                //        if (listView1.Items[i].SubItems[listView1.Columns["Status"].Index].Text == REVIEWED_TEXT)
                //            listView1.Items.RemoveAt(i);
                //}
            }
            else if (e.Index == 2)
            {
                showPending = e.NewValue != CheckState.Checked;

                //if (e.NewValue == CheckState.Unchecked)
                //{
                //    for (int i = listView1.Items.Count - 1; i >= 0; i--)
                //        if (listView1.Items[i].SubItems[listView1.Columns["Status"].Index].Text == PENDING_TEXT)
                //            listView1.Items.RemoveAt(i);
                //}
            }
            //else if (e.Index == 3)
            //{
            //    showFuture = e.NewValue == CheckState.Checked;

            //    if (e.NewValue == CheckState.Unchecked)
            //    {
            //        for (int i = listView1.Items.Count - 1; i >= 0; i--)
            //            if (DateTime.Parse(listView1.Items[i].SubItems[1].Text) > DateTime.Today)
            //                listView1.Items.RemoveAt(i);
            //    }
            //}

            //if(e.NewValue == CheckState.Checked)
            //    refresh();
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            AccountsAll va = new AccountsAll(true);
            va.ShowDialog();

            if (va.selection != "")
                (new Timesheet(va.selection)).ShowDialog();
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            if (resultsList.SelectedItems.Count > 0)
            {
                bool canEdit = true;
                string errorStatus = "";

                for (int i = 0; i < resultsList.SelectedItems.Count && canEdit; i++)
                {
                    string status = resultsList.SelectedItems[i].SubItems[resultsList.Columns["Status"].Index].Text;

                    if (status == APPROVED_TEXT && !Core.canApprove(Core.getUsername()))
                    {
                        canEdit = false;
                        errorStatus = "Can't edit records that have been approved.";
                    }
                }

                if (canEdit)
                {
                    for (int i = 0; i < resultsList.SelectedItems.Count; i++)
                    {
                        ListViewItem item = resultsList.SelectedItems[i];

                        (new Timesheet(int.Parse(item.Tag.ToString()))).ShowDialog();

                        SQL sql = new SQL(
@"SELECT
t.[TIMECARDDETAILID] as ID,
u.username as Username,
t.dateworked AS DateWorked,
t.starttime + ' - ' + t.finishtime as [TimeRange],
pt.[DESCRIPTION] as PayType,
t.[HOURS] as Hours,
convert(varchar,wo.workorder) + ' - ' + wo.[Description] as WorkOrder,
'$'+convert(varchar,convert(int, t.[LUMPSUM])) as LSum,
DATENAME(MM, t.DATEENTERED) + RIGHT(CONVERT(VARCHAR(12), t.DATEENTERED, 107), 9) as DateEntered,
t.[REVIEWED] as Reviewed,
t.[RecordLocked] as Processed,
t.[DESCRIPTION] as [Description],
t.DATEWORKED as FullDateWorked

FROM [Timesheets] t

INNER JOIN Users u ON t.employeeid = u.employeeid
INNER JOIN PayCodes pt ON t.paytype = pt.paytype
LEFT OUTER JOIN Mirror_WorkOrders wo ON t.workorder = wo.workorder

WHERE
t.timecarddetailid = @ID

ORDER BY t.DATEWORKED ASC");
                        sql.AddParameter("@ID", item.Tag.ToString());
                        DataTable dt = sql.Run();

                        string ID = dt.Rows[i]["ID"].ToString();
                        string username = dt.Rows[i]["Username"].ToString();
                        string dateWorked = DateTime.Parse(dt.Rows[i]["DateWorked"].ToString()).ToString("MMM dd ddd");
                        string fullDateWorked = dt.Rows[i]["FullDateWorked"].ToString();
                        string time = dt.Rows[i]["TimeRange"].ToString();
                        string payType = dt.Rows[i]["PayType"].ToString();
                        string hours = dt.Rows[i]["Hours"].ToString();
                        string workOrder = dt.Rows[i]["WorkOrder"].ToString().Trim();
                        string lumpSum = dt.Rows[i]["LSum"].ToString();
                        string description = dt.Rows[i]["Description"].ToString();
                        bool reviewed = bool.Parse(dt.Rows[i]["Reviewed"].ToString());
                        bool recordLocked = bool.Parse(dt.Rows[i]["Processed"].ToString());
                        string status;
                        Color statusColor;

                        sql = new SQL("select equipid from equipmenttimeentry where timesheetid=@ID");
                        sql.AddParameter("@ID", ID);
                        DataTable eq = sql.Run();

                        string equipment = "";

                        for (int x = 0; x < eq.Rows.Count; x++)
                            equipment += eq.Rows[x]["equipid"].ToString() + (x + 1 < eq.Rows.Count ? ", " : "");

                        if (recordLocked)
                        {
                            status = APPROVED_TEXT;
                            statusColor = APPROVED_COLOR;
                        }
                        else
                        {
                            if (reviewed)
                            {
                                status = REVIEWED_TEXT;
                                statusColor = REVIEWED_COLOR;
                            }
                            else
                            {
                                status = PENDING_TEXT;
                                statusColor = PENDING_COLOR;
                            }
                        }

                        item.SubItems[resultsList.Columns["Date"].Index].Text = dateWorked;
                        item.SubItems[resultsList.Columns["Time"].Index].Text = time;
                        item.SubItems[resultsList.Columns["Pay Type"].Index].Text = payType;
                        item.SubItems[resultsList.Columns["Hours"].Index].Text = hours;

                        if (Core.showUserWorkOrders())
                            item.SubItems[resultsList.Columns["Work Order"].Index].Text = workOrder;

                        item.SubItems[resultsList.Columns["Status"].Index].Text = status;

                        if (Core.showUserEquipment())
                            item.SubItems[resultsList.Columns["Equipment"].Index].Text = equipment;

                        item.SubItems[resultsList.Columns["Description"].Index].Text = description;
                        item.SubItems[resultsList.Columns["Status"].Index].ForeColor = statusColor;
                        
                        for (int j = 0; j < resultsList.Columns.Count; j++)
                            resultsList.Columns[j].Width = -2;

                        boxLookingAt = "";
                        resultsList_SelectedIndexChanged(null, null);
                    }

                    //refresh();
                }
                else
                    MessageBox.Show(errorStatus);
            }
            else
            {
                MessageBox.Show("No timesheet selected.");
            }
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            if (resultsList.SelectedItems.Count > 0)
            {
                bool canRemove = true;
                string errorStatus = "";

                for (int i = 0; i < resultsList.SelectedItems.Count && canRemove; i++)
                {
                    string status = resultsList.SelectedItems[i].SubItems[resultsList.Columns["Status"].Index].Text;

                    if (status == APPROVED_TEXT && !Core.canApprove(Core.getUsername()))
                    {
                        canRemove = false;
                        errorStatus = "Can't remove records that have been approved.";
                    }
                }

                if (canRemove)
                {
                    if (MessageBox.Show("Are you sure you want to delete " + (resultsList.SelectedItems.Count > 1 ? "these " + resultsList.SelectedItems.Count + " records?" : "this record?"), "Are you sure?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {
                        for (int i = 0; i < resultsList.SelectedItems.Count; i++)
                        {
                            int id = int.Parse(resultsList.SelectedItems[i].Tag.ToString());
                            
                            Core.deleteTimesheet(id);

                            resultsList.Items.Remove(resultsList.SelectedItems[i]);
                        }
                    }
                }
                else
                    MessageBox.Show(errorStatus);
            }
            else
            {
                MessageBox.Show("No timesheet selected.");
            }
        }

        private void ReviewTimesheets_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();

            if (e.KeyCode == Keys.Delete)
                removeButton_Click(null, null);
        }

        private void resultsList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            editButton_Click(null, null);
        }

        string boxLookingAt = "";
        private void resultsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (resultsList.SelectedItems.Count == 1)
            {
                SQL sql = new SQL("select u.username, t.dateworked from Timesheets t, Users u where t.timecarddetailid = @ID and t.employeeid = u.employeeid");
                sql.AddParameter("@ID", resultsList.SelectedItems[0].Tag.ToString());
                DataTable dt = sql.Run();

                username = dt.Rows[0]["username"].ToString();

                if (username != boxLookingAt)
                {
                    boxLookingAt = username;

                    DateTime dateWorked = DateTime.Parse(dt.Rows[0]["dateworked"].ToString());

                    if (calendarLookingAt != username)
                    {
                        calendarLookingAt = username;
                        hoursCalendar1.username = username;
                    }

                    hoursCalendar1.showDate(dateWorked);

                    sql = new SQL(@"
SELECT p.PayType as pid, p.description as paytype
      
  FROM [DATS].[dbo].[Timesheets] t, users u, paycodes p
  where t.EmployeeID = u.EMPLOYEEID
  and t.PayType = p.PayType
  and u.username = @USERNAME
  and t.dateworked >= @DATEWORKED
  and t.dateworked <= @DATEEND
  
and p.paytype not in (2, 4)

  group by p.PayType, p.description
  order by p.PayType");

                    sql.AddParameter("@USERNAME", username);
                    sql.AddParameter("@DATEWORKED", hoursCalendar1.getDay(0).getDate());
                    sql.AddParameter("@DATEEND", hoursCalendar1.getDay(13).getDate());

                    DataTable payTypes = sql.Run();

                    listView2.Items.Clear();

                    double w1Hours = 0;
                    double w2Hours = 0;
                    double totalHours = 0;

                    for (int i = 0; i < payTypes.Rows.Count; i++)
                    {
                        sql = new SQL(@"select sum(hours) as Hours
                    from Timesheets t, users u
where t.employeeid = u.employeeid and u.username = @USERNAME and t.paytype = @PID and dateworked >= @DATEWORKED and dateworked <= @DATEEND");
                        sql.AddParameter("@PID", payTypes.Rows[i]["pid"].ToString());
                        sql.AddParameter("@USERNAME", username);
                        sql.AddParameter("@DATEWORKED", hoursCalendar1.getDay(0).getDate());
                        sql.AddParameter("@DATEEND", hoursCalendar1.getDay(6).getDate());

                        DataTable w1 = sql.Run();

                        sql = new SQL(@"select sum(hours) as Hours
                    from Timesheets t, users u
where t.employeeid = u.employeeid and u.username = @USERNAME and t.paytype = @PID and dateworked >= @DATEWORKED and dateworked <= @DATEEND");
                        sql.AddParameter("@PID", payTypes.Rows[i]["pid"].ToString());
                        sql.AddParameter("@USERNAME", username);
                        sql.AddParameter("@DATEWORKED", hoursCalendar1.getDay(7).getDate());
                        sql.AddParameter("@DATEEND", hoursCalendar1.getDay(13).getDate());

                        DataTable w2 = sql.Run();

                        sql = new SQL(@"select sum(hours) as Hours
                    from Timesheets t, users u
where t.employeeid = u.employeeid and u.username = @USERNAME and t.paytype = @PID and dateworked >= @DATEWORKED and dateworked <= @DATEEND");
                        sql.AddParameter("@PID", payTypes.Rows[i]["pid"].ToString());
                        sql.AddParameter("@USERNAME", username);
                        sql.AddParameter("@DATEWORKED", hoursCalendar1.getDay(0).getDate());
                        sql.AddParameter("@DATEEND", hoursCalendar1.getDay(13).getDate());

                        DataTable pp = sql.Run();

                        ListViewItem li = new ListViewItem(payTypes.Rows[i]["PayType"].ToString());
                        li.ForeColor = Core.getPayTypeColor(payTypes.Rows[i]["PayType"].ToString());
                        li.Font = new Font(li.Font, FontStyle.Bold);
                        li.UseItemStyleForSubItems = false;
                        li.Tag = payTypes.Rows[i]["pid"].ToString();
                        li.SubItems.Add(w1.Rows[0]["Hours"].ToString());
                        li.SubItems.Add(w2.Rows[0]["Hours"].ToString());
                        li.SubItems.Add(pp.Rows[0]["Hours"].ToString());

                        try
                        {
                            w1Hours += double.Parse(w1.Rows[0]["Hours"].ToString());
                        }
                        catch
                        {

                        }

                        try
                        {
                            w2Hours += double.Parse(w2.Rows[0]["Hours"].ToString());
                        }
                        catch
                        {

                        }

                        listView2.Items.Add(li);
                    }

                    totalHours = w1Hours + w2Hours;

                    ListViewItem total = new ListViewItem("Total");
                    total.ForeColor = Core.getPayTypeColor("Total");
                    total.Font = new Font(total.Font, FontStyle.Bold);
                    total.UseItemStyleForSubItems = false;
                    total.Tag = "-1";
                    total.SubItems.Add(w1Hours.ToString());
                    total.SubItems.Add(w2Hours.ToString());
                    total.SubItems.Add(totalHours.ToString());

                    listView2.Items.Add(total);

                    for (int i = 0; i < listView2.Columns.Count; i++)
                        listView2.Columns[i].Width = -2;
                }
            }
        }

        private void hoursCalendar1_DayClicked(DateTime x)
        {
            if(username != "")
                (new ViewTimesheets(x, username)).ShowDialog();
        }

        private void selectAllButton_Click(object sender, EventArgs e)
        {

            listBox1.SelectedItems.Clear();

            for (int i = 0; i < listBox1.Items.Count; i++)
                listBox1.SelectedItems.Add(listBox1.Items[i]);
        }

        private void deselectAllButton_Click(object sender, EventArgs e)
        {
            listBox1.SelectedItems.Clear();
        }

        private void thisPeriodButton_Click(object sender, EventArgs e)
        {
            SQL sql = new SQL("select [from] from periods where [from]<=@DATE and [to]>=@DATE");
            sql.AddParameter("@DATE", DateTime.Today);

            DateTime startingMonday = DateTime.Parse(sql.Run().Rows[0][0].ToString());
            monthCalendar1.SelectionStart = startingMonday;
            monthCalendar1.SelectionEnd = startingMonday.AddDays(13);
            monthCalendar1.Select();
        }

        private void lastPeriodButton_Click(object sender, EventArgs e)
        {
            SQL sql = new SQL("select [from] from periods where [from]<=@DATE and [to]>=@DATE");
            sql.AddParameter("@DATE", DateTime.Today);

            DateTime startingMonday = DateTime.Parse(sql.Run().Rows[0][0].ToString());
            monthCalendar1.SelectionStart = startingMonday.AddDays(-14);
            monthCalendar1.SelectionEnd = startingMonday.AddDays(-1);
            monthCalendar1.Select();
        }

        private void employeeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SQL sql;

            if (employeeComboBox.Text == "All")
            {
                sql = new SQL(@"select username from users order by username");
            }
            else
            {
                sql = new SQL("select u.username from users u, departmentassociations da, department d where u.userid = da.userid and da.departmentid = d.departmentid and d.department=@DEPARTMENT order by u.username");
                sql.AddParameter("@DEPARTMENT", employeeComboBox.Text);
            }
            DataTable dt = sql.Run();

            listBox1.Items.Clear();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                listBox1.Items.Add(dt.Rows[i]["username"].ToString());
            }

            selectAllButton_Click(null, null);
        }

        private void backToFilter(object sender, EventArgs e)
        {
            splitContainer1.Panel1Collapsed = false;
            splitContainer1.Panel2Collapsed = true;
            Width = 400;
            CenterToScreen();

            approveToolStripMenuItem.Visible = false;
            reviewToolStripMenuItem.Visible = false;
            pendingApprovalToolStripMenuItem.Visible = false;
            editToolStripMenuItem.Visible = false;
            deleteToolStripMenuItem.Visible = false;
            goBackToFilterToolStripMenuItem.Visible = false;
            selectAllToolStripMenuItem.Visible = false;
            toolStripSeparator1.Visible = false;
            historyToolStripMenuItem.Visible = false;
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in resultsList.Items)
            {
                item.Selected = true;
            }
            resultsList.Focus();
        }

        private void historyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (resultsList.SelectedItems.Count == 0)
                MessageBox.Show("Please select a timesheet.");
            else
            {
                for (int i = 0; i < resultsList.SelectedItems.Count; i++)
                {
                    ListViewItem item = resultsList.SelectedItems[i];

                    SQL sql = new SQL(@"
select 
DATENAME(DW, t.HistoryTimestamp) + ', ' + DATENAME(MM, t.HistoryTimestamp) + ' ' + DATENAME(DD, t.HistoryTimestamp) + ', ' + CONVERT(VARCHAR(8),t.HistoryTimestamp,108) as 'Timestamp',
PerformedBy as 'Performed By',
HistoryDescription as 'Action Description',
Version,
Period,
DATENAME(MM, t.DateWorked) + ' ' + DATENAME(DD, t.dateworked) + ' (' + DATENAME(DW, t.DateWorked) + ')' as 'Date Worked',
p.Description as 'Pay Type',
StartTime as 'Start Time',
FinishTime as 'Finish Time',
Hours,
WorkOrder as 'Work Order',
LumpSum as 'Lump Sum',
Reviewed,
RecordLocked as 'Approved',
t.Description,
Exported,
BatchId

from historyversions t
join PayCodes p on t.PayType = p.PayType
where timecarddetailid=@ID
order by version");
                    sql.AddParameter("@ID", item.Tag.ToString());
                    DataTable dt = sql.Run();

                    Report r = new Report("Timesheet History", dt);
                    r.ShowDialog();
                }
            }
        }
    }
}
