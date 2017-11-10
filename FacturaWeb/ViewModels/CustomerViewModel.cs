using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace FacturaWeb.ViewModels
{
    public class CustomerViewModel
    {
        public int Id { get; set; }
        public string Name { get;  set; }
        public string Surname { get; set; }
        public Customer Customer { get; set; }
        public List<Customer> Customers { get; set; }
    }
}