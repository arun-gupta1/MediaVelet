using AgentApp.Entity;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Text.Json;

namespace AgentApp
{
    class Program
    {
        public static string connstring = "UseDevelopmentStorage=true";
        static void Main(string[] args)
        {
            ProcessQueueMessage();            
        }
        public static void ProcessQueueMessage()
        {
            try
            {
                var agentId = Guid.NewGuid();

                Random rd = new Random();
                int rand_num = rd.Next(1, 10);

                Console.WriteLine(" I’m agent " + agentId + ", my magic number is "+ rand_num );


                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connstring);
                CloudQueueClient cloudQueueClient = storageAccount.CreateCloudQueueClient();
                CloudQueue cloudQueue = cloudQueueClient.GetQueueReference("orderqueue");
                cloudQueue.CreateIfNotExists();

                for (int a = 0; a < 1; a--)
                {
                    CloudQueueMessage retrievedMessage = cloudQueue.GetMessage();
                    if (retrievedMessage != null)
                    {
                        if (!string.IsNullOrEmpty(retrievedMessage.AsString))
                        {
                            Order orderEntity = JsonSerializer.Deserialize<Order>(Base64Decode(retrievedMessage.AsString));
                            Console.WriteLine("Received order: " + orderEntity.OrderId);

                            if (rand_num == orderEntity.RandomNumber)
                            {
                                Console.WriteLine("Oh no, my magic number was found ");
                                break;
                            }
                            Console.WriteLine("Order message: " + orderEntity.OrderText);


                            Microsoft.WindowsAzure.Storage.CloudStorageAccount TablestorageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(connstring);
                            CloudTableClient Tableclient = TablestorageAccount.CreateCloudTableClient();
                            CloudTable table = Tableclient.GetTableReference("confirmation");
                            table.CreateIfNotExistsAsync();

                            Confirmation obj = new Confirmation(orderEntity.OrderId, orderEntity.RandomNumber)
                            {
                                AgentId = agentId.ToString(),
                                OrderStatus = "Processed"

                            };

                            TableOperation insertOperation = TableOperation.Insert(obj);
                            table.ExecuteAsync(insertOperation);
                            cloudQueue.DeleteMessage(retrievedMessage);

                        }
                    }

                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
            }
            

        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
