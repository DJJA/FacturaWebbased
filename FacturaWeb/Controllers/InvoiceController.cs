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

        public ActionResult InvoiceDetails(int id)
        {
            Invoice invoice = invoiceLogic.GetById(id);
            Customer customer = customerLogic.GetById(invoice.Customer.ID);
            Invoice invoiceTasks = invoiceLogic.GetTasksOnInvoice(invoice);

            invoice.Customer = customer;


            return View(invoice);
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




        public void CreateInvoiceForCustomer(string customerInd, string tasks, string dates, string totalPrice, string amountTask, string[] tasky, string[] units)
        {
            List<Task> tasksOnInvoice = new List<Task>();

            List<string> datesList = new List<string>();
            List <string> pricesList = new List<string>();
            List <string> amountsList = new List<string>();
            List<string> tasksList = new List<string>();
            List<string> unitsList = new List<string>(units);

            datesList = invoiceLogic.GetId(dates, datesList);
            pricesList =invoiceLogic.GetId(totalPrice, pricesList);
            amountsList = invoiceLogic.GetId(amountTask, amountsList);
            tasksList = invoiceLogic.GetId(tasks, tasksList);

            if (tasksList.Count == amountsList.Count && tasksList.Count == datesList.Count &&
                tasksList.Count == pricesList.Count)
            {
                for (int i = 0; i < tasksList.Count; i++)
                {
                    Task task = taskLogic.GetTaskById(Convert.ToInt16(tasksList[i]));
                    task.Amount = Convert.ToDecimal(amountsList[i].Replace('.', ','));
                    task.Date = Convert.ToDateTime(datesList[i]);
                    task.Price = Convert.ToDecimal(pricesList[i].Replace('.', ','));
                    task.Unit = (Unit)Enum.Parse(typeof(Unit), unitsList[i].ToString());
                    tasksOnInvoice.Add(task);
                }
            }

            var invoice = new Invoice(
                id: 0,
                customer: customerLogic.GetById(Convert.ToInt16(customerInd)),
                dateSend: DateTime.Now,
                datePayed: DateTime.Now,
                totalPrice: 50,
                tasks: tasksOnInvoice
                );

            invoiceLogic.CreateInvoice(invoice);
        }

    }
}