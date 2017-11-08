using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public struct Address
    {
        public string StreetName { get; private set; }
        public string Place { get; private set; }
        public string ZipCode { get; private set; }

        public Address(string streetName, string place, string zipCode)
        {
            StreetName = streetName;
            Place = place;
            ZipCode = zipCode;
        }

        public override string ToString()
        {
            return $"Straat: {StreetName} Postcode: {ZipCode} Plaats: {Place}";
        }
    }
}
