using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DatabaseAnalyzer.Models;

namespace DatabaseAnalyzer.SQL_Queries
{
    public class UserDefinedTypesQuery : SQLQuery<Models.Type>
    {
        private readonly string SQLScript = "SELECT  USRTPS.name AS 'TYPE_NAME', SCHMS.schema_id AS 'SCHEMA_ID', SCHMS.name AS 'SCHEMA_NAME', SYSTPS.name AS 'BASE_TYPE_NAME', CASE  WHEN USRTPS.precision = 0 AND USRTPS.scale = 0 THEN CAST(USRTPS.max_length AS nvarchar(100))  ELSE CONCAT(USRTPS.PRECISION,', ',USRTPS.scale) END AS 'PRECISION', USRTPS.is_nullable AS 'NULLABLE',  CASE  WHEN USRTPS.default_object_id <> 0 THEN SUBSTRING(DEFVAL.definition, 3, LEN(DEFVAL.definition)-4)  ELSE '' END AS 'DEFAULT_VALUE'  FROM sys.types USRTPS JOIN sys.schemas SCHMS ON USRTPS.schema_id = SCHMS.schema_id JOIN sys.types SYSTPS ON USRTPS.system_type_id = SYSTPS.system_type_id AND USRTPS.system_type_id = SYSTPS.user_type_id LEFT JOIN sys.default_constraints DEFVAL ON USRTPS.default_object_id = DEFVAL.object_id WHERE USRTPS.is_table_type = 0 AND USRTPS.is_user_defined = 1";

        public override IEnumerable<Models.Type> ExecuteAndReturn()
        {
            return (IEnumerable<Models.Type>)DatabaseAnalyzer.Main.Database.ExecuteQuery(SQLScript, Creator);
        }

        private object Creator(DataTableCollection tables)
        {
            List<Models.Type> result = new List<Models.Type>();

            DataTable data = tables[0];

            for(int i = 0; i < data.Rows.Count; i++)
            {
                string name = data.Rows[i]["TYPE_NAME"].ToString();
                string precision = data.Rows[i]["PRECISION"].ToString();
                string defValue = data.Rows[i]["DEFAULT_VALUE"].ToString();

                Models.Type type = new Models.Type(name, precision, defValue);
                result.Add(type);
            }

            return result;
        }

    }
}
