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
    public partial class Entitlements : Form
    {
        int empID = -1;
        string username = "";

        public Entitlements(int employeeID)
        {
            InitializeComponent();

            empID = employeeID;
            username = Core.getUsernameFromEmpID(empID);

            refresh();
        }
        
        public void refresh()
        {
            Core.fillListView(listView1, SQL.Run("select Year, Type, Entitlement, '' as Balance from entitlements where employeeid = @EMPID", empID));
            double balance = 0;
            for(int i = 0; i < listView1.Items.Count; i++)
             {
                int year = int.Parse(listView1.Items[i].SubItems[listView1.Columns.IndexOfKey("Year")].Text);
                string type = listView1.Items[i].SubItems[listView1.Columns.IndexOfKey("Type")].Text;
                double entitlement = double.Parse(listView1.Items[i].SubItems[listView1.Columns.IndexOfKey("Entitlement")].Text);
                //double balance = (type == "Vacation" ? Core.getVacationBalance(year, empID, year < DateTime.Now.Year) : Core.getBankedVacationBalance(empID));
                // MCL Code added Aug 2020
                switch (type)
                {
                    case "Vacation":
                        balance = Core.getVacationBalance(year, empID, year < DateTime.Now.Year);
                        break;
                    case "MCL Vacation":
                        balance = Core.getMCLBalance(year, empID, year < DateTime.Now.Year);
                        break;
                    default:
                        balance = Core.getBankedVacationBalance(empID);
                        break;
                }


                listView1.Items[i].SubItems[listView1.Columns.IndexOfKey("Balance")].Text = balance.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // MCL Code added Aug 2020
            if (Core.isAdmin(Core.getUsername()))
            {
                EntitlementType entType = new EntitlementType(username,empID);
                entType.ShowDialog();
                refresh();
            }

            /*    bool success;
                int year = Core.getInt("Enter vacation year", 1990, 2100, DateTime.Today.Year, out success);

                if (!success)
                    return;

                int count = SQL.Run("select * from entitlements where year = @YEAR and type = 'Vacation'", year).Rows.Count;

                if(count > 0)
                {
                    MessageBox.Show(username + " already has an entitlement entry for " + year + "." + Environment.NewLine
                        + "Please view it on the left side of the Entitlements screen.");
                    return;
                }

                double value = Core.getDouble("Enter " + username + " 's " + year + " vacation entitlement", 0, 1000, 0, out success);

                if (!success)
                    return;

                SQL.Run("insert into entitlements values (@EMPID, @YEAR, @TYPE, @ENTITLEMENT)", empID, year, "Vacation", value);
                Core.logHistory("Entitlement added", year + " Vacation = " + value + " for " + username, "");

                refresh();
            */
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string type = "";
            int year = -1;
            DataTable dt = null;
            double entitlementValue = -1;

            if (type == "Vacation")
            {
                entitlementValue = double.Parse(SQL.RunString("select entitlement from entitlements where employeeid = @EMPID and year = @YEAR and type = @TYPE", empID, year, type));

                DateTime start = new DateTime(year, 1, 1);
                dt = SQL.Run(@"
/*Prior Year used this year*/
select @YEAR as 'Year', 'Vacation' as 'Type', a.dateentered as 'Date Entered', a.dateworked as 'Date Worked', -a.hours as 'Net Change', '' as 'Balance' from
(select DateEntered, dateworked, hours from Timesheets
where employeeid = @EMPID1
and paytype in (807, 808)
and dateworked >= @START1
and dateworked < @END1) a

union

/*Current Year used last year*/
select @YEAR as 'Year', 'Vacation' as 'Type', a.dateentered as 'Date Entered', a.dateworked as 'Date Worked', -a.hours as 'Net Change', '' as 'Balance' from
(select DateEntered, dateworked, hours from Timesheets t
where employeeid = @EMPID2
and paytype in (809, 821, 810)
and dateworked >= @START2
and dateworked < @END2) a
", year, empID, start, start.AddYears(1), year, empID, start.AddYears(1), start.AddYears(2));

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    double netChange = double.Parse(dt.Rows[i]["Hours"].ToString());
                    entitlementValue += netChange;

                    dt.Rows[i]["Balance"] = entitlementValue;
                }
            }
            
            Report r = new Report("Entitlement Balance for " + username, dt);
            r.Show();
        }

        private void ViewAccounts_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                int year = int.Parse(listView1.SelectedItems[0].SubItems[listView1.Columns.IndexOfKey("Year")].Text);
                string type = listView1.SelectedItems[0].SubItems[listView1.Columns.IndexOfKey("Type")].Text;
                double oldValue = double.Parse(listView1.SelectedItems[0].SubItems[listView1.Columns.IndexOfKey("Entitlement")].Text);
                double balance = -1;

                bool success;

                double newValue = Core.getDouble("Enter new entitlement value", 0, 1000, oldValue, out success);

                if (type == "Banked Vacation")
                    balance = Core.getBankedVacationBalance(empID);
                else
                    balance = Core.getVacationBalance(year, empID, year < DateTime.Now.Year);
                
                if (!success)
                    return;

                if (balance - oldValue + newValue < 0)
                {
                    MessageBox.Show(username + " has " + balance + " hours of " + (year != 0 ? year + " " : "") + type + ". This change would bring them below 0. Please try a different number.");
                    return;
                }
                
                SQL.Run("update entitlements set entitlement = @NEWVALUE where year = @YEAR and type = @TYPE and employeeid = @EMPID", newValue, year, type, empID);
                Core.logHistory("Entitlement edited", (year != 0 ? year + " " : "" ) + type + " = " + newValue + " for " + username, "");
                
                refresh();
            }
            else
                MessageBox.Show("No entitlement selected.");
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            button6_Click(null, null);
        }
    }
}
