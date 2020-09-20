using System;
using System.Collections.Generic;
using System.Text;

namespace Digify.Micro.Tests.Entities
{
    public class Sample1
    {
        public int Id { get; set; }

        public void Apply(int idIncr)
        {
            Id += idIncr;
        }

    }
}
