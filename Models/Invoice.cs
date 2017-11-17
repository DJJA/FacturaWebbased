using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Invoice
    {
        public Invoice(int id, Customer customer, DateTime dateSend, DateTime datePayed,decimal totalPrice)
        {
            Id = id;
            Customer = customer;
            DateSend = dateSend;
            DatePayed = datePayed;
            TotalPrice = totalPrice;
        }
        public Invoice(int id, Customer customer, DateTime dateSend, DateTime datePayed, decimal totalPrice, List<Task> tasks)
        {
            Id = id;
            Customer = customer;
            DateSend = dateSend;
            DatePayed = datePayed;
            TotalPrice = totalPrice;
            Tasks = tasks;
        }

        //customerID, dateSend, datePayed, amount, totalPrice
        public int Id { get; set; }
        public Customer Customer { get; set; }
        public List<Task> Tasks { get; set; }
        public DateTime DateSend { get; set; }
        public DateTime DatePayed { get; set; }
        public int Amount { get; set; }
        public decimal TotalPrice { get; set; }

       
        

    }
}
