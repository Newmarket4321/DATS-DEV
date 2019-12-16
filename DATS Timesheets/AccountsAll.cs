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
    public partial class AccountsAll : Form
    {
        const int VIEWMODE = 0;
        const int SELECTMODE = 1;
        int mode = VIEWMODE;
        public string selection = "";
        bool firstLoad = true;

        public AccountsAll()
        {
            InitializeComponent();

            for (int i = 0; i < listView1.Columns.Count; i++)
                listView1.Columns[i].Name = listView1.Columns[i].Text;

            SQL sql;

            if (Core.isAdmin(Core.getUsername()))
            {
                sql = new SQL("select department from department order by department asc");
                comboBox1.Items.Add("All");
            }
            else
            {
                sql = new SQL("select d.department from department d, departmentassociations da, users u where d.departmentid = da.departmentid and da.userid = u.userid and u.displayname = @USERNAME order by d.department asc");
                sql.AddParameter("@USERNAME", Core.getUsername());
            }
            DataTable dt = sql.Run();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                comboBox1.Items.Add(dt.Rows[i][0].ToString());
            }

            comboBox1.SelectedIndex = 0;
        }

        public AccountsAll(bool selectMode)
        {
            InitializeComponent();

            for (int i = 0; i < listView1.Columns.Count; i++)
                listView1.Columns[i].Name = listView1.Columns[i].Text;

            SQL sql;

            if (Core.isAdmin(Core.getUsername()))
            {
                sql = new SQL("select department from department order by department asc");
            }
            else
            {
                sql = new SQL("select d.department from department d, departmentassociations da, users u where d.departmentid = da.departmentid and da.userid = u.userid and u.displayname = @USERNAME order by d.department asc");
                sql.AddParameter("@USERNAME", Core.getUsername());
            }
            DataTable dt = sql.Run();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                comboBox1.Items.Add(dt.Rows[i][0].ToString());
            }

            comboBox1.SelectedIndex = 0;

            if(selectMode)
            {
                mode = SELECTMODE;
                button7.Visible = false;
                button8.Visible = false;
                //button1.Visible = false;
                //button1.BackColor = Color.FromArgb(164, 209, 255);
                button9.Text = "Select";
                this.Text = "Select Account";
            }
        }

        public int refresh()
        {
            SQL sql = null;

            if (comboBox1.Text != "All")
            {
                sql = new SQL(
    @"SELECT
u.userid as ID, u.displayname as Username, u.reviewer, u.approver, u.employeeid as EmpID, u.active as Active, u.admin as Admin
FROM users u, department d, departmentassociations da
WHERE u.userid = da.userid and da.departmentid = d.departmentid and d.department = @DEPARTMENT
order by u.displayname
");
                sql.AddParameter("@DEPARTMENT", comboBox1.Text);
            }
            else
            {
                sql = new SQL(
    @"SELECT
u.userid as ID, u.displayname as Username, u.reviewer, u.approver, u.employeeid as EmpID, u.active as Active, u.admin as Admin
FROM users u order by u.displayname
");
            }
            DataTable dt = sql.Run();

            listView1.Items.Clear();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string ID = dt.Rows[i]["ID"].ToString();
                string username = dt.Rows[i]["Username"].ToString();
                string employeeID = dt.Rows[i]["EmpID"].ToString();
                bool supervisor = bool.Parse(dt.Rows[i]["reviewer"].ToString());
                bool manager = bool.Parse(dt.Rows[i]["approver"].ToString());
                bool admin = bool.Parse(dt.Rows[i]["admin"].ToString());
                bool active = bool.Parse(dt.Rows[i]["active"].ToString());
                string role;
                Color roleColor;

                if(!active)
                {
                    role = "Inactive";
                    roleColor = Color.Red;
                }
                else if (admin)
                {
                    role = "Administrator";
                    roleColor = Color.FromArgb(255, 149, 0);
                }
                else if (manager)
                {
                    role = "Approver";
                    roleColor = Color.Purple;
                }
                else
                {
                    if (supervisor)
                    {
                        role = "Reviewer";
                        roleColor = Color.Blue;
                    }
                    else
                    {
                        role = "User";
                        roleColor = Color.Green;
                    }
                }
                
                ListViewItem x = new ListViewItem(new[] { username, role, employeeID });
                x.Tag = ID;
                x.SubItems[listView1.Columns["Role"].Index].ForeColor = roleColor;
                x.UseItemStyleForSubItems = false;

                //if (firstLoad)
                //{
                //    Oracle ora = new Oracle("select YAPAST from " + Core.getSchema(Core.getEnvironment()) + ".F060116 where YAAN8=:EMPID");
                //    ora.AddParameter("EMPID", employeeID);
                //    DataTable ck = ora.Run();
                //    bool showMessage = false;

                //    if (ck.Rows.Count > 0 && ck.Rows[0]["YAPAST"].ToString() == "T")
                //        showMessage = true;

                //    if (showMessage)
                //    {
                //        if (MessageBox.Show("According to JDE, " + username + " is not currently an employee." + Environment.NewLine + "Remove " + username + " from DATS?" + Environment.NewLine + Environment.NewLine + "(Tip: Hold the 'Y' or 'N' keys to speed through these messages.)", "Auto-Check", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                //            listView1.Items.Add(x);
                //        else
                //        {
                //            SQL rm = new SQL("delete from users where username=@USERNAME");
                //            rm.AddParameter("@USERNAME", username);
                //            rm.Run();

                //            Core.logHistory("Deleted account", "Account: " + username, "");
                //        }
                //    }
                //    else
                //        listView1.Items.Add(x);
                //}
                //else
                listView1.Items.Add(x);


            }

            firstLoad = false;

            for (int i = 0; i < listView1.Columns.Count; i++)
                listView1.Columns[i].Width = -2;

            return dt.Rows.Count;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (mode == VIEWMODE)
            {
                (new Account()).ShowDialog();
                refresh();
            }
            else if (mode == SELECTMODE)
            {
                if(listView1.SelectedItems.Count > 0)
                {
                    selection = listView1.SelectedItems[0].SubItems[listView1.Columns.IndexOfKey("Account")].Text;
                    this.Close();
                }
                else
                    MessageBox.Show("No user selected.");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                if (MessageBox.Show("Are you sure you want to delete " + listView1.SelectedItems[0].SubItems[listView1.Columns.IndexOfKey("Account")].Text +"'s account?", "Are you sure?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    SQL sql = new SQL("delete from users where USERID=@USERID");
                    sql.AddParameter("@USERID", listView1.SelectedItems[0].Tag.ToString());
                    sql.Run();

                    sql = new SQL("delete from departmentassociations where USERID=@USERID");
                    sql.AddParameter("@USERID", listView1.SelectedItems[0].Tag.ToString());
                    sql.Run();

                    Core.logHistory("Deleted account", "Account: " + listView1.SelectedItems[0].SubItems[listView1.Columns.IndexOfKey("Account")].Text, "");

                    refresh();
                }
            }
            else
                MessageBox.Show("No user selected.");
        }

        private void ViewAccounts_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();

            if (e.KeyCode == Keys.Delete)
                button5_Click(null, null);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                (new Account(listView1.SelectedItems[0].SubItems[listView1.Columns.IndexOfKey("Account")].Text)).ShowDialog();
                refresh();
            }
            else
                MessageBox.Show("No user selected.");
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (mode == VIEWMODE)
                button6_Click(null, null);
            else if (mode == SELECTMODE)
                button1_Click(null, null);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            firstLoad = true;
            refresh();
        }
    }
}
