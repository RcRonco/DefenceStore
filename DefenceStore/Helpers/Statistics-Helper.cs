using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DefenceStore.DAL;
using DefenceStore.Models;
using System.Text;

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
                return getTopSellers(db, topN);

            if (db.Orders.Where(o => o.CustomerID == customerID).Count() > 0)
            {
                return getProductsForUser(db, customer, topN);
            }
            else {
                return getTopSellers(db, topN);
            }
        }

        protected static IEnumerable<Product> getProductsForUser(DefenceStoreContext db, Customer customer, int topN)
        {
            Dictionary<Product, int> alsoBoughtList = new Dictionary<Product, int>();

            // Get the most recent products the user bought
            List<Product> recentProducts = getRecentProducts(db, customer);
            
            // Run on each product in the recent products
            foreach (Product p in recentProducts)
            {
                int index = 0;

                // Get the products customers also bought with this product
                // Add top N products to the Dictionary
                foreach (var pp in alsoBought(db, customer, p))
                {
                    if (index == topN)
                        break;

                    if (alsoBoughtList.ContainsKey(pp.Key))
                        alsoBoughtList[pp.Key] += pp.Value;
                    else
                        alsoBoughtList.Add(pp.Key, pp.Value);

                    index++;
                }
            }

            // Reorder the product dictionary by quantity first
            alsoBoughtList = alsoBoughtList.OrderByDescending(pc => pc.Value).ToDictionary(x => x.Key, x => x.Value);

            if (alsoBoughtList.Count < topN)
            {
                List<Product> result = new List<Product>();

                result.AddRange(alsoBoughtList.Keys);
                foreach (var p in getTopSellers(db, topN))
                {
                    if (!result.Contains(p))
                        result.Add(p);
                }

                return result;
            }

            // Return the most bought product that connected the the recent products of the last order
            return alsoBoughtList.Keys;
        }
        protected static Dictionary<Product,int> alsoBought(DefenceStoreContext db, Customer customer, Product connectedProduct)
        {
            // Get all orders where the product is bought in.
            var orders = db.OrderProducts.Where(op => op.ProductID == connectedProduct.ID).GroupBy(op => op.OrderID);
            Dictionary<Product, int> alsoBoughtList = new Dictionary<Product, int>();

            // Run on each order and fetch the product bought along with the current product
            foreach (var order_id in orders)
            {
                Order current_order = db.Orders.Find(order_id.Key);
                foreach (var prod in current_order.products)
                {
                    if (prod.ProductID == connectedProduct.ID)
                        continue;
                    if (alsoBoughtList.Keys.Contains(prod.Product))
                    {
                        alsoBoughtList[prod.Product] += prod.Quantity;
                    }
                    else
                    {
                        alsoBoughtList.Add(prod.Product, prod.Quantity);
                    }
                }
            }

            // Reorder the product dictionary by quantity first
            alsoBoughtList = alsoBoughtList.OrderByDescending(pc => pc.Value).ToDictionary(x => x.Key, x => x.Value);

            return alsoBoughtList;
        }
        protected static Product topProduct(DefenceStoreContext db, Customer customer)
        {
            // Select all products ordered with the given customer id
            var user_products = db.OrderProducts.Where(op => op.Order.CustomerID == customer.ID);

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
        
            return db.Products.Find(productCounter.Keys.FirstOrDefault());
        }
        protected static List<Product> getTopSellers(DefenceStoreContext db, int topN)
        {
            // Select all products ordered with the given customer id
            var user_products = db.OrderProducts;

            // Count customer product consumption 
            Dictionary<int, int> productCounter = new Dictionary<int, int>();
            user_products.ToList().ForEach(up => {
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
        protected static List<Product> getRecentProducts(DefenceStoreContext db, Customer customer)
        {
            Order lastOrder = db.Orders.Where(o => o.CustomerID == customer.ID).OrderByDescending(o => o.ID).First();
            List<Product> recentProducts = new List<Product>();
            var order_prods = db.OrderProducts.Where(op => op.OrderID == lastOrder.ID);
            foreach (OrderProduct op in order_prods)
            {
                recentProducts.Add(op.Product);
            }

            return recentProducts;
        }
    }

    public class GraphGenerator
    {
        class TimePoint
        {
            public DateTime Date { get; set; }
            public float TotalIncome { get; set; }
        }

        public static string generateTotalIncome(DefenceStoreContext db)
        {
            StringBuilder ss = new StringBuilder();
            var orders = db.Orders;

            List<TimePoint> incomes = new List<TimePoint>();

            foreach (var order in orders)
            {
                var income_day = incomes.Find(inc => inc.Date.Date == order.Date.Date);
                if (income_day == null)
                {
                    incomes.Add( new TimePoint {
                        Date = order.Date,
                        TotalIncome = order.TotalBill
                    });
                }
                else
                {
                    income_day.TotalIncome += order.TotalBill;
                }
            }

            incomes = incomes.OrderBy(inc => inc.Date).ToList();

            ss.AppendLine("Date,Income");
            foreach (TimePoint tp in incomes)
            {
                ss.AppendLine($"{ tp.Date.ToString("dd-MM-yy") }, { tp.TotalIncome }");
            }

            return ss.ToString();
        }

        public static string generateProductConsumption(DefenceStoreContext db)
        {
            StringBuilder ss = new StringBuilder();
            ss.AppendLine("{");
            ss.AppendLine("\t\"label\": \"Products Consumption\",");
            var Products = db.Products;

            foreach(var p in Products)
            {
                var order_products = db.OrderProducts.Where(op => op.ProductID == p.ID);
                int totalConsumption = 0;
                foreach (var op in order_products)
                {
                    totalConsumption += op.Quantity;
                }

                ss.AppendLine($"\t\"{ p.Name }\": { totalConsumption.ToString() },");
            }
            ss.Remove(ss.Length - 3, 2);
            ss.AppendLine("}");

            return ss.ToString();

       }

        public static string generateProductIncome(DefenceStoreContext db)
        {
            Dictionary<Product, float> incomes = new Dictionary<Product, float>();
            foreach (var op in db.OrderProducts)
            {
                if (incomes.ContainsKey(op.Product))
                {
                    incomes[op.Product] += op.Quantity * op.Product.Price;
                }
                else
                {
                    incomes.Add(op.Product, op.Quantity * op.Product.Price);
                }
            }
            StringBuilder ss = new StringBuilder();
            ss.AppendLine("Name,Income");
            foreach(var inc in incomes)
            {
                ss.AppendLine($"{ inc.Key.Name },{ inc.Value.ToString() }");
            }
            
            return ss.ToString();
        }
    }
}