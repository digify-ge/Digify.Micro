using System.Threading;
using System.Threading.Tasks;

namespace Digify.Micro
{
    public interface IRequestHandlerAsync<in TRequest, TResponse>
      where TRequest : IRequest<TResponse>
    {
        Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
    }

    public interface IRequestHandlerAsync<in TRequest> : IRequestHandlerAsync<TRequest, Part>
        where TRequest : IRequest<Part>
    {
    }
    public abstract class AsyncRequestHandler<TRequest> : IRequestHandlerAsync<TRequest>
        where TRequest : IRequest
    {
        async Task<Part> IRequestHandlerAsync<TRequest, Part>.HandleAsync(TRequest request, CancellationToken cancellationToken)
        {
            await Handle(request, cancellationToken).ConfigureAwait(false);
            return Part.Value;
        }

        protected abstract Task Handle(TRequest request, CancellationToken cancellationToken);
    }

    public abstract class RequestHandler<TRequest, TResponse> : IRequestHandlerAsync<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> IRequestHandlerAsync<TRequest, TResponse>.HandleAsync(TRequest request, CancellationToken cancellationToken)
            => Task.FromResult(Handle(request));

        protected abstract TResponse Handle(TRequest request);
    }

    public abstract class RequestHandler<TRequest> : IRequestHandlerAsync<TRequest>
        where TRequest : IRequest
    {
        Task<Part> IRequestHandlerAsync<TRequest, Part>.HandleAsync(TRequest request, CancellationToken cancellationToken)
        {
            Handle(request);
            return Part.Task;
        }

        protected abstract void Handle(TRequest request);
    }
}
