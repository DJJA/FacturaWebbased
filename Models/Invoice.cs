using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Invoice
    {
        public Invoice(int id, Customer customer, DateTime dateSend, DateTime datePayed, decimal totalPrice)
        {
            Id = id;
            Customer = customer;
            DateSend = dateSend;
            DatePayed = datePayed;
            TotalPrice = totalPrice;
        }
        public Invoice(int id, Customer customer, DateTime dateSend, DateTime datePayed, List<Task> tasks)
        {
            Id = id;
            
            Customer = customer;
            DateSend = dateSend;
            DatePayed = datePayed;
            Tasks = tasks;
        }

        public Invoice(int id, decimal totalPrice, DateTime datePayed, Customer customer)
        {
            Id = id;
            TotalPrice = totalPrice;
            DatePayed = datePayed;
            Customer = customer;
        }

        public Invoice(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
        public Customer Customer { get; set; }
        public List<Task> Tasks { get; set; }
        public DateTime DateSend { get; private set; }
        public DateTime DatePayed { get; private set; }

        private decimal totalPrice;
        public decimal TotalPrice
        {
            get
            {
                if (totalPrice != 0)
                {
                    return Convert.ToDecimal(totalPrice.ToString("0.00"));
                }
                else
                {
                    foreach (Task task in Tasks)
                    {
                        totalPrice += task.Amount * task.Price;
                    }
                    return totalPrice;
                }

            }
            private set { totalPrice = value; }
        }
        
    }
}
