using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace DataLayer
{
    public interface ICompanyStatisticsRepository
    {
        CompanyStatistics GetTotalIncomeByYear(int year);
        CompanyStatistics GetTop3Customers();
        List<Task> GetTop3Tasks(string year);
        CompanyStatistics GetCustomersWithInvoices();

    }
}
