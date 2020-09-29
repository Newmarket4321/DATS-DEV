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
    public partial class GetDepartment : Form
    {
        public int deptID = -1;
        public bool quit = true;

        public GetDepartment()
        {
            InitializeComponent();
            SQL sql;

            if (Core.isAdmin(Core.getUsername()))
            {
                sql = new SQL("select departmentid, department from department order by department");
            }
            else
            {
                sql = new SQL("select d.departmentid, d.department from department d, departmentassociations da, users u where d.departmentid = da.departmentid and da.userid = u.userid and u.displayname = @USERNAME order by department");
                sql.AddParameter("@USERNAME", Core.getUsername());
            }
            DataTable dt = sql.Run();

            for(int i = 0; i < dt.Rows.Count; i++)
            {
                ListViewItem li = new ListViewItem(dt.Rows[i]["department"].ToString());
                li.Tag = dt.Rows[i]["departmentid"].ToString();

                listView1.Items.Add(li);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 0)
            {
                deptID = int.Parse(listView1.SelectedItems[0].Tag.ToString());
                quit = false;

                Close();
            }
            else
                MessageBox.Show("Please select a department.");
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count != 0)
            {
                deptID = int.Parse(listView1.SelectedItems[0].Tag.ToString());
                quit = false;

                Close();
            }
        }

        private void GetDepartment_Load(object sender, EventArgs e)
        {
            if (listView1.Items.Count == 1)
            {
                deptID = int.Parse(listView1.Items[0].Tag.ToString());
                quit = false;
                Close();
            }
        }

        private void GetDepartment_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }
    }
}
