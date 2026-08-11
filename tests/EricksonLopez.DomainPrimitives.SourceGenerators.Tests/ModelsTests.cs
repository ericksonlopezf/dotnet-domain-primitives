using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using EricksonLopez.DomainPrimitives.Generators.Models;
using System.Collections.Immutable;

namespace EricksonLopez.DomainPrimitives.Generators.Tests
{
    public class ModelsTests
    {
        [Fact]
        public void Models_PropertiesSetCorrectly()
        {
            var stringInfo = new StringPrimitiveTypeInfo(
                Namespace: "NS",
                TypeName: "Name",
                Accessibility: "public",
                ContainingTypes: new EquatableArray<string>([]),
                Trim: true,
                TrimStart: false,
                TrimEnd: false,
                LowerCase: false,
                UpperCase: false,
                NormalizeWhitespace: false,
                MinLength: 1,
                MaxLength: 10,
                ExactLength: null,
                NotEmpty: true,
                RegexPatterns: new EquatableArray<RegexInfo>([new RegexInfo("pat", "err")]),
                DomainShortcut: "Email",
                HasCustomValidator: true);

                
            stringInfo.Namespace.Should().Be("NS");
            stringInfo.Trim.Should().BeTrue();
            
            var numericInfo = new NumericPrimitiveTypeInfo(
                "NS", "Name", "int", "public", new EquatableArray<string>([]),
                true, true, true, true, true, 1, 10, false, false, "Money");
                
            numericInfo.AllowAddition.Should().BeTrue();
            
            var dateInfo = new DatePrimitiveTypeInfo(
                "NS", "Name", "DateOnly", "public", new EquatableArray<string>([]),
                "DateOnly", true, false, 100, "BirthDate");
                
            dateInfo.PastOnly.Should().BeTrue();
            
            var valProp = new ValueObjectPropertyInfo("Prop", "string", "prop");
            valProp.Name.Should().Be("Prop");
            
            var valInfo = new ValueObjectTypeInfo(
                "NS", "Name", "public", new EquatableArray<string>([]),
                new EquatableArray<ValueObjectPropertyInfo>([valProp]));
                
            valInfo.Properties.Length.Should().Be(1);
        }
    }
}

