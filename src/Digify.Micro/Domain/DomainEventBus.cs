using Autofac;
using Digify.Micro.Behaviors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Digify.Micro.Domain
{
    public interface IDomainEventBusAsync
    {
        Task ExecuteAsync<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent;
        Task ExecuteAsync<TDomainEvent>(IEnumerable<TDomainEvent> domainEvents) where TDomainEvent : IDomainEvent;
    }

    public class DomainEventBusAsync : IDomainEventBusAsync
    {
        private readonly ILifetimeScope context;

        public DomainEventBusAsync(ILifetimeScope context) => this.context = context ?? throw new ArgumentNullException(nameof(context));

        public async Task ExecuteAsync<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent
        {
            if (domainEvent == null)
                throw new ArgumentNullException($"Domain event shouldn't be null");

            try
            {
                using (var scope = context.BeginLifetimeScope())
                {
                    var validationHandler = scope.ResolveOptional<MicroHandlerValidator<TDomainEvent>>();
                    if (validationHandler != null) await validationHandler.Handle(domainEvent);

                    var eventHandlerType = typeof(IDomainEventHandlerAsync<>).MakeGenericType(domainEvent.GetType());
                    var listOfType = typeof(IEnumerable<>).MakeGenericType(eventHandlerType);
                    var handlers = (IEnumerable<object>)scope.ResolveOptional(listOfType);

                    if (handlers != null && handlers.Any())
                        Parallel.ForEach(handlers,
                        async (handler) => await (Task)eventHandlerType.GetMethod("HandleAsync").Invoke(handler, new object[] { domainEvent })
                        );
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task ExecuteAsync<TDomainEvent>(IEnumerable<TDomainEvent> domainEvents) where TDomainEvent : IDomainEvent
        {
            if (domainEvents == null || !domainEvents.Any())
                throw new ArgumentNullException($"Domain events shouldn't be empty");

            try
            {
                using (var scope = context.BeginLifetimeScope())
                {
                    foreach (var domainEvent in domainEvents)
                    {
                        if (domainEvent == null)
                            throw new ArgumentNullException($"Domain event shouldn't be null");

                        var validationHandler = scope.ResolveOptional<MicroHandlerValidator<TDomainEvent>>();
                        if (validationHandler != null) await validationHandler.Handle(domainEvent);

                        var eventHandlerType = typeof(IDomainEventHandlerAsync<>).MakeGenericType(domainEvent.GetType());
                        var listOfType = typeof(IEnumerable<>).MakeGenericType(eventHandlerType);
                        var handlers = (IEnumerable<object>)scope.ResolveOptional(listOfType);

                        if (handlers != null && handlers.Any())
                            Parallel.ForEach(handlers,
                            async (handler) => await (Task)eventHandlerType.GetMethod("HandleAsync").Invoke(handler, new object[] { domainEvent })
                            );
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
