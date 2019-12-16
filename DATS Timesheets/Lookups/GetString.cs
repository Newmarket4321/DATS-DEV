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
    public partial class GetString : Form
    {
        public string r = "";
        public bool quit = true;

        public GetString(string title)
        {
            InitializeComponent();

            Text = title;
        }

        public GetString(string title, bool multiline)
        {
            InitializeComponent();

            if (multiline)
            {
                FormBorderStyle = FormBorderStyle.Sizable;
                textBox1.Multiline = true;
                textBox1.AcceptsReturn = true;
                Height = 400;
            }

            Text = title;
        }

        public GetString(string title, string defaultText)
        {
            InitializeComponent();

            Text = title;
            textBox1.Text = defaultText;
        }

        public GetString(string title, string defaultText, bool multiline)
        {
            InitializeComponent();

            if (multiline)
            {
                FormBorderStyle = FormBorderStyle.Sizable;
                textBox1.Multiline = true;
                textBox1.AcceptsReturn = true;
                Height = 400;
            }

            Text = title;
            textBox1.Text = defaultText;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            r = textBox1.Text;
            quit = false;
            
            Close();
        }

        private void LookupTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A && e.Control)
                textBox1.SelectAll();
            else if (e.KeyCode == Keys.Enter && textBox1.Multiline == false)
                button8_Click(null, null);
            else if (e.KeyCode == Keys.Escape)
                Close();
        }
    }
}
