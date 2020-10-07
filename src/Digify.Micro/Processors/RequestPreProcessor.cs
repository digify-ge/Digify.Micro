using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Digify.Micro.Processors
{
    public class RequestPreProcessorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
         where TRequest : notnull
    {
        private readonly IEnumerable<IRequestPreProcessor<TRequest>> _preProcessors;

        public RequestPreProcessorBehavior(IEnumerable<IRequestPreProcessor<TRequest>> preProcessors)
        {
            _preProcessors = preProcessors;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            foreach (var processor in _preProcessors.OrderBy(e => e.Order))
            {
                await processor.Process(request, cancellationToken).ConfigureAwait(false);
            }

            return await next().ConfigureAwait(false);
        }
    }
}
