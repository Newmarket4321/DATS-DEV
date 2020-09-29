using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DATS_Timesheets
{
    public class Printer
    {
        Queue<PrintItem> queue = new Queue<PrintItem>();
        public Font font = new Font(new FontFamily("Arial"), 8);
        public static Rectangle margins = new Rectangle(30, 30, 790, 1040);

        public PrintItem Next()
        {
            return queue.Dequeue();
        }

        public void AddDivider()
        {
            queue.Enqueue(new DividerPrintItem());
        }

        public void AddPage()
        {
            queue.Enqueue(new PagePrintItem());
        }

        public void AddImage(Image picture)
        {
            queue.Enqueue(new ImagePrintItem(picture));
        }

        public void AddImage(Image picture, int x)
        {
            queue.Enqueue(new ImagePrintItem(picture, x));
        }

        public void AddImage(Image picture, int w, int h)
        {
            queue.Enqueue(new ImagePrintItem(picture, w, h));
        }

        public void AddImage(Image picture, int x, int w, int h)
        {
            queue.Enqueue(new ImagePrintItem(picture, x, w, h));
        }

        public void AddImage(Image picture, double percentSize)
        {
            queue.Enqueue(new ImagePrintItem(picture, percentSize));
        }

        public void AddImage(Image picture, int x, double percentSize)
        {
            queue.Enqueue(new ImagePrintItem(picture, x, percentSize));
        }

        public void Add(string item)
        {
            queue.Enqueue(new StringPrintItem(item));
        }

        public void Add(string item, int x)
        {
            queue.Enqueue(new StringPrintItem(item, x));
        }

        public void AddBold(string item)
        {
            StringPrintItem spi = new StringPrintItem(item);
            spi.Bold = true;

            queue.Enqueue(spi);
        }

        public void AddBold(string item, int x)
        {
            StringPrintItem spi = new StringPrintItem(item, x);
            spi.Bold = true;

            queue.Enqueue(spi);
        }

        public void AddLine()
        {
            StringPrintItem item = new StringPrintItem("");
            item.newLine = true;

            queue.Enqueue(item);
        }

        public PrintItem Peek()
        {
            return queue.Peek();
        }

        public void Print()
        {
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(pd_PrintPage);
            PrintDialog dia = new PrintDialog();
            dia.Document = pd;

            // Print the document.
            if (dia.ShowDialog() == DialogResult.OK)
                pd.Print();
        }

        // The PrintPage event is raised for each page to be printed.
        private void pd_PrintPage(object sender, PrintPageEventArgs ev)
        {
            //Rectangle margins = ev.MarginBounds;
            float yPos = margins.Top;

            // Calculate the number of lines per page.

            //ev.Graphics.DrawRectangle(new Pen(Color.Black), new Rectangle(margins.Left, margins.Top, margins.Width, margins.Height));

            // Iterate over the file, printing each line.
            bool stopEarly = false;
            while (!stopEarly && queue.Count > 0)
            {
                PrintItem peek = queue.Peek();

                if (peek is StringPrintItem)
                {
                    StringPrintItem item = ((StringPrintItem)peek);

                    if (item.Bold)
                        font = new Font(font, FontStyle.Bold);
                    else
                        font = new Font(font, FontStyle.Regular);

                    SizeF sf = ev.Graphics.MeasureString(item.Text == "" ? "." : item.Text, font, margins.Width);

                    if (yPos + sf.Height < margins.Bottom)
                    {
                        ev.Graphics.DrawString(item.Text, font, Brushes.Black,
                                new RectangleF(new PointF(margins.Left + item.X, yPos), sf),
                                StringFormat.GenericTypographic);

                        if (item.newLine)
                            yPos += sf.Height;

                        queue.Dequeue();
                    }
                    else
                        stopEarly = true;
                }
                else if (peek is DividerPrintItem)
                {
                    DividerPrintItem item = ((DividerPrintItem)peek);

                    SizeF sf = ev.Graphics.MeasureString(".", font, margins.Width);

                    if (yPos + sf.Height < margins.Bottom)
                    {
                        ev.Graphics.DrawLine(new Pen(Brushes.Black), new Point(margins.Left, (int)yPos), new Point(margins.Right, (int)yPos));

                        if (item.newLine)
                            yPos += sf.Height;

                        queue.Dequeue();
                    }
                    else
                        stopEarly = true;
                }
                else if (peek is PagePrintItem)
                {
                    queue.Dequeue();
                    stopEarly = true;
                }
                else if (peek is ImagePrintItem)
                {
                    ImagePrintItem item = ((ImagePrintItem)peek);

                    //Include a catch for if the picture is super-huge, to not push it to the next page
                    //Without catch, super-huge images will cause infinite loops
                    if (yPos + item.picture.Height < margins.Bottom || item.picture.Height >= margins.Height)
                    {
                        ev.Graphics.DrawImage(item.picture, margins.Left + item.X, yPos, item.Width, item.Height);
                        queue.Dequeue();
                    }
                    else
                        stopEarly = true;
                }

            }

            // If more lines exist, print another page.
            if (queue.Count > 0)
                ev.HasMorePages = true;
            else
                ev.HasMorePages = false;
        }

        public static float GetWidth(string text, Font font)
        {
            Graphics g = Graphics.FromImage(new Bitmap(1, 1));
            SizeF sf = g.MeasureString(text, font, Printer.margins.Width);

            return sf.Width;
        }

        public static void PrintListView(string reportTitle, ListView list)
        {
            Printer queue = new Printer();
            int buffer = 10;

            queue.AddBold(reportTitle, 0);
            queue.Add("Printed:", 612);
            queue.Add(DateTime.Now.ToString(), 667);
            queue.AddLine();
            queue.AddLine();
            queue.AddDivider();

            float[] columnWidths = new float[list.Columns.Count];

            for (int c = 0; c < list.Columns.Count; c++)
            {
                columnWidths[c] = Printer.GetWidth(list.Columns[c].Text, queue.font);

                for (int r = 0; r < list.Items.Count; r++)
                {
                    if (Printer.GetWidth(list.Items[r].SubItems[c].Text, queue.font) > columnWidths[c])
                        columnWidths[c] = Printer.GetWidth(list.Items[r].SubItems[c].Text, queue.font);
                }
            }

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
                    queue.Add(list.Items[r].SubItems[c].Text, x);

                    x += (int)columnWidths[c] + buffer;
                }

                queue.AddLine();
            }

            queue.Print();
        }

        public static void PrintDataTable(string reportTitle, DataTable dt)
        {
            Printer queue = new Printer();
            int buffer = 10;

            queue.AddBold(reportTitle, 0);
            queue.Add("Printed:", 612);
            queue.Add(DateTime.Now.ToString(), 667);
            queue.AddLine();
            queue.AddLine();
            queue.AddDivider();

            float[] columnWidths = new float[dt.Columns.Count];

            for (int c = 0; c < dt.Columns.Count; c++)
            {
                columnWidths[c] = Printer.GetWidth(dt.Columns[c].ColumnName, queue.font);

                for (int r = 0; r < dt.Rows.Count; r++)
                {
                    if (Printer.GetWidth(dt.Rows[r][c].ToString(), queue.font) > columnWidths[c])
                        columnWidths[c] = Printer.GetWidth(dt.Rows[r][c].ToString(), queue.font);
                }
            }

            int x = 0;
            for (int c = 0; c < dt.Columns.Count; c++)
            {
                queue.AddBold(dt.Columns[c].ColumnName, x);

                x += (int)columnWidths[c] + buffer;
            }

            queue.AddLine();

            for (int r = 0; r < dt.Rows.Count; r++)
            {
                x = 0;
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    queue.Add(dt.Rows[r][c].ToString(), x);

                    x += (int)columnWidths[c] + buffer;
                }

                queue.AddLine();
            }

            queue.Print();
        }
    }

    public abstract class PrintItem
    {
        public float Width, Height, X;
        public bool newLine = false;
    }

    public class StringPrintItem : PrintItem
    {
        public string Text;
        public bool Bold = false;

        public StringPrintItem(string text)
        {
            Text = text;
            X = -1F;
        }

        public StringPrintItem(string text, int x)
        {
            Text = text;
            X = (float)x;
        }
    }

    public class ImagePrintItem : PrintItem
    {
        public Image picture;

        public ImagePrintItem(Image image)
        {
            picture = image;
            Width = image.Width;
            Height = image.Height;
            X = -1F;
        }

        public ImagePrintItem(Image image, int x)
        {
            picture = image;
            Width = image.Width;
            Height = image.Height;
            X = (float)x;
        }

        public ImagePrintItem(Image image, int w, int h)
        {
            picture = image;
            Width = w;
            Height = h;
            X = -1F;
        }

        public ImagePrintItem(Image image, int x, int w, int h)
        {
            picture = image;
            Width = w;
            Height = h;
            X = (float)x;
        }

        public ImagePrintItem(Image image, double percentSize)
        {
            picture = image;
            Width = image.Width * (float)percentSize;
            Height = image.Height * (float)percentSize;
            X = -1F;
        }

        public ImagePrintItem(Image image, int x, double percentSize)
        {
            picture = image;
            Width = image.Width * (float)percentSize;
            Height = image.Height * (float)percentSize;
            X = x;
        }
    }

    public class DividerPrintItem : PrintItem
    {
        public DividerPrintItem()
        {

        }
    }

    public class PagePrintItem : PrintItem
    {
        public PagePrintItem()
        {

        }
    }
}
