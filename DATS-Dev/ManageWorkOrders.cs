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
    public partial class ManageWorkOrders : Form
    {
        public int deptID = -1;

        public ManageWorkOrders(int departmentID)
        {
            InitializeComponent();

            deptID = departmentID;

            string deptDesc = SQL.Run("select department from department where departmentid = " + deptID.ToString()).Rows[0][0].ToString();

            label2.Text = "Work Orders for " + deptDesc;

            try
            {
                Oracle ora = new Oracle("Type initialization test.");
            }
            catch (System.TypeInitializationException e2)
            {
                MessageBox.Show("This component requires Oracle to be installed. Please call IT to have this done.", "Error");
                Close();
            }

            SQL sql = new SQL("select workorder, description from departmentworkorders where departmentid = @DEPARTMENTID order by workorder");
            sql.AddParameter("@DEPARTMENTID", deptID);
            DataTable dt = sql.Run();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string woID = dt.Rows[i]["workorder"].ToString();
                string description = dt.Rows[i]["description"].ToString();

                ListViewItem x = new ListViewItem(new[] { woID, description });
                x.Tag = woID;
                listView2.Items.Add(x);
            }

            for (int i = 0; i < listView2.Columns.Count; i++)
                listView2.Columns[i].Width = -2;

            button2.Enabled = false;
            button2.BackColor = Color.DarkGray;
            button2.Text = "Please select a work order";
        }

        private void textBox3_KeyUp(object sender, KeyEventArgs e)
        {
            //WO Search
            string queryString = "";

            if (textBox3.Text != "")
                queryString = "select WADOCO, WADL01 from " + Core.getSchema(Core.getEnvironment()) + ".F4801 where (WADOCO like '%' || :QUERY || '%' or upper(WADL01) like '%' || :QUERY || '%') and WASRST <> '99' order by WADOCO";
            else
                queryString = "select WADOCO, WADL01 from " + Core.getSchema(Core.getEnvironment()) + ".F4801 where WASRST <> '99' order by WADOCO";

            Oracle ora = new Oracle(queryString);
            ora.AddParameter("QUERY", textBox3.Text.ToUpper());
            DataTable dt = ora.Run();

            listView1.Items.Clear();

            button2.Enabled = false;
            button2.BackColor = Color.DarkGray;
            button2.Text = "Please select a work order";

            for (int i = 0; i < Math.Min(dt.Rows.Count, 100); i++)
            {
                string woID = dt.Rows[i]["WADOCO"].ToString();
                string description = dt.Rows[i]["WADL01"].ToString();

                if (textBox3.Text != "")
                {
                    ListViewItem x = new ListViewItem(new[] { woID, description });
                    x.Tag = woID;
                    listView1.Items.Add(x);
                }
            }

            if (listView1.Items.Count == 1)
                listView1.Items[0].Selected = true;

            for (int i = 0; i < listView1.Columns.Count; i++)
                listView1.Columns[i].Width = -2;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for(int i = 0; i < listView1.SelectedItems.Count; i++)
            {
                try //In case of duplicates
                {
                    string woID = listView1.SelectedItems[i].Tag.ToString();
                    string description = listView1.SelectedItems[i].SubItems[1].Text.Trim();

                    SQL sql = new SQL("insert into departmentworkorders values (@DEPARTMENTID, @WORKORDER, @DESCRIPTION)");
                    sql.AddParameter("@DEPARTMENTID", deptID);
                    sql.AddParameter("@WORKORDER", woID);
                    sql.AddParameter("@DESCRIPTION", description);
                    sql.Run();

                    ListViewItem x = new ListViewItem(new[] { woID, description });
                    x.Tag = woID;
                    listView2.Items.Add(x);

                    Core.logHistory("Added work order", woID, "");
                }
                catch
                {

                }
            }

            for (int i = 0; i < listView2.Columns.Count; i++)
                listView2.Columns[i].Width = -2;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            while(listView2.SelectedItems.Count > 0)
            {
                SQL sql = new SQL("delete from departmentworkorders where workorder = @WORKORDER and departmentid = @DEPARTMENTID");
                sql.AddParameter("@WORKORDER", listView2.SelectedItems[0].Tag.ToString());
                sql.AddParameter("@DEPARTMENTID", deptID);
                sql.Run();

                listView2.SelectedItems[0].Remove();
            }

            for (int i = 0; i < listView2.Columns.Count; i++)
                listView2.Columns[i].Width = -2;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                button2.Enabled = true;
                button2.BackColor = Color.FromArgb(166, 204, 20);
                button2.Text = "Add to DATS";
            }
            else
            {
                button2.Enabled = false;
                button2.BackColor = Color.DarkGray;
                button2.Text = "Please select a work order";
            }
        }

        private void ManageWorkOrders_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
            if (e.KeyCode == Keys.Enter && textBox3.Focused && button2.Enabled == true)
                button2_Click(null, null);
        }
    }
}
