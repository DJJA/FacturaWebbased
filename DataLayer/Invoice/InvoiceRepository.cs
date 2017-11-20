using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace DataLayer
{
    public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
    {
        private readonly IInvoiceContext invoiceContext;

        public InvoiceRepository(IInvoiceContext context) 
            : base(context)
        {
            this.invoiceContext = context;
        }

        public IEnumerable<Invoice> GetInvoicesPerCustomer(int customerId)
        {
            return invoiceContext.GetInvoicesPerCustomer(customerId);
        }

        public Invoice GetTasksOnInvoice(Invoice recentInvoice)
        {
            return invoiceContext.GetTasksOnInvoice(recentInvoice);
        }
    }
}
