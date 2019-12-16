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
    public partial class ReviewRow : UserControl
    {
        public string id = "";
        public decimal hours = 0;
        public DateTime dateWorked;
        public string workOrder = "";
        public string payType = "";

        public ReviewRow()
        {
            InitializeComponent();
        }

        public void refresh()
        {
            hoursBox.Value = hours;
            dateWorkedBox.Value = dateWorked;
            payTypeBox.Items.Add(payType);
            payTypeBox.SelectedIndex = 0;
            workOrderBox.Text = workOrder.ToString();

            workOrderBox_KeyUp(null, null);
        }

        private void workOrderBox_KeyUp(object sender, KeyEventArgs e)
        {
            Oracle ora = new Oracle("select WADOCO, WADL01 from " + Core.getSchema(Core.getEnvironment()) + ".F4801 where WADOCO=:WO");
            ora.AddParameter("WO", workOrderBox.Text);
            DataTable dt = ora.Run();

            if (dt.Rows.Count > 0)
            {
                workOrderLabel.Text = dt.Rows[0]["WADL01"].ToString();
                workOrderBox.ForeColor = Color.Black;
            }
            else
            {
                workOrderLabel.Text = "";
                workOrderBox.ForeColor = Color.Red;
            }

        }

        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            MessageBox.Show("Hey!");
        }
    }
}
