using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DatabaseAnalyzer.Models;

namespace DatabaseAnalyzer.DatabaseActions
{
    public class DataContainerQuery : SQLQuery<DataContainer>
    {
        private readonly string SQLScript = Queries.SQLQueries.Query_DataContainers;

        public override IEnumerable<DataContainer> ExecuteAndReturn()
        {            
            return (IEnumerable<DataContainer>)DatabaseAnalyzer.Main.Database.ExecuteQuery(SQLScript,Creator);
        }

        private object Creator(DataTableCollection Tables)
        {
            List<DataContainer> result = new List<DataContainer>();
            DataTable data = Tables[0];

            List<string> tableNames = data.AsEnumerable().Select(x => x.Field<string>("TABLE_NAME")).Distinct().ToList();
            Dictionary<string, IEnumerable<Column>> columnsForTables = new Dictionary<string, IEnumerable<Column>>();
            
            foreach(var tableName in tableNames)
            {
                List<Column> columns = new List<Column>();

                foreach(var columnData in data.AsEnumerable().Where(x => x.Field<string>("TABLE_NAME").Equals(tableName)))
                {
                    string colName = columnData["COLUMN_NAME"].ToString();
                    bool nullable = (bool)columnData["NULLABLE"];
                    Models.Type colType = new Models.Type(columnData["TYPE_NAME"].ToString(), columnData["PRECISION"].ToString(), columnData["DEFAULT_VALUE"].ToString());

                    columns.Add(new Column(colName, nullable, colType));
                }
                columnsForTables.Add(tableName, columns);
            }           

            
            foreach(var tableDetails in data.AsEnumerable().GroupBy(x => x.Field<string>("TABLE_NAME")).Select(y => y.First()).ToList())
            {
                DataContainer t;
                string name = tableDetails["TABLE_NAME"].ToString();
                Schema sch = new Schema((int)tableDetails["SCHEMA_ID"], tableDetails["SCHEMA_NAME"].ToString());
                IEnumerable<Column> columns = columnsForTables[name];

                if (tableDetails["TABLE_TYPE"].ToString() == "T")
                {
                    t = new Table(name, sch, columns);

                }
                else if (tableDetails["TABLE_TYPE"].ToString() == "V")
                {
                    t = new View(name, sch, columns);
                }
                else
                {
                    throw new Exception($"Unknown table type {tableDetails["TABLE_TYPE"]}!");
                }

                result.Add(t);
            }

            return result;
        }
    }
}
