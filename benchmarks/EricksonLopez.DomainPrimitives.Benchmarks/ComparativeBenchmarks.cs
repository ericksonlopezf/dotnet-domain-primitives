using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using EricksonLopez.DomainPrimitives;
using StronglyTypedIds;
using ValueOf;

namespace EricksonLopez.DomainPrimitives.Benchmarks;

// ─── Guid Strong ID Primitives ───────────────────────────────────────────────

// EricksonLopez.DomainPrimitives
[StrongId<System.Guid>]
public readonly partial record struct DPUserId;

// Vogen
[Vogen.ValueObject(typeof(System.Guid))]
public partial struct VogenUserId { }

// StronglyTypedId
[global::StronglyTypedIds.StronglyTypedId]
public partial struct StronglyTypedUserId { }

// ValueOf
public class ValueOfUserId : ValueOf<System.Guid, ValueOfUserId> { }

// Meziantou
[global::Meziantou.Framework.Annotations.StronglyTypedId(typeof(System.Guid))]
public readonly partial struct MeziantouUserId { }

// TinyTypes pattern
public readonly struct TinyTypeUserId : System.IEquatable<TinyTypeUserId>
{
    public System.Guid Value { get; }
    public TinyTypeUserId(System.Guid value) => Value = value;
    public bool Equals(TinyTypeUserId other) => Value.Equals(other.Value);
    public override bool Equals(object? obj) => obj is TinyTypeUserId other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
}

// ─── Domain Specific Primitives (String & Numeric) ──────────────────────────

[Email]
public readonly partial record struct DPEmailAddress;

[Money(Min = 0)]
public readonly partial record struct DPMoney;

// ─── ValueObject Primitives ────────────────────────────────────────────────
[ValueObject]
public readonly partial record struct DPAddress
{
    public string Street { get; init; }
    public string City { get; init; }
}

// ─── SmartEnum Primitives ──────────────────────────────────────────────────
[SmartEnum<int>]
public readonly partial record struct DPOrderStatus
{
    public static readonly DPOrderStatus Pending = new(1, "Pending");
    public static readonly DPOrderStatus Shipped = new(2, "Shipped");
}

// ─── Comparative Benchmarks ──────────────────────────────────────────────────

[MemoryDiagnoser]
[RankColumn]
[SimpleJob(RuntimeMoniker.Net90)]
public class ComparativeBenchmarks
{
    private System.Guid _testGuid = System.Guid.NewGuid();
    private string _testGuidString = string.Empty;
    private DPUserId _dpUserId;
    private string _jsonSerializedGuid = string.Empty;
    private string _jsonSerializedDPUserId = string.Empty;
    private char[] _formatBuffer = new char[36]; // Guid-sized output buffer
    private byte[] _utf8Buffer = new byte[36];   // UTF-8 Guid-sized output buffer

    private string _testEmailString = "user.name+tag@domain.example.com";
    private decimal _testMoneyValue = 199.99m;
    private DPEmailAddress _dpEmailAddress;
    private DPMoney _dpMoney1;
    private DPMoney _dpMoney2;
    private DPAddress _dpAddress;
    private DPOrderStatus _dpOrderStatus;

    [GlobalSetup]
    public void Setup()
    {
        _testGuidString = _testGuid.ToString();
        _dpUserId = DPUserId.Create(_testGuid);
        _jsonSerializedGuid = System.Text.Json.JsonSerializer.Serialize(_testGuid);
        _jsonSerializedDPUserId = System.Text.Json.JsonSerializer.Serialize(_dpUserId);

        _dpEmailAddress = DPEmailAddress.Create(_testEmailString);
        _dpMoney1 = DPMoney.Create(100.50m);
        _dpMoney2 = DPMoney.Create(99.49m);
        _dpAddress = DPAddress.Create("123 Main St", "Metropolis");
        _dpOrderStatus = DPOrderStatus.Pending;
    }

    // ─── Strong ID Benchmarks ────────────────────────────────────────────────

    [Benchmark(Baseline = true)]
    public System.Guid RawGuid_Create()
    {
        return _testGuid;
    }

    [Benchmark]
    public DPUserId DomainPrimitives_Create()
    {
        return DPUserId.Create(_testGuid);
    }

    [Benchmark]
    public VogenUserId Vogen_Create()
    {
        return VogenUserId.From(_testGuid);
    }

    [Benchmark]
    public StronglyTypedUserId StronglyTypedId_Create()
    {
        return new StronglyTypedUserId(_testGuid);
    }

