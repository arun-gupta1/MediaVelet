using Microsoft.Azure.Storage;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage.Table;
using SupervisorAPI.Infrastructure.AzureStorageSetting;
using SupervisorAPI.Service.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupervisorAPI.Service.BusinessLogic
{
    public class ConfirmationTable: IConfirmationTable
    {
        private readonly CloudTableClient _tableClient;
        public ConfirmationTable(IOptions<AzureStorageConnection> settings)
        {
            Microsoft.WindowsAzure.Storage.CloudStorageAccount storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(settings.Value.ConnectionString);
            _tableClient = storageAccount.CreateCloudTableClient();
        }

        public CloudTable GetTable(string tableName)
        {
            return _tableClient.GetTableReference(tableName);
        }
    }
}
