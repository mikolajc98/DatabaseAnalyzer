using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseAnalyzer.Models;

namespace ConsoleController
{
    class Program
    {
        static void Main(string[] args)
        {
            // you have to provide program with 4 parameters in this order: "server name" "database name" "user name" "user password"
            string[] data = LoadArguments(args);

            string
                server = data[0],
                db = data[1],
                userName = data[2],
                userPwd = data[3];



            if (string.IsNullOrEmpty(userName))
                DatabaseAnalyzer.Main.Database.InitializeConnection(server, db);

            else
                DatabaseAnalyzer.Main.Database.InitializeConnection(server, db, userName, userPwd);


            //done
            DatabaseAnalyzer.SQL_Queries.DataContainerQuery dcq = new DatabaseAnalyzer.SQL_Queries.DataContainerQuery();
            List<DatabaseAnalyzer.Models.DataContainer> databaseDataContainers = (List<DatabaseAnalyzer.Models.DataContainer>)dcq.ExecuteAndReturn();

            //done
            DatabaseAnalyzer.SQL_Queries.ExecutableQuery eq = new DatabaseAnalyzer.SQL_Queries.ExecutableQuery();
            List<DatabaseAnalyzer.Models.Executable> databaseExecutables = (List<DatabaseAnalyzer.Models.Executable>)eq.ExecuteAndReturn();

            //done
            DatabaseAnalyzer.SQL_Queries.UserDefinedTypesQuery udt = new DatabaseAnalyzer.SQL_Queries.UserDefinedTypesQuery();
            List<DatabaseAnalyzer.Models.Type> userTypes = (List<DatabaseAnalyzer.Models.Type>)udt.ExecuteAndReturn();

            //done
            DatabaseAnalyzer.SQL_Queries.UserDefinedTableTypesQuery udtt = new DatabaseAnalyzer.SQL_Queries.UserDefinedTableTypesQuery();
            List<DatabaseAnalyzer.Models.Table> userTableTypes = (List<DatabaseAnalyzer.Models.Table>)udtt.ExecuteAndReturn();

            WriteStatsToConsole(databaseDataContainers, databaseExecutables, userTypes, userTableTypes);

            Console.ReadLine();
        }


        private static string[] LoadArguments(string[] args)
        {
            if (args.Length != 4)
            {
                throw new Exception($"Parameter not provided!");
            }

            string[] result = new string[args.Length];


            for (int i = 0; i < args.Length; i++)
                result[i] = args[i];

            return result;
        }

        private static void WriteStatsToConsole(List<DataContainer> databaseDataContainers, List<Executable> databaseExecutables, List<DatabaseAnalyzer.Models.Type> userTypes, List<Table> userTableTypes)
        {
            Console.WriteLine($"Scanning ... Server:{DatabaseAnalyzer.Main.Database.Server} DB:{DatabaseAnalyzer.Main.Database.DatabaseName}");

            Console.WriteLine();
            Console.WriteLine($"Category: Tables / Views");
            Console.WriteLine();
            Console.WriteLine($"Found {databaseDataContainers.Count(x => x is DatabaseAnalyzer.Models.Table)} Tables and {databaseDataContainers.Count(y => y is DatabaseAnalyzer.Models.View)} Views");

            foreach (var dt in databaseDataContainers)
            {
                Console.WriteLine();
                Console.WriteLine($"{dt.SchemaDetails.Name}.{dt.Name} ({dt.TypeStr})");
                foreach (var col in dt.Columns)
                {
                    string defValue = string.IsNullOrEmpty(col.TypeDetails.DefaultValue) ? "" : $"[{col.TypeDetails.DefaultValue}]";
                    Console.WriteLine($"\t{col.Name} {col.TypeDetails.Name} ({col.TypeDetails.Precision}) {defValue}");
                }
            }

            Console.WriteLine();
            Console.WriteLine($"Category: Executables");
            Console.WriteLine();
            Console.WriteLine($"Found {databaseExecutables.Count(x => x is DatabaseAnalyzer.Models.Procedure)} Procedures and {databaseExecutables.Count(x => x is DatabaseAnalyzer.Models.Function)} Functions");

            foreach (var exec in databaseExecutables)
            {
                Console.WriteLine();
                Console.WriteLine($"{exec.SchemaDetails.Name}.{exec.Name} ({exec.TypeStr})");
                foreach (var param in exec.Params)
                {
                    string defValue = string.IsNullOrEmpty(param.TypeDetails.DefaultValue) ? "" : $"[{param.TypeDetails.DefaultValue}]";
                    Console.WriteLine($"\t{param.Name} {param.TypeDetails.Name} ({param.TypeDetails.Precision}) {defValue}");
                }
            }

            Console.WriteLine();
            Console.WriteLine($"Category: User defined types");
            Console.WriteLine();
            Console.WriteLine($"Found {userTypes.Count()} User-defined types");

            foreach (var t in userTypes)
            {
                string defValue = string.IsNullOrEmpty(t.DefaultValue) ? "" : $"[{t.DefaultValue}]";
                Console.WriteLine($"\t{t.Name} ({t.Precision}) {defValue}");
            }

            Console.WriteLine();
            Console.WriteLine($"Category: user defined table types");
            Console.WriteLine();
            Console.WriteLine($"Found {userTableTypes.Count()} User-defined table types");

            foreach (var userTableType in userTableTypes)
            {
                Console.WriteLine();
                Console.WriteLine($"{userTableType.SchemaDetails.Name}.{userTableType.Name} ({userTableType.TypeStr})");
                foreach (var col in userTableType.Columns)
                {
                    Console.WriteLine($"\t{col.Name} {col.TypeDetails.Name} ({col.TypeDetails.Precision})");
                }
            }
        }
    }
}
