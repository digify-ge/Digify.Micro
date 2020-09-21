using Digify.Micro.Commands;
using Digify.Micro.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Digify.Micro.Tests.DomainEventBus.Handlers
{
    public class TestDomainEvent : IDomainEvent
    {
        public Guid AggregateRootId => new Guid();

        public long Version { get; set; }

        public string FirstName { get; set; }

        public TestDomainEvent(string firstName)
        {
            FirstName = firstName;
        }
    }
}
