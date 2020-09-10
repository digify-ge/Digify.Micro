using Autofac;
using Digify.Micro.Behaviors;
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
        public CommandBusAsync(ILifetimeScope context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<TResult> ExecuteAsync<TCommand, TResult>(TCommand command) where TCommand : ICommand
        {
            if (command == null)
                throw new ArgumentNullException($"Command shouldn't be null");

            TResult result;

            using (var scope = context.BeginLifetimeScope())
            {
                var validationHandler = scope.ResolveOptional<CommandValidator<TCommand>>();
                if (validationHandler != null) await validationHandler.Handle(command);

                var handler = scope.Resolve<ICommandHandlerAsync<TCommand, TResult>>()
                    ?? throw new InvalidOperationException($"Handler not found for specified command");

                result = await handler.HandleAsync(command);
            }

            return result;
        }

        public async Task ExecuteAsync<TCommand>(TCommand command) where TCommand : ICommand
        {
            if (command == null)
                throw new ArgumentNullException($"Command shouldn't be null");

            using (var scope = context.BeginLifetimeScope())
            {
                var validationHandler = scope.ResolveOptional<ICommandValidationBehaviour<TCommand>>();
                if (validationHandler != null) await validationHandler.Handle(command);

                var handler = scope.Resolve<ICommandHandlerAsync<TCommand>>()
                    ?? throw new InvalidOperationException($"Handler not found for specified command");

                await handler.HandleAsync(command);
            }
        }
    }
}