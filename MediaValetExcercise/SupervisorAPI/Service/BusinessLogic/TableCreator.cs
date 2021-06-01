using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupervisorAPI.Service.BusinessLogic
{
    public class TableCreator
    {

        public static void CreateAzureTables(string azureConnectionString, string tableName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(azureConnectionString);
            CloudTableClient Tableclient = storageAccount.CreateCloudTableClient();
            CloudTable table = Tableclient.GetTableReference(tableName);
            table.CreateIfNotExistsAsync();          

        }
    }
}
