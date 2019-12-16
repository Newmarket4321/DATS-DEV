using System.Data;

namespace DATS_Timesheets
{
    class Oracle : Database
    {
        static DatabaseType type = DatabaseType.Oracle;
        static string hostServer = "e1";
        static string database = "jde";
        static string username = "jde";
        static string password = "purple1";

        public Oracle(string str) : base(type, str, hostServer, database, username, password)
        {

        }

        public static DataTable Run(string str, params object[] list)
        {
            return Run(type, str, hostServer, database, username, password, list);
        }

        new public static string RunString(string str, params object[] list)
        {
            return Run(str, list).Rows[0][0].ToString();
        }
    }
}
