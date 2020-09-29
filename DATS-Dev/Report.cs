using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DATS_Timesheets
{
    public partial class Report : Form
    {
        DataTable original = null;
        DataTable storage = null;
        bool updatable = false;
        public char separator = '쓰'; //Something you don't expect to see in everyday text

        public Report(string title, DataTable dt)
        {
            InitializeComponent();

            Text = title;
            toolStripStatusLabel1.Text = "";

            original = dt;
            storage = dt;

            grid.DataTable = dt;
            grid.Selection.EnableMultiSelection = true;

            toolStripStatusLabel1.Text = "Returned " + grid.DataTable.Rows.Count + " rows";

            SourceGrid.Cells.Views.ColumnHeader headerView = new SourceGrid.Cells.Views.ColumnHeader();
            headerView.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            headerView.ForeColor = Color.FromArgb(38, 38, 38); //Off-black

            CellBackColorAlternate alternatingView = new CellBackColorAlternate(Color.Khaki, Color.DarkKhaki);
            PopupMenu menuController = new PopupMenu(this);
            ValueChangedEvent valueChangedController = new ValueChangedEvent();
            Font font = new Font(menuStrip1.Font.FontFamily, 12);

            SourceGrid.Cells.Views.Link alternateView = new SourceGrid.Cells.Views.Link();
            alternateView.BackColor = Color.FromArgb(0, 0, 255); //Off-black

            SourceGrid.Cells.Views.Cell numericView = new SourceGrid.Cells.Views.Cell();
            numericView.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleRight;

            bool addBefore = grid.Columns.Count > 25;

            if (addBefore)
                addCustomOption(font);

            //Width of columns
            for (int i = 0; i < grid.Columns.Count; i++)
            {
                grid.Columns[i].AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize;
                grid.GetCell(0, i).View = headerView;
                grid.GetCell(0, i).AddController(menuController);
                grid.GetCell(1, i).AddController(menuController);
                grid.GetCell(1, i).AddController(valueChangedController);
                grid.GetCell(1, i).Editor.EnableEdit = false;

                Type type = grid.DataTable.Columns[i].DataType;
                bool isString = type == typeof(String) || type == typeof(DateTime);

                if (type == typeof(int) || type == typeof(double) || type == typeof(decimal))
                    grid.GetCell(1, i).View = numericView;

                ToolStripMenuItem item = new ToolStripMenuItem(grid.Columns[i].PropertyName);
                item.Font = font;

                ToolStripMenuItem temp = new ToolStripMenuItem("Is");
                temp.Image = Properties.Resources.EqualsIcon;
                temp.Click += Equals_Click;
                item.DropDownItems.Add(temp);

                temp = new ToolStripMenuItem("Is not");
                temp.Image = Properties.Resources.NotEquals;
                temp.Click += NotEquals_Click;
                item.DropDownItems.Add(temp);

                if (isString)
                {
                    temp = new ToolStripMenuItem("Contains");
                    temp.Image = Properties.Resources.Contains;
                    temp.Click += Contains_Click;
                    item.DropDownItems.Add(temp);

                    temp = new ToolStripMenuItem("Does not contain");
                    temp.Image = Properties.Resources.NotContains;
                    temp.Click += NotContains_Click;
                    item.DropDownItems.Add(temp);
                }

                temp = new ToolStripMenuItem("In list");
                temp.Image = Properties.Resources.List;
                temp.Click += List_Click;
                item.DropDownItems.Add(temp);

                temp = new ToolStripMenuItem("Not in list");
                temp.Image = Properties.Resources.NotList;
                temp.Click += NotList_Click;
                item.DropDownItems.Add(temp);

                temp = new ToolStripMenuItem("Less than");
                temp.Image = Properties.Resources.LessThan;
                temp.Click += LessThan_Click;
                item.DropDownItems.Add(temp);

                temp = new ToolStripMenuItem("Less than or equal to");
                temp.Image = Properties.Resources.LeEquals;
                temp.Click += LessThanEq_Click;
                item.DropDownItems.Add(temp);

                temp = new ToolStripMenuItem("Greater than");
                temp.Image = Properties.Resources.GreaterThan;
                temp.Click += GreaterThan_Click;
                item.DropDownItems.Add(temp);

                temp = new ToolStripMenuItem("Greater than or equal to");
                temp.Image = Properties.Resources.GrEquals;
                temp.Click += GreaterThanEq_Click;
                item.DropDownItems.Add(temp);

                toolStripMenuItem2.DropDownItems.Add(item);
            }

            if (!addBefore)
                addCustomOption(font);

            grid.AutoSizeCells();
            grid.Focus();
        }

        private void addCustomOption(Font font)
        {
            ToolStripMenuItem customItem = new ToolStripMenuItem("Custom");
            customItem.Font = font;
            customItem.Image = Properties.Resources.Custom;
            customItem.Click += whereToolStripMenuItem_Click;
            toolStripMenuItem2.DropDownItems.Add(customItem);
        }

        public void GreaterThanEq_Click(object sender, EventArgs e)
        {
            string column = "";

            if (sender is ToolStripMenuItem)
                column = (sender as ToolStripMenuItem).OwnerItem.Text;
            else if (sender is MenuItem)
                column = (sender as MenuItem).Tag.ToString();

            bool success;
            string filter = getFilterFromType(column, ">=", "is greater than or equal to", out success);

            if (success)
            {
                grid.DataTable = Report.filter(grid.DataTable, filter, out success);

                addUnfilter(filter);
            }
        }

        public void GreaterThan_Click(object sender, EventArgs e)
        {
            string column = "";

            if (sender is ToolStripMenuItem)
                column = (sender as ToolStripMenuItem).OwnerItem.Text;
            else if (sender is MenuItem)
                column = (sender as MenuItem).Tag.ToString();

            bool success;
            string filter = getFilterFromType(column, ">", "is greater than", out success);

            if (success)
            {
                grid.DataTable = Report.filter(grid.DataTable, filter, out success);

                addUnfilter(filter);
            }
        }

        public void LessThanEq_Click(object sender, EventArgs e)
        {
            string column = "";

            if (sender is ToolStripMenuItem)
                column = (sender as ToolStripMenuItem).OwnerItem.Text;
            else if (sender is MenuItem)
                column = (sender as MenuItem).Tag.ToString();

            bool success;
            string filter = getFilterFromType(column, "<=", "is less than or equal to", out success);

            if (success)
            {
                grid.DataTable = Report.filter(grid.DataTable, filter, out success);

                addUnfilter(filter);
            }
        }

        public void LessThan_Click(object sender, EventArgs e)
        {
            string column = "";

            if (sender is ToolStripMenuItem)
                column = (sender as ToolStripMenuItem).OwnerItem.Text;
            else if (sender is MenuItem)
                column = (sender as MenuItem).Tag.ToString();

            bool success;
            string filter = getFilterFromType(column, "<", "is less than", out success);

            if (success)
            {
                grid.DataTable = Report.filter(grid.DataTable, filter, out success);

                addUnfilter(filter);
            }
        }

        public void NotList_Click(object sender, EventArgs e)
        {
            string column = "";

            if (sender is ToolStripMenuItem)
                column = (sender as ToolStripMenuItem).OwnerItem.Text;
            else if (sender is MenuItem)
                column = (sender as MenuItem).Tag.ToString();

            bool success;
            string filter = getFilterFromType(column, "not in", "not list", out success);

            if (success)
            {
                grid.DataTable = Report.filter(grid.DataTable, filter, out success);

                addUnfilter(filter);
            }
        }

        public void List_Click(object sender, EventArgs e)
        {
            string column = "";

            if (sender is ToolStripMenuItem)
                column = (sender as ToolStripMenuItem).OwnerItem.Text;
            else if (sender is MenuItem)
                column = (sender as MenuItem).Tag.ToString();

            bool success;
            string filter = getFilterFromType(column, "in", "list", out success);

            if (success)
            {
                grid.DataTable = Report.filter(grid.DataTable, filter, out success);

                addUnfilter(filter);
            }
        }

        public void NotContains_Click(object sender, EventArgs e)
        {
            string column = "";

            if (sender is ToolStripMenuItem)
                column = (sender as ToolStripMenuItem).OwnerItem.Text;
            else if (sender is MenuItem)
                column = (sender as MenuItem).Tag.ToString();

            bool success;
            string filter = getFilterFromType(column, "not like", "does not contain", out success);

            if (success)
            {
                grid.DataTable = Report.filter(grid.DataTable, filter, out success);

                addUnfilter(filter);
            }
        }

        public void Contains_Click(object sender, EventArgs e)
        {
            string column = "";

            if (sender is ToolStripMenuItem)
                column = (sender as ToolStripMenuItem).OwnerItem.Text;
            else if (sender is MenuItem)
                column = (sender as MenuItem).Tag.ToString();

            bool success;
            string filter = getFilterFromType(column, "like", "contains", out success);

            if (success)
            {
                grid.DataTable = Report.filter(grid.DataTable, filter, out success);

                addUnfilter(filter);
            }
        }

        public void NotEquals_Click(object sender, EventArgs e)
        {
            string column = "";

            if (sender is ToolStripMenuItem)
                column = (sender as ToolStripMenuItem).OwnerItem.Text;
            else if (sender is MenuItem)
                column = (sender as MenuItem).Tag.ToString();

            bool success;
            string filter = getFilterFromType(column, "<>", "is not", out success);

            if (success)
            {
                grid.DataTable = Report.filter(grid.DataTable, filter, out success);

                addUnfilter(filter);
            }
        }

        public void Equals_Click(object sender, EventArgs e)
        {
            string column = "";

            if (sender is ToolStripMenuItem)
                column = (sender as ToolStripMenuItem).OwnerItem.Text;
            else if (sender is MenuItem)
                column = (sender as MenuItem).Tag.ToString();

            bool success;
            string filter = getFilterFromType(column, "=", "is", out success);

            if (success)
            {
                grid.DataTable = Report.filter(grid.DataTable, filter, out success);

                addUnfilter(filter);
            }
        }

        public string getFilterFromType(string column, string code, string wording, out bool success)
        {
            string filter = "";
            string value = "";
            Type type = grid.DataTable.Columns[column.Split(separator)[0]].DataType;
            bool useQuotes = (type == typeof(string) || type == typeof(DateTime));
            bool useBrackets = code == "in" || code == "not in";
            bool useWildcards = code == "like" || code == "not like";

            if (code == "in" || code == "not in")
            {
                string output = Core.getString("Enter your list, one item per line, no punctuation.", true, out success);

                for (int i = 0; i < output.Split('\n').Length; i++)
                    value += (i != 0 ? ", " : "") + "'" + output.Split('\n')[i].Trim() + "'";

                useQuotes = false; //Already taken care of
            }
            else if (column.Contains(separator))
            {
                string[] temp = column.Split(separator);
                column = temp[0];
                value = temp[1];
                success = true;
            }
            else if (type == typeof(string))
                value = Core.getString("Only show records where " + column + " " + wording + " what value?", out success).Trim();
            else if (type == typeof(DateTime))
                value = Core.getDate("Only show records where " + column + " " + wording + " what value?", out success).ToString("yyyy-MM-dd");
            else if (type == typeof(short) || type == typeof(int) || type == typeof(long)) //Int16, Int32, Int64
                value = Core.getInt("Only show records where " + column + " " + wording + " what value?", out success).ToString();
            else if (type == typeof(double) || type == typeof(decimal))
                value = Core.getDouble("Only show records where " + column + " " + wording + " what value?", out success).ToString();
            else
            {
                success = false;
                MessageBox.Show("Data type not recognized.");
            }

            filter = value;
            filter = (useWildcards ? "%" : "") + filter + (useWildcards ? "%" : "");
            filter = (useQuotes ? "'" : "") + filter + (useQuotes ? "'" : "");
            filter = (useBrackets ? "(" : "") + filter + (useBrackets ? ")" : "");
            filter = "[" + column + "] " + code + " " + filter;

            return filter;
        }

        public Report(string title, Database db) : this(title, db.Run())
        {
            string query = db.Query.ToLower();

            if (query.Contains("join") || query.Contains("group") || !query.Contains("select"))
                throw new Exception("Query not formatted for updatable report.");

            string tableName = query.Substring(9).Split(' ')[1];
            grid.db = db;
            grid.DataTable.TableName = tableName;

            SourceGrid.Cells.Controllers.ToolTipText toolTipController = new SourceGrid.Cells.Controllers.ToolTipText();
            toolTipController.ToolTipTitle = "ToolTip example";
            toolTipController.ToolTipIcon = ToolTipIcon.Info;
            toolTipController.IsBalloon = true;

            //for (int i = 0; i < grid.Columns.Count; i++)
            //{
            //    grid.GetCell(1, i).Editor.EnableEdit = true;

            //    if (db is Oracle) //JDE UDC lookup
            //    {
            //        string dataItem = grid.Columns[i].PropertyName.Substring(2);

            //        bool hasLookup = Oracle.Run("select * from DD910.F9210 where FRDTAI = @DATAITEM and trim(FROWER) = 'UDC'", dataItem).Rows.Count > 0;

            //        if (hasLookup)
            //        {
            //            grid.GetCell(1, i).AddController(toolTipController);
            //        }
            //    }
            //}

            grid.Focus();
        }

        private void Report_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();

            if (!e.Control && e.KeyCode == Keys.Home)
            {
                SourceGrid.Position current = grid.Selection.ActivePosition;
                SourceGrid.Position now = new SourceGrid.Position(current.Row, 0);

                grid.Selection.Focus(now, true);
            }

            if (!e.Control && e.KeyCode == Keys.End)
            {
                SourceGrid.Position current = grid.Selection.ActivePosition;

                if (current.Column == -1 || current.Row == -1)
                    return;

                SourceGrid.Position now = new SourceGrid.Position(current.Row, grid.Columns.Count - 1);

                grid.Selection.Focus(now, true);
            }

            if (e.Control && e.KeyCode == Keys.Home)
            {
                SourceGrid.Position current = grid.Selection.ActivePosition;
                SourceGrid.Position now = new SourceGrid.Position(1, current.Column);

                grid.Selection.Focus(now, true);
            }

            if (e.Control && e.KeyCode == Keys.End)
            {
                SourceGrid.Position current = grid.Selection.ActivePosition;

                if (current.Column == -1 || current.Row == -1)
                    return;

                SourceGrid.Position now = new SourceGrid.Position(grid.Rows.Count - 1, current.Column);

                grid.Selection.Focus(now, true);
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Core.export(Text, grid.DataTable);
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Printer.PrintListView(Text, list);
            Printer.PrintDataTable(Text, grid.DataTable);
        }

        private void Report_Shown(object sender, EventArgs e)
        {
            if (storage.Rows.Count == 0)
            {
                Hide();
                MessageBox.Show("The report returned no information.");
                Close();
            }
        }

        private void list_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (list.SelectedIndices.Count > 0)
            //{
            //    toolStripStatusLabel1.Text = "Row " + (list.SelectedIndices[0] + 1) + " of " + storage.Rows.Count;

            //    if (list.SelectedIndices[0] + 1 == list.Items.Count)
            //    {
            //        for(int i = 1; i <= 100; i++)
            //            loadItem(list.SelectedIndices[0] + i);
            //    }
            //}
        }

        private void whereToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string x = "";
            bool success = false;

            //Where filterForm = new Where(grid.DataTable);
            //filterForm.ShowDialog();

            do
            {
                x = Core.getString("Where...", x, out success);

                if (!success)
                    return;

                grid.DataTable = filter(grid.DataTable, x, out success);

                if (success)
                    addUnfilter(x);
            } while (!success);
        }

        public void addUnfilter(string text)
        {
            unfilterToolStripMenuItem.Visible = true;
            Font font = new Font(menuStrip1.Font.FontFamily, 12);

            if (unfilterToolStripMenuItem.DropDownItems.Count == 0)
            {
                ToolStripMenuItem removeAll = new ToolStripMenuItem();
                removeAll.Text = "Remove all filters";
                removeAll.Image = Properties.Resources.False.ToBitmap();
                removeAll.Click += Item_Click_All;
                removeAll.Font = font;
                unfilterToolStripMenuItem.DropDownItems.Add(removeAll);

                ToolStripSeparator line = new ToolStripSeparator();
                unfilterToolStripMenuItem.DropDownItems.Add(line);
            }

            ToolStripMenuItem item = new ToolStripMenuItem();
            item.Text = text;
            item.Image = Properties.Resources.False.ToBitmap();
            item.Click += Item_Click;
            item.Tag = "Dynamic";
            item.Font = font;
            unfilterToolStripMenuItem.DropDownItems.Add(item);
        }

        private void Item_Click(object sender, EventArgs e)
        {
            unfilterToolStripMenuItem.DropDownItems.Remove(sender as ToolStripMenuItem);
            DataTable temp = original;
            int count = 0;

            for (int i = 0; i < unfilterToolStripMenuItem.DropDownItems.Count; i++)
            {
                if (unfilterToolStripMenuItem.DropDownItems[i].Tag != null && unfilterToolStripMenuItem.DropDownItems[i].Tag.ToString() == "Dynamic")
                {
                    DataRow[] result = temp.Select(unfilterToolStripMenuItem.DropDownItems[i].Text);

                    if (result.Length > 0)
                        temp = result.CopyToDataTable();
                    else
                        temp.Rows.Clear();

                    count++;
                }
            }

            if (count == 0)
            {
                unfilterToolStripMenuItem.DropDownItems.Clear();
                unfilterToolStripMenuItem.Visible = false;
            }

            grid.DataTable = temp;
        }

        private void Item_Click_All(object sender, EventArgs e)
        {
            unfilterToolStripMenuItem.DropDownItems.Clear();
            unfilterToolStripMenuItem.Visible = false;

            grid.DataTable = original;
        }

        public static DataTable filter(DataTable dt, string filter, out bool success)
        {
            try
            {
                DataRow[] result = dt.Select(filter);

                if (result.Length > 0)
                    dt = result.CopyToDataTable();
                else
                    dt.Rows.Clear();

                success = true;
            }
            catch (Exception e2)
            {
                MessageBox.Show("Something didn't quite work right. Possible syntax error." + Environment.NewLine
                    + Environment.NewLine
                    + Core.getErrorText(e2));

                success = false;
            }

            return dt;
        }

        private void unfilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //storage = original;
            //grid.DataTable = storage;
        }
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (grid.DataTable.Rows.Count == original.Rows.Count)
                toolStripStatusLabel1.Text = "Returned " + grid.DataTable.Rows.Count + " rows";
            else
                toolStripStatusLabel1.Text = "Showing " + grid.DataTable.Rows.Count + " of " + original.Rows.Count + " rows";
        }

        private void toolStripProgressBar1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {

        }

        private void testToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }
    }

    public class CellBackColorAlternate : SourceGrid.Cells.Views.Cell
    {
        public CellBackColorAlternate(Color firstColor, Color secondColor)
        {
            FirstBackground = new DevAge.Drawing.VisualElements.BackgroundSolid(firstColor);
            SecondBackground = new DevAge.Drawing.VisualElements.BackgroundSolid(secondColor);
        }

        private DevAge.Drawing.VisualElements.IVisualElement mFirstBackground;
        public DevAge.Drawing.VisualElements.IVisualElement FirstBackground
        {
            get { return mFirstBackground; }
            set { mFirstBackground = value; }
        }

        private DevAge.Drawing.VisualElements.IVisualElement mSecondBackground;
        public DevAge.Drawing.VisualElements.IVisualElement SecondBackground
        {
            get { return mSecondBackground; }
            set { mSecondBackground = value; }
        }

        protected override void PrepareView(SourceGrid.CellContext context)
        {
            base.PrepareView(context);

            if (Math.IEEERemainder(context.Position.Row, 2) == 0)
                Background = FirstBackground;
            else
                Background = SecondBackground;
        }
    }

    public class PopupMenu : SourceGrid.Cells.Controllers.ControllerBase
    {
        Report r;
        ContextMenu menu = new ContextMenu();
        SourceGrid.CellContext cell;

        public PopupMenu(Report sender)
        {
            this.r = sender;
        }

        public override void OnMouseUp(SourceGrid.CellContext sender, MouseEventArgs e)
        {
            base.OnMouseUp(sender, e);
            cell = sender;

            if (e.Button == MouseButtons.Right)
            {
                SourceGrid.DataGrid grid = cell.Grid as SourceGrid.DataGrid;
                SourceGrid.Cells.ICellVirtual header = grid.GetCell(0, cell.Position.Column);
                Type type;

                try
                {
                    type = grid.DataTable.Columns[cell.Position.Column].DataType;
                }
                catch
                {
                    return;
                }

                bool isString = type == typeof(String) || type == typeof(DateTime);

                string column = header.Model.ValueModel.GetValue(cell).ToString();
                string value = cell.DisplayText;

                menu.MenuItems.Clear();

                bool isUDCCol = isUDCColumn(column);

                if (isUDCCol)
                {
                    if (cell.Position.Row == 0)
                    {
                        string columnDescription = getJDEDesc(column);
                        menu.MenuItems.Add("\"" + columnDescription + "\"", new EventHandler(UDC_Click));
                        menu.MenuItems.Add("-");
                    }
                    else
                    {
                        string udcValue = getUDCValue(column, value);
                        menu.MenuItems.Add("\"" + udcValue + "\"", new EventHandler(UDC_Click));
                        menu.MenuItems.Add("-");
                    }
                }
                else
                {
                    if (cell.Position.Row == 0)
                    {
                        string columnDescription = getJDEDesc(column);
                        menu.MenuItems.Add("\"" + columnDescription + "\"");
                        menu.MenuItems.Add("-");
                    }
                }

                MenuItem item;

                if (cell.Position.Row == 0) //If header
                {
                    item = new MenuItem("Filter where \"" + column + "\" is a specific value", new EventHandler(r.Equals_Click));
                    item.Tag = column;
                    menu.MenuItems.Add(item);

                    item = new MenuItem("Filter where \"" + column + "\" is not a specific value", new EventHandler(r.NotEquals_Click));
                    item.Tag = column;
                    menu.MenuItems.Add(item);

                    if (isString)
                    {
                        item = new MenuItem("Filter where \"" + column + "\" contains specific text", new EventHandler(r.Contains_Click));
                        item.Tag = column;
                        menu.MenuItems.Add(item);

                        item = new MenuItem("Filter where \"" + column + "\" does not contain specific text", new EventHandler(r.NotContains_Click));
                        item.Tag = column;
                        menu.MenuItems.Add(item);
                    }

                    item = new MenuItem("Filter where \"" + column + "\" is within a list", new EventHandler(r.List_Click));
                    item.Tag = column;
                    menu.MenuItems.Add(item);

                    item = new MenuItem("Filter where \"" + column + "\" is not within a list", new EventHandler(r.NotList_Click));
                    item.Tag = column;
                    menu.MenuItems.Add(item);

                    item = new MenuItem("Filter where \"" + column + "\" is less than a specific value", new EventHandler(r.LessThan_Click));
                    item.Tag = column;
                    menu.MenuItems.Add(item);

                    item = new MenuItem("Filter where \"" + column + "\" is less than or equal to a specific value", new EventHandler(r.LessThanEq_Click));
                    item.Tag = column;
                    menu.MenuItems.Add(item);

                    item = new MenuItem("Filter where \"" + column + "\" is greater than a specific value", new EventHandler(r.GreaterThan_Click));
                    item.Tag = column;
                    menu.MenuItems.Add(item);

                    item = new MenuItem("Filter where \"" + column + "\" is greater than or equal to a specific value", new EventHandler(r.GreaterThanEq_Click));
                    item.Tag = column;
                    menu.MenuItems.Add(item);
                }
                else if (cell.Position.Row > 0) //If on a specific cell
                {
                    item = new MenuItem("Filter where \"" + column + "\" is \"" + value + "\"", new EventHandler(r.Equals_Click));
                    item.Tag = column + r.separator + value;
                    menu.MenuItems.Add(item);

                    item = new MenuItem("Filter where \"" + column + "\" is not \"" + value + "\"", new EventHandler(r.NotEquals_Click));
                    item.Tag = column + r.separator + value;
                    menu.MenuItems.Add(item);

                    item = new MenuItem("Filter where \"" + column + "\" is less than \"" + value + "\"", new EventHandler(r.LessThan_Click));
                    item.Tag = column + r.separator + value;
                    menu.MenuItems.Add(item);

                    item = new MenuItem("Filter where \"" + column + "\" is less than or equal to \"" + value + "\"", new EventHandler(r.LessThanEq_Click));
                    item.Tag = column + r.separator + value;
                    menu.MenuItems.Add(item);

                    item = new MenuItem("Filter where \"" + column + "\" is greater than \"" + value + "\"", new EventHandler(r.GreaterThan_Click));
                    item.Tag = column + r.separator + value;
                    menu.MenuItems.Add(item);

                    item = new MenuItem("Filter where \"" + column + "\" is greater than or equal to \"" + value + "\"", new EventHandler(r.GreaterThanEq_Click));
                    item.Tag = column + r.separator + value;
                    menu.MenuItems.Add(item);
                }

                menu.Show(sender.Grid, new Point(e.X, e.Y));
            }
        }

        private string getJDEDesc(string column)
        {
            try
            {
                return Oracle.RunString(@"
SELECT min(DD910.F9202.FRDSCR)
FROM DD910.F9202
WHERE trim(FRDTAI) = '" + column.Substring(2) + "'").Trim();
            }
            catch
            {
                return "";
            }
        }

        private bool isUDCColumn(string column)
        {
            try
            {
                string dataItem = column.Substring(2);
                DataTable dt = Oracle.Run("select FROER1, FROER2 from DD910.F9210 where trim(FRDTAI) = '" + dataItem + "' and trim(FROWER) = 'UDC'");

                if (dt.Rows.Count > 0)
                    return true;
            }
            catch
            {

            }

            return false;
        }

        private string getUDCValue(string column, string value)
        {
            try
            {
                string dataItem = column.Substring(2);
                DataTable dt = Oracle.Run("select FROER1, FROER2 from DD910.F9210 where trim(FRDTAI) = '" + dataItem + "' and trim(FROWER) = 'UDC'");

                if (dt.Rows.Count > 0)
                {
                    string udc1 = dt.Rows[0]["FROER1"].ToString().Trim();
                    string udc2 = dt.Rows[0]["FROER2"].ToString().Trim();

                    return Oracle.RunString("select DRDL01 from PRODCTL.F0005 where trim(DRSY) = @UDC1 and trim(DRRT) = @UDC2 and trim(DRKY) = @UDCVAL", udc1, udc2, value.Trim()).Trim();
                }
                else
                {

                }
            }
            catch (Exception e)
            {

            }

            return "";
        }

        private void UDC_Click(object sender, EventArgs e)
        {
            SourceGrid.DataGrid grid = cell.Grid as SourceGrid.DataGrid;
            SourceGrid.Cells.ICellVirtual header = grid.GetCell(0, cell.Position.Column);

            string column = header.Model.ValueModel.GetValue(cell).ToString();

            string dataItem = column.Substring(2);
            DataTable dt = Oracle.Run("select FROER1, FROER2 from DD910.F9210 where trim(FRDTAI) = '" + dataItem + "' and trim(FROWER) = 'UDC'");

            if (dt.Rows.Count > 0)
            {
                string udc1 = dt.Rows[0]["FROER1"].ToString().Trim();
                string udc2 = dt.Rows[0]["FROER2"].ToString().Trim();

                dt = Oracle.Run("select * from PRODCTL.F0005 where trim(DRSY) = @UDC1 and trim(DRRT) = @UDC2 order by DRKY", udc1, udc2);

                Report r = new Report("UDC Lookup " + udc1 + "/" + udc2, dt);
                r.Show();
            }
        }

        private void Copy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(cell.DisplayText);
        }
    }

    public class ValueChangedEvent : SourceGrid.Cells.Controllers.ControllerBase
    {
        public override void OnValueChanged(SourceGrid.CellContext sender, EventArgs e)
        {
            base.OnValueChanged(sender, e);

            //SourceGrid.DataGrid grid = sender.Grid as SourceGrid.DataGrid;
            //SourceGrid.Cells.ICellVirtual header = grid.GetCell(0, sender.Position.Column);

            //if (grid.db != null && Core.getUsername() == "Graeme Smyth")
            //{
            //    string column = header.Model.ValueModel.GetValue(sender).ToString();
            //    string value = sender.DisplayText;
            //    Type type = grid.DataTable.Columns[sender.Position.Column].DataType;
            //    string tableName = grid.DataTable.TableName.ToUpper();
            //    string schema = "";
            //    string filter = "";

            //    char open = ' ';
            //    char close = ' ';

            //    if (tableName.Contains('.'))
            //    {
            //        schema = tableName.Split('.')[0];
            //        tableName = tableName.Split('.')[1];
            //    }

            //    DataTable pk = null;

            //    if (grid.db.GetType() == DatabaseType.SQL)
            //    {
            //        open = '[';
            //        close = ']';

            //        grid.db.Query = @"
            //            SELECT column_name
            //            FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
            //            WHERE OBJECTPROPERTY(OBJECT_ID(constraint_name), 'IsPrimaryKey') = 1
            //            AND table_name = '" + tableName + "'";

            //        if (schema != "")
            //            grid.db.Query += " and table_schema = '" + schema + "'";
            //    }
            //    else if (grid.db.GetType() == DatabaseType.Oracle)
            //    {
            //        open = '"';
            //        close = '"';

            //        grid.db.Query = @"
            //            SELECT trim(all_tab_cols.column_name) as ""column_name""

            //            FROM ALL_TAB_COLS
            //            LEFT JOIN ALL_CONS_COLUMNS ON ALL_CONS_COLUMNS.COLUMN_NAME = ALL_TAB_COLS.COLUMN_NAME AND ALL_CONS_COLUMNS.TABLE_NAME = ALL_TAB_COLS.TABLE_NAME AND ALL_CONS_COLUMNS.OWNER = ALL_TAB_COLS.OWNER
            //            LEFT JOIN ALL_CONSTRAINTS ON ALL_CONSTRAINTS.CONSTRAINT_NAME = ALL_CONS_COLUMNS.CONSTRAINT_NAME AND ALL_CONSTRAINTS.OWNER = ALL_CONS_COLUMNS.OWNER AND ALL_CONSTRAINTS.TABLE_NAME = ALL_CONS_COLUMNS.TABLE_NAME

            //            WHERE upper(trim(all_constraints.table_name)) = '" + tableName + @"'
            //            AND ALL_CONSTRAINTS.CONSTRAINT_TYPE = 'P'";

            //        if (schema != "")
            //            grid.db.Query += " AND upper(ALL_TAB_COLS.OWNER) = '" + schema + "'";
            //    }
            //    else
            //        throw new Exception("Database type not recognized.");

            //    pk = grid.db.Run();

            //    if (pk.Rows.Count == 0)
            //    {
            //        MessageBox.Show("Unable to update " + (schema != "" ? open + schema + close + "." : "") + open + tableName + close + ", as a primary key could not be found." );
            //        return;
            //    }

            //    for (int i = 0; i < pk.Rows.Count; i++)
            //    {
            //        filter += (i != 0 ? "AND " + open : "" + open) + pk.Rows[i]["column_name"].ToString() + close + " = '" + grid.OriginalDataTable.Rows[sender.Position.Row - 1][pk.Rows[i]["column_name"].ToString()].ToString() + "' ";
            //        grid.OriginalDataTable.Rows[sender.Position.Row - 1][pk.Rows[i]["column_name"].ToString()] = grid.DataTable.Rows[sender.Position.Row - 1][pk.Rows[i]["column_name"].ToString()].ToString();
            //    }

            //    string str = "update " + (schema != "" ? open + schema + close + "." : "") + open + tableName + close + " set " + open + column + close + " = '" + value + "' where " + filter;
            //    Core.log("Report", "Update test", str);

            //    //Work on this next.
            //    //MessageBox.Show(str);
            //    grid.db.Run(str);
            //}
        }
    }
}
