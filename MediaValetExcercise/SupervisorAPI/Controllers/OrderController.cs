using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using SupervisorAPI.Model;
using SupervisorAPI.Service.Contract;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SupervisorAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class OrderController : ControllerBase
    {

        private  int _orderCounter = 0;
       
       private readonly IOrderQueue _orderQueue;
        private readonly IConfirmationTable _confirmationTable;
        public OrderController(IOrderQueue orderQueue, IConfirmationTable confirmationTable)
        {
            _orderQueue = orderQueue;
            _confirmationTable = confirmationTable;
        }

        [HttpPost]
        [Route("AddMessage")]
        public  async Task<Confirmationmsg> SendMessageToQueue (OrderMessage msg)
        {
           Confirmationmsg obj=null;
            try
            {
                _orderCounter = GenerateOrderId();

                Random rd = new Random();
                int rand_num = rd.Next(1, 10);

                Order orderEntity = new Order
                {
                    OrderId = _orderCounter,             //generateCounter(),
                    RandomNumber = rand_num,
                    OrderText = msg.OrderText

                };
                string jsonString = System.Text.Json.JsonSerializer.Serialize(orderEntity);

                var orderListQueue = _orderQueue.GetQueue("orderqueue");
                orderListQueue.AddMessage(new CloudQueueMessage(Base64Encode(jsonString)));

                CloudTable table = _confirmationTable.GetTable("confirmation");

                TableOperation tableOp = TableOperation.Retrieve<Confirmation>(orderEntity.OrderId.ToString(), orderEntity.RandomNumber.ToString());
                System.Threading.Thread.Sleep(2000);

                TableResult tr = await table.ExecuteAsync(tableOp);
                Confirmation data = tr.Result as Confirmation;
                
                if (!string.IsNullOrEmpty(data.OrderStatus))
                {
                     obj = new Confirmationmsg
                    {
                        OrderID = data.PartitionKey,
                        AgentId = data.AgentId,
                        OrderStatus = data.OrderStatus
                    };
                }     
            }
            
            catch (Exception ex)
            {
                throw ex;
            }
            return obj;

        }
        [NonAction]
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        [NonAction]
        public  int GenerateOrderId()
        {
            int orderid=0;
            CloudTable table = _confirmationTable.GetTable("ordercount");

            TableContinuationToken token = null;
            var entities = new List<OrderCounter>();
            do
            {
                var queryResult = table.ExecuteQuerySegmentedAsync(new TableQuery<OrderCounter>(), token);
                entities.AddRange(queryResult.Result);                
            } while (token != null);
            

            if (entities.Count > 0)
            {
                var data = entities[0] as OrderCounter;
                orderid = Convert.ToInt32(data.orderid) + 1;
                data.orderid =orderid;

                TableOperation updateOperation = TableOperation.Replace(data);
                table.ExecuteAsync(updateOperation);
            }
            else
            {
                OrderCounter obj = new OrderCounter("Order", 1);
                obj.orderid = orderid;
                TableOperation insertOperation = TableOperation.Insert(obj);
                table.ExecuteAsync(insertOperation);
            }

            return orderid;


        }


    }
}