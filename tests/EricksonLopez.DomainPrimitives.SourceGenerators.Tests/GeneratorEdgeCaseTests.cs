using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;
using EricksonLopez.DomainPrimitives.Generators;

namespace EricksonLopez.DomainPrimitives.Generators.Tests;

public class GeneratorEdgeCaseTests
{
    private static void RunGenerator(IIncrementalGenerator generator, string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.CSharp11));
        var compilation = CSharpCompilation.Create("Tests", new[] { syntaxTree }, Basic.Reference.Assemblies.Net80.References.All, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGenerators(compilation);
    }

    [Fact]
    public void StringPrimitive_EdgeCases_AreCovered()
    {
        var source = @"
using EricksonLopez.DomainPrimitives;

namespace TestNamespace1 {
    using StringPrimitiveAttribute = System.ObsoleteAttribute;
    [StringPrimitive] public record struct FakeAttributeStruct {}
}

namespace TestNamespace {
[StringPrimitive] public record NotAStructRecord {}
[System.Obsolete] public record struct NoAttributesStruct {}
[StringPrimitive]
[EricksonLopez.DomainPrimitives.Normalization.MinLength(5)]
[EricksonLopez.DomainPrimitives.Normalization.MaxLength(10)]
[EricksonLopez.DomainPrimitives.Normalization.NotEmpty]
public readonly partial record struct MinMaxPrimitive;

[StringPrimitive]
[EricksonLopez.DomainPrimitives.Normalization.Length(1, 100)]
public readonly partial record struct LengthPrimitive;

public class CustomValidatorAttribute : System.Attribute {}
[StringPrimitive]
[CustomValidator]
public readonly partial record struct InvalidValidatorPrimitive;

}

namespace EricksonLopez.DomainPrimitives { public class StringPrimitiveAttribute : System.Attribute {} }
namespace EricksonLopez.DomainPrimitives.Normalization
{
    public class MinLengthAttribute : System.Attribute { public MinLengthAttribute(int i) {} }
    public class MaxLengthAttribute : System.Attribute { public MaxLengthAttribute(int i) {} }
    public class LengthAttribute : System.Attribute { public LengthAttribute(int min, int max) {} }
    public class NotEmptyAttribute : System.Attribute {}
}
";
        RunGenerator(new StringPrimitiveGenerator(), source);
    }

    [Fact]
    public void NumericPrimitive_EdgeCases_AreCovered()
    {
        var source = @"

namespace TestNamespace1 {
    using NumericPrimitiveAttribute = System.ObsoleteAttribute;
    [NumericPrimitive] public record struct FakeAttributeStruct {}
}

namespace TestNamespace {
[NumericPrimitive<int>] public record NotAStructRecord {}
[System.Obsolete] public record struct NoAttributesStruct {}
[NumericPrimitive<int>]
[EricksonLopez.DomainPrimitives.Normalization.Minimum(5)]
[EricksonLopez.DomainPrimitives.Normalization.Maximum(10)]
public readonly partial record struct MinMaxPrimitive;
[NumericPrimitive<int>]
[EricksonLopez.DomainPrimitives.Normalization.Range(1, 100, MinExclusive = true, MaxExclusive = true)]
public readonly partial record struct RangePrimitive;

}

namespace EricksonLopez.DomainPrimitives { public class NumericPrimitiveAttribute<T> : System.Attribute { public bool AllowNegation { get; set; } } }
namespace EricksonLopez.DomainPrimitives.Normalization
{
    public class MinimumAttribute : System.Attribute { public MinimumAttribute(object i) {} }
    public class MaximumAttribute : System.Attribute { public MaximumAttribute(object i) {} }
    public class RangeAttribute : System.Attribute { public RangeAttribute(object min, object max) { MinExclusive = false; MaxExclusive = false; } public bool MinExclusive { get; set; } public bool MaxExclusive { get; set; } }
}
";
        RunGenerator(new NumericPrimitiveGenerator(), source);
    }
    
    [Fact]
    public void DatePrimitive_EdgeCases_AreCovered()
    {
        var source = @"
using System;

namespace TestNamespace1 {
    using DatePrimitiveAttribute = System.ObsoleteAttribute;
    [DatePrimitive] public record struct FakeAttributeStruct {}
}

namespace TestNamespace {
[DatePrimitive] public record NotAStructRecord {}
[System.Obsolete] public record struct NoAttributesStruct {}

[DatePrimitive]
[EricksonLopez.DomainPrimitives.Normalization.Past]
[EricksonLopez.DomainPrimitives.Normalization.PastOrPresent]
[DatePrimitive]
[EricksonLopez.DomainPrimitives.Normalization.FutureOrPresent]
public readonly partial record struct DatePrimitiveRules;

[EricksonLopez.DomainPrimitives.DateOfBirth]
public readonly partial record struct DobPrimitive;

}

namespace EricksonLopez.DomainPrimitives { public class DatePrimitiveAttribute : System.Attribute {} public class DateOfBirthAttribute : System.Attribute {} }
namespace EricksonLopez.DomainPrimitives.Normalization
{
    public class PastAttribute : System.Attribute {}
    public class PastOrPresentAttribute : System.Attribute {}
    public class FutureAttribute : System.Attribute {}
    public class FutureOrPresentAttribute : System.Attribute {}
}
";
        RunGenerator(new DatePrimitiveGenerator(), source);
    }
    
    [Fact]
    public void SmartEnum_EdgeCases_AreCovered()
    {
        var source = @"

namespace TestNamespace1 {
    using SmartEnumAttribute = System.ObsoleteAttribute;
    [SmartEnum] public record struct FakeAttributeStruct {}
}

namespace TestNamespace {
[SmartEnum<int>] public record NotAStructRecord {}
[System.Obsolete] public record struct NoAttributesStruct {}

}

namespace EricksonLopez.DomainPrimitives { public class SmartEnumAttribute<T> : System.Attribute {} }
";
        RunGenerator(new SmartEnumGenerator(), source);
    }
    
    [Fact]
    public void StrongId_EdgeCases_AreCovered()
    {
        var source = @"

namespace TestNamespace1 {
    using StrongIdAttribute = System.ObsoleteAttribute;
    [StrongId] public record struct FakeAttributeStruct {}
}

namespace TestNamespace {
[StrongId<int>] public record NotAStructRecord {}
[System.Obsolete] public record struct NoAttributesStruct {}

[StrongId<int>]
public readonly partial record struct IntId;
[StrongId<long>]
public readonly partial record struct LongId;
[StrongId<string>]
public readonly partial record struct StringId;
[StrongId<System.Guid>]
public readonly partial record struct GuidId;

}

namespace EricksonLopez.DomainPrimitives { public class StrongIdAttribute<TValue> : System.Attribute {} }
";
        RunGenerator(new StrongIdGenerator(), source);
    }
    
    [Fact]
    public void ValueObject_EdgeCases_AreCovered()
    {
        var source = @"

namespace TestNamespace1 {
    using ValueObjectAttribute = System.ObsoleteAttribute;
    [ValueObject] public record struct FakeAttributeStruct {}
}

namespace TestNamespace {
[ValueObject] public record NotAStructRecord {}
[System.Obsolete] public record struct NoAttributesStruct {}

}

namespace EricksonLopez.DomainPrimitives { public class ValueObjectAttribute : System.Attribute {} }
";
        RunGenerator(new ValueObjectGenerator(), source);
    }
}
