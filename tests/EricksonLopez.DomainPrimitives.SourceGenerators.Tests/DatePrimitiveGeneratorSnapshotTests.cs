using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VerifyXunit;
using Xunit;
using EricksonLopez.DomainPrimitives.Generators;

namespace EricksonLopez.DomainPrimitives.Generators.Tests
{
    [UsesVerify]
    public class DatePrimitiveGeneratorSnapshotTests
    {
        [Fact]
        public Task GeneratesDatePrimitiveCorrectly()
        {
            string source = @"
using EricksonLopez.DomainPrimitives;

namespace TestNamespace
{
    [DatePrimitive(Kind = DatePrimitiveKind.DateOnly, PastOnly = true)]
    public readonly partial record struct BirthDate;
}

namespace EricksonLopez.DomainPrimitives
{
    public class DatePrimitiveAttribute : System.Attribute {
        public DatePrimitiveKind Kind { get; set; }
        public bool PastOnly { get; set; }
    }
    public enum DatePrimitiveKind { DateOnly, DateTime }
}
";

            var syntaxTree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.CSharp11));
            var compilation = CSharpCompilation.Create(
                assemblyName: "Tests",
                syntaxTrees: new[] { syntaxTree },
                references: Basic.Reference.Assemblies.Net80.References.All,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var generator = new DatePrimitiveGenerator();
            var driver = (GeneratorDriver)CSharpGeneratorDriver.Create(generator);
            driver = driver.RunGenerators(compilation);

            return Verifier.Verify(driver).UseDirectory("Snapshots");
        }

        [Fact]
        public Task GeneratesDomainShortcutsCorrectly()
        {
            string source = @"

namespace TestNamespace
{
    [BirthDate(MaxAge = 150)] public readonly partial record struct EmployeeBirthDate;
    [ExpirationDate] public readonly partial record struct CreditCardExpiry;
    [BusinessDate] public readonly partial record struct TransactionDate;
    [FiscalYear] public readonly partial record struct FY;
    [Month] public readonly partial record struct ReportingMonth;
    [Quarter] public readonly partial record struct ReportingQuarter;
    [Week] public readonly partial record struct ReportingWeek;
    [DateRange] public readonly partial record struct EventPeriod;
    [TimeRange] public readonly partial record struct ShiftTime;
    
    [DatePrimitive(FutureOnly = true)]
    public readonly partial record struct FutureEventDate;
}

namespace EricksonLopez.DomainPrimitives
{
    public class BirthDateAttribute : System.Attribute { public int MaxAge { get; set; } }
    public class ExpirationDateAttribute : System.Attribute {}
    public class BusinessDateAttribute : System.Attribute {}
    public class FiscalYearAttribute : System.Attribute {}
    public class MonthAttribute : System.Attribute {}
    public class QuarterAttribute : System.Attribute {}
    public class WeekAttribute : System.Attribute {}
    public class DateRangeAttribute : System.Attribute {}
    public class TimeRangeAttribute : System.Attribute {}
}
";
            var syntaxTree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.CSharp11));
            var compilation = CSharpCompilation.Create(
                assemblyName: "Tests",
                syntaxTrees: new[] { syntaxTree },
                references: Basic.Reference.Assemblies.Net80.References.All,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var generator = new DatePrimitiveGenerator();
            var driver = (GeneratorDriver)CSharpGeneratorDriver.Create(generator);
            driver = driver.RunGenerators(compilation);

            return Verifier.Verify(driver).UseDirectory("Snapshots");
        }
        [Fact]
        public Task GeneratesNestedTypeCorrectly()
        {
            string source = @"
namespace TestNamespace
{
    public partial class OuterClass
    {
        public partial class InnerClass
        {
            [DatePrimitive]
            public readonly partial record struct NestedPrimitive;
        }
    }
}
namespace EricksonLopez.DomainPrimitives
{
    public class DatePrimitiveAttribute : System.Attribute {}
}
";
            var syntaxTree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.CSharp11));
            var compilation = Microsoft.CodeAnalysis.CSharp.CSharpCompilation.Create("Tests", new[] { syntaxTree }, Basic.Reference.Assemblies.Net80.References.All, new Microsoft.CodeAnalysis.CSharp.CSharpCompilationOptions(Microsoft.CodeAnalysis.OutputKind.DynamicallyLinkedLibrary));
            var generator = new EricksonLopez.DomainPrimitives.Generators.DatePrimitiveGenerator();
            Microsoft.CodeAnalysis.GeneratorDriver driver = Microsoft.CodeAnalysis.CSharp.CSharpGeneratorDriver.Create(generator);
            driver = driver.RunGenerators(compilation);
            return VerifyXunit.Verifier.Verify(driver).UseDirectory("Snapshots");
        }
    }
}



