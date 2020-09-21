using Digify.Micro.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Digify.Micro.Tests.DomainEventBus.Handlers
{
    public class TestOneHandler : IDomainEventHandlerAsync<TestDomainEvent>
    {
        public Task HandleAsync(TestDomainEvent domainEvent)
        {
            Console.WriteLine(nameof(TestOneHandler));
            return Task.CompletedTask;
        }
    }
    public class TestTwoHandler : IDomainEventHandlerAsync<TestDomainEvent>
    {
        public Task HandleAsync(TestDomainEvent domainEvent)
        {
            Console.WriteLine(nameof(TestTwoHandler));
            return Task.CompletedTask;
        }
    }
}
