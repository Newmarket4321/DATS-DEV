using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DATS_Timesheets
{
    public partial class Form1 : Form
    {
        string APPROVED_TEXT = "Approved";
        string REVIEWED_TEXT = "Reviewed";
        string PENDING_TEXT = "Unapproved";
        string EXPORTED_TEXT = "Processed";
        Color APPROVED_COLOR = Color.FromArgb(76, 217, 100);
        Color REVIEWED_COLOR = Color.FromArgb(255, 149, 0);
        Color PENDING_COLOR = Color.Red;
        Color EXPORTED_COLOR = Color.FromArgb(0, 122, 255);
        bool firstLoad = true;

        public Form1()
        {
            //Timesheet t = new Timesheet(1022334);
            //t.ShowDialog();

            if (Core.getEnvironment() != "PD")
                MessageBox.Show("Warning: You are connected to the test database");

            //Get who is logged in
            SQL sql = new SQL("select username, active from users where displayname=@USERNAME");
            sql.AddParameter("@USERNAME", Core.getUsername());
            DataTable dt = sql.Run();

            if (dt.Rows.Count == 0) //If not recognized...
            {
                MessageBox.Show("User \"" + Core.getUsername() + "\" not found. Please ask a supervisor to create you a DATS account.");
                this.Close();
            }
            else //Else, do initial setup
            {
                InitializeComponent();

                Text = "DATS - " + Core.getEnvironment();

                if (bool.Parse(dt.Rows[0]["active"].ToString()) == false)
                {
                    MessageBox.Show("Your account is inactive. Please ask a supervisor to have your account activated.");
                    this.Close();
                }
                else
                {
                    Core.logHistory("Log in", Environment.MachineName + " on ver. " + Core.getVersion().ToString("MMMM d, yyyy (h:mm:ss tt)"), "");
                    
                    for (int i = 0; i < resultsList.Columns.Count; i++)
                        resultsList.Columns[i].Name = resultsList.Columns[i].Text;

                    if (!Core.canReview(Core.getUsername()))
                    {
                        toolStripComboBox1.Enabled = false;
                        toolStripComboBox2.Enabled = false;
                        toolStripMenuItem5.Visible = false;
                        toolStripMenuItem6.Visible = false;
                        exportedToolStripMenuItem.Visible = false;
                        reviewToolStripMenuItem.Visible = false;
                        managementOptionsToolStripMenuItem.Visible = false;
                        //allTimesheetsToolStripMenuItem.Visible = false;

                        for (int i = 0; i < toolStripMenuItem2.DropDownItems.Count; i++)
                            toolStripMenuItem2.DropDownItems[i].Visible = false;

                        allTimesheetsToolStripMenuItem.Visible = true;
                        paychequesToolStripMenuItem.Visible = true;
                        paytypeSummaryToolStripMenuItem.Visible = true;
                        bankedTimeRemainingToolStripMenuItem.Visible = true;
                    }
                    if (Core.CanViewOnly(Core.getUsername()))
                    {
                        timesheetsToolStripMenuItem.Visible = false;
                        toolStripMenuItem2.Visible = false;
                        PaychequestoolStripMenuItem1.Visible = true;
                        managementOptionsToolStripMenuItem.Visible = false;
                        toolStripMenuItem1.Visible = false;
                        toolStripMenuItem4.Visible = false;
                        approveToolStripMenuItem.Visible = false;
                        reviewToolStripMenuItem.Visible = false;
                        toolStripMenuItem3.Visible = false;
                        exportedToolStripMenuItem.Visible = false;
                        hoursCalendar1.Enabled = false;
                        menuStrip2.Enabled = false;
                    }

                        if (!Core.isAdmin(Core.getUsername()))
                    {
                        payrollToolStripMenuItem.Visible = false;
                        exportedToolStripMenuItem.Visible = false;
                        toolStripMenuItem4.Visible = false;
                    }

                    if (!Core.canApprove(Core.getUsername()))
                        approveToolStripMenuItem.Visible = false;

                    //Which departments to show
                    if (Core.isAdmin(Core.getUsername()))
                    {
                        dt = SQL.Run("select department from department d order by department");
                        toolStripComboBox2.Items.Add("All");
                    }
                    else
                        dt = SQL.Run(@"
select department

from department d
join departmentassociations da on d.departmentid = da.departmentid
join users u on da.userid = u.userid

where u.displayname = @USERNAME
and d.DepartmentID in

(
select DepartmentID
from DepartmentAssociations da
join Users u on da.UserID = u.USERID
where u.active = 1
and u.ENTERSTIME = 1
group by DepartmentID
)

order by department", Core.getUsername());
                    

                    for (int i = 0; i < dt.Rows.Count; i++)
                        toolStripComboBox2.Items.Add(dt.Rows[i]["department"].ToString());
                    
                    if (toolStripComboBox2.Items.Count > 0)
                        toolStripComboBox2.SelectedIndex = 0;

                    toolStripComboBox2.SelectedIndexChanged += comboBox2_SelectedIndexChanged;

                    loadUsers();

                    //loadPeriods();
                    //updateCalendars();
                    
                    toolStripComboBox3.SelectedIndexChanged += comboBox3_SelectedIndexChanged;

                    refresh();
                    firstLoad = false;
                }
            }
        }

        public string getEmpType()
        {
            return Core.isHourly(Core.getEmpID(getUsername())) ? "H" : "S";
        }

        private void loadPeriods()
        {
            toolStripComboBox3.SelectedIndexChanged -= comboBox3_SelectedIndexChanged;

            string type = getEmpType();

            string currentPeriod = Core.getCurrentPeriod(type).ToString();
            
            DataTable dt = Oracle.Run("select JDDTEY, JDPPNB from " + Core.getSchema(Core.getEnvironment()) + ".F069066 where JDPCCD='" + (type == "H" ? "HR" : "SAL") + "'");

            toolStripComboBox3.Items.Clear();

            string empType = getEmpType();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string period = "20" + (dt.Rows[i]["JDDTEY"].ToString().Length == 2 ? dt.Rows[i]["JDDTEY"].ToString() : ("0" + dt.Rows[i]["JDDTEY"].ToString())) + dt.Rows[i]["JDPPNB"].ToString().Substring(1, 2);

                DateTime from = Core.getPeriodStart(int.Parse(period), empType);
                DateTime to = Core.getPeriodEnd(int.Parse(period), empType);

                toolStripComboBox3.Items.Add(period + type + " - " + from.ToString(@"MMM d") + " to " + to.ToString(@"MMM d") + (currentPeriod == period ? " (Current)" : ""));

                if (currentPeriod == period)
                    toolStripComboBox3.SelectedIndex = toolStripComboBox3.Items.Count - 1;
            }

            updateCalendars();

            toolStripComboBox3.SelectedIndexChanged += comboBox3_SelectedIndexChanged;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(!checkForPrePeriod())
                refresh();
        }

        private bool checkForPrePeriod()
        {
            DataTable cur = Oracle.Run("select YDCTRY, YDDTEY, YDPPNB from " + Core.getSchema(Core.getEnvironment()) + ".F07210 where YDPAYD='TOWNHOURLY'");
            int period = int.Parse(cur.Rows[0]["YDCTRY"].ToString() + cur.Rows[0]["YDDTEY"].ToString() + cur.Rows[0]["YDPPNB"].ToString().Substring(1, 2));

            //Returns false when no pre-period found, or when user decides to ignore pre-period
            SQL sql = new SQL(@"SELECT t.timecarddetailid, t.dateworked, DATENAME(MM, t.DateWorked) + ' ' + DATENAME(DD, t.dateworked) + ', ' + DATENAME(YYYY, t.dateworked) + ' (' + DATENAME(DW, t.DateWorked) + ')' as 'Date Worked'
  FROM Timesheets t
  where employeeid=@EMPID
  and Period<@PERIOD
  and exported=0
  and recordlocked=0
  and paytype<>0
order by dateworked asc");
            sql.AddParameter("@PERIOD", period);
            sql.AddParameter("@EMPID", Core.getEmpID(getUsername()));
            DataTable dt = sql.Run();

            if(dt.Rows.Count > 0 && Core.canReview(Core.getUsername()))
            {
                string message = getUsername() + " has unapproved pre-period on the following dates:" + Environment.NewLine + Environment.NewLine;

                for(int i = 0; i < dt.Rows.Count && i < 15; i++)
                {
                    message += dt.Rows[i]["Date Worked"].ToString() + Environment.NewLine;
                }

                Core.lookupMessageBox("Pre-period Reminder", message, "OK");
            }

            return false;
        }

        private int getPeriod()
        {
            return int.Parse(toolStripComboBox3.Text.Substring(0, 6));
        }

        private DateTime getStart()
        {
            return Core.getPeriodStart(getPeriod(), getEmpType());
        }

        private DateTime getEnd()
        {
            return Core.getPeriodEnd(getPeriod(), getEmpType());
        }

        private void refresh()
        {
            DateTime xxx ;
            DateTime yyy ;

            if ((getEmpType() == "S" && toolStripComboBox3.Items.Count > 0 && toolStripComboBox3.Text[6] == 'H') ||
                (getEmpType() == "H" && toolStripComboBox3.Items.Count > 0 && toolStripComboBox3.Text[6] == 'S') ||
                firstLoad)
                loadPeriods();

            hoursCalendar1.username = getUsername();
            hoursCalendar1.empType = getEmpType();
            hoursCalendar1.showDate(getStart());

            SQL sql = new SQL(
    @"SELECT
datediff(day, @DATEWORKED, dateworked) as 'DaysIntoPeriod',
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
t.[RecordLocked] as Approved,
t.[Exported] as Exported,
t.[DESCRIPTION] as [Description],
t.DATEWORKED as FullDateWorked

FROM [Timesheets] t

INNER JOIN Users u ON t.employeeid = u.employeeid
INNER JOIN PayCodes pt ON t.paytype = pt.paytype
LEFT OUTER JOIN Mirror_WorkOrders wo ON t.workorder = wo.workorder

WHERE
u.displayname = @USERNAME
AND t.DATEWORKED >= @DATEWORKED
AND t.DATEWORKED <= @DATECUTOFF

ORDER BY t.DATEWORKED ASC");

            
//INNER JOIN Roles r ON u.roleid = r.roleid
//INNER JOIN Department d ON r.departmentid = d.departmentid

            //                sql.AddParameter("@DEPARTMENTUSERNAME", Core.getUsername()); //Username of current user to determine current department
            // Selects TimeSheets data for the calendar's range -Soleil

            sql.AddParameter("@USERNAME", getUsername());
            sql.AddParameter("@DATEWORKED", getStart());
            // sql.AddParameter("@DATECUTOFF", getEnd());

            //soleil 
            xxx = getEnd();
            yyy = DateTime.Parse(xxx.ToString("yyyy/MM/dd")+" 23:59:00 PM");
            
            sql.AddParameter("@DATECUTOFF", yyy);

            DataTable dt = sql.Run();
            //*
            //Report r = new Report(Text, dt);
            //r.Show();
            //*
            double totalHours = 0;

            resultsList.Items.Clear();

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
                int daysIntoPeriod = int.Parse(dt.Rows[i]["DaysIntoPeriod"].ToString());

                totalHours += double.Parse(hours);

                Oracle ora = new Oracle("select WADL01, WAVR01 from " + Core.getSchema(Core.getEnvironment()) + ".F4801 where WADOCO = :WO");
                ora.AddParameter("WO", workOrder);
                DataTable wodt = ora.Run();

                if (wodt.Rows.Count > 0)
                {
                    workOrder += " - " + wodt.Rows[0]["WADL01"].ToString().Trim();

                    if (wodt.Rows[0]["WAVR01"].ToString().Trim() != "")
                        workOrder += " (" + wodt.Rows[0]["WAVR01"].ToString().Trim() + ")";
                }

                string lumpSum = (dt.Rows[i]["LSum"].ToString() == "$0" ? "" : dt.Rows[i]["LSum"].ToString());
                string equipment = "";

                sql = new SQL("select equipid from equipmenttimeentry where timesheetid = @TIMESHEETID order by equipid");
                sql.AddParameter("@TIMESHEETID", ID);
                DataTable eqdt = sql.Run();

                for (int j = 0; j < eqdt.Rows.Count; j++)
                {
                    if (equipment == "")
                        equipment = eqdt.Rows[j]["equipid"].ToString().Trim();
                    else
                        equipment += ", " + eqdt.Rows[j]["equipid"].ToString().Trim();
                }

                string description = dt.Rows[i]["Description"].ToString();
                bool reviewed = bool.Parse(dt.Rows[i]["Reviewed"].ToString());
                bool recordLocked = bool.Parse(dt.Rows[i]["Approved"].ToString());
                bool exported = bool.Parse(dt.Rows[i]["Exported"].ToString());
                string status;
                Color statusColor;

                resultsList.Groups.Add(new ListViewGroup("Week " + ((daysIntoPeriod / 7)+1), "Week " + ((daysIntoPeriod / 7)+1)));

                if (exported)
                {
                    status = EXPORTED_TEXT;
                    statusColor = EXPORTED_COLOR;
                }
                else
                {
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
                }
                

                ListViewItem x;

                x = new ListViewItem(new[] { dateWorked, time, payType, hours, workOrder, status, equipment, lumpSum, description });

                x.Tag = ID;
                //x.SubItems[resultsList.Columns["Pay Type"].Index].ForeColor = Core.getPayTypeColor(payType);
                x.SubItems[resultsList.Columns["Status"].Index].ForeColor = statusColor;
                x.UseItemStyleForSubItems = false;
                x.Group = resultsList.Groups["Week " + ((daysIntoPeriod / 7)+1)];
                resultsList.Items.Add(x);
            }

            for (int i = 0; i < resultsList.Columns.Count; i++)
                resultsList.Columns[i].Width = -2;

            loadPayTypes();
            loadEntitlements();

            resultsList.Focus();

            //MessageBox.Show(output);
        }

        private void loadEntitlements()
        {
            int year = DateTime.Today.Year;
            DateTime start = new DateTime(year, 1, 1);
            DateTime end = start.AddYears(1);
            int empID = Core.getEmpID(getUsername());

            double bankedTimeIn = Core.getBankedTimeIn(start, end, empID);
            double bankedTimeUsed = Core.getBankedTimeUsed(start, end, empID);
            double bankedTimeBalance = Core.getBankedTimeBalance(start, end, empID);

            double vacationPriorMax = Core.getVacationMax(year - 1, empID);
            double vacationPriorUsed = Core.getVacationUsed(year - 1, empID);
            double vacationPriorBalance = Core.getVacationBalance(year - 1, empID, true);

            double vacationMax = Core.getVacationMax(year, empID);
            double vacationUsed = Core.getVacationUsed(year, empID);
            double vacationBalance = Core.getVacationBalance(year, empID, false);

            double bankedVacationMax = Core.getBankedVacationMax(empID);
            double bankedVacationUsed = Core.getBankedVacationUsed(empID);
            double bankedVacationBalance = Core.getBankedVacationBalance(empID);

            double floaterMax = Core.getFloaterMax(empID);
            double floaterUsed = Core.getFloaterUsed(start, end, empID);
            double floaterBalance = Core.getFloaterBalance(start, end, empID);

            double statMax = Core.getStatMax(start, end, empID);
            double statUsed = Core.getStatUsed(start, end, empID);
            double statBalance = Core.getStatBalance(start, end, empID);

            // MCL Bank
            double MCLMax = Core.getMCLMax(year, empID);
            double MCLUsed = Core.getMCLUsed(year, empID);
            double MCLBalance = Core.getMCLBalance(year, empID, false);

            //Report
            DataTable dt = new DataTable();
            dt.Columns.Add("Category");
            dt.Columns.Add("Entitlement");
            dt.Columns.Add("Used");
            dt.Columns.Add("Balance");

            dt.Rows.Add(new object[] { "Banked Overtime", bankedTimeIn, bankedTimeUsed, bankedTimeBalance });

//          if (DateTime.Today.Month < 4) soleil April
            dt.Rows.Add(new object[] { (start.Year - 1) + " Vacation", vacationPriorMax, vacationPriorUsed, vacationPriorBalance });

            dt.Rows.Add(new object[] { start.Year + " Vacation", vacationMax, vacationUsed, vacationBalance });

            if ((!Core.isFacilities(getUsername()) && !Core.isFacilityMaintenance(getUsername())) || Core.getBankedVacationBalance(empID) > 0)
                dt.Rows.Add(new object[] { "Banked Vacation", "", "", bankedVacationBalance });

            if (MCLMax > 0)
                dt.Rows.Add(new object[] { "MCL Vacation", MCLMax, MCLUsed, MCLBalance });

            dt.Rows.Add(new object[] { "Floater", floaterMax, floaterUsed, floaterBalance });

            if (Core.isFacilities(getUsername()) || Core.isFacilityMaintenance(getUsername()))
                dt.Rows.Add(new object[] { "Statutory Holiday", statMax, statUsed, statBalance });

            

            listView1.Items.Clear();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string[] items = { dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString(), dt.Rows[i][2].ToString(), dt.Rows[i][3].ToString() };
                listView1.Items.Add(new ListViewItem(items));
            }

            for (int i = 0; i < listView1.Columns.Count; i++)
                listView1.Columns[i].Width = -2;
        }

        private void loadPayTypes()
        {
            DateTime start = DateTime.Now;
            TimeSpan ts = DateTime.Now - start;
            string output = "";

            ts = DateTime.Now - start;
            output += "Step 1 " + ts.TotalSeconds + Environment.NewLine;

            SQL sql = new SQL(@"
SELECT p.PayType as pid, p.description as paytype
      
  FROM [Timesheets] t, users u, paycodes p
  where t.EmployeeID = u.EMPLOYEEID
  and t.PayType = p.PayType
  and u.displayname = @USERNAME
  and t.dateworked >= @DATEWORKED
  and t.dateworked <= @DATEEND
  
and p.paytype not in (2, 4)

  group by p.PayType, p.description
  order by p.PayType");

            sql.AddParameter("@USERNAME", getUsername());
            sql.AddParameter("@DATEWORKED", getStart());
            sql.AddParameter("@DATEEND", getEnd());

            DataTable payTypes = sql.Run();

            ts = DateTime.Now - start;
            output += "Step 2 " + ts.TotalSeconds + Environment.NewLine;

            listView2.Items.Clear();

            double w1Hours = 0;
            double w2Hours = 0;
            double totalHours = 0;
            bool periodView = (getEnd() - getStart()).TotalDays == 13; //14, inclusive
            
            for (int i = 0; i < payTypes.Rows.Count; i++)
            {
                sql = new SQL(@"select sum(hours) as Hours
                    from Timesheets t, users u
where t.employeeid = u.employeeid and u.displayname = @USERNAME and t.paytype = @PID and dateworked >= @DATEWORKED and dateworked <= @DATEEND");
                sql.AddParameter("@PID", payTypes.Rows[i]["pid"].ToString());
                sql.AddParameter("@USERNAME", getUsername());
                sql.AddParameter("@DATEWORKED", getStart());
                sql.AddParameter("@DATEEND", getEnd().AddDays(-7));

                if(payTypes.Rows[i]["pid"].ToString() == "80")
                {
                    int x = 5;
                }

                DataTable w1 = sql.Run();

                sql = new SQL(@"select sum(hours) as Hours
                    from Timesheets t, users u
where t.employeeid = u.employeeid and u.displayname = @USERNAME and t.paytype = @PID and dateworked >= @DATEWORKED and dateworked <= @DATEEND");
                sql.AddParameter("@PID", payTypes.Rows[i]["pid"].ToString());
                sql.AddParameter("@USERNAME", getUsername());
                sql.AddParameter("@DATEWORKED", getStart().AddDays(7));
                sql.AddParameter("@DATEEND", getEnd());

                DataTable w2 = sql.Run();

                sql = new SQL(@"select sum(hours) as Hours
                    from Timesheets t, users u
where t.employeeid = u.employeeid and u.displayname = @USERNAME and t.paytype = @PID and dateworked >= @DATEWORKED and dateworked <= @DATEEND");
                sql.AddParameter("@PID", payTypes.Rows[i]["pid"].ToString());
                sql.AddParameter("@USERNAME", getUsername());
                sql.AddParameter("@DATEWORKED", getStart());
                sql.AddParameter("@DATEEND", getEnd());

                DataTable pp = sql.Run();

                string pt = payTypes.Rows[i]["PayType"].ToString();

                if (w1.Rows[0]["Hours"].ToString() != "")
                    w1Hours += double.Parse(w1.Rows[0]["Hours"].ToString());

                if (w2.Rows[0]["Hours"].ToString() != "")
                    w2Hours += double.Parse(w2.Rows[0]["Hours"].ToString());

                if (pp.Rows[0]["Hours"].ToString() != "")
                    totalHours += double.Parse(pp.Rows[0]["Hours"].ToString());

                ListViewItem li = new ListViewItem(pt);
                //li.ForeColor = Core.getPayTypeColor(payTypes.Rows[i]["PayType"].ToString());
                //li.Font = new Font(li.Font, FontStyle.Bold);
                li.UseItemStyleForSubItems = false;
                li.Tag = payTypes.Rows[i]["pid"].ToString();

                if (periodView)
                {
                    li.SubItems.Add(w1.Rows[0]["Hours"].ToString());
                    li.SubItems.Add(w2.Rows[0]["Hours"].ToString());
                }
                li.SubItems.Add(pp.Rows[0]["Hours"].ToString());

                listView2.Items.Add(li);
            }

            ts = DateTime.Now - start;
            output += "Step 3 " + ts.TotalSeconds + Environment.NewLine;

            //totalHours = w1Hours + w2Hours;

            ListViewItem total = new ListViewItem("Total");
            total.ForeColor = Color.FromArgb(50, 50, 50);
            //total.Font = new Font(total.Font, FontStyle.Bold);
            total.UseItemStyleForSubItems = false;
            total.Tag = "-1";

            if (periodView)
            {
                total.SubItems.Add(w1Hours.ToString());
                total.SubItems.Add(w2Hours.ToString());

                var temp1 = Core.canReview(Core.getUsername());
                var temp2 = Core.isFullTime(getUsername());
                //if (Core.canReview(Core.getUsername()) && Core.isFullTime(getUsername()) && ((decimal)w1Hours < Core.getDepartmentDailyHours() * 5 || (decimal)w2Hours < Core.getDepartmentDailyHours() * 5))
                if (Core.canReview(Core.getUsername()) && Core.isFullTime(getUsername()) && ((decimal)totalHours < Core.getDepartmentDailyHours() * 10))
                    label1.Visible = true;
                else
                    label1.Visible = false;
            }
            total.SubItems.Add(totalHours.ToString());

            listView2.Items.Insert(0, total);

            ts = DateTime.Now - start;
            output += "Step 4 " + ts.TotalSeconds + Environment.NewLine;

            for (int i = 0; i < listView2.Columns.Count; i++)
                listView2.Columns[i].Width = -2;

            ts = DateTime.Now - start;
            output += "Step 5 " + ts.TotalSeconds + Environment.NewLine;

            //MessageBox.Show(output);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (toolStripComboBox1.SelectedIndex > 0)
                toolStripComboBox1.SelectedIndex--;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (toolStripComboBox1.SelectedIndex < toolStripComboBox1.Items.Count - 1)
                toolStripComboBox1.SelectedIndex++;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if(Core.getLockout())
            //{
            //    MessageBox.Show(Core.getLockoutText());
            //    return;
            //}

            (new Timesheet(getUsername())).ShowDialog();
            refresh();
        }

        private string getUsername()
        {
            return toolStripComboBox1.Text.Split('(')[0].Trim();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (!ignoreFromCal)
                refresh();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            if (!ignoreToCal)
                refresh();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (Core.getLockout())
            //{
            //    MessageBox.Show(Core.getLockoutText());
            //    return;
            //}

            edit();
        }

        private void edit()
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

                    if (status == "Processed" && !Core.isAdmin(Core.getUsername()))
                    {
                        canEdit = false;
                        errorStatus = "Can't edit records that have been processed.";
                    }

                    if (Core.isTimesheetLocked(int.Parse(resultsList.SelectedItems[i].Tag.ToString()), getEmpType()))
                    {
                        canEdit = false;
                        errorStatus = Core.getLockoutText();
                    }
                }

                if (canEdit)
                {
                    for (int i = 0; i < resultsList.SelectedItems.Count; i++)
                    {
                        ListViewItem item = resultsList.SelectedItems[i];

                        (new Timesheet(int.Parse(item.Tag.ToString()))).ShowDialog();
                        hoursCalendar1.username = getUsername();
                        hoursCalendar1.showDate(getStart());
                    }

                    refresh();
                }
                else
                    MessageBox.Show(errorStatus);
            }
            else
            {
                MessageBox.Show("No timesheet selected.");
            }
        }

        private void resultsList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            edit();
        }

        private void viewHistoryToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadUsers();
        }

        private void loadUsers()
        {
            SQL sql;

            if(!Core.canReview(Core.getUsername()))
            {
                sql = new SQL("select * from users u where u.displayname=@U");
                sql.AddParameter("@U", Core.getUsername());
            }
            else if (toolStripComboBox2.Text != "All")
            {
                sql = new SQL("select * from users u join departmentassociations da on u.userid = da.userid join department d on da.departmentid = d.departmentid where d.department=@DEPT and active=1 and enterstime=1 order by displayname");
                sql.AddParameter("@DEPT", toolStripComboBox2.Text);
            }
            else
            {
                sql = new SQL("select * from users where active=1 and enterstime=1 order by displayname");
            }
            DataTable dt = sql.Run();

            toolStripComboBox1.Items.Clear();

            toolStripComboBox1.SelectedIndexChanged -= comboBox1_SelectedIndexChanged;

            for (int i = 0; i < dt.Rows.Count; i++)
                toolStripComboBox1.Items.Add(dt.Rows[i]["displayname"].ToString() + " (" + dt.Rows[i]["employeeid"].ToString() + ")");

            if(firstLoad)
                if (toolStripComboBox1.Items.Count > 0)
                    toolStripComboBox1.SelectedIndex = 0;

            toolStripComboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;

            if(!firstLoad)
                if (toolStripComboBox1.Items.Count > 0)
                    toolStripComboBox1.SelectedIndex = 0;
        }

        bool ignoreFromCal = false;
        private void dateTimePicker1_DropDown(object sender, EventArgs e)
        {
            ignoreFromCal = true;
        }

        private void dateTimePicker1_CloseUp(object sender, EventArgs e)
        {
            ignoreFromCal = false;
            dateTimePicker1_ValueChanged(null, null);
        }

        bool ignoreToCal = false;
        private void dateTimePicker2_DropDown(object sender, EventArgs e)
        {
            ignoreToCal = true;
        }

        private void dateTimePicker2_CloseUp(object sender, EventArgs e)
        {
            ignoreToCal = false;
            dateTimePicker2_ValueChanged(null, null);
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in resultsList.Items)
            {
                item.Selected = true;
            }
            resultsList.Focus();
        }

        private void exportToExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
