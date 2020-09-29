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
    public partial class ReviewGrid : UserControl
    {
        public string username = "";
        public DateTime start = new DateTime();
        public DateTime end = new DateTime();
        public decimal regularHours = 0;
        public decimal otherHours = 0;

        public ReviewGrid()
        {
            InitializeComponent();
        }

        private void ReviewGrid_Load(object sender, EventArgs e)
        {
            ReviewRow[] rr = new ReviewRow[14];

            for (int i = 0; i < rr.Length; i++)
            {
                rr[i] = new ReviewRow();
                rr[i].Location = new Point(5, i * 23);
                Controls.Add(rr[i]);
            }
        }

        public void regenerate()
        {
            SQL sql = new SQL("select t.*, p.description as pt from Timesheets t, paycodes p, users u where t.employeeid = u.employeeid and u.username = @USERNAME and t.dateworked >= @DATESTART and t.dateworked <= @DATEEND and t.paytype = p.paytype order by t.dateworked desc");
            sql.AddParameter("@USERNAME", username);
            sql.AddParameter("@DATESTART", start);
            sql.AddParameter("@DATEEND", end);
            DataTable dt = sql.Run();

            for (int i = Controls.Count - 1; i >= 0; i--)
                if (Controls[i] is ReviewRow)
                    Controls.RemoveAt(i);

            regularHours = 0;
            otherHours = 0;

            for(int i = 0; i < dt.Rows.Count; i++)
            {
                ReviewRow rr = new ReviewRow();
                rr.Location = new Point(5, i * 23);
                rr.hours = decimal.Parse(dt.Rows[i]["hours"].ToString());
                rr.dateWorked = DateTime.Parse(dt.Rows[i]["dateworked"].ToString());
                rr.dateWorked = DateTime.Parse(dt.Rows[i]["dateworked"].ToString());
                rr.workOrder = dt.Rows[i]["workorder"].ToString();
                rr.payType = dt.Rows[i]["pt"].ToString();
                rr.refresh();
                Controls.Add(rr);

                if (rr.payType == "Regular")
                    regularHours += rr.hours;
                else
                    otherHours += rr.hours;
            }
        }
    }
}
