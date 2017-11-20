using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public enum Unit { Hectare, Stuks, Uren }

    public class Task
    {
        public int Id { get; private set; }
        public string Description { get; private set; }
        public decimal Amount { get; set; }
        public decimal Price { get; set; }
        public DateTime Date { get; set; }
        public Unit Unit { get; set; }  

        public Task(int id, string description)
        {
            Description = description;
            Id = id;
        }

        public Task(string description, decimal amount, decimal price, DateTime date)
        {
            Description = description;
            Amount = amount;
            Price = price;
            Date = date;
        }
    }
}
