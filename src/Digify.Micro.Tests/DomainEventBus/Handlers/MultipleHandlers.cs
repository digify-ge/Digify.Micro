using Digify.Micro.Commands;
using Digify.Micro.Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Digify.Micro.Tests.DomainEventBus.Handlers
{
    public class TestOneHandler : IDomainEventHandlerAsync<TestDomainEvent>
    {
        public Task HandleAsync(TestDomainEvent domainEvent)
        {
            Debug.WriteLine(nameof(TestOneHandler));
            //Task.Delay(4000);
            DomainEventTests.HandlerOnePassed = true;
            return Task.CompletedTask;
        }
    }

    public class TestTwoHandler : IDomainEventHandlerAsync<TestDomainEvent>
    {
        public Task HandleAsync(TestDomainEvent domainEvent)
        {
            Debug.WriteLine(nameof(TestTwoHandler));
            DomainEventTests.HandlerTwoPassed = true;
            return Task.CompletedTask;
        }
    }
}
