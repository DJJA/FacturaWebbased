using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Models;

namespace FacturaWeb.PdfGenerator
{
    public class PdfGenerator
    {

        //file path
        //string dir = System.IO.Path.GetDirectoryName(
        //    System.Reflection.Assembly.GetExecutingAssembly().Location);
        //string file = dir + @"\TestFile.txt";

        //FileStream fst = new FileStream(Server.MapPath("~/imgName.jpg"), FileMode.Open, FileAccess.Read, FileShare.Read);

        Document doc = new Document();
        public Document CreatePdfInvoice(Invoice invoice)
        {

            FileStream fs = new FileStream(@"D:\Test.pdf", FileMode.Create, FileAccess.Write, FileShare.None);
            //FileStream fts = new FileStream(Request.PhysicalApplicationPath + @"\1.pdf", FileMode.Create);


            //PdfWriter.GetInstance(doc, fts);
            //doc.Open();
            //doc.Add(new Paragraph("Hello World"));
            //doc.Close();
            //Response.Redirect("~/1.pdf");

            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
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
            return doc;
        }

        //PdfInvoice invoicePdf = new PdfInvoice();


        //invoicePdf.ContentType = postedFile.ContentType;
        //    invoicePdf.Name_File = Path.GetFileName(postedFile.FileName);
        //    invoicePdf.Extension = Path.GetExtension(invoicePdf.Name_File);
        //    HttpPostedFileBase file = postedFile;
        //byte[] document = new byte[file.ContentLength];
        //file.InputStream.Read(document, 0, file.ContentLength);
        //    invoicePdf.FileData = document;
        //    invoicePdf.FileSize = document.Length;
        //    invoicePdf.DisplayName = postedFile.FileName;

        //    invoiceLogic.InsertInvoiceFile(invoicePdf);

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
            Paragraph details = new Paragraph(
                "CTW A.J. van de Laar \n " +
                "Albert Kuijpersstraat 10 \n" +
                "5712CK Someren-Eind \n" +
                "adrie.vd.laar@hotmail.com \n" +
                "BTW nr: " + "932480 \n" +
                "KvK nr: \n" +
                "IBAN: "
            );

            details.Alignment = Element.ALIGN_RIGHT;

            return details;
        }
        public Paragraph CreatePdfInvoiceDetails(Invoice invoice)
        {
            Paragraph detailed = new Paragraph(
                $"Factuur datum:  {invoice.DateSend} \n" +
                $"Factuur nummer: {invoice.Id}"
            );
            detailed.Alignment = Element.ALIGN_LEFT;
            //detailed.SpacingBefore = 100f;
            detailed.SpacingAfter = 50f;

            return detailed;
        }

        private PdfPTable CreateTable(Invoice invoice)
        {

            PdfPTable table = new PdfPTable(new float[] { 195f, 20f, 30f, 40f });
            table.TotalWidth = 500f;
            table.LockedWidth = true;


            PdfPCell definition = new PdfPCell(new Phrase(new Chunk("Omschrijving", FontFactory.GetFont("Arial", 10f, Font.BOLD))));
            PdfPCell amount = new PdfPCell(new Phrase(new Chunk("Aantal", FontFactory.GetFont("Arial", 10f, Font.BOLD))));
            PdfPCell pricePerPiece = new PdfPCell(new Phrase(new Chunk("Stuks prijs", FontFactory.GetFont("Arial", 10f, Font.BOLD))));
            //PdfPCell totalPrice = new PdfPCell(new Phrase(new Chunk("Totaal prijs", FontFactory.GetFont("Arial", 10f, Font.BOLD))));


            table.DefaultCell.Border = Rectangle.NO_BORDER;


            definition.Border = Rectangle.NO_BORDER;
            amount.Border = Rectangle.NO_BORDER;
            pricePerPiece.Border = Rectangle.NO_BORDER;


            table.AddCell(definition);
            table.AddCell(amount);
            table.AddCell("");
            table.AddCell(pricePerPiece);

            foreach (var task in invoice.Tasks)
            {
                table.AddCell(task.Description);
                table.AddCell($"à {task.Amount}");
                table.AddCell(task.Unit.ToString());
                table.AddCell($"€ {task.Price.ToString("0.##")}");
            }

            //TODO: Totaalprijs toevoegen en rechts uitlijnen
            //PdfPCell totalPrice = new PdfPCell(new Phrase(new Chunk("Totaal prijs", FontFactory.GetFont("Arial", 10f, Font.BOLD))));
            //totalPrice.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            //totalPrice.Border = Rectangle.NO_BORDER;

            //table.AddCell($"{totalPrice} {invoice.TotalPrice}");



            return table;
        }

        private Paragraph CreatePaymentText()
        {
            Paragraph paragraph;

            paragraph = new Paragraph(
                "Gelieve het factuurbedrag binnen 14 dagen na factuurdatum over te maken op rekeningnummer xxxxxxxx t.n.v. xxxxxx te xxxxx. \n" +
                "Mocht u het niet eens zijn met deze factuur, gelieve binnen 5 werkdagen schriftelijk te reageren.");

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
