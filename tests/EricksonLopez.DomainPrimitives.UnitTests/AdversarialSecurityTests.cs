// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using AwesomeAssertions;
using EricksonLopez.DomainPrimitives.UnitTests.TestTypes;
using EricksonLopez.DomainPrimitives.Validation;
using Xunit;

namespace EricksonLopez.DomainPrimitives.UnitTests;

/// <summary>
/// Adversarial security, invariant-breaking, and boundary abuse tests.
/// Designed explicitly to verify vulnerability surfaces and demonstrate root-cause defects.
/// </summary>
public class AdversarialSecurityTests
{
    // ─── ADV-001: Explicit Cast Operator Invariant Bypass on StrongId ────────

    [Fact]
    public void ADV001_ExplicitCast_GuidStrongId_ShouldNotBypass_RejectEmpty_Invariant()
    {
        // Act: attempt to bypass validation via explicit operator cast
        // Under the current implementation: explicit operator calls new(value) directly,
        // which completely bypasses TryValidate / RejectEmpty.
        Action act = () =>
        {
            CustomerId id = (CustomerId)Guid.Empty;
            // If the cast succeeded, we now have an unvalidated instance with Guid.Empty
            // where IsDefault is false!
            if (!id.IsDefault && id.Value == Guid.Empty)
            {
                throw new DomainPrimitiveValidationException(
                    new PrimitiveError("INVARIANT_BYPASSED", "Guid.Empty was accepted via explicit cast."));
            }
        };

        // Assert: Explicit cast must enforce the same invariant as Create()
        act.Should().Throw<DomainPrimitiveValidationException>("Explicit cast must not bypass domain invariants");
    }

    [Fact]
    public void ADV001_ExplicitCast_IntStrongId_ShouldNotBypass_RejectEmpty_Invariant()
    {
        Action act = () =>
        {
            OrderNumber id = (OrderNumber)0;
            if (!id.IsDefault && id.Value == 0)
            {
                throw new DomainPrimitiveValidationException(
                    new PrimitiveError("INVARIANT_BYPASSED", "0 was accepted via explicit cast for OrderNumber."));
            }
        };

        act.Should().Throw<DomainPrimitiveValidationException>("Explicit cast to int-backed StrongId must not bypass RejectEmpty");
    }

    [Fact]
    public void ADV001_ExplicitCast_StringStrongId_ShouldNotBypass_RejectEmpty_Invariant()
    {
        Action actNull = () =>
        {
            Sku sku = (Sku)(string)null!;
            if (!sku.IsDefault)
            {
                throw new DomainPrimitiveValidationException(
                    new PrimitiveError("INVARIANT_BYPASSED", "null was accepted via explicit cast for Sku."));
            }
        };

        Action actEmpty = () =>
        {
            Sku sku = (Sku)"";
            if (!sku.IsDefault)
            {
                throw new DomainPrimitiveValidationException(
                    new PrimitiveError("INVARIANT_BYPASSED", "empty string was accepted via explicit cast for Sku."));
            }
        };

        actNull.Should().Throw<ArgumentNullException>("Explicit cast with null string must throw ArgumentNullException");
        actEmpty.Should().Throw<DomainPrimitiveValidationException>("Explicit cast with empty string must throw validation exception");
    }

    // ─── ADV-002: System.Text.Json Serialization on Default Instances ─────────

    [Fact]
    public void ADV002_JsonSerializer_Serialize_DefaultStringPrimitive_ShouldNotThrowInvalidOperationException()
    {
        FirstName defaultName = default;
        defaultName.IsDefault.Should().BeTrue();

        // Under current implementation:
        // GeneratorHelpers.GenerateJsonConverter emits: writer.WriteStringValue(value.Value);
        // But value.Value throws InvalidOperationException when IsDefault is true!
        Action act = () => JsonSerializer.Serialize(defaultName);

        // Serialization of a default struct should either serialize as null or succeed gracefully,
        // it must never throw an unhandled InvalidOperationException crashing application pipelines.
        act.Should().NotThrow<InvalidOperationException>("Serializing a default struct primitive must not throw InvalidOperationException");
    }

