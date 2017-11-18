using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DefenceStore.DAL;
using DefenceStore.Models;
using DefenceStore.ViewModels;
using System.Collections;

namespace DefenceStore.Controllers
{
    public class OrdersController : Controller
    {
        private DefenceStoreContext db = new DefenceStoreContext();

        // GET: Orders
        public ActionResult Index()
        {
            if (Session["Customer"] == null)
            {
                return View(new List<Order>());
            }

            Customer customer = Session["Customer"] as Customer;
            List<Order> orders;

            if (customer.IsAdmin)
            {
                orders = db.Orders.Include(o => o.Customer).ToList();
            }
            else
            {
                orders = db.Orders.Include(o => o.Customer).Where(o => o.CustomerID == customer.ID).ToList();
            }


            foreach (Order o in orders)
            {
                float res = calculateTotalBill(o);
            }

            db.SaveChanges();
            return View(orders.ToList());
        }

        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        public ActionResult UserNotFound()
        {
            return View();
        }

        public ActionResult EmptyCart()
        {
            return View();
        }

        public ActionResult CreateOrder()
        {
            if (Session["Customer"] == null)
            {
                return Redirect("UserNotFound");
            }
            int? cid = ((Customer)Session["Customer"]).ID;
            List<Product> products = Session["Products"] as List<Product>;

            if (products == null || products.Count < 1)
            {
                return Redirect("EmptyCart");
            }

            Order order = new Order();
            order.CustomerID = cid.Value;
            order.Customer = db.Customers.Find(cid);

            var UserOrders = db.Orders.Where(o => o.CustomerID == cid.Value).ToList<Order>();


            order.Date = DateTime.Now;
            order.BillingType = UserOrders[UserOrders.Count - 1].BillingType;
            order.Address = UserOrders[UserOrders.Count - 1].Address;
            order.products = new List<OrderProduct>();

            foreach (Product prod in products)
            {
                order.products.Add(new OrderProduct
                {
                    Product = prod,
                    ProductID = prod.ID,
                    Order = order,
                    OrderID = order.ID,
                    Quantity = 1
                });
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "ID", "FirstName");
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrder(Order order)
        {
            if (isValid(order))
            {
                if (order.Desciption == null)
                    order.Desciption = "";

                order.Customer = db.Customers.Find(order.CustomerID);

                foreach (OrderProduct p in order.products)
                {
                    var product = db.Products.Find(p.ProductID);
                    p.Product = product;
                    order.TotalBill += p.Product.Price * p.Quantity;
                }
                List<OrderProduct> op = order.products;
                order.products = null;

                if (ModelState.IsValid)
                {
                    db.Orders.Add(order);
                    db.SaveChanges();
                }


                for (int i = 0; i < op.Count; i++)
                {
                    op[i].Order = order;
                    op[i].OrderID = order.ID;
                }

                db.OrderProducts.AddRange(op);

                db.SaveChanges();

                Session.Remove("Products");
                return RedirectToAction("Index");
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "ID", "FirstName");
            return View(order);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(db.Customers, "ID", "FirstName");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,CustomerID,BillingType,Address,Date,Desciption,TotalBill")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "ID", "FirstName", order.CustomerID);
            return View(order);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "ID", "FirstName", order.CustomerID);
            order.products = db.OrderProducts.Where(op => op.OrderID == order.ID).ToList();
            calculateTotalBill(order);
            return View(order);
        }

        private bool isValid(Order order)
        {
            if (order.Address == string.Empty || order.BillingType == string.Empty)
                return false;

            foreach (OrderProduct op in order.products)
            {
                var p = db.Products.Find(op.ProductID);
                if (op.Quantity < 1 || p.Price < 1 /*|| op.Quantity > p.QuantityInStock*/)
                    return false;
            }

            return true;
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Order order)
        {
            if (isValid(order))
            {
                order.Customer = db.Customers.Find(order.CustomerID);

                var ops = db.OrderProducts.Where(op => order.ID == op.OrderID);
                foreach (var cop in order.products)
                {
                    OrderProduct top = ops.Where(op => op.ID == cop.ID).First();
                    top.Quantity = cop.Quantity;
                }

                calculateTotalBill(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "ID", "FirstName", order.CustomerID);

            return View(order);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            calculateTotalBill(order);
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.OrderProducts.RemoveRange(db.OrderProducts.Where(op => op.OrderID == order.ID));
            db.Orders.Remove(order);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Graphs(int? id)
        {
            return View("Graphs");
        }

        protected float calculateTotalBill(Order order)
        {
            var orders = db.Orders.Include(o => o.Customer);
            var order_prd = db.OrderProducts.Include(o => o.Product).Include(o => o.Order);

            /* 
                SELECT dbo.Orders.ID, dbo.Products.Desciption, dbo.OrderProducts.ID
                FROM dbo.Orders 
	                JOIN dbo.OrderProducts ON dbo.Orders.ID = dbo.OrderProducts.OrderID 
	                JOIN dbo.Products ON dbo.OrderProducts.ProductID = dbo.Products.ID 
             */

            var q = (from ords in orders
                     join ord_p in order_prd on ords.ID equals ord_p.OrderID
                     join prd in db.Products on ord_p.ProductID equals prd.ID
                     select new { ords.ID, prd.Price, OPID = ord_p.ID });

            order.TotalBill = 0;
            var q_list = q.Where(q_i => q_i.ID == order.ID).ToList();
            q_list.ForEach(q_i =>
            {
                var x = order_prd.Where(op => op.ID == q_i.OPID).ToList();
                var quant = order_prd.Where(op => op.ID == q_i.OPID).First().Quantity;
                order.TotalBill += q_i.Price * quant;
            });

            return order.TotalBill;
        }

        public ActionResult GetBillingTypesCount()
        {
            var queryResult =
                from order in db.Orders
                select new
                {
                    BillingType = order.BillingType
                };

            var final = from order in queryResult
                        group order by order into g
                        let count = g.Count()
                        select new OrdersBillingTypesViewModel { BillingType = g.Key.BillingType, NumberOfOrders = count };

            var data = Json(final, JsonRequestBehavior.AllowGet);

            return data;
        }

        public ActionResult GetOrdersByDates()
        {
            var queryResult =
                from order in db.Orders
                select new
                {
                    OrderDate = ((DateTime)order.Date).ToString()
                };

            var final = from order in queryResult
                        group order by order into g
                        let count = g.Count()
                        select new OrdersByDateTypesViewModel { OrderDate = g.Key.OrderDate, NumberOfOrders = count };

            var data = Json(final, JsonRequestBehavior.AllowGet);

            return data;
        }

        public string OrdersByGender()
        {
            var genderQuery =
                from order in db.Orders
                join customer in db.Customers on order.CustomerID equals customer.ID 
                select new { customerGender = customer.Gender };
            int total = genderQuery.ToList().Count;
            int male = genderQuery.Count(x => x.customerGender.ToString() == "Male");
            int female = total - male;

            return "There are a total of " + total + " orders, " + male + " of them made by males and " + female + " made by females";
        }
    }
}
