using System;
using System.Collections.Generic;
using System.Text;

namespace Digify.Micro.Tests.Entities
{
    public class AddressValueObject : ValueObject
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Flat { get; set; }

        public AddressValueObject(string country, string city, string street, string flat)
        {
            Country = country;
            City = city;
            Street = street;
            Flat = flat;
        }
    }
}
