using Digify.Micro.Tests.Domain.DomainEvents;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Digify.Micro.Tests.Application.CommandHandlers
{
    public class TestCommandHandler : IRequestHandlerAsync<TestCommand>
    {
        private readonly IBusAsync _domainEventBus;

        public TestCommandHandler(IBusAsync domainEventBus)
        {
            _domainEventBus = domainEventBus;
        }

        public async Task<Part> HandleAsync(TestCommand request, CancellationToken cancellationToken)
        {
            IDomainEvent evt = new SomethingGoodHappenedDomainEvent("Givi");
            await _domainEventBus.PublishEvent(evt);
            return await Part.Task;
        }
    }
}
