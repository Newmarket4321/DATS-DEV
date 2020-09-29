using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DATS_Timesheets
{
    public partial class FindEmpID : Form
    {
        public string value = "";

        public FindEmpID()
        {
            InitializeComponent();
            textBox1.Focus();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            value = textBox1.Text;
            Close();
        }

        private void FindEmpID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
            if (e.KeyCode == Keys.Enter)
                button8_Click(null, null);
        }
    }
}
