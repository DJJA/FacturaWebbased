using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace LogicLayer
{
    public interface IInvoiceLogic
    {
        List<Invoice> GetAllInvoices();
        void CreateInvoice(Invoice invoice);
        List<string> GetId(string ids, List<string> list);
        Invoice GetById(int id);
        Invoice GetTasksOnInvoice(Invoice invoice);
        List<Invoice> GetInvoicesPercustomer(int customerId);
        void GeneratePdf(Invoice invoice);
    }
}
