using Autofac;
using System;
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
        Task ExecuteBulkAsync(IEnumerable<IDomainEvent> domainEvents);
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
                    var eventHandlerType = typeof(IDomainEventHandlerAsync<>).MakeGenericType(domainEvent.GetType());
                    var handler = scope.Resolve(eventHandlerType);
                    await (Task)eventHandlerType.GetMethod("HandleAsync").Invoke(handler, new object[] { domainEvent });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }


    public class DomainEventBusBulkAsync : IDomainEventBusBulkAsync
    {
        private readonly ILifetimeScope context;

        public DomainEventBusBulkAsync(ILifetimeScope context) => this.context = context ?? throw new ArgumentNullException(nameof(context));

        public async Task ExecuteBulkAsync(IEnumerable<IDomainEvent> domainEvents)
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

                        var eventHandlerType = typeof(IDomainEventHandlerAsync<>).MakeGenericType(domainEvent.GetType());
                        var handler = scope.Resolve(eventHandlerType);
                        await (Task)eventHandlerType.GetMethod("HandleAsync").Invoke(handler, new object[] { domainEvent });
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
