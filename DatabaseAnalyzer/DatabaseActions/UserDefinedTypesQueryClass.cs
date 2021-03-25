using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DatabaseAnalyzer.Models;

namespace DatabaseAnalyzer.DatabaseActions
{
    public class UserDefinedTypesQuery : SQLQuery<Models.Type>
    {
        private readonly string SQLScript = Queries.SQLQueries.Query_UserDefinedTypes;

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
                string baseName = data.Rows[i]["BASE_TYPE_NAME"].ToString();
                string precision = data.Rows[i]["PRECISION"].ToString();
                string defValue = data.Rows[i]["DEFAULT_VALUE"].ToString();

                Models.Type type = new Models.Type(name, baseName, precision, defValue);
                result.Add(type);
            }

            return result;
        }

    }
}
