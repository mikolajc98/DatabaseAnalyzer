using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAnalyzer.Main
{
    public class Report
    {
        public string ReportLocation { get; private set; }
        public bool CreateDictionary { get; private set; }
        public Report(string reportFolderLocation, bool createDictionaryIfNotExists = true)
        {
            this.ReportLocation = reportFolderLocation;
            this.CreateDictionary = createDictionaryIfNotExists;

        }

        public Models.ActionResult GenerateAndSaveReport()
        {   
            if(!System.IO.Directory.Exists(this.ReportLocation) && !this.CreateDictionary)
            {
                return new Models.ActionResult(Models.ActionResultStatus.Warning, $"Directory \"{this.ReportLocation}\" not exists.");
            }
            else
            {
                System.IO.Directory.CreateDirectory(this.ReportLocation);
            }

            if (!Database.IsInitialized)
            {
                return new Models.ActionResult(Models.ActionResultStatus.Error,"Connection to database is not initialized.");
            }

            DatabaseActions.DataContainerQuery dcq = new DatabaseActions.DataContainerQuery();
            DatabaseActions.ExecutableQuery eq = new DatabaseActions.ExecutableQuery();
            DatabaseActions.UserDefinedTypesQuery udt = new DatabaseActions.UserDefinedTypesQuery();
            DatabaseActions.UserDefinedTableTypesQuery udtt = new DatabaseActions.UserDefinedTableTypesQuery();

            List<Models.DataContainer> databaseDataContainers = (List<Models.DataContainer>)dcq.ExecuteAndReturn();
            List<Models.Executable> databaseExecutables = (List<Models.Executable>)eq.ExecuteAndReturn();
            List<Models.Type> userTypes = (List<Models.Type>)udt.ExecuteAndReturn();
            List<Models.Table> userTableTypes = (List<Models.Table>)udtt.ExecuteAndReturn();

            string directory = System.IO.Path.Combine(this.ReportLocation,"Data");
            System.IO.Directory.CreateDirectory(directory);
            foreach(var container in databaseDataContainers)
            {
                string dataContainerFileLocation = System.IO.Path.Combine(directory,$"{container.SchemaDetails.Name}_{container.Name}.txt");
                string fileContents = string.Join("\r\n", container.Columns.Select(x => $"{x.Name} {x.TypeDetails.Name} ({x.TypeDetails.Precision})"));
                System.IO.File.WriteAllText(dataContainerFileLocation,fileContents);                
            }

            directory = System.IO.Path.Combine(this.ReportLocation,"Executables");
            System.IO.Directory.CreateDirectory(directory);
            foreach(var executable in databaseExecutables)
            {
                string executableFileLocation = System.IO.Path.Combine(directory,$"{executable.SchemaDetails.Name}_{executable.Name}.txt");
                string fileContents = string.Join("\r\n", executable.Params.Select(x => $"{x.Name} {x.TypeDetails.Name} ({x.TypeDetails.Precision})"));
                System.IO.File.WriteAllText(executableFileLocation, fileContents);
            }            

            directory = System.IO.Path.Combine(this.ReportLocation, "UserTypes");
            System.IO.Directory.CreateDirectory(directory);            
            {
                string userTypesFileLocation = System.IO.Path.Combine(directory,"UserTypes.txt");
                string fileContents = string.Join("\r\n",userTypes.Select(x => $"{x.Name} ({x.BaseTypeName}) {x.Precision}"));
                System.IO.File.WriteAllText(userTypesFileLocation, fileContents);
            }

            directory = System.IO.Path.Combine(this.ReportLocation, "UserTableTypes");
            System.IO.Directory.CreateDirectory(directory);            
            foreach (var userTableType in userTableTypes)
            {
                string userTableFileLocation = System.IO.Path.Combine(directory, $"{userTableType.SchemaDetails.Name}_{userTableType.Name}.txt");
                string fileContents = string.Join("\r\n", userTableType.Columns.Select(x => $"{x.Name} {x.TypeDetails.Name} ({x.TypeDetails.Precision})"));
                System.IO.File.WriteAllText(userTableFileLocation, fileContents);
            }

            return new Models.ActionResult(Models.ActionResultStatus.Success,"");
        }
    }
}
