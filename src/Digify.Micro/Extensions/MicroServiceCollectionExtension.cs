using Autofac;
using Digify.Micro.Behaviors;
using Digify.Micro.Commands;
using Digify.Micro.Queries;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Digify.Micro.Extensions
{
    public static class MicroServiceCollectionExtension
    {
        public static IServiceCollection AddMicro(this IServiceCollection services, ContainerBuilder builder)
        {
            services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());

            builder.AddCommands(services).AddQueries(services);
            builder.RegisterGeneric(typeof(CommandValidator<>));
            return services;
        }
        private static ContainerBuilder AddCommands(this ContainerBuilder container, IServiceCollection services)
        {
            services.AddTransient<ICommandBusAsync, CommandBusAsync>();
            var exportedTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(e => e.ExportedTypes)
            .Where(e => e.GetTypeInfo().ImplementedInterfaces.Any(x => x.IsGenericType
            && x.GetGenericTypeDefinition() == typeof(ICommandHandlerAsync<,>)))
            .ToList();
            foreach (var types in exportedTypes)
            {
                container.RegisterAssemblyTypes(types.Assembly).AsClosedTypesOf(typeof(ICommandHandlerAsync<>));
                container.RegisterAssemblyTypes(types.Assembly).AsClosedTypesOf(typeof(ICommandHandlerAsync<,>));
            }
            return container;
        }
        private static ContainerBuilder AddQueries(this ContainerBuilder container, IServiceCollection services)
        {
            services.AddTransient<IQueryBusAsync, QueryBusAsync>();
            var exportedTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(e => e.ExportedTypes)
            .Where(e => e.GetTypeInfo().ImplementedInterfaces.Any(x => x.IsGenericType
            && x.GetGenericTypeDefinition() == typeof(Queries.IQueryHandlerAsync<,>)))
            .ToList();
            foreach (var types in exportedTypes)
            {
                container.RegisterAssemblyTypes(types.Assembly).AsClosedTypesOf(typeof(Queries.IQueryHandlerAsync<,>));
            }
            return container;
        }
    }
}
