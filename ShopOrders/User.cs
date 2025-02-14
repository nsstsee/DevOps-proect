using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOrders
{
    public class User
    {
        public int ID { get; set; }
        public String Login { get; set; }
        public String Password { get; set; }    
        public bool IsAdmin { get; set; }
    }
}
