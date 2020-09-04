using System;

namespace Digify.Micro.Domain
{
    public interface IDomainEvent
    {
        Guid AggregateRootId { get; }
        long Version { get; set; }
    }
}
