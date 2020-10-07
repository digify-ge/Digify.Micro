using System.Collections.Generic;

namespace Digify.Micro
{
    public interface IEventSourcingAggregate
    {
        void ApplyEvent(IDomainEvent @event, long version);
        IEnumerable<IDomainEvent> GetUncommittedEvents();
        void ClearUncommittedEvents();
        void RaiseEvent(IDomainEvent @event);
    }
}
