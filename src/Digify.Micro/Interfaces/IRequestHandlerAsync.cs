using System.Threading;
using System.Threading.Tasks;

namespace Digify.Micro
{
    public interface IRequestHandlerAsync<in TRequest, TResponse>  where TRequest : IRequest<TResponse>
    {
        Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
    }

    public interface IRequestHandlerAsync<in TRequest> where TRequest : IRequest
    {
        Task HandleAsync(TRequest request, CancellationToken cancellationToken);
    }
}
