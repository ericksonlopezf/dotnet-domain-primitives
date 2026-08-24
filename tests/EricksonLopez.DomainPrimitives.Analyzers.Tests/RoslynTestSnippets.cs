// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives.Analyzers.Tests;

/// <summary>
/// Centralized synthetic attribute definitions and Roslyn code snippets for analyzer tests.
/// Eliminates code duplication across DP0001–DP0017 tests.
/// </summary>
public static class RoslynTestSnippets
{
    private const string BaseAttributesContent = @"
namespace EricksonLopez.DomainPrimitives
{
    using System;

    public class StrongIdAttribute<T> : Attribute {}
    public class IdAttribute : Attribute {}
    public class MyCodeAttribute : Attribute {}
    public class StringPrimitiveAttribute : Attribute {}
    public class NumericPrimitiveAttribute<T> : Attribute {}
    public class DatePrimitiveAttribute : Attribute 
    { 
        public bool PastOnly { get; set; } 
        public bool FutureOnly { get; set; }
        public string Format { get; set; }
    }
    public class ValueObjectAttribute : Attribute {}
    public class SmartEnumAttribute<T> : Attribute {}
    public class EmailAttribute : Attribute {}
    public interface IDomainPrimitive {}
    public interface IDomainPrimitive<T> {}
    public interface IDomainPrimitive<TSelf, TValue> : IDomainPrimitive<TSelf> {}
    public interface IStrongId<TSelf, TValue> {}
}
";

    private const string ValidationAttributesContent = @"
namespace EricksonLopez.DomainPrimitives.Validation
{
    using System;

    public class NotEmptyAttribute : Attribute {}
    public class RegexAttribute : Attribute { public RegexAttribute() {} public RegexAttribute(string pattern) {} }
    public class MinLengthAttribute : Attribute { public MinLengthAttribute() {} public MinLengthAttribute(int len) {} }
    public class MaxLengthAttribute : Attribute { public MaxLengthAttribute() {} public MaxLengthAttribute(int len) {} }
    public class LengthAttribute : Attribute { public LengthAttribute() {} public LengthAttribute(int min, int max) {} }
    public class RangeAttribute : Attribute 
    { 
        public RangeAttribute() {}
        public RangeAttribute(double min, double max) {} 
        public RangeAttribute(int min, int max) {} 
        public RangeAttribute(string min, string max) {} 
    }
}
";

    private const string NormalizationAttributesContent = @"
namespace EricksonLopez.DomainPrimitives.Normalization
{
    using System;

    public class LowerCaseAttribute : Attribute {}
    public class UpperCaseAttribute : Attribute {}
    public class TrimAttribute : Attribute {}
    public class TrimStartAttribute : Attribute {}
}
";

    private const string DefaultsAttributesContent = @"
namespace EricksonLopez.DomainPrimitives
{
    using System;

    [AttributeUsage(AttributeTargets.Assembly)]
    public class DomainPrimitivesDefaultsAttribute : Attribute
    {
        public bool Trim { get; set; }
        public bool NotEmpty { get; set; }
        public int MaxLength { get; set; }
        public Type ExceptionType { get; set; }
    }
}
";

    public const string BaseAttributes = @"
using System;
using EricksonLopez.DomainPrimitives;
" + BaseAttributesContent;

    public const string ValidationAttributes = @"
using System;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.DomainPrimitives.Validation;
" + BaseAttributesContent + ValidationAttributesContent;

    public const string NormalizationAttributes = @"
using System;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.DomainPrimitives.Normalization;
" + BaseAttributesContent + NormalizationAttributesContent;

    public const string DefaultsAttributes = DefaultsAttributesContent;

    public const string AllAttributes = @"
using System;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.DomainPrimitives.Validation;
using EricksonLopez.DomainPrimitives.Normalization;
" + BaseAttributesContent + ValidationAttributesContent + NormalizationAttributesContent + DefaultsAttributesContent;

    public const string CommonFrameworkStubs = AllAttributes;
}
