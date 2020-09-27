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
        public static ContainerBuilder AddMicroCore(this IServiceCollection services, ContainerBuilder builder, MicroSettings settings = default)
        {
            MicroSettings microSettings = settings;
            if (settings == null)
                microSettings = new MicroSettings();

            services.AddSingleton(microSettings);

            builder.AddCommands(services).AddQueries(services).AddDomains(services);
            return builder;
        }

        /// <summary>
        /// Adding Commands, Queries, Domains and Fluent Validation for CQRS.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static ContainerBuilder AddMicro(this IServiceCollection services, ContainerBuilder builder, MicroSettings settings = default)
        {
            services.AddValidatorsFromAssemblies(_assemblies);

            MicroSettings microSettings = settings;
            if (settings == null)
                microSettings = new MicroSettings();

            services.AddSingleton(microSettings);

            builder.AddCommands(services).AddQueries(services).AddDomains(services).AddRequestHandlers(services);
            builder.RegisterGeneric(typeof(MicroHandlerValidator<>));
            return builder;
        }
        private static ContainerBuilder AddRequestHandlers(this ContainerBuilder container, IServiceCollection services)
        {
            services.AddTransient<IEventBusAsync, EventBus>();
            var exportedTypes = _assemblies.SelectMany(e => e.ExportedTypes)
            .Where(e => e.GetTypeInfo().ImplementedInterfaces.Any(x => x.IsGenericType
            && (x.GetGenericTypeDefinition() == typeof(IRequestHandlerAsync<,>) || x.GetGenericTypeDefinition() == typeof(IRequestHandlerAsync<>))))
            .ToList();

            foreach (var assembly in exportedTypes.Select(e => e.Assembly).Distinct())
            {
                container.RegisterAssemblyTypes(assembly).AsClosedTypesOf(typeof(IRequestHandlerAsync<>));
                container.RegisterAssemblyTypes(assembly).AsClosedTypesOf(typeof(IRequestHandlerAsync<,>));
            }
            return container;
        }
        private static ContainerBuilder AddCommands(this ContainerBuilder container, IServiceCollection services)
        {
            services.AddTransient<ICommandBusAsync, CommandBusAsync>();
            var exportedTypes = _assemblies.SelectMany(e => e.ExportedTypes)
            .Where(e => e.GetTypeInfo().ImplementedInterfaces.Any(x => x.IsGenericType
            && (x.GetGenericTypeDefinition() == typeof(ICommandHandlerAsync<,>) || x.GetGenericTypeDefinition() == typeof(ICommandHandlerAsync<>))))
            .ToList();

            foreach (var assembly in exportedTypes.Select(e => e.Assembly).Distinct())
            {
                container.RegisterAssemblyTypes(assembly).AsClosedTypesOf(typeof(ICommandHandlerAsync<>));
                container.RegisterAssemblyTypes(assembly).AsClosedTypesOf(typeof(ICommandHandlerAsync<,>));
            }
            return container;
        }

        private static ContainerBuilder AddQueries(this ContainerBuilder container, IServiceCollection services)
        {
            services.AddTransient<IQueryBusAsync, QueryBusAsync>();
            var exportedTypes = _assemblies.SelectMany(e => e.ExportedTypes)
            .Where(e => e.GetTypeInfo().ImplementedInterfaces.Any(x => x.IsGenericType
            && x.GetGenericTypeDefinition() == typeof(IQueryHandlerAsync<,>)))
            .ToList();

            foreach (var assembly in exportedTypes.Select(e => e.Assembly).Distinct())
            {
                container.RegisterAssemblyTypes(assembly).AsClosedTypesOf(typeof(IQueryHandlerAsync<,>));
            }
            return container;
        }

        private static ContainerBuilder AddDomains(this ContainerBuilder container, IServiceCollection services)
        {
            services.AddSingleton<IDomainEventBusAsync, DomainEventBusAsync>();

            var exportedTypes = _assemblies.SelectMany(e => e.ExportedTypes)
            .Where(e => e.GetTypeInfo().ImplementedInterfaces.Any(x => x.IsGenericType
            && x.GetGenericTypeDefinition() == typeof(IDomainEventHandlerAsync<>)))
            .ToList();

            foreach (var assembly in exportedTypes.Select(e => e.Assembly).Distinct())
            {
                container.RegisterAssemblyTypes(assembly).AsClosedTypesOf(typeof(IDomainEventHandlerAsync<>));
            }
            return container;
        }
    }
}
