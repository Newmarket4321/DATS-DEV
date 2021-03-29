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
    public partial class ManageTimesheetViewers : Form
    {
        string userID;
        public ManageTimesheetViewers()
        {
            InitializeComponent();
            startup(Core.getUsername());
        }
        public ManageTimesheetViewers(string username)
        {
            InitializeComponent();
            startup(username);

        }
        private void startup(string username)
        {
            SQL sql = new SQL("select DISPLAYNAME from Users order by DISPLAYNAME");
            DataTable dt = sql.Run();
                    CheckBox box;
            string str = null;
       if (dt.Rows.Count > 0)
             for (int i = 0; i < dt.Rows.Count; i++)
                  checkedListBox1.Items.Add(dt.Rows[i]["DISPLAYNAME"].ToString());
       
             
            //Grab userID
             sql = new SQL("select userid from users where DISPLAYNAME=@USERNAME");
            sql.AddParameter("@USERNAME", username);
             userID = sql.Run().Rows[0]["userid"].ToString();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (checkedListBox1.CheckedItems.Count == 0) //No departments are selected
            {
                MessageBox.Show("Please select a department.");
                return;
            }
            //Update departments
            for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
            {
              SQL  sql = new SQL("select * from Users where DISPLAYNAME=@DISPLAYNAME order by DISPLAYNAME");
                sql.AddParameter("@DISPLAYNAME", checkedListBox1.CheckedItems[i].ToString());
                string Permission_ID = sql.Run().Rows[0]["USERID"].ToString();

                sql = new SQL("insert into Users_Level values (@USERID, @PermissionID)");
                sql.AddParameter("@USERID", userID);
                sql.AddParameter("@PermissionID", Permission_ID);
                sql.Run();
            }
            this.Close();
        }
    }
}
