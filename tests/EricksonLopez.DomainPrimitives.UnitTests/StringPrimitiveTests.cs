// Copyright © Erickson Lopez. MIT License.
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using AwesomeAssertions;
using EricksonLopez.DomainPrimitives.Testing;
using EricksonLopez.DomainPrimitives.UnitTests.TestTypes;
using Xunit;

namespace EricksonLopez.DomainPrimitives.UnitTests;

public class StringPrimitiveTests
{
    [Fact]
    public void FirstName_Create_ValidValue_CreatesInstance()
    {
        // Trim + MinLength(1) + MaxLength(100)
        var name = FirstName.Create("  John  ");
        name.Value.Should().Be("John");
    }

    [Fact]
    public void FirstName_Create_TooShort_ThrowsDomainPrimitiveValidationException()
    {
        Action act = () => FirstName.Create("   "); // Trims to empty
        act.Should().Throw<DomainPrimitiveValidationException>()
            .WithMessage("*must be at least 1*")
            .Where(e => e.Error.Code == "LENGTH");
    }

    [Fact]
    public void EmailAddress_Create_ValidValue_NormalizesAndValidates()
    {
        // Email: Trim + LowerCase + NotEmpty + Regex
        var email = EmailAddress.Create("  USER@Example.COM  ");
        email.Value.Should().Be("user@example.com");
    }

    [Fact]
    public void EmailAddress_Create_InvalidFormat_ThrowsDomainPrimitiveValidationException()
    {
        Action act = () => EmailAddress.Create("invalid-email");
        act.Should().Throw<DomainPrimitiveValidationException>()
            .WithMessage("*Invalid email format*")
            .Where(e => e.Error.Code == "FORMAT");
    }

    [Fact]
    public void EmailAddress_TryCreate_InvalidFormat_ReturnsFailureResult()
    {
        var success = EmailAddress.TryCreate("invalid-email", out var result, out var error);
        success.Should().BeFalse();
        result.IsDefault.Should().BeTrue();
        error.Code.Should().Be("FORMAT");
    }

    [Fact]
    public void SecureFtpUrl_AllowedSchemes_Validation_Works()
    {
        var validHttps = SecureFtpUrl.Create("https://myfiles.com/doc.pdf");
        var validFtp = SecureFtpUrl.Create("ftp://myfiles.com/doc.pdf");

        validHttps.Value.Should().Be("https://myfiles.com/doc.pdf");
        validFtp.Value.Should().Be("ftp://myfiles.com/doc.pdf");

        Action act = () => SecureFtpUrl.Create("http://myfiles.com/doc.pdf");
        act.Should().Throw<DomainPrimitiveValidationException>()
            .WithMessage("*SecureFtpUrl must be a valid absolute HTTPS/FTP URL.*");
    }

    [Fact]
    public void Country_Create_ValidValue_NormalizesAndValidates()
    {
        // CountryCode: Trim + UpperCase + Length(2,2)
        var country = Country.Create(" us ");
        country.Value.Should().Be("US");
    }

    [Fact]
    public void Country_Create_InvalidLength_ThrowsDomainPrimitiveValidationException()
    {
        Action act1 = () => Country.Create("USA");
        act1.Should().Throw<DomainPrimitiveValidationException>()
            .Where(e => e.Error.Code == "LENGTH");

        Action act2 = () => Country.Create("U");
        act2.Should().Throw<DomainPrimitiveValidationException>()
            .Where(e => e.Error.Code == "LENGTH");
    }

    [Fact]
    public void ProductCode_Create_ValidValue_MatchesRegex()
    {
        // Regex: ^[A-Z]{2}-\d{4}$
        var product = ProductCode.Create("AB-1234");
        product.Value.Should().Be("AB-1234");
    }

    [Fact]
    public void ProductCode_Create_InvalidValue_ReturnsCustomRegexErrorMessage()
    {
        Action act = () => ProductCode.Create("AB1234");
        act.Should().Throw<DomainPrimitiveValidationException>()
            .WithMessage("*Must be in format XX-0000*")
            .Where(e => e.Error.Code == "FORMAT");
    }

    [Fact]
    public void DisplayName_Create_MultipleNormalizations_WorksCorrectly()
    {
        // Trim + UpperCase + NormalizeWhitespace
        var displayName = DisplayName.Create("  john   doe \t smith  ");
        displayName.Value.Should().Be("JOHN DOE SMITH");
    }

    [Fact]
    public void Equals_SameValues_ReturnsTrue()
    {
        var email1 = EmailAddress.Create("test@test.com");
        var email2 = EmailAddress.Create("TEST@TEST.COM"); // normalized to test@test.com

        email1.Should().Be(email2);
        (email1 == email2).Should().BeTrue();
    }

    [Fact]
    public void CompareTo_WorksCorrectly()
    {
        var countryA = Country.Create("AT");
        var countryB = Country.Create("BE");

        (countryA < countryB).Should().BeTrue();
        countryA.CompareTo(countryB).Should().BeLessThan(0);
    }

    [Fact]
    public void ExplicitCast_String_Works()
    {
        var str = "test@test.com";
        var email = (EmailAddress)str;
        email.Value.Should().Be(str);

        var castBack = (string)email;
        castBack.Should().Be(str);
    }

    [Fact]
    public void Parse_WorksCorrectly()
    {
        var email = EmailAddress.Parse("user@example.com", CultureInfo.InvariantCulture);
        email.Value.Should().Be("user@example.com");
    }

    [Fact]
    public void TypeConverter_ConvertsFromString()
    {
        var converter = TypeDescriptor.GetConverter(typeof(EmailAddress));
        converter.CanConvertFrom(typeof(string)).Should().BeTrue();
        
        var email = (EmailAddress)converter.ConvertFrom("user@example.com")!;
        email.Value.Should().Be("user@example.com");
    }

    [Fact]
    public void TypeConverter_ConvertsToString()
    {
        var converter = TypeDescriptor.GetConverter(typeof(EmailAddress));
        var email = EmailAddress.Create("user@example.com");
        
        converter.CanConvertTo(typeof(string)).Should().BeTrue();
        
        var str = (string)converter.ConvertTo(email, typeof(string))!;
        str.Should().Be("user@example.com");
    }

    [Fact]
    public void FirstName_TestingSdk_HavePrimitiveValue_ValidatesValue()
    {
        var name = FirstName.Create("John");
        ((object)name).Should().HavePrimitiveValue<FirstName, string>("John");
    }

    [Fact]
    public void EmailAddress_TestingSdk_ShouldBeValidPrimitive_ValidatesValue()
    {
        ((object)"user@example.com").Should().ShouldBeValidPrimitive<EmailAddress, string>();
    }

    [Fact]
    public void FirstName_TestingSdk_ShouldFailCreationWith_ValidatesErrorCode()
    {
        DomainPrimitiveAssertionsExtensions.ShouldFailCreationWith<FirstName, string>("   ", "LENGTH");
    }
}





