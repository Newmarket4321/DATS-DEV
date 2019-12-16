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
    public partial class Timesheet : Form
    {
        public bool changesMade = false;
        int editingTimesheetID = 0;
        bool forDifferentUser = false;
        string forUser = "";
        string[] equipment;
        double hours = 0;
        string start = "";
        string end = "";
        List<DateTime> holidays = new List<DateTime>()
        {
            new DateTime(2019,9,2),
            new DateTime(2019,10,14),
            new DateTime(2019,12,25),
            new DateTime(2019,12,26),
            new DateTime(2020,1,1)
        };

        

        public Timesheet()
        {

            InitializeComponent();

            forUser = Core.getUsername();

            dateCalendar.SelectionStart = DateTime.Today;
            dateCalendar.SelectionEnd = DateTime.Today;

            startingSetup();
        }

        public Timesheet(DateTime x)
        {
            InitializeComponent();

            forUser = Core.getUsername();

            dateCalendar.SelectionStart = x;
            dateCalendar.SelectionEnd = x;
            dateCalendar.Select();

            startingSetup();
        }

        public Timesheet(string username)
        {
            InitializeComponent();

            forDifferentUser = true;
            forUser = username;

            dateCalendar.SelectionStart = DateTime.Today;
            dateCalendar.SelectionEnd = DateTime.Today;
            
            startingSetup();

            this.Text = "Timesheet for " + username;
            dateCalendar.Select();
        }     

        public Timesheet(string username, DateTime x)
        {
            InitializeComponent();

            forDifferentUser = true;
            forUser = username;
            
            dateCalendar.SelectionStart = x;
            dateCalendar.SelectionEnd = x;
            dateCalendar.Select();

            dateCalendar_DateSelected(null, null);

            startingSetup();

            this.Text = "Timesheet for " + username;
            dateCalendar.Select();
        }
    
        public Timesheet(int timesheetID) //Edit Timesheet
        {
            InitializeComponent();

            editingTimesheetID = timesheetID;
            label8.Visible = false;

            SQL sql = new SQL("select * from Timesheets where timecarddetailid = @ID");
            sql.AddParameter("@ID", timesheetID);
            DataTable dt = sql.Run();
            int empID = int.Parse(dt.Rows[0]["employeeid"].ToString());
            string username = Core.getUsernameFromEmpID(empID);
            forUser = username;

            startingSetup();

            sql = new SQL("select [description] from paycodes where paytype=@PAYTYPE");
            sql.AddParameter("@PAYTYPE", dt.Rows[0]["PayType"].ToString());
            string payType = sql.Run().Rows[0][0].ToString();

            dateCalendar.MaxSelectionCount = 1;
            dateCalendar.SelectionStart = DateTime.Parse(dt.Rows[0]["DateWorked"].ToString());
            dateCalendar.SelectionEnd = DateTime.Parse(dt.Rows[0]["DateWorked"].ToString());
            timeBar.Items.Add(dt.Rows[0]["StartTime"].ToString() + " - " + dt.Rows[0]["FinishTime"].ToString() + " (" + double.Parse(dt.Rows[0]["Hours"].ToString()) + " hours)");
            timeBar.SelectedIndex = 0;

            hours = double.Parse(dt.Rows[0]["Hours"].ToString());
            start = dt.Rows[0]["StartTime"].ToString();
            end = dt.Rows[0]["FinishTime"].ToString();

            timeBar.Items.Clear();

            timeBar.Items.Add(start + " - " + end + " (" + hours + " hour" + (hours == 1 ? "" : "s") + ")");
            timeBar.SelectedIndex = 0;

            if (!payTypeBar.Items.Contains(payType))
                payTypeBar.Items.Add(payType);

            payTypeBar.SelectedItem = payType;

            if (Core.showUserWorkOrders(username))
            {
                workOrder = dt.Rows[0]["WorkOrder"].ToString();

                if (workOrder != "")
                {
                    Oracle ora = new Oracle("select WADL01 from " + Core.getSchema(Core.getEnvironment()) + ".F4801 where WADOCO = :WO");
                    ora.AddParameter("WO", workOrder);
                    string woDesc = ora.Run().Rows[0]["WADL01"].ToString();

                    //workOrderBar.Items.Clear();
                    workOrderBar.Items.Add(workOrder + " - " + woDesc);
                    workOrderBar.SelectedIndex = workOrderBar.Items.Count - 1;
                }
            }

            textBox1.Text = dt.Rows[0]["Description"].ToString();

            sql = new SQL("select equipid from equipmenttimeentry where timesheetid=@ID");
            sql.AddParameter("@ID", timesheetID);
            dt = sql.Run();

            equipment = new string[dt.Rows.Count];

            //equipmentBar.Items.Clear();
            string eq = "";

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                equipment[i] = dt.Rows[i]["EquipID"].ToString();

                if (i == 0)
                    eq = equipment[i].Trim();
                else
                    eq += ", " + equipment[i].Trim();
            }

            equipmentBar.Items.Add(eq);
            equipmentBar.SelectedIndex = equipmentBar.Items.Count - 1;
            
            this.Text = "Edit Timesheet";
            button2.Text = "Save";

            dateCalendar.Select();
        }

        public void startingSetup()
        {
            showHideWorkingElements();

            DataTable dt;
            
            if(Core.canReview(Core.getUsername()) && !Core.getDepartments().Contains(21)) //Supervisors Non-Engineering
            {
                dt = SQL.Run("select paytype, description from paycodes where paytype not in (900, 80, 4, 108, 922, 901, 903, 906) and paytypeactive=1 order by description");
            }
            else if(Core.canReview(Core.getUsername())) //Supervisors Engineering
            {
                dt = SQL.Run("select paytype, description from paycodes where paytype not in (900, 80, 4, 108) and paytypeactive=1 order by description");
            }
            else if (Core.isSalary(Core.getEmpID(forUser))) //Salary
            {
                dt = SQL.Run(@"
SELECT PayType, [Description]
FROM PayCodes
WHERE paytype in (1, 2, 81, 90, 100, 105, 111, 112, 300, 305, 310, 311, 400, 808, 810, 811, 813, 816, 818, 820, 822, 825, 826, 901, 903, 906, 915, 922, 950, 955)
and paytypeactive = 1

ORDER BY PayType
");
            }
            //Facilities staff
            else if(Core.isFacilities())
                dt = SQL.Run(@"
SELECT PayType, [Description]
FROM PayCodes

WHERE PayType in (1, 2, 5, 105, 106, 955, 81, 809, 821, 807, 905, 311, 812, 811, 907, 90, 109, 111, 0, 904)
and paytypeactive = 1

ORDER BY PayType
");
            else if (Core.getDepartments().Contains(5)) //Facilities maintenance
                dt = SQL.Run(@"
SELECT PayType, [Description]
FROM PayCodes

WHERE PayType in (1, 2, 105, 955, 81, 809, 821, 807, 811, 905, 311, 907, 90, 109, 5, 106, 812, 111, 0, 904)
and paytypeactive = 1

ORDER BY PayType
");
            else if (Core.getDepartments().Contains(13)) //IT
                dt = SQL.Run(@"
SELECT PayType, [Description]
FROM PayCodes

WHERE PayType in (1, 2, 105, 950, 955, 81, 809, 807, 821, 822, 905, 311, 112, 111, 904)
and paytypeactive = 1

ORDER BY PayType
");
            else if (Core.getDepartments().Contains(2) && Core.isPartTime(Core.getUsername())) //Part-time Parks
                dt = SQL.Run(@"
SELECT PayType, [Description]
FROM PayCodes

WHERE PayType in (1, 105, 2, 0, 904, 90)
and paytypeactive = 1

ORDER BY PayType
");
            else
                dt = SQL.Run(@"
SELECT PayType, [Description]
FROM PayCodes

WHERE PayType in (1, 105, 955, 81, 809, 807, 821, 905, 311, 112, 111, 812, 2, 0, 904, 90)
and paytypeactive = 1

ORDER BY PayType
");

            payTypeBar.Items.Clear();

            int regIndex = -1;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                payTypeBar.Items.Add(dt.Rows[i]["Description"].ToString());

                if (payTypeBar.Items[i].ToString() == "Regular")
                    regIndex = i;
            }

            payTypeBar.SelectedIndex = regIndex;
            showHideWorkingElements();

            GetTime gt = new GetTime(forUser, dateCalendar.SelectionStart);
            
            hours = gt.hours;
            start = gt.startTime;
            end = gt.endTime;

            timeBar.Items.Clear();

            timeBar.Items.Add(start + " - " + end + " (" + hours + " hour" + (hours == 1 ? "" : "s") + ")");
            timeBar.SelectedIndex = 0;

            //Work orders
            if (Core.showUserWorkOrders(forUser))
            {
                //Work orders this person has used recently
                SQL sql = new SQL(@"
SELECT t.workorder,
COUNT(t.WorkOrder) AS CountOfWorkOrder

FROM Timesheets t
JOIN Users u on u.EMPLOYEEID = t.EmployeeID

WHERE
t.DateWorked > @DATE AND
t.WorkOrder IS NOT NULL AND
u.displayname = @USERNAME

GROUP BY
t.workorder

ORDER BY Count(t.WorkOrder) DESC;
");
                sql.AddParameter("@DATE", DateTime.Now.AddMonths(-1));
                sql.AddParameter("@USERNAME", forUser);
                dt = sql.Run();

                workOrderBar.Items.Clear();

                workOrderBar.Items.Add("");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string workorder = dt.Rows[i]["workorder"].ToString();
                    string count = dt.Rows[i]["CountOfWorkOrder"].ToString();

                    Oracle ora = new Oracle("select WADL01, WAVR01, WASRST from " + Core.getSchema(Core.getEnvironment()) + ".F4801 where WADOCO = @WADOCO and WASRST<>99");
                    ora.AddParameter("@WADOCO", workorder);
                    DataTable dt2 = ora.Run();

                    if (dt2.Rows.Count > 0)
                    {
                        string description = dt2.Rows[0]["WADL01"].ToString().Trim();
                        string asset = dt2.Rows[0]["WAVR01"].ToString().Trim();

                        workOrderBar.Items.Add(workorder + " - " + description + (asset != "" ? " (" + asset + ")" : ""));
                    }
                }

                if (workOrderBar.Items.Count > 0)
                    workOrderBar.SelectedIndex = 0;
            }

            //Equipment
            if (Core.showUserEquipment(forUser) || Environment.MachineName == "SYSNW-05-19")
            {
                SQL sql = new SQL(@"
SELECT e.EQUIPID, count(t.EmployeeID)
FROM Timesheets t
LEFT JOIN EquipmentTimeEntry e on e.TIMESHEETID = t.TimeCardDetailID
JOIN users u on t.EmployeeID = u.EMPLOYEEID
where t.DateWorked > @AFTERDATE
and u.displayname = @USERNAME
group by t.employeeid, equipid
order by count(t.employeeid) desc");
                sql.AddParameter("@AFTERDATE", DateTime.Today.AddMonths(-1));
                sql.AddParameter("@USERNAME", forUser);
                dt = sql.Run();

                bool putBlank = true;

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["equipid"].ToString() == null || dt.Rows[i]["equipid"].ToString() == "")
                        putBlank = false;

                    bool addIt = false;
                    Oracle ora = new Oracle("select FAACL9 from " + Core.getSchema(Core.getEnvironment()) + ".F1201 where FAAPID = :EQUIPID");
                    ora.AddParameter("EQUIPID", dt.Rows[i]["equipid"].ToString());
                    DataTable odt = ora.Run();

                    if (odt.Rows.Count > 0)
                    {
                        string p = odt.Rows[0][0].ToString();

                        if (p == "P01" || p == "P02" || p == "P03" || p == "P04" || p == "P05" || p == "P06"
                            || p == "P07" || p == "P08" || p == "P09" || p == "P10")
                            addIt = true;
                    }
                    else if (dt.Rows[i]["equipid"].ToString() == "")
                    {
                        addIt = true;
                    }

                    if (addIt)
                        equipmentBar.Items.Add(dt.Rows[i]["equipid"].ToString());
                }

                if (putBlank)
                    equipmentBar.Items.Add("");

                if (equipmentBar.Items.Count > 0)
                    equipmentBar.SelectedIndex = 0;
            }

            SQL bank = new SQL(@"select
(SELECT sum(hours) from Timesheets where paytype=80 and EmployeeID=@EMPID and dateworked>=@DATE) as [In],
(SELECT sum(hours) from Timesheets where paytype=81 and EmployeeID=@EMPID and dateworked>=@DATE) as [Out]");
            bank.AddParameter("@EMPID", Core.getEmpID(forUser));
            bank.AddParameter("@DATE", new DateTime(DateTime.Today.Year, 1, 1));
            dt = bank.Run();

            double hoursIn = dt.Rows[0]["In"].ToString() == "" ? 0 : double.Parse(dt.Rows[0]["In"].ToString());
            double hoursOut = dt.Rows[0]["Out"].ToString() == "" ? 0 : double.Parse(dt.Rows[0]["Out"].ToString());

            label7.Text = (1.5*hoursIn - hoursOut) + " / 40";
        }

        string workOrder = "";
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                FindWorkOrder fwo;

                if (forDifferentUser)
                    fwo = new FindWorkOrder(forUser);
                else
                    fwo = new FindWorkOrder();

                fwo.ShowDialog();

                if (!fwo.quit)
                {
                    if (fwo.woID != null)
                    {
                        workOrder = fwo.woID;

                        workOrderBar.Items.Clear();
                        workOrderBar.Items.Add(fwo.woID + " - " + fwo.woDesc);
                        workOrderBar.SelectedIndex = 0;
                    }
                    else
                    {
                        workOrderBar.Items.Clear();
                    }
                }
            }
            catch(System.TypeInitializationException e2)
            {
                MessageBox.Show("This component requires Oracle to be installed. Please call IT to have this done.", "Error");
            }
        }

        private bool isHoliday(DateTime date)
        {
            bool returnable = false;

            try
            {
                DataTable dt = SQL.Run("select date from holidays");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (DateTime.Parse(dt.Rows[i][0].ToString()) == date)
                    {
                        returnable = true;
                    }
                }
            }
            catch (Exception)
            {
                return returnable;
            }
            return returnable;
        }

        private void comboBox2_MouseClick(object sender, MouseEventArgs e)
        {
            //workOrderBar.Enabled = false;
            //workOrderBar.Update();
            
            //button1_Click(null, null);
            
            //workOrderBar.Enabled = true;
            //workOrderBar.Update();
        }

        private void button2_Click(object sender, EventArgs e)
        {
           
            if (Core.isDateOpen(dateCalendar.SelectionStart, Core.isSalary(Core.getEmpID(forUser)) ? "S" : "H"))
            {
                if (Core.isAdmin(Core.getUsername()) || !checkForErrors()) //Be an administrator, or pass the check
                {
                    DateTime startDate = dateCalendar.SelectionStart;
                    DateTime endDate = dateCalendar.SelectionEnd;
                    var cancelled = false;
                    foreach (var holiday in holidays)
                    {
                        if (holiday.Ticks >=startDate.Ticks && holiday.Ticks <= endDate.Ticks)
                        {
                            DialogResult dialogResult = MessageBox.Show("You are entering hours on a day that is a Town holiday. Are you sure that you want to do this?", "Holiday Selected", MessageBoxButtons.YesNo);

                            if(dialogResult == DialogResult.Yes)
                            {
                                cancelled = false;
                            }
                            else if(dialogResult == DialogResult.No)
                            {
                                cancelled = true;
                            }
                        }                   
                    }
                    if(!cancelled)
                    {
                        changesMade = true;

                        if (isEditMode())
                            saveEdit();
                        else
                            submitNew();

                        this.Close();
                    }
                   
                }
            }
            else
                MessageBox.Show(Core.getLockoutText());
        }

        private bool checkForErrors()
        {
            bool foundErrors = false;
            
            //Don't work for too long
            if (hours > 24 && payTypeBar.Text != "Bk Hrs. Paid")
            {
                foundErrors = true;
                MessageBox.Show("Your timesheet cannot be for more than 24 hours.");

                return foundErrors;
            }

            //Submit in 15 minute increments
            if (hours * 400 % 100 != 0 && payTypeBar.Text != "Bk Hrs. Paid")
            {
                foundErrors = true;
                MessageBox.Show("Please have your hours be in 0.25 hour (15 minute) increments.");

                return foundErrors;
            }

            //No cross-year stuff
            if (dateCalendar.SelectionStart.Year != dateCalendar.SelectionEnd.Year)
            {
                foundErrors = true;
                MessageBox.Show("This entry would span different years. Please enter your timesheets in batches.");

                return foundErrors;
            }

            //Disabled for Engineering
            if ((workOrderBar.Visible == true && (workOrderBar.Items.Count == 0 || workOrderBar.SelectedItem.ToString().Split(' ')[0] == "")) && !Core.getDepartments(forUser).Contains(21))
            {
                foundErrors = true;
                MessageBox.Show("Please select a work order.");

                return foundErrors;
            }

            if (Core.getDepartmentForcesDescription(forUser) && !Core.canReview(Core.getUsername()))
            {
                if (textBox1.Text.Length == 0)
                {
                    foundErrors = true;
                    MessageBox.Show("Please enter a description.");

                    return foundErrors;
                }
            }
            
            if ((dateCalendar.SelectionStart.Month == 12 || dateCalendar.SelectionEnd.Month == 12) && (payTypeBar.Text == "Banked Time 1.0" || payTypeBar.Text == "Banked Time 1.5"))
            {
                foundErrors = true;
                MessageBox.Show("According to company policy, an employee cannot submit new banked time in December." + Environment.NewLine
                    + "You can still make use of your already-banked time. Please speak to your supervisor if you have any questions.");

                return foundErrors;
            }

            if ((dateCalendar.SelectionStart.Month >= 4 || dateCalendar.SelectionEnd.Month >= 4) && (payTypeBar.Text == "Vac(S)Pr.Yr." || payTypeBar.Text == "Vac.(H) Prior Year"))
            {
                foundErrors = true;
                MessageBox.Show("Effective April 1st, prior-year vacation is no longer accessible. Please use banked vacation." + Environment.NewLine
                    + "Please speak to your supervisor if you have any questions.");

                return foundErrors;
            }

            if (payTypeBar.Text != "Standby" && payTypeBar.Text != "Standby -Weekend" && payTypeBar.Text != "Meal Allowance")
            {
                if (hours == 0)
                {
                    foundErrors = true;
                    MessageBox.Show("The pay type you chose can't have 0 hours.");

                    return foundErrors;
                }
            }

            if (payTypeBar.Text == "Standby" || payTypeBar.Text == "Standby -Weekend")
            {
                bool alreadyExists = false;

                for (DateTime i = dateCalendar.SelectionStart; i <= dateCalendar.SelectionEnd; i = i.AddDays(1))
                {
                    SQL standby = new SQL(@"
select *
from Timesheets
where employeeid = @EMPID and dateworked = @DATE and paytype in (2, 4)");
                    standby.AddParameter("@EMPID", Core.getEmpID(forUser));
                    standby.AddParameter("@DATE", i);

                    if (standby.Run().Rows.Count > 0)
                        alreadyExists = true;
                }
                
                if (alreadyExists)
                {
                    foundErrors = true;
                    MessageBox.Show("A timesheet for standby already exists.");

                    return foundErrors;
                }
            }

            if (payTypeBar.Text == "Meal Allowance")
            {
                bool alreadyExists = false;

                for (DateTime i = dateCalendar.SelectionStart; i <= dateCalendar.SelectionEnd; i = i.AddDays(1))
                {
                    SQL standby = new SQL(@"
select *
from Timesheets
where employeeid = @EMPID and dateworked = @DATE and paytype in (90)");
                    standby.AddParameter("@EMPID", Core.getEmpID(forUser));
                    standby.AddParameter("@DATE", i);

                    if (standby.Run().Rows.Count > 0)
                        alreadyExists = true;
                }

                if (alreadyExists)
                {
                    foundErrors = true;
                    MessageBox.Show("A timesheet for meal allowance already exists.");

                    return foundErrors;
                }
            }

            try
            {
                int period = Core.getPeriod(dateCalendar.SelectionEnd, Core.getSH(forUser));
            }
            catch (IndexOutOfRangeException e)
            {
                foundErrors = true;
                MessageBox.Show("The date you chose falls within a pay period that doesn't exist yet." + Environment.NewLine
                    + "Pay periods are created near year-end by Payroll." + Environment.NewLine
                    + Environment.NewLine
                    + "Please contact them for more information.");

                return foundErrors;
            }

            //Appointment time
            SQL sql = new SQL("select paytype from paycodes where description=@DESCRIPTION");
            sql.AddParameter("@DESCRIPTION", payTypeBar.Text);
            int payType = int.Parse(sql.Run().Rows[0][0].ToString());

            if (payType == 311)
            {
                sql = new SQL(@"
select EmployeeID, SUM(hours) from Timesheets
where
PayType = 311
and DateWorked >= @START
and DateWorked < @END
and employeeid = @EMPID
group by EmployeeID");
                sql.AddParameter("@START", new DateTime(dateCalendar.SelectionStart.Year, 1, 1));
                sql.AddParameter("@END", new DateTime(dateCalendar.SelectionStart.Year + 1, 1, 1));
                sql.AddParameter("@EMPID", Core.getEmpID(forUser));
                DataTable dt2 = sql.Run();

                double used2 = 0;
                if (dt2.Rows.Count > 0)
                    used2 = double.Parse(dt2.Rows[0][1].ToString());

                if (isEditMode())
                {
                    sql = new SQL("select paytype, hours from Timesheets where timecarddetailid = @ID");
                    sql.AddParameter("@ID", editingTimesheetID);
                    bool isCurrently311 = double.Parse(sql.Run().Rows[0]["paytype"].ToString()) == 311;
                    double currentHours = double.Parse(sql.Run().Rows[0]["hours"].ToString());

                    if (!isCurrently311)
                    {
                        if (used2 + hours > Core.getAppointmentLimit())
                        {
                            foundErrors = true;
                            MessageBox.Show("You've used " + used2 + " hours of appointment time this year." + Environment.NewLine
                            + "This timesheet would bring you over the limit of " + Core.getAppointmentLimit() + " hours." + Environment.NewLine
                            + Environment.NewLine
                            + "Please select another pay type for the remaining balance after " + Core.getAppointmentLimit() + " hours.");

                            return foundErrors;
                        }
                    }
                    else
                    {
                        if (used2 - currentHours + hours > Core.getAppointmentLimit())
                        {
                            foundErrors = true;
                            MessageBox.Show("You've used " + used2 + " hours of appointment time this year." + Environment.NewLine
                            + "This timesheet would bring you over the limit of " + Core.getAppointmentLimit() + " hours." + Environment.NewLine
                            + Environment.NewLine
                            + "Please select another pay type for the remaining balance after " + Core.getAppointmentLimit() + " hours.");

                            return foundErrors;
                        }
                    }
                }
                else
                {
                    if (used2 + hours > Core.getAppointmentLimit())
                    {
                        foundErrors = true;
                        MessageBox.Show("You've used " + used2 + " hours of appointment time this year." + Environment.NewLine
                            + "This timesheet would bring you over the limit of " + Core.getAppointmentLimit() + " hours." + Environment.NewLine
                            + Environment.NewLine
                            + "Please select another pay type for the remaining balance after " + Core.getAppointmentLimit() + " hours.");

                        return foundErrors;
                    }
                }
            }

            //Stat
            sql = new SQL("select paytype from paycodes where description=@DESCRIPTION");
            sql.AddParameter("@DESCRIPTION", payTypeBar.Text);
            payType = int.Parse(sql.Run().Rows[0][0].ToString());

            if (payType == 811)
            {
                DateTime start = new DateTime(dateCalendar.SelectionStart.Year, 1, 1);
                DateTime end = new DateTime(dateCalendar.SelectionStart.Year + 1, 1, 1);

                double statUsed = Core.getStatUsed(start, end, Core.getEmpID(forUser));

                if (isEditMode())
                {
                    sql = new SQL("select paytype, hours from Timesheets where timecarddetailid = @ID");
                    sql.AddParameter("@ID", editingTimesheetID);
                    bool isCurrently811 = double.Parse(sql.Run().Rows[0]["paytype"].ToString()) == 811;

                    if (!isCurrently811)
                    {
                        if (statUsed + 1 > Core.getStatMax(start, end, Core.getEmpID(forUser)))
                        {
                            foundErrors = true;
                            MessageBox.Show("You've used " + statUsed + " stats this year." + Environment.NewLine
                            + "This timesheet would bring you over the limit of " + Core.getStatMax(start, end, Core.getEmpID(forUser)) + ".");

                            return foundErrors;
                        }
                    }
                }
                else
                {
                    if (statUsed + 1 > Core.getStatMax(start, end, Core.getEmpID(forUser)))
                    {
                        foundErrors = true;
                        MessageBox.Show("You've used " + statUsed + " stats this year." + Environment.NewLine
                            + "This timesheet would bring you over the limit of " + Core.getStatMax(start, end, Core.getEmpID(forUser)) + ".");

                        return foundErrors;
                    }
                }
            }

            //Prior year vacation
            payType = int.Parse(SQL.RunString("select paytype from paycodes where description=@DESCRIPTION", payTypeBar.Text));

            if (payType == 807)
            {
                int year = dateCalendar.SelectionStart.Year - 1;
                int empID = Core.getEmpID(forUser);
                double balance = Core.getVacationBalance(year, empID, year < DateTime.Now.Year);

                if (isEditMode())
                {
                    DataTable temp = SQL.Run("select paytype, hours from Timesheets where timecarddetailid = @ID", editingTimesheetID);
                    bool isCurrentlyVac = double.Parse(temp.Rows[0]["paytype"].ToString()) == 807;
                    double currentHours = double.Parse(temp.Rows[0]["hours"].ToString());

                    if ((!isCurrentlyVac && balance - hours < 0) ||
                        (balance + currentHours - hours < 0))
                        foundErrors = true;
                }
                else if (balance - hours < 0)
                    foundErrors = true;

                if (foundErrors)
                {
                    MessageBox.Show("Your " + year + " Vacation balance is " + balance + "." + Environment.NewLine
                        + "This timesheet would use more than you have remaining." + Environment.NewLine
                        + Environment.NewLine
                        + "Please select another pay type, or reduce the hours you're putting into this timesheet.");
                    return foundErrors;
                }
            }

            //Current year vacation
            payType = int.Parse(SQL.RunString("select paytype from paycodes where description=@DESCRIPTION", payTypeBar.Text));

            if (payType == 809 || payType == 821)
            {
                int year = dateCalendar.SelectionStart.Year;
                int empID = Core.getEmpID(forUser);
                double balance = Core.getVacationBalance(year, empID, year < DateTime.Now.Year);

                if (isEditMode())
                {
                    DataTable temp = SQL.Run("select paytype, hours from Timesheets where timecarddetailid = @ID", editingTimesheetID);
                    bool isCurrentlyVac = double.Parse(temp.Rows[0]["paytype"].ToString()) == 809 || double.Parse(temp.Rows[0]["paytype"].ToString()) == 821;
                    double currentHours = double.Parse(temp.Rows[0]["hours"].ToString());

                    if ((!isCurrentlyVac && balance - hours < 0) ||
                        (balance + currentHours - hours < 0))
                        foundErrors = true;
                }
                else if (balance - hours < 0)
                    foundErrors = true;

                if (foundErrors)
                {
                    MessageBox.Show("Your " + year + " Vacation balance is " + balance + "." + Environment.NewLine
                        + "This timesheet would use more than you have remaining." + Environment.NewLine
                        + Environment.NewLine
                        + "Please select another pay type, or reduce the hours you're putting into this timesheet.");
                    return foundErrors;
                }
            }

            //Too many hours
            sql = new SQL("select paytype from paycodes where description=@DESCRIPTION");
            sql.AddParameter("@DESCRIPTION", payTypeBar.Text);
            payType = int.Parse(sql.Run().Rows[0][0].ToString());

            for (int i = 0; dateCalendar.SelectionStart.AddDays(i) <= dateCalendar.SelectionEnd; i++)
            {
                DateTime date = dateCalendar.SelectionStart.AddDays(i);

                sql = new SQL(@"
select EmployeeID, SUM(hours) from Timesheets
where DateWorked >= @START
and DateWorked < @END
and employeeid = @EMPID
group by EmployeeID");
                sql.AddParameter("@START", date);
                sql.AddParameter("@END", date.AddDays(1));
                sql.AddParameter("@EMPID", Core.getEmpID(forUser));
                DataTable dt = sql.Run();

                double used = 0;
                if (dt.Rows.Count > 0)
                    used = double.Parse(dt.Rows[0][1].ToString());

                if (isEditMode())
                {
                    sql = new SQL("select paytype, hours from Timesheets where timecarddetailid = @ID");
                    sql.AddParameter("@ID", editingTimesheetID);
                    double currentHours = double.Parse(sql.Run().Rows[0]["hours"].ToString());

                    if (used - currentHours + hours > 8)
                    {
                        if (MessageBox.Show("More than 8 hours entered for " + date.ToString("D") + ", please confirm this is valid", "Please confirm", MessageBoxButtons.YesNo) == DialogResult.No)
                        {
                            foundErrors = true;
                            return foundErrors;
                        }
                    }
                }
                else
                {
                    if (used + hours > 8)
                    {
                        if (MessageBox.Show("More than 8 hours entered for " + date.ToString("D") + ", please confirm this is valid", "Please confirm", MessageBoxButtons.YesNo) == DialogResult.No)
                        {
                            foundErrors = true;
                            return foundErrors;
                        }
                    }
                }
            }
  
            //Banked time
            sql = new SQL("select paytype from paycodes where description=@DESCRIPTION");
            sql.AddParameter("@DESCRIPTION", payTypeBar.Text);
            payType = int.Parse(sql.Run().Rows[0][0].ToString());

            if (payType == 950 || payType == 955) //Putting in time
            {
                double banked = Core.getBankedTimeTotals(forUser);
                double hourValue = payType == 955 ? hours * 1.5 : hours;

                if (!Core.getDepartments().Contains(21)) //Engineering
                {
                    if (isEditMode()) //Edit
                    {
                        sql = new SQL("select paytype, hours from Timesheets where timecarddetailid = @ID");
                        sql.AddParameter("@ID", editingTimesheetID);
                        double currentPayType = double.Parse(sql.Run().Rows[0]["paytype"].ToString());
                        double currentHours = double.Parse(sql.Run().Rows[0]["hours"].ToString());
                        currentHours = currentPayType == 955 ? currentHours * 1.5 : currentHours;


                        if (banked - currentHours + hourValue > Core.getBankAllotment(forUser))
                        {
                            foundErrors = true;
                            MessageBox.Show("You've banked " + banked + " hours this year." + Environment.NewLine
                                + "This timesheet would bring you over the limit of " + Core.getBankAllotment(forUser) + " hours." + Environment.NewLine
                                + Environment.NewLine
                                + "Please select another pay type for the remaining balance after " + Core.getBankAllotment(forUser) + " hours.");

                            return foundErrors;
                        }
                    }
                    else //New
                    {
                        if (banked + hourValue > Core.getBankAllotment(forUser))
                        {
                            foundErrors = true;
                            MessageBox.Show("You've banked " + banked + " hours this year." + Environment.NewLine
                                    + "This timesheet would bring you over the limit of " + Core.getBankAllotment(forUser) + " hours." + Environment.NewLine
                                    + Environment.NewLine
                                    + "Please select another pay type for the remaining balance after " + Core.getBankAllotment(forUser) + " hours.");

                            return foundErrors;
                        }
                    }
                }
            }

            //Banked time off
            sql = new SQL("select paytype from paycodes where description=@DESCRIPTION");
            sql.AddParameter("@DESCRIPTION", payTypeBar.Text);
            payType = int.Parse(sql.Run().Rows[0][0].ToString());

            if (payType == 81) //Taking out time
            {
                double balance = Core.getBankedTimeBalance(forUser);
                double currentHours = 0;

                if (isEditMode()) //Edit
                {
                    sql = new SQL("select paytype, hours from Timesheets where timecarddetailid = @ID");
                    sql.AddParameter("@ID", editingTimesheetID);
                    int oldPayType = int.Parse(sql.Run().Rows[0]["paytype"].ToString());

                    if(oldPayType == 81) //Hours in other pay types don't affect balance
                        currentHours = double.Parse(sql.Run().Rows[0]["hours"].ToString());
                }

                if (balance + currentHours - hours < 0)
                {
                    foundErrors = true;
                    MessageBox.Show("You have " + balance + " hours in your bank." + Environment.NewLine
                        + "This timesheet is asking to use more time than is available." + Environment.NewLine
                        + Environment.NewLine
                        + "Please select another pay type.");

                    return foundErrors;
                }
            }

            //Floater
            sql = new SQL("select paytype from paycodes where description=@DESCRIPTION");
            sql.AddParameter("@DESCRIPTION", payTypeBar.Text);
            payType = int.Parse(sql.Run().Rows[0][0].ToString());

            if (payType == 812 || payType == 814)
            {
                sql = new SQL(
                    @"select count(*)
from Timesheets
where paytype in (812, 814) and dateworked >= @START and dateworked < @END and employeeid = @EMPID");
                sql.AddParameter("@START", new DateTime(dateCalendar.SelectionStart.Year, 1, 1));
                sql.AddParameter("@END", new DateTime(dateCalendar.SelectionStart.Year + 1, 1, 1));
                sql.AddParameter("@EMPID", Core.getEmpID(forUser));
                bool alreadyUsed = int.Parse(sql.Run().Rows[0][0].ToString()) >= 1;
                
                if (isEditMode()) //Editing
                {
                    sql = new SQL("select paytype from Timesheets where timecarddetailid = @ID");
                    sql.AddParameter("@ID", editingTimesheetID);
                    double currentPayType = double.Parse(sql.Run().Rows[0]["paytype"].ToString());

                    if (currentPayType != 812 && currentPayType != 814) //This timesheet isn't already floater
                    {
                        if (alreadyUsed)
                        {
                            foundErrors = true;
                            MessageBox.Show("Your floater has already been used for this year.");

                            return foundErrors;
                        }
                    }
                }
                else
                {
                    if (alreadyUsed)
                    {
                        foundErrors = true;
                        MessageBox.Show("Your floater has already been used for this year.");

                        return foundErrors;
                    }
                }
            }

            //Emergency Leave (H), (S)
            payType = int.Parse(SQL.RunString("select paytype from paycodes where description=@DESCRIPTION", payTypeBar.Text));
            
            if (payType == 904 || payType == 901)
            {
                sql = new SQL(
                    @"select count(*)
from Timesheets
where paytype in (904, 901) and dateworked >= @START and dateworked < @END and employeeid = @EMPID");
                sql.AddParameter("@START", new DateTime(dateCalendar.SelectionStart.Year, 1, 1));
                sql.AddParameter("@END", new DateTime(dateCalendar.SelectionStart.Year + 1, 1, 1));
                sql.AddParameter("@EMPID", Core.getEmpID(forUser));
                bool alreadyUsed = int.Parse(sql.Run().Rows[0][0].ToString()) >= 2;

                if (isEditMode()) //Editing
                {
                    sql = new SQL("select paytype from Timesheets where timecarddetailid = @ID");
                    sql.AddParameter("@ID", editingTimesheetID);
                    double currentPayType = double.Parse(sql.Run().Rows[0]["paytype"].ToString());

                    if (currentPayType != 904 && currentPayType != 901) //This timesheet isn't Emergency Leave (H), (S)
                    {
                        if (alreadyUsed)
                        {
                            foundErrors = true;
                            MessageBox.Show("Your paid emergency leave has already been used for this year.");

                            return foundErrors;
                        }
                    }
                }
                else
                {
                    if (alreadyUsed)
                    {
                        foundErrors = true;
                        MessageBox.Show("Your paid emergency leave has already been used for this year.");

                        return foundErrors;
                    }
                }
            }

            //Regular
            sql = new SQL("select regyn from paycodes where description=@DESCRIPTION");
            sql.AddParameter("@DESCRIPTION", payTypeBar.Text);
            int regyn = int.Parse(sql.Run().Rows[0][0].ToString());

            if (regyn != 0)
            {
                DateTime start = dateCalendar.SelectionStart;

                while (start.DayOfWeek != DayOfWeek.Monday)
                    start = start.AddDays(-1);

                DateTime end = start.AddDays(7);

                sql = new SQL(@"select sum(hours) as Hours
                    from Timesheets t, users u
where t.employeeid = u.employeeid
and u.displayname = @USERNAME
and t.paytype in (1, 100)
and dateworked >= @DATEWORKED
and dateworked < @DATEEND");
                sql.AddParameter("@USERNAME", forUser);
                sql.AddParameter("@DATEWORKED", start);
                sql.AddParameter("@DATEEND", end);
                DataTable w1 = sql.Run();

                string empType = Core.getSH(forUser);
                int overtimeThreshold = empType == "H" ? 44 : 40;                

                double used2 = 0;
                if (w1.Rows[0][0].ToString() != "")
                    used2 = double.Parse(w1.Rows[0][0].ToString());

                double totalTryingToUse = 0;

                for (int i = 0; dateCalendar.SelectionStart.AddDays(i) <= dateCalendar.SelectionEnd; i++)
                    totalTryingToUse += hours;

                if (isEditMode())
                {
                    sql = new SQL("select paytype, hours from Timesheets where timecarddetailid = @ID");
                    sql.AddParameter("@ID", editingTimesheetID);
                    double currentPayType = double.Parse(sql.Run().Rows[0]["paytype"].ToString());
                    double currentHours = double.Parse(sql.Run().Rows[0]["hours"].ToString());

                    if (currentPayType == 1 || currentPayType == 100)
                    {
                        if (used2 - currentHours + totalTryingToUse > overtimeThreshold)
                        {
                            foundErrors = true;
                            MessageBox.Show("You've used " + used2 + " regular hours this week." + Environment.NewLine
                                + "This timesheet would bring you over the limit of " + overtimeThreshold + " hours." + Environment.NewLine
                                + Environment.NewLine
                                + "Please select another pay type for the remaining balance after " + overtimeThreshold + " hours.");

                            return foundErrors;
                        }
                    }
                    else
                    {
                        if (used2 + totalTryingToUse > overtimeThreshold)
                        {
                            foundErrors = true;
                            MessageBox.Show("You've used " + used2 + " regular hours this week." + Environment.NewLine
                                + "This timesheet would bring you over the limit of " + overtimeThreshold + " hours." + Environment.NewLine
                                + Environment.NewLine
                                + "Please select another pay type for the remaining balance after " + overtimeThreshold + " hours.");

                            return foundErrors;
                        }
                    }
                }
                else
                {
                    if (Core.isPartTime(forUser) && used2 + totalTryingToUse > overtimeThreshold)
                    {
                        foundErrors = true;
                        MessageBox.Show("You've used " + used2 + " regular hours this week." + Environment.NewLine
                                + "This timesheet would bring you over the limit of " + overtimeThreshold + " hours." + Environment.NewLine
                                + Environment.NewLine
                                + "Please select another pay type for the remaining balance after " + overtimeThreshold + " hours.");

                        return foundErrors;
                    }
                }
            }

            return foundErrors;
        }

        private void saveEdit()
        {
            //Grab values
            SQL sql = new SQL("select paytype from paycodes where description=@DESCRIPTION");
            sql.AddParameter("@DESCRIPTION", payTypeBar.Text);
            int payType = int.Parse(sql.Run().Rows[0][0].ToString());
            int period = Core.getPeriod(dateCalendar.SelectionStart, Core.getSH(forUser));

            double lumpSum = 0.00;

            if (payType == 2 || payType == 4) //Standby, Standby Weekend
            {
                if (dateCalendar.SelectionStart.DayOfWeek.ToString() == "Saturday" || dateCalendar.SelectionStart.DayOfWeek.ToString() == "Sunday" || (isHoliday(dateCalendar.SelectionStart) && !Core.isEasterMonday(dateCalendar.SelectionStart)))
                    lumpSum = 45.00;
                else
                    lumpSum = 24.00;
            }
            if (payType == 90) //Meal allowance
                lumpSum = 10.00;

            sql = new SQL("select * from Timesheets where timecarddetailid = @ID");
            sql.AddParameter("@ID", editingTimesheetID);
            DataTable before = sql.Run();


            //Update detail
            sql = new SQL(@"update Timesheets set
                                        period = @PERIOD,
                                        dateworked = @DATEWORKED,
                                        paytype = @PAYTYPE,
                                        hours = @HOURS,
                                        workorder = @WORKORDER,
                                        lumpsum = @LUMPSUM,
                                        dateentered = @DATEENTERED,
                                        [description] = @DESCRIPTION,
                                        starttime = @STARTTIME,
                                        finishtime = @FINISHTIME
 
                                        WHERE timecarddetailid = @TIMESHEETID");

            sql.AddParameter("@TIMESHEETID", editingTimesheetID);
            sql.AddParameter("@PERIOD", period);
            sql.AddParameter("@DATEWORKED", dateCalendar.SelectionStart);
            sql.AddParameter("@PAYTYPE", payType);
            sql.AddParameter("@HOURS", hours);

            if (workOrderBar.Items.Count == 0 || workOrderBar.Visible == false || workOrderBar.SelectedItem.ToString().Split(' ')[0].Trim() == "")
                sql.AddParameter("@WORKORDER", DBNull.Value);
            else
                sql.AddParameter("@WORKORDER", workOrderBar.SelectedItem.ToString().Split(' ')[0]);

            sql.AddParameter("@LUMPSUM", lumpSum);
            sql.AddParameter("@DATEENTERED", DateTime.Now);
            sql.AddParameter("@DESCRIPTION", textBox1.Text);
            sql.AddParameter("@STARTTIME", start);
            sql.AddParameter("@FINISHTIME", end);

            sql.Run();

            sql = new SQL("select * from Timesheets where timecarddetailid = @ID");
            sql.AddParameter("@ID", editingTimesheetID);
            DataTable after = sql.Run();

            string beforeString = "", afterString = "";

            for (int i = 0; i < before.Columns.Count; i++)
            {
                if(before.Rows[0][i].ToString() != after.Rows[0][i].ToString() && before.Columns[i].ColumnName.ToLower() != "dateentered")
                {
                    beforeString += (beforeString == "" ? "" : ", ") + before.Columns[i].ColumnName.ToLower() + "=" + before.Rows[0][i].ToString();
                    afterString += (afterString == "" ? "" : ", ") + after.Columns[i].ColumnName.ToLower() + "=" + after.Rows[0][i].ToString();
                }
            }

            Core.logHistory("Edited timesheet", "Timesheet ID# " + editingTimesheetID + " - " + (beforeString == "" ? "No changes" : beforeString), afterString);
            Core.logTimesheetHistory("Timesheet edited", editingTimesheetID);

            //Delete equipment
            sql = new SQL("delete from equipmenttimeentry where timesheetid=@TIMESHEETID");
            sql.AddParameter("@TIMESHEETID", editingTimesheetID);
            sql.Run();

            //Insert equipment
            for (int i = 0; equipment != null && i < equipment.Length; i++)
            {
                if (equipment[i] != "")
                {
                    sql = new SQL(@"insert into equipmenttimeentry values(
                @TIMESHEETID,
                @EQUIPID,
                @SENT)");

                    sql.AddParameter("@EQUIPID", equipment[i].TrimStart());
                    sql.AddParameter("@TIMESHEETID", editingTimesheetID);
                    sql.AddParameter("@SENT", false);
                    
                    sql.Run();
                }
            }
        }

        private void submitNew()
        {
            string username = forUser;
            DateTime date = dateCalendar.SelectionStart;
            string payCode = payTypeBar.Text;
            string workorder = "";
            string description = textBox1.Text;
            
            if (workOrderBar.Items.Count > 0 && workOrderBar.Visible)
                workorder = workOrderBar.SelectedItem.ToString().Split(' ')[0];

            if (equipmentBar.Visible == false)
                equipment = null;

            for (int i = 0; dateCalendar.SelectionStart.AddDays(i) <= dateCalendar.SelectionEnd; i++)
                Core.newTimesheet(username, dateCalendar.SelectionStart.AddDays(i), start, end, payCode, hours, workorder, description, equipment);
        }

        private int getPeriod(DateTime x)
        {
            SQL sql = new SQL("select period from periods where [from]<=@DATE and [to]>=@DATE");
            sql.AddParameter("@DATE", x);

            return int.Parse(sql.Run().Rows[0][0].ToString());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                FindEquipment feq;

                if (forDifferentUser)
                    feq = new FindEquipment(forUser, equipment);
                else
                    feq = new FindEquipment(equipment);

                feq.ShowDialog();

                if (!feq.quit)
                {
                    equipment = feq.equipment;

                    equipmentBar.Items.Clear();

                    string str = "";

                    for(int i = 0; i < equipment.Length; i++)
                    {
                        if (i == 0)
                            str = equipment[i].Trim();
                        else
                            str += ", " + equipment[i].Trim();
                    }

                    equipmentBar.Items.Add(str);
                    equipmentBar.SelectedIndex = 0;
                }
                else
                {
                
                }
            }
            catch (System.TypeInitializationException e2)
            {
                MessageBox.Show("This component requires Oracle to be installed. Please call IT to have this done.", "Error");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //listBox1.Items.Remove(listBox1.SelectedItem);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            FindPayType fpt = new FindPayType(true);
            fpt.ShowDialog();

            if (fpt.payType != null)
            {
                payTypeBar.SelectedItem = fpt.payType;
            }
            else
            {
                
            }
        }

        public bool isEditMode()
        {
            return editingTimesheetID != 0;
        }

        private void evaluateHours(string oldStart, string oldEnd)
        {
            //TimeSpan now = DateTime.Parse(label10.Text) - DateTime.Parse(label9.Text);
            //TimeSpan before = DateTime.Parse(oldEnd) - DateTime.Parse(oldStart);

            //decimal beforeHours = Math.Abs(before.Hours) + (decimal)(Math.Abs(before.Minutes))/60;
            //decimal nowHours = Math.Abs(now.Hours) + (decimal)(Math.Abs(now.Minutes))/60;

            //if(Core.calculateHoursRelatively())
            //    numericUpDown1.Value = Math.Max(numericUpDown1.Value + nowHours - beforeHours, 0);
            //else //Mathematically
            //    numericUpDown1.Value = Math.Abs(now.Hours) + (decimal)(Math.Abs(now.Minutes))/60;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            //if (((decimal)(int)(numericUpDown1.Value * 4)) / 4 != numericUpDown1.Value)
            //    numericUpDown1.Value = Math.Round(numericUpDown1.Value * 4, 0)/4;
        }

        private void showHideWorkingElements()
        {
            bool mulock = !Core.showUserEquipment(forUser) && !Core.showUserWorkOrders(forUser);

            SQL sql = new SQL("select transtype from paycodes where description=@PAYTYPE");
            sql.AddParameter("@PAYTYPE", payTypeBar.Text);
            DataTable dt = sql.Run();

            bool payTypeUsesWOs = false;
            
            if(dt.Rows.Count > 0)
                payTypeUsesWOs = dt.Rows[0][0].ToString() != "1";

            bool answer = !mulock && payTypeUsesWOs;

            label7.Visible = (payTypeBar.Text == "Bank Hrs" || payTypeBar.Text == "Bank Time Off");
            label3.Visible = answer && Core.showUserWorkOrders(forUser);
            workOrderBar.Visible = answer && Core.showUserWorkOrders(forUser);
            button1.Visible = answer && Core.showUserWorkOrders(forUser);

            label4.Visible = answer && Core.showUserEquipment(forUser);
            equipmentBar.Visible = answer && Core.showUserEquipment(forUser);
            button4.Visible = answer && Core.showUserEquipment(forUser);

            if (answer) //Show
            {
                //textBox1.Location = new Point(textBox1.Location.X, comboBox3.Bottom + 6);

                //label5.Location = new Point(label5.Location.X, textBox1.Location.Y + 3);
                //button2.Location = new Point(button2.Location.X, textBox1.Bottom + 6);
                //this.Height = button2.Bottom + 49;
            }
            else //Hide
            {
                if(Core.showUserWorkOrders(forUser) && Core.showUserEquipment(forUser))
                {
                    //Do nothing
                }
                else if(Core.showUserWorkOrders(forUser) && !Core.showUserEquipment(forUser))
                {
                    int upBy = textBox1.Location.Y - equipmentBar.Location.Y;
                    label5.Location = label4.Location;
                    textBox1.Location = equipmentBar.Location;

                    label4.Visible = false;
                    equipmentBar.Visible = false;

                    this.Height = 630 - upBy;
                }
                else if(!Core.showUserWorkOrders(forUser) && Core.showUserEquipment(forUser))
                {
                    MessageBox.Show("You shouldn't see this message. Please contact IT.");
                }
                else if(!Core.showUserWorkOrders(forUser) && !Core.showUserEquipment(forUser))
                {
                    label5.Location = label3.Location;
                    textBox1.Location = workOrderBar.Location;

                    this.Height = 538;
                    this.Width = 529;
                }

                //equipmentBar.Items.Clear();
                //workOrderBar.Items.Clear();

                //textBox1.Location = new Point(textBox1.Location.X, comboBox1.Bottom + 6);

                //label5.Location = new Point(label5.Location.X, label2.Location.Y + 27);
                //button2.Location = new Point(button2.Location.X, textBox1.Bottom + 6);
                //this.Height = button2.Bottom + 49;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            showHideWorkingElements();

            if(payTypeBar.Text == "Standby" || payTypeBar.Text == "Standby -Weekend")
            {
                hours = 0;
                start = "";
                end = "";

                timeBar.Items.Clear();

                timeBar.Items.Add(start + " - " + end + " (" + hours + " hour" + (hours == 1 ? "" : "s") + ")");
                timeBar.SelectedIndex = 0;
            }
        }

        private void NewTimesheet_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                button4_Click(null, null);
        }

        private void NewTimesheetold_Load(object sender, EventArgs e)
        {

        }

        private void comboBox3_MouseClick(object sender, MouseEventArgs e)
        {
            //MessageBox.Show("This feature will become available soon.");

            //FindEquipment fe = new FindEquipment(forUser, equipment);
            //fe.ShowDialog();

            //if(!fe.quit)
            //{
            //    equipment = fe.equipment;

            //    equipmentBar.Items.Clear();

            //    string eq = "";

            //    for (int i = 0; equipment != null && i < equipment.Length; i++)
            //    {
            //        if (i == 0)
            //            eq += equipment[i].Trim();
            //        else
            //            eq += ", " + equipment[i].Trim();
            //    }

            //    equipmentBar.Items.Add(eq);
            //    equipmentBar.SelectedIndex = 0;
            //}
        }

        private void comboBox4_MouseClick(object sender, MouseEventArgs e)
        {
            timeBar.Enabled = false;
            timeBar.Update();

            if (payTypeBar.Text == "Standby" || payTypeBar.Text == "Standby -Weekend")
            {
                MessageBox.Show("Standby records only denote you are on standby, and are always 0 hours. If you were called out while on standby, those hours go into a separate timesheet.");
            }
            else
            {
                GetTime getTime;

                if (editingTimesheetID != 0)
                    getTime = new GetTime(editingTimesheetID);
                else
                {
                    if (start == "" && end == "" && hours == 0)
                        getTime = new GetTime(forUser, dateCalendar.SelectionStart);
                    else
                        getTime = new GetTime(start, end, hours);
                }

                getTime.ShowDialog();

                if (!getTime.quit)
                {
                    hours = getTime.hours;
                    start = getTime.startTime;
                    end = getTime.endTime;

                    timeBar.Items.Clear();

                    timeBar.Items.Add(start + " - " + end + " (" + hours + " hour" + (hours == 1 ? "" : "s") + ")");
                    timeBar.SelectedIndex = 0;
                }
            }

            timeBar.Enabled = true;
            timeBar.Update();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            FindPayType fpt = new FindPayType(true);
            fpt.ShowDialog();

            if(!fpt.quit)
                payTypeBar.Text = fpt.payType;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            FindWorkOrder fwo = new FindWorkOrder(forUser);
            fwo.ShowDialog();

            if (!fwo.quit)
            {
                workOrderBar.Items.Add(fwo.woID + " - " + fwo.woDesc);
                workOrderBar.SelectedIndex = workOrderBar.Items.Count - 1;
            }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            FindEquipment fe = new FindEquipment(forUser, equipment);
            fe.ShowDialog();

            if (!fe.quit)
            {
                string str = "";

                for (int i = 0; fe.equipment != null && i < fe.equipment.Length; i++)
                {
                    str += (i == 0 ? "" : ", ") + fe.equipment[i].Trim();
                }

                equipmentBar.Items.Add(str);
                equipmentBar.SelectedIndex = equipmentBar.Items.Count - 1;

                equipment = fe.equipment;
            }
        }

        private void equipmentBar_SelectedIndexChanged(object sender, EventArgs e)
        {
            equipment = equipmentBar.Text.Split(',');
        }

        private void dateCalendar_DateSelected(object sender, DateRangeEventArgs e)
        {
            if (!Core.isDateOpen(dateCalendar.SelectionStart, Core.getSH(Core.getEmpID(forUser))) && !Core.canApprove(Core.getUsername()) && !Core.canReview(Core.getUsername()))
            {
                MessageBox.Show("The window to enter time on this day is closed." + Environment.NewLine + "Please speak to your supervisor.");

                dateCalendar.SelectionStart = DateTime.Today;
            }
            else if(editingTimesheetID == 0) //If not editing
            {
                if (payTypeBar.Text != "Standby" && payTypeBar.Text != "Standby -Weekend")
                {
                    GetTime gt = new GetTime(forUser, dateCalendar.SelectionStart);

                    hours = gt.hours;
                    start = gt.startTime;
                    end = gt.endTime;

                    timeBar.Items.Clear();

                    timeBar.Items.Add(start + " - " + end + " (" + hours + " hour" + (hours == 1 ? "" : "s") + ")");
                    timeBar.SelectedIndex = 0;
                }
            }
        }
    }
}
