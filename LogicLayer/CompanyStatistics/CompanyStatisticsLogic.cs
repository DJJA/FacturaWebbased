using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using Models;

namespace LogicLayer
{
    public class CompanyStatisticsLogic : ICompanyStatisticsLogic
    {
        private ICompanyStatisticsRepository companyStatisticsRepository;

        public CompanyStatisticsLogic(ICompanyStatisticsRepository companyStatisticsRepository)
        {
            this.companyStatisticsRepository = companyStatisticsRepository;
        }

        public CompanyStatistics GetTotalIncomeByYear(int year)
        {
            throw new NotImplementedException();
        }

        public CompanyStatistics GetTop3Customers()
        {
            return companyStatisticsRepository.GetTop3Customers();
        }
    }
}
