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
    public class OrderProductsController : Controller
    {
        private DefenceStoreContext db = new DefenceStoreContext();

        // GET: OrderProducts
        public ActionResult Index()
        {
            var orderProducts = db.OrderProducts.Include(o => o.Order).Include(o => o.Product);
            return View(orderProducts.ToList());
        }

        // GET: OrderProducts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderProduct orderProduct = db.OrderProducts.Find(id);
            if (orderProduct == null)
            {
                return HttpNotFound();
            }
            return View(orderProduct);
        }

        // GET: OrderProducts/Details/5
        public ActionResult Search(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var orderProduct = db.OrderProducts.Where( op => op.OrderID == id).Include(op => op.Product).Include(op => op.Order).AsEnumerable();
            
            if (orderProduct == null || orderProduct.Count() < 1)
            {
                return HttpNotFound();
            }
            return PartialView(orderProduct);
        }

        // GET: OrderProducts/Details/5
        public ActionResult SearchEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var orderProduct = db.OrderProducts.Where(op => op.OrderID == id).Include(op => op.Product).Include(op => op.Order).AsEnumerable();

            if (orderProduct == null || orderProduct.Count() < 1)
            {
                return HttpNotFound();
            }
            return PartialView(orderProduct);
        }

        // GET: OrderProducts/Create
        public ActionResult Create()
        {
            ViewBag.OrderID = new SelectList(db.Orders, "ID", "BillingType");
            ViewBag.ProductID = new SelectList(db.Products, "ID", "Name");
            return View();
        }

        // POST: OrderProducts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,OrderID,ProductID")] OrderProduct orderProduct)
        {
            if (ModelState.IsValid)
            {
                db.OrderProducts.Add(orderProduct);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OrderID = new SelectList(db.Orders, "ID", "BillingType", orderProduct.OrderID);
            ViewBag.ProductID = new SelectList(db.Products, "ID", "Name", orderProduct.ProductID);
            return View(orderProduct);
        }

        // GET: OrderProducts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderProduct orderProduct = db.OrderProducts.Find(id);
            if (orderProduct == null)
            {
                return HttpNotFound();
            }
            ViewBag.OrderID = new SelectList(db.Orders, "ID", "BillingType", orderProduct.OrderID);
            ViewBag.ProductID = new SelectList(db.Products, "ID", "Name", orderProduct.ProductID);
            return View(orderProduct);
        }

        // POST: OrderProducts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,OrderID,ProductID")] OrderProduct orderProduct)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orderProduct).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OrderID = new SelectList(db.Orders, "ID", "BillingType", orderProduct.OrderID);
            ViewBag.ProductID = new SelectList(db.Products, "ID", "Name", orderProduct.ProductID);
            return View(orderProduct);
        }

        // GET: OrderProducts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderProduct orderProduct = db.OrderProducts.Find(id);
            if (orderProduct == null)
            {
                return HttpNotFound();
            }

            return View(orderProduct);
        }

        // POST: OrderProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OrderProduct orderProduct = db.OrderProducts.Find(id);
            orderProduct.Order.TotalBill -= orderProduct.Quantity * orderProduct.Product.Price;
            db.OrderProducts.Remove(orderProduct);
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
