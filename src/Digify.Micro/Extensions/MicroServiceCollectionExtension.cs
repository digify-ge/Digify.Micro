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
        /// Adding only Commands, Queries, Domains 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMicroCore(this IServiceCollection services, MicroSettings settings = default)
        {
            MicroSettings microSettings = settings;
            if (settings == null)
                microSettings = new MicroSettings();

            services.AddSingleton(microSettings);
            services.AddRequiredServices();
            services.AddRequestHandlers();
            return services;
        }

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
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(FluentValidationBehavior<,>));
            services.AddRequiredServices();
            services.AddRequestHandlers();
            return services;
        }
        private static IServiceCollection AddRequestHandlers(this IServiceCollection services)
        {
            services.AddTransient<IBusAsync, BusAsync>();
            var exportedTypes = _assemblies.SelectMany(e => e.ExportedTypes)
            .Where(e => e.GetTypeInfo().ImplementedInterfaces.Any(x => x.IsGenericType
            && (x.GetGenericTypeDefinition() == typeof(IRequestHandlerAsync<,>))))
            .Distinct()
            .ToList();


            foreach (var @type in exportedTypes)
            {
                services.Scan(scan => scan
                        .AddTypes(@type)
                        .AddClasses(classes => classes.AssignableTo(typeof(IRequestHandlerAsync<,>)))
                        .AsImplementedInterfaces()
                        .WithTransientLifetime());
            }
            return services;
        }

        private static IServiceCollection AddRequiredServices(this IServiceCollection services)
        {
            services.AddTransient<ServiceFactory>(p => p.GetService);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));
            return services;
        }
    }
}
