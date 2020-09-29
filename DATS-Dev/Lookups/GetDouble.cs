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
    public partial class GetDouble : Form
    {
        public double r;
        public bool quit = true;
        private string lastValue = "";
        double minValue = double.MinValue;
        double maxValue = double.MaxValue;

        public GetDouble(string title)
        {
            InitializeComponent();

            Text = title;
        }

        public GetDouble(string title, double defaultValue)
        {
            InitializeComponent();

            Text = title;
            textBox1.Text = defaultValue.ToString();
        }

        public GetDouble(string title, double min, double max)
        {
            InitializeComponent();

            Text = title;
            minValue = min;
            maxValue = max;
        }

        public GetDouble(string title, double min, double max, double defaultValue)
        {
            InitializeComponent();

            Text = title;
            minValue = min;
            maxValue = max;
            textBox1.Text = defaultValue.ToString();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Please enter a number.");
                return;
            }

            double value = double.Parse(textBox1.Text);

            if (value > maxValue)
            {
                MessageBox.Show("Number is greater than the maximum of " + maxValue + ".");
                return;
            }

            if (value < minValue)
            {
                MessageBox.Show("Number is less than the minimum of " + minValue + ".");
                return;
            }

            r = value;
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            for (int i = textBox1.Text.Length - 1; i >= 0; i--)
            {
                if (!Core.isIn(textBox1.Text[i], '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.') ||
                    textBox1.Text.Count(f => f == '.') > 1)
                {
                    textBox1.Text = lastValue;

                    textBox1.SelectionStart = Math.Max(textBox1.Text.Length, 0);
                    textBox1.SelectionLength = 0;

                    return;
                }
            }

            lastValue = textBox1.Text;
        }
    }
}
