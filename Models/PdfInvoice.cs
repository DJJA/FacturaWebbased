using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class PdfInvoice
    {
        public string Name_File { get; set; }
        public string DisplayName { get; set; }
        public string Extension { get; set; }
        public string ContentType { get; set; }
        public byte[] FileData { get; set; }
        public int FileSize { get; set; }
        public PdfInvoice()
        {

        }
    }
}
