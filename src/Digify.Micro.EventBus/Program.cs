using Autofac;
using Autofac.Extensions.DependencyInjection;
using Digify.Micro.Commands;
using Digify.Micro.Domain;
using Digify.Micro.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Digify.Micro
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IServiceCollection svc = new ServiceCollection();
            var container = new ContainerBuilder();
            svc.AddMicro(container);
            svc.AddTransient<IBusActionFilter, BusActionFilter>();
            container.Populate(svc);
            var newSvc = new AutofacServiceProvider(container.Build());
            var rs = newSvc.GetService<IEventBusAsync>();
            //Sendd Command with result
            var result = await rs.ExecuteAsync(new UserCommand());
            //Send Command without result
            await rs.ExecuteAsync(new UserRegisterCommand());

            //Publish Event
            await rs.Publish(new DomainEvent());

            Console.WriteLine("Hello World!");
        }
    }
    public class BusActionFilter : IBusActionFilter
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
    public class UserCommand : QueryRequest<int>
    {

    }
    public class UserRegisterCommand : IRequest, ICommand
    {

    }
    public class DomainEvent : IDomainEvent
    {
        public Guid AggregateRootId { get; }

        public long Version { get; set; }
    }
    public class DomainEventHandler : IRequestHandlerAsync<DomainEvent>
    {
        public Task HandleAsync(DomainEvent command)
        {
            Debug.WriteLine(nameof(DomainEvent));
            return Task.CompletedTask;
        }
    }

    public class CommandHandler : IRequestHandlerAsync<UserCommand, int>,
                                  IRequestHandlerAsync<UserRegisterCommand>
    {
        public Task<int> HandleAsync(UserCommand command)
        {
            Debug.WriteLine(nameof(UserCommand));
            return Task.FromResult(5);
        }

        public Task HandleAsync(UserRegisterCommand command)
        {
            Debug.WriteLine(nameof(UserRegisterCommand));
            return Task.CompletedTask;
        }
    }
}
