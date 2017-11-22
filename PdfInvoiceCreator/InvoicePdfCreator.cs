using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using iTextSharp.text.pdf;
using System.IO;
using System.Linq.Expressions;

namespace PdfInvoiceCreator
{
    public class InvoicePdfCreator
    {
        //file path
        //string dir = System.IO.Path.GetDirectoryName(
        //    System.Reflection.Assembly.GetExecutingAssembly().Location);
        //string file = dir + @"\TestFile.txt";

        //FileStream fst = new FileStream(Server.MapPath("~/imgName.jpg"), FileMode.Open, FileAccess.Read, FileShare.Read);

        public byte[] CreatePdf(Invoice invoice)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (Document document = new Document())
                {
                    PdfWriter.GetInstance(document, ms);
                    document.Open();
                    document.Add(CreateCompanyDetails());
                    document.Add(CreateCustomerDetails(invoice));
                    document.Add(CreatePdfTitel());
                    document.Add(CreatePdfInvoiceDetails(invoice));
                    document.Add(CreateTable(invoice));
                    document.Add(CreateTotalPriceTable(invoice));
                    document.Add(CreatePaymentText());
                }
                return ms.ToArray();
            }
        }

        public byte[] CreatePdfInvoice(Invoice invoice)
        {

            //FileStream fs = new FileStream(@"D:\Test.pdf", FileMode.Create, FileAccess.Write, FileShare.None);
            //FileStream fts = new FileStream(Request.PhysicalApplicationPath + @"\1.pdf", FileMode.Create);
            byte[] result = null;
            #region oldworkingcode
            //PdfWriter writer = PdfWriter.GetInstance(doc, fs);
            //doc.Open();

            //Element elemnt = new Element();

            //doc.AddTitle("FactuurTitel");
            //doc.AddHeader("hoi", "header");

            //doc.Add(CreateCompanyDetails());
            //doc.Add(CreateCustomerDetails(invoice));

            //PdfPTable table = new PdfPTable(1);
            //table.AddCell("");
            //table.TotalWidth = 525f;
            //table.LockedWidth = true;

            //doc.Add(CreatePdfTitel());
            //doc.Add(CreatePdfInvoiceDetails(invoice));
            //doc.Add(table);

            //doc.Add(CreateTable(invoice));

            //doc.Add(CreatePaymentText());

            //doc.Close();
            #endregion
            using (MemoryStream ms = new MemoryStream())
            {
                Document doc = new Document();
                PdfWriter writer = PdfWriter.GetInstance(doc, ms);
                doc.Open();

                Element elemnt = new Element();

                doc.AddTitle("FactuurTitel");
                doc.AddHeader("hoi", "header");

                doc.Add(CreateCompanyDetails());
                doc.Add(CreateCustomerDetails(invoice));

                PdfPTable table = new PdfPTable(1);
                table.AddCell("");
                table.TotalWidth = 525f;
                table.LockedWidth = true;

                doc.Add(CreatePdfTitel());
                doc.Add(CreatePdfInvoiceDetails(invoice));
                doc.Add(table);

                doc.Add(CreateTable(invoice));

                doc.Add(CreatePaymentText());

                doc.Close();
                result = ms.ToArray();
            }
            return result;
        }

        private Paragraph CreateCustomerDetails(Invoice invoice)
        {
            Paragraph cusDetails = new Paragraph(
                $"{invoice.Customer.FirstName} {invoice.Customer.Preposition} {invoice.Customer.LastName} \n" +
                $"{invoice.Customer.Address.StreetName}\n" +
                $"{invoice.Customer.Address.ZipCode} {invoice.Customer.Address.Place} \n" +
                $"");

            cusDetails.Alignment = Element.ALIGN_LEFT;
            cusDetails.IndentationLeft = 100;

            return cusDetails;
        }

        private Paragraph CreateCompanyDetails()
        {
            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

            Font font = new Font(bf, 10, Font.NORMAL);
            Paragraph details = new Paragraph(
                "CTW A.J. van de Laar \n " +
                "Albert Kuijpersstraat 10 \n" +
                "5712CK Someren-Eind \n" +
                "adrie.vd.laar@hotmail.nl \n" +
                "BTW nr: " + "932480 \n" +
                "KvK nr: \n" +
                "IBAN: ", font
            );

            details.Alignment = Element.ALIGN_RIGHT;

            return details;
        }
        public Paragraph CreatePdfInvoiceDetails(Invoice invoice)
        {
            string date = invoice.DateSend.ToShortDateString();
            Paragraph detailed = new Paragraph(
                $"Factuur datum:  {invoice.DateSend.ToShortDateString()} \n" +
                $"Factuur nummer: {invoice.Id}"
            );
            detailed.Alignment = Element.ALIGN_LEFT;
            detailed.SpacingAfter = 50f;

            return detailed;
        }

        private PdfPTable CreateTotalPriceTable(Invoice invoice)
        {
            PdfPTable table = new PdfPTable(new float[] { 150f, 30f, 35f, 50f, 45f });
            table.TotalWidth = 500f;
            table.LockedWidth = true;

            table.DefaultCell.Border = Rectangle.NO_BORDER;

            PdfPCell exclBtw = new PdfPCell(new Phrase(new Chunk("excl. btw:", FontFactory.GetFont("Arial", 10f, Font.BOLD))));
            exclBtw.HorizontalAlignment = Element.ALIGN_RIGHT;
            exclBtw.VerticalAlignment = Element.ALIGN_BOTTOM;
            exclBtw.Border = Rectangle.TOP_BORDER;
            exclBtw.FixedHeight = 35f;

            table.AddCell("");
            table.AddCell("");
            table.AddCell("");
            table.AddCell(exclBtw);

            PdfPCell cellInvoiceTotalPrice = new PdfPCell(new Phrase($"€ {invoice.TotalPrice}"));
            cellInvoiceTotalPrice.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellInvoiceTotalPrice.VerticalAlignment = Element.ALIGN_BOTTOM;
            cellInvoiceTotalPrice.Border = Rectangle.TOP_BORDER;
            cellInvoiceTotalPrice.FixedHeight = 35f;

            table.AddCell(cellInvoiceTotalPrice);



            PdfPCell inclBtw = new PdfPCell(new Phrase(new Chunk("Totaal incl. btw:", FontFactory.GetFont("Arial", 10f, Font.BOLD))));
            inclBtw.HorizontalAlignment = Element.ALIGN_RIGHT;
            inclBtw.VerticalAlignment = Element.ALIGN_BOTTOM;
            inclBtw.Border = Rectangle.NO_BORDER;

            table.AddCell("");
            table.AddCell("");
            table.AddCell("");
            table.AddCell(inclBtw);

            decimal btw = 1.21m;
            decimal inclbtw = Convert.ToDecimal((invoice.TotalPrice * btw).ToString("0.00"));

            PdfPCell cellInvoiceinclTotalPrice = new PdfPCell(new Phrase($"€ {inclbtw}"));
            cellInvoiceinclTotalPrice.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellInvoiceinclTotalPrice.VerticalAlignment = Element.ALIGN_BOTTOM;
            cellInvoiceinclTotalPrice.Border = Rectangle.NO_BORDER;

            table.AddCell(cellInvoiceinclTotalPrice);

            return table;
        }

        private PdfPTable CreateTable(Invoice invoice)
        {

            PdfPTable table = new PdfPTable(new float[] { 150f, 30f, 35f, 5f, 30f, 5f, 35f });
            table.TotalWidth = 500f;
            table.LockedWidth = true;


            PdfPCell definition = new PdfPCell(new Phrase(new Chunk("Omschrijving", FontFactory.GetFont("Arial", 10f, Font.BOLD))));
            PdfPCell amount = new PdfPCell(new Phrase(new Chunk("Aantal", FontFactory.GetFont("Arial", 10f, Font.BOLD))));
            PdfPCell totalPrice = new PdfPCell(new Phrase(new Chunk("Totale prijs", FontFactory.GetFont("Arial", 10f, Font.BOLD))));
            PdfPCell pricePerPiece = new PdfPCell(new Phrase(new Chunk("Stukprijs", FontFactory.GetFont("Arial", 10f, Font.BOLD))));
            //PdfPCell totalPrice = new PdfPCell(new Phrase(new Chunk("Totaal prijs", FontFactory.GetFont("Arial", 10f, Font.BOLD))));


            table.DefaultCell.Border = Rectangle.NO_BORDER;

            totalPrice.Border = Rectangle.NO_BORDER;
            totalPrice.HorizontalAlignment = Element.ALIGN_RIGHT;
            definition.Border = Rectangle.NO_BORDER;
            amount.Border = Rectangle.NO_BORDER;
            pricePerPiece.Border = Rectangle.NO_BORDER;
            pricePerPiece.HorizontalAlignment = Element.ALIGN_RIGHT;



            table.AddCell(definition);
            table.AddCell(amount);
            table.AddCell("");
            table.AddCell("");
            table.AddCell(pricePerPiece);
            table.AddCell("");
            table.AddCell(totalPrice);

            

            foreach (var task in invoice.Tasks)
            {
                table.AddCell(task.Description);
                table.AddCell($"à {task.Amount}");
                table.AddCell(task.Unit.ToString());

                table.AddCell("€");
                PdfPCell cellTaskPricePiece = new PdfPCell(new Phrase($"{task.Price}"));
                cellTaskPricePiece.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellTaskPricePiece.Border = Rectangle.NO_BORDER;

                table.AddCell(cellTaskPricePiece);

                PdfPCell cellTaskTotalPrice = new PdfPCell(new Phrase($"{task.TotalPrice}"));
                cellTaskTotalPrice.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellTaskTotalPrice.Border = Rectangle.NO_BORDER;
                table.AddCell("€");
                table.AddCell(cellTaskTotalPrice);
            }

            return table;
        }

        private Paragraph CreatePaymentText()
        {
            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

            Font font = new Font(bf, 8, Font.NORMAL);

            Paragraph paragraph = new Paragraph(
                "Gelieve het factuurbedrag binnen 14 dagen na factuurdatum over te maken op rekeningnummer xxxxxxxx t.n.v. xxxxxx te xxxxx. \n" +
                "Mocht u het niet eens zijn met deze factuur, gelieve binnen 5 werkdagen schriftelijk te reageren.", font);

            //paragraph.Alignment = Element.ALIGN_TOP;
            paragraph.SpacingBefore = 200;
            paragraph.Alignment = Element.ALIGN_BOTTOM;

            return paragraph;
        }

        public Paragraph CreatePdfTitel()
        {
            Paragraph titlepara = new Paragraph(new Chunk("Factuur", FontFactory.GetFont("Arial", 14f, Font.BOLD)));
            titlepara.Alignment = Element.ALIGN_LEFT;
            titlepara.SpacingBefore = 100f;
            titlepara.SpacingAfter = 5f;

            return titlepara;
        }
    }
}
