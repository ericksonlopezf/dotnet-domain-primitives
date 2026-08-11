using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using EricksonLopez.DomainPrimitives.Generators;

namespace EricksonLopez.DomainPrimitives.Generators.Tests
{
    public class DiagnosticDescriptorsTests
    {
        [Fact]
        public void Descriptors_AreNotNull()
        {
            DiagnosticDescriptors.TypeMustBePartial.Should().NotBeNull();
            DiagnosticDescriptors.TypeMustBeReadonlyRecordStruct.Should().NotBeNull();
            DiagnosticDescriptors.UnsupportedBackingType.Should().NotBeNull();
            DiagnosticDescriptors.ConflictingAttributes.Should().NotBeNull();
            DiagnosticDescriptors.InvalidAttributeParameter.Should().NotBeNull();
        }
    }
}