u.displayname = @USERNAME
AND t.DATEWORKED >= @DATEWORKED
AND t.DATEWORKED <= @DATECUTOFF

ORDER BY u.username, t.DATEWORKED ASC");
            //                sql.AddParameter("@DEPARTMENTUSERNAME", Core.getUsername()); //Username of current user to determine current department
            sql.AddParameter("@USERNAME", getUsername());
            sql.AddParameter("@DATEWORKED", getStart());
            sql.AddParameter("@DATECUTOFF", getEnd());
            DataTable dt = sql.Run();

            Core.export("Timesheet Report", dt);
        }

        private void approveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool showMessage = false;
            bool showMessage2 = false;

            SQL sqluser = new SQL("select username, active from users where displayname=@USERNAME");
            sqluser.AddParameter("@USERNAME", getUsername());
            DataTable dt = sqluser.Run();

            var username = Environment.UserName;
            if (dt.Rows[0][0].ToString() == username)
            {
                MessageBox.Show("You can not approve your own requests.");
            }
            else
            {
                for (int i = 0; i < resultsList.SelectedItems.Count; i++)
                {
                    string currentStatus = resultsList.SelectedItems[i].SubItems[resultsList.Columns["Status"].Index].Text;

                    //Locked
                    if (Core.isTimesheetLocked(int.Parse(resultsList.SelectedItems[i].Tag.ToString()), getEmpType()) && resultsList.SelectedItems.Count == 1)
                    {
                        MessageBox.Show(Core.getLockoutText());
                    }
                    else if (Core.isTimesheetLocked(int.Parse(resultsList.SelectedItems[i].Tag.ToString()), getEmpType()) && resultsList.SelectedItems.Count == 1)
                    {
                        showMessage = true;
                    }
                    //Exported
                    else if (currentStatus == EXPORTED_TEXT && resultsList.SelectedItems.Count == 1 && !Core.isAdmin(Core.getUsername()))
                    {
                        MessageBox.Show("This timesheet has already been processed by Payroll." + Environment.NewLine +
                            "Please contact Payroll if changes are needed.");
                    }
                    else if (currentStatus == EXPORTED_TEXT && resultsList.SelectedItems.Count != 1 && !Core.isAdmin(Core.getUsername()))
                    {
                        showMessage2 = true;
                    }
                    else
                    {
                        SQL sql = new SQL("update Timesheets set recordlocked='True' where timecarddetailid=@ID");
                        sql.AddParameter("@ID", resultsList.SelectedItems[i].Tag.ToString());
                        sql.Run();

                        resultsList.SelectedItems[i].SubItems[resultsList.Columns["Status"].Index].Text = APPROVED_TEXT;
                        resultsList.SelectedItems[i].SubItems[resultsList.Columns["Status"].Index].ForeColor = APPROVED_COLOR;

                        Core.logTimesheetHistory("Timesheet approved", int.Parse(resultsList.SelectedItems[i].Tag.ToString()));
                        Core.logHistory("Set timesheet to Approved", "Timesheet ID# " + resultsList.SelectedItems[i].Tag.ToString(), "");
                    }
                }

                resultsList.Columns["Status"].Width = -2;

                if (showMessage)
                    MessageBox.Show(Core.getLockoutText());

                if (showMessage2)
                    MessageBox.Show("Some of the selected timesheets have already been processed by Payroll." + Environment.NewLine +
                            "Please contact Payroll if changes are needed.");
            }           
        }

        private void reviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool showMessage = false;
            bool showMessage2 = false;
            bool showMessage3 = false;

            for (int i = 0; i < resultsList.SelectedItems.Count; i++)
            {
                string currentStatus = resultsList.SelectedItems[i].SubItems[resultsList.Columns["Status"].Index].Text;

                //Locked
                if (Core.isTimesheetLocked(int.Parse(resultsList.SelectedItems[i].Tag.ToString()), getEmpType()) && resultsList.SelectedItems.Count == 1)
                {
                    MessageBox.Show(Core.getLockoutText());
                }
                else if (Core.isTimesheetLocked(int.Parse(resultsList.SelectedItems[i].Tag.ToString()), getEmpType()) && resultsList.SelectedItems.Count == 1)
                {
                    showMessage3 = true;
                }
                //Exported
                else if (currentStatus == EXPORTED_TEXT && resultsList.SelectedItems.Count == 1 && !Core.isAdmin(Core.getUsername()))
                {
                    MessageBox.Show("This timesheet has already been processed by Payroll." + Environment.NewLine +
                        "Please contact Payroll if changes are needed.");
                }
                else if (currentStatus == EXPORTED_TEXT && resultsList.SelectedItems.Count != 1 && !Core.isAdmin(Core.getUsername()))
                {
                    showMessage2 = true;
                }
                //Approved, and not allowed to unapprove
                else if (currentStatus == APPROVED_TEXT && !Core.canApprove(Core.getUsername()) && resultsList.SelectedItems.Count == 1)
                {
                    MessageBox.Show("You don't have permission to change the status on an approved timesheet.");
                }
                else if (currentStatus == APPROVED_TEXT && !Core.canApprove(Core.getUsername()) && resultsList.SelectedItems.Count != 1)
                {
                    showMessage = true;
                }
                //Do it
                else
                {
                    SQL sql = new SQL("update Timesheets set reviewed='True', recordlocked='False' where timecarddetailid=@ID");
                    sql.AddParameter("@ID", resultsList.SelectedItems[i].Tag.ToString());
                    sql.Run();

                    resultsList.SelectedItems[i].SubItems[resultsList.Columns["Status"].Index].Text = REVIEWED_TEXT;
                    resultsList.SelectedItems[i].SubItems[resultsList.Columns["Status"].Index].ForeColor = REVIEWED_COLOR;

                    Core.logTimesheetHistory("Timesheet reviewed", int.Parse(resultsList.SelectedItems[i].Tag.ToString()));
                    Core.logHistory("Set timesheet to Reviewed", "Timesheet ID# " + resultsList.SelectedItems[i].Tag.ToString(), "");
                }
            }

            resultsList.Columns["Status"].Width = -2;

            if (showMessage)
                MessageBox.Show("Some records were not updated because you don't have permission to change approved records.");

            if (showMessage2)
                MessageBox.Show("Some of the selected timesheets have already been processed by Payroll." + Environment.NewLine +
                        "Please contact Payroll if changes are needed.");

            if (showMessage3)
                MessageBox.Show(Core.getLockoutText());
        }

        private void pendingApprovalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool showMessage = false;
            bool showMessage2 = false;
            bool showMessage3 = false;

            SQL sqluser = new SQL("select username, active from users where displayname=@USERNAME");
            sqluser.AddParameter("@USERNAME", getUsername());
            DataTable dt = sqluser.Run();

            var username = Environment.UserName;
            if (dt.Rows[0][0].ToString() == username)
            {
                MessageBox.Show("You can not unapprove your own requests.");
            }
            else
            {
                for (int i = 0; i < resultsList.SelectedItems.Count; i++)
                {
                    string currentStatus = resultsList.SelectedItems[i].SubItems[resultsList.Columns["Status"].Index].Text;

                    //Locked
                    if (Core.isTimesheetLocked(int.Parse(resultsList.SelectedItems[i].Tag.ToString()), getEmpType()) && resultsList.SelectedItems.Count == 1)
                    {
                        MessageBox.Show(Core.getLockoutText());
                    }
                    else if (Core.isTimesheetLocked(int.Parse(resultsList.SelectedItems[i].Tag.ToString()), getEmpType()) && resultsList.SelectedItems.Count == 1)
                    {
                        showMessage3 = true;
                    }
                    //Exported
                    else if (currentStatus == EXPORTED_TEXT && resultsList.SelectedItems.Count == 1 && !Core.isAdmin(Core.getUsername()))
                    {
                        MessageBox.Show("This timesheet has already been processed by Payroll." + Environment.NewLine +
                            "Please contact Payroll if changes are needed.");
                    }
                    else if (currentStatus == EXPORTED_TEXT && resultsList.SelectedItems.Count != 1 && !Core.isAdmin(Core.getUsername()))
                    {
                        showMessage2 = true;
                    }
                    //Approved, and not allowed to unapprove
                    else if (currentStatus == APPROVED_TEXT && !Core.canApprove(Core.getUsername()) && resultsList.SelectedItems.Count == 1)
                    {
                        MessageBox.Show("You don't have permission to change the status on an approved timesheet.");
                    }
                    else if (currentStatus == APPROVED_TEXT && !Core.canApprove(Core.getUsername()) && resultsList.SelectedItems.Count != 1)
                    {
                        showMessage = true;
                    }
                    //Do it
                    else
                    {
                        SQL sql = new SQL("update Timesheets set reviewed='False', recordlocked='False' where timecarddetailid=@ID");
                        sql.AddParameter("@ID", resultsList.SelectedItems[i].Tag.ToString());
                        sql.Run();

                        resultsList.SelectedItems[i].SubItems[resultsList.Columns["Status"].Index].Text = PENDING_TEXT;
                        resultsList.SelectedItems[i].SubItems[resultsList.Columns["Status"].Index].ForeColor = PENDING_COLOR;

                        Core.logTimesheetHistory("Timesheet unapproved", int.Parse(resultsList.SelectedItems[i].Tag.ToString()));
                        Core.logHistory("Set timesheet to Unapproved", "Timesheet ID# " + resultsList.SelectedItems[i].Tag.ToString(), "");
                    }
                }

                resultsList.Columns["Status"].Width = -2;

                if (showMessage)
                    MessageBox.Show("Some records were not updated because you don't have permission to change approved records.");

                if (showMessage2)
                    MessageBox.Show("Some of the selected timesheets have already been processed by Payroll." + Environment.NewLine +
                            "Please contact Payroll if changes are needed.");

                if (showMessage3)
                    MessageBox.Show(Core.getLockoutText());
            }                        
        }

        private void Profile_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
            if (e.KeyCode == Keys.Delete)
                deleteToolStripMenuItem_Click(null, null);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (Core.getLockout())
            //{
            //    MessageBox.Show(Core.getLockoutText());
            //    return;
            //}

            if (resultsList.SelectedItems.Count > 0)
            {
                bool canRemove = true;
                string errorStatus = "";

                for (int i = 0; i < resultsList.SelectedItems.Count && canRemove; i++)
                {
                    string status = resultsList.SelectedItems[i].SubItems[resultsList.Columns["Status"].Index].Text;

                    if (status == EXPORTED_TEXT && !Core.isAdmin(Core.getUsername()))
                    {
                        canRemove = false;
                        errorStatus = "Can't remove records that have been processed.";
                    }
                    else if (status == APPROVED_TEXT && !Core.canApprove(Core.getUsername()))
                    {
                        canRemove = false;
                        errorStatus = "Can't remove records that have been approved.";
                    }
                    else if(Core.isTimesheetLocked(int.Parse(resultsList.SelectedItems[i].Tag.ToString()), getEmpType()))
                    {
                        canRemove = false;
                        errorStatus = Core.getLockoutText();
                    }
                }

                if (canRemove)
                {
                    if (MessageBox.Show("Are you sure you want to delete " + (resultsList.SelectedItems.Count > 1 ? "these " + resultsList.SelectedItems.Count + " records?" : "this record?"), "Are you sure?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {
                        for (int i = resultsList.SelectedItems.Count - 1; i >= 0; i--)
                        {
                            string id = resultsList.SelectedItems[i].Tag.ToString();

                            Core.logTimesheetHistory("Timesheet deleted", int.Parse(id));

                            SQL sql = new SQL("delete from Timesheets where timecarddetailid = @ID");
                            sql.AddParameter("@ID", resultsList.SelectedItems[i].Tag.ToString());
                            sql.Run();

                            sql = new SQL("delete from equipmenttimeentry where timesheetid = @ID");
                            sql.AddParameter("@ID", resultsList.SelectedItems[i].Tag.ToString());
                            sql.Run();

                            resultsList.Items.Remove(resultsList.SelectedItems[i]);
                            hoursCalendar1.username = getUsername();
                            hoursCalendar1.showDate(getStart());
                            loadPayTypes();
                            loadEntitlements();

                            Core.logHistory("Deleted timesheet", "Timesheet ID# " + id, "");
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

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateCalendars();

            if(!firstLoad)
                refresh();
        }

        private void updateCalendars()
        {
            //int period = int.Parse(toolStripComboBox3.Text.Split('-')[0].Trim().Replace("H","").Replace("S",""));
            //string empType = getEmpType();
            //DateTime start = Core.getPeriodStart(period, empType);
            //DateTime end = Core.getPeriodEnd(period, empType);

            //ignoreFromCal = true;
            //dateTimePicker1.Value = start;
            //ignoreFromCal = false;

            //ignoreToCal = true;
            //dateTimePicker2.Value = end;
            //ignoreToCal = false;
        }

        private void comboBox3_DropDownClosed(object sender, EventArgs e)
        {
            comboBox3_SelectedIndexChanged(null, null);
        }

        private void hoursCalendar1_DayClicked(DateTime x)
        {
            if (getUsername() != "")
                (new Timesheet(getUsername(), x)).ShowDialog();

            refresh();
        }

        private void resultsList_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                viewHistoryToolStripMenuItem_Click(null, null);
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e2)
        {
            Printer queue = new Printer();
            int employeeID = Core.getEmpID(getUsername());
            int period = int.Parse(toolStripComboBox3.Text.Split('-')[0].Trim().Replace("H", "").Replace("S", ""));
            DateTime date = Core.getPeriodStart(period, getEmpType()).AddDays(7);

            SQL sql = new SQL(@"
select dateworked, hours, p.description, workorder, lumpsum, recordlocked
from Timesheets t
join paycodes p on t.paytype = p.paytype
where t.employeeid = @EMPID
and period=@PERIOD and dateworked < @DATE
order by t.dateworked asc");
            sql.AddParameter("@EMPID", employeeID);
            sql.AddParameter("@PERIOD", period);
            sql.AddParameter("@DATE", date);
            DataTable week1 = sql.Run();

            sql = new SQL(@"
select dateworked, hours, p.description, workorder, lumpsum, recordlocked
from Timesheets t
join paycodes p on t.paytype = p.paytype
where t.employeeid = @EMPID
and period=@PERIOD and dateworked >= @DATE
order by t.dateworked asc");
            sql.AddParameter("@EMPID", employeeID);
            sql.AddParameter("@PERIOD", period);
            sql.AddParameter("@DATE", date);
            DataTable week2 = sql.Run();

            sql = new SQL(@"
select p.description, sum(hours) as Hours
from Timesheets t
join paycodes p on t.paytype = p.paytype
where t.employeeid = @EMPID
and period=@PERIOD and dateworked < @DATE
group by p.description, p.paytype
order by p.paytype");
            sql.AddParameter("@EMPID", employeeID);
            sql.AddParameter("@PERIOD", period);
            sql.AddParameter("@DATE", date);
            DataTable week1PayTypes = sql.Run();

            sql = new SQL(@"
select p.description, sum(hours) as Hours
from Timesheets t
join paycodes p on t.paytype = p.paytype
where t.employeeid = @EMPID
and period=@PERIOD and dateworked >= @DATE
group by p.description, p.paytype
order by p.paytype");
            sql.AddParameter("@EMPID", employeeID);
            sql.AddParameter("@PERIOD", period);
            sql.AddParameter("@DATE", date);
            DataTable week2PayTypes = sql.Run();

            sql = new SQL(@"
select dateworked, hours, p.description, workorder, lumpsum, recordlocked
from Timesheets t
join paycodes p on t.paytype = p.paytype
where t.employeeid = @EMPID
and period<@PERIOD and exported=0
and t.paytype<>0
order by t.dateworked asc");
            sql.AddParameter("@EMPID", employeeID);
            sql.AddParameter("@PERIOD", period);
            DataTable preperiod = sql.Run();

            queue.AddBold("Employee Timesheet");
            queue.Add("From " + Core.getPeriodStart(period, getEmpType()).ToString("d") + " to " + Core.getPeriodEnd(period, getEmpType()).ToString("d"), 350);
            queue.Add("Period:", 640);
            queue.Add(period.ToString(), 740);
            queue.AddLine();
            queue.Add("Employee:");
            queue.Add(Oracle.Run("select YAALPH from " + Core.getSchema(Core.getEnvironment()) + ".F060116 where YAAN8=" + employeeID.ToString()).Rows[0]["YAALPH"].ToString(), 100);
            queue.Add("Employee ID:", 640);
            queue.Add(employeeID.ToString(), 740);
            queue.AddLine();
            queue.Add("Printed:");
            queue.Add(DateTime.Now.ToString(), 100);
            queue.Add("Department:", 640);
            queue.Add(SQL.Run("select da.departmentid from users u join departmentassociations da on u.userid = da.userid where u.employeeid=" + employeeID).Rows.Count > 1 ? "Multiple" : SQL.Run("select d.department from department d join departmentassociations da on d.departmentid = da.departmentid join users u on u.userid = da.userid where u.employeeid = " + employeeID).Rows[0][0].ToString(), 740);
            queue.AddLine();
            queue.AddDivider();

            queue.AddBold("Week 1");
            queue.AddLine();
            queue.AddBold("Date Worked", 10);
            queue.AddBold("Hours", 118);
            queue.AddBold("Pay Type", 178);
            queue.AddBold("Work Order", 283);
            queue.AddBold("Description", 353);
            queue.AddBold("Lump Sum", 525);
            queue.AddBold("Approved", 615);
            queue.AddLine();

            double week1Sub = 0;
            double lumpSumTotal = 0;

            for (int i = 0; i < week1.Rows.Count; i++)
            {
                week1Sub += double.Parse(week1.Rows[i]["hours"].ToString());
                lumpSumTotal += double.Parse(week1.Rows[i]["lumpsum"].ToString());

                queue.Add(DateTime.Parse(week1.Rows[i]["dateworked"].ToString()).ToString("yyyy-MM-dd ddd"), 10);
                queue.Add(week1.Rows[i]["hours"].ToString(), 118);
                queue.Add(week1.Rows[i]["description"].ToString(), 178);
                queue.Add(week1.Rows[i]["workorder"].ToString(), 283);
                queue.Add(week1.Rows[i]["workorder"].ToString() != "" ? Oracle.Run("select wadl01 from " + Core.getSchema(Core.getEnvironment()) + ".f4801 where wadoco=" + week1.Rows[i]["workorder"].ToString()).Rows[0][0].ToString() : "", 353);
                queue.Add(((int)double.Parse(week1.Rows[i]["lumpsum"].ToString())) == 0 ? "" : "$" + ((int)double.Parse(week1.Rows[i]["lumpsum"].ToString())), 525);
                queue.Add(week1.Rows[i]["recordlocked"].ToString() == "True" ? "Yes" : "No", 615);
                queue.AddLine();
            }
            double week1LumpSumTotal = lumpSumTotal;

            queue.AddBold("Week 1 sub-total", 10);
            queue.AddBold(week1Sub.ToString(), 118);
            queue.AddBold(string.Format("${0:N2}", lumpSumTotal), 525);
            queue.AddLine();
            queue.AddDivider();

            queue.AddBold("Week 2");
            queue.AddLine();
            queue.AddBold("Date Worked", 10);
            queue.AddBold("Hours", 118);
            queue.AddBold("Pay Type", 178);
            queue.AddBold("Work Order", 283);
            queue.AddBold("Description", 353);
            queue.AddBold("Lump Sum", 525);
            queue.AddBold("Approved", 615);
            queue.AddLine();

            double week2Sub = 0;

            for (int i = 0; i < week2.Rows.Count; i++)
            {
                week2Sub += double.Parse(week2.Rows[i]["hours"].ToString());
                lumpSumTotal += double.Parse(week2.Rows[i]["lumpsum"].ToString());

                queue.Add(DateTime.Parse(week2.Rows[i]["dateworked"].ToString()).ToString("yyyy-MM-dd ddd"), 10);
                queue.Add(week2.Rows[i]["hours"].ToString(), 118);
                queue.Add(week2.Rows[i]["description"].ToString(), 178);
                queue.Add(week2.Rows[i]["workorder"].ToString(), 283);
                queue.Add(week2.Rows[i]["workorder"].ToString() != "" ? Oracle.Run("select wadl01 from " + Core.getSchema(Core.getEnvironment()) + ".f4801 where wadoco=" + week2.Rows[i]["workorder"].ToString()).Rows[0][0].ToString() : "", 353);
                queue.Add(((int)double.Parse(week2.Rows[i]["lumpsum"].ToString())) == 0 ? "" : "$" + ((int)double.Parse(week2.Rows[i]["lumpsum"].ToString())), 525);
                queue.Add(week2.Rows[i]["recordlocked"].ToString() == "True" ? "Yes" : "No", 615);
                queue.AddLine();
            }

            queue.AddBold("Week 2 sub-total", 10);
            queue.AddBold(week2Sub.ToString(), 118);
            queue.AddBold(string.Format("${0:N2}", lumpSumTotal - week1LumpSumTotal), 525);
            queue.AddLine();
            queue.AddDivider();

            queue.AddBold(string.Format("${0:N2}", lumpSumTotal), 525);
            queue.AddLine();

            for (int i = 0; i < week1PayTypes.Rows.Count; i++)
            {
                queue.Add("Week 1 " + week1PayTypes.Rows[i]["description"].ToString());
                queue.Add(week1PayTypes.Rows[i]["hours"].ToString(), 150);
                queue.AddLine();
            }

            queue.Add("    Week 1 sub-total");
            queue.AddBold(week1Sub.ToString(), 150);
            queue.AddLine();
            queue.AddLine();

            for (int i = 0; i < week2PayTypes.Rows.Count; i++)
            {
                queue.Add("Week 2 " + week2PayTypes.Rows[i]["description"].ToString());
                queue.Add(week2PayTypes.Rows[i]["hours"].ToString(), 150);
                queue.AddLine();
            }

            queue.Add("    Week 2 sub-total");
            queue.AddBold(week2Sub.ToString(), 150);
            queue.AddLine();
            queue.AddLine();

            queue.Add("    Period " + period + " total");
            queue.AddBold((week1Sub + week2Sub).ToString(), 150);
            queue.AddLine();
            queue.AddDivider();

            if(preperiod.Rows.Count > 0)
            {
                queue.AddLine();
                queue.AddBold("Un-Exported Hours form Previous Employee Time Sheets");
                queue.AddLine();
                queue.AddLine();

                lumpSumTotal = 0;
                double preperiodSub = 0;

                for (int i = 0; i < preperiod.Rows.Count; i++)
                {
                    preperiodSub += double.Parse(preperiod.Rows[i]["hours"].ToString());
                    lumpSumTotal += double.Parse(preperiod.Rows[i]["lumpsum"].ToString());

                    queue.Add(DateTime.Parse(preperiod.Rows[i]["dateworked"].ToString()).ToString("yyyy-MM-dd ddd"), 10);
                    queue.Add(preperiod.Rows[i]["hours"].ToString(), 118);
                    queue.Add(preperiod.Rows[i]["description"].ToString(), 178);
                    queue.Add(preperiod.Rows[i]["workorder"].ToString(), 283);
                    queue.Add(preperiod.Rows[i]["workorder"].ToString() != "" ? Oracle.Run("select wadl01 from " + Core.getSchema(Core.getEnvironment()) + ".f4801 where wadoco=" + preperiod.Rows[i]["workorder"].ToString()).Rows[0][0].ToString() : "", 353);
                    queue.Add(((int)double.Parse(preperiod.Rows[i]["lumpsum"].ToString())) == 0 ? "" : "$" + ((int)double.Parse(preperiod.Rows[i]["lumpsum"].ToString())), 525);
                    queue.Add(preperiod.Rows[i]["recordlocked"].ToString() == "True" ? "Yes" : "No", 615);
                    queue.AddLine();
                }

                queue.AddBold(preperiodSub.ToString(), 118);
                queue.AddBold(string.Format("${0:N2}", lumpSumTotal), 525);
                queue.AddLine();
                queue.AddLine();
            }
            queue.AddPage();

            queue.Print();
        }

        private void selectSpecificDatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void toolStripMenuItem10_Click(object sender, EventArgs e)
        {
            
        }

        private void paychequesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Oracle ora = new Oracle("select YUCKCN, YUPPED from " + Core.getSchema(Core.getEnvironment()) + ".F06156 where YUAN8 = @EMPID group by YUCKCN, YUPPED");
            ora.AddParameter("@EMPID", Core.getEmpID(Core.getUsername()));
            DataTable dt = ora.Run();

            FindPaycheque fpc = new FindPaycheque(dt);
            fpc.ShowDialog();

            string chequeNumber = fpc.number;

            if (fpc.number != "")
            {
                ora = new Oracle(@"
select
Y8DL01 as "" "",
to_char(Y8HRW/100, '999,999.99') as ""HRS"",
to_char(Y8SHRT/1000, '999,999.999') as ""Rate"",
to_char(Y8GPAY/100, '999,999.99') as ""Current"",
to_char(Y8YTD/100, '999,999.99') as ""Year to Date"",
Y8DL02 as ""  "",
to_char(Y8SCUR/100, '999,999.99') as ""Current "",
to_char(Y8YTDG/100, '999,999.99') as ""Year to Date ""

from " + Core.getSchema(Core.getEnvironment()) + @".F07186

where Y8CKCN = @CN

order by Y8STLN");
                ora.AddParameter("@CN", chequeNumber);
                dt = ora.Run();

                DataTable dtCloned = dt.Clone();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    dtCloned.Columns[i].DataType = typeof(string);
                }

                foreach (DataRow row in dt.Rows)
                {
                    dtCloned.ImportRow(row);
                }

                double totalHours = 0;
                double totalCurrent = 0;
                double totalCurrent2 = 0;
                double total = 0;

                for (int r = 0; r < dtCloned.Rows.Count; r++)
                {
                    for (int c = 0; c < dtCloned.Columns.Count; c++)
                    {
                        if (dtCloned.Rows[r][c].ToString() == "0" || dtCloned.Rows[r][c].ToString().Trim() == ".00" || dtCloned.Rows[r][c].ToString().Trim() == ".000")
                            dtCloned.Rows[r][c] = "";

                        if (dtCloned.Rows[r][c].ToString().Trim() == "Gross Wages")
                            dtCloned.Rows[r][c] = "----------- Gross -----------";

                        if (dtCloned.Rows[r][c].ToString().Trim() == "Bkd OT @ Reg"
                            || dtCloned.Rows[r][c].ToString().Trim() == "Bk $ Rev"
                            || dtCloned.Rows[r][c].ToString().Trim() == "Bkd OT @ 1.5")
                        {
                            for (int x = 0; x < dtCloned.Columns.Count; x++)
                                dtCloned.Rows[r][x] = "";
                        }
                    }

                    if (dtCloned.Rows[r][1].ToString() != "")
                        totalHours += double.Parse(dtCloned.Rows[r][1].ToString());

                    if (dtCloned.Rows[r][3].ToString() != "")
                        totalCurrent += double.Parse(dtCloned.Rows[r][3].ToString().Replace('$', ' ').Trim());

                    if (dtCloned.Rows[r][6].ToString() != "")
                        totalCurrent2 += double.Parse(dtCloned.Rows[r][6].ToString().Replace('$', ' ').Trim());
                }

                totalCurrent2 -= double.Parse(dtCloned.Rows[0][6].ToString().Replace('$', ' ').Trim());
                total = totalCurrent - totalCurrent2;

                string[] temp = { };

                dtCloned.Rows.Add(temp);
                dtCloned.Rows.Add(temp);
                dtCloned.Rows.Add(temp);

                //string[] temp2 = { "* Gross", totalHours.ToString(), "", totalCurrent.ToString(), "", "* Deductions", totalCurrent2.ToString(), "* Net " + total };

                //dtCloned.Rows.Add(temp2);
                //            sql = new SQL(@"SELECT d.Department as Department, u.USERNAME as Username, u.EMPLOYEEID as 'Employee ID',
                //t.PayType as 'Pay Code', p.Description as 'Pay Type', t.hours as Hours, t.DateWorked as 'Date Worked'
                //  FROM [DATS].[dbo].Users u
                //  join DepartmentAssociations da on da.UserID = u.USERID
                //  join Department d on d.DepartmentID = da.DepartmentID
                //  join Timesheets t on u.EMPLOYEEID = t.EmployeeID
                //  join PayCodes p on p.PayType = t.PayType
                //  where Period=@PERIOD and p.AbsYN <> 0
                //  and d.departmentid in (" + departmentSpread + @")
                //  order by t.DateWorked, d.Department, u.USERNAME");
                //            sql.AddParameter("@PERIOD", Core.getPeriod(DateTime.Now));
                //            dt = sql.Run();

                ora = new Oracle(@"select
YAALPH,
YUAN8,
YUMAIL,
YUDOCM,
YUCKD,
YUPPED,
to_char(YUHRW/100, '999,999.99') as YUHRW,
to_char(sum(YUGPAY)/100, '999,999.99') as YUGPAY,
to_char(sum((YUGTXW+YUGDED))/100, '999,999.99') as DED,
to_char(sum(YUNPAY)/100, '999,999.99') as YUNPAY
from " + Core.getSchema(Core.getEnvironment()) + @".F06156
join " + Core.getSchema(Core.getEnvironment()) + @".F060116 on YUAN8 = YAAN8
where YUCKCN=@CHEQUENO
group by YAALPH, YUAN8, YUMAIL, YUDOCM, YUCKD, YUPPED, YUHRW");
                ora.AddParameter("@CHEQUENO", chequeNumber);
                DataTable dt2 = ora.Run();

                ReportPaycheque rpt = new ReportPaycheque("Paycheque", dtCloned, dt2, chequeNumber);
                rpt.ShowDialog();
            }
        }

        private void absencesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string searchByTime = Core.lookupMessageBox("Time filter", "This report is filtered by time." + Environment.NewLine
                + "Would you like to search by pay period or date range?", "Period", "Date range");

            if (searchByTime == "")
                return;

            string empType = Core.lookupMessageBox("Employee type filter", "This report is filtered by employee type." + Environment.NewLine
                + "Would you like to search for hourly or salary?", "Hourly", "Salary");

            if (empType == "")
                return;

            empType = empType.Substring(0, 1); //S, H

            string period = "";
            DateTime start = new DateTime(1900, 1, 1);
            DateTime end = new DateTime(1900, 1, 1);

            if (searchByTime == "Period")
            {
                FindPeriod fp = new FindPeriod(empType, false);
                fp.ShowDialog();
                period = fp.period;

                if (period == "")
                    return;
            }
            else if(searchByTime == "Date range")
            {
                FindDate fd = new FindDate("From date");
                fd.ShowDialog();
                start = fd.date;

                if (start.Year == 1900)
                    return;

                fd = new FindDate("To date");
                fd.ShowDialog();
                end = fd.date;

                if (end.Year == 1900)
                    return;
            }

            string departmentSpread = "";
            DataTable dt = null;

            if (!Core.isAdmin(Core.getUsername()))
                dt = SQL.Run("select departmentid from departmentassociations da join users u on da.userid = u.userid where u.displayname=@USERNAME", Core.getUsername());
            else
                dt = SQL.Run("select departmentid from department");

            for (int i = 0; i < dt.Rows.Count; i++)
                departmentSpread += (i == 0 ? "" : ", ") + dt.Rows[i][0].ToString();

            if (searchByTime == "Period")
            {
                dt = SQL.Run(@"SELECT d.Department as Department, u.USERNAME as Username, u.EMPLOYEEID as 'Employee ID',
t.PayType as 'Pay Code', p.Description as 'Pay Type', t.hours as Hours, t.DateWorked as 'Date Worked'
FROM [DATS].[dbo].Users u
join DepartmentAssociations da on da.UserID = u.USERID
join Department d on d.DepartmentID = da.DepartmentID
join Timesheets t on u.EMPLOYEEID = t.EmployeeID
join PayCodes p on p.PayType = t.PayType
where Period=@PERIOD and p.AbsYN <> 0
and d.departmentid in (" + departmentSpread + @")
and u.active=1
and u.enterstime=1
and u.emptype=@EMPTYPE

order by t.DateWorked, d.Department, u.USERNAME", period, empType);
                //and t.paytype <> 0
            }
            else
            {
                dt = SQL.Run(@"SELECT d.Department as Department, u.USERNAME as Username, u.EMPLOYEEID as 'Employee ID',
t.PayType as 'Pay Code', p.Description as 'Pay Type', t.hours as Hours, t.DateWorked as 'Date Worked'
FROM [DATS].[dbo].Users u
join DepartmentAssociations da on da.UserID = u.USERID
join Department d on d.DepartmentID = da.DepartmentID
join Timesheets t on u.EMPLOYEEID = t.EmployeeID
join PayCodes p on p.PayType = t.PayType
where t.dateworked >= @START and t.dateworked < @END and p.AbsYN <> 0
and d.departmentid in (" + departmentSpread + @")
and u.active=1
and u.enterstime=1
and u.emptype=@EMPTYPE
order by t.DateWorked, d.Department, u.USERNAME", start, end.AddDays(1), empType);
            }
            //and t.paytype <> 0

            Report rpt = new Report("Absence Report", dt);
            rpt.Show();
        }

        private void closedWorkOrdersToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string searchByTime = Core.lookupMessageBox("Time filter", "This report looks at work orders used during a timeframe." + Environment.NewLine
                + "Would you like to search by pay period or date range?", "Period", "Date range");

            if (searchByTime == "")
                return;

            string period = "";
            DateTime start = new DateTime(1900, 1, 1);
            DateTime end = new DateTime(1900, 1, 1);

            if (searchByTime == "Period")
            {
                string empType = Core.lookupMessageBox("Period selection", "You are about to select from a list of pay periods." + Environment.NewLine
                + "Would you like to see for hourly or salary pay periods?", "Hourly", "Salary");

                if (empType == "")
                    return;

                empType = empType.Substring(0, 1); //S, H

                FindPeriod fp = new FindPeriod(empType, false);
                fp.ShowDialog();
                period = fp.period;

                if (period == "")
                    return;
            }
            else if (searchByTime == "Date range")
            {
                FindDate fd = new FindDate("From date");
                fd.ShowDialog();
                start = fd.date;

                if (start.Year == 1900)
                    return;

                fd = new FindDate("To date");
                fd.ShowDialog();
                end = fd.date;

                if (end.Year == 1900)
                    return;
            }

            string departmentSpread = "";
            DataTable dt = null;

            if (!Core.isAdmin(Core.getUsername()))
                dt = SQL.Run("select departmentid from departmentassociations da join users u on da.userid = u.userid where u.displayname=@USERNAME", Core.getUsername());
            else
                dt = SQL.Run("select departmentid from department");

            for (int i = 0; i < dt.Rows.Count; i++)
                departmentSpread += (i == 0 ? "" : ", ") + dt.Rows[i][0].ToString();

            dt = Oracle.Run("select WADOCO from " + Core.getSchema(Core.getEnvironment()) + ".F4801 where WASRST = '99'");

            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("No work orders in JDE are closed.");
                return;
            }

            string workorderSpread = "";

            for (int i = 0; i < dt.Rows.Count; i++)
                workorderSpread += (i == 0 ? "" : ", ") + dt.Rows[i][0].ToString();

            if(searchByTime == "Period")
                dt = SQL.Run(@"select u.username as Employee, Hours, dateworked as 'Date Worked', WorkOrder as 'Work Order' from Timesheets t
    join Users u on t.EmployeeID = u.EMPLOYEEID
    join DepartmentAssociations da on u.USERID = da.UserID
    join Department d on d.DepartmentID = da.DepartmentID

    where
    t.WorkOrder <> 0
    and u.active = 1
    and t.Period = @PERIOD
    and t.WorkOrder is not null
    and d.DepartmentID in (" + departmentSpread + @")
    and t.workorder in (" + workorderSpread + ") order by dateworked", period);
            else if(searchByTime == "Date range")
                dt = SQL.Run(@"select u.username as Employee, Hours, dateworked as 'Date Worked', WorkOrder as 'Work Order' from Timesheets t
    join Users u on t.EmployeeID = u.EMPLOYEEID
    join DepartmentAssociations da on u.USERID = da.UserID
    join Department d on d.DepartmentID = da.DepartmentID

    where
    t.WorkOrder <> 0
    and u.active = 1
    and t.dateworked >= @STARTDATE and t.dateworked < @ENDDATE
    and t.WorkOrder is not null
    and d.DepartmentID in (" + departmentSpread + @")
    and t.workorder in (" + workorderSpread + ") order by dateworked", start, end.AddDays(1));

            Report rpt = new Report("Closed Work Order Report", dt);
            rpt.Show();
        }

        private void notApprovedToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string searchByTime = Core.lookupMessageBox("Time filter", "This report is filtered by time." + Environment.NewLine
                + "Would you like to search by pay period or date range?", "Period", "Date range");

            if (searchByTime == "")
                return;

            string empType = Core.lookupMessageBox("Employee type filter", "This report is filtered by employee type." + Environment.NewLine
                + "Would you like to search for hourly or salary?", "Hourly", "Salary");

            if (empType == "")
                return;

            empType = empType.Substring(0, 1); //S, H

            string period = "";
            DateTime start = new DateTime(1900, 1, 1);
            DateTime end = new DateTime(1900, 1, 1);

            if (searchByTime == "Period")
            {
                FindPeriod fp = new FindPeriod(empType, false);
                fp.ShowDialog();
                period = fp.period;

                if (period == "")
                    return;
            }
            else if (searchByTime == "Date range")
            {
                FindDate fd = new FindDate("From date");
                fd.ShowDialog();
                start = fd.date;

                if (start.Year == 1900)
                    return;

                fd = new FindDate("To date");
                fd.ShowDialog();
                end = fd.date;

                if (end.Year == 1900)
                    return;
            }

            string departmentSpread = "";
            DataTable dt = null;

            if (!Core.isAdmin(Core.getUsername()))
                dt = SQL.Run("select departmentid from departmentassociations da join users u on da.userid = u.userid where u.displayname=@USERNAME", Core.getUsername());
            else
                dt = SQL.Run("select departmentid from department");

            for (int i = 0; i < dt.Rows.Count; i++)
                departmentSpread += (i == 0 ? "" : ", ") + dt.Rows[i][0].ToString();

            if(searchByTime == "Period")
                dt = SQL.Run(@"SELECT d.Department as Department, u.USERNAME as Username, u.EMPLOYEEID as 'Employee ID', sum(t.Hours) as 'Hours Unapproved'
      FROM [DATS].[dbo].Users u
      join DepartmentAssociations da on da.UserID = u.USERID
      join Department d on d.DepartmentID = da.DepartmentID
      join Timesheets t on u.EMPLOYEEID = t.EmployeeID
      where t.RecordLocked = 'False'
      and Period=@PERIOD
      and u.active = 1
      and t.paytype <> 0
      and u.emptype = @EMPTYPE
      and d.departmentid in (" + departmentSpread + @")
      group by d.Department, u.USERNAME, u.EMPLOYEEID
      order by d.Department, u.USERNAME", period, empType);
            else if (searchByTime == "Date range")
                dt = SQL.Run(@"SELECT d.Department as Department, u.USERNAME as Username, u.EMPLOYEEID as 'Employee ID', sum(t.Hours) as 'Hours Unapproved'
  FROM [DATS].[dbo].Users u
  join DepartmentAssociations da on da.UserID = u.USERID
  join Department d on d.DepartmentID = da.DepartmentID
  join Timesheets t on u.EMPLOYEEID = t.EmployeeID
  where t.RecordLocked = 'False'
  and t.dateworked >= @STARTDATE and t.dateworked < @ENDDATE
  and u.active = 1
  and u.emptype = @EMPTYPE
  and t.paytype <> 0
  and d.departmentid in (" + departmentSpread + @")
  group by d.Department, u.USERNAME, u.EMPLOYEEID
  order by d.Department, u.USERNAME", start, end.AddDays(1), empType);

            Report rpt = new Report("Not Approved Report", dt);
            rpt.Show();
        }

        private void notExportedToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string searchByTime = Core.lookupMessageBox("Time filter", "This report is filtered by time." + Environment.NewLine
                + "Would you like to search by pay period or date range?", "Period", "Date range");

            if (searchByTime == "")
                return;

            string empType = Core.lookupMessageBox("Employee type filter", "This report is filtered by employee type." + Environment.NewLine
                + "Would you like to search for hourly or salary?", "Hourly", "Salary");

            if (empType == "")
                return;

            empType = empType.Substring(0, 1); //S, H

            string period = "";
            DateTime start = new DateTime(1900, 1, 1);
            DateTime end = new DateTime(1900, 1, 1);

            if (searchByTime == "Period")
            {
                FindPeriod fp = new FindPeriod(empType, false);
                fp.ShowDialog();
                period = fp.period;

                if (period == "")
                    return;
            }
            else if (searchByTime == "Date range")
            {
                FindDate fd = new FindDate("From date");
                fd.ShowDialog();
                start = fd.date;

                if (start.Year == 1900)
                    return;

                fd = new FindDate("To date");
                fd.ShowDialog();
                end = fd.date;

                if (end.Year == 1900)
                    return;
            }

            string departmentSpread = "";
            DataTable dt = null;

            if (!Core.isAdmin(Core.getUsername()))
                dt = SQL.Run("select departmentid from departmentassociations da join users u on da.userid = u.userid where u.displayname=@USERNAME", Core.getUsername());
            else
                dt = SQL.Run("select departmentid from department");

            for (int i = 0; i < dt.Rows.Count; i++)
                departmentSpread += (i == 0 ? "" : ", ") + dt.Rows[i][0].ToString();

            if (searchByTime == "Period")
                dt = SQL.Run(@"SELECT d.Department as Department, u.USERNAME as Username, u.EMPLOYEEID as 'Employee ID', sum(t.Hours) as 'Hours Unapproved'
      FROM [DATS].[dbo].Users u
      join DepartmentAssociations da on da.UserID = u.USERID
      join Department d on d.DepartmentID = da.DepartmentID
      join Timesheets t on u.EMPLOYEEID = t.EmployeeID
      where exported = 0
      and Period=@PERIOD
      and u.active = 1
      and t.paytype <> 0
      and u.emptype = @EMPTYPE
      and d.departmentid in (" + departmentSpread + @")
      group by d.Department, u.USERNAME, u.EMPLOYEEID
      order by d.Department, u.USERNAME", period, empType);
            else if (searchByTime == "Date range")
                dt = SQL.Run(@"SELECT d.Department as Department, u.USERNAME as Username, u.EMPLOYEEID as 'Employee ID', sum(t.Hours) as 'Hours Unapproved'
  FROM [DATS].[dbo].Users u
  join DepartmentAssociations da on da.UserID = u.USERID
  join Department d on d.DepartmentID = da.DepartmentID
  join Timesheets t on u.EMPLOYEEID = t.EmployeeID
  where exported = 0
  and t.dateworked >= @STARTDATE and t.dateworked < @ENDDATE
  and u.active = 1
  and u.emptype = @EMPTYPE
  and t.paytype <> 0
  and d.departmentid in (" + departmentSpread + @")
  group by d.Department, u.USERNAME, u.EMPLOYEEID
  order by d.Department, u.USERNAME", start, end.AddDays(1), empType);

            Report rpt = new Report("Not Exported Report", dt);
            rpt.Show();
        }

        private void paytypeSummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SQL sql = new SQL(@"select p.Description as 'Pay Type', sum(hours) as 'Hours this year' from Timesheets t
join Users u on t.EmployeeID = u.EMPLOYEEID
join PayCodes p on t.PayType = p.PayType

where u.displayname = @USERNAME
and DateWorked >= @DATE

group by p.Description

order by p.Description");
            sql.AddParameter("@USERNAME", Core.getUsername());
            sql.AddParameter("@DATE", new DateTime(DateTime.Today.Year, 1, 1));
            DataTable dt = sql.Run();

            Report rpt = new Report("Paytype Summary", dt);
            rpt.ShowDialog();
        }

        private void prePeriodToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string searchByTime = Core.lookupMessageBox("Time filter", "This report is filtered by time." + Environment.NewLine
    + "Would you like to search by pay period or date range?", "Period", "Date range");

            if (searchByTime == "")
                return;

            string empType = Core.lookupMessageBox("Employee type filter", "This report is filtered by employee type." + Environment.NewLine
                + "Would you like to search for hourly or salary?", "Hourly", "Salary");

            if (empType == "")
                return;

            empType = empType.Substring(0, 1); //S, H

            string period = "";
            DateTime start = new DateTime(1900, 1, 1);
            DateTime end = new DateTime(1900, 1, 1);

            if (searchByTime == "Period")
            {
                FindPeriod fp = new FindPeriod(empType, false);
                fp.ShowDialog();
                period = fp.period;

                if (period == "")
                    return;
            }
            else if (searchByTime == "Date range")
            {
                FindDate fd = new FindDate("From date");
                fd.ShowDialog();
                start = fd.date;

                if (start.Year == 1900)
                    return;

                fd = new FindDate("To date");
                fd.ShowDialog();
                end = fd.date;

                if (end.Year == 1900)
                    return;
            }

            string departmentSpread = "";
            DataTable dt = null;

            if (!Core.isAdmin(Core.getUsername()))
                dt = SQL.Run("select departmentid from departmentassociations da join users u on da.userid = u.userid where u.displayname=@USERNAME", Core.getUsername());
            else
                dt = SQL.Run("select departmentid from department");

            for (int i = 0; i < dt.Rows.Count; i++)
                departmentSpread += (i == 0 ? "" : ", ") + dt.Rows[i][0].ToString();

            if(searchByTime == "Period")
                dt = SQL.Run(@"SELECT min(d.Department) as 'Department', u.USERNAME, u.EMPLOYEEID, convert(varchar(50), t.DateWorked) as DateWorked, p.Description, t.Hours, t.WorkOrder, t.LumpSum, t.RecordLocked as 'Approved', t.Exported as 'Exported'
      FROM [DATS].[dbo].Users u
      join DepartmentAssociations da on da.UserID = u.USERID
      join Department d on d.DepartmentID = da.DepartmentID
      join Timesheets t on u.EMPLOYEEID = t.EmployeeID
      join PayCodes p on t.PayType = p.PayType
      where d.departmentid in (" + departmentSpread + @")
      and Period<@PERIOD
      and exported=0
      and p.paytype<>0
      and u.active = 1
      and u.emptype = @EMPTYPE
      and not (p.paytype = 811 and u.emptype = 'S')
      group by USERNAME, u.EmployeeID, DateWorked, p.Description, t.Hours, t.WorkOrder, t.LumpSum, t.RecordLocked, t.Exported
      order by t.dateworked, min(d.Department), u.USERNAME", period, empType);
            else if(searchByTime == "Date range")
                dt = SQL.Run(@"SELECT min(d.Department) as 'Department', u.USERNAME, u.EMPLOYEEID, convert(varchar(50), t.DateWorked) as DateWorked, p.Description, t.Hours, t.WorkOrder, t.LumpSum, t.RecordLocked as 'Approved', t.Exported as 'Exported'
  FROM [DATS].[dbo].Users u
  join DepartmentAssociations da on da.UserID = u.USERID
  join Department d on d.DepartmentID = da.DepartmentID
  join Timesheets t on u.EMPLOYEEID = t.EmployeeID
  join PayCodes p on t.PayType = p.PayType
  where d.departmentid in (" + departmentSpread + @")
  and t.dateworked >= @STARTDATE and t.dateworked < @ENDDATE
  and exported=0
  and p.paytype<>0
  and u.active = 1
  and u.emptype = @EMPTYPE
  and not (p.paytype = 811 and u.emptype = 'S')
  group by USERNAME, u.EmployeeID, DateWorked, p.Description, t.Hours, t.WorkOrder, t.LumpSum, t.RecordLocked, t.Exported
  order by t.dateworked, min(d.Department), u.USERNAME", start, end.AddDays(1), empType);
            
            for (int i = 0; i < dt.Rows.Count; i++)
                dt.Rows[i]["DateWorked"] = DateTime.Parse(dt.Rows[i]["DateWorked"].ToString()).ToString("D");

            Report rpt = new Report("Pre-Period Report", dt);
            rpt.Show();
        }

        private void summaryToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string searchByTime = Core.lookupMessageBox("Time filter", "This report is filtered by time." + Environment.NewLine
    + "Would you like to search by pay period or date range?", "Period", "Date range");

            if (searchByTime == "")
                return;

            string empType = Core.lookupMessageBox("Employee type filter", "This report is filtered by employee type." + Environment.NewLine
                + "Would you like to search for hourly or salary?", "Hourly", "Salary");

            if (empType == "")
                return;

            empType = empType.Substring(0, 1); //S, H

            string period = "";
            DateTime start = new DateTime(1900, 1, 1);
            DateTime end = new DateTime(1900, 1, 1);

            if (searchByTime == "Period")
            {
                FindPeriod fp = new FindPeriod(empType, false);
                fp.ShowDialog();
                period = fp.period;

                if (period == "")
                    return;
            }
            else if (searchByTime == "Date range")
            {
                FindDate fd = new FindDate("From date");
                fd.ShowDialog();
                start = fd.date;

                if (start.Year == 1900)
                    return;

                fd = new FindDate("To date");
                fd.ShowDialog();
                end = fd.date;

                if (end.Year == 1900)
                    return;
            }

            string departmentSpread = "";
            DataTable dt = null;

            if (!Core.isAdmin(Core.getUsername()))
                dt = SQL.Run("select departmentid from departmentassociations da join users u on da.userid = u.userid where u.displayname=@USERNAME", Core.getUsername());
            else
                dt = SQL.Run("select departmentid from department");

            for (int i = 0; i < dt.Rows.Count; i++)
                departmentSpread += (i == 0 ? "" : ", ") + dt.Rows[i][0].ToString();

            if(searchByTime == "Period")
                dt = SQL.Run(@"SELECT min(d.Department) as 'Department', u.USERNAME as 'Username', u.EMPLOYEEID as 'Employee ID',

    (select SUM(hours)
    from Timesheets t2
    join PayCodes p2 on t2.PayType = p2.PayType
    where t2.employeeid = min(t.EmployeeID)
    and Period=@PERIOD1
    and p2.Description = 'Regular') as Regular,

    (select SUM(hours)
    from Timesheets t2
    join PayCodes p2 on t2.PayType = p2.PayType
    where t2.employeeid = min(t.EmployeeID)
    and Period=@PERIOD2
    and p2.OTYN <> 0) as Overtime,

    (select SUM(hours)
    from Timesheets t2
    join PayCodes p2 on t2.PayType = p2.PayType
    where t2.employeeid = min(t.EmployeeID)
    and Period=@PERIOD3
    and p2.VacYN <> 0) as Vacation,

    (select SUM(hours)
    from Timesheets t2
    join PayCodes p2 on t2.PayType = p2.PayType
    where t2.employeeid = min(t.EmployeeID)
    and Period=@PERIOD4
    and p2.AbsYN <> 0) as Absent,

    (select SUM(hours)
    from Timesheets t2
    join PayCodes p2 on t2.PayType = p2.PayType
    where t2.employeeid = min(t.EmployeeID)
    and Period=@PERIOD5
    and p2.Description <> 'Regular' and p2.VacYN = 0 and p2.OTYN = 0 and p2.AbsYN = 0) as Other,

    (select SUM(hours)
    from Timesheets t2
    join PayCodes p2 on t2.PayType = p2.PayType
    where t2.employeeid = min(t.EmployeeID)
    and Period=@PERIOD6) as Total,

    (select SUM(LumpSum)
    from Timesheets t2
    join PayCodes p2 on t2.PayType = p2.PayType
    where t2.employeeid = min(t.EmployeeID)
    and Period=@PERIOD7) as 'Lump Sum'

      FROM [DATS].[dbo].Users u
      join DepartmentAssociations da on da.UserID = u.USERID
      join Department d on d.DepartmentID = da.DepartmentID
      join Timesheets t on u.EMPLOYEEID = t.EmployeeID
      join PayCodes p on p.PayType = t.PayType
      where Period=@PERIOD8
      and d.departmentid in (" + departmentSpread + @")
      and u.active = 1
      and u.enterstime=1
      and u.emptype = @EMPTYPE
      group by u.USERNAME, u.EMPLOYEEID
      order by department, username", period, period, period, period, period, period, period, period, empType);
            else if(searchByTime == "Date range")
                dt = SQL.Run(@"SELECT min(d.Department) as 'Department', u.USERNAME as 'Username', u.EMPLOYEEID as 'Employee ID',

(select SUM(hours)
from Timesheets t2
join PayCodes p2 on t2.PayType = p2.PayType
where t2.employeeid = min(t.EmployeeID)
and t2.dateworked >= @STARTDATE1 and t2.dateworked < @ENDDATE1
and p2.Description = 'Regular') as Regular,

(select SUM(hours)
from Timesheets t2
join PayCodes p2 on t2.PayType = p2.PayType
where t2.employeeid = min(t.EmployeeID)
and t2.dateworked >= @STARTDATE2 and t2.dateworked < @ENDDATE2
and p2.OTYN <> 0) as Overtime,

(select SUM(hours)
from Timesheets t2
join PayCodes p2 on t2.PayType = p2.PayType
where t2.employeeid = min(t.EmployeeID)
and t2.dateworked >= @STARTDATE3 and t2.dateworked < @ENDDATE3
and p2.VacYN <> 0) as Vacation,

(select SUM(hours)
from Timesheets t2
join PayCodes p2 on t2.PayType = p2.PayType
where t2.employeeid = min(t.EmployeeID)
and t2.dateworked >= @STARTDATE4 and t2.dateworked < @ENDDATE4
and p2.AbsYN <> 0) as Absent,

(select SUM(hours)
from Timesheets t2
join PayCodes p2 on t2.PayType = p2.PayType
where t2.employeeid = min(t.EmployeeID)
and t2.dateworked >= @STARTDATE5 and t2.dateworked < @ENDDATE5
and p2.Description <> 'Regular' and p2.VacYN = 0 and p2.OTYN = 0 and p2.AbsYN = 0) as Other,

(select SUM(hours)
from Timesheets t2
join PayCodes p2 on t2.PayType = p2.PayType
where t2.employeeid = min(t.EmployeeID)
and t2.dateworked >= @STARTDATE6 and t2.dateworked < @ENDDATE6) as Total,

(select SUM(LumpSum)
from Timesheets t2
join PayCodes p2 on t2.PayType = p2.PayType
where t2.employeeid = min(t.EmployeeID)
and t2.dateworked >= @STARTDATE7 and t2.dateworked < @ENDDATE7) as 'Lump Sum'

  FROM [DATS].[dbo].Users u
  join DepartmentAssociations da on da.UserID = u.USERID
  join Department d on d.DepartmentID = da.DepartmentID
  join Timesheets t on u.EMPLOYEEID = t.EmployeeID
  join PayCodes p on p.PayType = t.PayType
  where t.dateworked >= @STARTDATE8 and t.dateworked < @ENDDATE8
  and d.departmentid in (" + departmentSpread + @")
  and u.active = 1
  and u.emptype = @EMPTYPE
  group by u.USERNAME, u.EMPLOYEEID
  order by department, username", start, end.AddDays(1), start, end.AddDays(1), start, end.AddDays(1), start, end.AddDays(1), start, end.AddDays(1), start, end.AddDays(1), start, end.AddDays(1), start, end.AddDays(1), empType);

            Report rpt = new Report("Summary Report", dt);
            rpt.Show();
        }

        private void summaryBreakdownToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string searchByTime = Core.lookupMessageBox("Time filter", "This report is filtered by time." + Environment.NewLine
    + "Would you like to search by pay period or date range?", "Period", "Date range");

            if (searchByTime == "")
                return;

            string empType = Core.lookupMessageBox("Employee type filter", "This report is filtered by employee type." + Environment.NewLine
                + "Would you like to search for hourly or salary?", "Hourly", "Salary");

            if (empType == "")
                return;

            empType = empType.Substring(0, 1); //S, H

            string period = "";
            DateTime start = new DateTime(1900, 1, 1);
            DateTime end = new DateTime(1900, 1, 1);

            if (searchByTime == "Period")
            {
                FindPeriod fp = new FindPeriod(empType, false);
                fp.ShowDialog();
                period = fp.period;

                if (period == "")
                    return;
            }
            else if (searchByTime == "Date range")
            {
                FindDate fd = new FindDate("From date");
                fd.ShowDialog();
                start = fd.date;

                if (start.Year == 1900)
                    return;

                fd = new FindDate("To date");
                fd.ShowDialog();
                end = fd.date;

                if (end.Year == 1900)
                    return;
            }

            string departmentSpread = "";
            DataTable dt = null;

            if (!Core.isAdmin(Core.getUsername()))
                dt = SQL.Run("select departmentid from departmentassociations da join users u on da.userid = u.userid where u.displayname=@USERNAME", Core.getUsername());
            else
                dt = SQL.Run("select departmentid from department");

            for (int i = 0; i < dt.Rows.Count; i++)
                departmentSpread += (i == 0 ? "" : ", ") + dt.Rows[i][0].ToString();

            if (searchByTime == "Period")
                dt = SQL.Run(@"
    select
    min(d.Department) as 'Department',
    u.USERNAME as 'Name',
    u.EMPLOYEEID as 'Employee ID',
    p.Description as 'Pay Type',
    SUM(hours) / count(distinct d.departmentid) as 'Sum of Hours',
    SUM(lumpsum) / count(distinct d.departmentid) as 'Lump Sum'

    from users u
    join Timesheets t on t.EmployeeID = u.EMPLOYEEID
    join PayCodes p on t.PayType = p.PayType
    join DepartmentAssociations da on u.USERID = da.UserID
    join Department d on da.DepartmentID = d.DepartmentID

    where d.departmentid in (" + departmentSpread + @")
    and t.period = @PERIOD
    and u.active = 1
    and u.enterstime=1
    and u.emptype = @EMPTYPE
    and t.paytype <> 0

    group by
    u.USERNAME,
    u.EMPLOYEEID,
    p.description

    order by min(d.Department)
    ", period, empType);
            else if(searchByTime == "Date range")
                dt = SQL.Run(@"
    select
    min(d.Department) as 'Department',
    u.USERNAME as 'Name',
    u.EMPLOYEEID as 'Employee ID',
    p.Description as 'Pay Type',
    SUM(hours) / count(distinct d.departmentid) as 'Sum of Hours',
    SUM(lumpsum) / count(distinct d.departmentid) as 'Lump Sum'

    from users u
    join Timesheets t on t.EmployeeID = u.EMPLOYEEID
    join PayCodes p on t.PayType = p.PayType
    join DepartmentAssociations da on u.USERID = da.UserID
    join Department d on da.DepartmentID = d.DepartmentID

    where d.departmentid in (" + departmentSpread + @")
    and t.dateworked >= @START
    and t.dateworked <= @END
    and u.active = 1
    and u.emptype = @EMPTYPE
    and t.paytype <> 0

    group by
    u.USERNAME,
    u.EMPLOYEEID,
    p.description

    order by min(d.Department)
    ", start, end.AddDays(1), empType);

            Report rpt = new Report("Summary Breakdown Report", dt);
            rpt.Show();
        }

        private static int exportToJDE(string type)
        {
            PayrollExport pe = new PayrollExport(type);
            pe.ShowDialog();

            return pe.batchID;
        }

        private void modifyLockoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string result = Core.lookupMessageBox("Change Lockout", "The system is currently " + (Core.getVariable("Lockout") == "True" ? "locked." : "open."),
                (Core.getVariable("Lockout") == "True" ? "Unlock it" : "Lock it"),
                "Keep it as is");

            if (result == "Unlock it")
            {
                Core.logHistory("Variable changed", "Lockout = False", "");
                SQL.Run("update systemvariables set value='False' where variable='Lockout'");
                MessageBox.Show("The system has been unlocked.");
            }
            else if (result == "Lock it")
            {
                Core.logHistory("Variable changed", "Lockout = True", "");
                SQL.Run("update systemvariables set value='True' where variable='Lockout'");
                MessageBox.Show("The system has been locked.");
            }
        }

        private void modifyEmailPayrollToolStripMenuItem_Click(char type)
        {
            if (type == 'H')
                modifyEmailPayrollHourly(null, null);
            else
                modifyEmailPayrollSalary(null, null);
        }

        private void sendDataToJDEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("You are about to export hourly timesheet data to JDE." + Environment.NewLine + "Would you like to proceed?", "Export to JDE", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                int batchID = exportToJDE("H");

                if (batchID != -1)
                {
                    Core.logHistory("Exported hourly batch", "Batch ID " + batchID, "");
                    modifyLockoutToolStripMenuItem_Click(null, null);
                    modifyEmailPayrollToolStripMenuItem_Click('H');
                }
            }
        }

        private void sendSalary_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("You are about to export salary timesheet data to JDE." + Environment.NewLine + "Would you like to proceed?", "Export to JDE", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                int batchID = exportToJDE("S");

                if (batchID != -1)
                {
                    Core.logHistory("Exported salary batch", "Batch ID " + batchID, "");
                    modifyLockoutToolStripMenuItem_Click(null, null);
                    modifyEmailPayrollToolStripMenuItem_Click('S');
                }
            }
        }

        private void undoSendToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Note, this only helps for records not yet processed in JDE.");

            FindBatch fb = new FindBatch();
            fb.ShowDialog();
            string id = fb.selectedID;

            if (id != "")
            {
                Oracle.Run("delete from " + Core.getSchema(Core.getEnvironment()) + ".F06116Z1");

                SQL sql = new SQL("update Timesheets set batchID=0, exported=0 where batchID=@BATCHID");
                sql.AddParameter("@BATCHID", id);
                sql.Run();

                Core.logHistory("Undid batch", "Batch ID " + id, "");
                MessageBox.Show("Batch has been undone to a pre-export state.");
            }
        }

        private void manageAccountsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new AccountsAll()).ShowDialog();
            loadUsers();
        }

        private void ManagePayTypeMenuItem_click(object sender, EventArgs e)
        {
            (new ManagePayTypes()).ShowDialog();
            
        }

        private void manageWorkOrdersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetDepartment gd = new GetDepartment();
            gd.ShowDialog();

            if (!gd.quit)
                (new ManageWorkOrders(gd.deptID)).ShowDialog();
        }

        private void loginHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SQL sql;

            if (Core.isAdmin(Core.getUsername()))
            {
                sql = new SQL(@"select h.Timestamp, h.Username, h.Event, h.datafrom as 'Data From', h.datato as 'Data To'
from history h
where timestamp > @CUTOFF
order by timestamp desc");
            }
            else
            {
                sql = new SQL(@"
select h.Timestamp, h.Username, h.Event, h.datafrom as 'Data From', h.datato as 'Data To'
from history h
where timestamp > @CUTOFF
and h.username in

(select u.username
from DepartmentAssociations da, users u
where da.userid = u.USERID
and da.DepartmentID in 

(select departmentid
from departmentassociations da, users u
where da.userid = u.userid
and u.displayname = @USERNAME)

group by u.username)

order by timestamp desc");
            }
            sql.AddParameter("@CUTOFF", DateTime.Now.AddDays(-2));
            sql.AddParameter("@USERNAME", Core.getUsername());
            DataTable dt = sql.Run();

            Report r = new Report("View History", dt);
            r.Show();
        }

        private void printTimesheetsToolStripMenuItem_Click(object sender, EventArgs e2)
        {
            string section = Core.lookupMessageBox("Employee section", "Which section of employees would you like to print?", "Parks && Facilities", "Operations", "395 Mulock");

            if (section == "")
                return;

            string empType = Core.lookupMessageBox("Employee type filter", "This report is filtered by employee type." + Environment.NewLine
                + "Would you like to search for hourly or salary?", "Hourly", "Salary");

            if (empType == "")
                return;

            empType = empType.Substring(0, 1);

            string searchByTime = Core.lookupMessageBox("Time filter", "This report is filtered by time." + Environment.NewLine
                + "Would you like to search by pay period or date range?", "Period", "Date range");

            if (searchByTime == "")
                return;

            string period = "";
            DateTime start = new DateTime(1900, 1, 1);
            DateTime end = new DateTime(1900, 1, 1);

            if (searchByTime == "Period")
            {
                FindPeriod fp = new FindPeriod(empType, false);
                fp.ShowDialog();
                period = fp.period;

                if (period == "")
                    return;
            }
            else if (searchByTime == "Date range")
            {
                FindDate fd = new FindDate("From date");
                fd.ShowDialog();
                start = fd.date;

                if (start.Year == 1900)
                    return;

                fd = new FindDate("To date");
                fd.ShowDialog();
                end = fd.date;

                if (end.Year == 1900)
                    return;
            }
            
            int division = -1;

            if (section == "Parks && Facilities")
                division = 2;
            else if (section == "Operations")
                division = 1;
            else if (section == "395 Mulock")
                division = 3;

            PayrollExport.updateEmployeeTypes();

            DataTable employees = SQL.Run(@"
select u.employeeid from users u
join departmentassociations da on u.userid = da.userid
join department d on da.departmentid = d.departmentid
where d.division = @DIV
and u.active = 1 and u.enterstime=1 and u.emptype = '" + empType + @"'
group by u.username, u.employeeid", division);

            string empIDSpread = "";

            for (int i = 0; i < employees.Rows.Count; i++)
                empIDSpread += (i == 0 ? "" : ", ") + employees.Rows[i]["employeeid"].ToString();

            employees = Oracle.Run("select YAALPH as username, YAAN8 as employeeid from " + Core.getSchema(Core.getEnvironment()) + ".F060116 where YAAN8 in (" + empIDSpread + ") order by YAALPH");

            Printer queue = new Printer();

            SQL sql;

            for (int e = 0; e < employees.Rows.Count; e++)
            {
                int empID = int.Parse(employees.Rows[e]["employeeid"].ToString());
                string type = Core.isSalary(empID) ? "S" : "H";

                sql = new SQL(@"
select dateworked, hours, p.description, workorder, lumpsum, recordlocked
from Timesheets t
join paycodes p on t.paytype = p.paytype
where t.employeeid = @EMPID
and period=@PERIOD and dateworked < @DATE
order by t.dateworked asc");
                sql.AddParameter("@EMPID", empID);
                sql.AddParameter("@PERIOD", period);
                sql.AddParameter("@DATE", Core.getPeriodStart(int.Parse(period), type).AddDays(7));
                DataTable week1 = sql.Run();

                sql = new SQL(@"
select dateworked, hours, p.description, workorder, lumpsum, recordlocked
from Timesheets t
join paycodes p on t.paytype = p.paytype
where t.employeeid = @EMPID
and period=@PERIOD and dateworked >= @DATE
order by t.dateworked asc");
                sql.AddParameter("@EMPID", empID);
                sql.AddParameter("@PERIOD", period);
                sql.AddParameter("@DATE", Core.getPeriodStart(int.Parse(period), type).AddDays(7));
                DataTable week2 = sql.Run();

                sql = new SQL(@"
select p.description, sum(hours) as Hours
from Timesheets t
join paycodes p on t.paytype = p.paytype
where t.employeeid = @EMPID
and period=@PERIOD and dateworked < @DATE
group by p.description, p.paytype
order by p.paytype");
                sql.AddParameter("@EMPID", empID);
                sql.AddParameter("@PERIOD", period);
                sql.AddParameter("@DATE", Core.getPeriodStart(int.Parse(period), type).AddDays(7));
                DataTable week1PayTypes = sql.Run();

                sql = new SQL(@"
select p.description, sum(hours) as Hours
from Timesheets t
join paycodes p on t.paytype = p.paytype
where t.employeeid = @EMPID
and period=@PERIOD and dateworked >= @DATE
group by p.description, p.paytype
order by p.paytype");
                sql.AddParameter("@EMPID", empID);
                sql.AddParameter("@PERIOD", period);
                sql.AddParameter("@DATE", Core.getPeriodStart(int.Parse(period), type).AddDays(7));
                DataTable week2PayTypes = sql.Run();

                sql = new SQL(@"
select dateworked, hours, p.description, workorder, lumpsum, recordlocked
from Timesheets t
join paycodes p on t.paytype = p.paytype
join users u on t.employeeid = u.employeeid
where t.employeeid = @EMPID
and period<@PERIOD and exported=0
and t.paytype<>0
and not (u.emptype = 'S' and t.paytype = 811)
order by t.dateworked asc");
                sql.AddParameter("@EMPID", empID);
                sql.AddParameter("@PERIOD", period);
                DataTable preperiod = sql.Run();

                queue.AddBold("Employee Timesheet");
                queue.Add("From " + Core.getPeriodStart(int.Parse(period), type).ToString("d") + " to " + Core.getPeriodEnd(int.Parse(period), type).ToString("d"), 350);
                queue.Add("Period:", 640);
                queue.Add(period, 740);
                queue.AddLine();
                queue.Add("Employee:");
                queue.Add(Oracle.Run("select YAALPH from " + Core.getSchema(Core.getEnvironment()) + ".F060116 where YAAN8=" + employees.Rows[e]["employeeid"].ToString()).Rows[0]["YAALPH"].ToString(), 100);
                queue.Add("Employee ID:", 640);
                queue.Add(employees.Rows[e]["employeeid"].ToString(), 740);
                queue.AddLine();
                queue.Add("Printed:");
                queue.Add(DateTime.Now.ToString(), 100);
                queue.Add("Department:", 640);
                queue.Add(SQL.Run("select da.departmentid from users u join departmentassociations da on u.userid = da.userid where u.employeeid=" + employees.Rows[e]["employeeid"].ToString()).Rows.Count > 1 ? "Multiple" : SQL.Run("select d.department from department d join departmentassociations da on d.departmentid = da.departmentid join users u on u.userid = da.userid where u.employeeid = " + employees.Rows[e]["employeeid"].ToString()).Rows[0][0].ToString(), 740);
                queue.AddLine();
                queue.AddDivider();

                queue.AddBold("Week 1");
                queue.AddLine();
                queue.AddBold("Date Worked", 10);
                queue.AddBold("Hours", 118);
                queue.AddBold("Pay Type", 178);
                queue.AddBold("Work Order", 283);
                queue.AddBold("Description", 353);
                queue.AddBold("Lump Sum", 525);
                queue.AddBold("Approved", 615);
                queue.AddLine();

                double week1Sub = 0;
                double lumpSumTotal = 0;

                for (int i = 0; i < week1.Rows.Count; i++)
                {
                    week1Sub += double.Parse(week1.Rows[i]["hours"].ToString());
                    lumpSumTotal += double.Parse(week1.Rows[i]["lumpsum"].ToString());

                    queue.Add(DateTime.Parse(week1.Rows[i]["dateworked"].ToString()).ToString("yyyy-MM-dd ddd"), 10);
                    queue.Add(week1.Rows[i]["hours"].ToString(), 118);
                    queue.Add(week1.Rows[i]["description"].ToString(), 178);
                    queue.Add(week1.Rows[i]["workorder"].ToString(), 283);
                    queue.Add(week1.Rows[i]["workorder"].ToString() != "" ? Oracle.Run("select wadl01 from " + Core.getSchema(Core.getEnvironment()) + ".f4801 where wadoco=" + week1.Rows[i]["workorder"].ToString()).Rows[0][0].ToString() : "", 353);
                    queue.Add(((int)double.Parse(week1.Rows[i]["lumpsum"].ToString())) == 0 ? "" : "$" + ((int)double.Parse(week1.Rows[i]["lumpsum"].ToString())), 525);
                    queue.Add(week1.Rows[i]["recordlocked"].ToString() == "True" ? "Yes" : "No", 615);
                    queue.AddLine();
                }
                double week1LumpSumTotal = lumpSumTotal;

                queue.AddBold("Week 1 sub-total", 10);
                queue.AddBold(week1Sub.ToString(), 118);
                queue.AddBold(string.Format("${0:N2}", lumpSumTotal), 525);
                queue.AddLine();
                queue.AddDivider();

                queue.AddBold("Week 2");
                queue.AddLine();
                queue.AddBold("Date Worked", 10);
                queue.AddBold("Hours", 118);
                queue.AddBold("Pay Type", 178);
                queue.AddBold("Work Order", 283);
                queue.AddBold("Description", 353);
                queue.AddBold("Lump Sum", 525);
                queue.AddBold("Approved", 615);
                queue.AddLine();

                double week2Sub = 0;

                for (int i = 0; i < week2.Rows.Count; i++)
                {
                    week2Sub += double.Parse(week2.Rows[i]["hours"].ToString());
                    lumpSumTotal += double.Parse(week2.Rows[i]["lumpsum"].ToString());

                    queue.Add(DateTime.Parse(week2.Rows[i]["dateworked"].ToString()).ToString("yyyy-MM-dd ddd"), 10);
                    queue.Add(week2.Rows[i]["hours"].ToString(), 118);
                    queue.Add(week2.Rows[i]["description"].ToString(), 178);
                    queue.Add(week2.Rows[i]["workorder"].ToString(), 283);
                    queue.Add(week2.Rows[i]["workorder"].ToString() != "" ? Oracle.Run("select wadl01 from " + Core.getSchema(Core.getEnvironment()) + ".f4801 where wadoco=" + week2.Rows[i]["workorder"].ToString()).Rows[0][0].ToString() : "", 353);
                    queue.Add(((int)double.Parse(week2.Rows[i]["lumpsum"].ToString())) == 0 ? "" : "$" + ((int)double.Parse(week2.Rows[i]["lumpsum"].ToString())), 525);
                    queue.Add(week2.Rows[i]["recordlocked"].ToString() == "True" ? "Yes" : "No", 615);
                    queue.AddLine();
                }

                queue.AddBold("Week 2 sub-total", 10);
                queue.AddBold(week2Sub.ToString(), 118);
                queue.AddBold(string.Format("${0:N2}", lumpSumTotal - week1LumpSumTotal), 525);
                queue.AddLine();
                queue.AddDivider();

                queue.AddBold(string.Format("${0:N2}", lumpSumTotal), 525);
                queue.AddLine();

                for (int i = 0; i < week1PayTypes.Rows.Count; i++)
                {
                    queue.Add("Week 1 " + week1PayTypes.Rows[i]["description"].ToString());
                    queue.Add(week1PayTypes.Rows[i]["hours"].ToString(), 150);
                    queue.AddLine();
                }

                queue.Add("    Week 1 sub-total");
                queue.AddBold(week1Sub.ToString(), 150);
                queue.AddLine();
                queue.AddLine();

                for (int i = 0; i < week2PayTypes.Rows.Count; i++)
                {
                    queue.Add("Week 2 " + week2PayTypes.Rows[i]["description"].ToString());
                    queue.Add(week2PayTypes.Rows[i]["hours"].ToString(), 150);
                    queue.AddLine();
                }

                queue.Add("    Week 2 sub-total");
                queue.AddBold(week2Sub.ToString(), 150);
                queue.AddLine();
                queue.AddLine();

                queue.Add("    Period " + period + " total");
                queue.AddBold((week1Sub + week2Sub).ToString(), 150);
                queue.AddLine();
                queue.AddDivider();

                if (preperiod.Rows.Count > 0)
                {
                    queue.AddLine();
                    queue.AddBold("Un-Exported Hours form Previous Employee Time Sheets");
                    queue.AddLine();
                    queue.AddLine();

                    lumpSumTotal = 0;
                    double preperiodSub = 0;

                    for (int i = 0; i < preperiod.Rows.Count; i++)
                    {
                        preperiodSub += double.Parse(preperiod.Rows[i]["hours"].ToString());
                        lumpSumTotal += double.Parse(preperiod.Rows[i]["lumpsum"].ToString());

                        queue.Add(DateTime.Parse(preperiod.Rows[i]["dateworked"].ToString()).ToString("yyyy-MM-dd ddd"), 10);
                        queue.Add(preperiod.Rows[i]["hours"].ToString(), 118);
                        queue.Add(preperiod.Rows[i]["description"].ToString(), 178);
                        queue.Add(preperiod.Rows[i]["workorder"].ToString(), 283);
                        queue.Add(preperiod.Rows[i]["workorder"].ToString() != "" ? Oracle.Run("select wadl01 from " + Core.getSchema(Core.getEnvironment()) + ".f4801 where wadoco=" + preperiod.Rows[i]["workorder"].ToString()).Rows[0][0].ToString() : "", 353);
                        queue.Add(((int)double.Parse(preperiod.Rows[i]["lumpsum"].ToString())) == 0 ? "" : "$" + ((int)double.Parse(preperiod.Rows[i]["lumpsum"].ToString())), 525);
                        queue.Add(preperiod.Rows[i]["recordlocked"].ToString() == "True" ? "Yes" : "No", 615);
                        queue.AddLine();
                    }

                    queue.AddBold(preperiodSub.ToString(), 118);
                    queue.AddBold(string.Format("${0:N2}", lumpSumTotal), 525);
                    queue.AddLine();
                    queue.AddLine();
                }

                queue.AddPage();
            }

            queue.Print();

            if(empType == "H")
                modifyEmailPayrollToolStripMenuItem_Click('H');
            else
                modifyEmailPayrollToolStripMenuItem_Click('S');
        }

        private void exceptionReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int rowNumber = 1;

            DataTable dt = new DataTable();
            dt.Columns.Add("Exception Number");
            dt.Columns.Add("Details");

            DateTime[,] dates = new DateTime[2, 2]; //Week 1/2, start/end
            dates[0, 0] = Core.getStartingMonday(getEmpType());
            dates[0, 1] = dates[0, 0].AddDays(6);
            dates[1, 0] = dates[0, 0].AddDays(7);
            dates[1, 1] = dates[0, 0].AddDays(13);

            for (int w = 0; w < 2; w++) //For each week
            {
                SQL sql = new SQL("select u.displayname, sum(t.hours) from users u, Timesheets t where u.employeeid = t.employeeid and t.dateworked >= @START and t.dateworked <= @END and paytype = 1 group by u.displayname");
                sql.AddParameter("@START", dates[w, 0]);
                sql.AddParameter("@END", dates[w, 1].AddDays(1));
                DataTable regular = sql.Run();

                for (int i = 0; i < regular.Rows.Count; i++) //For each employee
                {
                    if (Core.isFullTime(regular.Rows[i][0].ToString())) //If full time
                    {
                        if (double.Parse(regular.Rows[i][1].ToString()) < 40) //If less than 40 regular hours
                        {
                            sql = new SQL("select u.displayname, sum(t.hours) from users u, Timesheets t where u.employeeid = t.employeeid and t.dateworked >= @START and t.dateworked <= @END and paytype = 105 and u.displayname = @USERNAME group by u.displayname");
                            sql.AddParameter("@START", dates[w, 0]);
                            sql.AddParameter("@END", dates[w, 1].AddDays(1));
                            sql.AddParameter("@USERNAME", regular.Rows[i][0].ToString());
                            DataTable overtime = sql.Run();

                            if (overtime.Rows.Count > 0)
                            {
                                if (double.Parse(overtime.Rows[0][1].ToString()) > 0)
                                {
                                    object[] r = { rowNumber++, "From " + dates[w, 0].ToString("MMMM d") + " to " + dates[w, 1].ToString("MMMM d") + ", " + regular.Rows[i][0].ToString() + " has " + overtime.Rows[0][1].ToString() + " hours of overtime despite only having " + regular.Rows[i][1].ToString() + " hours of regular." };
                                    dt.Rows.Add(r);
                                }
                            }
                        }
                    }

                    if (Core.isPartTime(regular.Rows[i][0].ToString())) //If part time
                    {
                        if (double.Parse(regular.Rows[i][1].ToString()) < 44) //If less than 44 regular hours
                        {
                            sql = new SQL("select u.displayname, sum(t.hours) from users u, Timesheets t where u.employeeid = t.employeeid and t.dateworked >= @START and t.dateworked <= @END and paytype = 105 and u.displayname = @USERNAME group by u.displayname");
                            sql.AddParameter("@START", dates[w, 0]);
                            sql.AddParameter("@END", dates[w, 1].AddDays(1));
                            sql.AddParameter("@USERNAME", regular.Rows[i][0].ToString());
                            DataTable overtime = sql.Run();

                            if (overtime.Rows.Count > 0)
                            {
                                if (double.Parse(overtime.Rows[0][1].ToString()) > 0)
                                {
                                    object[] r = { rowNumber++, "From " + dates[w, 0].ToString("MMMM d") + " to " + dates[w, 1].ToString("MMMM d") + ", " + regular.Rows[i][0].ToString() + " has " + overtime.Rows[0][1].ToString() + " hours of overtime despite only having " + regular.Rows[i][1].ToString() + " hours of regular." };
                                    dt.Rows.Add(r);
                                }
                            }
                        }
                    }
                }
            }

            SQL sql2 = new SQL("select u.displayname, t.dateworked from users u, Timesheets t where u.employeeid = t.employeeid and t.dateworked >= @START and t.dateworked <= @END and paytype = 905 group by u.displayname, t.dateworked");
            sql2.AddParameter("@START", dates[0, 0]);
            sql2.AddParameter("@END", dates[1, 1].AddDays(1));
            DataTable illness = sql2.Run();

            for (int i = 0; i < illness.Rows.Count; i++)
            {
                if (Core.isPartTime(illness.Rows[i][0].ToString()))
                {
                    object[] r = { rowNumber++, "On " + DateTime.Parse(illness.Rows[i][1].ToString()).ToString("MMMM d") + ", " + illness.Rows[i][0].ToString() + " used the Illness (H) paycode despite being part-time." };
                    dt.Rows.Add(r);
                }
            }

            sql2 = new SQL("select u.displayname, count(*), t.dateworked from users u, Timesheets t where u.employeeid = t.employeeid and t.dateworked >= @START and t.dateworked <= @END and (paytype = 2 or paytype = 4) group by u.displayname, t.dateworked");
            sql2.AddParameter("@START", dates[0, 0]);
            sql2.AddParameter("@END", dates[1, 1].AddDays(1));
            DataTable standby = sql2.Run();

            for (int i = 0; i < standby.Rows.Count; i++)
            {
                if (int.Parse(standby.Rows[i][1].ToString()) > 1)
                {
                    object[] r = { rowNumber++, standby.Rows[i][0].ToString() + " used the Standby paycode " + standby.Rows[i][1].ToString() + " times on " + DateTime.Parse(standby.Rows[i][2].ToString()).ToString("MMMM d") + "." };
                    dt.Rows.Add(r);
                }
            }

            sql2 = new SQL("select u.displayname, t.dateworked from users u, Timesheets t where u.employeeid = t.employeeid and t.dateworked >= @START and t.dateworked <= @END and paytype = 310 group by u.displayname, t.dateworked");
            sql2.AddParameter("@START", dates[0, 0]);
            sql2.AddParameter("@END", dates[1, 1].AddDays(1));
            DataTable authorizedAbsence = sql2.Run();

            for (int i = 0; i < authorizedAbsence.Rows.Count; i++)
            {
                if (Core.isPartTime(authorizedAbsence.Rows[i][0].ToString()))
                {
                    object[] r = { rowNumber++, "On " + DateTime.Parse(authorizedAbsence.Rows[i][1].ToString()).ToString("MMMM d") + ", " + authorizedAbsence.Rows[i][0].ToString() + " used the Authorized Absence paycode despite being part time." };
                    dt.Rows.Add(r);
                }
            }

            Report rpt = new Report("Exception Report", dt);
            rpt.Show();
        }

        private void resetROEsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FindEmpID fei = new FindEmpID();
            fei.ShowDialog();

            if (fei.value != "")
            {
                int empID = -1;

                int.TryParse(fei.value, out empID);

                if (empID <= 0)
                {
                    MessageBox.Show("Employee ID not understood. Unable to parse.");
                    return;
                }

                Oracle ora = new Oracle("select JFPCT# from " + Core.getSchema(Core.getEnvironment()) + ".F0717 where JFAN8=@EMPID order by JFPCT# desc");
                ora.AddParameter("@EMPID", empID);
                DataTable dt = ora.Run();

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No ROE records found for " + empID + ".");
                    return;
                }

                string PCT = dt.Rows[0]["JFPCT#"].ToString();

                string name = Oracle.Run("select YAALPH from " + Core.getSchema(Core.getEnvironment()) + ".F060116 where YAAN8=" + empID).Rows[0]["YAALPH"].ToString().Trim();

                if (Core.lookupMessageBox("Are you sure?", empID + " is " + name + ". Is this correct?", "Yes", "No") == "No")
                {
                    return;
                }

                //if(dt.Rows[0]["JFPCT#"].ToString().Trim() == "0")
                //{
                //    MessageBox.Show("ROE record found, however not needing reset.");
                //    return;
                //}

                ora = new Oracle("update " + Core.getSchema(Core.getEnvironment()) + ".F0717 set JFROEN=@ROEN, JFPCT#=0 where JFPCT#=@PCT");
                ora.AddParameter("@ROEN", "           ");
                ora.AddParameter("@PCT", PCT);
                ora.Run();

                ora = new Oracle("update " + Core.getSchema(Core.getEnvironment()) + ".F06176 set YSDTM=0, YSPCT#=0, YSROEN=@ROEN where YSPCT#=@PCT");
                ora.AddParameter("@ROEN", "           ");
                ora.AddParameter("@PCT", PCT);
                ora.Run();

                MessageBox.Show("ROE has been reset!");
            }
        }

        private void modifyLockoutToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            string empType = "H";
            //eac
            string word = empType == "H" ? "hourly" : "salary";
            string message;
            int currentPeriod = Core.getCurrentPeriod(empType);
            DateTime cutoff = Core.getPeriodEnd(currentPeriod, empType);

            if (empType == "S")
                cutoff = cutoff.AddDays(-14);

            if (Core.getVariable("Lockout" + empType) == "True")
                message = "The " + word + " payroll is currently locked." + Environment.NewLine + "Changes to " + word + " timesheets on " + cutoff.ToString("MMMM dd, yyyy") + " and prior are being prevented.";
            else
                message = "The " + word + " payroll is currently open." + Environment.NewLine + "When locked, changes to " + word + " timesheets on " + cutoff.ToString("MMMM dd, yyyy") + " and prior will be prevented.";

            string result = Core.lookupMessageBox("Change " + word + " lockout", message,
                (Core.getVariable("Lockout" + empType) == "True" ? "Unlock it" : "Lock it"),
                "Keep it as is");

            if (result == "Unlock it")
            {
                Core.logHistory("Variable changed", "Lockout" + empType + " = False", "");
                SQL.Run("update systemvariables set value='False' where variable='Lockout" + empType + "'");
                MessageBox.Show("The system has been unlocked.");
            }
            else if (result == "Lock it")
            {
                Core.logHistory("Variable changed", "Lockout" + empType + " = True", "");
                SQL.Run("update systemvariables set value='True' where variable='Lockout" + empType + "'");
                MessageBox.Show("The system has been locked.");
            }
        }

        private void modifyEmailPayrollHourly(object sender, EventArgs e)
        {
            string empType = "H";
            string word = empType == "H" ? "hourly" : "salary";

            string result = Core.lookupMessageBox("E-mail Payroll about changes to the " + word + " payroll", "The system is currently " + (Core.getVariable("EmailPayroll" + empType) == "True" ? "e-mailing updates to Payroll." : "not e-mailing updates to Payroll"),
                (Core.getVariable("EmailPayroll" + empType) == "True" ? "Stop sending e-mails" : "Send e-mails"),
                "Keep it as is");

            if (result == "Stop sending e-mails")
            {
                Core.logHistory("Variable changed", "EmailPayroll" + empType + " = False", "");
                SQL.Run("update systemvariables set value='False' where variable='EmailPayroll" + empType + "'");
                MessageBox.Show("Payroll will no longer receive update e-mails.");
            }
            else if (result == "Send e-mails")
            {
                Core.logHistory("Variable changed", "EmailPayroll" + empType + " = True", "");
                SQL.Run("update systemvariables set value='True' where variable='EmailPayroll" + empType + "'");
                MessageBox.Show("Payroll will receive update e-mails.");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (toolStripComboBox3.SelectedIndex > 0)
                toolStripComboBox3.SelectedIndex--;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (toolStripComboBox3.SelectedIndex < toolStripComboBox3.Items.Count - 1)
                toolStripComboBox3.SelectedIndex++;
        }

        private void vacationRemainingToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void bankedTimeRemainingToolStripMenuItem_Click(object sender, EventArgs e)
        {
  
            int year = DateTime.Today.Year;
            DateTime start = new DateTime(year, 1, 1);
            DateTime end = start.AddYears(1);
            int empID = Core.getEmpID(getUsername());

            double bankedTimeIn = Core.getBankedTimeIn(start, end, empID);
            double bankedTimeUsed = Core.getBankedTimeUsed(start, end, empID);
            double bankedTimeBalance = Core.getBankedTimeBalance(start, end, empID);

            double vacationPriorMax = Core.getVacationMax(year - 1, empID);
            double vacationPriorUsed = Core.getVacationUsed(year - 1, empID);
            double vacationPriorBalance = Core.getVacationBalance(year - 1, empID, true);

            double vacationMax = Core.getVacationMax(year, empID);
            double vacationUsed = Core.getVacationUsed(year, empID);
            double vacationBalance = Core.getVacationBalance(year, empID, false);

            double bankedVacationMax = Core.getBankedVacationMax(empID);
            double bankedVacationUsed = Core.getBankedVacationUsed(empID);
            double bankedVacationBalance = Core.getBankedVacationBalance(empID);

            double floaterMax = Core.getFloaterMax(empID);
            double floaterUsed = Core.getFloaterUsed(start, end, empID);
            double floaterBalance = Core.getFloaterBalance(start, end, empID);

            double statMax = Core.getStatMax(start, end, empID);
            double statUsed = Core.getStatUsed(start, end, empID);
            double statBalance = Core.getStatBalance(start, end, empID);

            // MCL Bank
            double MCLMax = Core.getMCLMax(year, empID);
            double MCLUsed = Core.getMCLUsed(year, empID);
            double MCLBalance = Core.getMCLBalance(year, empID, false);


            //Report
            DataTable dt = new DataTable();
            dt.Columns.Add("Category");
            dt.Columns.Add("Entitlement");
            dt.Columns.Add("Used");
            dt.Columns.Add("Balance");

            DataTable payTypes = SQL.Run(@"select p.paytype, p.description
from Timesheets t
join paycodes p on t.paytype = p.paytype
join users u on t.employeeid = u.employeeid

where dateworked >= @START
and dateworked < @END
and displayname = @USERNAME

group by p.paytype, p.description
order by p.paytype", start, end, getUsername());

            dt.Rows.Add(new object[] { "Banked Time", bankedTimeIn, bankedTimeUsed, bankedTimeBalance });

 //           if (DateTime.Today.Month < 4)  Soleil
                dt.Rows.Add(new object[] { (start.Year - 1) + " Vacation", vacationPriorMax, vacationPriorUsed, vacationPriorBalance });
            
            dt.Rows.Add(new object[] { start.Year + " Vacation", vacationMax, vacationUsed, vacationBalance });

            if (!Core.isFacilities(getUsername()) && !Core.isFacilityMaintenance(getUsername()))
                dt.Rows.Add(new object[] { "Banked Vacation", bankedVacationMax, bankedVacationUsed, bankedVacationBalance });

            if (MCLMax > 0)
                dt.Rows.Add(new object[] { start.Year + " MCL Vacation", MCLMax, MCLUsed, MCLBalance });


            dt.Rows.Add(new object[] { "Floater", floaterMax, floaterUsed, floaterBalance });

            if (Core.isFacilities(getUsername()) || Core.isFacilityMaintenance(getUsername()))
                dt.Rows.Add(new object[] { "Statutory Holiday", statMax, statUsed, statBalance });

            dt.Rows.Add(new object[] { "", "", "", "" });
            dt.Rows.Add(new object[] { "-----------------------", "----------", "----------", "----------" });
            dt.Rows.Add(new object[] { "", "", "", "" });

            for (int i = 0; i < payTypes.Rows.Count; i++)
                dt.Rows.Add(new object[] { payTypes.Rows[i][1].ToString(), "", gev(int.Parse(payTypes.Rows[i][0].ToString())), "" });

            Report r = new Report(start.Year + " Summary for " + getUsername(), dt);
            r.Show();
        }

        private double gev(int paytype)
        {
            SQL sql = new SQL("select sum(t.hours) from Timesheets t join users u on t.employeeid = u.employeeid where u.displayname=@NAME and t.paytype=@PAY and t.dateworked >= @START and t.dateworked < @END");
            sql.AddParameter("@NAME", getUsername());
            sql.AddParameter("@PAY", paytype);
            sql.AddParameter("@START", new DateTime(DateTime.Now.Year, 1, 1));
            sql.AddParameter("@END", new DateTime(DateTime.Now.Year + 1, 1, 1));

            double k = 0;
            double.TryParse(sql.Run().Rows[0][0].ToString(), out k);
            
            return k;
        }

        private double gev(int paytype, string name)
        {
            SQL sql = new SQL("select sum(t.hours) from Timesheets t join users u on t.employeeid = u.employeeid where u.displayname=@NAME and t.paytype=@PAY and t.dateworked >= @START and t.dateworked < @END");
            sql.AddParameter("@NAME", name);
            sql.AddParameter("@PAY", paytype);
            sql.AddParameter("@START", new DateTime(DateTime.Now.Year, 1, 1));
            sql.AddParameter("@END", new DateTime(DateTime.Now.Year + 1, 1, 1));

            double k = 0;
            double.TryParse(sql.Run().Rows[0][0].ToString(), out k);

            return k;
        }

        private void departmentEntitlementSummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Aqui me quede
            string departmentSpread = "";
            SQL sql;
            DataTable dt;

            if (!Core.isAdmin(Core.getUsername()))
                dt = SQL.Run("select departmentid from departmentassociations da join users u on da.userid = u.userid where u.displayname=@USERNAME", Core.getUsername());
            else
                dt = SQL.Run("select departmentid from department");

            for (int i = 0; i < dt.Rows.Count; i++)
                departmentSpread += (i == 0 ? "" : ", ") + dt.Rows[i][0].ToString();

            sql = new SQL(@"SELECT u.DISPLAYNAME, u.EMPLOYEEID, min(d.department) as department
  FROM [DATS].[dbo].Users u
  join DepartmentAssociations da on da.UserID = u.USERID
  join Department d on d.DepartmentID = da.DepartmentID
  where d.departmentid in (" + departmentSpread + @")
  and u.active=1
  and u.enterstime=1
  group by u.displayname, u.employeeid
  order by u.displayname");
            DataTable users = sql.Run();

            int year = DateTime.Today.Year;

            DataTable result = new DataTable();
            result.Columns.Add("Name");
            result.Columns.Add("Department");
            result.Columns.Add("Employment");
            result.Columns.Add("Bank Hours Remaining");

 //          if (DateTime.Today.Month < 4)Soleil
            
                result.Columns.Add((year - 1) + " Vacation Entitlement");
                result.Columns.Add((year - 1) + " Vacation Balance");
            
            // MCL Code added on Aug 2020
            result.Columns.Add(year + " Vacation Entitlement");
            result.Columns.Add(year + " Vacation Balance");
            result.Columns.Add("Banked Vacation Entitlement");
            result.Columns.Add("Banked Vacation Balance");
            result.Columns.Add("MCL Vacation Balance");
            result.Columns.Add("Floaters Remaining");
            result.Columns.Add("Stats Remaining (Facilities)");

            for (int u = 0; u < users.Rows.Count; u++)
            {
                DateTime start = new DateTime(year, 1, 1);
                DateTime end = start.AddYears(1);
                int empID = int.Parse(users.Rows[u]["employeeid"].ToString());

                double bankedTimeIn = Core.getBankedTimeIn(start, end, empID);
                double bankedTimeUsed = Core.getBankedTimeUsed(start, end, empID);
                double bankedTimeBalance = Core.getBankedTimeBalance(start, end, empID);

                double vacationPriorMax = Core.getVacationMax(year - 1, empID);
                double vacationPriorUsed = Core.getVacationUsed(year - 1, empID);
                double vacationPriorBalance = Core.getVacationBalance(year - 1, empID, true);

                double vacationMax = Core.getVacationMax(year, empID);
                double vacationUsed = Core.getVacationUsed(year, empID);
                double vacationBalance = Core.getVacationBalance(year, empID, false);

                double bankedVacationMax = Core.getBankedVacationMax(empID);
                double bankedVacationUsed = Core.getBankedVacationUsed(empID);
                double bankedVacationBalance = Core.getBankedVacationBalance(empID);

                // MCL Bank
                double MCLMax = Core.getMCLMax(year, empID);
                double MCLUsed = Core.getMCLUsed(year, empID);
                double MCLBalance = Core.getMCLBalance(year, empID, false);

                double floaterMax = Core.getFloaterMax(empID);
                double floaterUsed = Core.getFloaterUsed(start, end, empID);
                double floaterBalance = Core.getFloaterBalance(start, end, empID);

                double statMax = Core.getStatMax(start, end, empID);
                double statUsed = Core.getStatUsed(start, end, empID);
                double statBalance = Core.getStatBalance(start, end, empID);
                string statLabel = "";

                if (Core.isFacilities(users.Rows[u]["DISPLAYNAME"].ToString()) || Core.isFacilityMaintenance(users.Rows[u]["DISPLAYNAME"].ToString()))
                    statLabel = statBalance.ToString();

                //   Full - time Regular
                //1  Part - Time Casual
                //2  Part - Time Hourly
                //3  Contract
                //4  Elected Officials
                //5  Full Time Hourly
                //6  Contract Salary
                //7  LTD

                string code = Core.getEmpType(Core.getEmpID(users.Rows[u]["DISPLAYNAME"].ToString()));

                if (code == "")
                    code = "Full-time Regular";
                else if (code == "1")
                    code = "Part-time Casual";
                else if (code == "2")
                    code = "Part-time Hourly";
                else if (code == "3")
                    code = "Contract";
                else if (code == "4")
                    code = "Elected Officials";
                else if (code == "5")
                    code = "Full-time Hourly";
                else if (code == "6")
                    code = "Contract Salary";
                else if (code == "7")
                    code = "LTD";
                else
                    code = "Unknown";

                //Result
                /*
                if (DateTime.Today.Month >= 4)
                result.Rows.Add(new object[] {
                    users.Rows[u]["DISPLAYNAME"].ToString(),
                    users.Rows[u]["DEPARTMENT"].ToString(),
                    code,
                    bankedTimeBalance,
                    vacationMax,
                    vacationBalance,
                    bankedVacationMax,
                    bankedVacationBalance,
                    MCLBalance,
                    floaterBalance,
                    statLabel });
                else */
                    result.Rows.Add(new object[] {
                    users.Rows[u]["DISPLAYNAME"].ToString(),
                    users.Rows[u]["DEPARTMENT"].ToString(),
                    code,
                    bankedTimeBalance,
                    vacationPriorMax,
                    vacationPriorBalance,
                    vacationMax,
                    vacationBalance,
                    bankedVacationMax,
                    bankedVacationBalance,
                    MCLBalance,
                    floaterBalance,
                    statLabel });
            }

        Report r = new Report("Staff Entitlement Summary", result);
            r.Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Visible = !label1.Visible;
        }

        private void workOrderUsageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string searchByTime = Core.lookupMessageBox("Time filter", "This report is filtered by time." + Environment.NewLine
    + "Would you like to search by pay period or date range?", "Period", "Date range");

            if (searchByTime == "")
                return;

            string empType = Core.lookupMessageBox("Employee type filter", "This report is filtered by employee type." + Environment.NewLine
                + "Would you like to search for hourly or salary?", "Hourly", "Salary");

            if (empType == "")
                return;

            empType = empType.Substring(0, 1); //S, H

            string period = "";
            DateTime start = new DateTime(1900, 1, 1);
            DateTime end = new DateTime(1900, 1, 1);

            if (searchByTime == "Period")
            {
                FindPeriod fp = new FindPeriod(empType, false);
                fp.ShowDialog();
                period = fp.period;

                if (period == "")
                    return;
            }
            else if (searchByTime == "Date range")
            {
                FindDate fd = new FindDate("From date");
                fd.ShowDialog();
                start = fd.date;

                if (start.Year == 1900)
                    return;

                fd = new FindDate("To date");
                fd.ShowDialog();
                end = fd.date;

                if (end.Year == 1900)
                    return;
            }

            string departmentSpread = "";
            DataTable dt = null;

            if (!Core.isAdmin(Core.getUsername()))
                dt = SQL.Run("select departmentid from departmentassociations da join users u on da.userid = u.userid where u.displayname=@USERNAME", Core.getUsername());
            else
                dt = SQL.Run("select departmentid from department");

            for (int i = 0; i < dt.Rows.Count; i++)
                departmentSpread += (i == 0 ? "" : ", ") + dt.Rows[i][0].ToString();

            MessageBox.Show("On the next screen, search for the work order you would like information on.");

            FindWorkOrder fw = new FindWorkOrder(Core.getUsername());
            fw.ShowDialog();

            if (fw.woID == null)
                return;

            int woid = int.Parse(fw.woID);
            string wodesc = fw.woDesc;

            if(searchByTime == "Period")
                dt = SQL.Run(@"
    select
    TimeCardDetailID as ID,
    CreateUser as 'Created By',
    Period as 'Pay Period',
    u.DISPLAYNAME as Employee,
    DateWorked as 'Date Worked',
    p.Description as 'Pay Type',
    Hours,
    WorkOrder,
    LumpSum as 'Lump Sum',
    DateEntered as 'Date Entered',
    Reviewed as 'Reviewed',
    RecordLocked as 'Approved',
    Exported as 'Exported',
    BatchId as 'Batch ID',
    t.Description as 'Description',
    StartTime as 'Start Time',
    FinishTime as 'Finish Time'

    from Timesheets t
    join PayCodes p on t.PayType = p.PayType
    join Users u on t.EmployeeID = u.EMPLOYEEID

    where t.workorder = @WOID
    and t.period = @PERIOD
    and u.emptype = @EMPTYPE

    order by t.dateworked
    ", woid, period, empType);
        else if(searchByTime == "Date range")
            dt = SQL.Run(@"
    select
    TimeCardDetailID as ID,
    CreateUser as 'Created By',
    Period as 'Pay Period',
    u.DISPLAYNAME as Employee,
    DateWorked as 'Date Worked',
    p.Description as 'Pay Type',
    Hours,
    WorkOrder,
    LumpSum as 'Lump Sum',
    DateEntered as 'Date Entered',
    Reviewed as 'Reviewed',
    RecordLocked as 'Approved',
    Exported as 'Exported',
    BatchId as 'Batch ID',
    t.Description as 'Description',
    StartTime as 'Start Time',
    FinishTime as 'Finish Time'

    from Timesheets t
    join PayCodes p on t.PayType = p.PayType
    join Users u on t.EmployeeID = u.EMPLOYEEID

    where t.workorder = @WOID
    and t.dateworked >= @START
    and t.dateworked <= @END
    and u.emptype = @EMPTYPE

    order by t.dateworked
    ", woid, start, end.AddDays(1), empType);

            Report rpt = new Report("Work Order Usage Report", dt);
            rpt.Show();
        }

        private void goForwardToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exportedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < resultsList.SelectedItems.Count; i++)
            {
                string currentStatus = resultsList.SelectedItems[i].SubItems[resultsList.Columns["Status"].Index].Text;

                SQL sql = new SQL("update Timesheets set exported='True', reviewed='True', recordlocked='True' where timecarddetailid=@ID");
                sql.AddParameter("@ID", resultsList.SelectedItems[i].Tag.ToString());
                sql.Run();

                resultsList.SelectedItems[i].SubItems[resultsList.Columns["Status"].Index].Text = EXPORTED_TEXT;
                resultsList.SelectedItems[i].SubItems[resultsList.Columns["Status"].Index].ForeColor = EXPORTED_COLOR;

                Core.logTimesheetHistory("Timesheet marked as processed", int.Parse(resultsList.SelectedItems[i].Tag.ToString()));
                Core.logHistory("Set timesheet to Processed", "Timesheet ID# " + resultsList.SelectedItems[i].Tag.ToString(), "");
            }

            resultsList.Columns["Status"].Width = -2;
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            string response = Core.lookupMessageBox("Are you sure?", "Deploy new version?", "Deploy", "Cancel");

            if (response == "Deploy")
            {
                File.Copy(@"\\sysgs-11-13\c$\Users\graeme smyth\Documents\Visual Studio 2013\Projects\DATS Timesheets\DATS Timesheets\bin\Debug\DATS.exe", Core.getDeploymentPath(), true);
                Core.backup(@"C:\Users\graeme smyth\Documents\Visual Studio 2013\Projects\DATS Timesheets", @"Z:\DATS\DATS");
            }
        }

        private void lockoutSalaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string empType = "S";

            string word = empType == "H" ? "hourly" : "salary";
            string message;
            int currentPeriod = Core.getCurrentPeriod(empType);
            DateTime cutoff = Core.getPeriodEnd(currentPeriod, empType);

            if (empType == "S")
                cutoff = cutoff.AddDays(-14);

            if (Core.getVariable("Lockout" + empType) == "True")
                message = "The " + word + " payroll is currently locked." + Environment.NewLine + "Changes to " + word + " timesheets on " + cutoff.ToString("MMMM dd, yyyy") + " and prior are being prevented.";
            else
                message = "The " + word + " payroll is currently open." + Environment.NewLine + "When locked, changes to " + word + " timesheets on " + cutoff.ToString("MMMM dd, yyyy") + " and prior will be prevented.";

            string result = Core.lookupMessageBox("Change " + word + " lockout", message,
                (Core.getVariable("Lockout" + empType) == "True" ? "Unlock it" : "Lock it"),
                "Keep it as is");

            if (result == "Unlock it")
            {
                Core.logHistory("Variable changed", "Lockout" + empType + " = False", "");
                SQL.Run("update systemvariables set value='False' where variable='Lockout" + empType + "'");
                MessageBox.Show("The system has been unlocked.");
            }
            else if (result == "Lock it")
            {
                Core.logHistory("Variable changed", "Lockout" + empType + " = True", "");
                SQL.Run("update systemvariables set value='True' where variable='Lockout" + empType + "'");
                MessageBox.Show("The system has been locked.");
            }
        }

        private void modifyEmailPayrollSalary(object sender, EventArgs e)
        {
            string empType = "S";
            string word = empType == "H" ? "hourly" : "salary";

            string result = Core.lookupMessageBox("E-mail Payroll about changes to the " + word + " payroll", "The system is currently " + (Core.getVariable("EmailPayroll" + empType) == "True" ? "e-mailing updates to Payroll." : "not e-mailing updates to Payroll"),
                (Core.getVariable("EmailPayroll" + empType) == "True" ? "Stop sending e-mails" : "Send e-mails"),
                "Keep it as is");

            if (result == "Stop sending e-mails")
            {
                Core.logHistory("Variable changed", "EmailPayroll" + empType + " = False", "");
                SQL.Run("update systemvariables set value='False' where variable='EmailPayroll" + empType + "'");
                MessageBox.Show("Payroll will no longer receive update e-mails.");
            }
            else if (result == "Send e-mails")
            {
                Core.logHistory("Variable changed", "EmailPayroll" + empType + " = True", "");
                SQL.Run("update systemvariables set value='True' where variable='EmailPayroll" + empType + "'");
                MessageBox.Show("Payroll will receive update e-mails.");
            }
        }

        private void allTimesheetsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string departmentSpread = "";
            DataTable dt = null;
            bool personalView = false;

            if (Core.isAdmin(Core.getUsername()))
                dt = SQL.Run("select department from department");
            else if (Core.canReview(Core.getUsername()))
                dt = SQL.Run("select department from departmentassociations da join users u on da.userid = u.userid join department d on da.departmentid = d.departmentid where u.displayname=@USERNAME", Core.getUsername());
            else
                personalView = true;

            for (int i = 0; dt != null && i < dt.Rows.Count; i++)
                departmentSpread += (i == 0 ? "'" : ", '") + dt.Rows[i][0].ToString() + "'";

            if(dt != null && dt.Rows.Count > 1)
                departmentSpread += ", 'Multi'";

            dt = SQL.Run(@"
SELECT [DateEntered] as 'Date Entered'
	  ,u.DISPLAYNAME as 'Employee Name'
	  ,case when (select count(*) from DepartmentAssociations da where da.UserID = u.USERID) > 1 then 'Multi' else (select d.Department from Department d join DepartmentAssociations da on d.DepartmentID = da.DepartmentID join users u2 on da.UserID = u2.USERID and u2.USERID = u.USERID) end as 'Department'
	  ,case when u.EMPTYPE = 'H' then 'Hourly' else case when u.emptype = 'S' then 'Salary' else u.emptype end end as 'Employee Type'
      ,convert(date, [DateWorked]) as 'Date Worked'
	  ,[Period] as 'Pay Period'
      ,p.Description as 'Pay Type'
      ,[Hours]
      ,[StartTime] as 'Start Time'
      ,[FinishTime] as 'Finish Time'
      ,[WorkOrder] as 'Work Order'
      ,[LumpSum] as 'Lump Sum'
      ,case when [Reviewed] <> 0 then 'Yes' else 'No' end as 'Reviewed'
      ,case when [RecordLocked] <> 0 then 'Yes' else 'No' end as 'Approved'
      ,case when [Exported] <> 0 then 'Yes' else 'No' end as 'Processed'
      ,t.[Description]
	  ,(select rtrim(e.EQUIPID) + ', ' as 'data()' from EquipmentTimeEntry e where t.TimeCardDetailID = e.TIMESHEETID FOR XML PATH('')) as 'Equipment'
  FROM [DATS].[dbo].[Timesheets] t
  join users u on t.employeeid = u.EMPLOYEEID
  join PayCodes p on t.PayType = p.PayType

  where "
+ (personalView ?
    "u.displayname = '" + Core.getUsername() + "'"
  :
    "case when (select count(*) from DepartmentAssociations da where da.UserID = u.USERID) > 1 then 'Multi' else (select d.Department from Department d join DepartmentAssociations da on d.DepartmentID = da.DepartmentID join users u2 on da.UserID = u2.USERID and u2.USERID = u.USERID) end in (" + departmentSpread + ")" ) + @"

  order by dateentered
");

            Report r = new Report("All Timesheets Report", dt);
            r.Show();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            bankedTimeRemainingToolStripMenuItem_Click(null, null);
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            allTimesheetsToolStripMenuItem_Click(null, null);
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            paychequesToolStripMenuItem_Click(null, null);
        }

        private void recentChangesReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Report r = new Report("History Versions", SQL.Run(@"
select
HistoryTimestamp as 'Action Timestamp',
performedby as 'Action By',
HistoryDescription as 'Action',
TimeCardDetailID as 'Timesheet ID',
u.DISPLAYNAME as 'Timesheet For',
u.EMPTYPE as 'Employee Type',
dateworked as 'Date Worked',
p.Description as 'Pay Type',
hours as 'Hours'

from HistoryVersions h
join users u on h.EmployeeID = u.EMPLOYEEID
join PayCodes p on h.PayType = p.PayType
where HistoryTimestamp > @DATE
and HistoryDescription <> 'Timesheet reviewed'
and HistoryDescription <> 'Timesheet approved'
and HistoryDescription <> 'Timesheet unapproved'
and HistoryDescription <> 'Timesheet exported'

order by HistoryTimestamp desc
", DateTime.Today.AddMonths(-2)));
            r.Show();
        }

    }
}
