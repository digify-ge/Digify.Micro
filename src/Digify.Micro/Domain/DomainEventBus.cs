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
    }

    public interface IDomainEventBusBulkAsync
    {
        Task ExecuteBulkAsync<TDomainEvent>(IEnumerable<TDomainEvent> domainEvents) where TDomainEvent : IDomainEvent;
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

                    var handlers = scope.Resolve<IEnumerable<IDomainEventHandlerAsync<TDomainEvent>>>()
                        ?? throw new InvalidOperationException($"Handler not found for specified Domain event");

                    Parallel.ForEach(handlers, async (handler) => await handler.HandleAsync(domainEvent));
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }


    public class DomainEventBusBulkAsync : IDomainEventBusBulkAsync
    {
        private readonly ILifetimeScope context;

        public DomainEventBusBulkAsync(ILifetimeScope context) => this.context = context ?? throw new ArgumentNullException(nameof(context));

        public async Task ExecuteBulkAsync<TDomainEvent>(IEnumerable<TDomainEvent> domainEvents) where TDomainEvent : IDomainEvent
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

                        var handlers = scope.Resolve<IEnumerable<IDomainEventHandlerAsync<TDomainEvent>>>()
                            ?? throw new InvalidOperationException($"Handler not found for specified Domain event");

                        Parallel.ForEach(handlers, async (handler) => await handler.HandleAsync(domainEvent));
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
