using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using EricksonLopez.DomainPrimitives.Tests.TestTypes;
using Xunit;
using Xunit.Abstractions;

namespace EricksonLopez.DomainPrimitives.UnitTests;

/// <summary>
/// Outputs a detailed member list for a single type — useful for understanding
/// what is counted by the ApiSurfaceBudgetTests.
/// Run with: dotnet test --filter "FullyQualifiedName~MemberInspectionTests"
/// </summary>
public sealed class MemberInspectionTests(ITestOutputHelper output)
{
    private void InspectType(Type t)
    {
        const BindingFlags f = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
        var all = t.GetMembers(f)
            .Where(m => m.DeclaringType != typeof(object))
            .Where(m => m.MemberType != MemberTypes.Constructor)
            .Where(m => m.MemberType != MemberTypes.NestedType)
            .OrderBy(m => m.MemberType.ToString())
            .ThenBy(m => m.Name)
            .ToList();

        output.WriteLine($"\n=== {t.Name} ({all.Count} total) ===");
        int visibleCount = 0;
        foreach (var m in all)
        {
            var eb = m.GetCustomAttribute<EditorBrowsableAttribute>();
            var hidden = eb?.State == EditorBrowsableState.Never ? " [HIDDEN]" : "";
            if (hidden == string.Empty) visibleCount++;
            output.WriteLine($"  {m.MemberType,-12} {m.Name}{hidden}");
        }
        output.WriteLine($"  --> Visible (non-hidden): {visibleCount}");
    }

    [Fact]
    public void Inspect_StringPrimitive_FirstName()
    {
        InspectType(typeof(FirstName));
        Assert.True(true); // always pass — output only
    }

    [Fact]
    public void Inspect_StrongId_Guid_CustomerId()
    {
        InspectType(typeof(CustomerId));
        Assert.True(true);
    }

    [Fact]
    public void Inspect_NumericPrimitive_Score()
    {
        InspectType(typeof(Score));
        Assert.True(true);
    }

    [Fact]
    public void Inspect_ValueObject_Address()
    {
        InspectType(typeof(Address));
        Assert.True(true);
    }
}
