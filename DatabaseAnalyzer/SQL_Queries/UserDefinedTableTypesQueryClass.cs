using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DatabaseAnalyzer.Models;

namespace DatabaseAnalyzer.SQL_Queries
{
    public class UserDefinedTableTypesQuery : SQLQuery<Table>
    {
        private readonly string SQLScript = "SELECT	TABLEVIEW.name AS 'TABLE_NAME',	TABLEVIEW.schema_id 'SCHEMA_ID',	SCHMS.name AS 'SCHEMA_NAME',	COLS.name AS 'COLUMN_NAME',		CASE 	 WHEN COLS.system_type_id = COLS.user_type_id THEN SYSTYPE.name	 ELSE USRTYPE.name 	END AS 'TYPE_NAME',	CASE	 WHEN COLS.precision = 0 AND COLS.scale = 0 THEN CAST(COLS.max_length AS nvarchar(100))	 ELSE CONCAT(COLS.PRECISION,', ',COLS.scale)	END AS 'PRECISION',		COLS.is_nullable AS 'NULLABLE',	CASE	 WHEN COLS.default_object_id <> 0 THEN SUBSTRING(DEFVALUE.definition, 3, LEN(DEFVALUE.definition)-4)	 ELSE ''	END AS 'DEFAULT_VALUE'			 FROM sys.all_columns COLS		JOIN sys.table_types TABLEVIEW ON TABLEVIEW.type_table_object_id = COLS.object_id	JOIN sys.schemas SCHMS ON TABLEVIEW.schema_id = SCHMS.schema_id		JOIN sys.types SYSTYPE ON COLS.system_type_id = SYSTYPE.system_type_id AND COLS.system_type_id = SYSTYPE.user_type_id	LEFT JOIN sys.types USRTYPE ON COLS.system_type_id = USRTYPE.system_type_id AND COLS.user_type_id = USRTYPE.user_type_id	LEFT JOIN sys.default_constraints DEFVALUE ON COLS.default_object_id = DEFVALUE.object_id";

                
        public override IEnumerable<Table> ExecuteAndReturn()
        {
            return (IEnumerable<Table>)DatabaseAnalyzer.Main.Database.ExecuteQuery(SQLScript, Creator);
        }

        private object Creator(DataTableCollection tables)
        {
            List<Table> result = new List<Table>();
            DataTable data = tables[0];

            List<string> tableNames = data.AsEnumerable().Select(x => x.Field<string>("TABLE_NAME")).Distinct().ToList();
            Dictionary<string, IEnumerable<Column>> columnsForTables = new Dictionary<string, IEnumerable<Column>>();

            foreach (var tableName in tableNames)
            {
                List<Column> columns = new List<Column>();

                foreach (var columnData in data.AsEnumerable().Where(x => x.Field<string>("TABLE_NAME").Equals(tableName)))
                {
                    string colName = columnData["COLUMN_NAME"].ToString();
                    bool nullable = (bool)columnData["NULLABLE"];
                    Models.Type colType = new Models.Type(columnData["TYPE_NAME"].ToString(), columnData["PRECISION"].ToString(), columnData["DEFAULT_VALUE"].ToString());

                    columns.Add(new Column(colName, nullable, colType));
                }
                columnsForTables.Add(tableName, columns);
            }


            foreach (var tableDetails in data.AsEnumerable().GroupBy(x => x.Field<string>("TABLE_NAME")).Select(y => y.First()).ToList())
            {
                Table t;
                string name = tableDetails["TABLE_NAME"].ToString();
                Schema sch = new Schema((int)tableDetails["SCHEMA_ID"], tableDetails["SCHEMA_NAME"].ToString());
                IEnumerable<Column> columns = columnsForTables[name];

                t = new Table(name, sch, columns);
                result.Add(t);
            }

            return result;
        }
    }
}
