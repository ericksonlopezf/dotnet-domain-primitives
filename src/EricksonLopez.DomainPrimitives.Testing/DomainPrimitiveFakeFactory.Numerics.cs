// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;

namespace EricksonLopez.DomainPrimitives.Testing;

public static partial class DomainPrimitiveFakeFactory
{
    /// <summary>Provides deterministic fake test data for numeric domain primitives.</summary>
    public static class Numerics
    {
        /// <summary>Gets a set of valid monetary amounts for testing.</summary>
        public static IReadOnlyList<decimal> ValidMoneyAmounts { get; } =
        [
            0m, 0.01m, 9.99m, 100m, 9999999.99m
        ];

        /// <summary>Gets the first valid money amount — convenient for single-value tests.</summary>
        public static decimal ValidMoneyAmount => ValidMoneyAmounts[0];

        /// <summary>Gets a set of invalid monetary amounts (negative) for negative testing.</summary>
        public static IReadOnlyList<decimal> InvalidMoneyAmounts { get; } =
        [
            -0.01m, -1m, decimal.MinValue
        ];

        /// <summary>Gets a set of valid age values for testing.</summary>
        public static IReadOnlyList<int> ValidAges { get; } = [0, 1, 18, 65, 100, 150];

        /// <summary>Gets the first valid age — convenient for single-value tests.</summary>
        public static int ValidAge => ValidAges[2]; // 18

        /// <summary>Gets a set of invalid age values for negative testing.</summary>
        public static IReadOnlyList<int> InvalidAges { get; } = [-1, 151, int.MaxValue];

        /// <summary>Gets a set of valid latitude values for testing.</summary>
        public static IReadOnlyList<double> ValidLatitudes { get; } = [-90.0, -45.5, 0.0, 45.5, 90.0];

        /// <summary>Gets a set of invalid latitude values for negative testing.</summary>
        public static IReadOnlyList<double> InvalidLatitudes { get; } = [-90.1, 90.1, double.MaxValue];

        /// <summary>Gets a set of valid longitude values for testing.</summary>
        public static IReadOnlyList<double> ValidLongitudes { get; } = [-180.0, -90.0, 0.0, 90.0, 180.0];

        /// <summary>Gets a set of invalid longitude values for negative testing.</summary>
        public static IReadOnlyList<double> InvalidLongitudes { get; } = [-180.1, 180.1, double.MinValue];

        /// <summary>Gets a set of valid percentages (0-100) for testing.</summary>
        public static IReadOnlyList<decimal> ValidPercentages { get; } = [0m, 25.5m, 50m, 100m];

        /// <summary>Gets a set of invalid percentages for negative testing.</summary>
        public static IReadOnlyList<decimal> InvalidPercentages { get; } = [-0.01m, 100.01m, 150m];

        /// <summary>Gets a set of valid weight values in kg for testing.</summary>
        public static IReadOnlyList<double> ValidWeights { get; } = [0.1, 70.5, 500.0, 1000.0];

        /// <summary>Gets a set of invalid weight values for negative testing.</summary>
        public static IReadOnlyList<double> InvalidWeights { get; } = [-1.0, 1000.1];

        /// <summary>Gets a set of valid height values in cm for testing.</summary>
        public static IReadOnlyList<double> ValidHeights { get; } = [1.0, 175.5, 290.0, 300.0];

        /// <summary>Gets a set of invalid height values for negative testing.</summary>
        public static IReadOnlyList<double> InvalidHeights { get; } = [-5.0, 300.1];

        /// <summary>Gets a set of valid distance values in meters for testing.</summary>
        public static IReadOnlyList<double> ValidDistances { get; } = [0.0, 1000.5, 40075000.0];

        /// <summary>Gets a set of invalid distance values for negative testing.</summary>
        public static IReadOnlyList<double> InvalidDistances { get; } = [-0.1, -100.0];

        /// <summary>Gets a set of valid temperature values in Celsius for testing.</summary>
        public static IReadOnlyList<double> ValidTemperatures { get; } = [-273.15, 0.0, 36.6, 100.0];

        /// <summary>Gets a set of invalid temperature values (below absolute zero) for negative testing.</summary>
        public static IReadOnlyList<double> InvalidTemperatures { get; } = [-273.16, -500.0];

        /// <summary>Gets a set of valid scores (0-100) for testing.</summary>
        public static IReadOnlyList<int> ValidScores { get; } = [0, 50, 100];

        /// <summary>Gets a set of invalid scores for negative testing.</summary>
        public static IReadOnlyList<int> InvalidScores { get; } = [-1, 101];

        /// <summary>Gets a set of valid non-negative quantities for testing.</summary>
        public static IReadOnlyList<int> ValidQuantities { get; } = [0, 1, 100, 1000];

        /// <summary>Gets a set of invalid quantities for negative testing.</summary>
        public static IReadOnlyList<int> InvalidQuantities { get; } = [-1, -100];

        /// <summary>Gets a set of valid prices for testing.</summary>
        public static IReadOnlyList<decimal> ValidPrices { get; } = [0m, 19.99m, 1500m];

        /// <summary>Gets a set of invalid prices for negative testing.</summary>
        public static IReadOnlyList<decimal> InvalidPrices { get; } = [-0.01m, -100m];

        /// <summary>Gets a set of valid tax rates for testing.</summary>
        public static IReadOnlyList<decimal> ValidTaxRates { get; } = [0m, 16m, 21m, 100m];

        /// <summary>Gets a set of invalid tax rates for negative testing.</summary>
        public static IReadOnlyList<decimal> InvalidTaxRates { get; } = [-0.1m, 100.1m];

        /// <summary>Gets a set of valid discount percentages for testing.</summary>
        public static IReadOnlyList<decimal> ValidDiscounts { get; } = [0m, 10m, 50m, 100m];

        /// <summary>Gets a set of invalid discount percentages for negative testing.</summary>
        public static IReadOnlyList<decimal> InvalidDiscounts { get; } = [-1m, 101m];
    }
}

