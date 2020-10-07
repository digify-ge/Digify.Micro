using System;
using System.Collections.Generic;
using System.Text;

namespace Digify.Micro.Tests.Domain.DomainEvents
{
    public class SomethingGoodHappenedDomainEvent : IDomainEvent
    {
        public Guid AggregateRootId => new Guid();

        public long Version { get; set; }

        public string FirstName { get; set; }

        public SomethingGoodHappenedDomainEvent(string firstName)
        {
            FirstName = firstName;
        }
    }
}
