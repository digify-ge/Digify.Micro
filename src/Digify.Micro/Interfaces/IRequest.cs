using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Digify.Micro
{
    public interface IRequest : IRequest { }

    public interface IRequest<out TResponse> : IRequestBase { }
    public interface IRequestBase { }
    public interface IQuery<T> : IRequest<T> { }
    public interface ICommand<T> : IRequest<T> { }
    public interface ICommand : IRequest { }
    public interface IDomainEvent
    {
        Guid AggregateRootId { get; }
        long Version { get; set; }
    }
}
