using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using LogicLayer;
using Models;

namespace Factory
{
    public static class CustomerFactory
    {
        public static ICustomerLogic ManageCustomers()
        {
            return new CustomerLogic(new CustomerRepository(new SqlCustomerContext()));
        }
    }
}
