using Digify.Micro.Commands;
using Digify.Micro.Domain;
using Digify.Micro.Tests.Domain.DomainEvents;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Digify.Micro.Tests.Application.CommandHandlers
{
    public class TestCommandHandler : ICommandHandlerAsync<TestCommand>
    {
        private readonly IDomainEventBusAsync _domainEventBus;

        public TestCommandHandler(IDomainEventBusAsync domainEventBus)
        {
            _domainEventBus = domainEventBus;
        }

        public async Task HandleAsync(TestCommand command)
        {
            IDomainEvent evt = new SomethingGoodHappenedDomainEvent("Givi");
            await _domainEventBus.ExecuteAsync(evt);
        }

    }
}
