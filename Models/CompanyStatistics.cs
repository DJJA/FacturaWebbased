using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class CompanyStatistics
    {
        public List<Customer> TopCustomers { get; set; }
        public List<Task> TopTasks { get; set; }

        private decimal totalAmount;
        public decimal TotalIncomeByYear
        {
            get { return Convert.ToDecimal(totalAmount.ToString("0.00")); }
            set { totalAmount = value; }
        }

    }
}
