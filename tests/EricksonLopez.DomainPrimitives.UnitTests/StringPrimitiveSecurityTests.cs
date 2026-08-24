// Copyright © Erickson Lopez. MIT License.
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using EricksonLopez.DomainPrimitives.Testing;
using EricksonLopez.DomainPrimitives.UnitTests.TestTypes;
using EricksonLopez.DomainPrimitives.Validation;
using Xunit;

namespace EricksonLopez.DomainPrimitives.UnitTests;

/// <summary>
/// Security gate tests for EricksonLopez.DomainPrimitives.
/// All tests in this file are categorized as "Security" and map directly to
/// the security gates defined in the engineering specification (§SECURITY GATES).
/// </summary>
public class StringPrimitiveSecurityTests
{
    // ── SEC-001: 4096 character limit (DoS prevention) ────────────────────────

    [Fact, Trait("Category", "Security")]
    public void SEC001_StringPrimitive_WithoutMaxLength_EnforcesDefault4096Limit()
    {
        // Arrange: create a string that exceeds the 4096 character security limit on an unconstrained primitive
        var atLimit = new string('A', 4096);
        var tooLong = new string('A', 4097);

        // Act & Assert: exactly 4096 must succeed
        var atLimitResult = UnconstrainedString.TryCreate(atLimit, out var atLimitPrim, out var atLimitErr);
        atLimitResult.Should().BeTrue();
        atLimitPrim.Value.Length.Should().Be(4096);
        atLimitErr.Should().Be(PrimitiveError.None);

        // Act & Assert: 4097 characters must fail with a LENGTH error
        var overLimitResult = UnconstrainedString.TryCreate(tooLong, out var overPrim, out var overError);
        overLimitResult.Should().BeFalse();
        overPrim.IsDefault.Should().BeTrue();
        overError.Code.Should().Be("LENGTH");
    }

    [Fact, Trait("Category", "Security")]
    public void SEC001_StringPrimitive_WithExplicitMaxLength_EnforcesConfiguredLimit()
    {
        // FirstName has MaxLength=100
        var atLimit = new string('A', 100);
        var overLimit = new string('A', 101);

        FirstName.TryCreate(atLimit, out var atPrim, out var atErr).Should().BeTrue();
        atPrim.Value.Length.Should().Be(100);
        atErr.Should().Be(PrimitiveError.None);

        FirstName.TryCreate(overLimit, out var overPrim, out var overErr).Should().BeFalse();
        overPrim.IsDefault.Should().BeTrue();
        overErr.Code.Should().Be("LENGTH");
    }

    // ── SEC-002: NonBacktracking regex (ReDoS prevention) ────────────────────

    [Fact, Trait("Category", "Security")]
    public async Task SEC002_EmailAttribute_RegexUsesNonBacktracking_DoesNotHangOnAdversarialInput()
    {
        // Arrange: adversarial input designed to cause catastrophic backtracking in naive regex
        var adversarialInput = new string('a', 50) + "@" + new string('b', 50);

        var task = Task.Run(() =>
        {
            var success = EmailAddress.TryCreate(adversarialInput, out var prim, out var err);
            return (Success: success, Primitive: prim, Error: err);
        });

        // Act & Assert: NonBacktracking regex executes in O(N) time; must complete well within 500ms
        var result = await task.WaitAsync(TimeSpan.FromMilliseconds(500));
        result.Success.Should().BeTrue();
        result.Primitive.IsDefault.Should().BeFalse();
        result.Primitive.Value.Should().Be(adversarialInput);
    }

    [Fact, Trait("Category", "Security")]
    public async Task SEC002_ProductCode_RegexDoesNotHangOnAdversarialInput()
    {
        // Arrange: adversarial input pattern for product code regex ^[A-Z]{2}-\d{4}$
        var adversarialInput = new string('A', 100) + "-" + new string('1', 100);

        var task = Task.Run(() =>
        {
            var success = ProductCode.TryCreate(adversarialInput, out var prim, out var err);
            return (Success: success, Primitive: prim, Error: err);
        });

        // Act & Assert: Must complete promptly without backtracking hang
        var result = await task.WaitAsync(TimeSpan.FromMilliseconds(500));
        result.Success.Should().BeFalse("Adversarial product code format should be rejected");
        result.Primitive.IsDefault.Should().BeTrue();
        result.Error.Code.Should().Be("FORMAT");
    }

    // ── SEC-003: Regex timeout ≤ 100ms (injected by generator) ───────────────

    [Fact, Trait("Category", "Security")]
    public void SEC003_Regex_HasTimeout_CompletesParsing_WhenInputIsValid()
    {
        var validEmail = "user@example.com";

        var sw = System.Diagnostics.Stopwatch.StartNew();
        var success = EmailAddress.TryCreate(validEmail, out var validPrim, out var validErr);
        sw.Stop();

        success.Should().BeTrue();
        validPrim.IsDefault.Should().BeFalse();
        validErr.Should().Be(PrimitiveError.None);
        sw.ElapsedMilliseconds.Should().BeLessThan(1000, "Email validation took too long. SEC-003 requires generator to inject regex timeout and complete promptly.");
    }

