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
    public partial class ReviewTimesheetsTE : Form
    {
        public ReviewTimesheetsTE()
        {
            InitializeComponent();
        }

        private void ReviewTimesheets_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        }

        private void ReviewTimesheets_Load(object sender, EventArgs e)
        {
            //SQL sql = new SQL("select u.username from users u, roles r where u.roleid = r.roleid and r.departmentid = @DEPTID and u.deleted='False' order by username");
            //sql.AddParameter("@DEPTID", Core.getDepartment());
            //DataTable dt = sql.Run();

            //for(int i = 0; i < dt.Rows.Count; i++)
            //{
            //    employeeList.Items.Add(dt.Rows[i]["username"].ToString());
            //}

            //employeeList.SelectedIndex = 0;
            //viewList.SelectedIndex = 0;

            //loadTimesheets();
        }

        private void loadTimesheets()
        {
            reviewGrid1.username = employeeList.Text;

            textBox1.Text = "";
            textBox4.Text = "";

            textBox1.Update();
            textBox4.Update();
            
            if(viewList.Text == "Last 30 days")
            {
                reviewGrid1.start = DateTime.Today.AddDays(-30);
                reviewGrid1.end = DateTime.Today.AddDays(1);
            }
            else if(viewList.Text == "This year")
            {
                reviewGrid1.start = new DateTime(DateTime.Today.Year, 1, 1);
                reviewGrid1.end = DateTime.Today.AddDays(1);
            }
            else if(viewList.Text == "All time")
            {
                reviewGrid1.start = DateTime.Today.AddYears(-20);
                reviewGrid1.end = DateTime.Today.AddYears(20);
            }
            else if (viewList.Text == "Last pay period")
            {
                SQL sql = new SQL("select [to], [from] from periods where [from]<=@DATE and [to]>=@DATE");
                sql.AddParameter("@DATE", DateTime.Today);
                DataTable dt = sql.Run();

                reviewGrid1.start = DateTime.Parse(dt.Rows[0]["from"].ToString()).AddDays(-14);
                reviewGrid1.end = DateTime.Parse(dt.Rows[0]["to"].ToString()).AddDays(-14);
            }
            else //Current pay period & fallback
            {
                SQL sql = new SQL("select [to], [from] from periods where [from]<=@DATE and [to]>=@DATE");
                sql.AddParameter("@DATE", DateTime.Today);
                DataTable dt = sql.Run();

                reviewGrid1.start = DateTime.Parse(dt.Rows[0]["from"].ToString());
                reviewGrid1.end = DateTime.Parse(dt.Rows[0]["to"].ToString());
            }

            reviewGrid1.regenerate();

            if (reviewGrid1.regularHours != 0)
                textBox1.Text = reviewGrid1.regularHours.ToString();
            
            if (reviewGrid1.otherHours != 0)
                textBox4.Text = reviewGrid1.otherHours.ToString();
        }

        private void employeeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadTimesheets();
        }

        private void prev_Click(object sender, EventArgs e)
        {
            if(employeeList.SelectedIndex > 0)
                employeeList.SelectedIndex--;
        }

        private void next_Click(object sender, EventArgs e)
        {
            if (employeeList.SelectedIndex < employeeList.Items.Count - 1)
                employeeList.SelectedIndex++;
        }

        private void viewList_SelectedIndexChanged(object sender, EventArgs e)
        {
            reviewGrid1.Focus();

            loadTimesheets();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //loadEmployees();
        }
    }
}
