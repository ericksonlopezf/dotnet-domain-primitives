using System;
using FluentAssertions;
using Xunit;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.Tests
{
    public class DomainShortcutAttributesTests
    {
        [Fact]
        public void ShortcutAttributes_CanBeInstantiated()
        {
            var email = new EmailAttribute { MaxLength = 100 };
            email.MaxLength.Should().Be(100);

            var phone = new PhoneAttribute();
            phone.Should().NotBeNull();

            var url = new UrlAttribute { AllowedSchemes = ["ftp"] };
            url.AllowedSchemes.Should().Contain("ftp");

            var slug = new SlugAttribute { MaxLength = 50 };
            slug.MaxLength.Should().Be(50);

            var country = new CountryCodeAttribute();
            country.Should().NotBeNull();

            var lang = new LanguageCodeAttribute();
            lang.Should().NotBeNull();

            var currency = new CurrencyCodeAttribute();
            currency.Should().NotBeNull();

            var user = new UsernameAttribute { MinLength = 5, MaxLength = 10 };
            user.MinLength.Should().Be(5);
            user.MaxLength.Should().Be(10);

            var password = new PasswordHashAttribute();
            password.Should().NotBeNull();

            var hex = new HexColorAttribute();
            hex.Should().NotBeNull();

            var money = new MoneyAttribute { Currency = "EUR", Min = 10, Max = 1000 };
            money.Currency.Should().Be("EUR");
            money.Min.Should().Be(10);
            money.Max.Should().Be(1000);

            var pct = new PercentageAttribute { Min = 1, Max = 99 };
            pct.Min.Should().Be(1);
            pct.Max.Should().Be(99);

            var birth = new BirthDateAttribute { MaxAge = 120 };
            birth.MaxAge.Should().Be(120);

            var exp = new ExpirationDateAttribute();
            exp.Should().NotBeNull();

            var ip = new IPAddressAttribute(); ip.Should().NotBeNull();
            var mac = new MacAddressAttribute(); mac.Should().NotBeNull();
            var iban = new IBANAttribute(); iban.Should().NotBeNull();
            var isbn = new ISBNAttribute(); isbn.Should().NotBeNull();
            var vin = new VINAttribute(); vin.Should().NotBeNull();

            var lat = new LatitudeAttribute(); lat.Should().NotBeNull();
            var lon = new LongitudeAttribute(); lon.Should().NotBeNull();
            var age = new AgeAttribute(); age.Should().NotBeNull();

            var weight = new WeightAttribute { Min = 1, Max = 100 };
            weight.Min.Should().Be(1); weight.Max.Should().Be(100);

            var height = new HeightAttribute { Min = 1, Max = 200 };
            height.Min.Should().Be(1); height.Max.Should().Be(200);

            var dist = new DistanceAttribute { Min = 0, Max = 1000 };
            dist.Min.Should().Be(0); dist.Max.Should().Be(1000);

            var temp = new TemperatureAttribute { Min = -10, Max = 50 };
            temp.Min.Should().Be(-10); temp.Max.Should().Be(50);

            var score = new ScoreAttribute { Min = 0, Max = 10 };
            score.Min.Should().Be(0); score.Max.Should().Be(10);

            var qty = new QuantityAttribute { Min = 1, Max = 50 };
            qty.Min.Should().Be(1); qty.Max.Should().Be(50);

            var price = new PriceAttribute { Min = 0, Max = 999 };
            price.Min.Should().Be(0); price.Max.Should().Be(999);

            var tax = new TaxRateAttribute { Min = 0, Max = 25 };
            tax.Min.Should().Be(0); tax.Max.Should().Be(25);

            var disc = new DiscountAttribute { Min = 0, Max = 100 };
            disc.Min.Should().Be(0); disc.Max.Should().Be(100);

            var rating = new RatingAttribute { Min = 0, Max = 10, Scale = 2 };
            rating.Min.Should().Be(0); rating.Max.Should().Be(10); rating.Scale.Should().Be(2);

            var bizDate = new BusinessDateAttribute(); bizDate.Should().NotBeNull();
            var fy = new FiscalYearAttribute(); fy.Should().NotBeNull();
            var month = new MonthAttribute(); month.Should().NotBeNull();
            var quarter = new QuarterAttribute(); quarter.Should().NotBeNull();
            var week = new WeekAttribute(); week.Should().NotBeNull();
            var dRange = new DateRangeAttribute(); dRange.Should().NotBeNull();
            var tRange = new TimeRangeAttribute(); tRange.Should().NotBeNull();
        }
    }
}
