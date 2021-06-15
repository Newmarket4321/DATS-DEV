using DATS_Timesheets.Properties;
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
    public partial class ReportPaycheque : Form
    {
        DataTable storage = null;

        public ReportPaycheque(string title, DataTable dt, DataTable dt2, string chequeNumber)
        {
            InitializeComponent();

            Text = title;

            storage = dt;

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                list.Columns.Add(dt.Columns[i].ColumnName);
                list.Columns[i].TextAlign = HorizontalAlignment.Right;
            }

            list.Columns[0].TextAlign = HorizontalAlignment.Left;
            list.Columns[5].TextAlign = HorizontalAlignment.Left;

            //YAALPH, YUAN8, YUMAIL, YUDOCM, YUCKD, YUPPED, YUHRW, YUGPAY, YUGTXW, YUGDED, YUNPAY;
            label2.Text = dt2.Rows[0]["YAALPH"].ToString();
            label4.Text = dt2.Rows[0]["YUAN8"].ToString();
            label5.Text = dt2.Rows[0]["YUMAIL"].ToString();
            label7.Text = dt2.Rows[0]["YUDOCM"].ToString();
            label9.Text = Core.JDEToDate(dt2.Rows[0]["YUCKD"].ToString()).ToString("MM/dd/yy").Replace('-', '/');
            label11.Text = Core.JDEToDate(dt2.Rows[0]["YUPPED"].ToString()).ToString("MM/dd/yy").Replace('-', '/');
            string YUHRW = dt2.Rows[0]["YUHRW"].ToString();
            string YUGPAY = dt2.Rows[0]["YUGPAY"].ToString();
            string DED = dt2.Rows[0]["DED"].ToString();
            string YUNPAY = dt2.Rows[0]["YUNPAY"].ToString();
            decimal bankPaid = 0;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string[] data = new string[dt.Columns.Count];

                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    data[j] = dt.Rows[i][j].ToString();

                   // No longer needed. Banked hours calculation was already done
                   //if (j == 4 && data[0].Contains("BkHrs Usd/Pd"))
                   //     bankPaid = decimal.Parse(data[4].Trim());

                   // if (j == 4 && data[0].Contains("Bk Hrs Bal"))
                   //     data[4] = (decimal.Parse(data[4].Trim()) - bankPaid).ToString();
                }

                ListViewItem x = new ListViewItem(data);
                list.Items.Add(x);
            }

            Oracle ora = new Oracle("select to_char(sum(Y8HRW)/100, '999,999.99') as gross from " + Core.getSchema(Core.getEnvironment()) + ".F07186 where Y8CKCN=@CN and Y8PDBA <> 950 and Y8PDBA <> 955 and Y8PDBA <> 8350");
            ora.AddParameter("@CN", chequeNumber);
            DataTable gross = ora.Run();

            string[] totals = { "* Gross", gross.Rows[0]["gross"].ToString(), "", YUGPAY, "", "* Deductions", DED, "* Net " + YUNPAY };
            ListViewItem item = new ListViewItem(totals);
            list.Items.Add(item);

            for (int i = 0; i < list.Columns.Count; i++)
                list.Columns[i].Width = -2;
        }

        private void Report_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Core.export(Text, storage);
        }

        private void toolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            Printer queue = new Printer();
            int buffer = 10;
            queue.font = new Font("Times New Roman", 9);

            //Mailing letter front
            queue.AddLine();
            queue.AddImage(Resources.Newmarket_BW, 5, 0.065);
            queue.AddBold("Town of Newmarket", 130);
            queue.AddLine();
            queue.AddBold("395 Mulock Drive", 130);
            queue.AddLine();
            queue.AddBold("P.O. Box 328, Stn Main", 130);
            queue.AddLine();
            queue.AddBold("Newmarket ON L3Y 4X7", 130);
            queue.AddLine();
            queue.AddLine();
            queue.AddLine();
            queue.AddLine();
            queue.AddLine();
            queue.Add(label2.Text, 105);
            queue.Add(label4.Text, 330);
            queue.AddLine();

            DataTable addressInformation = Oracle.Run("select min(ALADD1), min(ALCTY1), min(ALADDS), min(ALADDZ) from " + Core.getSchema(Core.getEnvironment()) + ".F0116 where ALAN8 = " + label4.Text);

            queue.Add(addressInformation.Rows[0][0].ToString().Trim(), 105);
            queue.Add(label5.Text, 330);
            queue.AddLine();
            queue.Add(addressInformation.Rows[0][1].ToString().Trim() + " " + addressInformation.Rows[0][2].ToString().Trim() + " " + addressInformation.Rows[0][3].ToString().Trim(), 105);
            queue.AddLine();
            queue.AddLine();
            queue.AddLine();
            queue.AddLine();
            queue.AddLine();
            queue.AddDivider();
            queue.AddLine();

            //Pay stub header
            queue.AddImage(Resources.Newmarket_BW, 5, 0.065);
            queue.AddBold("Town of Newmarket", 130);
            queue.AddBold(label2.Text, 520);
            queue.AddLine();
            queue.AddBold("395 Mulock Drive", 130);
            queue.Add("Employee Number", 520);
            queue.Add(label4.Text, 640);
            queue.AddLine();
            queue.AddBold("P.O. Box 328, Stn Main", 130);
            queue.Add("Route:", 520);
            queue.Add(label5.Text, 640);
            queue.AddLine();
            queue.AddBold("Newmarket ON L3Y 4X7", 130);
            queue.Add("Advice No:", 520);
            queue.Add(label7.Text, 640);
            queue.AddLine();
            queue.Add("Deposit Date:", 520);
            queue.Add(label9.Text, 640);
            queue.AddLine();
            queue.Add("Period end:", 520);
            queue.Add(label11.Text, 640);
            queue.AddLine();
            queue.AddLine();
            queue.AddLine();

            float[] columnWidths = new float[list.Columns.Count];

            columnWidths[0] = 150;
            columnWidths[1] = 30;
            columnWidths[2] = 40;
            columnWidths[3] = 50;
            columnWidths[4] = 70;
            columnWidths[5] = 240;
            columnWidths[6] = 65;
            columnWidths[7] = 100;

            //for (int c = 0; c < list.Columns.Count; c++)
            //{
            //    columnWidths[c] = Printer.GetWidth(list.Columns[c].Text, queue.font);

            //    for (int r = 0; r < list.Items.Count; r++)
            //    {
            //        if (Printer.GetWidth(list.Items[r].SubItems[c].Text, queue.font) > columnWidths[c])
            //            columnWidths[c] = Printer.GetWidth(list.Items[r].SubItems[c].Text, queue.font);
            //    }
            //}

            int x = 0;
            for (int c = 0; c < list.Columns.Count; c++)
            {
                queue.AddBold(list.Columns[c].Text, x);

                x += (int)columnWidths[c] + buffer;
            }

            queue.AddLine();

            for (int r = 0; r < list.Items.Count; r++)
            {
                x = 0;
                for (int c = 0; c < list.Columns.Count; c++)
                {
                    queue.Add(list.Items[r].SubItems[c].Text.Trim(), x);

                    x += (int)columnWidths[c] + buffer;
                }

                queue.AddLine();
                queue.AddLine();
            }

            queue.Print();
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Printer queue = new Printer();
            int buffer = 10;
            queue.font = new Font("Times New Roman", 9);

            //Mailing letter front
            queue.AddLine();
            queue.AddImage(Resources.Newmarket_BW, 5, 0.065);
            queue.AddBold("Town of Newmarket", 130);
            queue.AddLine();
            queue.AddBold("395 Mulock Drive", 130);
            queue.AddLine();
            queue.AddBold("P.O. Box 328, Stn Main", 130);
            queue.AddLine();
            queue.AddBold("Newmarket ON L3Y 4X7", 130);
            queue.AddLine();
            queue.AddLine();
            queue.AddLine();
            queue.AddLine();
            queue.AddLine();
            queue.Add(label2.Text, 105);
            queue.Add(label4.Text, 330);
            queue.AddLine();

            //DataTable addressInformation = Oracle.Run("select min(ALADD1), min(ALCTY1), min(ALADDS), min(ALADDZ) from " + Core.getSchema(Core.getEnvironment()) + ".F0116 where ALAN8 = " + label4.Text);
            DataTable addressInformation = Oracle.Run("select ALADD1, ALCTY1, ALADDS, ALADDZ, ALEFTB  from " + Core.getSchema(Core.getEnvironment()) + ".F0116 where ALAN8 = " + label4.Text);

            DataRow recentAddress = addressInformation.Select("", "ALEFTB ASC").LastOrDefault();

            //queue.Add(addressInformation.Rows[0][0].ToString().Trim(), 105);
            queue.Add(recentAddress[0].ToString().Trim(), 105);
            queue.Add(label5.Text, 330);
            queue.AddLine();
            //queue.Add(addressInformation.Rows[0][1].ToString().Trim() + " " + addressInformation.Rows[0][2].ToString().Trim() + " " + addressInformation.Rows[0][3].ToString().Trim(), 105);
            queue.Add(recentAddress[1].ToString().Trim() + " " + recentAddress[2].ToString().Trim() + " " + recentAddress[3].ToString().Trim(), 105);

            queue.AddLine();
            queue.AddLine();
            queue.AddLine();
            queue.AddLine();
            queue.AddLine();
            queue.AddDivider();
            queue.AddLine();

            //Pay stub header
            queue.AddImage(Resources.Newmarket_BW, 5, 0.065);
            queue.AddBold("Town of Newmarket", 130);
            queue.AddBold(label2.Text, 520);
            queue.AddLine();
            queue.AddBold("395 Mulock Drive", 130);
            queue.Add("Employee Number", 520);
            queue.Add(label4.Text, 640);
            queue.AddLine();
            queue.AddBold("P.O. Box 328, Stn Main", 130);
            queue.Add("Route:", 520);
            queue.Add(label5.Text, 640);
            queue.AddLine();
            queue.AddBold("Newmarket ON L3Y 4X7", 130);
            queue.Add("Advice No:", 520);
            queue.Add(label7.Text, 640);
            queue.AddLine();
            queue.Add("Deposit Date:", 520);
            queue.Add(label9.Text, 640);
            queue.AddLine();
            queue.Add("Period end:", 520);
            queue.Add(label11.Text, 640);
            queue.AddLine();
            queue.AddLine();
            queue.AddLine();

            float[] columnWidths = new float[list.Columns.Count];

            columnWidths[0] = 150;
            columnWidths[1] = 30;
            columnWidths[2] = 40;
            columnWidths[3] = 50;
            columnWidths[4] = 70;
            columnWidths[5] = 200;
            columnWidths[6] = 65;
            columnWidths[7] = 100;

            //for (int c = 0; c < list.Columns.Count; c++)
            //{
            //    columnWidths[c] = Printer.GetWidth(list.Columns[c].Text, queue.font);

            //    for (int r = 0; r < list.Items.Count; r++)
            //    {
            //        if (Printer.GetWidth(list.Items[r].SubItems[c].Text, queue.font) > columnWidths[c])
            //            columnWidths[c] = Printer.GetWidth(list.Items[r].SubItems[c].Text, queue.font);
            //    }
            //}

            int x = 0;
            for (int c = 0; c < list.Columns.Count; c++)
            {
                queue.AddBold(list.Columns[c].Text, x);

                x += (int)columnWidths[c] + buffer;
            }

            queue.AddLine();

            for (int r = 0; r < list.Items.Count; r++)
            {
                x = 0;
                for (int c = 0; c < list.Columns.Count; c++)
                {
                    queue.Add(list.Items[r].SubItems[c].Text.Trim(), x);

                    x += (int)columnWidths[c] + buffer;
                }

                queue.AddLine();
                queue.AddLine();
            }

            queue.Print();
        }
    }
}
