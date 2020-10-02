using System.Threading;
using System.Threading.Tasks;

namespace Digify.Micro
{
    public interface IRequestHandlerAsync<in TRequest, TResponse>
      where TRequest : IRequest<TResponse>
    {
        Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
    }

    public interface IRequestHandlerAsync<in TRequest> : IRequestHandlerAsync<TRequest, Unit>
        where TRequest : IRequest<Unit>
    {
    }
    public abstract class AsyncRequestHandler<TRequest> : IRequestHandlerAsync<TRequest>
        where TRequest : IRequest
    {
        async Task<Unit> IRequestHandlerAsync<TRequest, Unit>.HandleAsync(TRequest request, CancellationToken cancellationToken)
        {
            await Handle(request, cancellationToken).ConfigureAwait(false);
            return Unit.Value;
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
        Task<Unit> IRequestHandlerAsync<TRequest, Unit>.HandleAsync(TRequest request, CancellationToken cancellationToken)
        {
            Handle(request);
            return Unit.Task;
        }

        protected abstract void Handle(TRequest request);
    }
}
