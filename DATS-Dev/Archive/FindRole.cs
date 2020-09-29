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
    public partial class FindRole : Form
    {
        public string role;

        public FindRole()
        {
            InitializeComponent();

            for (int i = 0; i < listView1.Columns.Count; i++)
                listView1.Columns[i].Name = listView1.Columns[i].Text;

            SQL sql;

            if (Core.canApprove(Core.getUsername()))
            {
                sql = new SQL("SELECT r.[RoleID] as ID, r.[RoleDescription] as \"Description\", r.manager as Supervisor, r.approver as Manager FROM [Roles] r, department d, roles r2, users emp where r.departmentid = d.departmentid and d.departmentid = r2.departmentid and r2.roleid = emp.roleid and emp.username = @USERNAME order by r.sort");
            }
            else
            {
                sql = new SQL("SELECT r.[RoleID] as ID, r.[RoleDescription] as \"Description\", r.manager as Supervisor, r.approver as Manager FROM [Roles] r, department d, roles r2, users emp where r.departmentid = d.departmentid and d.departmentid = r2.departmentid and r2.roleid = emp.roleid and emp.username = @USERNAME and r.approver = 'False' order by r.sort");
                listView1.Columns.RemoveByKey("Can Approve");
            }
            
            sql.AddParameter("@USERNAME", Core.getUsername());
            DataTable dt = sql.Run();

            listView1.Items.Clear();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string ID = dt.Rows[i]["ID"].ToString();
                string description = dt.Rows[i]["Description"].ToString();
                bool supervisor = bool.Parse(dt.Rows[i]["Supervisor"].ToString());
                bool manager = bool.Parse(dt.Rows[i]["Manager"].ToString());
                string rank;
                Color rankColor;

                if (manager)
                {
                    rank = "Manager";
                    rankColor = Color.Purple;
                }
                else
                {
                    if (supervisor)
                    {
                        rank = "Supervisor";
                        rankColor = Color.Blue;
                    }
                    else
                    {
                        rank = "Employee";
                        rankColor = Color.Green;
                    }
                }

                ListViewItem x = new ListViewItem(new[] { description, (supervisor ? "Yes" : "No"), (supervisor ? "Yes" : "No"), (manager ? "Yes" : "No") });
                x.Tag = ID;
                x.SubItems[listView1.Columns["Can Create Accounts"].Index].ForeColor = (supervisor ? Color.Green : Color.Red);
                x.SubItems[listView1.Columns["Can Review"].Index].ForeColor = (supervisor ? Color.Green : Color.Red);

                if (Core.canApprove(Core.getUsername()))
                    x.SubItems[listView1.Columns["Can Approve"].Index].ForeColor = (manager ? Color.Green : Color.Red);
                //x.SubItems[listView1.Columns["Description"].Index].ForeColor = rankColor;
                x.UseItemStyleForSubItems = false;
                listView1.Items.Add(x);
            }

            for (int i = 0; i < listView1.Columns.Count; i++)
                listView1.Columns[i].Width = -2;

            //return dt.Rows.Count;
        }

        private void FindRole_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }
    }
}
