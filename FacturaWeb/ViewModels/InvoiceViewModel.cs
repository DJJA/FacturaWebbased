using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace FacturaWeb.ViewModels
{
    public class InvoiceViewModel
    {
        public List<Task> Tasks { get; set; }
        public List<Invoice> Invoices { get; set; }
        public List<Customer> Customers { get; set; }
    }
}