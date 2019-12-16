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
    public partial class FindPaycheque : Form
    {
        public string number = "";

        public FindPaycheque(DataTable dt)
        {
            InitializeComponent();
            
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DateTime date = Core.JDEToDate(dt.Rows[i]["YUPPED"].ToString());

                comboBox1.Items.Add(dt.Rows[i]["YUCKCN"].ToString() + " - " + date.ToString("MMMM dd, yyyy"));
                comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            number = comboBox1.Text.Split(' ')[0];

            Close();
        }
    }
}
