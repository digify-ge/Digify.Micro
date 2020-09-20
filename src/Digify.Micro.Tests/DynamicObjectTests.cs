using Digify.Micro.Tests.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FluentAssertions;

namespace Digify.Micro.Tests
{
    public class DynamicObjectTests
    {
        [Fact]
        public void DynamicMethodInvoke()
        {
            var sample = new Sample1() { Id = 1 };
            sample.AsDynamic().Apply(5);
            sample.Id.Should().Be(6);
        }
    }
}
