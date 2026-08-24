// Copyright © Erickson Lopez. MIT License.
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
    public class NumericPrimitiveGeneratorSnapshotTests
    {
        [Fact]
        public Task GeneratesNumericPrimitiveCorrectly()
        {
            string source = @"
using EricksonLopez.DomainPrimitives;

namespace TestNamespace
{
    [NumericPrimitive<int>(AllowAddition = true)]
    public readonly partial record struct Score;

    [NumericPrimitive<double>(AllowMultiplication = true, AllowDivision = true)]
    [Range(0.0, 100.0)]
    public readonly partial record struct Percentage;

    [NumericPrimitive<decimal>]
    [GreaterThan(0)]
    [LessThanOrEqual(1000)]
    public readonly partial record struct Price;

    [NumericPrimitive<long>]
    [Positive]
    public readonly partial record struct Population;

    [NumericPrimitive<short>]
    [Negative]
    public readonly partial record struct Depth;
}

namespace EricksonLopez.DomainPrimitives
{
    public class NumericPrimitiveAttribute<T> : System.Attribute {
        public bool AllowAddition { get; set; }
        public bool AllowSubtraction { get; set; }
        public bool AllowMultiplication { get; set; }
        public bool AllowDivision { get; set; }
        public bool AllowModulus { get; set; }
        public bool AllowIncrement { get; set; }
        public bool AllowDecrement { get; set; }
    }
}
namespace EricksonLopez.DomainPrimitives.Validation
{
    public class RangeAttribute : System.Attribute { public RangeAttribute(object min, object max) {} }
    public class GreaterThanAttribute : System.Attribute { public GreaterThanAttribute(object val) {} }
    public class LessThanOrEqualAttribute : System.Attribute { public LessThanOrEqualAttribute(object val) {} }
    public class PositiveAttribute : System.Attribute {}
    public class NegativeAttribute : System.Attribute {}
}
";

            var syntaxTree = CSharpSyntaxTree.ParseText(EricksonLopez.DomainPrimitives.SourceGenerators.Tests.TestCompilationHelper.EnsureUsings(source), new CSharpParseOptions(LanguageVersion.CSharp11));
            var compilation = CSharpCompilation.Create(
                assemblyName: "Tests",
                syntaxTrees: new[] { syntaxTree },
                references: Basic.Reference.Assemblies.Net80.References.All,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var generator = new NumericPrimitiveGenerator();
            var driver = (GeneratorDriver)CSharpGeneratorDriver.Create(generator);
            driver = driver.RunGenerators(compilation);
            var runResult = driver.GetRunResult();
            var diagnostics = runResult.Diagnostics;
            Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
            Assert.NotEmpty(runResult.GeneratedTrees);

            return Verifier.Verify(driver).UseDirectory("Snapshots");
        }

        [Fact]
        public Task GeneratesDomainShortcutsCorrectly()
        {
            string source = @"
using EricksonLopez.DomainPrimitives;

namespace TestNamespace
{
    [Money] public readonly partial record struct Amount;
    [Percentage] public readonly partial record struct Discount;
    [Weight] public readonly partial record struct PackageWeight;
    [Height] public readonly partial record struct PersonHeight;
    [Temperature] public readonly partial record struct OutsideTemp;
    [Latitude] public readonly partial record struct MapLat;
    [Longitude] public readonly partial record struct MapLon;
}

namespace EricksonLopez.DomainPrimitives
{
    public class NumericPrimitiveAttribute<T> : System.Attribute {
        public bool AllowAddition { get; set; }
        public bool AllowSubtraction { get; set; }
        public bool AllowMultiplication { get; set; }
        public bool AllowDivision { get; set; }
        public bool AllowModulus { get; set; }
        public bool AllowIncrement { get; set; }
        public bool AllowDecrement { get; set; }
    }

    public class MoneyAttribute : NumericPrimitiveAttribute<decimal> {}
    public class PercentageAttribute : NumericPrimitiveAttribute<double> {}
    public class WeightAttribute : NumericPrimitiveAttribute<double> {}
    public class HeightAttribute : NumericPrimitiveAttribute<double> {}
    public class TemperatureAttribute : NumericPrimitiveAttribute<double> {}
    public class LatitudeAttribute : NumericPrimitiveAttribute<double> {}
    public class LongitudeAttribute : NumericPrimitiveAttribute<double> {}
}
";
            var syntaxTree = CSharpSyntaxTree.ParseText(EricksonLopez.DomainPrimitives.SourceGenerators.Tests.TestCompilationHelper.EnsureUsings(source), new CSharpParseOptions(LanguageVersion.CSharp11));
            var compilation = CSharpCompilation.Create(
                assemblyName: "Tests",
                syntaxTrees: new[] { syntaxTree },
                references: Basic.Reference.Assemblies.Net80.References.All,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var generator = new NumericPrimitiveGenerator();
            var driver = (GeneratorDriver)CSharpGeneratorDriver.Create(generator);
            driver = driver.RunGenerators(compilation);
            var runResult = driver.GetRunResult();
            var diagnostics = runResult.Diagnostics;
            Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
            Assert.NotEmpty(runResult.GeneratedTrees);

            return Verifier.Verify(driver).UseDirectory("Snapshots");
        }
        [Fact]
        public Task GeneratesNestedTypeCorrectly()
        {
            string source = @"
using EricksonLopez.DomainPrimitives;

namespace TestNamespace
{
    public partial class OuterClass
    {
        public partial class InnerClass
        {
            [NumericPrimitive<int>]
            public readonly partial record struct NestedPrimitive;
        }
    }
}
namespace EricksonLopez.DomainPrimitives
{
    public class NumericPrimitiveAttribute<T> : System.Attribute {}
}
";
            var syntaxTree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(EricksonLopez.DomainPrimitives.SourceGenerators.Tests.TestCompilationHelper.EnsureUsings(source), new CSharpParseOptions(LanguageVersion.CSharp11));
            var compilation = Microsoft.CodeAnalysis.CSharp.CSharpCompilation.Create("Tests", new[] { syntaxTree }, Basic.Reference.Assemblies.Net80.References.All, new Microsoft.CodeAnalysis.CSharp.CSharpCompilationOptions(Microsoft.CodeAnalysis.OutputKind.DynamicallyLinkedLibrary));
            var generator = new EricksonLopez.DomainPrimitives.Generators.NumericPrimitiveGenerator();
            Microsoft.CodeAnalysis.GeneratorDriver driver = Microsoft.CodeAnalysis.CSharp.CSharpGeneratorDriver.Create(generator);
            driver = driver.RunGenerators(compilation);
            var runResult = driver.GetRunResult();
            var diagnostics = runResult.Diagnostics;
            Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
            Assert.NotEmpty(runResult.GeneratedTrees);
            return VerifyXunit.Verifier.Verify(driver).UseDirectory("Snapshots");
        }
    }
}




