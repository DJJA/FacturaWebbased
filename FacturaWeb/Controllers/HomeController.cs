using System.Web.Mvc;
using Factory;

namespace FacturaWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult btnSubmit()
        {
            var good = "ok";

            return View(good);
        }

        public ActionResult Customer()
        {
            ViewBag.Message = "Overzicht van alle klanten";

            var customerLogic = CustomerFactory.ManageCustomers();

            var customers = customerLogic.GetAllCustomers();


            return View(customers);
        }

        public ActionResult Invoice()
        {
            ViewBag.Message = "Facturen van: ";


            return View();
        }
    }
}