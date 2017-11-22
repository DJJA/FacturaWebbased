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
            TotalPrice = Convert.ToDecimal(totalPrice.ToString("0.00"));
        }
        public Invoice(int id, Customer customer, DateTime dateSend, DateTime datePayed, List<Task> tasks)
        {
            Id = id;
            Customer = customer;
            DateSend = dateSend;
            DatePayed = datePayed;
            TotalPrice = Convert.ToDecimal(CalculateTotalPrice(tasks).ToString("0.00"));
            Tasks = tasks;
        }


        public decimal CalculateTotalPrice(List<Task> tasks)
        {
            foreach(var task in tasks)
            {
                TotalPrice += (task.Amount * task.Price);
            }

            return TotalPrice;
        }

        public int Id { get; set; }
        public Customer Customer { get; set; }
        public List<Task> Tasks { get; set; }
        public DateTime DateSend { get; set; }
        public DateTime DatePayed { get; set; }
        public decimal TotalPrice { get; set; }

    }
}
