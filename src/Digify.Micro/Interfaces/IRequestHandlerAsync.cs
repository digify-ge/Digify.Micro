using System.Threading;
using System.Threading.Tasks;

namespace Digify.Micro
{
    public interface IRequestHandlerAsync<in TRequest, TResult> where TRequest : IRequest<TResult>
    {
        Task<TResult> HandleAsync(TRequest command, CancellationToken cancellationToken);
    }
    public interface IRequestHandlerAsync<in TRequest> 
    {
        Task HandleAsync(TRequest command, CancellationToken cancellationToken);
    }
}
