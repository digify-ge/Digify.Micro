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
            var rsss = await rs.ExecuteAsync(new UserRegisterCommand());

            //Publish Event
            //await rs.Publish(new DomainEvent());

            Console.WriteLine("Hello World!");
        }
    }

    public class UserCommand : IRequest<int>
    {

    }
    public class UserRegisterCommand : IRequest<Task>
    {

    }
    public class DomainEvent : IDomainEvent
    {
        public Guid AggregateRootId { get; }

        public long Version { get; set; }
    }
    public class DomainEventHandler : IRequestHandlerAsync<DomainEvent>
    {
        public Task HandleAsync(DomainEvent command, CancellationToken token)
        {
            Debug.WriteLine(nameof(DomainEvent));
            return Task.CompletedTask;
        }
    }

    public class CommandHandler : IRequestHandlerAsync<UserCommand, int>,
                                  IRequestHandlerAsync<UserRegisterCommand>
    {
        public Task<int> HandleAsync(UserCommand command, CancellationToken token)
        {
            Debug.WriteLine(nameof(UserCommand));
            return Task.FromResult(5);
        }

        public Task HandleAsync(UserRegisterCommand command, CancellationToken token)
        {
            Debug.WriteLine(nameof(UserRegisterCommand));
            return Task.CompletedTask;
        }
    }
}
