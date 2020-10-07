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
        private readonly ServiceScope _serviceFactory;

        public BusAsync(ServiceScope serviceFactory)

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
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            var requestType = request.GetType();
            var requestInterfaceType = requestType
                .GetInterfaces()
                .FirstOrDefault(i => i == typeof(IRequest));
            var isValidRequest = requestInterfaceType != null;

            if (!isValidRequest)
            {
                throw new ArgumentException($"{nameof(request)} does not implement ${nameof(IRequest)}");
            }

            var handler = (NonReturnableHandlerWrapper)Activator.CreateInstance(typeof(NonReturnableHandlerWrapperImpl<>).MakeGenericType(requestType));

            return handler.Handle(request, cancellationToken, _serviceFactory);
        }

        public Task ExecutesAsync<TRequest>(IEnumerable<TRequest> requests, CancellationToken cancellationToken = default) where TRequest : IRequest
        {
            Parallel.ForEach(requests, request =>
            {
                ExecuteAsync(request, cancellationToken)
                .ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        throw t.Exception;
                    }
                    return t;
                }, cancellationToken);
            });
            return Task.CompletedTask;
        }

        public Task PublishEvent<TRequest>(TRequest request, CancellationToken cancellationToken) where TRequest : IDomainEvent
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            var requestType = request.GetType();
            var requestInterfaceType = requestType
                .GetInterfaces()
                .FirstOrDefault(i => i == typeof(IDomainEvent));
            var isValidRequest = requestInterfaceType != null;

            if (!isValidRequest)
            {
                throw new ArgumentException($"{nameof(request)} does not implement ${nameof(IRequest)}");
            }

            var handler = (DomainEventHandlerWrapper)Activator.CreateInstance(typeof(DomainEventHandlerWrapperImpl<>).MakeGenericType(requestType));

            return handler.Handle(request, cancellationToken, _serviceFactory);
        }

        public Task PublishEvents<TRequest>(IEnumerable<TRequest> events, CancellationToken cancellationToken = default) where TRequest : IDomainEvent
        {
            Parallel.ForEach(events, @event =>
            {
                PublishEvent(@event, cancellationToken)
                .ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        throw t.Exception;
                    }
                    return t;
                }, cancellationToken);
            });
            return Task.CompletedTask;
        }
    }
}
