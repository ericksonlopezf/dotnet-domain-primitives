// Copyright © Erickson Lopez. MIT License.
// ============================================================================
// CHAPTER 19: UNIT TESTING DOMAIN PRIMITIVES
// ============================================================================
// In this chapter you will learn the testing toolkit provided by the library:
//
// COVERED APIS (EricksonLopez.DomainPrimitives.Testing):
//  1. DomainPrimitiveFakeFactory     — deterministic fake data for strings,
//                                      numerics, dates, and identifiers.
//  2. DomainPrimitiveTestBuilder     — creation helpers for test scenarios.
//  3. DomainPrimitiveScenarios       — pre-defined parameterized test inputs.
//  4. DomainPrimitiveAssertionsExtensions — FluentAssertions / AwesomeAssertions
//                                            extension methods (xUnit only).
//  5. DomainPrimitiveVerifyExtensions — Verify snapshot serializer integration.
//
// NOTE:
//  DomainPrimitiveAssertionsExtensions and DomainPrimitiveVerifyExtensions
//  require a test runner (xUnit). Their signatures are documented here, but
//  cannot be run in a console app. The OfficialSample.Tests project contains
//  real xUnit usage.
// ============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using Chapter19;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.DomainPrimitives.Testing;
using EricksonLopez.DomainPrimitives.Validation;

Console.WriteLine("=========================================================");
Console.WriteLine(" 📘 CHAPTER 19: UNIT TESTING — TESTING LIBRARY SHOWCASE");
Console.WriteLine("=========================================================\n");

int passedTests = 0;
int totalTests = 0;

// ─────────────────────────────────────────────────────────────────────────────
// SECTION 1: DomainPrimitiveFakeFactory — Deterministic Fake Data
// ─────────────────────────────────────────────────────────────────────────────
Console.WriteLine("--- 🏭 SECTION 1: DomainPrimitiveFakeFactory ---\n");
Console.WriteLine("Provides deterministic (non-random) valid and invalid test values.");
Console.WriteLine("Unlike Bogus/AutoFixture, values are reproducible across test runs.\n");

// 1a. String fakes
Console.WriteLine("[1a] DomainPrimitiveFakeFactory.Strings");
Console.WriteLine($"  ValidEmails[0]:       '{DomainPrimitiveFakeFactory.Strings.ValidEmails[0]}'");
Console.WriteLine($"  ValidEmail (shortcut): '{DomainPrimitiveFakeFactory.Strings.ValidEmail}'");
Console.WriteLine($"  ValidEmails count:      {DomainPrimitiveFakeFactory.Strings.ValidEmails.Count}");
Console.WriteLine($"  InvalidEmails count:    {DomainPrimitiveFakeFactory.Strings.InvalidEmails.Count}");
Console.WriteLine($"  ValidPhones[0]:        '{DomainPrimitiveFakeFactory.Strings.ValidPhones[0]}'");
Console.WriteLine($"  ValidPhone (shortcut): '{DomainPrimitiveFakeFactory.Strings.ValidPhone}'");
Console.WriteLine($"  ValidUrls[0]:          '{DomainPrimitiveFakeFactory.Strings.ValidUrls[0]}'");
Console.WriteLine($"  ValidUrl (shortcut):   '{DomainPrimitiveFakeFactory.Strings.ValidUrl}'");
Console.WriteLine($"  ValidSlugs[0]:         '{DomainPrimitiveFakeFactory.Strings.ValidSlugs[0]}'");
Console.WriteLine($"  ValidSlug (shortcut):  '{DomainPrimitiveFakeFactory.Strings.ValidSlug}'");
Console.WriteLine($"  ValidCountryCodes:     [{string.Join(", ", DomainPrimitiveFakeFactory.Strings.ValidCountryCodes.Take(3))}...]");

Console.WriteLine();

