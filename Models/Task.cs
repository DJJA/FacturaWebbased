using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public enum Unit { Hectare = 1, Stuks = 2, Uren = 3 }

    public class Task
    {
        public int Id { get; private set; }
        public string Description { get; private set; }
        public decimal Amount { get; set; }
        public decimal Price { get; set; }
        public DateTime Date { get; set; }
        public Unit Unit { get; set; }

        private decimal totalAmountOfAllSimilarTasks;

        public decimal TotalAmountOfAllSimilarTasks
        {
            get { return Convert.ToDecimal(totalAmountOfAllSimilarTasks.ToString("0.00")); }
            set { totalAmountOfAllSimilarTasks = value; }
        }

        public decimal TotalPrice
        {
            get { return Convert.ToDecimal((Amount * Price).ToString("0.00")); }
            set { }

        }

        public Task(int id, string description)
        {
            Description = description;
            Id = id;
        }

        public Task(int id, decimal totalAmountOfAllSimilarTasks)
        {
            Id = id;
            TotalAmountOfAllSimilarTasks = totalAmountOfAllSimilarTasks;
        }

        public Task(string description, decimal amount, decimal price, DateTime date)
        {
            Description = description;
            Amount = Convert.ToDecimal(amount.ToString("0.00"));
            Price = Convert.ToDecimal(price.ToString("0.00"));
            Date = date;
            CalculateTotalPrice(Price, Amount); //properties omdat die afgerond zijn
        }
        public Task(string description, decimal amount, decimal price, DateTime date, Unit unit)
        {
            Description = description;
            Amount = Convert.ToDecimal(amount.ToString("0.00"));
            Price = Convert.ToDecimal(price.ToString("0.00"));
            Date = date;
            Unit = unit;
            CalculateTotalPrice(Price, Amount); //properties omdat die afgerond zijn
        }

        private void CalculateTotalPrice(decimal price, decimal amount)
        {
            TotalPrice = Convert.ToDecimal((price * amount).ToString("0.00"));
        }
    }
}
