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
    public partial class ViewTimesheets : Form
    {
        DateTime dateFocusFrom, dateFocusTo;
        string username = Core.getUsername();

        string APPROVED_TEXT = "Approved";
        string REVIEWED_TEXT = "Reviewed";
        string PENDING_TEXT = "Pending Approval";
        Color APPROVED_COLOR = Color.Green;
        Color REVIEWED_COLOR = Color.Orange;
        Color PENDING_COLOR = Color.Red;

        //public ViewTimesheets()
        //{
        //    InitializeComponent();

        //    for (int i = 0; i < listView1.Columns.Count; i++)
        //        listView1.Columns[i].Name = listView1.Columns[i].Text;

        //    int r = refresh();

        //    //if (r == 0)
        //    //    button1_Click(null, null);
        //}

        public ViewTimesheets(DateTime x)
        {
            InitializeComponent();

            dateFocusTo = x;
            dateFocusFrom = x;

            if (!Core.showUserEquipment())
                listView1.Columns.RemoveAt(5);

            if (!Core.showUserWorkOrders())
                listView1.Columns.RemoveAt(3);

            for (int i = 0; i < listView1.Columns.Count; i++)
                listView1.Columns[i].Name = listView1.Columns[i].Text;

            int r = refresh();

            this.Text = x.ToString("dddd, MMMM d, yyyy");

            //if (r == 0)
            //    button1_Click(null, null);
        }

        public ViewTimesheets(DateTime x, string person)
        {
            InitializeComponent();

            dateFocusTo = x;
            dateFocusFrom = x;
            username = person;

            if (!Core.showUserEquipment())
                listView1.Columns.RemoveAt(5);

            if (!Core.showUserWorkOrders())
                listView1.Columns.RemoveAt(3);

            this.Text = x.ToString("dddd, MMMM d, yyyy") + " - " + username;

            for (int i = 0; i < listView1.Columns.Count; i++)
                listView1.Columns[i].Name = listView1.Columns[i].Text;

            int r = refresh();

            //if (r == 0)
            //    button1_Click(null, null);
        }

        public bool isNull(DateTime x)
        {
            return x.Year == 1;
        }

        public int refresh()
        {
            SQL sql = new SQL(
@"SELECT
t.[TIMECARDDETAILID] as ID,
DATENAME(DW, t.DATEWORKED) + ', ' + DATENAME(MM, t.DATEWORKED) + ' ' + DATENAME(D, t.DATEWORKED) + ', ' + DATENAME(yyyy, t.DATEWORKED) AS DateWorked,
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

WHERE " + (!isNull(dateFocusFrom) ? "t.DATEWORKED >= @DATEFROM AND " : "") + (!isNull(dateFocusTo) ? "t.DATEWORKED < @DATETO AND " : "") + @"u.Username=@USERNAME

ORDER BY t.TIMECARDDETAILID ASC");
            if (!isNull(dateFocusFrom))
                sql.AddParameter("@DATEFROM", dateFocusFrom);
            if (!isNull(dateFocusTo))
                sql.AddParameter("@DATETO", dateFocusTo.AddDays(1));
            sql.AddParameter("@USERNAME", username);
            DataTable dt = sql.Run();

            listView1.Items.Clear();
            
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string ID = dt.Rows[i]["ID"].ToString();
                string dateWorked = dt.Rows[i]["DateWorked"].ToString();
                string fullDateWorked = dt.Rows[i]["FullDateWorked"].ToString();
                string time = dt.Rows[i]["TimeRange"].ToString();
                string payType = dt.Rows[i]["PayType"].ToString();
                string hours = dt.Rows[i]["Hours"].ToString();
                string workOrder = dt.Rows[i]["WorkOrder"].ToString().Trim();

                Oracle ora = new Oracle("select WADL01, WAVR01 from " + Core.getSchema(Core.getEnvironment()) + ".F4801 where WADOCO = :WO");
                ora.AddParameter("WO", workOrder);
                DataTable wodt = ora.Run();

                if (wodt.Rows.Count > 0)
                {
                    string woLabel = wodt.Rows[0]["WADL01"].ToString().Trim();
                    string woAsset = wodt.Rows[0]["WAVR01"].ToString().Trim();

                    workOrder += " - " + woLabel;

                    if (woAsset != "")
                        workOrder += " (" + woAsset + ")";
                }

                sql = new SQL("select equipid from equipmenttimeentry where timesheetid=@ID");
                sql.AddParameter("@ID", ID);
                DataTable eq = sql.Run();

                string equipment = "";

                for (int e = 0; e < eq.Rows.Count; e++)
                    equipment += eq.Rows[e]["equipid"].ToString() + (e+1 < eq.Rows.Count ? ", " : "");

                string lumpSum = dt.Rows[i]["LSum"].ToString();
                string description = dt.Rows[i]["Description"].ToString();
                bool reviewed = bool.Parse(dt.Rows[i]["Reviewed"].ToString());
                bool recordLocked = bool.Parse(dt.Rows[i]["Processed"].ToString());
                string status;
                Color statusColor;

                if(recordLocked)
                {
                    status = "Approved";
                    statusColor = Color.Green;
                }
                else
                {
                    if(reviewed)
                    {
                        status = "Reviewed";
                        statusColor = Color.Orange;
                    }
                    else
                    {
                        status = "Pending Approval";
                        statusColor = Color.Red;
                    }
                }
                    
                ListViewItem x;

                if (Core.showUserWorkOrders())
                {
                    if (Core.showUserEquipment())
                    {
                        x = new ListViewItem(new[] { time, payType, hours, workOrder, status, equipment, description });
                        x.SubItems[4].ForeColor = statusColor;
                    }
                    else
                    {
                        x = new ListViewItem(new[] { time, payType, hours, workOrder, status, description });
                        x.SubItems[4].ForeColor = statusColor;
                    }
                }
                else
                {
                    if (Core.showUserEquipment())
                    {
                        x = new ListViewItem(new[] { time, payType, hours, status, equipment, description });
                        x.SubItems[3].ForeColor = statusColor;
                    }
                    else
                    {
                        x = new ListViewItem(new[] { time, payType, hours, status, description });
                        x.SubItems[3].ForeColor = statusColor;
                    }
                }
                
                x.Tag = ID;
                x.UseItemStyleForSubItems = false;
                listView1.Items.Add(x);
            }

            for (int i = 0; i < listView1.Columns.Count; i++)
                listView1.Columns[i].Width = -2;

            return dt.Rows.Count;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //NewTimesheetold ts;

            //if (!isNull(dateFocusFrom))
            //    ts = new NewTimesheetold(dateFocusFrom);
            //else
            //    ts = new NewTimesheetold();

            //ts.ShowDialog();

            //if(ts.changesMade)

            //if (!Core.canEnterTimesheet(dateFocusFrom) && !Core.canApprove(Core.getUsername()) && !Core.canReview(Core.getUsername()))
            //    MessageBox.Show("The window to enter time on this day is closed." + Environment.NewLine + "Please speak to your supervisor.");
            //else
            //    (new NewTimesheetold(dateFocusFrom)).ShowDialog();

            refresh();

            //Check if there are any records. If not, we don't need this screen.
            SQL sql = new SQL(@"SELECT *
                                FROM [Timesheets] t, Users u
                                WHERE " + (!isNull(dateFocusFrom) ? "t.DATEWORKED >= @DATEFROM AND " : "") + (!isNull(dateFocusTo) ? "t.DATEWORKED <= @DATETO AND " : "") + "u.employeeid = t.employeeid AND u.Username=@USERNAME");
            if (!isNull(dateFocusFrom))
                sql.AddParameter("@DATEFROM", dateFocusFrom);
            if (!isNull(dateFocusTo))
                sql.AddParameter("@DATETO", dateFocusTo);
            sql.AddParameter("@USERNAME", Core.getUsername());

            if (sql.Run().Rows.Count == 0)
                this.Close();

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0 || listView1.Items.Count == 1)
            {
                bool canRemove = true;
                string errorStatus = "";

                if (listView1.Items.Count == 1)
                    listView1.Items[0].Selected = true;

                for (int i = 0; i < listView1.SelectedItems.Count && canRemove; i++)
                {
                    string status = listView1.SelectedItems[i].SubItems[listView1.Columns["Status"].Index].Text;

                    if (status == APPROVED_TEXT && !Core.canApprove(Core.getUsername()))
                    {
                        canRemove = false;
                        errorStatus = "Can't remove records that have been approved.";
                    }
                    else if (status == REVIEWED_TEXT && !Core.canReview(Core.getUsername()))
                    {
                        canRemove = false;
                        errorStatus = "Can't remove records that have been reviewed.";
                    }
                }

                if (canRemove)
                {
                    if (MessageBox.Show("Are you sure you want to delete " + (listView1.SelectedItems.Count > 1 ? "these " + listView1.SelectedItems.Count + " records?" : "this record?"), "Are you sure?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {
                        for (int i = 0; i < listView1.SelectedItems.Count; i++)
                        {
                            Core.deleteTimesheet(int.Parse(listView1.SelectedItems[i].Tag.ToString()));
                        }

                        refresh();

                        if (listView1.Items.Count == 0)
                            Close();
                    }
                }
                else
                    MessageBox.Show(errorStatus);
            }
            else
            {
                MessageBox.Show("Remove which record?" + Environment.NewLine + Environment.NewLine + "Please select a record.");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0 || listView1.Items.Count == 1)
            {
                bool canEdit = true;
                string errorStatus = "";

                if (listView1.Items.Count == 1)
                    listView1.Items[0].Selected = true;

                for (int i = 0; i < listView1.SelectedItems.Count && canEdit; i++)
                {
                    string status = listView1.SelectedItems[i].SubItems[listView1.Columns["Status"].Index].Text;

                    if (status == APPROVED_TEXT && !Core.canApprove(Core.getUsername()))
                    {
                        canEdit = false;
                        errorStatus = "Can't edit records that have been approved.";
                    }
                    else if (status == REVIEWED_TEXT && !Core.canReview(Core.getUsername()))
                    {
                        canEdit = false;
                        errorStatus = "Can't edit records that have been reviewed.";
                    }
                }

                if (canEdit)
                {
                    for (int i = 0; i < listView1.SelectedItems.Count; i++)
                        (new Timesheet(int.Parse(listView1.SelectedItems[i].Tag.ToString()))).ShowDialog();

                    

                    refresh();
                }
                else
                    MessageBox.Show(errorStatus);
            }
            else
            {
                MessageBox.Show("Edit which record?" + Environment.NewLine + Environment.NewLine + "Please select a record.");
            }
        }

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                button5_Click(null, null);
        }

        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            
        }

        private void ViewTimesheets_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }

        //On mouse double click
        private void listView1_ItemActivate(object sender, MouseEventArgs e)
        {
            button6_Click(null, null);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Copy to Another Day coming soon.");
        }
    }
}
