using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using DataLayer;

namespace LogicLayer
{
    public class CustomerLogic : ICustomerLogic
    {
        private ICustomerRepository customerRepository;

        public CustomerLogic(ICustomerRepository customerRepository)
        {
            this.customerRepository = customerRepository;
        }

        public List<Customer> GetAllCustomers()
        {
            return customerRepository.GetAll().ToList();
        }

        public void Insert(Customer customer)
        {
            customerRepository.Add(customer);
        }

        public void Update(Customer customer)
        {
            customerRepository.Update(customer);
        }

        public Customer GetById(int id)
        {
            return customerRepository.GetById(id);
        }

        public List<Customer> GetCustomersByLastName(string lastname)
        {
            return customerRepository.GetCustomersByLastName(lastname).ToList();
        }

        public List<Customer> GetCustomersByZipcode(string zipcode)
        {
            return customerRepository.GetCustomersByZipcode(zipcode).ToList();
        }

        public List<Customer> GetCustomersWithInvoice()
        {
            
            throw new NotImplementedException();
        }
    }
}
