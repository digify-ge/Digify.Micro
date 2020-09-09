﻿using Autofac;
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
        private readonly IPipelineBehavior pipeline;
        private readonly ILifetimeScope context;
        public CommandBusAsync(ILifetimeScope context, IPipelineBehavior pipeline)
        {
            this.pipeline = pipeline;
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<TResult> ExecuteAsync<TCommand, TResult>(TCommand command) where TCommand : ICommand
        {
            if (command == null)
                throw new ArgumentNullException($"Command shouldn't be null");

            TResult result;
            await pipeline.Handle<TCommand, TResult>(command);
            using (var scope = context.BeginLifetimeScope())
            {
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
                var handler = scope.Resolve<ICommandHandlerAsync<TCommand>>()
                    ?? throw new InvalidOperationException($"Handler not found for specified command");

                await handler.HandleAsync(command);
            }
        }
    }
}