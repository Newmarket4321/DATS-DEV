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
    public partial class GetDescription : Form
    {
        public string description = "";
        public bool quit = true;
        
        public GetDescription()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            description = textBox3.Text;
            quit = false;
            Close();
        }
    }
}
