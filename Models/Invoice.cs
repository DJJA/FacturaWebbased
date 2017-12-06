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
            TotalPrice = CalculateTotalPrice(tasks);
            Tasks = tasks;
        }

        public Invoice(int id, decimal totalPrice, DateTime datePayed, Customer customer)
        {
            Id = id;
            TotalPrice = totalPrice;
            DatePayed = datePayed;
            Customer = customer;
        }

        public Invoice()
        {
            
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
        //TODO:Kijken of date nullable weg kan
        public Nullable<DateTime> DatePayed { get; set; }

        public decimal TotalPrice
        {
            get { return Convert.ToDecimal(totalPrice.ToString("0.00")); }
            set { totalPrice = value; }
        }
        private decimal totalPrice;
        private decimal price;
        public decimal TotalPriceByYear
        {
            get { return Convert.ToDecimal(price.ToString("0.00")); }
            set { price = value; }
        }

    }
}
