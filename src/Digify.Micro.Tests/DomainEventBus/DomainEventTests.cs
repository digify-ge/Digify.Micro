using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
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
            service.AddMicro();
            var ltscope = service.BuildServiceProvider().GetService<IServiceScopeFactory>();
            using (var serviceScope = ltscope.CreateScope())
            {
                var scope = serviceScope.ServiceProvider;
                var commandBus = scope.GetService<IBusAsync>();
                await commandBus.ExecuteAsync(new TestCommand());


                Application.DomainEventHandlers.TestOneHandler.HandlerOnePassed.Should().BeTrue();
                Application.DomainEventHandlers.TestTwoHandler.HandlerTwoPassed.Should().BeTrue();
            }
        }
    }
}
