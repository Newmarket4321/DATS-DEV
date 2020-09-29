using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DATS_Timesheets
{
    public partial class AskAQuestion : Form
    {
        public AskAQuestion()
        {
            InitializeComponent();
        }

        private void submitButton_Click(object sender, EventArgs e)
        {
            Core.sendMail("gsmyth@newmarket.ca", "DATS Question from " + Core.getUsername() + " (" + Environment.MachineName + ")", questionBox.Text);

            Close();
            MessageBox.Show("Thank you!" + Environment.NewLine + "You should receive a response soon by e-mail." + Environment.NewLine + Environment.NewLine + "Please check your e-mail over the coming days.");
        }
    }
}
