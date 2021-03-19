using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DatabaseAnalyzer.Models;

namespace DatabaseAnalyzer.DatabaseActions
{
    public class ExecutableQuery : SQLQuery<Executable>
    {
        private readonly string SQLScript = Queries.SQLQueries.Query_Exeuctables;
        public override IEnumerable<Executable> ExecuteAndReturn()
        {
            return (IEnumerable<Executable>)DatabaseAnalyzer.Main.Database.ExecuteQuery(SQLScript, Creator);
        }

        private object Creator(DataTableCollection tables)
        {
            List<Executable> result = new List<Executable>();
            DataTable data = tables[0];

            List<string> executableNames = data.AsEnumerable().Select(x => x.Field<string>("NAME")).Distinct().ToList();
            Dictionary<string, IEnumerable<Parameter>> paramsForExecutable = new Dictionary<string, IEnumerable<Parameter>>();

            foreach (var executableName in executableNames)
            {
                List<Parameter> parameters = new List<Parameter>();

                foreach (var executableData in data.AsEnumerable().Where(x => x.Field<string>("NAME").Equals(executableName)))
                {
                    string parameterName = executableData["PARAM_NAME"].ToString();
                    bool nullable = (bool)executableData["NULLABLE"];
                    Models.Type paramType = new Models.Type(executableData["TYPE_NAME"].ToString(), executableData["PRECISION"].ToString(),"");
                    bool isParamTable = (bool)executableData["IS_TABLE"];

                    parameters.Add(new Parameter(parameterName, nullable, paramType, isParamTable));
                }
                paramsForExecutable.Add(executableName, parameters);
            }


            foreach (var executableDetails in data.AsEnumerable().GroupBy(x => x.Field<string>("NAME")).Select(y => y.First()).ToList())
            {
                Executable exec;
                string name = executableDetails["NAME"].ToString();
                Schema sch = new Schema((int)executableDetails["SCHEMA_ID"], executableDetails["SCHEMA_NAME"].ToString());
                IEnumerable<Parameter> parameters = paramsForExecutable[name];

                if (executableDetails["TYPE"].ToString() == "P")
                {
                    exec = new Procedure(name, sch, parameters);

                }
                else if (executableDetails["TYPE"].ToString() == "F")
                {
                    exec = new Function(name, sch, parameters);
                }
                else
                {
                    throw new Exception($"Unknown executable type {executableDetails["TYPE"]}!");
                }

                result.Add(exec);
            }

            return result;
        }
    }
}
