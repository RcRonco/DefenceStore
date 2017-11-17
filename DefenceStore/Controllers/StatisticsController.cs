using DefenceStore.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DefenceStore.Models
{
    public class StatisticsController : Controller
    {
        private DefenceStoreContext db = new DefenceStoreContext();

        // GET: Statistics
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Income()
        {
            return PartialView();
        }

        public ActionResult Consumption()
        {
            return PartialView();
        }

        public ActionResult ProductsIncome()
        {
            return PartialView();
        }

        public ContentResult Data(string data)
        {
            switch (data)
            {
                case "Income":
                    return Content(Helpers.GraphGenerator.generateTotalIncome(db), "application/vnd.ms-excel");
                case "Consumption":
                    return Content(Helpers.GraphGenerator.generateProductConsumption(db), "application/json");
                case "ProductIncome":
                    return Content(Helpers.GraphGenerator.generateProductIncome(db), "application/vnd.ms-excel");
                default:
                    return Content("");
            }

        }
    }
}
