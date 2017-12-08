using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using Models;
using Task = Models.Task;

namespace LogicLayer
{
    public class CompanyStatisticsLogic : ICompanyStatisticsLogic
    {
        private readonly ICompanyStatisticsRepository companyStatisticsRepository;

        public CompanyStatisticsLogic(ICompanyStatisticsRepository companyStatisticsRepository)
        {
            this.companyStatisticsRepository = companyStatisticsRepository;
        }

        public CompanyStatistics GetTotalIncomeByYear(int year) => companyStatisticsRepository.GetTotalIncomeByYear(year);
        public CompanyStatistics GetTop3Customers() => companyStatisticsRepository.GetTop3Customers();
        public List<Task> GetTop3Tasks(string year) => companyStatisticsRepository.GetTop3Tasks(year);
        public CompanyStatistics GetCustomersWithInvoices() => companyStatisticsRepository.GetCustomersWithInvoices();
    }
}
