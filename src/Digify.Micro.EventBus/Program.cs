using Digify.Micro.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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
            //await rs.ExecuteAsync(new UserRegisterCommand());

            await rs.ExecutesAsync(new List<ICommand>()
            {
                new SendOtp(),
                new UserRegisterCommand(),
            });


            //Publish Event
            await rs.PublishEvents(new List<IDomainEvent>(){
                (IDomainEvent)new DomainEvent()
            });

            Console.WriteLine("Hello World!");
        }
    }

    public class UserCommand : ICommand<int>
    {

    }
    public class UserRegisterCommand : ICommand
    {

    }
    public class SendOtp : ICommand
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
    public class CommandHandler2 : IRequestHandlerAsync<UserRegisterCommand>, IRequestHandlerAsync<SendOtp>
    {
        public Task HandleAsync(UserRegisterCommand request, CancellationToken cancellationToken)
        {
            Debug.WriteLine(nameof(CommandHandler2) + " " + nameof(UserRegisterCommand));
            return Task.CompletedTask;
        }

        public Task HandleAsync(SendOtp request, CancellationToken cancellationToken)
        {
            Thread.Sleep(4000);
            Debug.WriteLine(nameof(SendOtp));
            return Task.CompletedTask;
        }
    }
    public class CommandHandler : IRequestHandlerAsync<UserCommand, int>
    {
        public Task<int> HandleAsync(UserCommand request, CancellationToken cancellationToken)
        {
            Debug.WriteLine(nameof(CommandHandler) + " " + nameof(UserCommand));
            return Task.FromResult(5);
        }
    }
}
