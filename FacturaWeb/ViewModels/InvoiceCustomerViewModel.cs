using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace FacturaWeb.ViewModels
{
    public class InvoiceCustomerViewModel
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public Customer Customer { get; set; }
        public List<Customer> Customers { get; set; }
        public List<Invoice> InvoicesPerCustomer { get; set; }
    }
}