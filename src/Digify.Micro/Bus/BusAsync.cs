using Digify.Micro.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Digify.Micro
{
    public class BusAsync : IBusAsync
    {
        private readonly ServiceFactory _serviceFactory;

        public BusAsync(ServiceFactory serviceFactory)

        {
            this._serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
        }
        public Task<TResult> ExecuteAsync<TResult>(IRequest<TResult> request, CancellationToken cancellationToken)
        {
            var requestType = request.GetType();
            var handler = (RequestHandlerWrapper<TResult>)Activator.CreateInstance(typeof(RequestHandlerWrapperImpl<,>).MakeGenericType(requestType, typeof(TResult)));
            return handler.Handle(request, cancellationToken, _serviceFactory);
        }

        public Task ExecuteAsync<TRequest>(TRequest request, CancellationToken cancellationToken) where TRequest : IRequest
        {
            return PublishEvent(request, cancellationToken);
        }

        public Task PublishEvent<TRequest>(TRequest request, CancellationToken cancellationToken) where TRequest : IRequest
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            var requestType = request.GetType();
            var requestInterfaceType = requestType
                .GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));
            var isValidRequest = requestInterfaceType != null;

            if (!isValidRequest)
            {
                throw new ArgumentException($"{nameof(request)} does not implement ${nameof(IRequest)}");
            }

            var responseType = requestInterfaceType!.GetGenericArguments()[0];
            var handler = (NonReturnableHandlerWrapper)Activator.CreateInstance(typeof(NonReturnableHandlerWrapperImpl<>).MakeGenericType(requestType));

            return handler.Handle(request, cancellationToken, _serviceFactory, async (handlers, request, token) =>
            {
                foreach (var handler in handlers)
                {
                    await handler(request, cancellationToken).ConfigureAwait(false);
                }
            });
        }
    }
}
