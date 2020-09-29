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
    public partial class GetDate : Form
    {
        public DateTime r = new DateTime(1900, 1, 1);
        public bool quit = true;

        public GetDate(string title)
        {
            InitializeComponent();

            Text = title;
        }

        public GetDate(string title, DateTime defaultDate)
        {
            InitializeComponent();

            Text = title;
            dateTimePicker1.Value = defaultDate;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            r = dateTimePicker1.Value.Date;
            quit = false;
            
            Close();
        }

        private void GetDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                button8_Click(null, null);
            else if (e.KeyCode == Keys.Escape)
                Close();
        }
    }
}
