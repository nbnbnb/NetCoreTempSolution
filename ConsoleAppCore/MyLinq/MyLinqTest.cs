﻿using Microsoft.Data.SqlClient;
using System;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace ConsoleAppCore.MyLinq
{
    class MyLinqTest
    {
        internal static void Run()
        {
            string constr = @"Data Source=.\SQLExpress;Initial Catalog=Northwind;Integrated Security=True";
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                Northwind db = new Northwind(con);
                string city = "London";

                var query = db.Customers.Where(c => c.City == city)
                    .Select(c => new { Name = c.ContactName, Phone = c.Phone });

                Console.WriteLine("Query:\n{0}\n", query);

                return;

                var list = query.ToList();
                foreach (var item in list)
                {
                    Console.WriteLine("Name: {0}", item);
                }
            }
        }

        class Customers
        {
            public string CustomerID;
            public string ContactName;
            public string Phone;
            public string City;
            public string Country;
        }

        class Orders
        {
            public int OrderID;
            public string CustomerID;
            public DateTime OrderDate;
        }

        class Northwind
        {
            public Query<Customers> Customers;
            public Query<Orders> Orders;

            public Northwind(DbConnection connection)
            {
                // 创建 Provider 对象
                QueryProvider provider = new DbQueryProvider(connection);

                // 创建 Query 对象，关联 Provider
                this.Customers = new Query<Customers>(provider);
                this.Orders = new Query<Orders>(provider);
            }
        }
    }

}
