using FluentAssertions;
using Digify.Micro.Tests.Entities;
using System;
using Xunit;

namespace Digify.Micro.Tests
{
    public class ValueObjectTests
    {
        public class Equality
        {
            [Fact]
            public void EqualityOperatorShouldBeTrueIfValueObjectsMatchByValue()
            {
                AddressValueObject valueObject1 = new AddressValueObject("Georgia", "Tbilisi", "Street Ave", "13");
                AddressValueObject valueObject2 = new AddressValueObject("Georgia", "Tbilisi", "Street Ave", "13");

                (valueObject1 == valueObject2).Should().BeTrue();
            }


            [Fact]
            public void EqualityOperatorShouldBeTrueIfValueObjectsAreTheSameReference()
            {
                AddressValueObject valueObject1 = new AddressValueObject("Georgia", "Tbilisi", "Street Ave", "13");
                AddressValueObject sameReference = valueObject1;

                (valueObject1 == sameReference).Should().BeTrue();
            }

            [Fact]
            public void EqualityOperatorShouldBeFalseIfValueObjectsDoNotMatchByValue()
            {
                AddressValueObject valueObject1 = new AddressValueObject("Georgia", "Tbilisi", "Street Ave", "13");
                AddressValueObject valueObject2 = new AddressValueObject("Georgia", "Tbilisi", "Street Ave", "134");

                (valueObject1 == valueObject2).Should().BeFalse();
            }

            [Fact]
            public void EqualityOperatorShouldBeFalseIfComparedWithNull()
            {
                AddressValueObject valueObject1 = new AddressValueObject("Georgia", "Tbilisi", "Street Ave", "13");
                AddressValueObject valueObject2 = null;

                (valueObject1 == valueObject2).Should().BeFalse();
            }

            [Fact]
            public void EqualsShouldBeTrueIfValueObjectsMatchByValue()
            {
                AddressValueObject valueObject1 = new AddressValueObject("Georgia", "Tbilisi", "Street Ave", "13");
                AddressValueObject valueObject2 = new AddressValueObject("Georgia", "Tbilisi", "Street Ave", "13");

                valueObject1.Equals(valueObject2).Should().BeTrue();
            }

            [Fact]
            public void EqualsShouldBeTrueIfValueObjectsAreTheSameReference()
            {
                AddressValueObject valueObject1 = new AddressValueObject("Georgia", "Tbilisi", "Street Ave", "13");
                AddressValueObject sameReference = valueObject1;

                valueObject1.Equals(sameReference).Should().BeTrue();
            }

            [Fact]
            public void EqualsShouldNotBeTrueIfValueObjectsDoNotMatchByValue()
            {
                AddressValueObject valueObject1 = new AddressValueObject("Georgia", "Tbilisi", "Street Ave", "13");
                AddressValueObject valueObject2 = new AddressValueObject("Georgia", "Tbilisi", "Street Ave", "134");

                valueObject1.Equals(valueObject2).Should().BeFalse();
            }

            [Fact]
            public void EqualsOperatorShouldNotBeTrueIfComparedWithNull()
            {
                AddressValueObject valueObject1 = new AddressValueObject("Georgia", "Tbilisi", "Street Ave", "13");
                AddressValueObject valueObject2 = null;

                valueObject1.Equals(valueObject2).Should().BeFalse();
            }

        }

        public class GetHashCodeMethod
        {
            [Fact]
            public void ShouldBeSameForTheSameInstance()
            {
                AddressValueObject valueObject1 = new AddressValueObject("Georgia", "Tbilisi", "Street Ave", "13");

                int hashCode1 = valueObject1.GetHashCode();
                int hashCode2 = valueObject1.GetHashCode();

                hashCode1.Should().Be(hashCode2);
            }

            [Fact]
            public void ShouldBeSameForTheDifferentInstancesWithSameValues()
            {
                AddressValueObject valueObject1 = new AddressValueObject("Georgia", "Tbilisi", "Street Ave", "13");
                AddressValueObject valueObject2 = new AddressValueObject("Georgia", "Tbilisi", "Street Ave", "13");

                int hashCode1 = valueObject1.GetHashCode();
                int hashCode2 = valueObject2.GetHashCode();

                hashCode1.Should().Be(hashCode2);
            }

            [Fact]
            public void ShouldNotBeSameForTheDifferentInstancesWithDifferentValues()
            {
                AddressValueObject valueObject1 = new AddressValueObject("Georgia", "Tbilisi", "Street Ave", "13");
                AddressValueObject valueObject2 = new AddressValueObject("Georgia", "Kutaisi", "Street Ave", "134");

                int hashCode1 = valueObject1.GetHashCode();
                int hashCode2 = valueObject2.GetHashCode();

                hashCode1.Should().NotBe(hashCode2);
            }
        }
    }
}
