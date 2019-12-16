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
    public partial class NewTimesheetFast : Form
    {
        int numRows = 1;
        TextBox[] dates;
        Label[] dateLabels;
        TextBox[] hours;
        ComboBox[] payTypes;
        TextBox[] workOrders;
        Label[] workOrderLabels;
        Button[] rowStatuses;
        string username;

        public NewTimesheetFast(string user)
        {
            InitializeComponent();

            username = user;

            hoursCalendar1.username = username;
            hoursCalendar1.showDate(DateTime.Now);
            this.Text = "New Timesheets for " + username;

            DataTable dt = SQL.Run("select description from paycodes order by paytype");

            for (int i = 0; i < dt.Rows.Count; i++)
                comboBox1.Items.Add(dt.Rows[i]["description"].ToString());

            dates = new TextBox[numRows];
            dateLabels = new Label[numRows];
            hours = new TextBox[numRows];
            payTypes = new ComboBox[numRows];
            workOrders = new TextBox[numRows];
            workOrderLabels = new Label[numRows];
            rowStatuses = new Button[numRows];

            dates[0] = textBox2;
            dateLabels[0] = label6;
            hours[0] = textBox1;
            payTypes[0] = comboBox1;
            workOrders[0] = textBox3;
            workOrderLabels[0] = label5;
            rowStatuses[0] = button1;

            if(!Core.showUserWorkOrders(username))
            {
                textBox3.Visible = false;
                label4.Visible = false;
                label5.Visible = false;
            }
        }

        private void NewTimesheetFast_Load(object sender, EventArgs e)
        {

        }

        private void checkDate(object sender, EventArgs e)
        {
            TextBox box = sender as TextBox;

            if (box.Tag != null && box.Tag.ToString() == "-1")
            {
                box.Tag = 0;
            }
            else
            {
                int rint;
                DateTime rdate;
                if(box.Text == "")
                {

                }
                else if (int.TryParse(box.Text, out rint))
                {
                    try
                    {
                        DateTime date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, rint);

                        for (int i = 0; i < 14; i++)
                            if (hoursCalendar1.getDay(i).getDate().Day == rint)
                                date = hoursCalendar1.getDay(i).getDate();

                        box.Text = date.ToString("d");

                        for (int i = 0; i < dates.Length; i++)
                            if (dates[i] == box)
                                dateLabels[i].Text = date.DayOfWeek.ToString().Substring(0, 3);
                    }
                    catch (ArgumentOutOfRangeException e2)
                    {
                        MessageBox.Show(DateTime.Now.ToString("MMMM") + " doesn't have " + rint + " days.");
                        box.Text = "";
                        box.Select();
                    }
                }
                else if (DateTime.TryParse(box.Text, out rdate))
                {
                    box.Text = rdate.ToString("d");

                    for (int i = 0; i < dates.Length; i++)
                        if(dates[i] == box)
                            dateLabels[i].Text = rdate.DayOfWeek.ToString().Substring(0, 3);
                }
                else
                {
                    MessageBox.Show("Can't recognize date.");
                    box.Text = "";
                    box.Select();
                }
            }
        }

        private void checkHours(object sender, EventArgs e)
        {
            TextBox box = sender as TextBox;

            double rdoub;

            if(double.TryParse(box.Text, out rdoub))
            {
                if ((double)(int)(rdoub * 4) == (rdoub * 4))
                {
                    box.Text = rdoub.ToString();
                }
                else
                {
                    MessageBox.Show("Hours must be in multiples of 0.25.");
                    box.Text = "";
                    box.Select();
                }
                
            }
            else
            {
                MessageBox.Show("Can't recognize hours.");
                box.Text = "";
                box.Select();
            }
        }

        private void checkPayType(object sender, EventArgs e)
        {
            ComboBox box = sender as ComboBox;

            bool matchFound = false;
            for (int i = 0; i < box.Items.Count && !matchFound; i++)
            {
                if (box.Items[i].ToString().ToLower() == box.Text.ToLower())
                {
                    box.Text = box.Items[i].ToString();
                    matchFound = true;
                }
            }

            for (int i = 0; i < box.Items.Count && !matchFound; i++)
            {
                if (box.Items[i].ToString().ToLower().Contains(box.Text.ToLower()))
                {
                    box.Text = box.Items[i].ToString();
                    matchFound = true;
                }
            }

            if (!matchFound)
            {
                MessageBox.Show("Can't recognize pay type.");
                box.Text = "";
                box.Select();
            }
        }

        private void checkWorkOrder(object sender, EventArgs e)
        {
            TextBox box = sender as TextBox;

            if (box.Text != "")
            {
                Oracle ora = new Oracle("select WADOCO, WADL01 from " + Core.getSchema(Core.getEnvironment()) + ".F4801 where WADOCO like '%' || :QUERY || '%' or upper(WADL01) like '%' || :QUERY || '%' order by WADOCO");
                ora.AddParameter("QUERY", box.Text.ToUpper());
                DataTable dt = ora.Run();

                if (dt.Rows.Count > 0)
                {
                    box.Text = dt.Rows[0]["WADOCO"].ToString();

                    for (int i = 0; i < workOrders.Length; i++)
                        if (workOrders[i] == box)
                            workOrderLabels[i].Text = dt.Rows[0]["WADL01"].ToString();
                }
                else
                {
                    MessageBox.Show("Work order not found.");
                    FindWorkOrder fwo = new FindWorkOrder(username);
                    fwo.ShowDialog();

                    if (!fwo.quit)
                        box.Text = fwo.woID;
                    else
                        box.Select();
                }
            }
        }

        private void nextLine(object sender, EventArgs e)
        {
            Array.Resize(ref dates, numRows + 1);
            Array.Resize(ref dateLabels, numRows + 1);
            Array.Resize(ref hours, numRows + 1);
            Array.Resize(ref payTypes, numRows + 1);
            Array.Resize(ref workOrders, numRows + 1);
            Array.Resize(ref workOrderLabels, numRows + 1);
            Array.Resize(ref rowStatuses, numRows + 1);

            int rowHeight = 20;
            int tabindex = (sender as Button).TabIndex + 1;

            (sender as Button).Enabled = false;

            //Date
            TextBox tb = new TextBox();
            
            panel1.Controls.Add(tb);
            dates[numRows] = tb;

            tb.TabIndex = tabindex++;
            tb.Leave += new EventHandler(checkDate);
            tb.Top = dates[numRows - 1].Top + rowHeight;
            tb.Left = textBox2.Left;
            tb.Tag = -1;
            tb.Text = dates[numRows - 1].Text;
            tb.Select();

            //Hours
            tb = new TextBox();

            panel1.Controls.Add(tb);
            hours[numRows] = tb;

            tb.TabIndex = tabindex++;
            tb.Leave += new EventHandler(checkHours);
            tb.Top = dates[numRows - 1].Top + rowHeight;
            tb.Left = textBox1.Left;
            tb.Text = hours[numRows - 1].Text;

            //Pay Type
            ComboBox cb = new ComboBox();

            panel1.Controls.Add(cb);
            payTypes[numRows] = cb;

            cb.TabIndex = tabindex++;
            cb.Leave += new EventHandler(checkPayType);
            cb.Top = dates[numRows - 1].Top + rowHeight;
            cb.Left = comboBox1.Left;
            cb.Text = payTypes[numRows - 1].Text;

            for (int i = 0; i < comboBox1.Items.Count; i++)
                cb.Items.Add(comboBox1.Items[i]);

            Label la;

            if (Core.showUserWorkOrders())
            {
                //Work Order
                tb = new TextBox();

                panel1.Controls.Add(tb);
                workOrders[numRows] = tb;

                tb.TabIndex = tabindex++;
                tb.Leave += new EventHandler(checkWorkOrder);
                tb.Top = dates[numRows - 1].Top + rowHeight;
                tb.Left = textBox3.Left;
                tb.Text = workOrders[numRows - 1].Text;

                //Work Order Description
                la = new Label();

                panel1.Controls.Add(la);
                workOrderLabels[numRows] = la;

                la.Top = dates[numRows - 1].Top + rowHeight;
                la.Left = label5.Left;
                la.Text = workOrderLabels[numRows - 1].Text;
            }

            //Row Status
            Button bu = new Button();

            panel1.Controls.Add(bu);
            rowStatuses[numRows] = bu;

            bu.Click += new EventHandler(nextLine);
            bu.Enter += new EventHandler(nextLine);
            bu.TabIndex = tabindex++;
            bu.Top = dates[numRows - 1].Top + rowHeight;
            bu.Left = button1.Left;
            bu.Text = "Submit";

            //Date Label
            la = new Label();

            panel1.Controls.Add(la);
            dateLabels[numRows] = la;

            la.Top = dates[numRows - 1].Top + rowHeight;
            la.Left = label6.Left;

            Core.newTimesheet(username, DateTime.Parse(dates[numRows - 1].Text), "", "", payTypes[numRows - 1].Text, double.Parse(hours[numRows - 1].Text), (workOrders[numRows - 1] != null ? workOrders[numRows - 1].Text : ""), "", null);

            hoursCalendar1.showDate(DateTime.Parse(dates[numRows - 1].Text));

            numRows++;
        }

        private void hoursCalendar1_DayClicked(DateTime x)
        {
            if (username != "")
            {
                (new ViewTimesheets(x, username)).ShowDialog();
                Core.fillHoursCalendar(hoursCalendar1, username, x);
            }
        }

        private void NewTimesheetFast_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }
    }
}
