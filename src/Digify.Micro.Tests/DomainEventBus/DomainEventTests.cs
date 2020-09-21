using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Digify.Micro.Commands;
using Digify.Micro.Domain;
using Digify.Micro.Extensions;
using Digify.Micro.Tests.DomainEventBus.Handlers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Digify.Micro.Tests.DomainEventBus
{
    public class DomainEventTests
    {

        public static bool HandlerOnePassed = false;
        public static bool HandlerTwoPassed = false;


        [Fact]
        public async Task Executing_domain_event_which_has_multiple_implementations_should_work()
        {
            var service = new ServiceCollection();
            var ctr = new ContainerBuilder();
            service.AddMicroCore(ctr);
            ctr.Populate(service);
            IServiceProvider r = new AutofacServiceProvider(ctr.Build());
            var ltscope = r.GetService<ILifetimeScope>();
            using (var scope = ltscope.BeginLifetimeScope())
            {
                var domainEventBus = scope.Resolve<IDomainEventBusAsync>();
                await domainEventBus.ExecuteAsync(new TestDomainEvent("Givi"));
                HandlerOnePassed.Should().BeTrue();
                HandlerTwoPassed.Should().BeTrue();
            }
        }
    }
}
