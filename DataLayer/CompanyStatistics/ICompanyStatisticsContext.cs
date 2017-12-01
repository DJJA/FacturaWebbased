using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Models;

namespace DataLayer
{
    public interface ICompanyStatisticsContext
    {
        CompanyStatistics GetTotalIncomeByYear(int year);
        CompanyStatistics GetTop3Customers();
        List<Task> GetTop3Tasks(string year);
    }
}
