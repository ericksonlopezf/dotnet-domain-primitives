// Copyright © Erickson Lopez. MIT License.
using System;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.Benchmarks;

[StrongId<Guid>]
public readonly partial record struct CustomerId;

[StringPrimitive]
[Email]
public readonly partial record struct EmailAddress;

[MemoryDiagnoser]
public class DomainPrimitiveBenchmarks
{
    private static readonly Guid _rawGuid = Guid.NewGuid();
    private static readonly string _rawEmail = "user@example.com";
    private static readonly string _rawGuidString = _rawGuid.ToString();

    [Benchmark(Baseline = true)]
    public Guid RawGuid()
    {
        return _rawGuid;
    }

    [Benchmark]
    public CustomerId PrimitiveGuid()
    {
        return CustomerId.Create(_rawGuid);
    }

    [Benchmark]
    public CustomerId PrimitiveGuid_TryParse()
    {
        CustomerId.TryParse(_rawGuidString, null, out var result);
        return result;
    }

    [Benchmark]
    public EmailAddress PrimitiveEmail_Create()
    {
        return EmailAddress.Create(_rawEmail);
    }

    [Benchmark]
    public string PrimitiveEmail_JsonSerialize()
    {
        return System.Text.Json.JsonSerializer.Serialize(EmailAddress.Create(_rawEmail));
    }

    [Benchmark]
    public EmailAddress PrimitiveEmail_JsonDeserialize()
    {
        return System.Text.Json.JsonSerializer.Deserialize<EmailAddress>($"\"{_rawEmail}\"");
    }
}


[StrongId<Guid>]
public readonly partial record struct Size17LayoutId;

[MemoryDiagnoser]
public class StructLayoutBenchmark
{
    private CustomerId[] _defaultArray = null!;
    private Size17LayoutId[] _size17Array = null!;

    [Params(10_000, 100_000)]
    public int ArraySize { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _defaultArray = new CustomerId[ArraySize];
        _size17Array = new Size17LayoutId[ArraySize];

        for (int i = 0; i < ArraySize; i++)
        {
            var guid = Guid.NewGuid();
            _defaultArray[i] = CustomerId.Create(guid);
            _size17Array[i] = Size17LayoutId.Create(guid);
        }
    }

    [Benchmark(Baseline = true)]
    public int IterateDefault()
    {
        int count = 0;
        foreach (var id in _defaultArray)
        {
            if (id.IsDefault) count++;
        }
        return count;
    }

    [Benchmark]
    public int IterateSize17()
    {
        int count = 0;
        foreach (var id in _size17Array)
        {
            if (id.IsDefault) count++;
        }
        return count;
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }
}

