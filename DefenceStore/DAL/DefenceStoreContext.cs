using System;
using System.Collections.Generic;
using System.Data.Entity;
using DefenceStore.Models;
using System.Linq;
using System.Web;

namespace DefenceStore.DAL
{
    public class DefenceStoreContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Manufactor> Manufactors { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}