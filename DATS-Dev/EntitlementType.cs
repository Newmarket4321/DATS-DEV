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
    public partial class EntitlementType : Form
    {
        string cusername;
        int idEmp;
        double nvalue = 0;

        public EntitlementType(string username,int empID)
        {
            InitializeComponent();
            DataTable dt = null;
            cusername = username;
            idEmp = empID;
            
            dt = SQL.Run("select *  from EntitlementType");
            cbEntitlement.DataSource = dt;
            cbEntitlement.DisplayMember = "TypeDesc";
            cbEntitlement.ValueMember = "TypeID";
            nYear.Value = DateTime.Today.Year;
               


        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            bool success;
            int year = Convert.ToInt32(Math.Round(nYear.Value, 0));
            string Etype = cbEntitlement.Text.Trim();
            nvalue = Convert.ToDouble(nEntValue.Value);

            int count = SQL.Run("select * from entitlements where employeeid = @idEmp and year = @YEAR and type = @type",idEmp, year, Etype).Rows.Count;

            if (count > 0)
            {
                MessageBox.Show(cusername + " already has an entitlement entry for " + year + "." + Environment.NewLine
                    + "Please view it on the left side of the Entitlements screen.");
                return;
            }
            else
            {
                if(Environment.MachineName == "SYSMG-09-19")
                {
                    if(nvalue == 35 || nvalue == 40 || nvalue == 70 || nvalue == 80)
                    {
                        
                        SQL.Run("insert into entitlements values (@EMPID, @YEAR, @TYPE, @ENTITLEMENT)", idEmp, year, Etype, nvalue);
                        Core.logHistory("Entitlement added", year + " Vacation = " + nvalue + " for " + cusername, "");
                        
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Your overtime limit is 35,40,70 or 80.");
                    }
                 }
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

