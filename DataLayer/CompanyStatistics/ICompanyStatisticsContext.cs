using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace DataLayer
{
    public interface ICompanyStatisticsContext
    {
        CompanyStatistics GetTotalIncomeByYear(int year);
        CompanyStatistics GetTop3Customers();
    }
}
