using Autofac;
using Autofac.Extensions.DependencyInjection;
using Digify.Micro.Commands;
using Digify.Micro.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
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
            container.Populate(svc);
            var newSvc = new AutofacServiceProvider(container.Build());
            var rs = newSvc.GetService<IEventBusAsync>();
            var result = await rs.ExecuteAsync(new UserCommand());
            await rs.ExecuteAsync(new UserRegisterCommand());
            Console.WriteLine("Hello World!");
        }
    }
    public class UserCommand : IRequest<int>
    {

    }
    public class UserRegisterCommand : IRequest
    {

    }
    public class CommandHandler : IRequestHandlerAsync<UserCommand, int>,
        IRequestHandlerAsync<UserRegisterCommand>
    {
        public Task<int> HandleAsync(UserCommand command)
        {
            return Task.FromResult(5);
        }

        public Task HandleAsync(UserRegisterCommand command)
        {
            return Task.CompletedTask;
        }
    }
}
