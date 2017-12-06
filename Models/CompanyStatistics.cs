using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class CompanyStatistics
    {
        //TODO: constructor maken en properties private
        public List<Customer> TopCustomers { get; set; }
        public List<Task> TopTasks { get; set; }
        public List<Invoice> CustomerInvoices { get; set; }

        private decimal totalAmount;
        public decimal TotalIncomeByYear
        {
            get { return Convert.ToDecimal(totalAmount.ToString("0.00")); }
            set { totalAmount = value; }
        }

        public CompanyStatistics(List<Customer> topCustomers, List<Task> topTasks, decimal totalAmount)
        {
            TopCustomers = topCustomers;
            TopTasks = topTasks;
            TotalIncomeByYear = totalAmount;
        }

        public CompanyStatistics(List<Invoice> customerInvoices)
        {
            CustomerInvoices = customerInvoices;
        }

        public CompanyStatistics()
        {
            
        }

    }
}
