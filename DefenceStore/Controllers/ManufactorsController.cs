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
    public class ManufactorsController : Controller
    {
        private DefenceStoreContext db = new DefenceStoreContext();

        // GET: Manufactors
        public ActionResult Index()
        {
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
    }
}