// 1b. Numeric fakes
Console.WriteLine("[1b] DomainPrimitiveFakeFactory.Numerics");
Console.WriteLine($"  ValidMoneyAmounts:  [{string.Join(", ", DomainPrimitiveFakeFactory.Numerics.ValidMoneyAmounts)}]");
Console.WriteLine($"  ValidMoneyAmount:    {DomainPrimitiveFakeFactory.Numerics.ValidMoneyAmount}");
Console.WriteLine($"  InvalidMoneyAmounts:[{string.Join(", ", DomainPrimitiveFakeFactory.Numerics.InvalidMoneyAmounts)}]");
Console.WriteLine($"  ValidAges:          [{string.Join(", ", DomainPrimitiveFakeFactory.Numerics.ValidAges)}]");
Console.WriteLine($"  ValidAge:            {DomainPrimitiveFakeFactory.Numerics.ValidAge}");
Console.WriteLine($"  InvalidAges:        [{string.Join(", ", DomainPrimitiveFakeFactory.Numerics.InvalidAges)}]");
Console.WriteLine($"  ValidLatitudes:     [{string.Join(", ", DomainPrimitiveFakeFactory.Numerics.ValidLatitudes.Take(3))}...]");
Console.WriteLine($"  ValidPercentages:   [{string.Join(", ", DomainPrimitiveFakeFactory.Numerics.ValidPercentages)}]");
Console.WriteLine($"  ValidScores:        [{string.Join(", ", DomainPrimitiveFakeFactory.Numerics.ValidScores)}]");
Console.WriteLine($"  ValidQuantities:    [{string.Join(", ", DomainPrimitiveFakeFactory.Numerics.ValidQuantities)}]");
Console.WriteLine($"  ValidPrices:        [{string.Join(", ", DomainPrimitiveFakeFactory.Numerics.ValidPrices)}]");
Console.WriteLine($"  ValidTaxRates:      [{string.Join(", ", DomainPrimitiveFakeFactory.Numerics.ValidTaxRates)}]");
Console.WriteLine($"  ValidDiscounts:     [{string.Join(", ", DomainPrimitiveFakeFactory.Numerics.ValidDiscounts)}]");

Console.WriteLine();

// ─────────────────────────────────────────────────────────────────────────────
// SECTION 2: DomainPrimitiveTestBuilder — Creation Helpers
// ─────────────────────────────────────────────────────────────────────────────
Console.WriteLine("--- 🏗️  SECTION 2: DomainPrimitiveTestBuilder ---\n");

// 2a. Create<>() — validated creation (wraps TryCreate)
Console.WriteLine("[2a] DomainPrimitiveTestBuilder.Create<>() — validated creation");
RunTest("Create valid email", () =>
{
    var email = DomainPrimitiveTestBuilder.Create<TestEmail, string>(
        DomainPrimitiveFakeFactory.Strings.ValidEmail);
    Assert(email.Value == DomainPrimitiveFakeFactory.Strings.ValidEmail,
           $"Stored value should match. Got: {email.Value}");
    Console.WriteLine($"    Email created: '{email.Value}' ✅");
});

// 2b. AssertCreationFails<>() — validates that invalid input throws
Console.WriteLine("[2b] DomainPrimitiveTestBuilder.AssertCreationFails<>() — confirms invalid input fails");
RunTest("AssertCreationFails with invalid email", () =>
{
    var ex = DomainPrimitiveTestBuilder.AssertCreationFails<TestEmail, string>(
        DomainPrimitiveFakeFactory.Strings.InvalidEmails[0]); // ""
    Assert(ex != null, "Should have returned a DomainPrimitiveValidationException");
    Console.WriteLine($"    Exception captured: [{ex?.Error.Code}] {ex?.Error.Message} ✅");
});

// 2c. CreateUnvalidated<>() — bypasses validation (for dirty-data testing only)
Console.WriteLine("[2c] DomainPrimitiveTestBuilder.CreateUnvalidated<>() — bypass validation");
Console.WriteLine("     ⚠️  Use ONLY for testing repository/mapper behavior with pre-existing dirty data.");
RunTest("CreateUnvalidated — injects invalid value bypassing domain rules", () =>
{
    var dirtyEmail = DomainPrimitiveTestBuilder.CreateUnvalidated<TestEmail, string>("not-a-valid-email");
    Assert(dirtyEmail.Value == "not-a-valid-email", "Unvalidated value should be stored as-is");
    Console.WriteLine($"    Unvalidated value stored: '{dirtyEmail.Value}' (intentionally invalid) ✅");
    Console.WriteLine("    (Use this to test how your repository handles legacy/dirty DB data.)");
});

