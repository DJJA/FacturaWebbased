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

        public void CreateInvoice(Invoice invoice)
        {
            invoiceRepository.Add(invoice);
        }

        //TODO: unittest schrijven voor getID
        public List<string> GetId(string serializeString, List<string> ids)
        {
            bool idsLeft = false;

            if (serializeString.Contains('='))
            {
                //zoek voor het index nummer 
                int placeBefore = serializeString.IndexOf('='); //zoek het begin van de index
                int placeAfter; //declaratie voor het eind van de index

                if (serializeString.Contains('&')) //als er een eind notitie is 
                {
                    placeAfter = serializeString.IndexOf('&'); //zoek het eind van de index
                }
                else
                {
                    placeAfter = serializeString.Length; //het laatste deel van de string is de index
                }

                int length = placeAfter - placeBefore - 1;  //bekijk de lengte van de index

                //onthoudt het index nummer
                string id = serializeString.Substring(placeBefore + 1, length);
                ids.Add(id); //voeg het nummer toe aan de lijst

                if (serializeString.Length > placeAfter +1)
                {
                    //verwijder het eerste deel van de string
                    serializeString = serializeString.Substring(placeAfter + 1);
                    idsLeft = true;
                }
                
            }
            if (idsLeft)
            {
                GetId(serializeString, ids);
            }

            return ids;
        }

        public Invoice GetById(int id)
        {
            return invoiceRepository.GetById(id);
        }

        public Invoice GetTasksOnInvoice(Invoice invoice)
        {
            return invoiceRepository.GetTasksOnInvoice(invoice);
        }

        public List<Invoice> GetInvoicesPercustomer(int customerId)
        {
            return invoiceRepository.GetInvoicesPerCustomer(customerId).ToList();
        }
    }
}