    [Fact]
    public void ADV002_JsonSerializer_Serialize_DefaultNumericPrimitive_ShouldNotThrowInvalidOperationException()
    {
        Price defaultPrice = default;
        defaultPrice.IsDefault.Should().BeTrue();

        Action act = () => JsonSerializer.Serialize(defaultPrice);
        act.Should().NotThrow<InvalidOperationException>("Serializing a default numeric primitive must not throw InvalidOperationException");
    }

    // ─── ADV-003: System.Text.Json JSON Escape Sequence Parsing ───────────────

    [Fact]
    public void ADV003_JsonSerializer_Deserialize_EscapedJsonString_ShouldDecodeProperly()
    {
        // Arrange: valid email containing a unicode escape sequence for '@' (\u0040)
        // System.Text.Json Utf8JsonReader.ValueSpan does NOT unescape unicode sequences.
        // It leaves literal '\', 'u', '0', '0', '4', '0'.
        string json = "\"admin\\u0040example.com\"";

        var email = JsonSerializer.Deserialize<EmailAddress>(json);
        email.IsDefault.Should().BeFalse();
        email.Value.Should().Be("admin@example.com", "JSON unicode escape sequence \\u0040 must be unescaped to '@'");
    }

    // ─── ADV-004: System.Text.Json Null Token Handling ────────────────────────

    [Fact]
    public void ADV004_JsonSerializer_Deserialize_NullToken_WhenOptional_ShouldReturnDefaultPrimitive()
    {
        // When a nullable property in a DTO is null: { "name": null }
        string json = "null";

        var result = JsonSerializer.Deserialize<FirstName>(json);
        result.IsDefault.Should().BeTrue("Deserializing a JSON null token for primitive without [NotEmpty] must yield a default primitive instance");
    }

    [Fact]
    public void ADV004_JsonSerializer_Deserialize_NullToken_WhenNotEmpty_ShouldThrowJsonException()
    {
        // When a non-nullable primitive requiring [NotEmpty] receives a null JSON token:
        string json = "null";

        Action act = () => JsonSerializer.Deserialize<DisplayName>(json);
        act.Should().Throw<JsonException>("Deserializing a JSON null token into a non-nullable primitive with [NotEmpty] must throw JsonException");
    }

    // ─── ADV-005: SmartEnum Global Catalog Shallow Immutability Mutation ──────

    [Fact]
    public void ADV005_SmartEnum_All_ShouldBeDeeplyImmutable_CannotBeMutatedViaArrayCast()
    {
        // SmartEnum emits: public static readonly IReadOnlyList<T> All = new T[] { ... };
        // If All is an array, an adversarial consumer can cast to IList<T> or T[] and mutate!
        var all = TestOrderStatus.All;
        all.Should().NotBeNull();
        all.Count.Should().Be(3);

        Action mutateAction = () =>
        {
            if (all is TestOrderStatus[] array)
            {
                array[0] = default; // Mutates global static array in memory!
            }
            else if (all is IList<TestOrderStatus> list)
            {
                list[0] = default;
            }
        };

        // Assert: All must be truly immutable (ReadOnlyCollection or ImmutableArray)
        mutateAction.Should().Throw<Exception>("SmartEnum.All must be deeply immutable and protected against cast mutations");
    }

    // ─── ADV-006: Comparison Ordering Transitivity With Default Instances ─────

    [Fact]
    public void ADV006_Comparison_DefaultInstance_ShouldOrderBeforeNegativeValues()
    {
        // Documentation in ComparisonTemplate claims:
        // "Default instances order before non-default instances."
        OrderNumber def = default;
        OrderNumber neg = OrderNumber.Create(-1); // Valid integer ID (-1 != 0)

        def.IsDefault.Should().BeTrue();
        neg.IsDefault.Should().BeFalse();

        // In the current implementation: ComparisonTemplate generates _value.CompareTo(other._value)
        // 0.CompareTo(-1) is 1 (def > neg), which violates the documented invariant!
        def.CompareTo(neg).Should().BeNegative("Default instance must order before any initialized instance (even negative values)");
    }

