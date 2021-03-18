﻿using System;
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
    public partial class Account : Form
    {
        string oldUsername = "";
        const int NEWMODE = 0;
        const int EDITMODE = 1;
        int mode = NEWMODE;

        public Account()
        {
            InitializeComponent();
            //load_department();
            startup(Core.getUsername());
        }
        public void load_department()
        {
            
            DataTable dt = null;
            dt = SQL.Run("select * from department order by department");
            HomeDepartment.DataSource = dt;
            HomeDepartment.DisplayMember = "department";
            HomeDepartment.ValueMember = "DepartmentID";
        }

            public Account(string username)
        {
            //Edit account
            InitializeComponent();
            mode = EDITMODE;

            startup(username);

            this.Text = "Edit Account";
            button8.Text = "Save";

            textBox3.Text = username;
            oldUsername = username;
            
            SQL sql = new SQL("select u.username, u.vacmax, u.bankvacmax, u.employeeid as EmpID, u.reviewer, u.approver, u.active, u.enterstime, u.admin, u.viewonlyuser , u.home_department from users u where u.displayname = @USERNAME");
            sql.AddParameter("@USERNAME", username);
            DataTable dt = sql.Run();

            textBox2.Text = dt.Rows[0]["EmpId"].ToString();
            textBox1.Text = dt.Rows[0]["username"].ToString();
            oldUsername = dt.Rows[0]["username"].ToString();

            string dept = "";
            if (dt.Rows[0]["home_department"].ToString() != "")
            {
                sql = new SQL("select department from department where DepartmentID = @DEPARTMENTID");
                sql.AddParameter("@DEPARTMENTID", dt.Rows[0]["home_department"].ToString());
                dept = sql.Run().Rows[0]["Department"].ToString();
            }
            HomeDepartment.Text = dept;
            bool reviewer = bool.Parse(dt.Rows[0]["reviewer"].ToString());
            bool approver = bool.Parse(dt.Rows[0]["approver"].ToString());
            bool admin = bool.Parse(dt.Rows[0]["admin"].ToString());
            bool active = bool.Parse(dt.Rows[0]["active"].ToString());
            bool entersTime = bool.Parse(dt.Rows[0]["enterstime"].ToString());
            bool viewonly = false;
            if (dt.Rows[0]["viewonlyuser"].ToString() != null)
            {
                viewonly = bool.Parse(dt.Rows[0]["viewonlyuser"].ToString());
            }
            if (!active)
                InactiveUser.Checked = true;
            else if (admin)
            {
                if (reviewer == true && entersTime == true)
                {
                    Enter_Review_Timesheet.Checked = true;
                }
                else if (approver == true && entersTime == true)
                {
                    Enter_Approve_Timesheets.Checked = true;
                }
                else if (reviewer == true)
                    ReviewsTimesheets.Checked = true;
                else if (approver == true)
                    ApprovesTimesheets.Checked = true;
                
                Admin.Checked = true;
            }
            else if (approver)
            {
                if (entersTime)
                    Enter_Approve_Timesheets.Checked = true;
                else
                    ApprovesTimesheets.Checked = true;
            }
            else if (reviewer)
            {
                if (entersTime)
                    Enter_Review_Timesheet.Checked = true;
                else
                    ReviewsTimesheets.Checked = true;
            }
            else if (viewonly)
            {
                Viewonlyuser.Checked = true;
            }
            else
                EntersTimesheets.Checked = true;


          
        }

        private void startup(string username)
        {
          
          
         
            if (!Core.canApprove(Core.getUsername()) && !Core.canApprove(username)) //Neither of you can
            {
                ApprovesTimesheets.Enabled = false;
                Enter_Approve_Timesheets.Enabled = false;
            }
            else if (!Core.canApprove(Core.getUsername()) && Core.canApprove(username)) //They are above you
            {
                InactiveUser.Enabled = false;
                EntersTimesheets.Enabled = false;
                Enter_Review_Timesheet.Enabled = false;
                ReviewsTimesheets.Enabled = false;
                ApprovesTimesheets.Enabled = false;
                Enter_Approve_Timesheets.Enabled = false;
                Viewonlyuser.Enabled = false;
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                checkedListBox1.Enabled = false;
                button8.Enabled = false;
            }

            if (Core.isAdmin(Core.getUsername()))
            {
                DataTable dt = SQL.Run("select department from department order by department");
                
                for(int i = 0; i < dt.Rows.Count; i++)
                    checkedListBox1.Items.Add(dt.Rows[i]["department"].ToString());
            }
            else
            {
               
                Admin.Enabled = false;
                SQL sql = new SQL("select department from department, departmentassociations da, users u where department != 'All' and department.departmentid = da.departmentid and da.userid = u.userid and (u.displayname = @THEIRUSERNAME or u.displayname = @YOURUSERNAME) group by department order by department");
                sql.AddParameter("@THEIRUSERNAME", username);
                sql.AddParameter("@YOURUSERNAME", Core.getUsername());
                DataTable dt = sql.Run();

                for (int i = 0; i < dt.Rows.Count; i++)
                    checkedListBox1.Items.Add(dt.Rows[i]["department"].ToString());

                //Operations Bundle Pack
                if (checkedListBox1.Items.Count > 0 && (checkedListBox1.Items.Contains("Parks") ||
                    checkedListBox1.Items.Contains("Roads") ||
                    checkedListBox1.Items.Contains("Water") ||
                    checkedListBox1.Items.Contains("Facilities - Operations") ||
                    checkedListBox1.Items.Contains("Facilities - Part Time") ||
                    checkedListBox1.Items.Contains("Facilities Maintenance") ||
                    checkedListBox1.Items.Contains("Crossing Guards") ||
                    checkedListBox1.Items.Contains("Fleet") ||
                    checkedListBox1.Items.Contains("View Only"))) 
                {
                    if (!checkedListBox1.Items.Contains("Crossing Guards"))
                        checkedListBox1.Items.Add("Crossing Guards");
                    if (!checkedListBox1.Items.Contains("Facilities - Operations"))
                        checkedListBox1.Items.Add("Facilities - Operations");
                    if (!checkedListBox1.Items.Contains("Facilities - Part Time"))
                        checkedListBox1.Items.Add("Facilities - Part Time");
                    if (!checkedListBox1.Items.Contains("Facilities Maintenance"))
                        checkedListBox1.Items.Add("Facilities Maintenance");
                    if (!checkedListBox1.Items.Contains("Fleet"))
                        checkedListBox1.Items.Add("Fleet");
                    if (!checkedListBox1.Items.Contains("Parks"))
                        checkedListBox1.Items.Add("Parks");
                    if (!checkedListBox1.Items.Contains("Roads"))
                        checkedListBox1.Items.Add("Roads");
                    if (!checkedListBox1.Items.Contains("Water"))
                        checkedListBox1.Items.Add("Water");
                    if (!checkedListBox1.Items.Contains("View Only"))
                        checkedListBox1.Items.Add("View Only");
                }
            }

            //Select departments the user is part of as default
            for (int i = 0; i < checkedListBox1.Items.Count && mode == EDITMODE; i++)
            {
                string dept = checkedListBox1.Items[i].ToString();
                bool partOfDept = false;

                SQL sql = new SQL("select department from department d, departmentassociations da, users u where d.department = @DEPARTMENT and d.departmentid = da.departmentid and da.userid = u.userid and u.displayname=@USERNAME");
                sql.AddParameter("@DEPARTMENT", dept);
                sql.AddParameter("@USERNAME", username);
               
                if (sql.Run().Rows.Count > 0)
                    partOfDept = true;

                if (partOfDept)
                    checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                
            }

            EntersTimesheets.Checked = true;

            DataTable dt1 = null;
            dt1 = SQL.Run("select * from department order by department");
            //DepartmentList.Text = "--select--";
            
                HomeDepartment.DataSource = dt1;
                HomeDepartment.DisplayMember = "department";
                HomeDepartment.ValueMember = "DepartmentID";
                HomeDepartment.SelectedItem = null;
         
        }

        private void button2_Click(object sender, EventArgs e)
        {

            if (textBox1.Text == "") //Username can't be blank
            {
                MessageBox.Show("Please enter a computer logon.");
                return;
            }

            try
            {
                if (!Core.isEmpIDValid(textBox2.Text)) //Employee ID is not valid
                {
                    MessageBox.Show("Employee ID invalid. Ensure it was entered correctly.\n\nIf the employee is new, it could be that HR hasn't entered them into JDE yet. If you believe this to be the case, please contact HR.");
                    return;
                }
            }
            catch (System.TypeInitializationException)
            {
                MessageBox.Show("This component requires Oracle to be installed. Please call IT to have this done.", "Error");
                return;
            }

            if (Core.isEmployeeIDTaken(int.Parse(textBox2.Text)) && mode == NEWMODE) //Employee ID can't be taken
            {
                MessageBox.Show("That employee ID is already associated with a DATS account.");
                return;
            }

            if (Core.isUsernameTaken(textBox1.Text) && mode == NEWMODE) //Username can't be taken
            {
                MessageBox.Show("That computer logon already has an account.");
                return;
            }

            if (checkedListBox1.CheckedItems.Count == 0) //No departments are selected
            {
                MessageBox.Show("Please select a department.");
                return;
            }

            if (textBox2.Text == "") //Employee ID is blank
            {
                MessageBox.Show("Please enter an employee ID.");
                return;
            }


            //bool canReview = radioButton3.Checked || radioButton4.Checked || radioButton5.Checked || radioButton6.Checked;
            //bool canApprove = radioButton5.Checked || radioButton6.Checked;
            //bool entersTime = radioButton2.Checked || radioButton3.Checked || radioButton6.Checked;
            bool canReview = Enter_Review_Timesheet.Checked || ReviewsTimesheets.Checked || ApprovesTimesheets.Checked || Enter_Approve_Timesheets.Checked;
            bool canApprove = ApprovesTimesheets.Checked || Enter_Approve_Timesheets.Checked;
            bool entersTime = EntersTimesheets.Checked || Enter_Review_Timesheet.Checked || Enter_Approve_Timesheets.Checked;
            bool admin = Admin.Checked;
            bool active = !InactiveUser.Checked;
            bool viewonly = Viewonlyuser.Checked;

           DataTable dtdiv = SQL.Run(@"SELECT Division from Department where Department = @dept", HomeDepartment.Text.Trim());
            if (mode == NEWMODE)
            {

                //Create user
                SQL sql = new SQL("INSERT INTO Users VALUES (@Username, @DisplayName, @EmpID, @EmpType, @CanReview, @CanApprove, @VacMax, @BankVacMax, @Active, @PriorVacMax, @EntersTime, @Admin, @Viewonlyuser,@HomeDepartment)");
                sql.AddParameter("@Username", textBox1.Text);
                sql.AddParameter("@DisplayName", textBox3.Text);
                sql.AddParameter("@EmpID", textBox2.Text);
                sql.AddParameter("@EmpType", "");
                sql.AddParameter("@CanReview", canReview);
                sql.AddParameter("@CanApprove", canApprove);
                sql.AddParameter("@VacMax", 0);
                sql.AddParameter("@BankVacMax", 0);
                sql.AddParameter("@Active", active);
                sql.AddParameter("@PriorVacMax", 0);
                sql.AddParameter("@EntersTime", entersTime);
                sql.AddParameter("@Admin", admin);
                sql.AddParameter("@Viewonlyuser", viewonly); 
                if (dtdiv.Rows.Count > 0)
                {

                    if (checkedListBox1.CheckedItems.Contains(HomeDepartment.Text.Trim()))
                    {
                        sql.AddParameter("@HomeDepartment", HomeDepartment.SelectedValue.ToString());
                      
                        sql.Run();

                    }
                    else
                    {
                        MessageBox.Show("Selected home department is not in selected Departments list.");
                        return;
                    }


                }
                else
                {
                    if (checkedListBox1.CheckedItems.Count == 1)
                    {
                        string deptID = "";
                        for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
                        {
                            SQL sql1 = new SQL("select departmentid from department where department = @DEPARTMENT");
                            sql1.AddParameter("@DEPARTMENT", checkedListBox1.CheckedItems[i].ToString());

                            deptID = sql1.Run().Rows[0]["departmentid"].ToString();
                            sql.AddParameter("@HomeDepartment", deptID);
                        }
                    }
                    else
                        sql.AddParameter("@HomeDepartment", "");
                    sql.Run();

                }



                //Grab userID
                sql = new SQL("select userid from users where username=@USERNAME");
                sql.AddParameter("@USERNAME", textBox1.Text);
                string userID = sql.Run().Rows[0]["userid"].ToString();

                //Update departments
                for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
                {
                    sql = new SQL("select departmentid from department where department = @DEPARTMENT");
                    sql.AddParameter("@DEPARTMENT", checkedListBox1.CheckedItems[i].ToString());
                    string departmentID = sql.Run().Rows[0]["departmentid"].ToString();

                    sql = new SQL("insert into departmentassociations values (@USERID, @DEPARTMENTID)");
                    sql.AddParameter("@USERID", userID);
                    sql.AddParameter("@DEPARTMENTID", departmentID);
                    sql.Run();
                }
                PayrollExport.updateEmployeeTypes();

                Core.logHistory("Created account", textBox1.Text + " (" + textBox2.Text + ")", "");
            }
            else if (mode == EDITMODE)
            {
               
                //Remember what it started as
                SQL sql = new SQL("select * from users where username = @USERNAME");
                sql.AddParameter("@USERNAME", oldUsername);
                DataTable before = sql.Run();

                //Update the user
                sql = new SQL(@"
UPDATE Users

SET Username=@USERNAME,
DisplayName=@DISPLAYNAME,
reviewer=@CANREVIEW,
approver=@CANAPPROVE,
EmployeeID=@EMPLOYEEID,
Active=@ACTIVE,
enterstime=@ENTERSTIME,
admin=@ADMIN,
viewonlyuser=@Viewonlyuser,
Home_Department = @HomeDepartment
where userid=@USERID");
                sql.AddParameter("@USERNAME", textBox1.Text);
                sql.AddParameter("@DISPLAYNAME", textBox3.Text);
                sql.AddParameter("@CANREVIEW", canReview);
                sql.AddParameter("@CANAPPROVE", canApprove);
                sql.AddParameter("@EMPLOYEEID", textBox2.Text);
                sql.AddParameter("@USERID", before.Rows[0]["userid"].ToString());
                sql.AddParameter("@ACTIVE", active);
                sql.AddParameter("@ENTERSTIME", entersTime);
                sql.AddParameter("@ADMIN", admin);
                sql.AddParameter("@Viewonlyuser", viewonly);

              
                if (dtdiv.Rows.Count > 0 )
                {
                    if (checkedListBox1.CheckedItems.Contains(HomeDepartment.Text.Trim()))
                    {
                        sql.AddParameter("@HomeDepartment", HomeDepartment.SelectedValue.ToString());
                        
                       sql.Run();

                    }
                    else
                    {
                        MessageBox.Show("Selected home department is not in selected Departments list !");
                        return;
                    }
                   
                }
                else
                {
                    string deptID  ="";
                    if (checkedListBox1.CheckedItems.Count == 1)
                    {
                        for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
                        {
                          SQL  sql1 = new SQL("select departmentid from department where department = @DEPARTMENT");
                            sql1.AddParameter("@DEPARTMENT", checkedListBox1.CheckedItems[i].ToString());
                           
                           deptID = sql1.Run().Rows[0]["departmentid"].ToString();
                            sql.AddParameter("@HomeDepartment", deptID);
                     
                               
                        }
                    }
                    else
                        sql.AddParameter("@HomeDepartment", "");
                    sql.Run();
                    
                }
              

                //Grab userID
                sql = new SQL("select userid from users where username=@USERNAME");
                sql.AddParameter("@USERNAME", textBox1.Text);
                string userID = sql.Run().Rows[0]["userid"].ToString();

                //Delete existing departments
                sql = new SQL("delete from departmentassociations where userid=@USERID");
                sql.AddParameter("@USERID", userID);
                sql.Run();

                //Update departments
                for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
                {
                   
                    sql = new SQL("select departmentid from department where department = @DEPARTMENT");
                    sql.AddParameter("@DEPARTMENT", checkedListBox1.CheckedItems[i].ToString());
                    string departmentID = sql.Run().Rows[0]["departmentid"].ToString();

                    sql = new SQL("insert into departmentassociations values (@USERID, @DEPARTMENTID)");
                    sql.AddParameter("@USERID", userID);
                    sql.AddParameter("@DEPARTMENTID", departmentID);
                    sql.Run();
                }

                //Remember what it is after
                sql = new SQL("select * from users where username = @USERNAME");
                sql.AddParameter("@USERNAME", textBox1.Text);
                DataTable after = sql.Run();

                //Compare before and after
                string beforeString = "", afterString = "";

                for (int i = 0; i < before.Columns.Count; i++)
                {
                    if (before.Rows[0][i].ToString() != after.Rows[0][i].ToString())
                    {
                        beforeString += (beforeString == "" ? "" : ", ") + before.Columns[i].ColumnName.ToLower() + "=" + before.Rows[0][i].ToString();

                        afterString += (afterString == "" ? "" : ", ") + after.Columns[i].ColumnName.ToLower() + "=" + after.Rows[0][i].ToString();
                    }
                }

                string departments = "";

                for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
                    departments += (departments == "" ? "" : ", ") + checkedListBox1.CheckedItems[i].ToString();

                afterString += (afterString == "" ? "" : ", ") + "departments" + "=" + departments;

                Core.logHistory("Edited account details", "Account: " + oldUsername + " - " + (beforeString == "" ? "No changes" : beforeString), afterString);
            }

            //DataTable dt  = SQL.Run("select * from users where EMPLOYEEID = @EMPLOYEEID and Home_Department != '' " , textBox2.Text);
            
            //if(dt.Rows.Count == 0)
            //{

            //   Home_Deprtment homedept = new Home_Deprtment(int.Parse(textBox2.Text));
            // //  homedept.ShowDialog();
            //}
            
            this.Close();
        }

        private void NewAccount_Load(object sender, EventArgs e)
        {

        }

        private void NewAccount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
            if (e.KeyCode == Keys.Enter)
                button2_Click(null, null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(Core.isAdmin(Core.getUsername()))
            {
                Entitlements ent = new Entitlements(int.Parse(textBox2.Text));
                ent.ShowDialog();
            }
            else
            {
                MessageBox.Show("You do not have the rights view entitlements");
            }
            
        }

        private void InactiveUser_CheckedChanged(object sender, EventArgs e)
        {
            if (InactiveUser.Checked == true)
            {
                Admin.Enabled = false;
                Admin.Checked = false;
            }
            else
                Admin.Enabled = true;
        }

        private void Viewonlyuser_CheckedChanged(object sender, EventArgs e)
        {
            if (Viewonlyuser.Checked == true)
            {
                Admin.Enabled = false;
                Admin.Checked = false;
            }
            else
                Admin.Enabled = true;
        }
    }
}
