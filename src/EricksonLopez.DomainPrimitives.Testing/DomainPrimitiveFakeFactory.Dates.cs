// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;

namespace EricksonLopez.DomainPrimitives.Testing;

public static partial class DomainPrimitiveFakeFactory
{
    /// <summary>
    /// Provides deterministic fake test data for date-based domain primitives.
    /// </summary>
    public static class Dates
    {
        /// <summary>Gets current date in UTC for date-based testing.</summary>
        /// <remarks>
        /// <b>Timezone note:</b> Always uses <c>DateTime.UtcNow</c> (UTC), not the local system clock.
        /// This ensures consistent behavior across CI runners in different timezones.
        /// If your domain model uses local time, be aware that date-boundary tests (e.g., "today is valid",
        /// "yesterday is invalid") may behave differently if evaluated near midnight UTC.
        /// </remarks>
        public static DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);

        /// <summary>Gets a valid birth date for deterministic testing (30 years in the past, UTC).</summary>
        /// <remarks>
        /// This is a live-computed property (not static readonly). Each call returns 30 years before
        /// the current UTC date. For snapshot-based tests, capture the value once and reuse it.
        /// </remarks>
        public static DateOnly ValidBirthDate => Today.AddYears(-30);

        /// <summary>Gets a past date (5 years ago) for past date testing.</summary>
        public static DateOnly PastDate => Today.AddYears(-5);

        /// <summary>Gets a future date (5 years ahead) for expiration date testing.</summary>
        public static DateOnly FutureDate => Today.AddYears(5);

        /// <summary>Gets a set of valid future expiration dates for testing.</summary>
        public static List<DateOnly> ValidExpirationDates { get; } =
        [
            Today.AddDays(1), Today.AddMonths(6), Today.AddYears(2)
        ];

        /// <summary>Gets a set of invalid expiration dates (in the past) for negative testing.</summary>
        public static List<DateOnly> InvalidExpirationDates { get; } =
        [
            Today.AddDays(-1), Today.AddYears(-1)
        ];

        /// <summary>Gets valid business dates (Monday-Friday) for testing, computed dynamically from today.</summary>
        /// <remarks>Dates are computed at class initialization time to avoid clock drift within a single test run.</remarks>
        public static List<DateOnly> ValidBusinessDates { get; } = GetNextWeekdays(2);

        /// <summary>Gets invalid business dates (weekends) for testing.</summary>
        public static List<DateOnly> InvalidBusinessDates { get; } = GetNextWeekendDays(2);

        /// <summary>Finds the next <paramref name="count"/> weekday dates starting from tomorrow.</summary>
        private static List<DateOnly> GetNextWeekdays(int count)
        {
            var result = new List<DateOnly>(count);
            var candidate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1);
            while (result.Count < count)
            {
                if (candidate.DayOfWeek != DayOfWeek.Saturday && candidate.DayOfWeek != DayOfWeek.Sunday)
                    result.Add(candidate);
                candidate = candidate.AddDays(1);
            }
            return result;
        }

        /// <summary>Finds the next <paramref name="count"/> weekend dates starting from tomorrow.</summary>
        private static List<DateOnly> GetNextWeekendDays(int count)
        {
            var result = new List<DateOnly>(count);
            var candidate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1);
            while (result.Count < count)
            {
                if (candidate.DayOfWeek == DayOfWeek.Saturday || candidate.DayOfWeek == DayOfWeek.Sunday)
                    result.Add(candidate);
                candidate = candidate.AddDays(1);
            }
            return result;
        }

        /// <summary>Gets valid fiscal years for testing.</summary>
        public static IReadOnlyList<int> ValidFiscalYears { get; } = [1900, 2024, 2026];

        /// <summary>Gets invalid fiscal years for testing.</summary>
        public static IReadOnlyList<int> InvalidFiscalYears { get; } = [1899, 0, -1];
    }
}


