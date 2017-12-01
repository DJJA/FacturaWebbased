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
        void InvoicePayed(Invoice invoice);

        Invoice GetTotalPriceByYear(int year);

        Invoice GetTop3Customers();
        //IEnumerable<Invoice> GetInvoicesPerCustomer(int customerId);
    }
}
