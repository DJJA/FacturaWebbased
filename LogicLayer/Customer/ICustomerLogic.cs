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
        Customer GetById(int id);
        List<Customer> GetCustomersByLastName(string lastname);
        List<Customer> GetCustomersByZipcode(string zipcode);
        List<Customer> GetCustomersWithInvoice();
    }
}
