using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Customer
    {
        public int ID { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Preposition { get; private set; }
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }
        public Address Address { get; private set; }
       // public List<Invoice> Invoices { get; private set; }

        //public Customer(int id, string firstName, string lastName, string email, string phoneNumber, Address address, List<Invoice> invoices)
        //{
        //    ID = id;
        //    FirstName = firstName;
        //    LastName = lastName;
        //    Email = email;
        //    PhoneNumber = phoneNumber;
        //    Address = address;
        //    Invoices = invoices;
        //}
        public Customer(string firstname, string lastname)
        {
            LastName = lastname;
            FirstName = firstname;
        }

        public Customer(int id, string firstName, string lastName, string preposition, string email, string phoneNumber, Address address)
        {
            ID = id;
            FirstName = firstName;
            LastName = lastName;
            Preposition = preposition;
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
        }

        public Customer(int id, string firstName, string lastName, string email, string phoneNumber, Address address)
        {
            ID = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
        }

    }
}
