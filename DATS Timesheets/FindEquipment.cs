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
    public partial class FindEquipment : Form
    {
        string forUser = "";
        public bool quit = true;
        public string[] equipment;
        const int startingHeight = 430;
        const int expandedHeight = 674;
        int startingTop = 0;

        public FindEquipment(string[] existingEquipment)
        {
            InitializeComponent();

            for (int i = 0; i < existingEquipment.Length; i++)
            {
                ListViewItem li = new ListViewItem(existingEquipment[i]);
                li.Tag = existingEquipment[i];

                listView2.Items.Add(li);
            }
            
            button1_Click(null, null);
            
            //this.Height = startingHeight;
            //startingTop = Top;
        }

        public FindEquipment(string username)
        {
            InitializeComponent();

            forUser = username;

            this.Height = startingHeight;
            startingTop = Top;

            button1_Click(null, null);
        }

        public FindEquipment(string username, string[] existingEquipment)
        {
            InitializeComponent();

            for (int i = 0; existingEquipment != null && i < existingEquipment.Length; i++)
            {
                ListViewItem li = new ListViewItem(existingEquipment[i]);
                li.Tag = existingEquipment[i];

                listView2.Items.Add(li);
            }

            if (listView2.Items.Count == 0)
            {
                this.Height = startingHeight;
                startingTop = Top;
            }

            //this.Height = startingHeight;
            //startingTop = Top;

            forUser = username;

            button1_Click(null, null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //EQ Search

            disableContinue();

            string queryString = @"
select FAAPID, FADL01

from PRODDTA.F1201

where FAAPID like '__-__%'
and FAACL9 in ('P01', 'P02', 'P03', 'P04', 'P05', 'P06', 'P07', 'P08', 'P09', 'P10')
and FAAPID not like 'RP-%' ";

            if (textBox3.Text != "")
                queryString += "and (FAAPID like '%' || :QUERY || '%' or upper(FADL01) like '%' || upper(:QUERY) || '%')";

            queryString += " order by FAAPID";

            Oracle ora = new Oracle(queryString);

            if (textBox3.Text != "")
                ora.AddParameter("QUERY", textBox3.Text);

            DataTable dt = ora.Run();

            SQL sql = new SQL(
@"SELECT
EQUIPID as EQID,
Count(EQUIPID) AS CountOfEQUIPID

FROM EquipmentTimeEntry, Timesheets, Users

WHERE
TIMESHEETID = TimeCardDetailID AND
Timesheets.EmployeeID = Users.EmployeeID AND
Users.Username = @USERNAME AND
Timesheets.DateWorked > @DATE

GROUP BY EQUIPID

ORDER BY Count(EquipmentTimeEntry.EQUIPID) DESC;

");

//            SQL sql = new SQL(
//@"SELECT
//EQUIPID as EQID,
//Count(EQUIPID) AS CountOfEQUIPID

//FROM EquipmentTimeEntry, Timesheets, Users

//WHERE
//TIMESHEETID = TimeCardDetailID AND
//TimeSheetDetail.EmployeeID = Users.EmployeeID AND
//Users.Username = @USERNAME AND
//EquipmentTimeEntry.DELETED = 'False' AND
//TimeSheetDetail.DateWorked > @DATE

//GROUP BY EQUIPID

//ORDER BY Count(EquipmentTimeEntry.EQUIPID) DESC;

//");
            sql.AddParameter("@DATE", DateTime.Now.AddDays(-60));
            sql.AddParameter("@USERNAME", forUser == "" ? Core.getUsername() : forUser);
            DataTable dt2 = sql.Run();

            dt.Columns.Add("Count");

            DataTable dtSorted = dt.Clone();
            dtSorted.Columns["Count"].DataType = Type.GetType("System.Int32");

            foreach(DataRow dr in dt.Rows)
            {
                dtSorted.ImportRow(dr);
            }
            dtSorted.AcceptChanges();

            for (int i = 0; i < dt2.Rows.Count; i++)
            {
                dtSorted.PrimaryKey = new DataColumn[] { dtSorted.Columns[0] };
                DataRow dr = dtSorted.Rows.Find(dt2.Rows[i][0].ToString());

                if (dr != null)
                {
                    dr[2] = int.Parse(dt2.Rows[i][1].ToString());
                }
            }

            DataView dv = dtSorted.DefaultView;
            dv.Sort = "count desc";
            dtSorted = dv.ToTable();

            listView1.Items.Clear();

            for (int i = 0; i < dtSorted.Rows.Count; i++)
            {
                string woID = dtSorted.Rows[i]["FAAPID"].ToString();
                string description = dtSorted.Rows[i]["FADL01"].ToString();
                string count = dtSorted.Rows[i]["Count"].ToString(); //Custom column

                ListViewItem x = new ListViewItem(new[] { woID, description });
                x.Tag = woID;
                listView1.Items.Add(x);
            }

            for (int i = 0; i < listView1.Columns.Count; i++)
                listView1.Columns[i].Width = -2;
        }

        private void listView1_ColumnClick(object sender, System.Windows.Forms.ColumnClickEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                MessageBox.Show("You haven't selected any equipment!", "Error");
            else
            {
                //woSelection = listView1.SelectedItems[0].Tag.ToString();
                this.Close();
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            button2_Click(null, null);
        }

        private void FindEquipment_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }

        private void textBox3_KeyUp(object sender, KeyEventArgs e)
        {
            button1_Click(null, null);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listView2.Items.Count > 0)
            {
                equipment = new string[listView2.Items.Count];

                for (int i = 0; i < listView2.Items.Count; i++)
                    equipment[i] = listView2.Items[i].Tag.ToString();
            }
            else
                equipment = null;

            quit = false;
            Close();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                button1.Text = "I used " + listView1.SelectedItems[0].Text.Trim();
                enableContinue();
            }
        }

        public void disableContinue()
        {
            button1.BackColor = Color.DarkGray;
            button1.Text = "Please select equipment";
            button1.Enabled = false;
        }

        public void enableContinue()
        {
            button1.BackColor = Color.FromArgb(0, 108, 255);
            button1.Enabled = true;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            listView2.Items.Clear();
            this.Height = startingHeight;
            CenterToScreen();

            button3.BackColor = Color.FromArgb(255, 50, 25);
            button3.Text = "I didn't use equipment";
            button3.Top = 324;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem li = new ListViewItem();
                li.Text = listView1.SelectedItems[0].Tag.ToString();
                li.Tag = li.Text;

                listView2.Items.Add(li);

                this.Height = expandedHeight;

                if (startingTop <= 0)
                    startingTop = Math.Max(Top,0);

                CenterToScreen();

                button3.BackColor = Color.FromArgb(0, 108, 255);
                button3.Text = "Submit";
                button3.Top = 571;
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            button1_Click_1(null, null);
        }
    }
}
