// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using EricksonLopez.DomainPrimitives.UnitTests.TestTypes;
using EricksonLopez.DomainPrimitives.Validation;
using Xunit;

namespace EricksonLopez.DomainPrimitives.UnitTests;

/// <summary>
/// Concurrency stress testing, high-thread count contention tests, and randomized fuzzing attacks.
/// </summary>
public class ConcurrencyAndFuzzingTests
{
    // ─── Concurrency Stress Tests ─────────────────────────────────────────────

    [Fact]
    public void Concurrency_Parallel_Parsing_ShouldBeThreadSafe()
    {
        const int iterations = 10_000;
        var errors = new ConcurrentBag<Exception>();

        Parallel.For(0, iterations, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount * 2 }, i =>
        {
            try
            {
                // Concurrently parse string primitives
                string validName = $"User_{i % 100}";
                var name = FirstName.Parse(validName, null);
                if (name.Value != validName)
                    throw new InvalidOperationException($"Corrupted value: expected {validName}, got {name.Value}");

                // Concurrently parse StrongIds
                var guid = Guid.NewGuid();
                var id = CustomerId.Parse(guid.ToString(), null);
                if (id.Value != guid)
                    throw new InvalidOperationException($"Corrupted Guid: expected {guid}, got {id.Value}");

                // Concurrently parse SmartEnums
                if (!TestOrderStatus.TryFromValue(1, out var status) || status != TestOrderStatus.Pending)
                    throw new InvalidOperationException("SmartEnum TryFromValue failed under concurrency");
            }
            catch (Exception ex)
            {
                errors.Add(ex);
            }
        });

        errors.Should().BeEmpty("Concurrent parsing must not produce race conditions or data corruption");
    }

    [Fact]
    public async Task Concurrency_TaskWhenAll_HighContention_ShouldBeDeterministic()
    {
        const int taskCount = 100;
        var tasks = new Task[taskCount];

        for (int i = 0; i < taskCount; i++)
        {
            int index = i;
            tasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < 500; j++)
                {
                    var price = Price.Create(100.50m + (index % 10));
                    price.Value.Should().Be(100.50m + (index % 10));

                    var rating = MovieRating.Create(4.5m);
                    rating.Value.Should().Be(4.5m);
                }
            });
        }

        await Task.WhenAll(tasks);
    }

    // ─── Fuzzing Attacks ──────────────────────────────────────────────────────

    [Fact]
    public void Fuzzing_StringPrimitive_PathologicalUnicodeInputs_ShouldNeverCrashWithUnhandledExceptions()
    {
        // Pathological inputs: null bytes, surrogate pairs, right-to-left marks, homoglyphs, BOM, zero-width spaces
        string[] pathologicalInputs =
        [
            "\0\0\0\0",
            "\uFEFFhello",
            "\u200B\u200C\u200D\uFEFF",
            "\uD83D\uDE00", // Emoji surrogate pair 😀
            "\uD800",       // Unpaired high surrogate
            "\uDC00",       // Unpaired low surrogate
            new string('\u0301', 50), // Combining acute accents
            "admin@\u202Ereversed.com", // RTL override
            "user\t\r\n\0name",
            new string('A', 4095),
            new string('A', 4096),
            new string('A', 4097),
            new string('A', 65536)
        ];

        foreach (var input in pathologicalInputs)
        {
            // Must return bool without unhandled runtime crash (AccessViolationException, ArgumentException, etc.)
            Action act = () =>
            {
                _ = FirstName.TryCreate(input, out _, out _);
                _ = EmailAddress.TryCreate(input, out _, out _);
                _ = ProductCode.TryCreate(input, out _, out _);
            };

            act.Should().NotThrow<AccessViolationException>();
            act.Should().NotThrow<NullReferenceException>();
        }
    }

    [Fact]
    public void Fuzzing_NumericPrimitive_ExtremeFloatingPoint_ShouldHandleGracefully()
    {
        double[] fuzzedDoubles =
        [
            double.NaN,
            double.PositiveInfinity,
            double.NegativeInfinity,
            double.Epsilon,
            -double.Epsilon,
            double.MinValue,
            double.MaxValue,
            0.0,
            -0.0,
            1e-300,
            1e300
        ];

        foreach (var val in fuzzedDoubles)
        {
            Action act = () =>
            {
                _ = Distance.TryCreate(val, out _, out _);
                _ = PrimitiveRangeScore.TryCreate(val, out _, out _);
            };

            act.Should().NotThrow<AccessViolationException>();
        }
    }

    [Fact]
    public void Fuzzing_Randomized_Bytes_Utf8SpanParsing_ShouldBeResilient()
    {
        var rng = RandomNumberGenerator.Create();
        byte[] randomBytes = new byte[256];

        for (int i = 0; i < 500; i++)
        {
            rng.GetBytes(randomBytes);

            // Parsing random bytes as UTF-8 must never cause memory corruption, AV, or unhandled exceptions
            Action act = () =>
            {
                _ = FirstName.TryParse(randomBytes.AsSpan(), null, out _);
                _ = EmailAddress.TryParse(randomBytes.AsSpan(), null, out _);
            };

            act.Should().NotThrow<AccessViolationException>();
            act.Should().NotThrow<IndexOutOfRangeException>();
            act.Should().NotThrow<ArrayTypeMismatchException>();
        }
    }
}
