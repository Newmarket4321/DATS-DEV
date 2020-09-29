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
    public partial class GetInt : Form
    {
        public int r;
        public bool quit = true;

        public GetInt(string title)
        {
            InitializeComponent();

            Text = title;
            numericUpDown1.Select(0, numericUpDown1.Value.ToString().Length);
        }

        public GetInt(string title, int defaultValue)
        {
            InitializeComponent();

            Text = title;
            numericUpDown1.Value = defaultValue;
            numericUpDown1.Select(0, numericUpDown1.Value.ToString().Length);
        }

        public GetInt(string title, int min, int max)
        {
            InitializeComponent();

            Text = title;
            numericUpDown1.Minimum = min;
            numericUpDown1.Maximum = max;
            numericUpDown1.Select(0, numericUpDown1.Value.ToString().Length);
        }

        public GetInt(string title, int min, int max, int defaultValue)
        {
            InitializeComponent();

            Text = title;
            numericUpDown1.Minimum = min;
            numericUpDown1.Maximum = max;
            numericUpDown1.Value = defaultValue;
            numericUpDown1.Select(0, numericUpDown1.Value.ToString().Length);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            r = int.Parse(numericUpDown1.Value.ToString());
            quit = false;
            
            Close();
        }

        private void GetInt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                button8_Click(null, null);
            else if (e.KeyCode == Keys.Escape)
                Close();
        }
    }
}