    [Benchmark]
    public ValueOfUserId ValueOf_Create()
    {
        return ValueOfUserId.From(_testGuid);
    }

    [Benchmark]
    public MeziantouUserId Meziantou_Create()
    {
        return MeziantouUserId.FromGuid(_testGuid);
    }

    [Benchmark]
    public TinyTypeUserId TinyTypes_Create()
    {
        return new TinyTypeUserId(_testGuid);
    }

    [Benchmark]
    public System.Guid RawGuid_Parse()
    {
        return System.Guid.Parse(_testGuidString);
    }

    [Benchmark]
    public DPUserId DomainPrimitives_Parse()
    {
        return DPUserId.Parse(_testGuidString);
    }

    [Benchmark]
    public VogenUserId Vogen_Parse()
    {
        return VogenUserId.From(System.Guid.Parse(_testGuidString));
    }

    [Benchmark]
    public StronglyTypedUserId StronglyTypedId_Parse()
    {
        return new StronglyTypedUserId(System.Guid.Parse(_testGuidString));
    }

    [Benchmark]
    public ValueOfUserId ValueOf_Parse()
    {
        return ValueOfUserId.From(System.Guid.Parse(_testGuidString));
    }

    [Benchmark]
    public MeziantouUserId Meziantou_Parse()
    {
        return MeziantouUserId.Parse(_testGuidString);
    }

    [Benchmark]
    public TinyTypeUserId TinyTypes_Parse()
    {
        return new TinyTypeUserId(System.Guid.Parse(_testGuidString));
    }

    [Benchmark]
    public bool DomainPrimitives_EqualityCheck()
    {
        return _dpUserId.Equals(DPUserId.Create(_testGuid));
    }

    [Benchmark]
    public string RawGuid_ToString()
    {
        return _testGuid.ToString();
    }

    [Benchmark]
    public string DomainPrimitives_ToString()
    {
        return _dpUserId.ToString();
    }

    [Benchmark]
    public string Vogen_ToString()
    {
        return VogenUserId.From(_testGuid).ToString();
    }

    [Benchmark]
    public string StronglyTypedId_ToString()
    {
        return new StronglyTypedUserId(_testGuid).ToString();
    }

    [Benchmark]
    public string ValueOf_ToString()
    {
        return ValueOfUserId.From(_testGuid).ToString();
    }

    [Benchmark]
    public string Meziantou_ToString()
    {
        return MeziantouUserId.FromGuid(_testGuid).ToString();
    }

    [Benchmark]
    public string TinyTypes_ToString()
    {
        return new TinyTypeUserId(_testGuid).ToString() ?? string.Empty;
    }

    [Benchmark]
    public bool DomainPrimitives_TryParse()
    {
        return DPUserId.TryParse(_testGuidString, null, out _);
    }

    [Benchmark]
    public bool DomainPrimitives_SpanParse()
    {
        // ISpanParsable<T>.TryParse(ReadOnlySpan<char>): avoids string allocation entirely.
        return DPUserId.TryParse(_testGuidString.AsSpan(), null, out _);
    }

    [Benchmark]
    public bool DomainPrimitives_Utf8SpanParse()
    {
        // IUtf8SpanParsable<T>.TryParse(ReadOnlySpan<byte>): zero-copy parse from UTF-8 bytes.
        var utf8 = System.Text.Encoding.UTF8.GetBytes(_testGuidString);
        return DPUserId.TryParse(utf8, null, out _);
    }

    [Benchmark]
    public bool DomainPrimitives_SpanFormat()
    {
        // ISpanFormattable.TryFormat(): writes directly into a char buffer, no string allocation.
        return _dpUserId.TryFormat(_formatBuffer, out _, default, null);
    }

    [Benchmark]
    public bool DomainPrimitives_Utf8SpanFormat()
    {
        // IUtf8SpanFormattable.TryFormat(): writes directly into a byte buffer.
        return _dpUserId.TryFormat(_utf8Buffer, out _, default, null);
    }

    // ─── String Primitive Benchmarks ─────────────────────────────────────────

    [Benchmark]
    public DPEmailAddress StringPrimitive_Email_Create()
    {
        return DPEmailAddress.Create(_testEmailString);
    }

    [Benchmark]
    public bool StringPrimitive_Email_TryParse()
    {
        return DPEmailAddress.TryParse(_testEmailString, null, out _);
    }

    // ─── Numeric Primitive Benchmarks ────────────────────────────────────────

    [Benchmark]
    public DPMoney NumericPrimitive_Money_Create()
    {
        return DPMoney.Create(_testMoneyValue);
    }

