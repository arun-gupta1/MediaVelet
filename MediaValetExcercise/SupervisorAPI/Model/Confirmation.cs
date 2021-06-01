﻿using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupervisorAPI.Model
{
    public class Confirmation: TableEntity
    {

        public Confirmation(int OrderId, int ramdomNum)
        {
            this.PartitionKey = Convert.ToString(OrderId); this.RowKey = Convert.ToString(ramdomNum);
        }
        public Confirmation()
        { }
        public string AgentId { get; set; }
        public string OrderStatus { get; set; }
    }
}
