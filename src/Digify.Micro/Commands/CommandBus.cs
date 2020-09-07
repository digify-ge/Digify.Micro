using Autofac;
using System;
using System.Threading.Tasks;

namespace Digify.Micro.Commands
{
    public interface ICommandBusAsync
    {
        Task<TResult> ExecuteAsync<TCommand, TResult>(TCommand command) where TCommand : ICommand;
        Task ExecuteAsync<TCommand>(TCommand command) where TCommand : ICommand;
    }

    public class CommandBusAsync : ICommandBusAsync
    {
        private readonly ILifetimeScope context;
        public CommandBusAsync(ILifetimeScope context) => this.context = context ?? throw new ArgumentNullException(nameof(context));

        public Task<TResult> ExecuteAsync<TCommand, TResult>(TCommand command) where TCommand : ICommand
        {
            if (command == null)
                throw new ArgumentNullException($"Command shouldn't be null");

            Task<TResult> result = null;

            using (var scope = context.BeginLifetimeScope())
            {
                var handler = scope.Resolve<ICommandHandlerAsync<TCommand, TResult>>()
                    ?? throw new InvalidOperationException($"Handler not found for specified command");

                result = handler.HandleAsync(command);
            }

            return result;
        }

        public Task ExecuteAsync<TCommand>(TCommand command) where TCommand : ICommand
        {
            if (command == null)
                throw new ArgumentNullException($"Command shouldn't be null");

            Task result = null;

            using (var scope = context.BeginLifetimeScope())
            {
                var handler = scope.Resolve<ICommandHandlerAsync<TCommand>>()
                    ?? throw new InvalidOperationException($"Handler not found for specified command");

                result = handler.HandleAsync(command);
            }

            return result;
        }
    }
}