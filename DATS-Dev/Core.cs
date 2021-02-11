using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Drawing;
using System.Net.Mail;
using System.Net;
using ClosedXML.Excel;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace DATS_Timesheets
{
    class Core
    {
        //////////////////////////
        static string environment = "PY";
        //////////////////////////

        public static string getUsername()
        {
            string name = Environment.UserName;

            //if (Environment.MachineName == "SYSEA-08-18")
            //    name = "Squires, Michael";

            ////if (Environment.MachineName == "ITTEMPDT-01-21")
            ////    name = "Grant, Dionne";
            try
            {
                name = SQL.RunString("select displayname from users where username=@NAME", name);
            }
            catch
            {

            }

                return name;
        }

        public static string getWindowsUsername()
        {
            string name = getUsername();

            try
            {
                SQL sql = new SQL("select username from users where displayname=@NAME");
                sql.AddParameter("@NAME", name);
                name = sql.Run().Rows[0][0].ToString();

                name = name[0].ToString().ToUpper() + name.Substring(1, name.Length - 1);

                for (int i = 1; i < name.Length; i++)
                    if (name[i - 1] == ' ')
                        name = name.Substring(0, i) + name[i].ToString().ToUpper() + name.Substring(i + 1, name.Length - i - 1);

                if (name.ToLower() == "mary-anne wigmore")
                    name = "Mary-Anne Wigmore";
            }
            catch
            {

            }

            return name;
        }


        public static DateTime getDate(string title, out bool success)
        {
            GetDate box = new GetDate(title);
            box.ShowDialog();

            success = !box.quit;

            return box.r;
        }

        public static DateTime getDate(string title, DateTime defaultDate, out bool success)
        {
            GetDate box = new GetDate(title, defaultDate);
            box.ShowDialog();

            success = !box.quit;

            return box.r;
        }

        public static DateTime getVersion()
        {
            return File.GetLastWriteTime(Assembly.GetEntryAssembly().Location);
        }

        public static DateTime getLatestVersion()
        {
            return File.GetLastWriteTime(@"\\data\Files\PCapps\DATS\DATS-Dev.exe");
        }

        public static void error(Exception e)
        {
            bool problem = true;

            if (e is ObjectDisposedException) //Form has been closed
                problem = false; //Not a problem
            else if (e is ArgumentNullException) //Form doesn't exist
                e = new Exception("A program of that name doesn't exist.");
            else if (e is OracleException) //Oracle timeouts, possibly others
            {
                problem = false; //Don't spam me
                Core.logHistory("Oracle exception", getErrorText(e), "");
            }
            else if(e is IOException && getErrorText(e).Contains("because it is being used by another process"))
            {
                MessageBox.Show("DATS Dev is trying to update to the latest version, but is unable to because your computer considers the file in use." + Environment.NewLine
                    + Environment.NewLine
                    + "This most often happens when someone else was using your computer, had DATS open, then walked away from the computer without properly logging out." + Environment.NewLine
                    + Environment.NewLine 
                    + "The easiest way to resolve this is to reboot your computer. This forces everyone else off.");

                return;
            }

            if (problem)
            {
                try
                {
                    //Report to IT
                    if (Environment.MachineName == "SYSMG-09-19")
                    {
                        sendMail("kpatel@newmarket.ca", "DATS-Dev Error", getErrorText(e));
                    }
                    else
                    {
                        sendMail("ealarcon@newmarket.ca", "DATS-Dev Error", getErrorText(e));
                    }
                        string response = Core.lookupMessageBox(
                        "Error",
                        "Unfortunately something went wrong, however this has automatically been reported to IT."
                        + Environment.NewLine + "You should receive an e-mail response soon.", "Show me the error", "OK");

                    if (response == "Show me the error")
                        MessageBox.Show(getErrorText(e));
                }
                catch
                {
                    try //Try to tell the user
                    {
                        MessageBox.Show("An error occurred, however it has automatically been reported to IT." + Environment.NewLine + "These things are typically resolved within an hour. Please try again soon.");
                    }
                    catch //Servers can't display messages, so just throw, and let it sit in Event Viewer
                    {
                        throw e;
                    }
                }
            }
        }

        public static string getExecutionPath()
        {
            return Assembly.GetEntryAssembly().Location.Replace(@"T:\", @"\\data\files\");
        }

        public static bool isIn(string leftSide, params string[] list)
        {
            for (int i = 0; i < list.Length; i++)
                if (leftSide == list[i])
                    return true;

            return false;
        }

        public static bool isIn(char leftSide, params char[] list)
        {
            for (int i = 0; i < list.Length; i++)
                if (leftSide == list[i])
                    return true;

            return false;
        }

        public static bool isIn(int leftSide, params int[] list)
        {
            for (int i = 0; i < list.Length; i++)
                if (leftSide == list[i])
                    return true;

            return false;
        }

        public static bool isIn(double leftSide, params double[] list)
        {
            for (int i = 0; i < list.Length; i++)
                if (leftSide == list[i])
                    return true;

            return false;
        }

        public static string devMachineName()
        {
            return "SYSGS-11-13";
        }

        public static bool isIn(decimal leftSide, params decimal[] list)
        {
            for (int i = 0; i < list.Length; i++)
                if (leftSide == list[i])
                    return true;

            return false;
        }

        public static bool isIn(DateTime leftSide, params DateTime[] list)
        {
            for (int i = 0; i < list.Length; i++)
                if (leftSide == list[i])
                    return true;

            return false;
        }

        public static void backup(string source, string destination)
        {
            string targetDestination = destination + " " + DateTime.Today.ToString("MMMM d yyyy");

            for (int i = 2; Directory.Exists(targetDestination); i++)
                targetDestination = destination + " " + DateTime.Today.ToString("MMMM d yyyy") + " (" + i + ")";

            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(source, targetDestination));

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(source, targetDestination), true);
        }

        public static string getEnvironment()
        {
            return environment;
        }

        public static string getSchema(string environment)
        {
            if (environment == "PD")
                return "PRODDTA";
            else if (environment == "PY")
                return "CRPDTA";
            else if (environment == "DV")
            {
                //TODO NW//
                //environment = "TESTDTA";
                return "CRPDTA";
            }

            else
                throw new Exception("JDE environment not recognized, check Core for env: " + environment);

            return "";
        }

        public static string getSchema()
        {
            return Core.getSchema(Core.getEnvironment());
        }

        public static string getControlSchema(string environment)
        {
            if (environment == "PD")
                return "PRODCTL";
            else if (environment == "PY")
                return "CRPCTL";
            else if (environment == "DV")
                environment = "TESTCTL";
            else
                throw new Exception("JDE environment not recognized, check Core for env: " + environment);

            return "";
        }

        public static string getControlSchema()
        {
            return Core.getControlSchema(Core.getEnvironment());
        }

        public static string getDeploymentDirectory()
        {
            return @"\\data\files\PCapps\DATS\";
        }

        public static string getDeploymentPath()
        {
            return getDeploymentDirectory() + Path.GetFileName(Assembly.GetEntryAssembly().Location);
        }

        public static string getUserDesktopDirectory()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        public static void cleanDelete(string path)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch
            {

            }
        }

        public static string getExecutionDirectory()
        {
            return Path.GetDirectoryName(getExecutionPath());
        }

        public static void checkPrerequisite(string filename)
        {
            try
            {
                string source = Path.GetDirectoryName(getDeploymentPath()) + "\\" + filename;
                string target = AppDomain.CurrentDomain.BaseDirectory + filename;

                if (!File.Exists(target)) //If it doesn't exist
                    File.Copy(source, target, true);

                long fileSize = (new FileInfo(target)).Length; //In bytes
                int attemptNo = 1;

                while (fileSize < 1024 && attemptNo <= 5) //If is a compressed stub
                {
                    File.Copy(source, target, true); //Try again
                    Core.sleep(1000);

                    fileSize = (new FileInfo(target)).Length;

                    attemptNo++;
                }

                File.SetAttributes(target, FileAttributes.Normal);
                File.SetAttributes(target, FileAttributes.Hidden);
            }
            catch (IOException e)
            {
                Core.logHistory("checkPrerequisite", filename + ": " + e.Message, "");

                if (e.Message.Contains("already exists"))
                {
                    //Ignore
                    //Unsure why this exception happens when we check for existence
                }
                else
                    throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static void sleep(int milliseconds)
        {
            System.Threading.Thread.Sleep(milliseconds);
        }

        public static void run(string path)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Verb = "runas";

            startInfo.Arguments = "/C \"" + path + "\"";
            process.StartInfo = startInfo;
            process.Start();
        }

        public static void run(string path, string arguments)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";

            startInfo.Arguments = "/C \"" + path + "\" " + arguments;
            process.StartInfo = startInfo;
            process.Start();
        }

        public static bool isTimesheetLocked(int id, string empType)
        {
            if (isAdmin(Core.getUsername()))
                return false;

            DateTime date = DateTime.Parse(SQL.Run("select dateworked from Timesheets where timecarddetailid=" + id).Rows[0]["dateworked"].ToString());
            return !isDateOpen(date, empType);
        }

        public static bool isDateOpen(DateTime date, string empType)
        {
            if (getVariable("Lockout" + empType) == "True")
            {
                //Block current and past, no matter who you are
                int currentPeriod = Core.getCurrentPeriod(empType);
                DateTime cutoff = getPeriodEnd(currentPeriod, empType);

                if (empType == "S")
                    cutoff = cutoff.AddDays(-14);
                
                return cutoff < date;
            }
            else
            {
                if (Core.isAdmin(Core.getUsername()) || Core.canReview(Core.getUsername()))
                    return true;
                else if (Core.isFacilities(Core.getUsername()) || Core.isFacilityMaintenance(Core.getUsername()))
                    return DateTime.Now <= getPeriodEnd(getPeriod(date, empType), empType).AddDays(1).AddHours(6); //Monday 6AM
                else if (Core.isSalary(Core.getEmpID(Core.getUsername())))
                    return DateTime.Now <= getPeriodEnd(getPeriod(date, empType), empType).AddDays(15).AddHours(8); //Monday 8AM + 1 pay period
                else if(Core.getDepartments().Contains(21))
                    return DateTime.Now <= getPeriodEnd(getPeriod(date, empType), empType).AddDays(1).AddHours(16); //Monday 4PM
                else if (Core.getDepartments().Contains(4))
                    return DateTime.Now <= getPeriodEnd(getPeriod(date, empType), empType).AddDays(1).AddHours(9); //Monday 9AM
                else
                    return DateTime.Now <= getPeriodEnd(getPeriod(date, empType), empType).AddDays(1).AddHours(8); //Monday 8AM
            }
            //return date <= getPeriodEnd(getPeriod(DateTime.Today.AddDays(-14)));
        }

        public static string getVariable(string var)
        {
            return SQL.Run("select value from systemvariables where variable='" + var + "'").Rows[0]["value"].ToString();
        }

        public static void setVariable(string var, string val)
        {
            SQL.Run("update systemvariables set value='" + val + "' where variable='" + var + "'");
        }

        public static string getString(string title)
        {
            GetString box = new GetString(title);
            box.ShowDialog();

            return box.r;
        }

        public static string getString(string title, bool multiline)
        {
            GetString box = new GetString(title, multiline);
            box.ShowDialog();

            return box.r;
        }

        public static string getString(string title, string defaultQuery)
        {
            GetString box = new GetString(title, defaultQuery);
            box.ShowDialog();

            return box.r;
        }

        public static string getString(string title, string defaultQuery, bool multiline)
        {
            GetString box = new GetString(title, defaultQuery, multiline);
            box.ShowDialog();

            return box.r;
        }

        public static string getString(string title, out bool success)
        {
            GetString box = new GetString(title);
            box.ShowDialog();

            success = !box.quit;

            return box.r;
        }

        public static string getString(string title, bool multiline, out bool success)
        {
            GetString box = new GetString(title, multiline);
            box.ShowDialog();

            success = !box.quit;

            return box.r;
        }

        public static string getString(string title, string defaultQuery, out bool success)
        {
            GetString box = new GetString(title, defaultQuery);
            box.ShowDialog();

            success = !box.quit;

            return box.r;
        }

        public static string getString(string title, string defaultQuery, bool multiline, out bool success)
        {
            GetString box = new GetString(title, defaultQuery, multiline);
            box.ShowDialog();

            success = !box.quit;

            return box.r;
        }

        public static int getInt(string title, out bool success)
        {
            GetInt box = new GetInt(title);
            box.ShowDialog();

            success = !box.quit;

            return box.r;
        }

        public static int getInt(string title, int defaultValue, out bool success)
        {
            GetInt box = new GetInt(title, defaultValue);
            box.ShowDialog();

            success = !box.quit;

            return box.r;
        }

        public static int getInt(string title, int min, int max, out bool success)
        {
            GetInt box = new GetInt(title, min, max);
            box.ShowDialog();

            success = !box.quit;

            return box.r;
        }

        public static int getInt(string title, int min, int max, int defaultValue, out bool success)
        {
            GetInt box = new GetInt(title, min, max, defaultValue);
            box.ShowDialog();

            success = !box.quit;

            return box.r;
        }

        public static double getDouble(string title, out bool success)
        {
            GetDouble box = new GetDouble(title);
            box.ShowDialog();

            success = !box.quit;

            return box.r;
        }

        public static double getDouble(string title, double defaultValue, out bool success)
        {
            GetDouble box = new GetDouble(title, defaultValue);
            box.ShowDialog();

            success = !box.quit;

            return box.r;
        }

        public static double getDouble(string title, double min, double max, out bool success)
        {
            GetDouble box = new GetDouble(title, min, max);
            box.ShowDialog();

            success = !box.quit;

            return box.r;
        }

        public static double getDouble(string title, double min, double max, double defaultValue, out bool success)
        {
            GetDouble box = new GetDouble(title, min, max, defaultValue);
            box.ShowDialog();

            success = !box.quit;

            return box.r;
        }

        public static string lookupMessageBox(string title, string message, params string[] options)
        {
            MessageBoxCustom box = new MessageBoxCustom(title, message, options);
            box.ShowDialog();

            return box.response;
        }

        public static bool getLockout()
        {
            bool result = false;
            DataTable dt = SQL.Run("select value from systemvariables where variable='Lockout'");
            
            if (dt.Rows.Count > 0 && bool.TryParse(dt.Rows[0]["value"].ToString(), out result))
                return result;

            return false;
        }

        public static string getLockoutText()
        {
            return "Unable to enter a timesheet on this date. The causes for this are either:" + Environment.NewLine
                + Environment.NewLine
                + "1) This function may have been temporarily locked down by Payroll while they process the pay." + Environment.NewLine
                + Environment.NewLine
                + "2) You could be trying to enter time into a pay period that has already been closed.";
        }

        //How many hours an employee can book using the Appointment Time paytype (311) per year
        public static double getAppointmentLimit()
        {
            return 6;
        }
        
        public static double getBankedTimeIn(DateTime start, DateTime end, int empID)
        {
            double bankedTimeIn;

            DataTable dt = SQL.Run(@"select
            (SELECT sum(hours) from Timesheets where paytype=950 and EmployeeID=@EMPID and dateworked>=@DATE) as [In1],
            (SELECT sum(hours) from Timesheets where paytype=955 and EmployeeID=@EMPID and dateworked>=@DATE) as [In15]", empID, start);

            bankedTimeIn = 1.5 * (dt.Rows[0]["In15"].ToString() == "" ? 0 : double.Parse(dt.Rows[0]["In15"].ToString()));
            bankedTimeIn += dt.Rows[0]["In1"].ToString() == "" ? 0 : double.Parse(dt.Rows[0]["In1"].ToString());

            return bankedTimeIn;
        }

        public static double getBankedTimeUsed(DateTime start, DateTime end, int empID)
        {
            double bankedTimeUsed;

            DataTable dt = SQL.Run(@"select
            (SELECT sum(hours) from Timesheets where (paytype=81 or paytype=82) and EmployeeID=@EMPID and dateworked>=@DATE) as [Out]", empID, start);

            bankedTimeUsed = dt.Rows[0]["Out"].ToString() == "" ? 0 : double.Parse(dt.Rows[0]["Out"].ToString());

            return bankedTimeUsed;
        }

        public static double getBankedTimeBalance(DateTime start, DateTime end, int empID)
        {
            return getBankedTimeIn(start, end, empID) - getBankedTimeUsed(start, end, empID);
        }

        public static double getVacationMax(int year, int empID)
        {
            double r = 0;
            
            DataTable dt = SQL.Run("select entitlement from entitlements where year = @YEAR and type = 'Vacation' and employeeid = @EMPID", year, empID);

            if (dt.Rows.Count == 0) //This year not found
            {
                //Check last year
                dt = SQL.Run("select entitlement from entitlements where year = @YEAR and type = 'Vacation' and employeeid = @EMPID", year - 1, empID);

                if (dt.Rows.Count == 0) //Nothing found there either
                {
                    r = 0;
                    SQL.Run("insert into entitlements values (@EMPID, @YEAR, @TYPE, @ENTITLEMENT)", empID, year, "Vacation", r);
                    Core.logHistory("Entitlement generated", year + " Vacation = " + r + " for " + Core.getUsernameFromEmpID(empID), "");
                }
                else //Last year had it
                {
                    r = double.Parse(dt.Rows[0][0].ToString());
                    SQL.Run("insert into entitlements values (@EMPID, @YEAR, @TYPE, @ENTITLEMENT)", empID, year, "Vacation", r);
                    Core.logHistory("Entitlement generated", year + " Vacation = " + r + " for " + Core.getUsernameFromEmpID(empID), "");
                }
            }
            else
                r = double.Parse(dt.Rows[0][0].ToString());

            return r;
        }

       
        public static double getVacationUsed(int year, int empID)
        {
            DataTable dt = SQL.Run(@"select sum(hours)
from Timesheets
where
(
    (year(dateworked) = @YEAR1 and paytype in (809, 821, 810, 822))
    or
    (year(dateworked) = @YEAR2 and paytype in (807, 808))
)
and employeeid = @EMPID", year, year + 1, empID);

            return dt.Rows[0][0].ToString() == "" ? 0 : double.Parse(dt.Rows[0][0].ToString());
        }

        public static double getVacationBalance(int year, int empID, bool priorYearLimit)
        {
        //    soleil
        //    if (priorYearLimit && DateTime.Now.Month >= 4)
        //        return 0;

            return getVacationMax(year, empID) - getVacationUsed(year, empID);
        }
        



        public static double getBankedVacationMax(int empID)
        {
            double r = 0;

            DataTable dt = SQL.Run("select entitlement from entitlements where type = 'Banked Vacation' and employeeid = @EMPID", empID);

            if (dt.Rows.Count == 0) //Not found
            {
                r = 0;
                SQL.Run("insert into entitlements values (@EMPID, @YEAR, @TYPE, @ENTITLEMENT)", empID, 0, "Banked Vacation", r);
                Core.logHistory("Entitlement generated", "Banked Vacation = " + r + " for " + Core.getUsernameFromEmpID(empID), "");
            }
            else
                r = double.Parse(dt.Rows[0][0].ToString());

            return r;
        }


        public static double getBankedVacationUsed(int empID)
        {
            DataTable dt = SQL.Run(@"select sum(hours)
from Timesheets
where (paytype = 816 or paytype = 817)
and dateworked >= '2017-10-19'
and employeeid = @EMPID", empID); //October 19th, 2017 is when the bankvacmax was last set, hence get all changes since then

            return dt.Rows[0][0].ToString() == "" ? 0 : double.Parse(dt.Rows[0][0].ToString());
        }

        public static double getBankedVacationBalance(int empID)
        {
            return getBankedVacationMax(empID) - getBankedVacationUsed(empID);
        }

        public static double getMCLMax(int year, int empID)
        {
            double r = 0;

            DataTable dt = SQL.Run("select entitlement from entitlements where year = @YEAR and type = 'MCL Vacation' and employeeid = @EMPID", year, empID);

            if (dt.Rows.Count == 0) //This year not found
            {
                //Check last year
                dt = SQL.Run("select entitlement from entitlements where year = @YEAR and type = 'MCL Vacation' and employeeid = @EMPID", year - 1, empID);

                if (dt.Rows.Count > 0) // Found last year
                {
                    r = double.Parse(dt.Rows[0][0].ToString());
                    SQL.Run("insert into entitlements values (@EMPID, @YEAR, @TYPE, @ENTITLEMENT)", empID, year, "MCL Vacation", r);
                    Core.logHistory("Entitlement generated", year + " MCL Vacation = " + r + " for " + Core.getUsernameFromEmpID(empID), "");
                }
            }
            else
                r = double.Parse(dt.Rows[0][0].ToString());

            return r;
        }
        public static double getMCLUsed(int year, int empID)
        {


            DataTable dt = SQL.Run(@"select sum(hours)
from Timesheets
where
    (year(dateworked) = @YEAR1 and paytype in (818))
 
and employeeid = @EMPID", year, empID);

            return dt.Rows[0][0].ToString() == "" ? 0 : double.Parse(dt.Rows[0][0].ToString());
        }

        public static double getMCLBalance(int year, int empID, bool priorYearLimit)
        {
            return getMCLMax(year, empID) - getMCLUsed(year, empID);
        }

        public static string getEmpGroup(int empID)
        {

            Oracle ora = new Oracle(@"
select YAUN from " + Core.getSchema(Core.getEnvironment()) + ".F060116 where YAAN8 = @EMPNO");
            ora.AddParameter("@EMPNO", empID);
            string code = ora.Run().Rows[0]["YAUN"].ToString().Trim();

            return code;

        }
            public static string getEmpType(int empID)
        {
            Oracle ora = new Oracle(@"
select YAEST from " + Core.getSchema(Core.getEnvironment()) + ".F060116 where YAAN8 = @EMPNO");
            ora.AddParameter("@EMPNO", empID);
            string code = ora.Run().Rows[0]["YAEST"].ToString().Trim();

            //   Full - time Regular
            //1  Part - Time Casual
            //2  Part - Time Hourly
            //3  Contract
            //4  Elected Officials
            //5  Full Time Hourly
            //6  Contract Salary
            //7  LTD

            return code;
        }

        public static bool isSalary(int empID)
        {
            Oracle ora = new Oracle(@"
            select YASALY from " + Core.getSchema(Core.getEnvironment()) + ".F060116 where YAAN8 = @EMPNO");
            ora.AddParameter("@EMPNO", empID);
            string code = ora.Run().Rows[0]["YASALY"].ToString().Trim();
            return code == "S";
        }

        public static bool isHourly(int empID)
        {
            Oracle ora = new Oracle(@"
select YASALY from " + Core.getSchema(Core.getEnvironment()) + ".F060116 where YAAN8 = @EMPNO");
            ora.AddParameter("@EMPNO", empID);
            string code = ora.Run().Rows[0]["YASALY"].ToString().Trim();

            return code == "H";
        }

        public static string getSH(int empID)
        {
            return isSalary(empID) ? "S" : "H";
        }

        public static string getSH(string username)
        {
            return isSalary(Core.getEmpID(username)) ? "S" : "H";
        }

        public static string getSH()
        {
            return isSalary(Core.getEmpID(Core.getUsername())) ? "S" : "H";
        }

        public static double getFloaterMax(int empID)
        {
            Oracle ora = new Oracle(@"
select YAEST from " + Core.getSchema(Core.getEnvironment()) + ".F060116 where YAAN8 = @EMPNO");
            ora.AddParameter("@EMPNO", empID);
            string code = ora.Run().Rows[0]["YAEST"].ToString().Trim();

            //   Full - time Regular
            //1  Part - Time Casual
            //2  Part - Time Hourly
            //3  Contract
            //4  Elected Officials
            //5  Full Time Hourly
            //6  Contract Salary
            //7  LTD

            if (code == "" || code == "2" || code == "5")
                return 1;

            return 0;
        }

        public static double getFloaterUsed(DateTime start, DateTime end, int empID)
        {
            return SQL.Run(@"select hours
from Timesheets
where paytype in (812, 813, 814, 815)
and dateworked >= @START
and dateworked < @END
and employeeid = @EMPID
and hours > 0", start, end, empID).Rows.Count;
        }

        public static double getFloaterBalance(DateTime start, DateTime end, int empID)
        {
            return getFloaterMax(empID) - getFloaterUsed(start, end, empID);
        }

        public static double getStatMax(DateTime start, DateTime end, int empID)
        {
            double statMax = double.Parse(SQL.RunString("select count(*) from holidays where date >= @START and date < @END", start, end));

            if (isPartTime(getUsernameFromEmpID(empID)))
                statMax -= 2; //Civic and Easter Monday

            return statMax;
        }

        public static double getStatUsed(DateTime start, DateTime end, int empID)
        {
            return SQL.Run(@"select sum(t.hours)
from Timesheets t
join users u on t.employeeid = u.employeeid

where u.employeeid = @EMPNO
and t.paytype=@PAY
and t.dateworked >= @START
and t.dateworked < @END

group by t.dateworked
having sum(t.hours) > 0", empID, 811, start, end).Rows.Count;
        }
        
        public static double getStatBalance(DateTime start, DateTime end, int empID)
        {
            return getStatMax(start, end, empID) - getStatUsed(start, end, empID);
        }

        public static double getBankAllotment(string username)
        {
            Oracle ora = new Oracle(@"
select YAEST from " + Core.getSchema(Core.getEnvironment()) + ".F060116 where YAAN8 = @EMPNO");
            ora.AddParameter("@EMPNO", getEmpID(username));
            string code = ora.Run().Rows[0]["YAEST"].ToString().Trim();

            //   Full - time Regular
            //1  Part - Time Casual
            //2  Part - Time Hourly
            //3  Contract
            //4  Elected Officials
            //5  Full Time Hourly
            //6  Contract Salary
            //7  LTD

            if (username == "Evison, Doug")
                return 80;
            else if (canReview(username) || Core.isFacilities(username) || Core.isFacilityMaintenance(username))
                return 80; //Facilities
            else if (code == "" || code == "5")
                return 40.5;
            else
                return 0;
        }

        public static bool isFullTime(string username)
        {
            Oracle ora = new Oracle(@"
select YAEST from " + Core.getSchema(Core.getEnvironment()) + ".F060116 where YAAN8 = @EMPNO");
            ora.AddParameter("@EMPNO", getEmpID(username));
            string code = ora.Run().Rows[0]["YAEST"].ToString().Trim();

            //   Full - time Regular
            //1  Part - Time Casual
            //2  Part - Time Hourly
            //3  Contract
            //4  Elected Officials
            //5  Full Time Hourly
            //6  Contract Salary
            //7  LTD

            return code == "" || code == "5";
        }

        public static DateTime getPeriodStart(int x, string empType)
        {
            string year = x.ToString().Substring(2, 2);
            string period = '0' + x.ToString().Substring(4, 2);

            DataTable dt = Oracle.Run("select JDPPED from " + Core.getSchema(Core.getEnvironment()) + ".F069066 where JDPCCD='" + (empType == "H" ? "HR" : "SAL") + "' and JDPPNB=" + period + " and JDDTEY=" + year);
            DateTime endDate = Core.JDEToDate(dt.Rows[0][0].ToString());

            return endDate.AddDays(-13);
        }
        
        public static DateTime getPeriodEnd(int x, string empType)
        {
            string year = x.ToString().Substring(2, 2);
            string period = '0' + x.ToString().Substring(4, 2);
            DataTable dt = Oracle.Run("select JDPPED from " + Core.getSchema(Core.getEnvironment()) + ".F069066 where JDPCCD='" + (empType == "H" ? "HR" : "SAL") + "' and JDPPNB=" + period + " and JDDTEY=" + year);
            DateTime endDate = Core.JDEToDate(dt.Rows[0][0].ToString());

            return endDate;
        }

        public static void deleteTimesheet(int id)
        {
            Core.logTimesheetHistory("Timesheet deleted", id);

            SQL sql = new SQL("delete from Timesheets where timecarddetailid = @ID");
            sql.AddParameter("@ID", id);
            sql.Run();

            sql = new SQL("delete from equipmenttimeentry where timesheetid = @ID");
            sql.AddParameter("@ID", id);
            sql.Run();

            Core.logHistory("Deleted timesheet", "Timesheet ID# " + id, "");
        }

        public static void sendMail(string to, string subject, string body)
        {
            body = "<font face=\"arial\">" + body.Replace("\r\n", "<br />") + "</font>";
            MailMessage objMail = new MailMessage("dats@newmarket.ca", to, subject, body);

            string emailGuess = "";

            if (Environment.UserName.Contains(' '))
                emailGuess = Environment.UserName.Split(' ')[0][0] + Environment.UserName.Split(' ')[1] + "@newmarket.ca";
            else
                emailGuess = Environment.UserName + "@newmarket.ca";

            objMail.ReplyTo = new MailAddress(emailGuess);
            objMail.IsBodyHtml = true;
            NetworkCredential objNC = new NetworkCredential("dats", "datsgood");
            SmtpClient objsmtp = new SmtpClient("mail.newmarket.ca", 25);
            //objsmtp.EnableSsl = true;
            objsmtp.Credentials = objNC;
            objsmtp.Send(objMail);

            //MailMessage objMail = new MailMessage("dats@newmarket.ca", to, subject, body);
            //NetworkCredential objNC = new NetworkCredential("dats", "datsgood");
            //SmtpClient objsmtp = new SmtpClient("mail.newmarket.ca", 25);
            ////objsmtp.EnableSsl = true;
            //objsmtp.Credentials = objNC;
            //objsmtp.Send(objMail);
        }
        

        public static bool isPartTime(string username)
        {
            Oracle ora = new Oracle(@"
select YAEST from " + Core.getSchema(Core.getEnvironment()) + ".F060116 where YAAN8 = @EMPNO");
            ora.AddParameter("@EMPNO", getEmpID(username));
            string code = ora.Run().Rows[0]["YAEST"].ToString().Trim();

            //   Full - time Regular
            //1  Part - Time Casual
            //2  Part - Time Hourly
            //3  Contract
            //4  Elected Officials
            //5  Full Time Hourly
            //6  Contract Salary
            //7  LTD
            return (code != "" && code != "5"); //If not full-time
        }

        public static string getUsernameFromEmpID(int empID)
        {
            SQL sql = new SQL("select displayname from users where employeeid = @EMPLOYEEID");
            sql.AddParameter("@EMPLOYEEID", empID);

            return sql.Run().Rows[0]["displayname"].ToString();
        }

        public static Color getPayTypeColor(string pt)
        {
            if (pt == "Regular" || pt == "Parks Shift Differential" || pt == "Shift Differential")
                return Color.FromArgb(76, 217, 100);
            else if (pt == "Overtime 1.5" || pt == "O/T Call Out 1.5" || pt == "Call Out - O/T 1.5")
                return Color.FromArgb(255, 59, 48);
            else if (pt == "Vac.(H) Prior Year" || pt == "Vac.(H) Curr.Year" || pt == "Vac. Y/End (H)")
                return Color.FromArgb(90, 200, 250);
            else if (pt == "Stat" || pt == "Stat. Premium")
                return Color.FromArgb(255, 149, 0);
            else if (pt == "Illness (H)" || pt == "Personal Appointment")
                return Color.FromArgb(255, 204, 0);
            else if (pt == "Bank Time Off")
                return Color.FromArgb(88, 86, 214);
            else if (pt == "Banked Time 1.5" || pt == "Banked Time 1.0")
                return Color.FromArgb(255, 45, 85);

            return Color.FromArgb(204, 204, 204);
        }

        public static void logHistory(string eventStr, string dataFrom, string dataTo)
        {
            string envName = Environment.UserName;

            try
            {
                envName = envName[0].ToString().ToUpper() + envName.Substring(1, envName.Length - 1);

                for (int i = 1; i < envName.Length; i++)
                    if (envName[i - 1] == ' ')
                        envName = envName.Substring(0, i) + envName[i].ToString().ToUpper() + envName.Substring(i + 1, envName.Length - i - 1);
            }
            catch
            {

            }

            string username = Core.getWindowsUsername();

            if (username.ToUpper() != envName.ToUpper()) //If logging in as someone else, reflect that in the logs
                username = envName + " --> " + username;

            DateTime date = DateTime.Now;

            SQL sql = new SQL("insert into history (username, timestamp, event, datafrom, datato) values (@USERNAME, @DATE, @EVENT, @DATAFROM, @DATATO)");
            sql.AddParameter("@USERNAME", username);
            sql.AddParameter("@DATE", date);
            sql.AddParameter("@EVENT", eventStr);
            sql.AddParameter("@DATAFROM", dataFrom);
            sql.AddParameter("@DATATO", dataTo);
            sql.Run();
        }

        public static void logTimesheetHistory(string description, int timesheetID)
        {
            string envName = Environment.UserName;

            try
            {
                envName = envName[0].ToString().ToUpper() + envName.Substring(1, envName.Length - 1);

                for (int i = 1; i < envName.Length; i++)
                    if (envName[i - 1] == ' ')
                        envName = envName.Substring(0, i) + envName[i].ToString().ToUpper() + envName.Substring(i + 1, envName.Length - i - 1);
            }
            catch
            {

            }

            string username = Core.getUsername();

            if (username != envName) //If logging in as someone else, reflect that in the logs
                username = envName + " --> " + username;

            DateTime date = DateTime.Now;

            int version = 1;

            SQL sql = new SQL("select version from historyversions where timecarddetailid=@ID order by version desc");
            sql.AddParameter("@ID", timesheetID);
            DataTable dt = sql.Run();

            if (dt.Rows.Count > 0)
                version = int.Parse(dt.Rows[0][0].ToString()) + 1;

            sql = new SQL("insert into historyversions select @TIMESTAMP, @USERNAME, @DESCRIPTION, @VERSION, * from Timesheets where timecarddetailid = @ID");
            sql.AddParameter("@ID", timesheetID);
            sql.AddParameter("@USERNAME", username);
            sql.AddParameter("@TIMESTAMP", DateTime.Now);
            sql.AddParameter("@DESCRIPTION", description);
            sql.AddParameter("@DATE", date);
            sql.AddParameter("@VERSION", version);
            sql.Run();

            dt = SQL.Run("select t.*, u.username, p.description as pay, DATENAME(MM, t.DateWorked) + ' ' + DATENAME(DD, t.dateworked) + ' (' + DATENAME(DW, t.DateWorked) + ')' as 'Date Worked' from Timesheets t join users u on t.employeeid = u.employeeid join paycodes p on t.paytype = p.paytype where timecarddetailid=" + timesheetID);
            int empID = int.Parse(dt.Rows[0]["employeeid"].ToString());
            string empType = Core.isSalary(empID) ? "S" : "H";

            if (Core.getVariable("EmailPayroll" + empType) == "True" && !description.Contains("exported") && !description.Contains("reviewed") && !description.Contains(" approved"))
            {
                DateTime dw = DateTime.Parse(dt.Rows[0]["dateworked"].ToString());
                
                int currentPeriod = Core.getCurrentPeriod(empType);
                int typeMod = empType == "S" ? -14 : 0;

                if (dw < Core.getPeriodEnd(currentPeriod, empType).AddDays(typeMod).AddDays(1))
                {
                    sendMail(Core.getVariable("PayrollContact"), "DATS Notification",
                        description + " by " + username + Environment.NewLine +
                        Environment.NewLine +
                        "Timesheet #" + timesheetID + Environment.NewLine +
                        "--------------------" + Environment.NewLine +
                        dt.Rows[0]["username"].ToString() + " (" + dt.Rows[0]["employeeid"].ToString() + ")" + Environment.NewLine +
                        dt.Rows[0]["Date Worked"].ToString() + Environment.NewLine +
                        dt.Rows[0]["hours"].ToString() + " hours" + Environment.NewLine +
                        dt.Rows[0]["pay"].ToString() + Environment.NewLine +
                        dt.Rows[0]["description"].ToString() + Environment.NewLine);
                }
            }
        }

        public static void export(string filename, DataTable dt)
        {
            XLWorkbook wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(dt, "Report");

            string filePath = "C:\\DATS\\" + filename;
            string add = "";

            //Keep looking for file names untl you find one that doesn't exist.
            //Starts with filename.xlsx, then filename 2.xlsx, and so on.
            while(File.Exists(filePath + (add != "" ? " " : "") + add + ".xlsx"))
            {
                if (add == "")
                    add = "2";
                else
                    add = (int.Parse(add) + 1).ToString();
            }

            filePath += (add != "" ? " " : "") + add + ".xlsx";

            try
            {
                wb.SaveAs(filePath);

                System.Diagnostics.Process.Start(filePath);
            }
            catch (OutOfMemoryException)
            {
                MessageBox.Show("Your computer doesn't have enough memory to export this to Excel. A few things you could try..." + Environment.NewLine
                    + "- Close some of the programs you have open, which frees up more memory" + Environment.NewLine
                    + "- Reboot your computer, which forces closed a lot of the behind-the-scenes programs your computer might have open, which frees up more memory" + Environment.NewLine
                    + "- If you're exporting from a report, try filtering the report to have fewer rows, which reduces the memory needed to perform the export");
            }
            catch
            {
                export("Report", dt);
            }
        }

        public static int getPeriod(DateTime x, string empType)
        {
            string jdeDate = Core.dateToJDE(x.ToString());
            string code = empType == "H" ? "HR" : "SAL";
            
            DataTable dt = Oracle.Run(@"
select
min(JDDTEY || JDPPNB)

from " + Core.getSchema(Core.getEnvironment()) + @".F069066

where
JDPCCD='" + code + @"' and
JDPPED >= " + jdeDate);

            string temp = dt.Rows[0][0].ToString();

            return int.Parse("20" + temp.Substring(0, 2) + temp.Substring(3, 2));
        }

        public static int getCurrentPeriod(string empType)
        {
            DataTable cur = Oracle.Run("select YDCTRY, YDDTEY, YDPPNB from " + Core.getSchema(Core.getEnvironment()) + ".F07210 where trim(YDPAYD) = '" + (empType == "H" ? "TOWNHOURLY" : "SALARY06") + "'");
            string currentPeriod = cur.Rows[0]["YDCTRY"].ToString() + cur.Rows[0]["YDDTEY"].ToString() + cur.Rows[0]["YDPPNB"].ToString().Substring(1, 2);

            return int.Parse(currentPeriod);
        }

        private static bool isHoliday(DateTime date)
        {
            bool returnable = false;

            try
            {
                DataTable dt = SQL.Run("select date from holidays");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (DateTime.Parse(dt.Rows[i][0].ToString()) == date)
                    {
                        returnable = true;
                    }
                }
            }
            catch (Exception)
            {
                return returnable;
            }
            return returnable;
        }

        public static bool isEasterMonday(DateTime date)
        {
            bool returnable = false;

            try
            {
                DataTable dt = SQL.Run("select date from holidays where description = 'Easter Monday'");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (DateTime.Parse(dt.Rows[i][0].ToString()) == date)
                    {
                        returnable = true;
                    }
                }
            }
            catch (Exception)
            {
                return returnable;
            }
            return returnable;
        }

        public static void newTimesheet(string username, DateTime date, string startTime, string endTime, string payCode, double hours, string workOrder, string description, string[] equipment)
        {
            //Grab values
            SQL sql = new SQL("select paytype from paycodes where description=@DESCRIPTION");
            sql.AddParameter("@DESCRIPTION", payCode);
            int payType = int.Parse(sql.Run().Rows[0][0].ToString());

            sql = new SQL("select employeeid from users where displayname=@USERNAME");

            sql.AddParameter("@USERNAME", username);
            
            string employeeID = sql.Run().Rows[0][0].ToString();
            int period = getPeriod(date, Core.getSH(int.Parse(employeeID)));

            double lumpSum = 0.00;

            if (payType == 2 || payType == 4) //Standby
            {
                if (date.DayOfWeek.ToString() == "Saturday" || date.DayOfWeek.ToString() == "Sunday" || isHoliday(date))
                {
                    if (isEasterMonday(date) && isPartTime(username))
                        lumpSum = 24.00;
                    else
                        lumpSum = 45.00;
                }
                else
                    lumpSum = 24.00;
            }
            if (payType == 90) //Meal allowance
                lumpSum = 10.00;

            //Insert header
            try
            {
                sql = new SQL(@"insert into timesheet values(
            @TIMECARDHEADERID,
            @EMPLOYEEID,
            @PERIOD,
            @CREATEUSER)");

                sql.AddParameter("@TIMECARDHEADERID", int.Parse(SQL.Run("select max(timecardheaderid) from timesheet").Rows[0][0].ToString()) + 1);
                sql.AddParameter("@EMPLOYEEID", employeeID);
                sql.AddParameter("@PERIOD", period);
                sql.AddParameter("@CREATEUSER", Core.getUsername());

                sql.Run();
            }
            catch
            {

            }

            //sql = new SQL("select overtimestyle from users where username = @USERNAME");
            //sql.AddParameter("@USERNAME", username);

            //double numHoursBeforeOT;

            //if (sql.Run().Rows[0]["overtimestyle"].ToString() != null && sql.Run().Rows[0]["overtimestyle"].ToString() != "")
            //    numHoursBeforeOT = double.Parse(sql.Run().Rows[0]["overtimestyle"].ToString());
            //else
            //    numHoursBeforeOT = 10000; //Always regular

            DateTime startingMonday = date;

            while (startingMonday.DayOfWeek != DayOfWeek.Monday)
                startingMonday = startingMonday.AddDays(-1);
      
            sql = new SQL("select sum(t.hours) from Timesheets t, users u where t.dateworked >= @START and t.dateworked <= @END and t.employeeid = u.employeeid and u.username = @USERNAME");
            sql.AddParameter("@START", startingMonday);
            sql.AddParameter("@END", startingMonday.AddDays(7));
            sql.AddParameter("@USERNAME", username);

            double numHoursSoFar = 0;

            try
            {
                numHoursSoFar = double.Parse(sql.Run().Rows[0][0].ToString());
            }
            catch
            {

            }

            double regularHours, overtimeHours;

            //Worked 20
            //Beyond 40 is OT
            //Today is 6

            //if (payCode != "Regular" || numHoursSoFar + hours <= numHoursBeforeOT) //All regular
            //{
                regularHours = hours;
                overtimeHours = 0;
            //}
            //else if (numHoursSoFar > numHoursBeforeOT) //Beyond threshold
            //{
            //    regularHours = 0;
            //    overtimeHours = hours;
            //}
            //else //Crossing threshold
            //{
            //    regularHours = numHoursBeforeOT - numHoursSoFar;
            //    overtimeHours = hours - regularHours;
            //}

            //Insert detail
            //if(regularHours > 0 || payType == 2 || payType == 4)
            //{
                sql = new SQL(@"insert into Timesheets output inserted.timecarddetailid values(
                                    @CREATEUSER,
                                    @PERIOD,
                                    @EMPLOYEEID,
                                    @DATEWORKED,
                                    @PAYTYPE,
                                    @HOURS,
                                    @WORKORDER,
                                    @LUMPSUM,
                                    @OVERRIDERATE,
                                    @DATEENTERED,
                                    @RECORDLOCKED,
                                    @TRANSACTIONTYPE,
                                    @EXPORTED,
                                    @BATCHID,
                                    @DESCRIPTION,
                                    @STARTTIME,
                                    @FINISHTIME,
                                    @REVIEWED)");


                //int timesheetID = int.Parse(SQL.Run("SELECT MAX(TIMECARDDETAILID) FROM Timesheets").Rows[0][0].ToString()) + 1;
                //sql.AddParameter("@TIMESHEETID", timesheetID);
                sql.AddParameter("@CREATEUSER", Core.getUsername());
                sql.AddParameter("@PERIOD", period);
                sql.AddParameter("@EMPLOYEEID", employeeID);
                sql.AddParameter("@DATEWORKED", date);
                sql.AddParameter("@PAYTYPE", payType);
                sql.AddParameter("@HOURS", regularHours);

                if (workOrder != "" && workOrder != null)
                    sql.AddParameter("@WORKORDER", workOrder);
                else
                    sql.AddParameter("@WORKORDER", DBNull.Value);

                sql.AddParameter("@LUMPSUM", lumpSum);
                sql.AddParameter("@OVERRIDERATE", DBNull.Value);
                sql.AddParameter("@DATEENTERED", DateTime.Now);
                sql.AddParameter("@RECORDLOCKED", false);
                sql.AddParameter("@TRANSACTIONTYPE", DBNull.Value);
                sql.AddParameter("@EXPORTED", false);
                sql.AddParameter("@BATCHID", 0);
                sql.AddParameter("@DESCRIPTION", description);
                sql.AddParameter("@STARTTIME", startTime);
                sql.AddParameter("@FINISHTIME", endTime);
                sql.AddParameter("@REVIEWED", false);

                DataTable output = sql.Run();
                int timesheetID = int.Parse(output.Rows[0][0].ToString());

                Core.logHistory("New timesheet", "Timesheet ID# " + timesheetID, "");
                Core.logTimesheetHistory("Timesheet created", timesheetID);
            //}

            //Insert overtime
//            if(overtimeHours > 0)
//            {
//                sql = new SQL(@"insert into Timesheets values(
//                                @TIMESHEETID,
//                                @CREATEUSER,
//                                @PERIOD,
//                                @EMPLOYEEID,
//                                @DATEWORKED,
//                                @PAYTYPE,
//                                @HOURS,
//                                @WORKORDER,
//                                @LUMPSUM,
//                                @OVERRIDERATE,
//                                @DATEENTERED,
//                                @RECORDLOCKED,
//                                @TRANSACTIONTYPE,
//                                @EXPORTED,
//                                @BATCHID,
//                                @DESCRIPTION,
//                                @STARTTIME,
//                                @FINISHTIME,
//                                @REVIEWED)");

//                int timesheetID = int.Parse(SQL.Run("SELECT MAX(TIMECARDDETAILID) FROM Timesheets").Rows[0][0].ToString()) + 1;
//                sql.AddParameter("@TIMESHEETID", timesheetID);
//                sql.AddParameter("@CREATEUSER", Core.getUsername());
//                sql.AddParameter("@PERIOD", period);
//                sql.AddParameter("@EMPLOYEEID", employeeID);
//                sql.AddParameter("@DATEWORKED", date);
//                sql.AddParameter("@PAYTYPE", 105); //Overtime
//                sql.AddParameter("@HOURS", overtimeHours);

//                if (workOrder != "" && workOrder != null)
//                    sql.AddParameter("@WORKORDER", workOrder);
//                else
//                    sql.AddParameter("@WORKORDER", DBNull.Value);

//                sql.AddParameter("@LUMPSUM", lumpSum);
//                sql.AddParameter("@OVERRIDERATE", DBNull.Value);
//                sql.AddParameter("@DATEENTERED", DateTime.Now);
//                sql.AddParameter("@RECORDLOCKED", false);
//                sql.AddParameter("@TRANSACTIONTYPE", DBNull.Value);
//                sql.AddParameter("@EXPORTED", false);
//                sql.AddParameter("@BATCHID", 0);
//                sql.AddParameter("@DESCRIPTION", description);
//                sql.AddParameter("@STARTTIME", startTime);
//                sql.AddParameter("@FINISHTIME", endTime);
//                sql.AddParameter("@REVIEWED", false);

//                sql.Run();
//            }

            //Insert equipment
            for (int i = 0; equipment != null && i < equipment.Length; i++)
            {
                if (equipment[i] != "")
                {
                    sql = new SQL(@"insert into equipmenttimeentry values(
                @TIMESHEETID,
                @EQUIPID,
                @SENT)");

                    //sql.AddParameter("@ENTRYID", int.Parse(SQL.Run("select max(entryid) from equipmenttimeentry").Rows[0][0].ToString())+1);
                    sql.AddParameter("@EQUIPID", equipment[i].TrimStart());
                    sql.AddParameter("@TIMESHEETID", SQL.Run("select max(timecarddetailid) from Timesheets").Rows[0][0].ToString());
                    sql.AddParameter("@SENT", false);
                    
                    sql.Run();
                }
            }
        }

        public static int getStringWidth(string s, Font font)
        {
            return TextRenderer.MeasureText(s, font).Width;
        }

        public static string getErrorText(Exception e)
        {
            string stackTrace = "";

            for (int i = 0; i < e.StackTrace.Split('\n').Length; i++)
            {
                if (e.StackTrace.Split('\n')[i].Contains(":line "))
                    stackTrace += e.StackTrace.Split('\n')[i];
            }

            stackTrace = stackTrace.Replace("   at DATS_Timesheets.", "");
            stackTrace = stackTrace.Replace("C:\\Users\\graeme smyth\\Documents\\Visual Studio 2013\\Projects\\DATS Timesheets\\DATS Timesheets\\", "");
            stackTrace = stackTrace.Replace("..ctor()", " constructor");
            stackTrace = stackTrace.Replace("\r", "\r\n");

            string arguments = "";

            foreach (object arg in e.Data.Values)
                arguments += arg.ToString() + Environment.NewLine;

            if (arguments != "")
                arguments += Environment.NewLine;

            return "User - " + getUsername() + Environment.NewLine
                + "Machine - " + Environment.MachineName + Environment.NewLine
                + "Version - " + getVersion() + Environment.NewLine
                + Environment.NewLine
                + arguments
                + e.GetType().Name + Environment.NewLine
                + e.Message + Environment.NewLine
                + Environment.NewLine
                + (e.InnerException != null ? e.InnerException + Environment.NewLine : "")
                + stackTrace + Environment.NewLine;
        }

        public static void fillHoursCalendar(HoursCalendar hc, string username, DateTime targetDay)
        {
            string empType = Core.isHourly(Core.getEmpID(username)) ? "H" : "S";
            //Calculate pay period
            //SQL sql = new SQL("select [from] from periods where [from]<=@DATE and [to]>=@DATE");
            //sql.AddParameter("@DATE", DateTime.Today);

            //DateTime startingMonday = DateTime.Parse(sql.Run().Rows[0][0].ToString());
            SQL sql;
            DateTime startingMonday = Core.getPeriodStart(Core.getPeriod(DateTime.Today, empType), empType);

            if(targetDay < startingMonday) //We are looking at the past
            {
                while (targetDay < startingMonday)
                    startingMonday = startingMonday.AddDays(-14);
            }
            else if (targetDay < startingMonday.AddDays(14)) //We are looking at the present
            {
                
            }
            else //We are looking at the future
            {
                while (startingMonday < targetDay)
                    startingMonday = startingMonday.AddDays(14);
            }

            sql = new SQL(
@"SELECT
pt.[DESCRIPTION] as PayType,
t.[HOURS] as Hours,
t.DATEWORKED as FullDateWorked

FROM [Timesheets] t

INNER JOIN Users u ON t.employeeid = u.employeeid
INNER JOIN PayCodes pt ON t.paytype = pt.paytype

WHERE u.displayname=@USERNAME
and t.dateworked >= @AFTERTHISDATE
and t.dateworked <= @BEFORETHISDATE

ORDER BY t.TIMECARDDETAILID ASC");
            sql.AddParameter("@AFTERTHISDATE", startingMonday);
            sql.AddParameter("@BEFORETHISDATE", startingMonday.AddDays(14));
            sql.AddParameter("@USERNAME", username);
            DataTable dt = sql.Run();

            hc.clearHours();
            
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string fullDateWorked = dt.Rows[i]["FullDateWorked"].ToString();
                string hours = dt.Rows[i]["Hours"].ToString();
                string payType = dt.Rows[i]["PayType"].ToString();

                hc.addHoursToDay(DateTime.Parse(fullDateWorked), payType, decimal.Parse(hours));
            }
        }

        public static int getEmpID(string username)
        {
            SQL sql = new SQL("select employeeid from users where displayname = @USERNAME");
            sql.AddParameter("@USERNAME", username);

            return int.Parse(sql.Run().Rows[0]["employeeid"].ToString());
        }

        public static bool isAdmin(string username)
        {
            SQL sql = new SQL("select * from users where displayname = @USERNAME and admin = 1");
            sql.AddParameter("@USERNAME", username);
            
            return sql.Run().Rows.Count > 0;
        }

        public static bool canReview(string username)
        {
            SQL sql = new SQL("select u.reviewer from Users u where u.displayname = @USERNAME");
            sql.AddParameter("@USERNAME", username);
            bool supervisor = bool.Parse(sql.Run().Rows[0]["reviewer"].ToString());

            return supervisor || isAdmin(username);
        }
        public static bool CanViewOnly(string username)
        {
            SQL sql = new SQL("select u.viewonlyuser from Users u where u.displayname = @USERNAME");
            sql.AddParameter("@USERNAME", username);
            bool ViewOnlyUser = bool.Parse(sql.Run().Rows[0]["viewonlyuser"].ToString());
            if (ViewOnlyUser != null)
                return ViewOnlyUser;
            else
                return false;
        }
        public static bool canApprove(string username)
        {
            SQL sql = new SQL("select u.approver from Users u where u.displayname = @USERNAME");
            sql.AddParameter("@USERNAME", username);
            bool manager = bool.Parse(sql.Run().Rows[0]["approver"].ToString());

            return manager || isAdmin(username);
        }

        public static bool isFacilities(string username)
        {
            //ALSO SEE:
            //printTimesheetsToolStripMenuItem_Click in Profile.cs

            int[] d = Core.getDepartments(username);

            for(int i = 0; i < d.Length; i++)
            {
                bool isFacilities = bool.Parse(SQL.RunString("select isfacilities from department where departmentid = @ID", d[0]));

                if(isFacilities)
                    return true;
            }

            return false;
        }

        public static double getBankedTimeTotals(string username)
        {
            SQL sql = new SQL(@"
select EmployeeID, SUM(hours) from Timesheets
where
PayType = 950
and DateWorked >= @START
and DateWorked < @END
and employeeid = @EMPID
group by EmployeeID");
            sql.AddParameter("@START", new DateTime(DateTime.Today.Year, 1, 1));
            sql.AddParameter("@END", new DateTime(DateTime.Today.Year + 1, 1, 1));
            sql.AddParameter("@EMPID", Core.getEmpID(username));
            DataTable dt = sql.Run();

            double banked = 0;
            if (dt.Rows.Count > 0)
                banked = 1.0 * double.Parse(dt.Rows[0][1].ToString());

            sql = new SQL(@"
select EmployeeID, SUM(hours) from Timesheets
where
PayType = 955
and DateWorked >= @START
and DateWorked < @END
and employeeid = @EMPID
group by EmployeeID");
            sql.AddParameter("@START", new DateTime(DateTime.Today.Year, 1, 1));
            sql.AddParameter("@END", new DateTime(DateTime.Today.Year + 1, 1, 1));
            sql.AddParameter("@EMPID", Core.getEmpID(username));
            dt = sql.Run();

            if (dt.Rows.Count > 0)
                banked += 1.5 * double.Parse(dt.Rows[0][1].ToString());

            return banked;
        }

        public static double getBankedTimeOffTotals(string username)
        {
            SQL sql = new SQL(@"
select EmployeeID, SUM(hours) from Timesheets
where
PayType = 81
and DateWorked >= @START
and DateWorked < @END
and employeeid = @EMPID
group by EmployeeID");
            sql.AddParameter("@START", new DateTime(DateTime.Today.Year, 1, 1));
            sql.AddParameter("@END", new DateTime(DateTime.Today.Year + 1, 1, 1));
            sql.AddParameter("@EMPID", Core.getEmpID(username));
            DataTable dt = sql.Run();

            double timeTakenOff = 0;
            if (dt.Rows.Count > 0)
                timeTakenOff = 1.0 * double.Parse(dt.Rows[0][1].ToString());

            return timeTakenOff;
        }

        public static double getBankedTimeBalance(string username)
        {
            return getBankedTimeTotals(username) - getBankedTimeOffTotals(username);
        }

        public static bool isFacilityMaintenance(string username)
        {
            int[] d = Core.getDepartments(username);

            if (d.Contains(5))
                return true;

            return false;
        }

        public static bool isFacilities()
        {
            return isFacilities(Core.getUsername());
        }

        public static int[] getDepartments()
        {
            return getDepartments(Core.getUsername());
        }

        public static int[] getDepartments(string username)
        {
            SQL sql = new SQL("select d.departmentid, d.userid from departmentassociations d, users u where d.userid = u.userid and u.displayname = @USERNAME order by d.departmentid desc");
            sql.AddParameter("@USERNAME", username);
            DataTable dt = sql.Run();

            int[] r = new int[dt.Rows.Count];

            for (int i = 0; i < dt.Rows.Count; i++)
                r[i] = int.Parse(dt.Rows[i]["departmentid"].ToString());

            return r;
        }

        public static bool showUserWorkOrders()
        {
            return showUserWorkOrders(Core.getUsername());
        }

        public static bool showUserWorkOrders(string username)
        {
            int[] departments = getDepartments(username);
            bool r = false;

            for (int i = 0; i < departments.Length; i++)
                if (departmentUsesWorkOrders(departments[i]))
                    r = true;

            return r;
        }

        public static bool showUserEquipment()
        {
            return showUserEquipment(Core.getUsername());
        }

        public static bool showUserEquipment(string username)
        {
            int[] departments = getDepartments(username);
            bool r = false;

            for (int i = 0; i < departments.Length; i++)
                if (departmentUsesEquipment(departments[i]))
                    r = true;

            return r;
        }

        public static bool departmentUsesWorkOrders(int d)
        {
            return bool.Parse(SQL.RunString("select usesworkorders from department where departmentid = @ID", d));
        }

        public static bool departmentUsesEquipment(int d)
        {
            return bool.Parse(SQL.RunString("select usesequipment from department where departmentid = @ID", d));
        }

        public static DateTime getStartingMonday(string empType)
        {
            return getStartingMonday(DateTime.Today, empType);
        }

        public static DateTime getStartingMonday(DateTime date, string empType)
        {
            //string jdeDate = Core.dateToJDE(date.ToString());

            //Oracle ora = new Oracle("select ");
            return getPeriodStart(getPeriod(date, empType), empType);


            //SQL sql = new SQL("select [from] from periods where [from]<=@DATE and [to]>=@DATE");
            //sql.AddParameter("@DATE", date);
            //DataTable dt = sql.Run();

            //if (dt.Rows.Count > 0)
            //    return DateTime.Parse(dt.Rows[0][0].ToString());
            //else
            //    return getStartingMonday(DateTime.Today);
        }

        public static string getDepartmentStartingTime()
        {
            return getDepartmentStartingTime(Core.getUsername());
        }

        public static string getDepartmentStartingTime(string username)
        {
            int d = getDepartments(username)[0];
            return SQL.RunString("select StartTime from department where departmentid = @ID", d);
        }

        public static bool getDepartmentForcesDescription(string username)
        {
            int d = getDepartments(username)[0];
            return bool.Parse(SQL.RunString("select DescriptionMandatory from department where departmentid = @ID", d));
        }

        public static string getDepartmentEndingTime()
        {
            return getDepartmentEndingTime(Core.getUsername());
        }

        public static string getDepartmentEndingTime(string username)
        {
            int d = getDepartments(username)[0];
            return SQL.RunString("select EndTime from department where departmentid = @ID", d);
        }

        public static decimal getDepartmentDailyHours()
        {
            return getDepartmentDailyHours(Core.getUsername());
        }

        public static void fillListView(ListView list, DataTable dt)
        {
            list.Items.Clear();
            list.Columns.Clear();

            for (int i = 0; i < dt.Columns.Count; i++)
                list.Columns.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName);

            for (int r = 0; r < dt.Rows.Count; r++)
            {
                ListViewItem li = new ListViewItem(dt.Rows[r][0].ToString());

                for (int c = 1; c < dt.Columns.Count; c++)
                    li.SubItems.Add(dt.Rows[r][c].ToString());

                list.Items.Add(li);
            }

            for (int i = 0; i < list.Columns.Count; i++)
            {
                list.Columns[i].Width = -2;
                list.Columns[i].Name = list.Columns[i].Text;
            }
        }

        public static decimal getDepartmentDailyHours(string username)
        {
            if (username == "Dionne Grant" || username == "Grant, Dionne Angela")
                return 8;

            int d = getDepartments(username)[0];
            return decimal.Parse(SQL.RunString("select dailyhours from department where departmentid = @ID", d));
        }

        //Takes a date in string format, and returns that string in JDE-style date
        public static string dateToJDE(string date)
        {
            string returnable = "1";

            DateTime temp = DateTime.Parse(date);

            int year = temp.Year;
            int day = temp.DayOfYear;

            returnable += (year.ToString()).Substring(2, 2);

            if (day < 10)
                returnable += "00";
            else if (day < 100)
                returnable += "0";

            returnable += day;

            return returnable;
        }

        //Takes a date in JDE format, and returns that string as a date
        public static DateTime JDEToDate(string JDEDate)
        {
            int year = 0;
            int daysToAdd = 0;

            if (JDEDate.Length == 5)
            {
                year = int.Parse(JDEDate.Substring(0, 2)) + 1900;
                daysToAdd = int.Parse(JDEDate.Substring(2, 3));
            }
            else if (JDEDate.Length == 6)
            {
                if (JDEDate[0] == '0')
                    year = int.Parse(JDEDate.Substring(1, 2)) + 1900;
                else if (JDEDate[0] == '1')
                    year = int.Parse(JDEDate.Substring(1, 2)) + 2000;

                daysToAdd = int.Parse(JDEDate.Substring(3, 3));
            }

            DateTime date = new DateTime(year, 1, 1);

            date = date.AddDays(daysToAdd - 1);

            return date;
        }

        public static bool isUsernameTaken(string username)
        {
            bool returnable;

            SQL sql = new SQL("select count(*) from users where username=@USERNAME");
            sql.AddParameter("@USERNAME", username);
            
            returnable = (int.Parse(sql.Run().Rows[0][0].ToString()) >= 1);

            return returnable;
        }

        public static bool isEmployeeIDTaken(int empID)
        {
            bool returnable;

            SQL sql = new SQL("select count(*) from users where employeeid=@EMPID");
            sql.AddParameter("@EMPID", empID);

            returnable = (int.Parse(sql.Run().Rows[0][0].ToString()) >= 1);

            return returnable;
        }

        public static bool isEmpIDValid(string empID)
        {
            bool returnable = false;

            if (empID == null || empID == "")
                return false;

            try
            {
                Oracle ora = new Oracle("select YAAN8 from " + Core.getSchema(Core.getEnvironment()) + ".F060116 where YAAN8 = :EMP_ID");
                ora.AddParameter("EMP_ID", int.Parse(empID));
                DataTable dt = ora.Run();

                returnable = dt.Rows.Count > 0; //If employeeID is valid
                //System.Diagnostics.Debug.WriteLine(dt.Rows[0][0].ToString());
            }
            catch (System.TypeInitializationException e2)
            {
                throw e2;
            }
            catch
            {
                return false;
            }
            return returnable;
        }
    }
}
