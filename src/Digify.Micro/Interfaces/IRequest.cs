using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Digify.Micro
{
    public interface IRequest : IRequest<Task> { }

    public interface IRequest<out TResponse> { }

    public interface IDomainEvent : IRequest
    {
        Guid AggregateRootId { get; }
        long Version { get; set; }
    }
}