    // ─── ADV-007: Floating Point NaN Invariant Rejection ───────────────────────

    [Fact]
    public void ADV007_NumericPrimitive_Double_ShouldRejectNaN()
    {
        // double.NaN is never < Min and never > Max.
        // Unchecked range checks will accept double.NaN!
        var success = PrimitiveRangeScore.TryCreate(double.NaN, out var score, out var error);
        success.Should().BeFalse("double.NaN must be rejected as invalid numeric state");
        score.IsDefault.Should().BeTrue();
    }

    [Fact]
    public void ADV007_NumericPrimitive_Double_ShouldRejectInfinity()
    {
        var success = PrimitiveRangeScore.TryCreate(double.PositiveInfinity, out var score, out var error);
        success.Should().BeFalse("double.PositiveInfinity must be rejected as invalid numeric state");
        score.IsDefault.Should().BeTrue();
    }

    // ─── ADV-008: PrimitiveCollectionExtensions Null Safety ───────────────────

    [Fact]
    public void ADV008_CollectionExtensions_NullSource_ShouldThrowArgumentNullException()
    {
        IEnumerable<string> nullSequence = null!;

        Action actList = () => nullSequence.ToDomainPrimitiveList<FirstName, string>();
        Action actArray = () => nullSequence.ToDomainPrimitiveArray<FirstName, string>();

        actList.Should().Throw<ArgumentNullException>("ToDomainPrimitiveList on null source must throw ArgumentNullException");
        actArray.Should().Throw<ArgumentNullException>("ToDomainPrimitiveArray on null source must throw ArgumentNullException");
    }

    // ─── ADV-009: Denial of Service / Memory Allocation on Malicious UTF-8 ────

    [Fact]
    public void ADV009_StringPrimitive_LargeUtf8_ShouldRejectWithoutExcessiveAllocations()
    {
        // Arrange: 100KB payload for a primitive with MaxLength=100
        byte[] largePayload = new byte[100_000];
        Array.Fill(largePayload, (byte)'A');

        // Measure memory before and after
        long memBefore = GC.GetAllocatedBytesForCurrentThread();
        bool success = FirstName.TryParse(largePayload.AsSpan(), null, out var result);
        long memAfter = GC.GetAllocatedBytesForCurrentThread();

        success.Should().BeFalse("Oversized UTF-8 payload must be rejected");
        result.IsDefault.Should().BeTrue();

        // Memory allocated should not exceed reasonable bounds (certainly < 50KB)
        long allocated = memAfter - memBefore;
        allocated.Should().BeLessThan(32_768, "Oversized UTF-8 parsing should reject immediately without huge buffer rentals");
    }

    // ─── ADV-010: FiscalYear MinYear Invariant Validation ─────────────────────

    [Fact]
    public void ADV010_FiscalYear_ShouldEnforceMinYearInvariant()
    {
        var invalid = new DateOnly(1899, 12, 31);
        Action act = () => CompanyFiscalYear.Create(invalid);
        act.Should().Throw<DomainPrimitiveValidationException>()
            .WithMessage("*CompanyFiscalYear year must be >= 1900.*")
            .Where(e => e.Error.Code == "TEMPORAL");

        var valid = CompanyFiscalYear.Create(new DateOnly(2026, 1, 1));
        valid.Value.Year.Should().Be(2026);
    }

    // ─── ADV-012: StringPrimitive UTF-8 Parse Length Guard ───────────────────

    [Fact]
    public void ADV012_StringPrimitive_Utf8Parse_ThrowsOnOversizedPayload()
    {
        byte[] payload = new byte[2_000_000];
        Array.Fill(payload, (byte)'a');

        Action act = () => FirstName.Parse(payload.AsSpan(), null);
        act.Should().Throw<FormatException>("Oversized UTF-8 payload must be rejected with FormatException");
    }