Console.WriteLine();

// ─────────────────────────────────────────────────────────────────────────────
// SECTION 3: DomainPrimitiveScenarios — Pre-defined Test Inputs
// ─────────────────────────────────────────────────────────────────────────────
Console.WriteLine("--- 📋 SECTION 3: DomainPrimitiveScenarios (MemberData sets) ---\n");
Console.WriteLine("Use these in xUnit [MemberData] attributes to get comprehensive coverage:\n");

// 3a. Email scenarios
Console.WriteLine("[3a] DomainPrimitiveScenarios.ValidEmailInputs");
foreach (var inputs in DomainPrimitiveScenarios.ValidEmailInputs.Take(3))
{
    var raw = inputs[0] as string ?? "";
    bool ok = TestEmail.TryCreate(raw, out var e, out var err);
    Console.WriteLine($"    '{raw}' → {(ok ? "✅ " + e.Value : "❌ " + err.Message)}");
}

Console.WriteLine();
Console.WriteLine("[3b] DomainPrimitiveScenarios.InvalidEmailInputs");
foreach (var inputs in DomainPrimitiveScenarios.InvalidEmailInputs.Take(3))
{
    var raw = inputs[0] as string ?? "(null)";
    bool ok = TestEmail.TryCreate(raw, out _, out var err);
    Console.WriteLine($"    '{(raw.Length > 20 ? raw[..20] + "..." : raw)}' → {(ok ? "✅ Unexpected success" : "❌ " + err.Code)}");
}

Console.WriteLine();
Console.WriteLine("[3c] DomainPrimitiveScenarios.ValidGuidStrings");
foreach (var inputs in DomainPrimitiveScenarios.ValidGuidStrings.Take(2))
{
    Console.WriteLine($"    '{inputs[0]}'");
}

Console.WriteLine();
Console.WriteLine("[3d] DomainPrimitiveScenarios.ValidAgeValues + InvalidAgeValues");
Console.Write("    Valid: ");
Console.WriteLine(string.Join(", ", DomainPrimitiveScenarios.ValidAgeValues.Select(x => x[0])));
Console.Write("    Invalid: ");
Console.WriteLine(string.Join(", ", DomainPrimitiveScenarios.InvalidAgeValues.Select(x => x[0])));

Console.WriteLine();
Console.WriteLine("[3e] DomainPrimitiveScenarios.EmailNormalizationScenarios");
foreach (var inputs in DomainPrimitiveScenarios.EmailNormalizationScenarios)
{
    var raw = inputs[0] as string ?? "";
    var expected = inputs[1] as string ?? "";
    bool ok = TestEmail.TryCreate(raw, out var e, out _);
    bool normalizedOk = ok && e.Value == expected;
    Console.WriteLine($"    Create('{raw}') → '{(ok ? e.Value : "?")}' {(normalizedOk ? "✅" : "❌ Expected: " + expected)}");
}

Console.WriteLine();

