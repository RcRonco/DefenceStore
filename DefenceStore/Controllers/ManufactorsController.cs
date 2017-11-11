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
using System.Reflection;

namespace DefenceStore.Controllers
{
    public class ManufactorsController : Controller
    {
        private DefenceStoreContext db = new DefenceStoreContext();

        public object TypeUtils { get; private set; }

        // GET: Manufactors
        public ActionResult Index()
        {
            /*
             -------------------------------------------
             SQL Syntax:
             -------------------------------------------
             SELECT name, count(name) FROM people
             GROUP by name 
             
            -------------------------------------------
            Linq Method 1:
            -------------------------------------------
            var query = from p in context.People
            group p by p.name into g
            select new
            {
              name = g.Key,
              count = g.Count()
            };

            -------------------------------------------
            Linq Method 2:
            -------------------------------------------
            var query = context.People
                   .GroupBy(p => p.name)
                   .Select(g => new { name = g.Key, count = g.Count() });

            

            var productsOfManufactors = (from products in db.Products
                                        group products by products.ManufactorId into manufactor
                                        select new { id = manufactor.Key, count = manufactor.Count() }).ToArray();

            /*
            Type t = productsOfManufactors.GetType();
            //This answer is long overdue for an update for C# 4:
            dynamic d = productsOfManufactors;
            
            ViewBag.productsOfManufactors = productsOfManufactors; 
            
            */

            UpdateTotalProducts();

            return View(db.Manufactors.ToList());
        }

        // GET: Manufactors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Manufactor manufactor = db.Manufactors.Find(id);
            if (manufactor == null)
            {
                return HttpNotFound();
            }
            return View(manufactor);
        }

    // GET: Manufactors/Search/
    public ActionResult Search(string name, string description, int? totalProduct)
    {
        UpdateTotalProducts();

        var manufactors = (from m in db.Manufactors
                          select m).ToList();
  
        if (!String.IsNullOrEmpty(name))
        {
            manufactors = manufactors.Where(m => m.Name.Contains(name)).ToList();
        }

        if (!String.IsNullOrEmpty(description))
        {
            manufactors = manufactors.Where(m => m.Desciption.Contains(description)).ToList();
        }

        if (totalProduct >= 0)
        {
            manufactors = manufactors.Where(m => m.TotalProduct.Equals(totalProduct)).ToList();
        }

        return View(manufactors);
    }

        // GET: Manufactors/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Manufactors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Desciption,Logo")] Manufactor manufactor)
        {
            if (ModelState.IsValid)
            {
                db.Manufactors.Add(manufactor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(manufactor);
        }

        // GET: Manufactors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Manufactor manufactor = db.Manufactors.Find(id);
            if (manufactor == null)
            {
                return HttpNotFound();
            }
            return View(manufactor);
        }

        // POST: Manufactors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Desciption,Logo")] Manufactor manufactor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(manufactor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(manufactor);
        }

        // GET: Manufactors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Manufactor manufactor = db.Manufactors.Find(id);
            if (manufactor == null)
            {
                return HttpNotFound();
            }
            return View(manufactor);
        }

        // POST: Manufactors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Manufactor manufactor = db.Manufactors.Find(id);
            db.Manufactors.Remove(manufactor);
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

        protected void UpdateTotalProducts()
        {
            var manufactors = from man in db.Manufactors
                              select man;

            var productsOfManufactors = (from products in db.Products
                                         group products by products.ManufactorId into manufactor
                                         select new { id = manufactor.Key, count = manufactor.Count() }).ToArray();

           foreach(var pom in productsOfManufactors)
            {
                var pomID = pom.GetType().GetProperty("id").GetValue(pom, null);
                var pomCount = pom.GetType().GetProperty("count").GetValue(pom, null);

                Manufactor manufactor = db.Manufactors.Find(pomID);
                manufactor.TotalProduct = (int)pomCount;
            }

            db.SaveChanges();
        }
    }
}
