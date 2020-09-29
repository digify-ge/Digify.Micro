using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Digify.Micro.Processors
{
    public class RequestPostProcessorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
       where TRequest : notnull
    {
        private readonly IEnumerable<IRequestPostProcessor<TRequest, TResponse>> _postProcessors;

        public RequestPostProcessorBehavior(IEnumerable<IRequestPostProcessor<TRequest, TResponse>> postProcessors)
        {
            _postProcessors = postProcessors;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var response = await next().ConfigureAwait(false);

            foreach (var processor in _postProcessors.OrderBy(e => e.Order))
            {
                await processor.Process(request, response, cancellationToken).ConfigureAwait(false);
            }

            return response;
        }
    }
}
