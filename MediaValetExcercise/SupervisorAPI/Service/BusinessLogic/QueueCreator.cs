using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupervisorAPI.Service.BusinessLogic
{
    public class QueueCreator
    {

        public static void CreateAzureQueues(string azureConnectionString, string queueName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(azureConnectionString);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            queueClient.GetQueueReference(queueName).CreateIfNotExists();
            
        }
    }
}
