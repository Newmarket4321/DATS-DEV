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
    public partial class GetTime : Form
    {
        public bool quit = true;
        public string startTime = "";
        public string endTime = "";
        public double hours = 0;

        public GetTime(string forUser, DateTime date)
        {
            InitializeComponent();
            
            string startTime = Core.getDepartmentStartingTime(forUser);
            string endTime = Core.getDepartmentEndingTime(forUser);
            string hours = Core.getDepartmentDailyHours(forUser).ToString();

            string lastFinish = "";

            if(!Core.canReview(Core.getUsername()) && !Core.canApprove(Core.getUsername()))
                hourBox.Minimum = 0;

            SQL sql = new SQL("select t.finishtime from Timesheets t, users u where t.employeeid = u.employeeid and u.displayname=@USERNAME and (t.dateworked > @AFTERDATE or t.dateworked = @AFTERDATE) and t.dateworked < @BEFOREDATE order by t.timecarddetailid desc");
            sql.AddParameter("@USERNAME", forUser);
            sql.AddParameter("@AFTERDATE", date.Date);
            sql.AddParameter("@BEFOREDATE", date.Date.AddDays(1));
            DataTable dt = sql.Run();

            if (dt.Rows.Count > 0)
            {
                lastFinish = dt.Rows[0]["finishtime"].ToString();

                try
                {
                    DateTime d1 = DateTime.Parse(DateTime.Today.ToString("D") + " " + startTime);
                    DateTime d2 = DateTime.Parse(DateTime.Today.ToString("D") + " " + lastFinish);

                    TimeSpan ts = d2 - d1;

                    hours = (Math.Max(0, double.Parse(hours) - ts.TotalHours)).ToString();
                    startTime = lastFinish;
                }
                catch
                {

                }
            }

            initialSetup(startTime, endTime, hours);
        }

        public GetTime(int existingTimesheetID)
        {
            InitializeComponent();
            
            SQL sql = new SQL("select starttime, finishtime, hours from Timesheets where timecarddetailid = @ID");
            sql.AddParameter("@ID", existingTimesheetID);
            DataTable dt = sql.Run();

            string startTime = dt.Rows[0]["starttime"].ToString();
            string endTime = dt.Rows[0]["finishtime"].ToString();
            string hours = dt.Rows[0]["hours"].ToString();

            initialSetup(startTime, endTime, hours);
        }

        public GetTime(string startTime, string endTime, double hours)
        {
            InitializeComponent();
            initialSetup(startTime, endTime, hours.ToString());
        }

        public void initialSetup(string startTime, string endTime, string hours)
        {
            startAMPM.SelectedIndex = startTime.Contains("AM") ? 0 : 1; //AM/PM
            try
            {
                startH.SelectedIndex = int.Parse(startTime.Substring(0, startTime.IndexOf(":"))) - 1;
                startM.SelectedIndex = int.Parse(startTime.Substring(startTime.IndexOf(":") + 1).Replace("AM", "").Replace("PM", "").Replace(" ", "")) / 15;
                endM.SelectedIndex = int.Parse(endTime.Substring(endTime.IndexOf(":") + 1).Replace("AM", "").Replace("PM", "").Replace(" ", "")) / 15;
                endH.SelectedIndex = int.Parse(endTime.Substring(0, endTime.IndexOf(":"))) - 1;
            }
            catch
            {
                
            }
            endAMPM.SelectedIndex = endTime.Contains("AM") ? 0 : 1; //AM/PM

            hourBox.Value = decimal.Parse(hours);

            save();
        }

        private void GetTime_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }

        private void updateVariables(object sender, EventArgs e)
        {
            string startTime = startH.Text + startM.Text + startAMPM.Text;
            string endTime = endH.Text + endM.Text + endAMPM.Text;

            try
            {
                DateTime start = DateTime.Parse(DateTime.Now.ToString("D") + " " + startTime);
                DateTime end = DateTime.Parse(DateTime.Now.ToString("D") + " " + endTime);

                if (start <= end)
                {
                    TimeSpan ts = end - start;

                    decimal hours = (decimal)ts.TotalHours;

                    if (Core.getDepartments().Contains(13) && hours > 5) //IT
                        hours--;

                    hourBox.Value = hours;
                }
                else
                {
                    end = end.AddDays(1);
                    TimeSpan ts = end - start;

                    decimal hours = (decimal)ts.TotalHours;

                    if (Core.getDepartments().Contains(13) && hours > 5) //IT
                        hours--;

                    hourBox.Value = hours;
                }
            }
            catch
            {

            }
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            if (true || check())
            {
                save();

                quit = false;
                Close();
            }
        }

        private bool check()
        {
            if (hourBox.Value * 400 % 100 != 0)
            {
                MessageBox.Show("Please have your hours be in 0.25 hour (15 minute) increments.");
                return false;
            }

            return true;
        }

        private void save()
        {
            startTime = startH.Text + startM.Text + startAMPM.Text;
            endTime = endH.Text + endM.Text + endAMPM.Text;

            //DateTime start = DateTime.Parse(DateTime.Now.ToString("D") + " " + startTime);
            //DateTime end = DateTime.Parse(DateTime.Now.ToString("D") + " " + endTime);

            //TimeSpan ts = end - start;

            hours = (double)hourBox.Value;
        }

        private void hourBox_ValueChanged(object sender, EventArgs e)
        {
            hours = (double)hourBox.Value;
        }
    }
}