using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class InvoiceException : Exception
    {
        public InvoiceException(string message)
            : base(message)
        {
        }
    }
}
