using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using Task = Models.Task;

namespace DataLayer
{
    public class SqlCompanyStatisticsContext : DbHandler, ICompanyStatisticsContext
    {
        CompanyStatistics companyStats = new CompanyStatistics();
        List<Customer> customers = new List<Customer>();

        private void StatisticsFromDataRow(DataRow datarow)
        {
            var customer = new Customer(
                    id: Convert.ToInt16(datarow["customerId"]),
                    totalpriceofallinv: Convert.ToDecimal(datarow["TotalAmountByCustomer"])
                );
            customers.Add(customer);
        }


        public CompanyStatistics GetTotalIncomeByYear(int year)
        {
            throw new NotImplementedException();
        }

        public CompanyStatistics GetTop3Customers()
        {
            try
            {
                var dataTable = GetDataByView($"SELECT * FROM vwGetTotalPricePerCustomer");
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        StatisticsFromDataRow(row);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new InvoiceException(
                    $"Neem contact op met de beheerder onder sqldatabase exceptionCode:{sqlEx.Number}");
            }
            catch (Exception ex)
            {
                throw new InvoiceException($"Neem contact op met de beheerder onder exceptionCode:{ex.HResult}");
            }
            
            companyStats.TopCustomers = customers;
            return companyStats;
        }
    }
}
