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
    public partial class FindPeriod : Form
    {
        public string period = "";
        private string type = "";

        public FindPeriod(string empType, bool autoClick)
        {
            InitializeComponent();

            //string response = Core.lookupMessageBox("Which pay cycle?", "You are about to select a pay period. Do you wish to see salary or hourly pay periods?", "Salary", "Hourly");
            
            type = empType;

            string periodFilter = type == "H" ? "TOWNHOURLY" : "SALARY06";
            string periodFilter2 = type == "H" ? "HR" : "SAL";

            DataTable cur = Oracle.Run("select YDCTRY, YDDTEY, YDPPNB from " + Core.getSchema(Core.getEnvironment()) + ".F07210 where trim(YDPAYD) = '" + periodFilter + "'");
            string currentPeriod = cur.Rows[0]["YDCTRY"].ToString() + cur.Rows[0]["YDDTEY"].ToString() + cur.Rows[0]["YDPPNB"].ToString().Substring(1, 2);

            DataTable dt = Oracle.Run("select JDDTEY, JDPPNB from " + Core.getSchema(Core.getEnvironment()) + ".F069066 where trim(JDPCCD) = '" + periodFilter2 + "'");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string period = "20" + (dt.Rows[i]["JDDTEY"].ToString().Length == 2 ? dt.Rows[i]["JDDTEY"].ToString() : ("0" + dt.Rows[i]["JDDTEY"].ToString())) + dt.Rows[i]["JDPPNB"].ToString().Substring(1, 2);

                DateTime from = Core.getPeriodStart(int.Parse(period), type);
                DateTime to = Core.getPeriodEnd(int.Parse(period), type);

                comboBox1.Items.Add(period + " - " + from.ToString(@"MMM d") + " to " + to.ToString(@"MMM d") + (currentPeriod == period ? " (Current)" : ""));

                if (currentPeriod == period)
                    comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
            }

            if (autoClick)
                button8_Click(null, null);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            period = comboBox1.Text.Substring(0, 6);

            Close();
        }
    }
}
