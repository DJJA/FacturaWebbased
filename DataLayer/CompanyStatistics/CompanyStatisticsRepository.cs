using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace DataLayer
{
    public class CompanyStatisticsRepository : ICompanyStatisticsRepository
    {
        private readonly ICompanyStatisticsContext companyStatisticsContext;
        public CompanyStatisticsRepository(ICompanyStatisticsContext context)
        {
            companyStatisticsContext = context;
        }

        public CompanyStatistics GetTotalIncomeByYear(int year)
        {
            throw new NotImplementedException();
        }

        public CompanyStatistics GetTop3Customers()
        {
            return companyStatisticsContext.GetTop3Customers();
        }
    }
}
