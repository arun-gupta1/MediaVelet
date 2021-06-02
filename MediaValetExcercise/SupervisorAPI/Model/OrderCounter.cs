using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupervisorAPI.Model
{
    public class OrderCounter: TableEntity

    {
        public OrderCounter(string tablename, int tableid)
        {
            this.PartitionKey = tablename; this.RowKey = Convert.ToString(tableid);
        }
        public OrderCounter()
        { }

        public int orderid { get; set; }
    }
}
