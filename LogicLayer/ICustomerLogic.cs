using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace LogicLayer
{
    public interface ICustomerLogic
    {
        List<Customer> GetAllCustomers();
        void Insert(Customer customer);
        void Update(Customer customer);
        Customer GetCustomerById(int id);
        List<Customer> GetCustomersByLastName(string lastname);
    }
}
