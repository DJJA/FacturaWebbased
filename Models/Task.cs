using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Task
    {
        public int Id { get; private set; }
        public string Description { get; private set; }

        public Task(int id, string description)
        {
            Description = description;
            Id = id;
        }
    }
}
