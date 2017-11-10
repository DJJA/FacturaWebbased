using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using Models;

namespace LogicLayer
{
    public class InvoiceLogic : IInvoiceLogic
    {
        private IInvoiceRepository invoiceRepository;

        public InvoiceLogic(IInvoiceRepository invoiceRepository)
        {
            this.invoiceRepository = invoiceRepository;
        }
        public List<Invoice> GetAllInvoices()
        {
            return invoiceRepository.GetAll().ToList();
        }
    }
}
