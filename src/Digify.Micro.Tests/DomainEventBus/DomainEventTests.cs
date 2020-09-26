using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Digify.Micro.Commands;
using Digify.Micro.Domain;
using Digify.Micro.Extensions;
using Digify.Micro.Tests.Application.CommandHandlers;
using Digify.Micro.Tests.Domain.DomainEvents;
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

        [Fact]
        public async Task Executing_domain_event_which_has_multiple_implementations_should_work()
        {
            var service = new ServiceCollection();
            var ctr = new ContainerBuilder();
            service.AddMicro(ctr);
            ctr.Populate(service);
            IServiceProvider r = new AutofacServiceProvider(ctr.Build());
            var ltscope = r.GetService<ILifetimeScope>();
            using (var scope = ltscope.BeginLifetimeScope())
            {
                var commandBus = scope.Resolve<ICommandBusAsync>();
                await commandBus.ExecuteAsync(new TestCommand());


                Application.DomainEventHandlers.TestOneHandler.HandlerOnePassed.Should().BeTrue();
                Application.DomainEventHandlers.TestTwoHandler.HandlerTwoPassed.Should().BeTrue();
            }
        }
    }
}
