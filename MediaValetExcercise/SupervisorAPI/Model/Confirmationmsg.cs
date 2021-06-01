using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupervisorAPI.Model
{
    public class Confirmationmsg
    {
        public string OrderID { get; set; }
        public string AgentId { get; set; }
        public string OrderStatus { get; set; }
    }
}
