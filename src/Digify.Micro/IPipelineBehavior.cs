using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Digify.Micro
{
    public interface IPipelineBehavior
    {
        Task<TResponse> Handle<TRequest, TResponse>(TRequest request);
    }
}
