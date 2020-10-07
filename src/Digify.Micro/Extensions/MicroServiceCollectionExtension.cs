using Digify.Micro.Internal;
using Digify.Micro.Processors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Digify.Micro.Extensions
{
    public static class MicroServiceCollectionExtension
    {
        private static IEnumerable<Assembly> _assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(e => !e.IsDynamic);

        /// <summary>
        /// Adding Commands, Queries, Domains and Fluent Validation for CQRS.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMicro(this IServiceCollection services, MicroSettings settings = default)
        {
            services.AddValidatorsFromAssemblies(_assemblies);

            MicroSettings microSettings = settings;
            if (settings == null)
                microSettings = new MicroSettings();

            services.AddSingleton(microSettings);
            services.AddRequiredServices();
            services.AddRequestHandlers();
            return services;
        }
        private static IServiceCollection AddRequestHandlers(this IServiceCollection services)
        {
            services.AddTransient<IBusAsync, BusAsync>();
            var exportedTypes = GetExportedTypes(typeof(IRequestHandlerAsync<,>), typeof(IRequestHandlerAsync<>), typeof(IDomainEventHandlerAsync<>))
                .GroupBy(e => e.GetTypeInfo().ImplementedInterfaces.Select(e => e.GetGenericTypeDefinition()).First())
            .Distinct()
            .ToList();


            foreach (var group in exportedTypes)
            {
                foreach (var @type in group)
                {
                    services.Scan(scan => scan
                                           .AddTypes(@type)
                                           .AddClasses(classes => classes.AssignableTo(group.Key))
                                           .AsImplementedInterfaces()
                                           .WithTransientLifetime());
                }

            }
            return services;
        }

        private static IServiceCollection AddRequiredServices(this IServiceCollection services)
        {
            services.AddTransient<ServiceScope>(p => p.GetService);

            var exportedTypes = GetExportedTypes(typeof(IPipelineBehavior<,>));
            foreach (var @type in exportedTypes)
            {
                services.Scan(scan => scan
                                         .AddTypes(@type)
                                         .AddClasses(classes => classes.AssignableTo(@type))
                                         .AsImplementedInterfaces()
                                         .WithTransientLifetime());
            }
            return services;
        }
        private static IEnumerable<Type> GetExportedTypes(params Type[] type)
        {
            var exportedTypes = _assemblies.SelectMany(e => e.ExportedTypes)
          .Where(e => e.GetTypeInfo().ImplementedInterfaces.Any(x => x.IsGenericType
          && type.Contains(x.GetGenericTypeDefinition())))
          .Distinct()
          .ToList();
            return exportedTypes;
        }
    }
}