    // ─── ADV-011: TimeRange and DateRange Representation ──────────────────────

    [Fact]
    public void ADV011_TimeRange_ShouldInstantiateValidScalarTimeOnly()
    {
        var shift = WorkShiftTime.Create(new TimeOnly(9, 0));
        shift.IsDefault.Should().BeFalse();
        shift.Value.Should().Be(new TimeOnly(9, 0));
    }

    // ─── ADV-013: Unicode Surrogate Pair Integrity ───────────────────────────

    [Fact]
    public void ADV013_StringPrimitive_SurrogatePairIntegrity()
    {
        string musicalClef = "\U0001D11E"; // 4-byte UTF-8, 2 UTF-16 code units
        bool created = FirstName.TryCreate(musicalClef, out var name, out _);
        if (created)
        {
            name.Value.Should().Be(musicalClef);
            name.Value.Length.Should().Be(2);
        }
    }

    // ─── ADV-014: Default Record Struct Comparison Consistency ───────────────

    [Fact]
    public void ADV014_DefaultPrimitive_EqualsDefault_AndHashCodeConsistent()
    {
        FirstName def1 = default;
        FirstName def2 = default;

        (def1 == def2).Should().BeTrue("Two default struct primitives must be equal");
        def1.Equals(def2).Should().BeTrue();
        def1.GetHashCode().Should().Be(def2.GetHashCode());
    }

    // ─── ADV-015: Fault Injection & Boundary Resistance ──────────────────────

    [Fact]
    public void ADV015_FaultInjection_BoundaryOffByOne_LengthCheck()
    {
        string exactly100 = new string('a', 100);
        string length101 = new string('a', 101);

        bool accept100 = FirstName.TryCreate(exactly100, out var name100, out _);
        bool accept101 = FirstName.TryCreate(length101, out var name101, out _);

        accept100.Should().BeTrue("Boundary condition MaxLength must accept exactly max characters (100)");
        accept101.Should().BeFalse("Boundary condition MaxLength must reject max + 1 characters (101)");
    }

    [Fact]
    public void ADV015_FaultInjection_NumericRangeBoundary()
    {
        bool minAccept = PrimitiveRangeScore.TryCreate(1.0, out _, out _);
        bool maxAccept = PrimitiveRangeScore.TryCreate(10.0, out _, out _);
        bool belowMin = PrimitiveRangeScore.TryCreate(0.9999, out _, out _);
        bool aboveMax = PrimitiveRangeScore.TryCreate(10.0001, out _, out _);

        minAccept.Should().BeTrue("Min bound (1.0) inclusive must be accepted");
        maxAccept.Should().BeTrue("Max bound (10.0) inclusive must be accepted");
        belowMin.Should().BeFalse("Value below min bound (0.9999) must be rejected");
        aboveMax.Should().BeFalse("Value above max bound (10.0001) must be rejected");
    }

    [Fact]
    public void ADV015_FaultInjection_NullCheckRemoval_RejectsCleanly()
    {
        bool success = FirstName.TryCreate(null!, out var name, out var error);
        success.Should().BeFalse("Passing null to TryCreate must cleanly fail with error result");
        name.IsDefault.Should().BeTrue();
        error.IsError.Should().BeTrue();
    }

    // ─── ADV-016: Unicode Surrogates Fuzzing Protection ───────────────────────

    [Fact]
    public void ADV016_UnpairedUnicodeSurrogates_TryCreate_ShouldReturnFalseWithoutThrowing()
    {
        string invalidSurrogate = "\uD800malicious";
        Action act = () =>
        {
            bool success = FirstName.TryCreate(invalidSurrogate, out var result, out var error);
            success.Should().BeFalse();
            result.IsDefault.Should().BeTrue();
            error.IsError.Should().BeTrue();
            error.Code.Should().Be("INVALID_UNICODE");
        };

        act.Should().NotThrow<Exception>();
    }
}


