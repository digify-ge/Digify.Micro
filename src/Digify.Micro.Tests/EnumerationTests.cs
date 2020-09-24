using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FluentAssertions;

namespace Digify.Micro.Tests
{
    public class EnumerationTests
    {
        [Fact]
        public void enumerations_should_be_equal_with_same_values()
        {
            var status = new Status(1, "Initial");
            status.Should().Be(Status.Initial);
        }

        [Fact]
        public void enumerations_should_be_different_with_different_values()
        {
            var status = new Status(1, "Initial");
            status.Should().NotBe(Status.Pending);
        }
    }

    public class Status : Enumeration
    {
        public static Status Initial = new Status(1, "Initial");
        public static Status Pending = new Status(2, "Pending");
        public static Status Completed = new Status(3, "Completed");

        public Status(int value, string displayName)
        : base(value, displayName)
        {
        }
    }
}
