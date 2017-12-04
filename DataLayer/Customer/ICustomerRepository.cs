using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace DataLayer
{
    public interface ICustomerRepository : IRepository<Customer> 
    {
        IEnumerable<Customer> GetCustomersByLastName(string lastname);
        IEnumerable<Customer> GetCustomersByZipcode(string zipcode);
    }
}
