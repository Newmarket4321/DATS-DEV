using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Diagnostics;

namespace DATS_Timesheets
{
    static public class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // Add the event handler for handling UI thread exceptions to the event.
                Application.ThreadException += new ThreadExceptionEventHandler(threadException);

                // Set the unhandled exception mode to force all Windows Forms errors
                // to go through our handler.
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

                // Add the event handler for handling non-UI thread exceptions to the event. 
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(unhandledException);

                string arguments = "";
                for (int i = 0; i < args.Length; i++)
                    arguments += args[i] + " ";

                //Install
                if (Core.getExecutionPath() == Core.getDeploymentPath())
                {
                    File.Copy(Core.getDeploymentPath(), Core.getUserDesktopDirectory() + @"\DATS.exe", true);
                    MessageBox.Show("DATS.exe has been installed! Check your desktop for the new icon.");
                    return;
                }

                //If this is not Update.exe, you can remove Update.exe
                if (args.Length == 0 || (args.Length > 0 && args[0] != "Update"))
                {
                    Core.cleanDelete(Core.getExecutionDirectory() + @"\Update.exe");

                    //Place prerequisite files
                    Core.checkPrerequisite("Oracle.ManagedDataAccess.dll");
                    Core.checkPrerequisite("DocumentFormat.OpenXml.dll");
                    Core.checkPrerequisite("ClosedXML.dll");
                }

                //Update is needed
                if (Core.getVersion() < Core.getLatestVersion()
                    && (args.Length == 0 || (args.Length > 0 && args[0] != "Update")))
                {
                    createAndStartUpdate(args);
                    return;
                }

                //Begin
                if (args.Length == 0) //No parameters
                    Application.Run(new Form1());
                else //With parameters
                {
                    if (args[0] == "Update") //First parameter is Update
                    {
                        Core.logHistory("Update", args[0], "");
                        iAmUpdate(args);
                    }
                    else //First parameter is other
                        throw new Exception("DATS parameter not recognized");
                }
            }
            catch (Exception e)
            {
                Core.error(e);
            }

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);

            //if (!File.Exists("C:\\DATS\\Oracle.ManagedDataAccess.dll"))
            //    File.Copy("\\\\data\\files\\PCapps\\DATS\\Oracle.ManagedDataAccess.dll", "C:\\DATS\\Oracle.DataAccess.dll");

            //if (!File.Exists("C:\\DATS\\ClosedXML.dll"))
            //    File.Copy("\\\\data\\files\\PCapps\\DATS\\ClosedXML.dll", "C:\\DATS\\ClosedXML.dll");

            //if (!File.Exists("C:\\DATS\\DocumentFormat.OpenXml.dll"))
            //    File.Copy("\\\\data\\files\\PCapps\\DATS\\DocumentFormat.OpenXml.dll", "C:\\DATS\\DocumentFormat.OpenXml.dll");

            //if (Environment.MachineName == "SYSGS-11-13")
            //{
            //    try
            //    {
            //        Application.Run(new Form1());
            //    }
            //    catch (ObjectDisposedException e)
            //    {

            //    }
            //}
            //else
            //{
            //    try
            //    {
            //        Application.Run(new Form1());
            //    }
            //    catch (ObjectDisposedException e)
            //    {

            //    }
            //    catch (Exception e)
            //    {
            //        Core.sendMail("gsmyth@newmarket.ca", "DATS Error",
            //            Core.getUsername() + Environment.NewLine + Environment.NewLine + Environment.MachineName + Environment.NewLine + Environment.NewLine + e.InnerException + Environment.NewLine + Environment.NewLine +
            //            e.Message + Environment.NewLine + Environment.NewLine +
            //            e.Source + Environment.NewLine + Environment.NewLine +
            //            e.StackTrace + Environment.NewLine + Environment.NewLine +
            //            e.TargetSite);

            //        MessageBox.Show("An error occurred, however it has automatically been reported to IT." + Environment.NewLine + "These things are typically resolved within an hour. Please try again soon.");
            //    }
            //}
        }

        static void createAndStartUpdate(string[] args)
        {
            string arguments = "Update";

            for (int i = 0; i < args.Length; i++)
                arguments += " " + args[i];

            string source = Assembly.GetEntryAssembly().Location;
            string destination = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\Update.exe";

            try
            {
                File.Copy(source, destination, true);
            }
            catch
            {
                foreach (var process in Process.GetProcessesByName("Update"))
                    process.Kill();

                Thread.Sleep(1000);

                try
                {
                    File.Copy(source, destination, true);
                }
                catch
                {
                    Thread.Sleep(1000);
                    File.Copy(source, destination, true);
                }
            }

            string path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Update.exe";
            Core.run(path, arguments);
        }

        public static void iAmUpdate(string[] args)
        {
            try
            {
                string arguments = "";

                for (int i = 1; i < args.Length; i++) //Start at 1 because 0th parameter is "Update"
                    arguments += " " + args[i];

                arguments = arguments.Trim();

                string source = Core.getDeploymentDirectory() + "DATS.exe";
                string destination = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\DATS.exe";

                bool copied = false;

                for (int i = 0; !copied; i++)
                {
                    try
                    {
                        File.Copy(source, destination, true);
                        copied = true;
                    }
                    catch (Exception e)
                    {
                        if (i < 10)
                            Core.sleep(1000);
                        else
                        {
                            Core.logHistory("Crash note", "Update.exe unable to copy from " + source + " to " + destination, "");
                            throw e;
                        }
                    }
                }

                Core.run(destination, arguments);
            }
            catch (Exception e)
            {
                Core.error(e);
            }
        }

        static void threadException(object sender, ThreadExceptionEventArgs t)
        {
            //MessageBox.Show("Got one!");
            Core.error(t.Exception);
        }

        static void unhandledException(object sender, UnhandledExceptionEventArgs u)
        {
            //MessageBox.Show("The one that tried to get away!");
            Core.error((Exception)u.ExceptionObject);
        }
    }
}
