using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

using SeidoDemoDb;
using SeidoDemoModels;

namespace SeidoApplication
{
    class Program
    {
        private static DbContextOptionsBuilder<SeidoDemoDbContext> _optionsBuilder;
        static void Main(string[] args)
        {
            BuildOptions();

            #region Uncomment to seed and query the Database
            SeedDataBase();
            QueryDatabaseAsync().Wait();
            QueryDatabase_Linq();
            QueryDatabase_DatamodelLinq();
            #endregion
        }

        private static void BuildOptions()
        {
            _optionsBuilder = new DbContextOptionsBuilder<SeidoDemoDbContext>();

            #region Ensuring appsettings.json is in the right location
            Console.WriteLine($"DbConnections Directory: {DBConnection.DbConnectionsDirectory}");

            var connectionString = DBConnection.ConfigurationRoot.GetConnectionString("SQLServer_seidodemo");
            if (!string.IsNullOrEmpty(connectionString))
                Console.WriteLine($"Connection string to Database: {connectionString}");
            else
            {
                Console.WriteLine($"Please copy the 'DbConnections.json' to this location");
                return;
            }
            #endregion

            _optionsBuilder.UseSqlServer(connectionString);

        }

        #region Uncomment to seed and query the Database
        private static void SeedDataBase()
        {
            using (var db = new SeidoDemoDbContext(_optionsBuilder.Options))
            {
                //Create some customers
                for (int i = 0; i < 10; i++)
                {
                    var cus = new Customer();

                    //Create some random orders linked to customers
                    var rnd = new Random();
                    for (int o = 0; o < rnd.Next(0,6); o++)
                    {
                        var order = new Order();
                        order.Customer = cus;
                        cus.Orders.Add(order);
                    }

                    db.Customers.Add(cus);
                }

                db.SaveChanges();
            }
        }
        private static async Task QueryDatabaseAsync()
        {
            using (var db = new SeidoDemoDbContext(_optionsBuilder.Options))
            {
                var customers = await db.Customers.CountAsync();
                var orders = await db.Orders.CountAsync();

                Console.WriteLine($"Nr of Customers: {customers}");
                Console.WriteLine($"Nr of Orders: {orders}");
            }
        }
        private static void QueryDatabase_Linq()
        {
            Console.WriteLine("\nQuery Database using Linq");
            using (var db = new SeidoDemoDbContext(_optionsBuilder.Options))
            {
                var customers = db.Customers.AsEnumerable().ToList();
                var orders = db.Orders.AsEnumerable().ToList();

                Console.WriteLine("\n\nQuery Database with Linq");
                Console.WriteLine("------------------------");
                Console.WriteLine($"Nr of customers: {customers.Count()}");
                Console.WriteLine($"Nr of orders: {orders.Count()}");

                var list = customers.GroupJoin(orders, cust => cust.CustomerID, order => order.CustomerID, (cust, orderList) => new { cust, orderList });
                Console.WriteLine($"\nGroupJoin Customer - Order");
                foreach (var orderGroup in list)
                {
                    Console.WriteLine($"Orders for customer {orderGroup.cust.CustomerID}:");
                    foreach (var order in orderGroup.orderList)
                    {
                        Console.WriteLine($"   OrderNr: {order.OrderID}");
                    }
                }
            }
        }
        private static void QueryDatabase_DatamodelLinq()
        {
            Console.WriteLine("\nQuery Database using Datamodel and Linq");
            using (var db = new SeidoDemoDbContext(_optionsBuilder.Options))
            {
                var customers = db.Customers.AsEnumerable().ToList();
                var orders = db.Orders.AsEnumerable().ToList();

                foreach (var customer in customers)
                {
                    Console.WriteLine($"Orders for customer {customer.CustomerID}:");
                    foreach (var order in customer.Orders)
                    {
                        Console.WriteLine($"   OrderNr: {order.OrderID}");
                    }
                }
            }
        }

        #endregion
    }
}
