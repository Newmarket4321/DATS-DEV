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
    public partial class PayrollExport : Form
    {
        public int batchID = -1;
        string type;

        public PayrollExport(string empType)
        {
            InitializeComponent();

            label6.Text = "";
            type = empType;

            MethodInvoker myProcessStarter = new MethodInvoker(process);
            myProcessStarter.BeginInvoke(null, null);
        }

        private void process()
        {
            try
            {
                DataTable dt = null;
                Oracle ora = null;
                string cutoff = "";
                DateTime endDate;

                if (Core.getEnvironment() != "PD")
                    MessageBox.Show("This export is in test mode. Environment: " + Core.getEnvironment() + Environment.NewLine
                        + "If this isn't intended or you're not sure what this means, speak to IT.");

                SQL sql = null;
                string F0002NumberPayroll = "";
                string F0002NumberWorkOrder = "";
                FindPeriod fp = null;

                fp = new FindPeriod(type, false);
                fp.ShowDialog();

                Invoke((MethodInvoker)delegate { label1.Font = new Font(label1.Font, FontStyle.Bold); });
                Invoke((MethodInvoker)delegate { label1.Update(); });

                if ((fp != null && fp.period != "") || type == "S")
                {
                    updateEmployeeTypes();

                    Invoke((MethodInvoker)delegate { label1.Font = new Font(label1.Font, FontStyle.Regular); });
                    Invoke((MethodInvoker)delegate { label1.ForeColor = Color.Gray; });
                    Invoke((MethodInvoker)delegate { label1.Update(); });
                    Invoke((MethodInvoker)delegate { label2.Font = new Font(label1.Font, FontStyle.Bold); });
                    Invoke((MethodInvoker)delegate { label2.Update(); });

  //                  if (Core.getEnvironment() == "PD")
  //                  {
                        //1. Get local batch ID
                        sql = new SQL("select max(batchid) from batchids");
                        batchID = int.Parse(sql.Run().Rows[0][0].ToString()) + 1;

                        //2. Insert new local batch ID
                        sql = new SQL("insert into batchids (batchid, bdate) values (@BATCHID, @BDATE)");
                        sql.AddParameter("@BATCHID", batchID);
                        sql.AddParameter("@BDATE", DateTime.Today);
                        sql.Run();

                        //3. Mark timesheets as exported, and give them local batch ID
                        cutoff = Oracle.RunString("select max(JDPPED) from " + Core.getSchema(Core.getEnvironment()) + ".F069066 where trim(JDPCCD) = '" + (type == "H" ? "HR" : "SAL") + "' and JDPPED <= @TODAY", Core.dateToJDE(DateTime.Today.ToString()));
                        // Select up to yyyy/mm/dd 23:59:00 Soleil

                        endDate = DateTime.Parse(Core.JDEToDate(cutoff).ToString("yyyy/MM/dd") + " 23:59:00 PM");

                        SQL.Run(@"
update Timesheets

set exported='True',
BatchId=@BATCHID

where dateworked<=@CUTOFF
and recordlocked='True'
and exported='False'
and paytype not in (0)
and not (@EMPTYPE1 = 'S' and paytype = 811)
and EmployeeID in (select EmployeeID from Users where EMPTYPE = @EMPTYPE2)", batchID, endDate, type, type);

                    //and EmployeeID in (select EmployeeID from Users where EMPTYPE = @EMPTYPE2)", batchID, Core.JDEToDate(cutoff), type, type);
                    //                  }

                    //4. Clear F06116Z1
                    Oracle.Run("delete from " + Core.getSchema(Core.getEnvironment()) + ".F06116Z1");

                    //5. Get JDE batch IDs
                    F0002NumberPayroll = Oracle.Run("Select NNN006 from " + Core.getControlSchema(Core.getEnvironment()) + ".F0002 where NNSY='00'").Rows[0][0].ToString();
                    F0002NumberWorkOrder = (int.Parse(F0002NumberPayroll) + 1).ToString();

                    //6. Use JDE batch IDs
                    Oracle.Run("UPDATE " + Core.getControlSchema(Core.getEnvironment()) + ".F0002 Set NNN006 = NNN006 + 2 where NNSY='00'");

                    //7. Get data from the new batch in DATS
                    cutoff = Oracle.RunString("select max(JDPPED) from " + Core.getSchema(Core.getEnvironment()) + ".F069066 where trim(JDPCCD) = '" + (type == "H" ? "HR" : "SAL") + "' and JDPPED <= @TODAY", Core.dateToJDE(DateTime.Today.ToString()));

                    dt = SQL.Run(@"
                    select t.employeeid, t.workorder, t.hours, p.rate_exp, p.rate_r, t.lumpsum, t.dateworked, t.timecarddetailid

                    from Timesheets t
                    join paycodes p on t.paytype = p.paytype
                    join users u on t.employeeid = u.employeeid

                    where dateworked<=@CUTOFF
                    and recordlocked='True'
                    and (exported='False' or batchid = @BATCHID)
                    and t.paytype not in (0)
                    and not (EMPTYPE = 'S' and t.paytype = 811)
                    and EMPTYPE = @EMPTYPE", Core.JDEToDate(cutoff), batchID, type);


//                    MessageBox.Show(Core.JDEToDate(cutoff) + "," + batchID + "," + type);

                    //8. Insert into F06116Z1 (Payroll)
                    double totalHours = 0;
                    double totalLSum = 0;
                    int lineNumber = 1000;

                    Invoke((MethodInvoker)delegate { label2.Font = new Font(label1.Font, FontStyle.Regular); });
                    Invoke((MethodInvoker)delegate { label2.ForeColor = Color.Gray; });
                    Invoke((MethodInvoker)delegate { label2.Update(); });
                    Invoke((MethodInvoker)delegate { label3.Font = new Font(label1.Font, FontStyle.Bold); });
                    Invoke((MethodInvoker)delegate { label3.Update(); });

                    Invoke((MethodInvoker)delegate { progressBar1.Value = 0; });
                    Invoke((MethodInvoker)delegate { progressBar1.Maximum = dt.Rows.Count; });
                    Invoke((MethodInvoker)delegate { progressBar1.Update(); });

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Invoke((MethodInvoker)delegate { label6.Text = (i + 1) + " of " + dt.Rows.Count; });
                        Invoke((MethodInvoker)delegate { label6.Update(); });

                        Invoke((MethodInvoker)delegate { progressBar1.Value = i + 1; });
                        Invoke((MethodInvoker)delegate { progressBar1.Update(); });

                        if (dt.Rows[i]["rate_exp"].ToString() == "0")
                            MessageBox.Show("Pay Type \"Absent\" is coming across. This shouldn't happen. Please speak to IT.");

                        double lsum = 0;
                        double.TryParse(dt.Rows[i]["lumpsum"].ToString(), out lsum);
                        int recordType = 1;
                        int hours = int.Parse((Math.Round(double.Parse(dt.Rows[i]["hours"].ToString()),2) * 100).ToString());
                        double hourRate = double.Parse(dt.Rows[i]["rate_r"].ToString());
                        hours = (int)((double)hours * hourRate);

                        ora = new Oracle("INSERT INTO " + Core.getSchema(Core.getEnvironment()) + ".F06116Z1 ( VLEDUS, VLICU, VLEDBT, VLAN8, VLDWK, VLPDBA, VLPHRW, VLANI, VLEPA, VLSHRT, VLRCCD, VLEDLN, VLEDTN, VLEDDT, VLEDER, VLEDSP, VLEDTC, VLEDTR, VLPRTR) VALUES ( @VLEDUS, @VLICU, @VLEDBT, @VLAN8, @VLDWK, @VLPDBA, @VLPHRW, @VLANI, @VLEPA, @VLSHRT, @VLRCCD, @VLEDLN, @VLEDTN, @VLEDDT, @VLEDER, @VLEDSP, @VLEDTC, @VLEDTR, @VLPRTR )");
                        ora.AddParameter("@VLEDUS", "JDE"); //Create user
                        ora.AddParameter("@VLICU", 0);
                        ora.AddParameter("@VLEDBT", F0002NumberPayroll); //Batch number
                        ora.AddParameter("@VLAN8", dt.Rows[i]["employeeid"].ToString()); //Employee number
                        ora.AddParameter("@VLDWK", Core.dateToJDE(dt.Rows[i]["dateworked"].ToString())); //Date worked, JDE format
                        ora.AddParameter("@VLPDBA", dt.Rows[i]["rate_exp"].ToString()); //Pay code
                        ora.AddParameter("@VLPHRW", hours); //Hours*100
                        ora.AddParameter("@VLANI", DBNull.Value); //Work order
                        ora.AddParameter("@VLEPA", int.Parse((lsum * 100).ToString())); //Lump sum
                        ora.AddParameter("@VLSHRT", DBNull.Value); //Override rate
                        ora.AddParameter("@VLRCCD", recordType); //Record type
                        ora.AddParameter("@VLEDLN", lineNumber);
                        ora.AddParameter("@VLEDTN", 0);
                        ora.AddParameter("@VLEDDT", Core.dateToJDE(DateTime.Today.ToString())); // EAC Trans Date
                        ora.AddParameter("@VLEDER", "R");
                        ora.AddParameter("@VLEDSP", 0);
                        ora.AddParameter("@VLEDTC", "A"); //Add
                        ora.AddParameter("@VLEDTR", 1);   //Transaction type
                        ora.AddParameter("@VLPRTR", 0);   //Unique Number

                        ora.Run();

                        totalLSum += lsum;
                        totalHours += ((double)hours) / 100;
                        lineNumber += 1000;
                    }

                    string output1 = "Done!"
                        + Environment.NewLine
                        + Environment.NewLine + "Batch type is PAYROLL."
                        + Environment.NewLine + "JDE Batch ID is " + F0002NumberPayroll + "."
                        + Environment.NewLine
                        + Environment.NewLine + "Total records is " + dt.Rows.Count + "."
                        + Environment.NewLine + "Total hours is " + totalHours + "."
                        + Environment.NewLine + "Total lump sum is $" + totalLSum + ".";

                    //9. Insert into F06116Z1 (Work order)
                    totalHours = 0;
                    totalLSum = 0;
                    lineNumber = 1000;

                    Invoke((MethodInvoker)delegate { label3.Font = new Font(label1.Font, FontStyle.Regular); });
                    Invoke((MethodInvoker)delegate { label3.ForeColor = Color.Gray; });
                    Invoke((MethodInvoker)delegate { label3.Update(); });
                    Invoke((MethodInvoker)delegate { label4.Font = new Font(label1.Font, FontStyle.Bold); });
                    Invoke((MethodInvoker)delegate { label4.Update(); });

                    Invoke((MethodInvoker)delegate { progressBar1.Value = 0; });
                    Invoke((MethodInvoker)delegate { progressBar1.Maximum = dt.Rows.Count; });
                    Invoke((MethodInvoker)delegate { progressBar1.Update(); });

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Invoke((MethodInvoker)delegate { label6.Text = (i + 1) + " of " + dt.Rows.Count; });
                        Invoke((MethodInvoker)delegate { label6.Update(); });

                        Invoke((MethodInvoker)delegate { progressBar1.Value = i + 1; });
                        Invoke((MethodInvoker)delegate { progressBar1.Update(); });

                        if (dt.Rows[i]["workorder"].ToString().Trim() != "")
                        {
                            double lsum = 0;
                            double.TryParse(dt.Rows[i]["lumpsum"].ToString(), out lsum);
                            int recordType = 3;
                            string workorder = dt.Rows[i]["workorder"].ToString().Trim() == "" ? "" : "\\" + dt.Rows[i]["workorder"].ToString() + ".";
                            int hours = int.Parse((Math.Round(double.Parse(dt.Rows[i]["hours"].ToString()), 2) * 100).ToString());
                            double hourRate = double.Parse(dt.Rows[i]["rate_r"].ToString());
                            hours = (int)((double)hours * hourRate);

                            ora = new Oracle("INSERT INTO " + Core.getSchema(Core.getEnvironment()) + ".F06116Z1 ( VLEDUS, VLICU, VLEDBT, VLAN8, VLDWK, VLPDBA, VLPHRW, VLANI, VLEPA, VLSHRT, VLRCCD, VLEDLN, VLEDTN, VLEDDT, VLEDER, VLEDSP, VLEDTC, VLEDTR, VLPRTR ) VALUES ( @VLEDUS, @VLICU, @VLEDBT, @VLAN8, @VLDWK, @VLPDBA, @VLPHRW, @VLANI, @VLEPA, @VLSHRT, @VLRCCD, @VLEDLN, @VLEDTN, @VLEDDT, @VLEDER, @VLEDSP, @VLEDTC, @VLEDTR, @VLPRTR )");
                            ora.AddParameter("@VLEDUS", "JDE"); //Create user
                            ora.AddParameter("@VLICU", 0);
                            ora.AddParameter("@VLEDBT", F0002NumberWorkOrder); //Batch number
                            ora.AddParameter("@VLAN8", dt.Rows[i]["employeeid"].ToString()); //Employee number
                            ora.AddParameter("@VLDWK", Core.dateToJDE(dt.Rows[i]["dateworked"].ToString())); //Date worked, JDE format
                            ora.AddParameter("@VLPDBA", dt.Rows[i]["rate_exp"].ToString()); //Pay code
                            ora.AddParameter("@VLPHRW", hours); //Hours*100
                            ora.AddParameter("@VLANI", workorder); //Work order
                            ora.AddParameter("@VLEPA", int.Parse((lsum * 100).ToString())); //Lump sum
                            ora.AddParameter("@VLSHRT", DBNull.Value); //Override rate
                            ora.AddParameter("@VLRCCD", recordType); //Record type
                            ora.AddParameter("@VLEDLN", lineNumber);
                            ora.AddParameter("@VLEDTN", 0);
                            ora.AddParameter("@VLEDDT", Core.dateToJDE(DateTime.Today.ToString())); // EAC Trans Date
                            ora.AddParameter("@VLEDER", "R");
                            ora.AddParameter("@VLEDSP", 0);
                            ora.AddParameter("@VLEDTC", "A"); //Add
                            ora.AddParameter("@VLEDTR", 1);   //Transaction type
                            ora.AddParameter("@VLPRTR", 0);   //Unique Number
                            ora.Run();

                            totalLSum += lsum;
                            totalHours += ((double)hours) / 100;
                            lineNumber += 1000;
                        }
                    }

                    Invoke((MethodInvoker)delegate { label4.Font = new Font(label1.Font, FontStyle.Regular); });
                    Invoke((MethodInvoker)delegate { label4.ForeColor = Color.Gray; });
                    Invoke((MethodInvoker)delegate { label4.Update(); });
                    Invoke((MethodInvoker)delegate { label5.Font = new Font(label1.Font, FontStyle.Bold); });
                    Invoke((MethodInvoker)delegate { label5.Update(); });

                    string output2 = "Done!"
                        + Environment.NewLine
                        + Environment.NewLine + "Batch type is WORKORDER."
                        + Environment.NewLine + "JDE Batch ID is " + F0002NumberWorkOrder + "."
                        + Environment.NewLine
                        + Environment.NewLine + "Total records is " + ((lineNumber / 1000) - 1) + "."
                        + Environment.NewLine + "Total hours is " + totalHours + "."
                        + Environment.NewLine + "Total lump sum is $" + totalLSum + ".";

                    //10. Update timesheet histories
                    if (Core.getEnvironment() == "PD")
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            Invoke((MethodInvoker)delegate { label6.Text = (i + 1) + " of " + dt.Rows.Count; });
                            Invoke((MethodInvoker)delegate { label6.Update(); });

                            Invoke((MethodInvoker)delegate { progressBar1.Value = i + 1; });
                            Invoke((MethodInvoker)delegate { progressBar1.Update(); });

                            Core.logTimesheetHistory("Timesheet exported", int.Parse(dt.Rows[i]["timecarddetailid"].ToString()));
                        }
                    }

                    Invoke((MethodInvoker)delegate { label5.Font = new Font(label1.Font, FontStyle.Regular); });
                    Invoke((MethodInvoker)delegate { label5.ForeColor = Color.Gray; });
                    Invoke((MethodInvoker)delegate { label5.Update(); });

                    Invoke((MethodInvoker)delegate { label6.Text = ""; });
                    Invoke((MethodInvoker)delegate { label6.Update(); });

                    string email = "";

                    //if (Core.getEnvironment() != "PD")
                    //    email = "msquires@newmarket.ca";
                    //else
                    if (Environment.MachineName == "ITTEMPDT-01-21")
                        email = "kpatel@newmarket.ca";
                    else
                        email = Core.getVariable("PayrollContact");

                    Core.sendMail(email, "DATS-Dev Export #" + batchID, output1 + Environment.NewLine + Environment.NewLine + output2);
                    MessageBox.Show("The export has finished. Please check your e-mail for results.");

                    Invoke((MethodInvoker)delegate { Close(); });
                }
                else
                {
                    Invoke((MethodInvoker)delegate { Close(); });
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(Core.getErrorText(e));
            }
        }

        public static void updateEmployeeTypes()
        {
            DataTable dt = SQL.Run("select employeeid from users where employeeid <> '999999' order by employeeid");

            //string emp = "";

            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    if (i > 0)
            //        emp += ", ";

            //    emp += dt.Rows[i][0].ToString();
            //}

            //DataTable dt2 = Oracle.Run("select YASALY from PRODDTA.F060116 where YAAN8 in (" + emp + ")");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                try
                {

                    Oracle ora = new Oracle("select YASALY, YAAN8 from " + Core.getSchema(Core.getEnvironment()) + ".F060116 where YAAN8 = @EMPID");
                    ora.AddParameter("@EMPID", dt.Rows[i][0].ToString());
                    string empType = ora.Run().Rows[0]["YASALY"].ToString();

                    SQL sql = new SQL("update users set emptype = @EMPTYPE where employeeid = @EMPID");
                    sql.AddParameter("@EMPTYPE", empType);
                    sql.AddParameter("@EMPID", dt.Rows[i][0].ToString());
                    sql.Run();
                }
                catch (Exception e2)
                {

                }
            }
        }
    }
}
