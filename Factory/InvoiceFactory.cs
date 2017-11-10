using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using LogicLayer;

namespace Factory
{
    public class InvoiceFactory
    {
        public static IInvoiceLogic ManageInvoices()
        {
            return new InvoiceLogic(new InvoiceRepository(new SqlInvoiceContext()));
        }
    }
}