    [Benchmark]
    public DPMoney NumericPrimitive_Money_Add()
    {
        return _dpMoney1 + _dpMoney2;
    }

    // ─── ValueObject Benchmarks ──────────────────────────────────────────────
    [Benchmark]
    public DPAddress ValueObject_Create()
    {
        return DPAddress.Create("123 Main St", "Metropolis");
    }

    // ─── SmartEnum Benchmarks ────────────────────────────────────────────────
    [Benchmark]
    public DPOrderStatus SmartEnum_FromValue()
    {
        return DPOrderStatus.FromValue(1);
    }

    // ─── JSON Benchmarks ─────────────────────────────────────────────────────

    [Benchmark]
    public string RawGuid_JsonSerialize()
    {
        return System.Text.Json.JsonSerializer.Serialize(_testGuid);
    }

    [Benchmark]
    public System.Guid RawGuid_JsonDeserialize()
    {
        return System.Text.Json.JsonSerializer.Deserialize<System.Guid>(_jsonSerializedGuid);
    }

    [Benchmark]
    public string DomainPrimitives_JsonSerialize()
    {
        return System.Text.Json.JsonSerializer.Serialize(_dpUserId);
    }

    [Benchmark]
    public DPUserId DomainPrimitives_JsonDeserialize()
    {
        return System.Text.Json.JsonSerializer.Deserialize<DPUserId>(_jsonSerializedDPUserId);
    }

    // ─── Integration Overhead Benchmarks ─────────────────────────────────────
    // These benchmarks measure the cost of the Dapper TypeHandler wrapper vs. raw primitive access.
    // They do NOT require an actual DB connection — they benchmark the type conversion layer only.

    private DPUserIdTypeHandler? _typeHandler;

    [GlobalSetup(Targets = new[] { nameof(Dapper_TypeHandler_SetValue), nameof(Dapper_TypeHandler_Parse) })]
    public void SetupTypeHandler()
    {
        Setup();
        _typeHandler = new DPUserIdTypeHandler();
    }

    [Benchmark]
    [BenchmarkCategory("Integration")]
    public void Dapper_TypeHandler_SetValue()
    {
        // Simulates the Dapper TypeHandler.SetValue() path (primitive → DB parameter).
        // This is called for every parameter in an INSERT/UPDATE statement.
        _typeHandler!.SimulateSetValue(_dpUserId);
    }

    [Benchmark]
    [BenchmarkCategory("Integration")]
    public DPUserId Dapper_TypeHandler_Parse()
    {
        // Simulates the Dapper TypeHandler.Parse() path (DB reader → domain primitive).
        // This is called for every row returned from a SELECT.
        return _typeHandler!.SimulateParse(_testGuid);
    }

    private DPUserIdValueConverter? _valueConverter;

    [GlobalSetup(Targets = new[] { nameof(EFCore_ValueConverter_ConvertToProvider), nameof(EFCore_ValueConverter_ConvertFromProvider) })]
    public void SetupValueConverter()
    {
        Setup();
        _valueConverter = new DPUserIdValueConverter();
    }

    [Benchmark]
    [BenchmarkCategory("Integration")]
    public System.Guid EFCore_ValueConverter_ConvertToProvider()
    {
        return _valueConverter!.SimulateConvertToProvider(_dpUserId);
    }

    [Benchmark]
    [BenchmarkCategory("Integration")]
    public DPUserId EFCore_ValueConverter_ConvertFromProvider()
    {
        return _valueConverter!.SimulateConvertFromProvider(_testGuid);
    }
}

/// <summary>
/// Minimal Dapper TypeHandler simulator for benchmarking type conversion overhead.
/// Does not require a live database connection.
/// </summary>
internal sealed class DPUserIdTypeHandler
{
    // Simulates TypeHandler.SetValue(): extracts the backing value from the primitive.
    public object SimulateSetValue(DPUserId id) => id.Value;

    // Simulates TypeHandler.Parse(): wraps a raw DB value into a domain primitive.
    public DPUserId SimulateParse(System.Guid raw) => DPUserId.Create(raw);
}

/// <summary>
/// Minimal EF Core ValueConverter simulator for benchmarking type conversion overhead.
/// Does not require a live database connection.
/// </summary>
internal sealed class DPUserIdValueConverter
{
    public System.Guid SimulateConvertToProvider(DPUserId id) => id.Value;
    public DPUserId SimulateConvertFromProvider(System.Guid raw) => DPUserId.Create(raw);
}

