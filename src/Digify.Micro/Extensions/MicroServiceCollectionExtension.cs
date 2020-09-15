using Autofac;
using Digify.Micro.Behaviors;
using Digify.Micro.Commands;
using Digify.Micro.Domain;
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
        private static IEnumerable<Assembly> _assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(e => !e.IsDynamic);
        /// <summary>
        /// Adding only Commands, Queries, Domains 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static ContainerBuilder AddMicroCore(this IServiceCollection services)
        {
            var builder = new ContainerBuilder();
            builder.AddCommands(services).AddQueries(services).AddDomains(services);
            return builder;
        }
        /// <summary>
        /// Adding Commands, Queries, Domains and Fluent Validation for CQRS.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static ContainerBuilder AddMicro(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblies(_assemblies);

            var builder = new ContainerBuilder();
            builder.AddCommands(services).AddQueries(services).AddDomains(services);
            builder.RegisterGeneric(typeof(CommandValidator<>));
            return builder;
        }
        private static ContainerBuilder AddCommands(this ContainerBuilder container, IServiceCollection services)
        {
            services.AddTransient<ICommandBusAsync, CommandBusAsync>();
            var exportedTypes = _assemblies.SelectMany(e => e.ExportedTypes)
            .Where(e => e.GetTypeInfo().ImplementedInterfaces.Any(x => x.IsGenericType
            && (x.GetGenericTypeDefinition() == typeof(ICommandHandlerAsync<,>) || x.GetGenericTypeDefinition() == typeof(ICommandHandlerAsync<>))))
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
            var exportedTypes = _assemblies.SelectMany(e => e.ExportedTypes)
            .Where(e => e.GetTypeInfo().ImplementedInterfaces.Any(x => x.IsGenericType
            && x.GetGenericTypeDefinition() == typeof(Queries.IQueryHandlerAsync<,>)))
            .ToList();
            foreach (var types in exportedTypes)
            {
                container.RegisterAssemblyTypes(types.Assembly).AsClosedTypesOf(typeof(Queries.IQueryHandlerAsync<,>));
            }
            return container;
        }
        private static ContainerBuilder AddDomains(this ContainerBuilder container, IServiceCollection services)
        {
            services.AddSingleton<IDomainEventBusBulkAsync, DomainEventBusBulkAsync>();

            var exportedTypes = _assemblies.SelectMany(e => e.ExportedTypes)
            .Where(e => e.GetTypeInfo().ImplementedInterfaces.Any(x =>
             x == typeof(Domain.IDomainEventHandlerAsync<>)))
            .ToList();
            foreach (var types in exportedTypes)
            {
                container.RegisterAssemblyTypes(types.Assembly).AsClosedTypesOf(typeof(Domain.IDomainEventHandlerAsync<>));
            }
            return container;
        }
    }
}
