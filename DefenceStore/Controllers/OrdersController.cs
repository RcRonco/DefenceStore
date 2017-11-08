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

namespace DefenceStore.Controllers
{
    public class OrdersController : Controller
    {
        private DefenceStoreContext db = new DefenceStoreContext();

        // GET: Orders
        public ActionResult Index()
        {
            var orders = db.Orders.Include(o => o.Customer);
            var order_prd = db.OrderProducts.Include(o => o.Product).Include(o => o.Order);

            /* 
                SELECT  SUM(Price*Quantity)
                FROM dbo.Orders 
	                JOIN dbo.OrderProducts ON dbo.Orders.ID = dbo.OrderProducts.OrderID 
	                JOIN dbo.Products ON dbo.OrderProducts.ProductID = dbo.Products.ID 
                WHERE dbo.Orders.ID = 1;
             */
            var q = (from ords in orders
                     join ord_p in order_prd on ords.ID equals ord_p.OrderID
                     join prd in db.Products on ord_p.ProductID equals prd.ID
                     select new { ord_p.ID, prd.Price, OPID = ord_p.ID });
            foreach (Order o in orders)
            {
                o.TotalBill = 0;
                var q_list = q.Where(q_i => q_i.ID == o.ID).ToList();
                q_list.ForEach(q_i => {
                    var x = order_prd.Where(op => op.ID == q_i.OPID).ToList();
                    var quant = order_prd.Where(op => op.ID == q_i.OPID).First().Quantity;
                    o.TotalBill += q_i.Price * quant;
                });
            }


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
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CustomerID,BillingType,Address,Date,Desciption,TotalBill")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
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
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
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
    }
}
