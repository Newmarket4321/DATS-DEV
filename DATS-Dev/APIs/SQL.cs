using System.Data;

namespace DATS_Timesheets
{
    public class SQL : Database
    {
        static DatabaseType type = DatabaseType.SQL;
        static string hostServer = Core.getEnvironment() == "PD" ? "172.16.0.34" : "172.16.0.46";
        static string database = "DATS";
        static string username = "DATS";
        static string password = "SQL2008@r2";

        public SQL(string str) : base(type, str, hostServer, database, username, password)
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
