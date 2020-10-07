using System.Threading;
using System.Threading.Tasks;

namespace Digify.Micro
{
    public interface IBusAsync
    {
        Task<TResult> ExecuteAsync<TResult>(IRequest<TResult> request, CancellationToken cancellationToken = default);
        Task ExecuteAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest;
        Task PublishEvent<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IDomainEvent;
    }
}
