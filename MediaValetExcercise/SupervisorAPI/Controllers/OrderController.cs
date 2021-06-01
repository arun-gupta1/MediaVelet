using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using SupervisorAPI.Model;
using SupervisorAPI.Service.Contract;

namespace SupervisorAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class OrderController : ControllerBase
    {

        private static int _orderCounter = 0;
        static int generateCounter()
        {
            return _orderCounter++;
        }
       private readonly IOrderQueue _orderQueue;
        private readonly IConfirmationTable _confirmationTable;
        public OrderController(IOrderQueue orderQueue, IConfirmationTable confirmationTable)
        {
            _orderQueue = orderQueue;
            _confirmationTable = confirmationTable;
        }

        [HttpPost]
        [Route("AddMessage")]
        public  async Task<Confirmationmsg> SendMessageToQueue (AzureMessage msg)
        {
           Confirmationmsg obj=null;
            try
            {
                             
                Random rd = new Random();
                int rand_num = rd.Next(1, 10);

                Order orderEntity = new Order
                {
                    OrderId = generateCounter(),
                    RandomNumber = rand_num,
                    OrderText = msg.AzureMessageText

                };
                string jsonString = System.Text.Json.JsonSerializer.Serialize(orderEntity);

                var orderListQueue = _orderQueue.GetQueue("orderqueue");
                orderListQueue.AddMessage(new CloudQueueMessage(Base64Encode(jsonString)));

                CloudTable table = _confirmationTable.GetTable("confirmation");

                TableOperation tableOp = TableOperation.Retrieve<Confirmation>(orderEntity.OrderId.ToString(), orderEntity.RandomNumber.ToString());

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


    }
}