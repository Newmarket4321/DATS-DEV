using Oracle.ManagedDataAccess.Client;
using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;

namespace DATS_Timesheets
{
    public enum DatabaseType
    {
        SQL,
        Oracle,
        MySQL
    }

    public class Database
    {
        DatabaseType type;
        object con;
        object com;
        object sda;

        public Database(DatabaseType dataType, string str, string connectionString)
        {
            type = dataType;

            if (type == DatabaseType.SQL)
            {
                con = new SqlConnection(connectionString);
                com = new SqlCommand(str, (SqlConnection)con);
            }
            else if (type == DatabaseType.Oracle)
            {
                str = str.Replace("@", ":");
                con = new OracleConnection(connectionString);
                com = new OracleCommand(str, (OracleConnection)con);
            }
        }

        public Database(DatabaseType dataType, string str, string hostServer, string database, string username, string password)
        {
            type = dataType;

            if (type == DatabaseType.SQL)
            {
                con = new SqlConnection("Server=" + hostServer + "; Database=" + database + "; User Id=" + username + "; password=" + password);
                com = new SqlCommand(str, (SqlConnection)con);
            }
            else if (type == DatabaseType.Oracle)
            {
                str = str.Replace("@", ":");
                con = new OracleConnection("Data Source=(DESCRIPTION =(ADDRESS_LIST =(ADDRESS = (PROTOCOL = TCP)(HOST = " + hostServer + ")(PORT = 1521)))(CONNECT_DATA =(SID = " + database + ")    )  );User Id=" + username + ";Password=" + password + ";");
                com = new OracleCommand(str, (OracleConnection)con);
            }
        }

        public string Query
        {
            get
            {
                if (type == DatabaseType.SQL)
                    return ((SqlCommand)com).CommandText;
                else if (type == DatabaseType.Oracle)
                    return ((OracleCommand)com).CommandText;
                else
                    return null;
            }
            set
            {
                if (type == DatabaseType.SQL)
                    ((SqlCommand)com).CommandText = value;
                else if (type == DatabaseType.Oracle)
                {
                    value = value.Replace("@", ":");
                    ((OracleCommand)com).CommandText = value;
                }

                ClearParameters();
            }
        }

        public new DatabaseType GetType()
        {
            return type;
        }

        public Database Clone()
        {
            Database d = null;

            if (type == DatabaseType.SQL)
                d = new Database(type, ((SqlCommand)com).CommandText, ((SqlConnection)con).ConnectionString);
            else if (type == DatabaseType.Oracle)
                d = new Database(type, ((OracleCommand)com).CommandText, ((OracleConnection)con).ConnectionString);

            return d;
        }

        public void ClearParameters()
        {
            if (type == DatabaseType.SQL)
                ((SqlCommand)com).Parameters.Clear();
            else if (type == DatabaseType.Oracle)
                ((OracleCommand)com).Parameters.Clear();
        }

        public void AddParameter(string tag, object x)
        {
            if (type == DatabaseType.SQL)
            {
                SqlDbType type = SqlDbType.VarChar;

                if (x is object[])
                    x = (x as object[])[0];

                if (x is int)
                    type = SqlDbType.Int;
                else if (x is double || x is float)
                    type = SqlDbType.Decimal;
                else if (x is bool)
                    type = SqlDbType.Bit;
                else if (x is char)
                    type = SqlDbType.Char;
                else if (x is string || x is DBNull)
                    type = SqlDbType.VarChar;
                else if (x is DateTime)
                    type = SqlDbType.DateTime;

                ((SqlCommand)com).Parameters.Add(tag, type).Value = x;
            }
            else if (type == DatabaseType.Oracle)
            {
                tag = tag.Replace("@", "");
                tag = tag.Replace(":", "");

                OracleDbType type = OracleDbType.Varchar2;

                if (x is object[])
                    x = (x as object[])[0];

                if (x is int)
                    type = OracleDbType.Int32;
                else if (x is double || x is bool || x is float)
                    type = OracleDbType.Decimal;
                else if (x is char)
                    type = OracleDbType.Char;
                else if (x is string || x is DBNull)
                    type = OracleDbType.Varchar2;
                else if (x is DateTime)
                    type = OracleDbType.Date;

                ((OracleCommand)com).Parameters.Add(tag, type).Value = x;
            }
        }

        public void AddParameter(object x)
        {
            string str = (type == DatabaseType.SQL ? ((SqlCommand)com).CommandText : ((OracleCommand)com).CommandText);
            int parNo = (type == DatabaseType.SQL ? ((SqlCommand)com).Parameters.Count : ((OracleCommand)com).Parameters.Count);

            char delimiter = (type == DatabaseType.SQL ? '@' : ':');
            string chunk = str.Split(delimiter)[parNo + 1];
            int length;

            for (length = 0; length < chunk.Length && Char.IsLetterOrDigit(chunk[length]); length++) ;

            string tag = (type == DatabaseType.SQL ? "@" : "") + chunk.Substring(0, length);

            AddParameter(tag, x);
        }

        public DataTable Run(string str, params object[] list)
        {
            Query = str;

            if (list.Length == 1 && list[0] is object[])
            {
                object[] innerList = list[0] as object[];

                for (int i = 0; i < innerList.Length; i++)
                    AddParameter(innerList[i]);
            }
            else
            {
                for (int i = 0; i < list.Length; i++)
                    AddParameter(list[i]);
            }

            return Run();
        }

        public string RunString(string str, params object[] list)
        {
            return Run(str, list).Rows[0][0].ToString();
        }

        public DataTable Run()
        {
            DataTable dt = new DataTable();

            if (type == DatabaseType.SQL)
            {
                sda = new SqlDataAdapter((SqlCommand)com);
                
                ((SqlDataAdapter)sda).Fill(dt);
            }
            else if (type == DatabaseType.Oracle)
            {
                sda = new OracleDataAdapter((OracleCommand)com);
                ((OracleDataAdapter)sda).Fill(dt);

                //Clean up to avoid "Pooled connection request timed out" exception.
                ((OracleCommand)com).Connection.Close();
            }

            return dt;
        }

        public static DataTable Run(DatabaseType type, string str, string hostServer, string database, string username, string password, params object[] list)
        {
            Database db = new Database(type, str, hostServer, database, username, password);

            if (list.Length == 1 && list[0] is object[])
            {
                object[] innerList = list[0] as object[];

                for (int i = 0; i < innerList.Length; i++)
                    db.AddParameter(innerList[i]);
            }
            else
            {
                for (int i = 0; i < list.Length; i++)
                    db.AddParameter(list[i]);
            }

            return db.Run();
        }
    }

    public class Data
    {
        DataRow dr;

        public Data(DataRow d)
        {
            dr = d;
        }

        public static implicit operator Data(DataRow d)
        {
            return new Data(d);
        }

        public Data(DataTable dt, int i)
        {
            dr = dt.Rows[i];
        }

        public string this[string key]
        {
            get { return dr[key].ToString(); }
            set { dr[key] = value; }
        }
    }
}
