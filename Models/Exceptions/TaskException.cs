using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class TaskException :Exception
    {
        public TaskException(string message)
            : base(message)
        {
        }
    }
}
