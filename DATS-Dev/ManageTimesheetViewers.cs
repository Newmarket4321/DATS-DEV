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

            sql = new SQL("SELECT Users.DISPLAYNAME, Users_Level.Permission_ID  FROM Users INNER JOIN Users_Level ON Users.USERID = Users_Level.Permission_ID where Users_Level.user_id=@user_id  order by DISPLAYNAME");
            sql.AddParameter("@user_id", userID);
            DataTable dt1 = sql.Run();
            if (dt.Rows.Count > 0)
                for (int i = 0; i < dt1.Rows.Count; i++)
            {
                for (int j = 0; j < checkedListBox1.Items.Count; j++)
                {
                    if(checkedListBox1.Items[j].ToString() == dt1.Rows[i]["DISPLAYNAME"].ToString())
                    {
                        checkedListBox1.SetItemCheckState(j, CheckState.Checked);
                    }
                          //  MessageBox.Show(dt1.Rows[i]["DISPLAYNAME"].ToString());
                }
            }
         
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (checkedListBox1.CheckedItems.Count == 0) //No departments are selected
            {
                MessageBox.Show("Please select a department.");
                return;
            }
            //Delete existing Users_Level
            SQL   sql = new SQL("delete from Users_Level where user_ID=@USERID");
            sql.AddParameter("@USERID", userID);
            sql.Run();

            for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
            {
                sql = new SQL("select * from Users where DISPLAYNAME=@DISPLAYNAME order by DISPLAYNAME");
                sql.AddParameter("@DISPLAYNAME", checkedListBox1.CheckedItems[i].ToString());
                string Permission_ID = sql.Run().Rows[0]["USERID"].ToString();

                //sql = new SQL("select user_ID,Permission_ID from Users_Level where User_ID=@UserID and  Permission_ID=@Permission_ID");
                //sql.AddParameter("@UserID", userID);
                //sql.AddParameter("@Permission_ID", Permission_ID);
                //DataTable dt = sql.Run();

                //if (dt.Rows.Count > 0 )            //Update Users_Level
                //{
                //   // MessageBox.Show("Update");
                //    sql = new SQL("UPDATE Users_Level set Permission_ID=@PermissionID, Allow=@Allow where user_id=@USERID and Permission_ID=@PermissionID");
                //    sql.AddParameter("@PermissionID", Permission_ID);
                //        sql.AddParameter("@Allow", "True");
                //    sql.AddParameter("@USERID", userID);
                //    sql.Run();
                //}
                    sql = new SQL("insert into Users_Level values (@USERID, @PermissionID)");
                    sql.AddParameter("@USERID", userID);
                    sql.AddParameter("@PermissionID", Permission_ID);
                    sql.Run();
            }
            this.Close();
        }
    }
}
