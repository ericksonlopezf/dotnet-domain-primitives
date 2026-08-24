// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using EricksonLopez.DomainPrimitives.AspNetCore;
using EricksonLopez.DomainPrimitives.UnitTests.TestTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Xunit;

namespace EricksonLopez.DomainPrimitives.AspNetCore.UnitTests;

public class DomainPrimitiveModelBinderTests
{
    private static DefaultModelBindingContext CreateBindingContext(Type modelType, string modelName, string? value)
    {
        var modelState = new ModelStateDictionary();
        var valueProvider = new CompositeValueProvider
        {
            new QueryStringValueProvider(
                BindingSource.Query,
                new QueryCollection(value is not null 
                    ? new Dictionary<string, StringValues> { { modelName, new StringValues(value) } }
                    : new Dictionary<string, StringValues>()),
                CultureInfo.InvariantCulture)
        };

        var metadataProvider = new EmptyModelMetadataProvider();
        var metadata = metadataProvider.GetMetadataForType(modelType);

        return new DefaultModelBindingContext
        {
            ModelMetadata = metadata,
            ModelName = modelName,
            ModelState = modelState,
            ValueProvider = valueProvider
        };
    }

    [Fact]
    public async Task BindModelAsync_WithNullContext_ThrowsArgumentNullException()
    {
        var binder = new DomainPrimitiveModelBinder<EmailAddress>();
        var act = () => binder.BindModelAsync(null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task BindModelAsync_WithMissingValue_ReturnsWithoutResult()
    {
        var binder = new DomainPrimitiveModelBinder<EmailAddress>();
        var context = CreateBindingContext(typeof(EmailAddress), "email", null);

        await binder.BindModelAsync(context);

        context.Result.IsModelSet.Should().BeFalse();
        context.ModelState.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task BindModelAsync_WithValidEmail_BindsSuccessfully()
    {
        var binder = new DomainPrimitiveModelBinder<EmailAddress>();
        var context = CreateBindingContext(typeof(EmailAddress), "email", "test@example.com");

        await binder.BindModelAsync(context);

        context.Result.IsModelSet.Should().BeTrue();
        context.Result.Model.Should().BeOfType<EmailAddress>();
        ((EmailAddress)context.Result.Model!).Value.Should().Be("test@example.com");
        context.ModelState.ErrorCount.Should().Be(0);
    }

    [Fact]
    public async Task BindModelAsync_WithInvalidEmail_AddsModelError()
    {
        var binder = new DomainPrimitiveModelBinder<EmailAddress>();
        var context = CreateBindingContext(typeof(EmailAddress), "email", "not-an-email");

        await binder.BindModelAsync(context);

        context.Result.IsModelSet.Should().BeFalse();
        context.ModelState.ErrorCount.Should().BeGreaterThan(0);
        context.ModelState["email"]!.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public async Task BindModelAsync_WithValidStrongId_BindsSuccessfully()
    {
        var binder = new DomainPrimitiveModelBinder<CustomerId>();
        var guid = Guid.NewGuid();
        var context = CreateBindingContext(typeof(CustomerId), "id", guid.ToString());

        await binder.BindModelAsync(context);

        context.Result.IsModelSet.Should().BeTrue();
        context.Result.Model.Should().BeOfType<CustomerId>();
        ((CustomerId)context.Result.Model!).Value.Should().Be(guid);
        context.ModelState.ErrorCount.Should().Be(0);
    }

    [Fact]
    public async Task BindModelAsync_WithInvalidStrongId_AddsModelError()
    {
        var binder = new DomainPrimitiveModelBinder<CustomerId>();
        var context = CreateBindingContext(typeof(CustomerId), "id", "invalid-guid");

        await binder.BindModelAsync(context);

        context.Result.IsModelSet.Should().BeFalse();
        context.ModelState.ErrorCount.Should().BeGreaterThan(0);
        context.ModelState["id"]!.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public async Task BindModelAsync_WithValidNumericPrimitive_BindsSuccessfully()
    {
        var binder = new DomainPrimitiveModelBinder<Score>();
        var context = CreateBindingContext(typeof(Score), "score", "85");

        await binder.BindModelAsync(context);

        context.Result.IsModelSet.Should().BeTrue();
        context.Result.Model.Should().BeOfType<Score>();
        ((Score)context.Result.Model!).Value.Should().Be(85);
        context.ModelState.ErrorCount.Should().Be(0);
    }

    [Fact]
    public async Task BindModelAsync_WithInvalidNumericPrimitive_AddsModelError()
    {
        var binder = new DomainPrimitiveModelBinder<Score>();
        var context = CreateBindingContext(typeof(Score), "score", "not-a-number");

        await binder.BindModelAsync(context);

        context.Result.IsModelSet.Should().BeFalse();
        context.ModelState.IsValid.Should().BeFalse();
        context.ModelState["score"]!.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void AddDomainPrimitivesModelBinding_OptionsExtension_ReturnsSameInstance()
    {
        var options = new MvcOptions();
        var result = options.AddDomainPrimitivesModelBinding();

        result.Should().BeSameAs(options);
    }

    [Fact]
    public void AddDomainPrimitivesModelBinding_NullOptions_ThrowsArgumentNullException()
    {
        MvcOptions options = null!;
        var act = () => options.AddDomainPrimitivesModelBinding();

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddDomainPrimitivesModelBinding_ServiceCollection_ConfiguresOptions()
    {
        var services = new ServiceCollection();
        var result = services.AddDomainPrimitivesModelBinding();

        result.Should().BeSameAs(services);
        var provider = services.BuildServiceProvider();
        provider.Should().NotBeNull();
    }

    [Fact]
    public void AddDomainPrimitivesModelBinding_NullServices_ThrowsArgumentNullException()
    {
        IServiceCollection services = null!;
        var act = () => services.AddDomainPrimitivesModelBinding();

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public async Task BindModelAsync_WithTypeHavingOnly1ParamParse_BindsSuccessfully()
    {
        var binder = new DomainPrimitiveModelBinder<TypeWith1ParamParse>();
        var context = CreateBindingContext(typeof(TypeWith1ParamParse), "val", "hello");

        await binder.BindModelAsync(context);

        context.Result.IsModelSet.Should().BeTrue();
        context.Result.Model.Should().BeOfType<TypeWith1ParamParse>();
        ((TypeWith1ParamParse)context.Result.Model!).Value.Should().Be("hello");
    }

    [Fact]
    public async Task BindModelAsync_WithTypeHavingOnly1ParamCreate_BindsSuccessfully()
    {
        var binder = new DomainPrimitiveModelBinder<TypeWith1ParamCreate>();
        var context = CreateBindingContext(typeof(TypeWith1ParamCreate), "val", "world");

        await binder.BindModelAsync(context);

        context.Result.IsModelSet.Should().BeTrue();
        context.Result.Model.Should().BeOfType<TypeWith1ParamCreate>();
        ((TypeWith1ParamCreate)context.Result.Model!).Value.Should().Be("world");
    }

    [Fact]
    public async Task BindModelAsync_WithTypeHavingCustomTypeConverter_BindsSuccessfully()
    {
        var binder = new DomainPrimitiveModelBinder<TypeWithCustomConverter>();
        var context = CreateBindingContext(typeof(TypeWithCustomConverter), "val", "custom_converted");

        await binder.BindModelAsync(context);

        context.Result.IsModelSet.Should().BeTrue();
        context.Result.Model.Should().BeOfType<TypeWithCustomConverter>();
        ((TypeWithCustomConverter)context.Result.Model!).Value.Should().Be("converted:custom_converted");
    }

    [Fact]
    public async Task BindModelAsync_WithTypeWithoutParseOrCreateOrConverter_AddsFallbackModelError()
    {
        var binder = new DomainPrimitiveModelBinder<TypeWithoutParseOrCreate>();
        var context = CreateBindingContext(typeof(TypeWithoutParseOrCreate), "val", "raw_val");

        await binder.BindModelAsync(context);

        context.Result.IsModelSet.Should().BeFalse();
        context.ModelState.IsValid.Should().BeFalse();
        context.ModelState["val"]!.Errors.Should().Contain(e => e.ErrorMessage.Contains("is not valid for TypeWithoutParseOrCreate"));
    }

    [Fact]
    public async Task BindModelAsync_WhenCreateThrowsExceptionWithoutInnerException_AddsModelError()
    {
        var binder = new DomainPrimitiveModelBinder<TypeThrowingError>();
        var context = CreateBindingContext(typeof(TypeThrowingError), "val", "fail");

        await binder.BindModelAsync(context);

        context.Result.IsModelSet.Should().BeFalse();
        context.ModelState.IsValid.Should().BeFalse();
        context.ModelState["val"]!.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public async Task BindModelAsync_WithTypeHavingOnly2ParamParse_BindsSuccessfully()
    {
        var binder = new DomainPrimitiveModelBinder<TypeWith2ParamParseOnly>();
        var context = CreateBindingContext(typeof(TypeWith2ParamParseOnly), "val", "test_val");

        await binder.BindModelAsync(context);

        context.Result.IsModelSet.Should().BeTrue();
        context.Result.Model.Should().BeOfType<TypeWith2ParamParseOnly>();
        ((TypeWith2ParamParseOnly)context.Result.Model!).Value.Should().Be("test_val:2param");
        context.ModelState.ContainsKey("val").Should().BeTrue();
        context.ModelState["val"]!.RawValue.Should().Be("test_val");
        context.ModelState.ErrorCount.Should().Be(0);
    }

    [Fact]
    public async Task BindModelAsync_SetsModelValueInModelState()
    {
        var binder = new DomainPrimitiveModelBinder<EmailAddress>();
        var context = CreateBindingContext(typeof(EmailAddress), "email", "test@example.com");

        await binder.BindModelAsync(context);

        context.ModelState.ContainsKey("email").Should().BeTrue();
        context.ModelState["email"]!.RawValue.Should().Be("test@example.com");
        context.ModelState.ErrorCount.Should().Be(0);
    }

    [Fact]
    public async Task BindModelAsync_WhenCreateThrowsTargetInvocationException_AddsInnerExceptionMessage()
    {
        var binder = new DomainPrimitiveModelBinder<TypeThrowingError>();
        var context = CreateBindingContext(typeof(TypeThrowingError), "val", "bad");

        await binder.BindModelAsync(context);

        context.Result.IsModelSet.Should().BeFalse();
        context.ModelState.IsValid.Should().BeFalse();
        context.ModelState["val"]!.Errors.Should().ContainSingle(e => e.ErrorMessage == "Failed creation: bad");
    }

    [Fact]
    public async Task BindModelAsync_WhenValueProviderReturnsNullFirstValue_ReturnsTaskCompleted()
    {
        var binder = new DomainPrimitiveModelBinder<EmailAddress>();
        var modelState = new ModelStateDictionary();
        var valueProvider = new CompositeValueProvider
        {
            new TestNullFirstValueProvider(CultureInfo.InvariantCulture)
        };
        var metadataProvider = new EmptyModelMetadataProvider();
        var metadata = metadataProvider.GetMetadataForType(typeof(EmailAddress));
        var context = new DefaultModelBindingContext
        {
            ModelMetadata = metadata,
            ModelName = "email",
            ModelState = modelState,
            ValueProvider = valueProvider
        };

        await binder.BindModelAsync(context);

        context.Result.IsModelSet.Should().BeFalse();
        context.ModelState.ErrorCount.Should().Be(0);
    }

    [Fact]
    public async Task BindModelAsync_WhenTypeConverterThrowsDirectException_CatchesGenericException()
    {
        var binder = new DomainPrimitiveModelBinder<TypeWithFaultyConverter>();
        var context = CreateBindingContext(typeof(TypeWithFaultyConverter), "val", "bad_val");

        await binder.BindModelAsync(context);

        context.Result.IsModelSet.Should().BeFalse();
        context.ModelState.IsValid.Should().BeFalse();
        context.ModelState["val"]!.Errors.Should().Contain(e => e.ErrorMessage.Contains("Faulty conversion"));
    }

    [Fact]
    public async Task BindModelAsync_WithValidSmartEnum_BindsSuccessfully()
    {
        var binder = new DomainPrimitiveModelBinder<TestOrderStatus>();
        var context = CreateBindingContext(typeof(TestOrderStatus), "status", "Completed");

        await binder.BindModelAsync(context);

        context.Result.IsModelSet.Should().BeTrue();
        context.Result.Model.Should().BeOfType<TestOrderStatus>();
        ((TestOrderStatus)context.Result.Model!).Should().Be(TestOrderStatus.Completed);
        context.ModelState.ErrorCount.Should().Be(0);
    }

    [Fact]
    public async Task BindModelAsync_WithInvalidSmartEnum_AddsModelError()
    {
        var binder = new DomainPrimitiveModelBinder<TestOrderStatus>();
        var context = CreateBindingContext(typeof(TestOrderStatus), "status", "InvalidState");

        await binder.BindModelAsync(context);

        context.Result.IsModelSet.Should().BeFalse();
        context.ModelState.IsValid.Should().BeFalse();
        context.ModelState["status"]!.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public async Task BindModelAsync_WithEmptyString_ForRequiredStringPrimitive_AddsModelError()
    {
        var binder = new DomainPrimitiveModelBinder<EmailAddress>();
        var context = CreateBindingContext(typeof(EmailAddress), "email", "   ");

        await binder.BindModelAsync(context);

        context.Result.IsModelSet.Should().BeFalse();
        context.ModelState.IsValid.Should().BeFalse();
        context.ModelState["email"]!.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void AddDomainPrimitivesModelBinding_ServiceCollection_ConfiguresMvcOptions()
    {
        var services = new ServiceCollection();
        services.AddDomainPrimitivesModelBinding();
        var provider = services.BuildServiceProvider();
        var mvcOptions = provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<MvcOptions>>().Value;
        mvcOptions.Should().NotBeNull();
    }

    [Fact]
    public void AddDomainPrimitivesModelBinding_NullServiceCollection_ThrowsArgumentNullException()
    {
        IServiceCollection services = null!;
        var act = () => services.AddDomainPrimitivesModelBinding();
        act.Should().Throw<ArgumentNullException>().WithParameterName("services");
    }

    private sealed class TestNullFirstValueProvider : IValueProvider
    {
        private readonly CultureInfo _culture;
        public TestNullFirstValueProvider(CultureInfo culture) => _culture = culture;
        public bool ContainsPrefix(string prefix) => true;
        public ValueProviderResult GetValue(string key) => new ValueProviderResult(new[] { (string?)null! }, _culture);
    }
}

public readonly struct TypeWith2ParamParseOnly
{
    public string Value { get; }
    public IFormatProvider? Provider { get; }
    public TypeWith2ParamParseOnly(string value, IFormatProvider? provider) { Value = value; Provider = provider; }
    public static TypeWith2ParamParseOnly Parse(string s, IFormatProvider? provider) => new(s + ":2param", provider);
}

public readonly struct TypeWith1ParamParse
{
    public string Value { get; }
    private TypeWith1ParamParse(string value) => Value = value;
    public static TypeWith1ParamParse Parse(string value) => new(value);
}

public readonly struct TypeWith1ParamCreate
{
    public string Value { get; }
    private TypeWith1ParamCreate(string value) => Value = value;
    public static TypeWith1ParamCreate Create(string value) => new(value);
}

[System.ComponentModel.TypeConverter(typeof(CustomConverter))]
public readonly struct TypeWithCustomConverter
{
    public string Value { get; }
    public TypeWithCustomConverter(string value) => Value = value;

    private sealed class CustomConverter : System.ComponentModel.TypeConverter
    {
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, Type sourceType)
            => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

        public override object? ConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string s) return new TypeWithCustomConverter("converted:" + s);
            return base.ConvertFrom(context, culture, value);
        }
    }
}

[System.ComponentModel.TypeConverter(typeof(FaultyConverter))]
public readonly struct TypeWithFaultyConverter
{
    public string Value { get; }
    public TypeWithFaultyConverter(string value) => Value = value;

    private sealed class FaultyConverter : System.ComponentModel.TypeConverter
    {
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, Type sourceType) => true;
        public override object? ConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, CultureInfo? culture, object value)
            => throw new FormatException("Faulty conversion failure");
    }
}

public readonly struct TypeWithoutParseOrCreate
{
    public int X { get; }
}

public readonly struct TypeThrowingError
{
    public static TypeThrowingError Create(string s) => throw new InvalidOperationException("Failed creation: " + s);
}







