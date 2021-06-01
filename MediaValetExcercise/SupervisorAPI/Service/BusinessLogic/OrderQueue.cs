using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Extensions.Options;
using SupervisorAPI.Infrastructure.AzureStorageSetting;
using SupervisorAPI.Service.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupervisorAPI.Service.BusinessLogic
{
    public class OrderQueue: IOrderQueue
    {
        private readonly CloudQueueClient _queueClient;
        public OrderQueue(IOptions<AzureStorageConnection> settings)
        {
           CloudStorageAccount storageAccount = CloudStorageAccount.Parse(settings.Value.ConnectionString);
            _queueClient = storageAccount.CreateCloudQueueClient();
        }

        public CloudQueue GetQueue(string queueName)
        {
            return _queueClient.GetQueueReference(queueName);
        }
    }
}
