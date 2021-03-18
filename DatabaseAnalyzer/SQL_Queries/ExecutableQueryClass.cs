using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DatabaseAnalyzer.Models;

namespace DatabaseAnalyzer.SQL_Queries
{
    public class ExecutableQuery : SQLQuery<Executable>
    {
        private readonly string SQLScript = ";WITH PF AS	(	 SELECT object_id, name, schema_id, 'P' AS 'TYPE' FROM sys.procedures		 UNION ALL 		 SELECT object_id, name, schema_id, 'F' FROM sys.all_objects WHERE TYPE IN ('IF','TF','FN')	)		SELECT	PF.TYPE 'TYPE',	PF.name AS 'NAME',	PF.schema_id 'SCHEMA_ID',	SCHMS.name AS 'SCHEMA_NAME',	CASE	 WHEN PARAMS.is_output = 1 THEN '@RETURNS'	 ELSE PARAMS.name	END AS 'PARAM_NAME',		CASE 	 WHEN PARAMS.system_type_id = PARAMS.user_type_id THEN SYSTYPE.name	 ELSE USRTYPE.name 	END AS 'TYPE_NAME',		CASE	 WHEN PARAMS.precision = 0 AND PARAMS.scale = 0 THEN CAST(PARAMS.max_length AS nvarchar(100))	 ELSE CONCAT(PARAMS.PRECISION,', ',PARAMS.scale)	END AS 'PRECISION',		PARAMS.is_nullable AS 'NULLABLE',		USRTYPE.is_table_type AS 'IS_TABLE'		FROM sys.all_parameters PARAMS		LEFT JOIN PF ON PF.object_id = PARAMS.object_id	JOIN sys.schemas SCHMS ON PF.schema_id = SCHMS.schema_id		JOIN sys.types SYSTYPE ON PARAMS.system_type_id = SYSTYPE.system_type_id AND PARAMS.system_type_id = SYSTYPE.user_type_id	LEFT JOIN sys.types USRTYPE ON PARAMS.system_type_id = USRTYPE.system_type_id AND PARAMS.user_type_id = USRTYPE.user_type_id";
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
