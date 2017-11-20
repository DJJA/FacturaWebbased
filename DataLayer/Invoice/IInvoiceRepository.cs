using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace DataLayer
{
    public interface IInvoiceRepository : IRepository<Invoice>
    {
        Invoice GetTasksOnInvoice(Invoice recentInvoice);
        IEnumerable<Invoice> GetInvoicesPerCustomer(int customerId);
    }


}
