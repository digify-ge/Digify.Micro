using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Digify.Micro.Internal
{
    internal abstract class RequestHandlerBase
    {
        public abstract Task<object> Handle(object request, CancellationToken cancellationToken,
            ServiceScope serviceFactory);
       
    }
    internal abstract class RequestHandlerWrapper<TResponse> : RequestHandlerBase
    {
        public abstract Task<TResponse> Handle(IRequest<TResponse> request, CancellationToken cancellationToken,
            ServiceScope serviceFactory);
    }

    internal class RequestHandlerWrapperImpl<TRequest, TResponse> : RequestHandlerWrapper<TResponse>
        where TRequest : IRequest<TResponse>
    {
        public override Task<object> Handle(object request, CancellationToken cancellationToken,
            ServiceScope serviceFactory)
        {
            return Handle((IRequest<TResponse>)request, cancellationToken, serviceFactory)
                .ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        throw t.Exception;
                    }
                    return (object)t.Result;
                }, cancellationToken);
        }
       
        public override Task<TResponse> Handle(IRequest<TResponse> request, CancellationToken cancellationToken,
            ServiceScope serviceFactory)
        {
            Task<TResponse> Handler() => serviceFactory.GetHandler<IRequestHandlerAsync<TRequest, TResponse>>().HandleAsync((TRequest)request, cancellationToken);

            return serviceFactory
                .GetInstances<IPipelineBehavior<TRequest, TResponse>>()
                .Reverse()
                .Aggregate((RequestHandlerDelegate<TResponse>)Handler, (next, pipeline) => () => pipeline.Handle((TRequest)request, cancellationToken, next))();
        }
    }
}
