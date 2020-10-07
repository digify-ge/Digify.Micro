using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Digify.Micro.Internal
{
    internal abstract class NonReturnableHandlerWrapper
    {
        public abstract Task Handle(IRequest request, CancellationToken cancellationToken, ServiceFactory serviceFactory,
                                    Func<IEnumerable<Func<IRequest, CancellationToken, Task>>, IRequest, CancellationToken, Task> publish);
    }

    internal class NonReturnableHandlerWrapperImpl<TRequest> : NonReturnableHandlerWrapper
        where TRequest : IRequest
    {
        public override Task Handle(IRequest request, CancellationToken cancellationToken, ServiceFactory serviceFactory,
                                    Func<IEnumerable<Func<IRequest, CancellationToken, Task>>, IRequest, CancellationToken, Task> publish)
        {
            var handlers = serviceFactory
                .GetInstances<IRequestHandlerAsync<TRequest, Part>>()
                .Select(x => new Func<IRequest, CancellationToken, Task>((theRequest, theToken) => x.HandleAsync((TRequest)theRequest, theToken)));

            return publish(handlers, request, cancellationToken);
        }
    }

    internal abstract class DomainEventHandlerWrapper
    {
        public abstract Task Handle(IDomainEvent request, CancellationToken cancellationToken, ServiceFactory serviceFactory,
                                    Func<IEnumerable<Func<IDomainEvent, CancellationToken, Task>>, IDomainEvent, CancellationToken, Task> publish);
    }

    internal class DomainEventHandlerWrapperImpl<TDomainEvent> : DomainEventHandlerWrapper
        where TDomainEvent : IDomainEvent
    {
        public override Task Handle(IDomainEvent request, CancellationToken cancellationToken, ServiceFactory serviceFactory, Func<IEnumerable<Func<IDomainEvent, CancellationToken, Task>>, IDomainEvent, CancellationToken, Task> publish)
        {
            var handlers = serviceFactory
               .GetInstances<IDomainEventHandlerAsync<TDomainEvent>>()
               .Select(x => new Func<IDomainEvent, CancellationToken, Task>((theRequest, theToken) => x.HandleAsync((TDomainEvent)theRequest, theToken)));

            return publish(handlers, request, cancellationToken);
        }
    }
}
