using Autofac;
using Digify.Micro.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Digify.Micro
{
    public interface IRequest
    {
    }
    public interface IRequest<TResponse>
    {
    }
    public interface IEventBusAsync
    {
        Task<TResult> ExecuteAsync<TResult>(IRequest<TResult> request);
        Task ExecuteAsync<TRequest>(TRequest request) where TRequest : IRequest;
    }

    public interface IRequestHandlerAsync<TRequest, TResult> where TRequest : IRequest<TResult>
    {
        Task<TResult> HandleAsync(TRequest command);
    }
    public interface IRequestHandlerAsync<in TRequest> where TRequest : IRequest
    {
        Task HandleAsync(TRequest command);
    }

    public class EventBus : IEventBusAsync
    {
        private readonly ILifetimeScope context;
        public EventBus(ILifetimeScope context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<TResult> ExecuteAsync<TResult>(IRequest<TResult> request)
        {
            if (request == null)
                throw new ArgumentNullException($"Command shouldn't be null");

            TResult result;

            using (var scope = context.BeginLifetimeScope())
            {
                var validatorHandlerType = typeof(MicroHandlerValidator<>).MakeGenericType(request.GetType());
                var validationHandler = scope.ResolveOptional(validatorHandlerType);
                if (validationHandler != null)
                    validatorHandlerType.GetMethod("Handle").Invoke(validationHandler, new object[] { request });

                var eventHandlerType = typeof(IRequestHandlerAsync<,>).MakeGenericType(request.GetType(), typeof(TResult));
                var handler = scope.ResolveOptional(eventHandlerType);
                result = await (Task<TResult>)eventHandlerType.GetMethod("HandleAsync").Invoke(handler, new object[] { request });
            }

            return result;
        }

        public async Task ExecuteAsync<TRequest>(TRequest request) where TRequest : IRequest
        {
            if (request == null)
                throw new ArgumentNullException($"Command shouldn't be null");

            using (var scope = context.BeginLifetimeScope())
            {
                var validationHandler = scope.ResolveOptional<MicroHandlerValidator<TRequest>>();
                if (validationHandler != null) await validationHandler.Handle(request);

                var handler = scope.Resolve<IRequestHandlerAsync<TRequest>>()
                    ?? throw new InvalidOperationException($"Handler not found for specified command");

                await handler.HandleAsync(request);
            }
        }
    }
}
