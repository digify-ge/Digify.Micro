
using Digify.Micro.Domain;
using Digify.Micro.Tests.Domain.DomainEvents;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Digify.Micro.Tests.Application.DomainEventHandlers
{
    public class TestOneHandler : IDomainEventHandlerAsync<SomethingGoodHappenedDomainEvent>
    {
        public static bool HandlerOnePassed { get; private set; }

        public Task HandleAsync(SomethingGoodHappenedDomainEvent domainEvent)
        {
            Debug.WriteLine(nameof(TestOneHandler));
            //Task.Delay(4000);
            HandlerOnePassed = true;
            return Task.CompletedTask;
        }
    }

    public class TestTwoHandler : IDomainEventHandlerAsync<SomethingGoodHappenedDomainEvent>
    {
        public static bool HandlerTwoPassed { get; private set; }

        public Task HandleAsync(SomethingGoodHappenedDomainEvent domainEvent)
        {
            Debug.WriteLine(nameof(TestTwoHandler));
            HandlerTwoPassed = true;
            return Task.CompletedTask;
        }
    }
}
