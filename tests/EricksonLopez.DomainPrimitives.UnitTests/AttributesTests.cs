using System;
using FluentAssertions;
using Xunit;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.Tests
{
    public class AttributesTests
    {
        [Fact]
        public void StrongIdAttribute_CanBeInstantiated()
        {
            var attribute = new StrongIdAttribute<Guid>();
            attribute.Should().NotBeNull();
        }

        [Fact]
        public void ValueObjectAttribute_CanBeInstantiated()
        {
            var attribute = new ValueObjectAttribute();
            attribute.Should().NotBeNull();
        }
    }
}
