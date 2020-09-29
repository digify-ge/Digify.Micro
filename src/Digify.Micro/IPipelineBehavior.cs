using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Digify.Micro
{
    public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();
    public delegate Task RequestHandlerDelegate();

    public interface IPipelineBehavior<in TRequest, TResponse> where TRequest : notnull
    {
        Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next);
    }
}
