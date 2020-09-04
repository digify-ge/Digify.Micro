using Newtonsoft.Json;
using Digify.Micro.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Digify.Micro
{
    public abstract class AggregateRoot<TAggregateRootId> : Entity<TAggregateRootId>, IEventSourcingAggregate, IAggregateRoot<TAggregateRootId> where TAggregateRootId : IComparable
    {
        [JsonProperty]
        public long Version { get; set; }

        protected ICollection<IDomainEvent> _uncommittedEvents = new List<IDomainEvent>();

        public void ApplyEvent(IDomainEvent @event, long version)
        {
            ((dynamic)this).Apply((dynamic)@event);
            Version = version;
        }

        public void ClearUncommittedEvents()
        {
            _uncommittedEvents.Clear();
        }

        public IEnumerable<IDomainEvent> GetUncommittedEvents()
        {
            return _uncommittedEvents.AsEnumerable();
        }

        public void RaiseEvent(IDomainEvent @event)
        {
            Version++;
            @event.Version = Version;
            _uncommittedEvents.Add(@event);
        }
    }

    public interface IAggregateRoot<TAggregateRootId> : IEntity<TAggregateRootId> where TAggregateRootId : IComparable
    {
        long Version { get; }
    }
}
