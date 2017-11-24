using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace DataLayer
{
    public interface IInvoiceContext : IContext<Invoice>
    {
        void InsertTasksToInvoice(Invoice invoice);
        Invoice GetTasksOnInvoice(Invoice invoice);
        //IEnumerable<Invoice> GetInvoicesPerCustomer(int customerId);
    }
}
