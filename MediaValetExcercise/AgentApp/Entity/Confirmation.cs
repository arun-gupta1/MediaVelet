using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgentApp.Entity
{
   public class Confirmation : TableEntity
    {
        public Confirmation(int OrderId, int ramdomNum)
        {
            this.PartitionKey = Convert.ToString( OrderId); this.RowKey = Convert.ToString(ramdomNum);
        }
        public Confirmation()
        { }
        public string AgentId { get; set; }
        public string OrderStatus { get; set; }
    }
}
