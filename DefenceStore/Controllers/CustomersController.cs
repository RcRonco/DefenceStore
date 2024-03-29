﻿using System;
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
    public class CustomersController : Controller
    {
        private DefenceStoreContext db = new DefenceStoreContext();

        // GET: Customers
        public ActionResult Index()
        {
            if (AuthorizationCheck.AdminAuthorized(Session))
            {
                return View(db.Customers.ToList());
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Recommended()
        {
            IEnumerable<Product> rec_products = null;
            if (Session["Customer"] == null)
                rec_products = Helpers.Recommender.getProducts(db, -1, 4);
            else
                rec_products = Helpers.Recommender.getProducts(db, (Session["Customer"] as Customer).ID, 4);

            foreach (Product p in rec_products)
            {
                if (p.Image == null || p.Image == string.Empty)
                    p.Image = "https://www.jainsusa.com/images/store/landscape/not-available.jpg";
            }

            return PartialView(rec_products);
        }

        // GET: Customers/Details/5
        public ActionResult Details(int? id)
        {
            if (!AuthorizationCheck.AdminAuthorized(Session)) return RedirectToAction("Index", "Home");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var customer = db.Customers.Find(id);

            if (customer == null)
            {
                return HttpNotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,FirstName,LastName,Gender,Birthday,Email,Phone,Username,Password,Latitude,Longitude,IsAdmin")] Customer customer)
        {
            if (!ModelState.IsValid) return View(customer);

            var requestedUser = db.Customers.FirstOrDefault(x => x.Username == customer.Username);

            if (requestedUser != null) return View(customer);

            db.Customers.Add(customer);
            db.SaveChanges();

            return RedirectToAction("CustLogin", "Customers");

            return View(customer);
        }

        // GET: Customers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!AuthorizationCheck.AdminAuthorized(Session)) return RedirectToAction("Index", "Home");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var customer = db.Customers.Find(id);

            if (customer == null)
            {
                return HttpNotFound();
            }

            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,FirstName,LastName,Gender,Birthday,Email,Phone,Username,Password,Latitude,Longitude,IsAdmin")] Customer customer)
        {
            if(!AuthorizationCheck.AdminAuthorized(Session)) return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid) return View(customer);

            db.Entry(customer).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Customers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!AuthorizationCheck.AdminAuthorized(Session)) return RedirectToAction("Index", "Home");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var customer = db.Customers.Find(id);

            if (customer == null)
            {
                return HttpNotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!AuthorizationCheck.AdminAuthorized(Session)) return RedirectToAction("Index", "Home");

            var customer = db.Customers.Find(id);

            db.Customers.Remove(customer);
            db.SaveChanges();

            if (((Customer)Session["Customer"]).ID == id)
            {
                Session.Clear();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "Username,Password")] Customer customer)
        {
            var pass = customer.Password;
            var username = customer.Username;

            var requestedClient = db.Customers.SingleOrDefault(u => u.Username.Equals(username) && u.Password.Equals(pass));

            if (requestedClient == null)
            {
                return RedirectToAction("FailedLogin", "Customers");
            }

            Session.Add("Customer", requestedClient);

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            Session.Clear();

            return RedirectToAction("Index", "Home");
        }

        public ActionResult FailedLogin()
        {
            return View();
        }

        public ActionResult CustLogin()
        {
            return View();
        }

        public ActionResult Search(string username)
        {
            var customer = (from m in db.Customers
                            select m).ToList();

            if (!String.IsNullOrEmpty(username))
            {
                customer = customer.Where(m => m.Username.Contains(username)).ToList();
            }

            return View(customer);
        }

        public ActionResult CustomersLocation()
        {
            string markers = "[";

            IEnumerable<Customer> queryResult =
                from cust in db.Customers
                where cust.Latitude != 0 && cust.Longitude != 0
                select cust;

            string fullname = "";

            foreach (var customer in queryResult)
            {
                markers += "{";
                fullname = customer.FirstName + " " + customer.LastName;
                markers += string.Format("'title': '{0}',",fullname);
                markers += string.Format("'lat': '{0}',", customer.Latitude);
                markers += string.Format("'lng': '{0}',", customer.Longitude);
                markers += string.Format("'description': '{0}',", customer.Username);
                markers += "},"; 
            }

            markers += "]";
            ViewBag.Markers = markers;
            return View();
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
