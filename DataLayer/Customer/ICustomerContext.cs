using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace DataLayer
{
    public interface ICustomerContext : IContext<Customer>
    {
        IEnumerable<Customer> GetCustomersByZipcode(string zipcode);
        IEnumerable<Customer> GetCustomersByLastName(string lastname);
    }
}
