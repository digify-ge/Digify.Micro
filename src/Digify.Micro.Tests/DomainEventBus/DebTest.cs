using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Digify.Micro.Domain;
using Digify.Micro.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Digify.Micro.Tests.DomainEventBus
{
    public class DebTest
    {
        [Fact]
        public async Task Test()
        {
            var service = new ServiceCollection();
            service.AddMicroCore();
            var ctr = new ContainerBuilder();
            ctr.Populate(service);
            IServiceProvider r = new AutofacServiceProvider(ctr.Build());
            var ltscope = r.GetService<ILifetimeScope>();
            using (var scope = ltscope.BeginLifetimeScope())
            {
                var domainEventBus = scope.Resolve<IDomainEventBusAsync>();
                await domainEventBus.ExecuteAsync(new TestDomainEvent("Givi"));
            }
        }
    }
}
