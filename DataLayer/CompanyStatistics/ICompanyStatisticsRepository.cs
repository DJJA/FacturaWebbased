using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public interface ICompanyStatisticsRepository
    {
        CompanyStatistics GetTotalIncomeByYear(int year);
        CompanyStatistics GetTop3Customers();
    }
}
