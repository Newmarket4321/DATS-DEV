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
    public partial class TimeEntry : Form
    {
        public TimeEntry()
        {
            InitializeComponent();

            loadUsers();
            loadPeriods();
            loadPayTypes();
        }

        private void loadPayTypes()
        {
            //dataGridView1.Columns[0].items
        }

        private void loadUsers()
        {
            SQL sql = new SQL("select username from users order by username");
            DataTable dt = sql.Run();

            for (int i = 0; i < dt.Rows.Count; i++)
                usernameBox.Items.Add(dt.Rows[i]["username"].ToString());

            if (usernameBox.Items.Count > 0)
                usernameBox.SelectedIndex = 0;
        }

        private void loadPeriods()
        {
            SQL sql = new SQL("select period, [from], [to], [Current Period] from periods order by period desc");
            DataTable dt = sql.Run();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                periodBox.Items.Add(dt.Rows[i]["period"].ToString());

                if(bool.Parse(dt.Rows[i]["current period"].ToString()))
                {
                    periodBox.SelectedIndex = periodBox.Items.Count - 1;
                }
            }

            if(periodBox.Items.Count > 0 && periodBox.Text == "")
                periodBox.SelectedIndex = 0;
        }



        private void reloadGrid(object sender, EventArgs e)
        {
            SQL sql = new SQL(@"
SELECT
t.timecarddetailid as TimeCardDetailID,
CONVERT(varchar(10),t.dateworked, 101) as DateWorked,
t.hours as Hours,
p.description as PayType,
t.workorder as WorkOrder,
t.lumpsum as LumpSum,
t.reviewed as Reviewed,
t.recordlocked as Approved,
t.exported as Exported

FROM
Timesheets t,
users u,
paycodes p

WHERE
t.period=@PERIOD and
t.employeeid = u.employeeid and
u.username=@USERNAME and
t.paytype = p.paytype

ORDER BY
t.dateworked desc
");
            sql.AddParameter("@PERIOD", periodBox.Text);
            sql.AddParameter("@USERNAME", usernameBox.Text);
            DataTable dt = sql.Run();

            dataGridView1.DataSource = dt;

            countHours();
        }

        private void countHours()
        {
            double wk1reg = 0, wk2reg = 0, wk1nonreg = 0, wk2nonreg = 0, wk1total = 0, wk2total = 0, lsumtotal = 0, total = 0;

            for(int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                double value = 0;

                if (dataGridView1.Rows[i].Cells["Hours"] != null &&
                    dataGridView1.Rows[i].Cells["Hours"].Value != null &&
                    double.TryParse(dataGridView1.Rows[i].Cells["Hours"].Value.ToString(), out value))
                {
                    if(dataGridView1.Rows[i].Cells["Week"].Value.ToString() == "1")
                    {
                        if (dataGridView1.Rows[i].Cells["PayType"].Value.ToString() == "Regular")
                        {
                            wk1reg += value;
                        }
                        else
                            wk1nonreg += value;

                        wk1total += value;
                    }
                    else
                    {
                        if (dataGridView1.Rows[i].Cells["PayType"].Value.ToString() == "Regular")
                        {
                            wk2reg += value;
                        }
                        else
                            wk2nonreg += value;

                        wk2total += value;
                    }

                    total += value;
                }

                if (dataGridView1.Rows[i].Cells["LumpSum"] != null &&
                    dataGridView1.Rows[i].Cells["LumpSum"].Value != null &&
                    double.TryParse(dataGridView1.Rows[i].Cells["LumpSum"].Value.ToString(), out value))
                {
                    lsumtotal += value;
                }
            }

            textBox5.Text = wk1reg.ToString();
            textBox3.Text = wk2reg.ToString();
            textBox7.Text = wk1nonreg.ToString();
            textBox6.Text = wk2nonreg.ToString();
            textBox11.Text = wk1total.ToString();
            textBox10.Text = wk2total.ToString();
            textBox8.Text = total.ToString();
            textBox9.Text = lsumtotal.ToString();
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns["PayType"].Index)
            {
                object eFV = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

                DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];

                if (!cell.Items.Contains(eFV))
                {
                    cell.Items.Add(eFV);
                    cell.Value = eFV;
                }
            }
            else if (e.ColumnIndex == dataGridView1.Columns["WorkOrder"].Index && e.Exception.Message == "Input string was not in a correct format.")
            {
                MessageBox.Show("Work order not found." + Environment.NewLine + Environment.NewLine + "Please type the work order number, or press F3 to search." + Environment.NewLine + "Press ESC to cancel your edit.");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            usernameBox.SelectedIndex = Math.Min(usernameBox.Items.Count - 1, usernameBox.SelectedIndex + 1);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            usernameBox.SelectedIndex = Math.Max(0, usernameBox.SelectedIndex - 1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            for (int i = e.RowIndex; i < e.RowIndex + e.RowCount; i++)
            {
                DataGridViewTextBoxCell cell = dataGridView1.Rows[i].Cells["Week"] as DataGridViewTextBoxCell;

                DateTime date = new DateTime();

                if (dataGridView1.Rows[i].Cells["DateWorked"].Value != null &&
                    DateTime.TryParse(dataGridView1.Rows[i].Cells["DateWorked"].Value.ToString(), out date))
                {
                    //if (date < Core.getStartingMonday(date).AddDays(7))
                    //    cell.Value = 1;
                    //else
                    //    cell.Value = 2;
                }


                cell = dataGridView1.Rows[i].Cells["D"] as DataGridViewTextBoxCell;

                if (dataGridView1.Rows[i].Cells["DateWorked"].Value != null &&
                    DateTime.TryParse(dataGridView1.Rows[i].Cells["DateWorked"].Value.ToString(), out date))
                {
                    cell.Value = date.ToString("dddd").Substring(0,2);
                }
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //if(e.ColumnIndex == dataGridView1.Columns["Week"].Index)
            //{
            //    DateTime date = DateTime.Parse(e.Value.ToString());

            //    if (date < Core.getStartingMonday(date).AddDays(7))
            //        e.Value = 1;
            //    else
            //        e.Value = 2;

            //    e.FormattingApplied = true;
            //}
        }

        private void usernameBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SQL sql = new SQL("select employeeid from users where username = @USERNAME");
            sql.AddParameter("@USERNAME", usernameBox.Text);
            DataTable dt = sql.Run();

            if(dt.Rows.Count > 0)
                textBox4.Text = dt.Rows[0]["employeeid"].ToString();

            reloadGrid(sender, e);
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
          //  dataGridView1.init
           // MessageBox.Show(dataGridView1.Columns[e.ColumnIndex].Name);
        }
    }
}
