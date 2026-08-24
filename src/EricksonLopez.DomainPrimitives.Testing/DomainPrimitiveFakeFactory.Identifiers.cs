// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;

namespace EricksonLopez.DomainPrimitives.Testing;

public static partial class DomainPrimitiveFakeFactory
{
    /// <summary>Provides deterministic fake test data for identifier-based domain primitives.</summary>
    public static class Identifiers
    {
        /// <summary>Gets a set of known non-empty GUIDs for testing.</summary>
        public static IReadOnlyList<Guid> ValidGuids { get; } =
        [
            new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
            new Guid("00000000-0000-0000-0000-000000000001"),
            new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff")
        ];

        /// <summary>Gets the first valid GUID — convenient for single-value tests.</summary>
        public static Guid ValidGuid => ValidGuids[0];

        /// <summary>Gets a set of GUID strings for testing string-based parsing.</summary>
        public static IReadOnlyList<string> ValidGuidStrings { get; } =
        [
            "3fa85f64-5717-4562-b3fc-2c963f66afa6",
            "3FA85F64-5717-4562-B3FC-2C963F66AFA6",  // Upper case
            "{3fa85f64-5717-4562-b3fc-2c963f66afa6}", // With braces
            "3fa85f6457174562b3fc2c963f66afa6"         // Without hyphens
        ];

        /// <summary>Gets a set of invalid GUID strings for negative testing.</summary>
        public static IReadOnlyList<string> InvalidGuidStrings { get; } =
        [
            "",
            "not-a-guid",
            "3fa85f64-5717-4562-b3fc",  // Too short
            "3fa85f64-5717-4562-b3fc-2c963f66afa6-extra" // Too long
        ];
    }
}

