using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;

namespace TestCustomer
{
    [TestClass]
    public class CustomerDatabaseTest
    {
        [TestMethod]
        public void Test_InsertionAndGetByLastname()
        {
            //Arrange
            bool check = true;
            Address address = new Address("Korenakker 17", "Someren-Eind", "5712PJ");
            Customer customer = new Customer(0, "Daphne", "Pouwels", "D.pouwels@live.nl", "0683431797", address);

            //Act
            SqlCustomerContext customersContext = new SqlCustomerContext();
            customersContext.Insert(customer);

            //Assert
            Customer customerbyid = customersContext.GetById(customersContext.CustomerIdAfterInsertion);
            Customer customerexpected = new Customer(customersContext.CustomerIdAfterInsertion, "Daphne", "Pouwels", "D.pouwels@live.nl", "0683431797", address);

            Assert.AreEqual(customerexpected.LastName, customerbyid.LastName);

        }

        private void DeleteTestCustomer(int id)
        {
            
        }
    }
}
