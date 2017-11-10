using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Factory;
using FacturaWeb.ViewModels;
using LogicLayer;
using Models;

namespace FacturaWeb.Controllers
{
    public class InvoiceController : Controller
    {
        // GET: Invoice
        private IInvoiceLogic invoiceLogic = InvoiceFactory.ManageInvoices();
        private ITaskLogic taskLogic = TaskFactory.ManageTasks();
        private ICustomerLogic customerLogic = CustomerFactory.ManageCustomers();

        public ActionResult Invoice()
        {
            InvoiceCustomerViewModel view = new InvoiceCustomerViewModel()
            {
                InvoicesPerCustomer = invoiceLogic.GetAllInvoices()
            };
            
            return View(view);
        }

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

        public ActionResult AddedTasks()
        {
            return PartialView("AddedTasks");
        }
    }
}