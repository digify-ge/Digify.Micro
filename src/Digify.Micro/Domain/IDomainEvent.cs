using System;

namespace Digify.Micro.Domain
{
    public interface IDomainEvent : IRequest
    {
        Guid AggregateRootId { get; }
        long Version { get; set; }
    }
}
