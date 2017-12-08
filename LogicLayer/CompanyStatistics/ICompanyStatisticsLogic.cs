using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Models;

namespace LogicLayer
{
    public interface ICompanyStatisticsLogic
    {
        CompanyStatistics GetTotalIncomeByYear(int year);
        CompanyStatistics GetTop3Customers(); //TODO: evt. list maken
        List<Task> GetTop3Tasks(string year); //TODO: evt. company object maken
        CompanyStatistics GetCustomersWithInvoices();
    }
}
