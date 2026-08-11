using System;
using System.Linq;
using FluentAssertions;
using Xunit;



namespace EricksonLopez.DomainPrimitives.Tests
{
    public class ValidationAndNormalizationTests
    {
        [Fact]
        public void ValidationAttributes_CanBeInstantiated()
        {
            var length = new LengthAttribute(1, 10);
            length.Min.Should().Be(1);
            length.Max.Should().Be(10);

            var minLength = new MinLengthAttribute(1);
            minLength.Length.Should().Be(1);

            var maxLength = new MaxLengthAttribute(10);
            maxLength.Length.Should().Be(10);

            var range = new PrimitiveRangeAttribute(1, 100);
            range.Min.Should().Be(1);
            range.Max.Should().Be(100);
            range.MinExclusive.Should().BeFalse();
            range.MaxExclusive.Should().BeFalse();
            
            var rangeExclusive = new PrimitiveRangeAttribute(1, 100) { MinExclusive = true, MaxExclusive = true };
            rangeExclusive.MinExclusive.Should().BeTrue();
            rangeExclusive.MaxExclusive.Should().BeTrue();

            var regex = new RegexAttribute("^[a-z]+$") { ErrorMessage = "Only lowercase letters allowed" };
            regex.Pattern.Should().Be("^[a-z]+$");
            regex.ErrorMessage.Should().Be("Only lowercase letters allowed");
            
            var notEmpty = new NotEmptyAttribute();
            notEmpty.Should().NotBeNull();
        }

        [Fact]
        public void NormalizationAttributes_CanBeInstantiated()
        {
            var lower = new LowerCaseAttribute();
            lower.Should().NotBeNull();

            var upper = new UpperCaseAttribute();
            upper.Should().NotBeNull();

            var trim = new TrimAttribute();
            trim.Should().NotBeNull();
            
            var normSpace = new NormalizeWhitespaceAttribute();
            normSpace.Should().NotBeNull();
        }
    }
}
