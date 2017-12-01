using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using Task = Models.Task;

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
            return companyStatisticsContext.GetTotalIncomeByYear(year);
        }

        public CompanyStatistics GetTop3Customers()
        {
            return companyStatisticsContext.GetTop3Customers();
        }

        public List<Task> GetTop3Tasks(string year)
        {
            return companyStatisticsContext.GetTop3Tasks(year);
        }
    }
}
