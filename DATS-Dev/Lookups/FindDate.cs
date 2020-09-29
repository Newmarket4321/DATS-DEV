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
    public partial class FindDate : Form
    {
        public DateTime date = new DateTime(1900, 1, 1);

        public FindDate(string description)
        {
            InitializeComponent();

            label1.Text = description;

            for (int i = 0; i < 12; i++)
            {
                comboBox1.Items.Add((new DateTime(2000, i+1, 1)).ToString("MMMM"));

                if (DateTime.Today.Month == i+1)
                    comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
            }

            for (int i = 0; i < 31; i++)
            {
                comboBox2.Items.Add((i+1));

                if (DateTime.Today.Day == i+1)
                    comboBox2.SelectedIndex = comboBox2.Items.Count - 1;
            }

            for (int i = 2003; i <= DateTime.Today.Year; i++)
            {
                comboBox3.Items.Add(i);

                if (DateTime.Today.Year == i)
                    comboBox3.SelectedIndex = comboBox3.Items.Count - 1;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            int month = DateTime.ParseExact(comboBox1.Text, "MMMM", CultureInfo.CurrentCulture).Month;
            int day = int.Parse(comboBox2.Text);
            int year = int.Parse(comboBox3.Text);

            try
            {
                date = new DateTime(year, month, day);

                Close();
            }
            catch(ArgumentOutOfRangeException e2)
            {
                MessageBox.Show(comboBox1.Text + " doesn't have " + comboBox2.Text + " days.");
            }
        }
    }
}
