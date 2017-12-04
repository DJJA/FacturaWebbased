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
        private ITaskLogic taskLogic = TaskFactory.ManageTasks();
        private ICustomerLogic customerLogic = CustomerFactory.ManageCustomers();

        // GET: CompanyStatistics
        public ActionResult Statistics()
        {
            var companyStats = companyStatisticsLogic.GetTop3Customers();

            for (int i = 0; i < companyStats.TopCustomers.Count; i++)
            {
                var totalprice = companyStats.TopCustomers[i].TotalPriceOfAllInvoices;
                companyStats.TopCustomers[i] = customerLogic.GetById(Convert.ToInt32(companyStats.TopCustomers[i].ID));
                companyStats.TopCustomers[i].TotalPriceOfAllInvoices = totalprice;
            }

            return View("Statistics", companyStats);
        }

        public ActionResult BestTasks(string year)
        {
            if (year == null)
            {
                year = 2016.ToString();
            }

            var tasks = companyStatisticsLogic.GetTop3Tasks(year);

            for (int i = 0; i < tasks.Count; i++)
            {
                var totalprice = tasks[i].TotalAmountOfAllSimilarTasks;
                tasks[i] = taskLogic.GetTaskById(tasks[i].Id);
                tasks[i].TotalAmountOfAllSimilarTasks = totalprice;
            }

            return View("BestTasks", tasks);

        }

        public ActionResult TotalIncome(string selectedYear)
        {
            if (selectedYear == null)
            {
                selectedYear = 2016.ToString();
            }
            return View("TotalIncome", companyStatisticsLogic.GetTotalIncomeByYear(Convert.ToInt32(selectedYear)));
        }

        public ActionResult CustomersWithInvoice()
        {
            var lijst = customerLogic.GetCustomersWithInvoice();
            return View(lijst);
        }
    }
}