using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Factory;
using FacturaWeb.ViewModels;
using LogicLayer;
using Models;
using System.IO;
using PdfInvoiceCreator;


namespace FacturaWeb.Controllers
{
    public class InvoiceController : Controller
    {
        private IInvoiceLogic invoiceLogic = InvoiceFactory.ManageInvoices();
        private ITaskLogic taskLogic = TaskFactory.ManageTasks();
        private ICustomerLogic customerLogic = CustomerFactory.ManageCustomers();
        private ICompanyStatisticsLogic companyStatisticsLogic = CompanyStatisticsFactory.ManageCompanyStatistics();

        public ActionResult Invoice()
        {
            InvoiceCustomerViewModel view = new InvoiceCustomerViewModel()
            {
                InvoicesPerCustomer = invoiceLogic.GetAllInvoices()
            };
            return View(view);
        }
        //TODO: test weghalen
        public void test(string id)
        {
            InvoicePdfCreator creator = new InvoicePdfCreator();
            Invoice invoice = invoiceLogic.GetById(Convert.ToInt32(id));
            Customer customer = customerLogic.GetById(invoice.Customer.ID);
            Invoice invoiceTasks = invoiceLogic.GetTasksOnInvoice(invoice);

            invoice.Customer = customer;
            invoice.Tasks = invoiceTasks.Tasks;

            try
            {
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", $"attachment;filename={invoice.Id}_{invoice.Customer.LastName}.pdf");
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.BinaryWrite(creator.CreatePdf(invoice));  //pdfgenerator doc return type
                Response.End();
            }
            catch
            {

            }
        }

        public ActionResult InvoicePayed(int id)
        {
            Invoice invoice = invoiceLogic.GetById(id); 

            invoiceLogic.InvoicePayed(invoice);

            invoice = invoiceLogic.GetById(id);
            Customer customer = customerLogic.GetById(invoice.Customer.ID);
            Invoice invoiceTasks = invoiceLogic.GetTasksOnInvoice(invoice);

            //TODO: invoice.customer private maken

            invoice.Customer = customer;
            invoice.Tasks = invoiceTasks.Tasks;

            return View("Invoice");
        }

        public ActionResult Confirmation(int id)
        {
            Invoice invoice = invoiceLogic.GetById(id);

            return View("Confirmation", invoice);
        }
      
        public ActionResult InvoicesPerCustomer(int id)
        {
            invoiceLogic.GetInvoicesPercustomer(id);
            return View();
        }
        public ActionResult InvoiceDetails(int id)
        {
            Invoice invoice = invoiceLogic.GetById(id);
            Customer customer = customerLogic.GetById(invoice.Customer.ID);
            Invoice invoiceTasks = invoiceLogic.GetTasksOnInvoice(invoice);

            invoice.Customer = customer;
            invoice.Tasks = invoiceTasks.Tasks;

            return View(invoice);
        }

        //public ActionResult Stats(string selectedYear)
        //{
        //    if (selectedYear == null)
        //    {
        //        selectedYear = 2016.ToString();
        //    }
        //    //invoiceLogic.GetTotalInvoiceAmountByYear(2017);
        //    Invoice invoice = new Invoice();

        //    var income = companyStatisticsLogic.GetTotalIncomeByYear(Convert.ToInt32(selectedYear));

        //    return View("Stats", income);
        //}

        //public ActionResult CalculateStats(string selectedYear)
        //{

        //   return View("CalculateStats", invoiceLogic.GetTotalInvoiceAmountByYear(Convert.ToInt32(selectedYear)))  ;
        //}

        public void Add(ICollection<string> taskId)
        {
            List<Task> tasks = new List<Task>();
            foreach (var task in taskId)
            {
                var taskid = Convert.ToInt16(task);
                tasks.Add(taskLogic.GetTaskById(taskid));
            }

        }

        public ActionResult CreateInvoice()
        {
            InvoiceViewModel viewModel = new InvoiceViewModel()
            {
                Tasks = taskLogic.GetAllTasks(),
                Customers = customerLogic.GetAllCustomers()
            };

            return View(viewModel);
        }

        public ActionResult AllCustomersView()
        {
            var view = new InvoiceViewModel()
            {
                Customers = customerLogic.GetAllCustomers()
            };

            return PartialView("AllCustomersView", view);
        }

        public ActionResult SwitchCustomerPartial(string hiddenCheck)
        {
            InvoiceViewModel viewModel = new InvoiceViewModel()
            {
                Customers = customerLogic.GetAllCustomers()
            };

            if (hiddenCheck == "back")
            {
                return PartialView("AllCustomersView", viewModel);
            }
            else
            {
                return PartialView("CustomerDetails");
            }
        }

        public ActionResult CustomerDetails(string customerId)
        {
            var id = Convert.ToInt16(customerId);
            var viewModel = new InvoiceViewModel()
            {
                Customer = customerLogic.GetById(id),
                Customers = customerLogic.GetAllCustomers()
            };

            return PartialView("CustomerDetails", viewModel);
        }

        public ActionResult AddedTasks()
        {
            return PartialView("AddedTasks");
        }

        private List<string> ids = new List<string>();
        public ActionResult CreateInvoiceForCustomer(string customerInd, string[] dates, string[] totalPrices, string[] amounts, string[] tasks, string[] units)
        {
            List<Task> tasksOnInvoice = new List<Task>();

            List<string> datesList = new List<string>(dates);
            List<string> pricesList = new List<string>(totalPrices);
            List<string> amountsList = new List<string>(amounts);
            List<string> tasksList = new List<string>(tasks);
            List<string> unitsList = new List<string>(units);

            if (tasksList.Count == amountsList.Count && tasksList.Count == datesList.Count &&
                tasksList.Count == pricesList.Count)
            {
                for (int i = 0; i < tasksList.Count; i++)
                {
                    Task task = taskLogic.GetTaskById(Convert.ToInt16(tasksList[i]));
                    task.Amount = Convert.ToDecimal(amountsList[i]);
                    task.Date = Convert.ToDateTime(datesList[i]);
                    task.Price = Convert.ToDecimal(pricesList[i]); //TODO: localhost, moet punt met komma vervangenworden. azure niet.
                    task.Unit = (Unit)Enum.Parse(typeof(Unit), unitsList[i].ToString());
                    tasksOnInvoice.Add(task);
                }
            }

            var invoice = new Invoice(
                id: 0,
                customer: customerLogic.GetById(Convert.ToInt16(customerInd)),
                dateSend: DateTime.Now,
                datePayed: default(DateTime),
                tasks: tasksOnInvoice
                );

            invoiceLogic.CreateInvoice(invoice);

            return View("Invoice", "Invoice");
        }

    }
}