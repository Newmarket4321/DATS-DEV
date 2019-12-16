using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DATS_Timesheets
{
    public partial class Day : UserControl
    {
        private decimal totalHours = 0;
        List<PictureBox> boxes = new List<PictureBox>();
        int nextTop = 18;
        private DateTime date;

        public Day()
        {
            InitializeComponent();
            //update();
        }

        public void blackText()
        {
            for (int i = 0; i < Controls.Count; i++)
            {
                if (Controls[i] is Label)
                {
                    Label la = Controls[i] as Label;

                    la.ForeColor = Color.Black;
                }
            }

            label1.ForeColor = Color.White;
        }

        public void resize(int size)
        {
            Width = size;
        }

        public void addHours(string pt, decimal h, string username)
        {
            line1.Visible = false;

            int areaHeight = Height - label1.Bottom;

            if (pt == "Standby" || pt == "Standby -Weekend")
                label3.Visible = true;

            totalHours += h;
            label1.Text = ((double)totalHours).ToString();
            label1.Visible = true;

            if (label1.Text == "0")
            {
                label1.Text = "";
                label1.BackColor = Color.White;
            }
            else if (totalHours > Core.getDepartmentDailyHours(username))
                label1.BackColor = Color.FromArgb(255, 149, 0);
            else
                label1.BackColor = Color.FromArgb(50, 50, 50);

            PictureBox box = new PictureBox();
            box.BackColor = Core.getPayTypeColor(pt);
            box.Anchor = AnchorStyles.Right | AnchorStyles.Left;

            //box.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            box.MouseEnter += Day_MouseEnter;
            box.MouseLeave += Day_MouseLeave;
            box.MouseClick += Day_MouseClick;
            //box.BackColor = Color.FromArgb((int)(r.NextDouble() * 255), (int)(r.NextDouble() * 255), (int)(r.NextDouble() * 255));
            box.Tag = h;

            Controls.Add(box);
            //Controls.SetChildIndex(box, 0);

            nextTop = label1.Bottom;
            bool firstHit = false;
            int lastBottom = 0;

            PictureBox[] boxes = new PictureBox[Controls.Count];
            int numBoxes = 0;

            for (int i = 0; i < Controls.Count; i++)
                if(Controls[i] is PictureBox)
                    boxes[numBoxes++] = Controls[i] as PictureBox;

            for (int i = 0; i < numBoxes; i++)
            {
                bool lastBox = i == numBoxes - 1;

                decimal hours = (decimal)boxes[i].Tag;
                decimal heightPerc = hours / Math.Max(Core.getDepartmentDailyHours(username), totalHours);
                int height = (int)(heightPerc * areaHeight);
                
                //Spread boxes to fill up space
                //if (lastBox)
                //    height = (areaHeight + label1.Height) - lastBottom;

                boxes[i].Size = new Size(Width, height);
                boxes[i].Location = new Point(-1, nextTop);
                lastBottom = boxes[i].Bottom;

                nextTop += height;
            }
        }

        void box_MouseEnter(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        public decimal getHours()
        {
            return totalHours;
        }

        public void clearHours()
        {
            nextTop = 18;
            totalHours = 0;
            label1.Text = "";
            label1.BackColor = Color.White;
            label3.Visible = false;

            for(int i = Controls.Count - 1; i > 0; i--)
            {
                if(Controls[i] is PictureBox)
                    Controls.RemoveAt(i);
            }
        }

        public void setLabel(DateTime day)
        {
            date = day.Date;
            label2.Text = day.Day.ToString();

            DateTime today = DateTime.Today;

            if (day.Year == today.Year && day.Month == today.Month && day.Day == today.Day)
                label1.BackColor = Color.Blue; //Today
            else
                label1.BackColor = Color.Black; //Other
        }

        public DateTime getDate()
        {
            return date.Date;
        }

        public void update()
        {
            //int topOfBar = label1.Bottom;

            //pictureBox1.Location = new Point(pictureBox1.Location.X, (int)((decimal)this.Height - Math.Min((Hours / Core.getDepartmentDailyHours()) * (decimal)(this.Height - topOfBar), this.Height - topOfBar)));
            //pictureBox1.Height = Math.Min((int)((Hours / Core.getDepartmentDailyHours()) * (this.Height - topOfBar)), this.Height - topOfBar);
            //label1.Text = double.Parse(Hours.ToString()).ToString() + "h"; //Parse to double so it shows up as 8 instead of 8.0 when multiple timesheets are added up
            ////label1.Location = new Point((int)(this.Width/2), (int)(this.Height/2));

            ////if (Hours > Core.getDepartmentDailyHours())
            //pictureBox1.BackColor = Color.FromArgb(12, 163, 232);
            ////else
            ////    pictureBox1.BackColor = Color.FromArgb(107, (int)(176 + (255 - 176) * (Math.Min(Hours / Core.getDepartmentDailyHours(), 1))), (int)(255 - (255 - 176) * (Math.Min(Hours / Core.getDepartmentDailyHours(), 1))));

            ////107,176,255
        }

        double fade = 0.8;

        private void Day_MouseEnter(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(100,0,0,0);
            this.Cursor = Cursors.Hand;

            for (int i = 0; i < Controls.Count; i++)
            {
                if (Controls[i] is PictureBox)
                {
                    PictureBox box = ((PictureBox)Controls[i]);
                    int r = Math.Min(255,Math.Max(0,(int)(fade * box.BackColor.R)));
                    int g = Math.Min(255,Math.Max(0,(int)(fade * box.BackColor.G)));
                    int b = Math.Min(255,Math.Max(0,(int)(fade * box.BackColor.B)));

                    box.BackColor = Color.FromArgb(r, g, b);
                }

                if (Controls[i] == label3)
                {
                    Label la = ((Label)Controls[i]);
                    int r = (int)(fade * la.BackColor.R);
                    int g = (int)(fade * la.BackColor.G);
                    int b = (int)(fade * la.BackColor.B);

                    la.BackColor = Color.FromArgb(r, g, b);
                }
            }
        }

        private void Day_MouseLeave(object sender, EventArgs e)
        {
            this.BackColor = Color.White;
            this.Cursor = Cursors.Default;

            for (int i = 0; i < Controls.Count; i++)
            {
                if (Controls[i] is PictureBox)
                {
                    PictureBox box = ((PictureBox)Controls[i]);
                    int r = Math.Min(255,Math.Max(0,(int)((1 / fade) * box.BackColor.R)));
                    int g = Math.Min(255,Math.Max(0,(int)((1 / fade) * box.BackColor.G)));
                    int b = Math.Min(255,Math.Max(0,(int)((1 / fade) * box.BackColor.B)));

                    box.BackColor = Color.FromArgb(r, g, b);
                }

                if (Controls[i] == label3)
                {
                    Label la = ((Label)Controls[i]);
                    int r = Math.Min(255,Math.Max(0,(int)((1 / fade) * la.BackColor.R)));
                    int g = Math.Min(255,Math.Max(0,(int)((1 / fade) * la.BackColor.G)));
                    int b = Math.Min(255,Math.Max(0,(int)((1 / fade) * la.BackColor.B)));

                    la.BackColor = Color.FromArgb(r, g, b);
                }
            }
        }

        public delegate void DayEventHandler(DateTime x);
        public event DayEventHandler DayClicked;

        private void Day_MouseClick(object sender, MouseEventArgs e)
        {
            DayClicked(date.Date);
        }
    }
}
