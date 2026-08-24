// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
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
            stringInfo.TypeName.Should().Be("Name");
            stringInfo.Accessibility.Should().Be("public");
            stringInfo.ContainingTypes.Length.Should().Be(0);
            stringInfo.Trim.Should().BeTrue();
            stringInfo.TrimStart.Should().BeFalse();
            stringInfo.TrimEnd.Should().BeFalse();
            stringInfo.LowerCase.Should().BeFalse();
            stringInfo.UpperCase.Should().BeFalse();
            stringInfo.NormalizeWhitespace.Should().BeFalse();
            stringInfo.MinLength.Should().Be(1);
            stringInfo.MaxLength.Should().Be(10);
            stringInfo.ExactLength.Should().BeNull();
            stringInfo.NotEmpty.Should().BeTrue();
            stringInfo.RegexPatterns.Length.Should().Be(1);
            stringInfo.DomainShortcut.Should().Be("Email");
            stringInfo.HasCustomValidator.Should().BeTrue();
            stringInfo.AllowedSchemes.Length.Should().Be(0);
            stringInfo.CustomExceptionType.Should().BeNull();
            
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

            var strongIdInfo = new StrongIdTypeInfo("NS", "Id", "Guid", "System.Guid", "public", new EquatableArray<string>([]), true);
            strongIdInfo.TypeName.Should().Be("Id");
            strongIdInfo.RejectEmpty.Should().BeTrue();
            strongIdInfo.IsGuidBacked.Should().BeTrue();
            strongIdInfo.IsStringBacked.Should().BeFalse();
            strongIdInfo.IsIntegerBacked.Should().BeFalse();

            var strongIdGuidShort = new StrongIdTypeInfo("NS", "Id", "Guid", "Guid", "public", new EquatableArray<string>([]), true);
            strongIdGuidShort.IsGuidBacked.Should().BeTrue();

            var strongIdString = new StrongIdTypeInfo("NS", "Id", "string", "System.String", "public", new EquatableArray<string>([]), true);
            strongIdString.IsStringBacked.Should().BeTrue();
            strongIdString.IsGuidBacked.Should().BeFalse();

            var strongIdStringShort = new StrongIdTypeInfo("NS", "Id", "string", "string", "public", new EquatableArray<string>([]), true);
            strongIdStringShort.IsStringBacked.Should().BeTrue();

            var strongIdInt32 = new StrongIdTypeInfo("NS", "Id", "int", "System.Int32", "public", new EquatableArray<string>([]), true);
            strongIdInt32.IsIntegerBacked.Should().BeTrue();
            strongIdInt32.IsGuidBacked.Should().BeFalse();

            var strongIdInt64 = new StrongIdTypeInfo("NS", "Id", "long", "System.Int64", "public", new EquatableArray<string>([]), true);
            strongIdInt64.IsIntegerBacked.Should().BeTrue();

            var strongIdIntShort = new StrongIdTypeInfo("NS", "Id", "int", "int", "public", new EquatableArray<string>([]), true);
            strongIdIntShort.IsIntegerBacked.Should().BeTrue();

            var strongIdLongShort = new StrongIdTypeInfo("NS", "Id", "long", "long", "public", new EquatableArray<string>([]), true);
            strongIdLongShort.IsIntegerBacked.Should().BeTrue();

            var smartEnumInfo = new SmartEnumTypeInfo("NS", "Status", "int", new EquatableArray<string>(["A", "B"]), false);
            smartEnumInfo.MemberNames.Length.Should().Be(2);

            var reg = new RegexInfo("^[0-9]+$", "Digits only");
            reg.Pattern.Should().Be("^[0-9]+$");
            reg.ErrorMessage.Should().Be("Digits only");

            var defaults = new AssemblyDefaultsInfo(true, true, 50, "System.ArgumentException");
            defaults.Trim.Should().BeTrue();
            defaults.NotEmpty.Should().BeTrue();
            defaults.MaxLength.Should().Be(50);
            defaults.ExceptionTypeFullName.Should().Be("System.ArgumentException");
        }

        [Fact]
        public void Models_EqualityAndHashCode_WorkCorrectly()
        {
            var stringInfo1 = new StringPrimitiveTypeInfo("NS", "Name", "public", new EquatableArray<string>([]), true, false, false, false, false, false, 1, 10, null, true, new EquatableArray<RegexInfo>([]), "Email", false);
            var stringInfo2 = new StringPrimitiveTypeInfo("NS", "Name", "public", new EquatableArray<string>([]), true, false, false, false, false, false, 1, 10, null, true, new EquatableArray<RegexInfo>([]), "Email", false);
            var stringInfo3 = new StringPrimitiveTypeInfo("NS", "Other", "public", new EquatableArray<string>([]), true, false, false, false, false, false, 1, 10, null, true, new EquatableArray<RegexInfo>([]), "Email", false);

            stringInfo1.Equals(stringInfo2).Should().BeTrue();
            stringInfo1.Equals(stringInfo3).Should().BeFalse();
            stringInfo1.GetHashCode().Should().Be(stringInfo2.GetHashCode());

            var numInfo1 = new NumericPrimitiveTypeInfo("NS", "Num", "int", "public", new EquatableArray<string>([]), true, true, false, false, false, 0, 100, false, false, null);
            var numInfo2 = new NumericPrimitiveTypeInfo("NS", "Num", "int", "public", new EquatableArray<string>([]), true, true, false, false, false, 0, 100, false, false, null);
            numInfo1.Equals(numInfo2).Should().BeTrue();
            numInfo1.GetHashCode().Should().Be(numInfo2.GetHashCode());

            var dateInfo1 = new DatePrimitiveTypeInfo("NS", "Date", "DateOnly", "public", new EquatableArray<string>([]), "DateOnly", true, false, null, null);
            var dateInfo2 = new DatePrimitiveTypeInfo("NS", "Date", "DateOnly", "public", new EquatableArray<string>([]), "DateOnly", true, false, null, null);
            dateInfo1.Equals(dateInfo2).Should().BeTrue();
            dateInfo1.GetHashCode().Should().Be(dateInfo2.GetHashCode());

            var vo1 = new ValueObjectTypeInfo("NS", "VO", "public", new EquatableArray<string>([]), new EquatableArray<ValueObjectPropertyInfo>([]));
            var vo2 = new ValueObjectTypeInfo("NS", "VO", "public", new EquatableArray<string>([]), new EquatableArray<ValueObjectPropertyInfo>([]));
            vo1.Equals(vo2).Should().BeTrue();
            vo1.GetHashCode().Should().Be(vo2.GetHashCode());

            var id1 = new StrongIdTypeInfo("NS", "ID", "Guid", "System.Guid", "public", new EquatableArray<string>([]), true);
            var id2 = new StrongIdTypeInfo("NS", "ID", "Guid", "System.Guid", "public", new EquatableArray<string>([]), true);
            id1.Equals(id2).Should().BeTrue();
            id1.GetHashCode().Should().Be(id2.GetHashCode());

            var se1 = new SmartEnumTypeInfo("NS", "Enum", "int", new EquatableArray<string>(["A"]), false);
            var se2 = new SmartEnumTypeInfo("NS", "Enum", "int", new EquatableArray<string>(["A"]), false);
            se1.Equals(se2).Should().BeTrue();
            se1.GetHashCode().Should().Be(se2.GetHashCode());

            // EquatableArray edge cases
            var eqArr1 = new EquatableArray<int>([1, 2]);
            var eqArr2 = new EquatableArray<int>([1, 2]);
            var eqArrDiffLen = new EquatableArray<int>([1, 2, 3]);
            var eqArrDiffElem = new EquatableArray<int>([1, 4]);

            eqArr1.Equals(eqArr2).Should().BeTrue();
            eqArr1.Equals(eqArrDiffLen).Should().BeFalse();
            eqArr1.Equals(eqArrDiffElem).Should().BeFalse();
            eqArr1.Equals((object)eqArr2).Should().BeTrue();
            eqArr1.Equals((object)"not an equatable array").Should().BeFalse();
            eqArr1.Equals(null).Should().BeFalse();

            eqArr1[0].Should().Be(1);
            eqArr1[1].Should().Be(2);

            eqArr1.GetHashCode().Should().Be(eqArr2.GetHashCode());
            eqArr1.GetHashCode().Should().NotBe(eqArrDiffElem.GetHashCode());

            int expectedHash = unchecked((17 * 31 + (1).GetHashCode()) * 31 + (2).GetHashCode());
            eqArr1.GetHashCode().Should().Be(expectedHash);

            var negArr = new EquatableArray<int>([-10, 20]);
            int expectedNegHash = unchecked((17 * 31 + (-10).GetHashCode()) * 31 + (20).GetHashCode());
            negArr.GetHashCode().Should().Be(expectedNegHash);

            var strArr = new EquatableArray<string>(["alpha", "beta"]);
            int expectedStrHash = unchecked((17 * 31 + "alpha".GetHashCode()) * 31 + "beta".GetHashCode());
            strArr.GetHashCode().Should().Be(expectedStrHash);

            var arrWithNull = new EquatableArray<string>(ImmutableArray.Create<string>("alpha", null!));
            int expectedNullHash = unchecked((17 * 31 + "alpha".GetHashCode()) * 31 + 0);
            arrWithNull.GetHashCode().Should().Be(expectedNullHash);

            EquatableArray<int> defaultEqArr = default;
            defaultEqArr.GetHashCode().Should().Be(17);
            defaultEqArr.Length.Should().Be(0);
            defaultEqArr.Equals((object)"not an equatable array").Should().BeFalse();
            defaultEqArr.Equals((object)123).Should().BeFalse();
            defaultEqArr.Equals(null).Should().BeFalse();
            defaultEqArr.Equals(new EquatableArray<int>([])).Should().BeTrue();

            EquatableArray<int> implicitArr = ImmutableArray.Create(10, 20);
            implicitArr.Length.Should().Be(2);
        }
    }
}


