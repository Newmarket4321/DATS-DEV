using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DATS_Timesheets
{
    public partial class HoursCalendar : UserControl
    {
        Day[] days = new Day[14];
        public int periodDifference = 0;
        public string username = Core.getUsername();
        public string empType = Core.getSH(Core.getEmpID(Core.getUsername()));
        Label[] dayLabels = null;

        public HoursCalendar()
        {
            InitializeComponent();
            int sidebarWidth = 0;

            dayLabels = new Label[14] { label3, label4, label5, label6, label7, label8, label2, label16, label15, label11, label14, label13, label12, label17 };

            this.Width = (new Day()).Width * 7;
            int startingHeight = 19;

            //Monday, Tuesday, etc.
            for (int i = 0; i < dayLabels.Length; i++)
            {
                dayLabels[i].Width = (this.Width - sidebarWidth) / 7;
                dayLabels[i].Location = new Point(((int)(((double)i) / 7 * (double)(this.Width - sidebarWidth))), dayLabels[i].Location.Y);
                dayLabels[i].Height = startingHeight - dayLabels[i].Location.Y;
            }

            label1.Width = this.Width - sidebarWidth;
            label1.Location = new Point(0, 0);

            //Structure day grid
            for (int i = 0; i < days.Length; i++)
            {
                days[i] = new Day();
                //days[i].MouseEnter += HoursCalendar_MouseEnter;
                //days[i].MouseLeave += HoursCalendar_MouseLeave;
                //days[i].MouseUp += HoursCalendar_MouseUp;
                //days[i].MouseDown += HoursCalendar_MouseDown;
                days[i].DayClicked += HoursCalendar_DayClicked;
                days[i].Width = dayLabels[i % 7].Width;
                this.Controls.Add(days[i]);
                days[i].Location = new Point(dayLabels[1].Location.X*i, startingHeight);
            }

            //label12.Size = new Size(label12.Width, days[0].Height);
            //label13.Size = new Size(label12.Width, days[7].Height);
            //label12.Location = new Point(label12.Location.X, days[0].Location.Y);
            //label13.Location = new Point(label12.Location.X, days[7].Location.Y);

            this.Height = days[days.Length - 1].Location.Y + days[days.Length - 1].Height;

            updateDayData(Core.getStartingMonday(DateTime.Today, empType));
        }

        public void blackText()
        {
            for(int i = 0; i < Controls.Count; i++)
            {
                if(Controls[i] is Label)
                {
                    Label la = Controls[i] as Label;

                    la.ForeColor = Color.Black;
                }
            }

            for (int i = 0; i < days.Length; i++)
                days[i].blackText();
        }

        public void updateDayData(DateTime startingMonday)
        {
            //DateTime startingMonday = Core.getStartingMonday(DateTime.Today, empType).AddDays(14 * periodDifference);

            for (int i = 0; i < days.Length; i++)
            {
                days[i].setLabel(startingMonday.AddDays(i));
                dayLabels[i].Text = startingMonday.AddDays(i).DayOfWeek.ToString();

                if(startingMonday.AddDays(i) == DateTime.Today)
                {
                    dayLabels[i].ForeColor = Color.White;
                    dayLabels[i].BackColor = Color.FromArgb(0, 122, 255);
                }
                else if(dayLabels[i].Text == "Saturday" || dayLabels[i].Text == "Sunday")
                {
                    dayLabels[i].ForeColor = Color.White;
                    dayLabels[i].BackColor = Color.FromArgb(50, 50, 50);
                }
                else
                {
                    dayLabels[i].ForeColor = Color.FromArgb(50, 50, 50);
                    dayLabels[i].BackColor = Color.White;
                }
            }

            label1.Text = startingMonday.ToString("MMMMM yyyy");

            if (startingMonday.Month != startingMonday.AddDays(13).Month)
            {
                if (startingMonday.Year != startingMonday.AddDays(13).Year)
                    label1.Text = startingMonday.ToString("MMMMM yyyy") + " - " + startingMonday.AddDays(13).ToString("MMMMM yyyy");
                else
                    label1.Text = startingMonday.ToString("MMMMM") + " - " + startingMonday.AddDays(13).ToString("MMMMM yyyy");
            }

            //if (username != Core.getUsername())
            //    label1.Text = "[" + username + "] " + label1.Text;

            //double total = 0;

            //for (int i = 0; i < days.Length / 2; i++)
            //    total += (double)days[i].getHours();

            //label12.Text = total.ToString();
            //if (label12.Text == "0")
            //    label12.Text = "";

            //total = 0;

            //for (int i = days.Length / 2; i < days.Length; i++)
            //    total += (double)days[i].getHours();

            //label13.Text = total.ToString();
            //if (label13.Text == "0")
            //    label13.Text = "";

            //label11.Text = (double.Parse(label12.Text == "" ? "0" : label12.Text) + double.Parse(label13.Text == "" ? "0" : label13.Text)).ToString();
        }

        public delegate void CalendarEventHandler(DateTime x);
        public event CalendarEventHandler DayClicked;

        void HoursCalendar_DayClicked(DateTime x)
        {
            DayClicked(x);
        }

        public void showDate(DateTime targetDay)
        {
            DateTime startingMonday = Core.getStartingMonday(targetDay, empType);
            int delta = 0;
            bool done = false;

            while (!done)
            {
                TimeSpan ts = targetDay - startingMonday.AddDays(14 * delta);
                int netDays = (int)(ts.TotalDays);

                if (netDays >= 0 && netDays <= 13)
                    done = true;
                else if (netDays < 0)
                    delta--;
                else
                    delta++;
            }

            periodDifference += delta;
            updateDayData(startingMonday);
            Core.fillHoursCalendar(this, username, days[0].getDate());
        }

        public Day getDay(int index)
        {
            return days[index];
        }

        public void addHoursToDay(DateTime d, string pt, decimal h)
        {
            for (int i = 0; i < days.Length; i++)
            {
                if (days[i].getDate().Date == d.Date)
                    days[i].addHours(pt, h, username);
            }
        }

        public void clearHours()
        {
            for (int i = 0; i < days.Length; i++)
                days[i].clearHours();
        }

        private void label1_MouseEnter(object sender, EventArgs e)
        {
            //label1.BackColor = Color.FromName("ControlDark");
        }

        private void label1_MouseLeave(object sender, EventArgs e)
        {
            //label1.BackColor = Color.FromName("Control");
        }
        
        private void HoursCalendar_Resize(object sender, EventArgs e)
        {
            label1.Width = Width;

            Label[] dayLabels = new Label[14] { label3, label4, label5, label6, label7, label8, label2, label16, label15, label11, label14, label13, label12, label17 };
            int remainder = Width - (Width / days.Length) * days.Length;

            for (int i = 0; i < days.Length; i++)
                if (days != null && days[i] != null)
                {
                    int newSize = Width / days.Length;
                    int newX = 0;

                    if (remainder > 0)
                    {
                        newSize++;
                        remainder--;
                    }

                    if(i > 0)
                        newX = days[i-1].Right;

                    days[i].resize(newSize);
                    days[i].Location = new Point(newX, days[i].Location.Y);
                    dayLabels[i].Size = new Size(newSize, dayLabels[i].Height);
                    dayLabels[i].Location = new Point(newX, dayLabels[i].Location.Y);
                }
        }
    }
}
