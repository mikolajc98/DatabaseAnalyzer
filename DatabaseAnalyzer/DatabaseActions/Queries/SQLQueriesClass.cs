using System.Text;

namespace DatabaseAnalyzer.DatabaseActions.Queries
{
    public static class SQLQueries
    {
        public static string Query_UserDefinedTypes { get; } = ReadAndPrepareQuery(@"DatabaseActions\Queries\Query_UserTypes.sql");
        public static string Query_UserDefinedTables { get; } = ReadAndPrepareQuery(@"DatabaseActions\Queries\Query_UserTables.sql");
        public static string Query_Exeuctables { get; } = ReadAndPrepareQuery(@"DatabaseActions\Queries\Query_Executables.sql");
        public static string Query_DataContainers { get; } = ReadAndPrepareQuery(@"DatabaseActions\Queries\Query_DataContainers.sql");


        public static string ReadAndPrepareQuery(string scriptName)
        {
            StringBuilder query = new StringBuilder();

            string[] lines = System.IO.File.ReadAllLines(scriptName);

            foreach (var line in lines)
                query.Append($"{Test(line)} ");

            return query.ToString();
        }

        private static string Test(string line)
        {
            if (line.Contains("--"))
            {
                line = line.Replace("--", "/*") + "*/";
            }

            return line;
        }

    }
}