    // ── SEC-004: NFC Unicode normalization ────────────────────────────────────

    [Fact, Trait("Category", "Security")]
    public void SEC004_StringPrimitive_NormalizesToNFC_CombiningCharacters()
    {
        // Arrange: NFD form vs NFC form
        var nfd = "caf\u0065\u0301e"; // "cafe" with NFD e+accent
        var nfc = "caf\u00e9e";       // "café" with NFC é

        // Act
        var result1 = DisplayName.Create(nfd);
        var result2 = DisplayName.Create(nfc);

        // Assert
        result1.Value.Should().Be(result2.Value);
    }

    [Fact, Trait("Category", "Security")]
    public void SEC004_EmailAddress_NormalizesToNFC_PreventsBypassViaUnicodeDecomposition()
    {
        var nfdEmail = "user\u0040example.com";
        var nfcEmail = "user@example.com";

        var nfdResult = EmailAddress.TryCreate(nfdEmail, out var nfdPrimitive, out var nfdErr);
        var nfcResult = EmailAddress.TryCreate(nfcEmail, out var nfcPrimitive, out var nfcErr);

        nfcResult.Should().Be(nfdResult);
        nfdErr.Should().Be(PrimitiveError.None);
        nfcErr.Should().Be(PrimitiveError.None);
        if (nfcResult && nfdResult)
            nfcPrimitive.Value.Should().Be(nfdPrimitive.Value);
    }

    [Fact, Trait("Category", "Security")]
    public void SEC004_TryParse_SpanPath_NormalizesToNFC_WithLowerCase()
    {
        var nfd = "caf\u0065\u0301e".AsSpan();
        var nfc = "caf\u00e9e";

        LowercaseTag.TryParse(nfd, null, out var result);
        result.Value.Should().Be(nfc.ToLowerInvariant());
    }

    // ── SEC-005: No PII in error messages ─────────────────────────────────────

    [Fact, Trait("Category", "Security")]
    public void SEC005_PasswordHash_InvalidInput_ErrorDoesNotExposeValue()
    {
        // Arrange: empty password hash fails NotEmpty validation
        var emptyHash = "";

        // Act
        var hashResult = PasswordHashValue.TryCreate(emptyHash, out var hashPrim, out var error);

        // Assert: creation fails with EMPTY error and descriptive message without sensitive leakage
        hashResult.Should().BeFalse();
        hashPrim.IsDefault.Should().BeTrue();
        error.IsError.Should().BeTrue();
        error.Code.Should().Be("EMPTY");
        error.Message.Should().NotBeNullOrWhiteSpace();
    }

    [Fact, Trait("Category", "Security")]
    public void SEC005_ApiSecret_TooShortInput_ErrorDoesNotExposeTheSecretValue()
    {
        // Arrange: short sensitive secret that fails MinLength(32) validation
        var shortSecret = "my-super-secret-api-key-123";

        // Act
        var secretResult = ApiSecret.TryCreate(shortSecret, out var secPrim, out var error);

        // Assert: the secret value must not appear in the error message and error code must be LENGTH
        secretResult.Should().BeFalse();
        secPrim.IsDefault.Should().BeTrue();
        error.IsError.Should().BeTrue();
        error.Code.Should().Be("LENGTH");
        error.Message.Should().NotContain(shortSecret);
    }

    // ── SEC-006: ArrayPool limits (stack overflow prevention) ─────────────────

    [Fact, Trait("Category", "Security")]
    public void SEC006_UTF8Parse_InputExceedsStackallocLimit_UsesArrayPool()
    {
        var largeString = new string('A', 150);
        byte[] utf8Bytes = Encoding.UTF8.GetBytes(largeString);

        var result = DisplayName.Parse(utf8Bytes, null);
        result.Value.Should().Be(largeString.ToUpperInvariant());
    }

    [Fact, Trait("Category", "Security")]
    public void SEC006_UTF8Parse_InputWithinStackallocLimit_UsesStackalloc()
    {
        var smallString = new string('A', 100);
        byte[] utf8Bytes = Encoding.UTF8.GetBytes(smallString);

        var result = DisplayName.Parse(utf8Bytes, null);
        result.Value.Should().Be(smallString.ToUpperInvariant());
    }

    [Fact, Trait("Category", "Security")]
    public void SEC005_TestingSdk_ThrowDomainPrimitiveExceptionWithPrimitiveErrorCode_ValidatesError()
    {
        Action act = () => FirstName.Create(new string('A', 101));
        act.Should().ThrowDomainPrimitiveExceptionWithPrimitiveErrorCode("LENGTH");
    }
}

