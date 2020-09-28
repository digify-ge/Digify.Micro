using Autofac;
using Digify.Micro.Behaviors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public interface QueryRequest<TResponse> : IRequest<TResponse>
    {

    }
    public interface IEventBusAsync
    {
        Task<TResult> ExecuteAsync<TResult>(IRequest<TResult> request);
        Task ExecuteAsync<TRequest>(TRequest request) where TRequest : IRequest;
        Task Publish<TRequest>(TRequest request) where TRequest : IRequest;
    }

    public interface IRequestHandlerAsync<in TRequest, TResult> where TRequest : IRequest<TResult>
    {
        Task<TResult> HandleAsync(TRequest command);
    }
    public interface IRequestHandlerAsync<in TRequest> where TRequest : IRequest
    {
        Task HandleAsync(TRequest command);
    }
    public class BaseBusActionFilter : IBusActionFilter
    {
        public Task OnExecuted<TResponse>(TResponse result)
        {
            return Task.CompletedTask;
        }

        public Task OnExecuting()
        {
            return Task.CompletedTask;
        }
    }
    public interface IBusActionFilter
    {
        Task OnExecuting();
        Task OnExecuted<TResponse>(TResponse result);
    }
    public interface IPreProcess
    {
        Task ProcessAsync<TRequest>(TRequest request);
    }
    public interface IPostProcess
    {
        Task ProcessAsync<TRequest, TResponse>(TRequest request, TResponse result);
    }
    public class PreProcessPipeline : IPreProcess
    {
        public Task ProcessAsync<TRequest>(TRequest request)
        {
            if (request == null)
                throw new ArgumentNullException($"Command shouldn't be null");
            return Task.CompletedTask;
        }
    }
    public class FluentValidationPipeline : IPreProcess
    {
        private readonly ILifetimeScope _context;

        public FluentValidationPipeline(ILifetimeScope context)
        {
            _context = context;
        }
        public async Task ProcessAsync<TRequest>(TRequest request)
        {
            using (var scope = _context.BeginLifetimeScope())
            {
                var validatorHandlerType = typeof(MicroHandlerValidator<>).MakeGenericType(request.GetType());
                var validationHandler = scope.ResolveOptional(validatorHandlerType);
                if (validationHandler != null)
                    await(Task)validatorHandlerType.GetMethod("Handle").Invoke(validationHandler, new object[] { request });
            }
        }
    }

    public class PostProcessPipeline : IPostProcess
    {
        public Task ProcessAsync<TRequest, TResponse>(TRequest request, TResponse result)
        {

            return Task.CompletedTask;
        }
    }
    public class EventBus : IEventBusAsync
    {
        private readonly ILifetimeScope context;
        private readonly ICollection<IPreProcess> preProcesses;
        private readonly ICollection<IPostProcess> postProcesses;
        private readonly ILogger<EventBus> logger;

        public EventBus(ILifetimeScope context,
            ICollection<IPreProcess> preprocesses,
            ICollection<IPostProcess> postProcesses
            )

        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.preProcesses = preprocesses;
            this.postProcesses = postProcesses;
        }
        public async Task<TResult> ExecuteAsync<TResult>(IRequest<TResult> request)
        {
            foreach (var process in preProcesses)
            {
                await process.ProcessAsync(request);
            }
            TResult result;

            using (var scope = context.BeginLifetimeScope())
            {
                var eventHandlerType = typeof(IRequestHandlerAsync<,>).MakeGenericType(request.GetType(), typeof(TResult));
                var handler = scope.ResolveOptional(eventHandlerType);
                result = await (Task<TResult>)eventHandlerType.GetMethod("HandleAsync").Invoke(handler, new object[] { request });
            }

            foreach (var process in postProcesses)
            {
                await process.ProcessAsync(request, result);
            }

            return result;
        }
        public async Task Publish<TRequest>(TRequest request) where TRequest : IRequest
        {
            if (request == null)
                throw new ArgumentNullException($"Command shouldn't be null");

            using (var scope = context.BeginLifetimeScope())
            {
                var validatorHandlerType = typeof(MicroHandlerValidator<>).MakeGenericType(request.GetType());
                var validationHandler = scope.ResolveOptional(validatorHandlerType);
                if (validationHandler != null)
                    await (Task)validatorHandlerType.GetMethod("Handle").Invoke(validationHandler, new object[] { request });

                var eventHandlerType = typeof(IRequestHandlerAsync<>).MakeGenericType(request.GetType());
                var listOfType = typeof(IEnumerable<>).MakeGenericType(eventHandlerType);
                var handlers = (IEnumerable<object>)scope.ResolveOptional(listOfType);

                if (handlers != null && handlers.Any())
                    Parallel.ForEach(handlers,
                        async (handler) => await (Task)eventHandlerType.GetMethod("HandleAsync").Invoke(handler, new object[] { request })
                    );
            }
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
