using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOrders
{
    public class TovarList:DbContext
    {
        public TovarList():base("DbConnection")
        {

        }

        public DbSet<Tovar> Tovars { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
