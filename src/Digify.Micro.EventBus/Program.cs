using Digify.Micro.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Digify.Micro
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IServiceCollection svc = new ServiceCollection();
            svc.AddMicro();
            var rs = svc.BuildServiceProvider().GetService<IBusAsync>();
            //Sendd Command with result
            var result = await rs.ExecuteAsync(new UserCommand());
            //Send Command without result
            await rs.ExecuteAsync(new UserRegisterCommand());

            //Publish Event
            await rs.PublishEvent(new DomainEvent());

            Console.WriteLine("Hello World!");
        }
    }

    public class UserCommand : ICommand<int>
    {

    }
    public class UserRegisterCommand : ICommand
    {

    }
    public class DomainEvent : IDomainEvent
    {
        public Guid AggregateRootId { get; }

        public long Version { get; set; }
    }
    public class DomainEventHandler : IDomainEventHandlerAsync<DomainEvent>
    {
        public Task HandleAsync(DomainEvent domainEvent, CancellationToken cancellationToken)
        {
            Debug.WriteLine(nameof(DomainEventHandler));
            return Task.CompletedTask;
        }
    }
    public class DomainEventHandler2 : IDomainEventHandlerAsync<DomainEvent>
    {
        public Task HandleAsync(DomainEvent request, CancellationToken cancellationToken)
        {
            Debug.WriteLine(nameof(DomainEventHandler2));
            return Task.CompletedTask;
        }
    }
    public class CommandHandler2 : IRequestHandlerAsync<UserRegisterCommand>
    {
        public Task HandleAsync(UserRegisterCommand request, CancellationToken cancellationToken)
        {
            Debug.WriteLine(nameof(CommandHandler2));
            return Task.CompletedTask;
        }
    }
    public class CommandHandler : IRequestHandlerAsync<UserCommand, int>,
                                  IRequestHandlerAsync<UserRegisterCommand>
    {
        public Task<int> HandleAsync(UserCommand request, CancellationToken cancellationToken)
        {
            Debug.WriteLine(nameof(CommandHandler) + " " + nameof(UserCommand));
            return Task.FromResult(5);
        }

        public Task HandleAsync(UserRegisterCommand request, CancellationToken cancellationToken)
        {
            Debug.WriteLine(nameof(CommandHandler) + " " + nameof(UserRegisterCommand));
            return Task.CompletedTask;
        }
    }
}
