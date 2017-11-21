using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace iTextSharp
{
    public class PdfGenerator
    {
        FileStream fs = new FileStream(@"D:\\test.pdf", FileMode.Create, FileAccess.Write, FileShare.None);


        public void CreatePdfInvoice()
        {
            Document doc = new Document();

            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
            doc.Open();

            Element elemnt = new Element();


            doc.AddTitle("FactuurTitel");
            doc.AddHeader("hoi", "header");

            doc.Add(CreateCompanyDetails());
            doc.Add(CreateCustomerDetails());

            PdfPTable table = new PdfPTable(1);
            table.AddCell("");
            table.TotalWidth = 525f;
            table.LockedWidth = true;

            doc.Add(CreatePdfTitel());
            doc.Add(table);
            doc.Add(CreatePdfInvoiceDetails());

            doc.Add(CreateTable());
            doc.Add(CreatePaymentText());

            doc.Close();

        }
        public Paragraph CreatePdfTitel()
        {
            Paragraph titlepara = new Paragraph(new Chunk("Factuur", FontFactory.GetFont("Arial", 14f, Font.BOLD)));
            titlepara.Alignment = Element.ALIGN_LEFT;
            titlepara.SpacingBefore = 100f;
            titlepara.SpacingAfter = 5f;

            return titlepara;
        }
        public Paragraph CreatePdfInvoiceDetails()
        {
            Paragraph detailed = new Paragraph(
                "Factuur datum: \n" +
                "Factuur nummer: "
                );
            detailed.Alignment = Element.ALIGN_LEFT;
            //detailed.SpacingBefore = 100f;
            detailed.SpacingAfter = 50f;

            return detailed;
        }


        private PdfPTable CreateTable()
        {

            PdfPTable table = new PdfPTable(new float[] { 200f, 30f, 30f, 30f });
            table.TotalWidth = 500f;
            table.LockedWidth = true;


            PdfPCell definition = new PdfPCell(new Phrase(new Chunk("Omschrijving", FontFactory.GetFont("Arial", 10f, Font.BOLD))));
            PdfPCell amount = new PdfPCell(new Phrase(new Chunk("Aantal", FontFactory.GetFont("Arial", 10f, Font.BOLD))));
            PdfPCell pricePerPiece = new PdfPCell(new Phrase(new Chunk("Stuks prijs", FontFactory.GetFont("Arial", 10f, Font.BOLD))));
            PdfPCell totalPrice = new PdfPCell(new Phrase(new Chunk("Totaal prijs", FontFactory.GetFont("Arial", 10f, Font.BOLD))));


            table.DefaultCell.Border = Rectangle.NO_BORDER;


            definition.Border = Rectangle.NO_BORDER;
            amount.Border = Rectangle.NO_BORDER;
            pricePerPiece.Border = Rectangle.NO_BORDER;
            totalPrice.Border = Rectangle.NO_BORDER;

            table.AddCell(definition);
            table.AddCell(amount);
            table.AddCell(pricePerPiece);
            table.AddCell(totalPrice);

            table.AddCell("hallo");
            table.AddCell("hallo");
            table.AddCell("hallo");
            table.AddCell("hallo");


            return table;
        }

        private Paragraph CreateCompanyDetails()
        {
            Paragraph details = new Paragraph(
                "Aj van de Laar \n " +
                "Albert kuijpersstraat 10 \n" +
                "5712CK Someren-Eind \n" +
                "email@email.nl \n" +
                "BTW nr: " + "932480 \n" +
                "KvK nr: \n" +
                "IBAN: "
                );

            details.Alignment = Element.ALIGN_RIGHT;

            return details;
        }

        private Paragraph CreateCustomerDetails()
        {
            Paragraph cusDetails = new Paragraph(
                "poppetje" + "\n"
                + "hoi");

            cusDetails.Alignment = Element.ALIGN_LEFT;
            cusDetails.IndentationLeft = 100;

            return cusDetails;
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
    }
}
