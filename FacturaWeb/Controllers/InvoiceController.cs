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

        public ActionResult Invoice()
        {
            InvoiceCustomerViewModel view = new InvoiceCustomerViewModel()
            {
                InvoicesPerCustomer = invoiceLogic.GetAllInvoices()
            };
            return View(view);
        }

        [HttpPost]
        public ActionResult Pdfje(string id, HttpPostedFileBase postedFile)
        {
            Invoice invoice = invoiceLogic.GetById(Convert.ToInt16(id));
            Customer customer = customerLogic.GetById(invoice.Customer.ID);
            Invoice invoiceTasks = invoiceLogic.GetTasksOnInvoice(invoice);

            invoice.Customer = customer;
            invoice.Tasks = invoiceTasks.Tasks;

            PdfInvoice invoicePdf = new PdfInvoice();

            invoicePdf.ContentType = postedFile.ContentType;




            invoicePdf.Name_File = Path.GetFileName(postedFile.FileName);
            invoicePdf.Extension = Path.GetExtension(invoicePdf.Name_File);
            HttpPostedFileBase file = postedFile;
            byte[] document = new byte[file.ContentLength];
            file.InputStream.Read(document, 0, file.ContentLength);
            invoicePdf.FileData = document;
            invoicePdf.FileSize = document.Length;
            invoicePdf.DisplayName = postedFile.FileName;

            invoiceLogic.InsertInvoiceFile(invoicePdf);


            PdfInvoice pdfie = new PdfInvoice();
            List<PdfInvoice> pdfs = new List<PdfInvoice>();
            pdfs = invoiceLogic.GetInvoiceFile();
            pdfie = pdfs[1];
            return View("GetFiles", pdfie);
            //invoiceLogic.GeneratePdf(invoice);
            //pdf.CreatePdfInvoice();
        }

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
                Response.AddHeader("content-disposition", "attachment;filename=test.pdf");
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.BinaryWrite(creator.CreatePdf(invoice));  //pdfgenerator doc return type
                Response.End();
            }
            catch
            {

            }
        }

        public ActionResult GetFiles()
        {
            return View();
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