// ─────────────────────────────────────────────────────────────────────────────
// SECTION 4: DomainPrimitiveAssertionsExtensions — xUnit/FluentAssertions APIs
// ─────────────────────────────────────────────────────────────────────────────
Console.WriteLine("--- 🔬 SECTION 4: DomainPrimitiveAssertionsExtensions (xUnit signatures) ---\n");
Console.WriteLine("These extension methods require AwesomeAssertions + a test runner.");
Console.WriteLine("They cannot be invoked in a console app — see OfficialSample.Tests for usage.\n");
Console.WriteLine("Available extension methods on ObjectAssertions:");
Console.WriteLine("  .HavePrimitiveValue<TPrimitive, TValue>(expectedValue)");
Console.WriteLine("     → Asserts that the subject domain primitive holds the expected backing value.\n");
Console.WriteLine("Available extension methods on ActionAssertions:");
Console.WriteLine("  .ThrowDomainPrimitiveException()");
Console.WriteLine("     → Asserts that the action throws DomainPrimitiveValidationException.\n");
Console.WriteLine("  .ThrowDomainPrimitiveExceptionWithPrimitiveErrorCode(errorCode)");
Console.WriteLine("     → Asserts the exception contains the specific error code.\n");
Console.WriteLine("Static helpers on DomainPrimitiveAssertionsExtensions:");
Console.WriteLine("  DomainPrimitiveAssertionsExtensions.ShouldFailCreationWith<TPrimitive, TValue>(value, errorCode)");
Console.WriteLine("     → Asserts that Create() fails with the specified error code.\n");
Console.WriteLine("  DomainPrimitiveAssertionsExtensions.ShouldSucceedCreation<TPrimitive, TValue>(value)");
Console.WriteLine("     → Asserts that TryCreate() succeeds and returns the primitive.\n");
Console.WriteLine("Available extension methods on ObjectAssertions (value validation):");
Console.WriteLine("  .ShouldBeValidPrimitive<TPrimitive, TValue>()");
Console.WriteLine("     → Asserts the subject value can be converted to the domain primitive.\n");
Console.WriteLine("  .ShouldHaveValidationPrimitiveError<TPrimitive, TValue>(errorCode)");
Console.WriteLine("     → Asserts Create() fails with the expected error code.");

Console.WriteLine();

// ─────────────────────────────────────────────────────────────────────────────
// SECTION 5: DomainPrimitiveVerifyExtensions — Verify Snapshot Integration
// ─────────────────────────────────────────────────────────────────────────────
Console.WriteLine("--- 📸 SECTION 5: DomainPrimitiveVerifyExtensions.Initialize() ---\n");
Console.WriteLine("Configures Verify (snapshot testing) to serialize domain primitives as");
Console.WriteLine("their underlying backing value — not as {Value: ...} wrapped objects.\n");
Console.WriteLine("Call once in a [ModuleInitializer] or static constructor:");
Console.WriteLine("  [ModuleInitializer]");
Console.WriteLine("  internal static void InitVerify()");
Console.WriteLine("  {");
Console.WriteLine("      DomainPrimitiveVerifyExtensions.Initialize();");
Console.WriteLine("      // Now Verify will serialize CustomerId as \"3fa85f64-...\"");
Console.WriteLine("      // instead of {\"Value\": \"3fa85f64-...\"}");
Console.WriteLine("  }");

Console.WriteLine();

// ─────────────────────────────────────────────────────────────────────────────
// SUMMARY
// ─────────────────────────────────────────────────────────────────────────────
Console.WriteLine($"=========================================================");
Console.WriteLine($" 🏆 TEST SUMMARY: {passedTests}/{totalTests} PASSED");
Console.WriteLine($"=========================================================");

Console.WriteLine("\nCHAPTER 19 COMPLETED SUCCESSFULLY.\n");


// ─────────────────────────────────────────────────────────────────────────────
// Test runner helpers
// ─────────────────────────────────────────────────────────────────────────────
void RunTest(string name, Action test)
{
    totalTests++;
    Console.WriteLine($"  🧪 {name}");
    try
    {
        test();
        passedTests++;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"    ❌ FAILED: {ex.Message}");
    }
}

static void Assert(bool condition, string message)
{
    if (!condition)
        throw new InvalidOperationException($"Assertion failed: {message}");
}


// ============================================================================
// DOMAIN TYPES USED IN THIS CHAPTER
// ============================================================================

namespace Chapter19
{
    [StrongId<Guid>]
    public readonly partial record struct OrderId;

    [StrongId<Guid>]
    public readonly partial record struct CustomerId;

    [Email]
    public readonly partial record struct TestEmail;

    [Age]
    public readonly partial record struct TestAge;
}
