﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DefenceStore.DAL;
using DefenceStore.Models;


namespace DefenceStore.Helpers
{
    public class IDNotFoundException : Exception {
        public IDNotFoundException(string model_name, int id) : base($"There is no {model_name} with the id {id}") {}
    }

    public class Recommender
    {
        public static IEnumerable<Product> getProducts(DefenceStoreContext db, int customerID, int topN = 5)
        {
            // Check that the customer exists
            Customer customer = db.Customers.Find(customerID);
            if (customer == null)
                throw new IDNotFoundException("Customer", customerID);

            var orders = db.Orders.Where(o => o.CustomerID == customerID);
            if (orders.Count() > 0)
                return getProductsSpecific(db, customerID, topN);
            else {
                return getProductsGeneral(db, topN);
            }
        }
        protected static IEnumerable<Product> getProductsSpecific(DefenceStoreContext db, int customerID, int topN) {
            // Check that the customer exists
            Customer customer = db.Customers.Find(customerID);
            if (customer == null)
                throw new IDNotFoundException("Customer", customerID);

            // Select all products ordered with the given customer id
            var user_products = db.OrderProducts.Where(op => op.Order.CustomerID == customerID);

            // Count customer product consumption 
            Dictionary<int, int> productCounter = new Dictionary<int, int>();
            user_products.ToList().ForEach(up =>
            {
                if (productCounter.ContainsKey(up.ProductID))
                    productCounter[up.ProductID] += up.Quantity;
                else
                {
                    productCounter.Add(up.ProductID, up.Quantity);
                }
            });

            // Order the dictionary by consumption rate order
            productCounter = productCounter.OrderByDescending(pc => pc.Value).ToDictionary(x => x.Key, x => x.Value);

            // Fetch Top N recommeneded products from DB
            List<Product> rec_prods = new List<Product>();
            int index = 0;
            foreach (int key in productCounter.Keys)
            {
                if (index >= topN)
                    break;
                rec_prods.Add(db.Products.Find(key));
                index++;
            }

            return rec_prods;
        }
        protected static IEnumerable<Product> getProductsGeneral(DefenceStoreContext db, int topN)
        {
            // Select all products ordered with the given customer id
            var user_products = db.OrderProducts;

            // Count customer product consumption 
            Dictionary<int, int> productCounter = new Dictionary<int, int>();
            user_products.ToList().ForEach(up => {
                if (productCounter.ContainsKey(up.ProductID))
                    productCounter[up.ProductID] += up.Quantity;
                else {
                    productCounter.Add(up.ProductID, up.Quantity);
                }
            });

            // Order the dictionary by consumption rate order
            productCounter = productCounter.OrderByDescending(pc => pc.Value).ToDictionary(x => x.Key, x => x.Value);

            // Fetch Top N recommeneded products from DB
            List<Product> rec_prods = new List<Product>();
            int index = 0;
            foreach (int key in productCounter.Keys)
            {
                if (index >= topN)
                    break;
                rec_prods.Add(db.Products.Find(key));
                index++;
            }

            return rec_prods;
        }
    }

    public class GraphGenerator
    {
        public static void generateTotalIncome()
        {
            
        }
    }
}