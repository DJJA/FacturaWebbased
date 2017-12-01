using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Factory;
using LogicLayer;

namespace FacturaWeb.Controllers
{
    public class CompanyStatisticsController : Controller
    {
        private ICompanyStatisticsLogic companyStatisticsLogic = CompanyStatisticsFactory.ManageCompanyStatistics();

        private ICustomerLogic customerLogic = CustomerFactory.ManageCustomers();

        // GET: CompanyStatistics
        public ActionResult Statistics(string year, string selectedYear)
        {
            var companyStats = companyStatisticsLogic.GetTop3Customers();

            for (int i = 0; i < companyStats.TopCustomers.Count; i++)
            {
                var totalprice = companyStats.TopCustomers[i].TotalPriceOfAllInvoices;
                companyStats.TopCustomers[i] = customerLogic.GetById(Convert.ToInt32(companyStats.TopCustomers[i].ID));
                companyStats.TopCustomers[i].TotalPriceOfAllInvoices = totalprice;
            }


            if (year == null & selectedYear == null)
            {
                year = 2016.ToString();
                selectedYear = 2016.ToString();
            }
            companyStats.TotalIncomeByYear = companyStatisticsLogic.GetTotalIncomeByYear(Convert.ToInt32(selectedYear)).TotalIncomeByYear;
            companyStats.TopTasks = companyStatisticsLogic.GetTop3Tasks(year);

            return View("Statistics", companyStats);
        }

        public ActionResult BestTasks(string year)
        {

            var tasks = companyStatisticsLogic.GetTop3Tasks(year);

            return View("BestTasks", tasks);

        }

        public ActionResult TotalIncome(string selectedYear)
        {

            return View("TotalIncome", companyStatisticsLogic.GetTotalIncomeByYear(Convert.ToInt32(selectedYear)));
        }
    }
}