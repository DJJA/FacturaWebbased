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
        public decimal TotalIncomeByYear { get; set; }

    }
}
