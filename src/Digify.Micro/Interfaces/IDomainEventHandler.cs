using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Digify.Micro
{
    public interface IDomainEventHandlerAsync<in TDomainEvent>
       where TDomainEvent : IDomainEvent
    {
        Task HandleAsync(TDomainEvent domainEvent, CancellationToken cancellationToken);
    }

    public abstract class DomainEventHandler<TDomainEvent> : IDomainEventHandlerAsync<TDomainEvent>
        where TDomainEvent : IDomainEvent
    {
        Task IDomainEventHandlerAsync<TDomainEvent>.HandleAsync(TDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            HandleAsync(domainEvent);
            return Task.CompletedTask;
        }
        protected abstract void HandleAsync(TDomainEvent notification);
    }
}
