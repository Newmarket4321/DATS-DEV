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
    public partial class FindPayType : Form
    {
        public string payType;
        int tabSelected = 0;
        public bool quit = true;

        public FindPayType(bool hasWO)
        {
            InitializeComponent();

            tabControl1.Location = new Point(-5, -23);
            tabControl1.Size = new Size(tabControl1.Size.Width + 5, tabControl1.Size.Height + 26);

            //if (Core.showUserWorkOrders())
            //    tabSelected = (hasWO ? 0 : 1);
            //else
                tabSelected = 2;

            tabControl1.SelectedIndex = tabSelected;

            button4.Text = "I used my " + DateTime.Now.Year + " vacation";
            button6.Text = "I used my " + (DateTime.Now.Year - 1) + " vacation";
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            payType = "Regular";
            quit = false;
            this.Close();
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            payType = "Bank Hrs";
            quit = false;
            this.Close();
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            payType = "Overtime 1.5";
            quit = false;
            this.Close();
        }

        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
            payType = "Illness (H)";
            quit = false;
            this.Close();
        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            payType = "Bank Time Off";
            quit = false;
            this.Close();
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            payType = "Vac.(H) Curr.Year";
            quit = false;
            this.Close();
        }

        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {
            payType = "Banked Vac. (H)";
            quit = false;
            this.Close();
        }

        private void FindPayType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(tabControl1.SelectedIndex != tabSelected)
                tabControl1.SelectedIndex = tabSelected;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            payType = "Illness (H)";
            quit = false;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            payType = "Bank Time Off";
            quit = false;
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            payType = "Vac.(H) Curr.Year";
            quit = false;
            this.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            payType = "Vac.(H) Prior Year";
            quit = false;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            payType = "Regular";
            quit = false;
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            payType = "Bank Hrs";
            quit = false;
            this.Close();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            payType = "Overtime 1.5";
            quit = false;
            this.Close();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            payType = "Personal Appointment";
            quit = false;
            this.Close();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            payType = "Personal Appointment";
            quit = false;
            this.Close();
        }
    }
}
