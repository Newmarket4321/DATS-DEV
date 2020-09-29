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
    public partial class MessageBoxCustom : Form
    {
        public string response = "";

        public MessageBoxCustom(string title, string message, params string[] options)
        {
            InitializeComponent();

            Text = title;
            label1.Text = message;

            while (label1.Bottom > pictureBox1.Bottom - 30)
                Height++;

            while (label1.Right > pictureBox1.Right - 24)
                Width++;

            for (int i = 0; i < options.Length; i++)
                AddButton(options[i]);

            if (options.Length == 0)
                AddButton("OK");
        }

        private void AddButton(string text)
        {
            const int spacing = 12; //This should probably stay the same
            const int buttonBuffer = 7; //Changes distance between buttons

            Button b = new Button();
            b.Text = text;

            Font font = new Font("Segoe UI", 9F);
            b.Font = font;

            b.Width = Core.getStringWidth(text, font) + 30;
            
            b.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            b.Top = 76 + Height - 143;
            b.Click += b_Click;

            Controls.Add(b);

            //Move buttons
            int xPos = Right;

            for (int i = Controls.Count - 1; i >= 0; i--)
            {
                if (Controls[i] is Button)
                {
                    Controls[i].Location = new Point(xPos - Controls[i].Width - (2 * spacing), Controls[i].Location.Y);
                    xPos -= Controls[i].Width + buttonBuffer;
                }
            }

            //Resize form
            for (int i = 0; i < Controls.Count; i++)
                if (Controls[i] is Button && Controls[i].Left < 0)
                    Width += -Controls[i].Left + spacing;
        }

        void b_Click(object sender, EventArgs e)
        {
            response = ((Button)sender).Text;
            Close();
        }
    }
}
