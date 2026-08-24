// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;

namespace EricksonLopez.DomainPrimitives.Testing;

public static partial class DomainPrimitiveFakeFactory
{
    /// <summary>Provides deterministic fake test data for shortcut domain primitives.</summary>
    public static class Shortcuts
    {
        /// <summary>Gets valid ISO currency codes for testing.</summary>
        public static IReadOnlyList<string> ValidCurrencyCodes { get; } = ["USD", "EUR", "GBP", "JPY", "CAD"];

        /// <summary>Gets valid IBAN strings for testing.</summary>
        public static IReadOnlyList<string> ValidIBANs { get; } = ["DE89370400440532013000", "GB29NWBK60161331926819"];

        /// <summary>Gets valid ISBN strings for testing.</summary>
        public static IReadOnlyList<string> ValidISBNs { get; } = ["978-3-16-148410-0", "978-0-306-40615-7"];

        /// <summary>Gets valid VIN strings for testing.</summary>
        public static IReadOnlyList<string> ValidVINs { get; } = ["1HGCR2F83HA000000", "1FA6P8CF0H5100000"];

        /// <summary>Gets valid Hex Colors for testing.</summary>
        public static IReadOnlyList<string> ValidHexColors { get; } = ["#FF5733", "#00FF00", "#000000", "#FFFFFF"];

        /// <summary>Gets valid rating values (0.0 to 5.0) for testing.</summary>
        public static IReadOnlyList<decimal> ValidRatings { get; } = [0.0m, 2.5m, 4.0m, 4.8m, 5.0m];
    }
}
