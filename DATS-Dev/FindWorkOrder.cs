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
    public partial class FindWorkOrder : Form
    {
        public string woID = null;
        public string woDesc = null;
        string forUser = Core.getUsername();
        bool useSQL;
        public bool quit = true;
        public string department;
        public string JDEdepartment = "";

        public FindWorkOrder()
        {
            InitializeComponent();

            forUser = Core.getUsername();
            //SQL sql = new SQL("SELECT * FROM[DATS].[dbo].[Users] inner join DepartmentAssociations on DepartmentAssociations.userID = users.userid Inner join Department on department.DepartmentID = DepartmentAssociations.DepartmentID where username = @USERNAME");
            SQL sql = new SQL("select * from departmentworkorders where departmentid in (select d.departmentid from departmentassociations d, users u where d.userid = u.userid and u.displayname = @USERNAME)");
            sql.AddParameter("@USERNAME", forUser);
            useSQL = sql.Run().Rows.Count > 0;
            //department = sql.Run().Rows
            var sqluser = sql.Run();
            if (forUser == "Bertuzzi, Dan")
                useSQL = false;

            //button1_Click(null, null);

            textBox3.Select();

            disableContinue();
        }

        public FindWorkOrder(string username)
        {
            InitializeComponent();

            forUser = username;

            //SQL sql = new SQL("SELECT * FROM[DATS].[dbo].[Users] inner join DepartmentAssociations on DepartmentAssociations.userID = users.userid Inner join Department on department.DepartmentID = DepartmentAssociations.DepartmentID where username = @USERNAME");

            SQL sql = new SQL("select * from departmentworkorders where departmentid in (select d.departmentid from departmentassociations d, users u where d.userid = u.userid and u.displayname = @USERNAME)");
            sql.AddParameter("@USERNAME", forUser);
            useSQL = sql.Run().Rows.Count > 0;
            var sqluser = sql.Run();
            if(sqluser.Rows.Count == 0)
            {
                department = "";
            }
            else
            {
                department = sqluser.Rows[0][0].ToString();
            }
            
            if (department == "1"){
                JDEdepartment = "60";
            }
            else if (department == "2"){
                JDEdepartment = "80";
            }
            else if(department == "3"){
                JDEdepartment = "40";
            }
            else if(department == "4"){
                JDEdepartment = "20";
            }
            else if(department == "5"){
                JDEdepartment = "10";
            }
            else if(department == "6"){
                JDEdepartment = "";
            }
            else if(department == "7"){
                JDEdepartment = "";
            }
            else if(department == "8"){
                JDEdepartment = "";
            }
            else if (department == "9"){
                JDEdepartment = "";
            }
            else if (department == "10"){
                JDEdepartment = "";
            }
            else if (department == "11"){
                JDEdepartment = "";
            }
            else if (department == "12"){
                JDEdepartment = "";
            }
            else if (department == "13"){
                JDEdepartment = "";
            }
            else if (department == "18"){
                JDEdepartment = "";
            }
            else if (department == "19"){
                JDEdepartment = "";
            }
            else if (department == "21"){
                JDEdepartment = "85";
            }
            else if (forUser == "Schritt, Craig")
            {
                JDEdepartment = "85";
                useSQL = true;
               // MessageBox.Show(forUser + " " + JDEdepartment);
            }
            else if (department == "22"){
                JDEdepartment = "85";
            }
            else{
                JDEdepartment = "";
            }

            if (forUser == "Bertuzzi, Dan")
                useSQL = false;

            button1_Click(null, null);
        }

        public void disableContinue()
        {
            button1.BackColor = Color.DarkGray;
            button1.Text = "Please select a work order";
            button1.Enabled = false;
        }

        public void enableContinue()
        {
            button1.BackColor = Color.FromArgb(0, 108, 255);
            button1.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //WO Search
            DataTable dt = new DataTable();
            SQL sql;

            disableContinue();

            if (useSQL)
            {
                string queryString = "";

                if (textBox3.Text != "")
                    queryString = @"
select
workorder as WADOCO,
min(description) as WADL01

from departmentworkorders

where
(workorder like '%' + @QUERY + '%'
or upper(description) like '%' + @QUERY + '%')
and departmentid in

(select d.departmentid
from departmentassociations d,
users u
where d.userid = u.userid
and u.displayname = @USERNAME)

group by workorder
order by workorder";
                else
                    queryString = @"
select
workorder as WADOCO,
min(description) as WADL01

from departmentworkorders

where departmentid in (select d.departmentid from departmentassociations d, users u where d.userid = u.userid and u.displayname = @USERNAME) group by workorder order by workorder";

                sql = new SQL(queryString);
                sql.AddParameter("@QUERY", textBox3.Text.ToUpper());
                sql.AddParameter("@USERNAME", forUser);
                dt = sql.Run();
                if (sql.Run().Rows.Count == 0) {
                    string queryString1 = "";

                    if (textBox3.Text != "")
                        queryString1 = "select WADOCO, WADL01, WAVR01 from " + Core.getSchema(Core.getEnvironment()) + ".F4801 where (WADOCO like '%' || :QUERY || '%' or upper(WADL01) like '%' || :QUERY || '%' or upper(WAVR01) like '%' || :QUERY || '%') and WAWR06 = '" + JDEdepartment.ToString().ToUpper() + "' and WASRST <> '99' order by WADOCO";
                    else
                        queryString1 = "select WADOCO, WADL01, WAVR01 from " + Core.getSchema(Core.getEnvironment()) + ".F4801 where WASRST <> '99' and upper(WAWR06) = '" + JDEdepartment.ToString().ToUpper() + "' order by WADOCO";

                    Oracle ora = new Oracle(queryString1);
                    ora.AddParameter("QUERY", textBox3.Text.ToUpper());
                    dt = ora.Run();

                }
                
            }
            else
            {
                string queryString = "";

                if (textBox3.Text != "")
                    queryString = "select WADOCO, WADL01, WAVR01 from " + Core.getSchema(Core.getEnvironment()) + ".F4801 where (WADOCO like '%' || :QUERY || '%' or upper(WADL01) like '%' || :QUERY || '%' or upper(WAVR01) like '%' || :QUERY || '%') and WASRST <> '99' order by WADOCO";
                else
                    queryString = "select WADOCO, WADL01, WAVR01 from " + Core.getSchema(Core.getEnvironment()) + ".F4801 where WASRST <> '99' order by WADOCO";

                Oracle ora = new Oracle(queryString);
                ora.AddParameter("QUERY", textBox3.Text.ToUpper());
                dt = ora.Run();
            }


            sql = new SQL(
@"SELECT
Timesheets.WorkOrder as WOID,
Count(Timesheets.WorkOrder) AS CountOfWorkOrder

FROM Timesheets, Users

WHERE
Timesheets.DateWorked > @DATE AND
Timesheets.WorkOrder IS NOT NULL AND
Users.EmployeeID = Timesheets.EmployeeID AND
Users.displayname = @USERNAME

GROUP BY
Timesheets.WorkOrder

ORDER BY Count(Timesheets.WorkOrder) DESC;

");
            sql.AddParameter("@DATE", DateTime.Now.AddDays(-60));
            sql.AddParameter("@USERNAME", forUser);
            DataTable dt2 = sql.Run();

            dt.Columns.Add("Count");

            DataTable dtSorted = dt.Clone();
            dtSorted.Columns["Count"].DataType = Type.GetType("System.Int32");

            foreach (DataRow dr in dt.Rows)
            {
                dtSorted.ImportRow(dr);
            }
            dtSorted.AcceptChanges();

            for (int i = 0; i < dt2.Rows.Count; i++)
            {
                dtSorted.PrimaryKey = new DataColumn[] { dtSorted.Columns[0] };
                DataRow dr = dtSorted.Rows.Find(dt2.Rows[i][0].ToString());

                if (dr != null)
                    dr[2] = dt2.Rows[i][1].ToString();
            }

            DataView dv = dtSorted.DefaultView;
            dv.Sort = "count desc";
            dtSorted = dv.ToTable();

            listView1.Items.Clear();

            for (int i = 0; i < Math.Min(dtSorted.Rows.Count, 500); i++)
            {
                string workorder = dtSorted.Rows[i]["WADOCO"].ToString();
                string description = dtSorted.Rows[i]["WADL01"].ToString().Trim();
                string count = dtSorted.Rows[i]["Count"].ToString(); //Custom column

                Oracle ora = new Oracle("select WAVR01 from " + Core.getSchema(Core.getEnvironment()) + ".F4801 where WADOCO=@WORKORDER and WASRST <> '99'");
                ora.AddParameter("@WORKORDER", workorder);
                dt = ora.Run();

                if (dt.Rows.Count > 0)
                {
                    string asset = dt.Rows[0]["WAVR01"].ToString().Trim();

                    if (asset != "")
                        description += " (" + asset + ")";



                    if (count != "" || (textBox3.Text != "" && !useSQL) || useSQL)
                    {
                        ListViewItem x = new ListViewItem(new[] { workorder, description });
                        x.Tag = workorder;
                        listView1.Items.Add(x);
                    }
                }
            }

            if (listView1.Items.Count == 1)
                listView1.SelectedIndices.Add(0);

            for (int i = 0; i < listView1.Columns.Count; i++)
                listView1.Columns[i].Width = -2;
        }

        private void listView1_ColumnClick(object sender, System.Windows.Forms.ColumnClickEventArgs e)
        {

        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            button1_Click_1(null, null);
        }

        private void FindWorkOrder_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
            if (e.KeyCode == Keys.Enter)
                button1_Click(null, null);
        }

        private void textBox3_KeyUp(object sender, KeyEventArgs e)
        {
            //button1_Click(null, null);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                button1.Text = "I used #" + listView1.SelectedItems[0].Text;
                enableContinue();
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                MessageBox.Show("You haven't selected a work order!", "Error");
            else
            {
                woID = listView1.SelectedItems[0].Tag.ToString();
                woDesc = listView1.SelectedItems[0].SubItems[1].Text;
                quit = false;
                this.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            quit = false;
            this.Close();
        }

        private void FindWorkOrder_Load(object sender, EventArgs e)
        {
            textBox3.Select();
        }
    }
}
