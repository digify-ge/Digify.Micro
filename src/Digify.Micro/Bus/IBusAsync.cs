using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Digify.Micro
{
    public interface IBusAsync
    {
        Task<TResult> ExecuteAsync<TResult>(IRequest<TResult> request, CancellationToken cancellationToken = default);
        Task ExecuteAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest;
        Task ExecutesAsync<TRequest>(IEnumerable<TRequest> requests, CancellationToken cancellationToken = default) where TRequest : IRequest;
        Task PublishAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IDomainEvent;
        Task PublishAsync<TRequest>(IEnumerable<TRequest> events, CancellationToken cancellationToken = default) where TRequest : IDomainEvent;
        Task PublishAsync<T>(AggregateRoot<T> aggregate, CancellationToken cancellationToken = default) where T : IComparable;
    }
}
