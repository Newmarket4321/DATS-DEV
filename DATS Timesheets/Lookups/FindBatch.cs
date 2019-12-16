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
    public partial class FindBatch : Form
    {
        public string selectedID = "";

        public FindBatch()
        {
            InitializeComponent();

            DataTable dt = SQL.Run("select batchid, bdate from batchids order by batchid");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string id = dt.Rows[i]["batchid"].ToString();
                DateTime date = DateTime.Parse(dt.Rows[i]["bdate"].ToString());

                comboBox1.Items.Add(id + " - " + date.ToString("MMMM dd, yyyy"));

                comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            selectedID = comboBox1.Text.Split(' ')[0];

            Close();
        }
    }
}
