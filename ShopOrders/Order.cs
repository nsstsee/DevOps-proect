using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOrders
{
    public class Order
    {
        public int ID { get; set; }
        public String Code { get; set; }
        public String Date { get; set; }
        public String Status { get; set; }
        public String Client { get; set; }
        public double Price { get; set; }
        public int TovarIds { get; set; }
        public int RecordID { get; set; }
    }
}
