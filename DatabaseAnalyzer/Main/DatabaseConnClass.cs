using System.Data.SqlClient;
using System;
using System.Data;

namespace DatabaseAnalyzer.Main
{
    // Static connection - only one per whole library
    public static class Database
    {
        private static SqlConnection Connection;
        private static SqlCommand Command;
        public static bool IsInitialized { get; private set; }

        public static string DatabaseName
        {
            get
            {
                if(IsInitialized)
                {
                    return Connection.Database;
                }
                return "";
            }
        }

        public static string Server
        {
            get
            {
                if(IsInitialized)
                {
                    return Connection.DataSource;
                }
                return "";
            }
        }
        public delegate object ObjectCreator(DataTableCollection Tables);

        public static bool InitializeConnection(string ServerAdress, string Database)
        {
            Connection = new SqlConnection()
            {
                ConnectionString =
                $"Data Source='{ServerAdress}';" +
                $"Initial Catalog='{Database}';" +
                $"Integrated Security='SSPI';"
            };

            return CheckConnetion();
        }


        public static bool InitializeConnection(string ServerAdress, string DatabaseName, string UserName, string UserPwd)
        {
            if (IsInitialized)
                return CheckConnetion();

            Connection = new SqlConnection()
            {
                ConnectionString =
                $"Data Source='{ServerAdress}';" +
                $"Initial Catalog='{DatabaseName}';" +
                $"User ID='{UserName}';" +
                $"Password='{UserPwd}'"
            };
            return CheckConnetion();
        }

        public static bool CheckConnetion()
        {
            try
            {
                Command = new SqlCommand("SELECT 1;", Connection);
                Connection.Open();

                switch (Connection.State)
                {
                    case ConnectionState.Open:
                        break;
                    case ConnectionState.Closed:
                        throw new Exception("Connection is closed!");
                    case ConnectionState.Broken:
                    case ConnectionState.Connecting:
                    case ConnectionState.Executing:
                    case ConnectionState.Fetching:
                        throw new Exception("Unknown connection state");
                }


                int result = (int)Command.ExecuteScalar();

                return IsInitialized = result.Equals(1) ? true : false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static object ExecuteQuery(string Query, ObjectCreator Oc)
        {
            if (Command.Connection.State != ConnectionState.Open)
            {
                throw new Exception($"Connection has wrong state ({Command.Connection.State})!");
            }

            // Set parameters to 0 - Clears Command memory
            Command.Parameters.Clear();

            Command.CommandType = CommandType.Text;
            Command.CommandText = Query;


            object result;
            SqlDataAdapter reader = new SqlDataAdapter(Command);
            DataSet ds = new DataSet();

            if (Oc != null)
            {
                reader.Fill(ds);
                result = Oc(ds.Tables);
            }
            else
            {
                result = Command.ExecuteScalar();
            }
            return result;
        }
    }
}
