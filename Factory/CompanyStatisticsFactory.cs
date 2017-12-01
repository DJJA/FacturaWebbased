using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using LogicLayer;

namespace Factory
{
    public class CompanyStatisticsFactory
    {
        public static ICompanyStatisticsLogic ManageCompanyStatistics()
        {
            return new CompanyStatisticsLogic(new CompanyStatisticsRepository(new SqlCompanyStatisticsContext()));
        }
    }
}
