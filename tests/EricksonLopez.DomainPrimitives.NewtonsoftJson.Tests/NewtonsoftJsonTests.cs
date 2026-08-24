// Copyright © Erickson Lopez. MIT License.
using System;
using System.Globalization;
using System.IO;
using AwesomeAssertions;
using EricksonLopez.DomainPrimitives.NewtonsoftJson;
using EricksonLopez.DomainPrimitives.UnitTests.TestTypes;
using EricksonLopez.DomainPrimitives.Validation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace EricksonLopez.DomainPrimitives.NewtonsoftJson.Tests;

#region Test Dummy Types for Edge Cases

[StringPrimitive]
public readonly record struct StringDummyVo { public string Value { get; init; } }

[NumericPrimitive<int>]
public readonly record struct NumericDummyVo { public int Value { get; init; } }

[DatePrimitive]
public readonly record struct DateDummyVo { public DateOnly Value { get; init; } }

[StrongId<Guid>]
public readonly record struct StrongIdDummyVo { public Guid Value { get; init; } }

[SmartEnum<int>]
public readonly record struct SmartEnumDummyVo { public int Value { get; init; } }

[Email]
public readonly record struct EmailDummyVo { public string Value { get; init; } }

[Phone]
public readonly record struct PhoneDummyVo { public string Value { get; init; } }

[Url]
public readonly record struct UrlDummyVo { public string Value { get; init; } }

[Money]
public readonly record struct MoneyDummyVo { public decimal Value { get; init; } }

[Percentage]
public readonly record struct PercentageDummyVo { public decimal Value { get; init; } }

[BirthDate]
public readonly record struct BirthDateDummyVo { public DateOnly Value { get; init; } }

[ExpirationDate]
public readonly record struct ExpirationDateDummyVo { public DateOnly Value { get; init; } }

[Email]
public readonly record struct CreateOnlyPrimitive
{
    public string Value { get; }
    public CreateOnlyPrimitive(string value) => Value = value;
    public static CreateOnlyPrimitive Create(string value)
    {
        if (value == "throw") throw new InvalidOperationException("Create failed");
        return new CreateOnlyPrimitive(value);
    }
}

[Email]
public readonly record struct TryCreateOnlyPrimitive
{
    public string Value { get; }
    public bool IsDefault => string.IsNullOrEmpty(Value);
    public TryCreateOnlyPrimitive(string value) => Value = value;
    public static bool TryCreate(string value, out TryCreateOnlyPrimitive result, out PrimitiveError validationError)
    {
        if (value == "invalid")
        {
            result = default;
            validationError = new PrimitiveError("TryCreateOnlyPrimitive", "Invalid value");
            return false;
        }
        result = new TryCreateOnlyPrimitive(value);
        validationError = default;
        return true;
    }
}

[Email]
public readonly record struct NoFactoryPrimitive
{
    public string Value { get; init; }
    public NoFactoryPrimitive() => Value = "constant";
}

[Email]
public readonly record struct CustomDefaultPrimitive : IDomainPrimitive<CustomDefaultPrimitive, string>
{
    public static string PrimitiveName => "CustomDefaultPrimitive";
    public string Value { get; }
    public bool IsDefault { get; }
    public CustomDefaultPrimitive(string value, bool isDefault) { Value = value; IsDefault = isDefault; }
    public static CustomDefaultPrimitive Create(string value) => new(value, false);
    public static bool TryCreate(string value, out CustomDefaultPrimitive result, out PrimitiveError validationError)
    {
        result = new CustomDefaultPrimitive(value, false);
        validationError = default;
        return true;
    }
}

[ValueObject]
public readonly record struct SingleParamParseVo
{
    public string Name { get; init; }
    public static SingleParamParseVo Parse(string json)
    {
        if (json.Contains("throw")) throw new InvalidOperationException("Single param parse failed");
        return new SingleParamParseVo { Name = "Parsed" };
    }
}

[ValueObject]
public readonly record struct TwoParamParseVo
{
    public string Name { get; init; }
    public static TwoParamParseVo Parse(string s, IFormatProvider? provider) => new TwoParamParseVo { Name = "TwoParam" };
}

[ValueObject]
public record struct PropertyMatchingVo
{
    public string Title { get; set; }
    public string? OptionalNote { get; set; }
}

[ValueObject]
public record struct WritableIsDefaultVo
{
    public string Title { get; set; }
    public bool IsDefault { get; set; }
}

[ValueObject]
public partial record struct ReadOnlyPropVo
{
    public string Title { get; set; }
    public int Calculated => Title?.Length ?? 0;
    public bool IsDefault => string.IsNullOrEmpty(Title);
}

[Email]
public readonly record struct NullValuePrimitive
{
    private readonly string? _val;
    public NullValuePrimitive() => _val = null;
    public string? Value => _val;
}

[Email]
public readonly record struct NoValuePropertyPrimitive
{
    public static bool TryCreate(string a, string b, string c) => true;
}

[StringPrimitive]
public readonly record struct InterfaceOnlyPrimitive : IDomainPrimitive<InterfaceOnlyPrimitive, string>
{
    public static string PrimitiveName => "InterfaceOnlyPrimitive";
    public string Value { get; }
    public bool IsDefault => string.IsNullOrEmpty(Value);
    public InterfaceOnlyPrimitive(string value) => Value = value;
    public static InterfaceOnlyPrimitive Create(string value) => new(value);
    public static bool TryCreate(string value, out InterfaceOnlyPrimitive result, out PrimitiveError validationError)
    {
        result = new InterfaceOnlyPrimitive(value);
        validationError = default;
        return true;
    }
}

[Email]
public readonly record struct Other3ParamMethodVo
{
    public string Value { get; }
    public Other3ParamMethodVo(string value) => Value = value;
    public static void Unrelated(int a, int b, int c) { }
    public static Other3ParamMethodVo Create(string value) => new(value);
}

[ValueObject]
public readonly record struct Other1ParamMethodVo
{
    public string Name { get; init; }
    public static void Helper(int x) { }
}

[ValueObject]
public readonly record struct VoWithOtherStringParamMethod
{
    public string Description { get; init; }
    public static void OtherSingleParamMethod(string s) { }
}

public struct NonPrimitiveStruct
{
    public int X { get; set; }
}

public class CustomStringReader : JsonReader
{
    private readonly string _val;
    private bool _read;
    public CustomStringReader(string val) => _val = val;
    public override bool Read()
    {
        if (_read) return false;
        _read = true;
        return true;
    }
    public override JsonToken TokenType => JsonToken.String;
    public override object? Value => _val;
    public override Type? ValueType => typeof(string);
}

public class TestDto
{
    public EmailAddress Email { get; set; }
    public Price Price { get; set; }
    public string NormalText { get; set; } = string.Empty;
}

public class NullStringReader : JsonReader
{
    public override bool Read() => true;
    public override JsonToken TokenType => JsonToken.String;
    public override object? Value => null;
}

public class DateTimeOffsetReader : JsonReader
{
    private readonly DateTimeOffset _dto;
    private bool _read;
    public DateTimeOffsetReader(DateTimeOffset dto) => _dto = dto;
    public override bool Read()
    {
        if (_read) return false;
        _read = true;
        return true;
    }
    public override JsonToken TokenType => JsonToken.Date;
    public override object? Value => _dto;
}

public class DateTimeReader : JsonReader
{
    private readonly DateTime _dt;
    private bool _read;
    public DateTimeReader(DateTime dt) => _dt = dt;
    public override bool Read()
    {
        if (_read) return false;
        _read = true;
        return true;
    }
    public override JsonToken TokenType => JsonToken.Date;
    public override object? Value => _dt;
}

public class NonDateTimeDateTokenReader : JsonReader
{
    private readonly object? _val;
    private bool _read;
    public NonDateTimeDateTokenReader(object? val) => _val = val;
    public override bool Read()
    {
        if (_read) return false;
        _read = true;
        return true;
    }
    public override JsonToken TokenType => JsonToken.Date;
    public override object? Value => _val;
}

#endregion

public class NewtonsoftJsonTests
{
    private readonly JsonSerializerSettings _settings = new JsonSerializerSettings().AddDomainPrimitives();

    #region String Primitives

    [Fact]
    public void StringPrimitive_SerializesToRawJsonString()
    {
        var email = EmailAddress.Create("test@example.com");
        var json = JsonConvert.SerializeObject(email, _settings);
        json.Should().Be("\"test@example.com\"");
    }

    [Fact]
    public void StringPrimitive_DeserializesFromString()
    {
        var json = "\"user@domain.com\"";
        var deserialized = JsonConvert.DeserializeObject<EmailAddress>(json, _settings);
        deserialized.Value.Should().Be("user@domain.com");
    }

    [Fact]
    public void StringPrimitive_DefaultValue_SerializesAsNull()
    {
        var defaultEmail = default(EmailAddress);
        var json = JsonConvert.SerializeObject(defaultEmail, _settings);
        json.Should().Be("null");
    }

    [Fact]
    public void StringPrimitive_DeserializingNull_ReturnsDefaultInstance()
    {
        var json = "null";
        var deserialized = JsonConvert.DeserializeObject<EmailAddress>(json, _settings);
        deserialized.IsDefault.Should().BeTrue();
    }

    [Fact]
    public void StringPrimitive_InvalidValue_ThrowsJsonSerializationException()
    {
        var json = "\"invalid-email\"";
        var act = () => JsonConvert.DeserializeObject<EmailAddress>(json, _settings);
        act.Should().Throw<JsonSerializationException>()
            .WithMessage("Invalid EmailAddress:*");
    }

    #endregion

    #region Numeric Primitives

    [Fact]
    public void NumericPrimitive_SerializesToRawJsonNumber()
    {
        var price = Price.Create(49.99m);
        var json = JsonConvert.SerializeObject(price, _settings);
        json.Should().Be("49.99");
    }

    [Fact]
    public void NumericPrimitive_DeserializesFromNumber()
    {
        var json = "49.99";
        var deserialized = JsonConvert.DeserializeObject<Price>(json, _settings);
        deserialized.Value.Should().Be(49.99m);
    }

    [Fact]
    public void NumericPrimitive_DefaultValue_SerializesAsNull()
    {
        var defaultPrice = default(Price);
        var json = JsonConvert.SerializeObject(defaultPrice, _settings);
        json.Should().Be("null");
    }

    [Fact]
    public void NumericPrimitive_DeserializingNull_ReturnsDefaultInstance()
    {
        var json = "null";
        var deserialized = JsonConvert.DeserializeObject<Price>(json, _settings);
        deserialized.IsDefault.Should().BeTrue();
    }

    [Fact]
    public void NumericPrimitive_InvalidValue_ThrowsJsonSerializationException()
    {
        var json = "-10";
        var act = () => JsonConvert.DeserializeObject<Price>(json, _settings);
        act.Should().Throw<JsonSerializationException>()
            .WithMessage("Invalid Price:*");
    }

    #endregion

    #region Temporal Primitives

    [Fact]
    public void DatePrimitive_SerializesToIso8601DateString()
    {
        var birthDate = CustomerBirthDate.Create(new DateOnly(1990, 5, 15));
        var json = JsonConvert.SerializeObject(birthDate, _settings);
        json.Should().Be("\"1990-05-15\"");
    }

    [Fact]
    public void DatePrimitive_DeserializesFromIsoDateString()
    {
        var json = "\"1990-05-15\"";
        var deserialized = JsonConvert.DeserializeObject<CustomerBirthDate>(json, _settings);
        deserialized.Value.Should().Be(new DateOnly(1990, 5, 15));
    }

    [Fact]
    public void DatePrimitive_DeserializesFromMicrosoftDateFormat()
    {
        var settings = new JsonSerializerSettings
        {
            DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
        }.AddDomainPrimitives();

        var json = "\"/Date(642729600000)/\"";
        var deserialized = JsonConvert.DeserializeObject<CustomerBirthDate>(json, settings);
        deserialized.IsDefault.Should().BeFalse();
    }

    [Fact]
    public void DatePrimitive_DefaultValue_SerializesAsNull()
    {
        var defaultDate = default(CustomerBirthDate);
        var json = JsonConvert.SerializeObject(defaultDate, _settings);
        json.Should().Be("null");
    }

    [Fact]
    public void DatePrimitive_DeserializingNull_ReturnsDefaultInstance()
    {
        var json = "null";
        var deserialized = JsonConvert.DeserializeObject<CustomerBirthDate>(json, _settings);
        deserialized.IsDefault.Should().BeTrue();
    }

    [Fact]
    public void TimePrimitive_SerializesToIsoTimeString()
    {
        var shiftTime = WorkShiftTime.Create(new TimeOnly(8, 30, 0));
        var json = JsonConvert.SerializeObject(shiftTime, _settings);
        json.Should().Be("\"08:30:00\"");
    }

    [Fact]
    public void TimePrimitive_DeserializesFromIsoTimeString()
    {
        var json = "\"08:30:00\"";
        var deserialized = JsonConvert.DeserializeObject<WorkShiftTime>(json, _settings);
        deserialized.Value.Should().Be(new TimeOnly(8, 30, 0));
    }

    [Fact]
    public void TimePrimitive_DefaultValue_SerializesAsNull()
    {
        var defaultTime = default(WorkShiftTime);
        var json = JsonConvert.SerializeObject(defaultTime, _settings);
        json.Should().Be("null");
    }

    [Fact]
    public void TimePrimitive_DeserializingNull_ReturnsDefaultInstance()
    {
        var json = "null";
        var deserialized = JsonConvert.DeserializeObject<WorkShiftTime>(json, _settings);
        deserialized.IsDefault.Should().BeTrue();
    }

    #endregion

    #region Strong IDs

    [Fact]
    public void GuidStrongId_SerializesToRawGuidString()
    {
        var guid = Guid.NewGuid();
        var id = CustomerId.Create(guid);
        var json = JsonConvert.SerializeObject(id, _settings);
        json.Should().Be($"\"{guid}\"");
    }

    [Fact]
    public void GuidStrongId_DeserializesFromGuidString()
    {
        var guid = Guid.NewGuid();
        var json = $"\"{guid}\"";
        var deserialized = JsonConvert.DeserializeObject<CustomerId>(json, _settings);
        deserialized.Value.Should().Be(guid);
    }

    [Fact]
    public void GuidStrongId_DefaultValue_SerializesAsNull()
    {
        var defaultId = default(CustomerId);
        var json = JsonConvert.SerializeObject(defaultId, _settings);
        json.Should().Be("null");
    }

    [Fact]
    public void GuidStrongId_DeserializingNull_ReturnsDefaultInstance()
    {
        var json = "null";
        var deserialized = JsonConvert.DeserializeObject<CustomerId>(json, _settings);
        deserialized.IsDefault.Should().BeTrue();
    }

    [Fact]
    public void IntStrongId_SerializesToRawJsonNumber()
    {
        var id = OrderNumber.Create(1001);
        var json = JsonConvert.SerializeObject(id, _settings);
        json.Should().Be("1001");
    }

    [Fact]
    public void IntStrongId_DeserializesFromNumber()
    {
        var json = "1001";
        var deserialized = JsonConvert.DeserializeObject<OrderNumber>(json, _settings);
        deserialized.Value.Should().Be(1001);
    }

    [Fact]
    public void StringStrongId_SerializesToRawJsonString()
    {
        var id = Sku.Create("PROD-12345");
        var json = JsonConvert.SerializeObject(id, _settings);
        json.Should().Be("\"PROD-12345\"");
    }

    [Fact]
    public void StringStrongId_DeserializesFromString()
    {
        var json = "\"PROD-12345\"";
        var deserialized = JsonConvert.DeserializeObject<Sku>(json, _settings);
        deserialized.Value.Should().Be("PROD-12345");
    }

    #endregion

    #region Smart Enums

    [Fact]
    public void SmartEnum_SerializesToRawJsonNumber()
    {
        var status = TestOrderStatus.Pending;
        var json = JsonConvert.SerializeObject(status, _settings);
        json.Should().Be("1");
    }

    [Fact]
    public void SmartEnum_DeserializesFromNumber()
    {
        var json = "1";
        var deserialized = JsonConvert.DeserializeObject<TestOrderStatus>(json, _settings);
        deserialized.Should().Be(TestOrderStatus.Pending);
    }

    [Fact]
    public void SmartEnum_DefaultValue_SerializesAsNull()
    {
        var defaultStatus = default(TestOrderStatus);
        var json = JsonConvert.SerializeObject(defaultStatus, _settings);
        json.Should().Be("null");
    }

    [Fact]
    public void SmartEnum_DeserializingNull_ReturnsDefaultInstance()
    {
        var json = "null";
        var deserialized = JsonConvert.DeserializeObject<TestOrderStatus>(json, _settings);
        deserialized.IsDefault.Should().BeTrue();
    }

    [Fact]
    public void SmartEnum_InvalidValue_ThrowsJsonSerializationException()
    {
        var json = "999";
        var act = () => JsonConvert.DeserializeObject<TestOrderStatus>(json, _settings);
        act.Should().Throw<JsonSerializationException>()
            .WithMessage("Invalid TestOrderStatus:*");
    }

    #endregion

    #region Value Objects

    [Fact]
    public void ValueObject_SingleParamParse_Works()
    {
        var vo = new SingleParamParseVo { Name = "Initial" };
        var json = JsonConvert.SerializeObject(vo, _settings);

        var deserialized = JsonConvert.DeserializeObject<SingleParamParseVo>(json, _settings);
        deserialized.Name.Should().Be("Parsed");
    }

    [Fact]
    public void ValueObject_SingleParamParse_ThrowsOnException_ChecksMessage()
    {
        var json = "{\"Name\":\"throw\"}";
        var act = () => JsonConvert.DeserializeObject<SingleParamParseVo>(json, _settings);
        act.Should().Throw<JsonSerializationException>()
            .WithMessage($"Invalid {nameof(SingleParamParseVo)}: Single param parse failed");
    }

    [Fact]
    public void ValueObject_TwoParamParse_Works()
    {
        var vo = new TwoParamParseVo { Name = "Initial" };
        var json = JsonConvert.SerializeObject(vo, _settings);

        var deserialized = JsonConvert.DeserializeObject<TwoParamParseVo>(json, _settings);
        deserialized.Name.Should().Be("TwoParam");
    }

    [Fact]
    public void ValueObject_PropertyMatchingFallback_Works()
    {
        var vo = new PropertyMatchingVo { Title = "Test Title", OptionalNote = null };
        var json = JsonConvert.SerializeObject(vo, _settings);
        json.Should().Contain("\"Title\":\"Test Title\"");

        var deserialized = JsonConvert.DeserializeObject<PropertyMatchingVo>(json, _settings);
        deserialized.Title.Should().Be("Test Title");
        deserialized.OptionalNote.Should().BeNull();
    }

    [Fact]
    public void ValueObject_WithNullProperty_SerializesNullToken()
    {
        var vo = new PropertyMatchingVo { Title = "Only Title", OptionalNote = null };
        var json = JsonConvert.SerializeObject(vo, _settings);
        json.Should().Contain("\"OptionalNote\":null");
    }

    [Fact]
    public void ValueObject_PropertyMatching_IgnoresWritableIsDefault()
    {
        var json = "{\"Title\":\"Hello\",\"IsDefault\":true}";
        var deserialized = JsonConvert.DeserializeObject<WritableIsDefaultVo>(json, _settings);
        deserialized.Title.Should().Be("Hello");
        deserialized.IsDefault.Should().BeFalse();
    }

    [Fact]
    public void ReadOnlyPropVo_PropertyMatchingFallback_IgnoresReadOnlyAndIsDefaultProps()
    {
        var json = "{\"Title\":\"Important Document\",\"Calculated\":999,\"IsDefault\":false}";
        var deserialized = JsonConvert.DeserializeObject<ReadOnlyPropVo>(json, _settings);
        deserialized.Title.Should().Be("Important Document");
        deserialized.Calculated.Should().Be(18);
    }

    #endregion

    #region Factory Edge Cases (TryCreateOnly, CreateOnly, NoFactory)

    [Fact]
    public void GenericConverter_TryCreateOnlyPrimitive_SuccessAndValidationFailure()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new DomainPrimitiveNewtonsoftJsonConverter<TryCreateOnlyPrimitive, string>());

        var valid = JsonConvert.DeserializeObject<TryCreateOnlyPrimitive>("\"valid\"", settings);
        valid.Value.Should().Be("valid");

        var act = () => JsonConvert.DeserializeObject<TryCreateOnlyPrimitive>("\"invalid\"", settings);
        act.Should().Throw<JsonSerializationException>()
            .WithMessage("Invalid TryCreateOnlyPrimitive: Invalid value");
    }

    [Fact]
    public void UniversalConverter_TryCreateOnlyPrimitive_SuccessAndValidationFailure()
    {
        var valid = JsonConvert.DeserializeObject<TryCreateOnlyPrimitive>("\"valid\"", _settings);
        valid.Value.Should().Be("valid");

        var act = () => JsonConvert.DeserializeObject<TryCreateOnlyPrimitive>("\"invalid\"", _settings);
        act.Should().Throw<JsonSerializationException>()
            .WithMessage("Invalid TryCreateOnlyPrimitive: Invalid value");
    }

    [Fact]
    public void GenericConverter_CreateOnlyPrimitive_SuccessAndTargetInvocationException()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new DomainPrimitiveNewtonsoftJsonConverter<CreateOnlyPrimitive, string>());

        var item = CreateOnlyPrimitive.Create("valid");
        var json = JsonConvert.SerializeObject(item, settings);
        json.Should().Be("\"valid\"");

        var deserialized = JsonConvert.DeserializeObject<CreateOnlyPrimitive>(json, settings);
        deserialized.Value.Should().Be("valid");

        var act = () => JsonConvert.DeserializeObject<CreateOnlyPrimitive>("\"throw\"", settings);
        act.Should().Throw<JsonSerializationException>()
            .WithMessage("Invalid CreateOnlyPrimitive: Create failed");
    }

    [Fact]
    public void UniversalConverter_CreateOnlyPrimitive_SuccessAndTargetInvocationException()
    {
        var item = CreateOnlyPrimitive.Create("valid");
        var json = JsonConvert.SerializeObject(item, _settings);
        json.Should().Be("\"valid\"");

        var deserialized = JsonConvert.DeserializeObject<CreateOnlyPrimitive>(json, _settings);
        deserialized.Value.Should().Be("valid");

        var act = () => JsonConvert.DeserializeObject<CreateOnlyPrimitive>("\"throw\"", _settings);
        act.Should().Throw<JsonSerializationException>()
            .WithMessage("Invalid CreateOnlyPrimitive: Create failed");
    }

    [Fact]
    public void GenericConverter_NoFactoryPrimitive_ThrowsClearMessage()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new DomainPrimitiveNewtonsoftJsonConverter<NoFactoryPrimitive, string>());

        var act = () => JsonConvert.DeserializeObject<NoFactoryPrimitive>("\"something\"", settings);
        act.Should().Throw<JsonSerializationException>()
            .WithMessage($"Type {nameof(NoFactoryPrimitive)} does not define a suitable Create or TryCreate factory method.");
    }

    [Fact]
    public void UniversalConverter_NoFactoryPrimitive_ThrowsClearMessage()
    {
        var act = () => JsonConvert.DeserializeObject<NoFactoryPrimitive>("\"something\"", _settings);
        act.Should().Throw<JsonSerializationException>()
            .WithMessage($"Type {nameof(NoFactoryPrimitive)} does not define a suitable Create or TryCreate factory method.");
    }

    #endregion

    #region Custom Default and Value Serialization

    [Fact]
    public void GenericConverter_DirectWriteJson_WithCustomDefaultAndNonDefault()
    {
        var genericConv = new DomainPrimitiveNewtonsoftJsonConverter<CustomDefaultPrimitive, string>();
        var serializer = JsonSerializer.CreateDefault();

        var swDefault = new StringWriter();
        genericConv.WriteJson(new JsonTextWriter(swDefault), new CustomDefaultPrimitive("has-val", isDefault: true), serializer);
        swDefault.ToString().Should().Be("null");

        var swNonDefault = new StringWriter();
        genericConv.WriteJson(new JsonTextWriter(swNonDefault), new CustomDefaultPrimitive("my-value", isDefault: false), serializer);
        swNonDefault.ToString().Should().Be("\"my-value\"");
    }

    [Fact]
    public void UniversalConverter_DirectWriteJson_WithCustomDefaultAndNonDefault()
    {
        var universal = new DomainPrimitiveUniversalNewtonsoftJsonConverter();
        var serializer = JsonSerializer.CreateDefault();

        var swDefault = new StringWriter();
        universal.WriteJson(new JsonTextWriter(swDefault), new CustomDefaultPrimitive("has-val", isDefault: true), serializer);
        swDefault.ToString().Should().Be("null");

        var swNonDefault = new StringWriter();
        universal.WriteJson(new JsonTextWriter(swNonDefault), new CustomDefaultPrimitive("my-value", isDefault: false), serializer);
        swNonDefault.ToString().Should().Be("\"my-value\"");
    }

    #endregion

    #region Contract Resolver & Universal Converter Detection

    [Fact]
    public void ContractResolver_AutomaticallyAppliesToDomainPrimitives()
    {
        var settings = new JsonSerializerSettings
        {
            ContractResolver = new DomainPrimitivesContractResolver()
        };

        var email = EmailAddress.Create("admin@company.org");
        var json = JsonConvert.SerializeObject(email, settings);
        json.Should().Be("\"admin@company.org\"");

        var deserialized = JsonConvert.DeserializeObject<EmailAddress>(json, settings);
        deserialized.Should().Be(email);
    }

    [Fact]
    public void ContractResolver_WithComplexDto_SerializesAllMembers()
    {
        var settings = new JsonSerializerSettings
        {
            ContractResolver = new DomainPrimitivesContractResolver()
        };

        var dto = new TestDto
        {
            Email = EmailAddress.Create("dto@test.com"),
            Price = Price.Create(19.99m),
            NormalText = "Standard Property"
        };

        var json = JsonConvert.SerializeObject(dto, settings);
        json.Should().Contain("\"Email\":\"dto@test.com\"");
        json.Should().Contain("\"Price\":19.99");
        json.Should().Contain("\"NormalText\":\"Standard Property\"");

        var deserialized = JsonConvert.DeserializeObject<TestDto>(json, settings);
        deserialized!.Email.Value.Should().Be("dto@test.com");
        deserialized.Price.Value.Should().Be(19.99m);
        deserialized.NormalText.Should().Be("Standard Property");
    }

    [Fact]
    public void UniversalConverter_CanConvert_DetectsAll12AttributeKinds()
    {
        var universal = new DomainPrimitiveUniversalNewtonsoftJsonConverter();
        universal.CanConvert(typeof(StringDummyVo)).Should().BeTrue();
        universal.CanConvert(typeof(NumericDummyVo)).Should().BeTrue();
        universal.CanConvert(typeof(DateDummyVo)).Should().BeTrue();
        universal.CanConvert(typeof(StrongIdDummyVo)).Should().BeTrue();
        universal.CanConvert(typeof(SmartEnumDummyVo)).Should().BeTrue();
        universal.CanConvert(typeof(EmailDummyVo)).Should().BeTrue();
        universal.CanConvert(typeof(PhoneDummyVo)).Should().BeTrue();
        universal.CanConvert(typeof(UrlDummyVo)).Should().BeTrue();
        universal.CanConvert(typeof(MoneyDummyVo)).Should().BeTrue();
        universal.CanConvert(typeof(PercentageDummyVo)).Should().BeTrue();
        universal.CanConvert(typeof(BirthDateDummyVo)).Should().BeTrue();
        universal.CanConvert(typeof(ExpirationDateDummyVo)).Should().BeTrue();
        universal.CanConvert(typeof(SingleParamParseVo)).Should().BeTrue();
        universal.CanConvert(typeof(NonPrimitiveStruct)).Should().BeFalse();
    }

    [Fact]
    public void InterfaceOnlyPrimitive_UniversalConverter_RecognizesInterface()
    {
        var prim = InterfaceOnlyPrimitive.Create("test-val");
        var json = JsonConvert.SerializeObject(prim, _settings);
        json.Should().Be("\"test-val\"");

        var deserialized = JsonConvert.DeserializeObject<InterfaceOnlyPrimitive>(json, _settings);
        deserialized.Value.Should().Be("test-val");
    }

    #endregion

    #region Custom Readers & Readers Edge Cases

    [Fact]
    public void NullStringReader_ReturnsDefaultInstance()
    {
        var universal = new DomainPrimitiveUniversalNewtonsoftJsonConverter();
        var readerUni = new NullStringReader();
        var resultUni = universal.ReadJson(readerUni, typeof(EmailAddress), null, JsonSerializer.CreateDefault());
        resultUni.Should().Be(default(EmailAddress));

        var generic = new DomainPrimitiveNewtonsoftJsonConverter<EmailAddress, string>();
        var readerGen = new NullStringReader();
        var resultGen = generic.ReadJson(readerGen, typeof(EmailAddress), default, false, JsonSerializer.CreateDefault());
        resultGen.Should().Be(default(EmailAddress));
    }

    [Fact]
    public void DateTimeOffsetReader_DirectReadJson_DeserializesDateOnly()
    {
        var dto = new DateTimeOffset(2020, 10, 5, 12, 0, 0, TimeSpan.FromHours(2));
        var readerGeneric = new DateTimeOffsetReader(dto);
        var genericConv = new DomainPrimitiveNewtonsoftJsonConverter<CustomerBirthDate, DateOnly>();
        var resultGeneric = genericConv.ReadJson(readerGeneric, typeof(CustomerBirthDate), default, false, JsonSerializer.CreateDefault());
        resultGeneric.Value.Should().Be(new DateOnly(2020, 10, 5));

        var readerUniversal = new DateTimeOffsetReader(dto);
        var universalConv = new DomainPrimitiveUniversalNewtonsoftJsonConverter();
        var resultUniversal = (CustomerBirthDate)universalConv.ReadJson(readerUniversal, typeof(CustomerBirthDate), null, JsonSerializer.CreateDefault())!;
        resultUniversal.Value.Should().Be(new DateOnly(2020, 10, 5));
    }

    [Fact]
    public void DateTimeReader_DirectReadJson_DeserializesDateOnly()
    {
        var dt = new DateTime(2021, 3, 15, 8, 30, 0);
        var readerGeneric = new DateTimeReader(dt);
        var genericConv = new DomainPrimitiveNewtonsoftJsonConverter<CustomerBirthDate, DateOnly>();
        var resultGeneric = genericConv.ReadJson(readerGeneric, typeof(CustomerBirthDate), default, false, JsonSerializer.CreateDefault());
        resultGeneric.Value.Should().Be(new DateOnly(2021, 3, 15));

        var readerUniversal = new DateTimeReader(dt);
        var universalConv = new DomainPrimitiveUniversalNewtonsoftJsonConverter();
        var resultUniversal = (CustomerBirthDate)universalConv.ReadJson(readerUniversal, typeof(CustomerBirthDate), null, JsonSerializer.CreateDefault())!;
        resultUniversal.Value.Should().Be(new DateOnly(2021, 3, 15));
    }

    [Fact]
    public void NonDateTimeDateTokenReader_WithStringValue_ParsesSuccessfully()
    {
        var readerGeneric = new NonDateTimeDateTokenReader("1999-12-31");
        var genericConv = new DomainPrimitiveNewtonsoftJsonConverter<CustomerBirthDate, DateOnly>();
        var resultGeneric = genericConv.ReadJson(readerGeneric, typeof(CustomerBirthDate), default, false, JsonSerializer.CreateDefault());
        resultGeneric.Value.Should().Be(new DateOnly(1999, 12, 31));

        var readerUniversal = new NonDateTimeDateTokenReader("1999-12-31");
        var universalConv = new DomainPrimitiveUniversalNewtonsoftJsonConverter();
        var resultUniversal = (CustomerBirthDate)universalConv.ReadJson(readerUniversal, typeof(CustomerBirthDate), null, JsonSerializer.CreateDefault())!;
        resultUniversal.Value.Should().Be(new DateOnly(1999, 12, 31));
    }

    [Fact]
    public void NonDateTimeDateTokenReader_WithNonStringValue_FallsBackToDeserialize()
    {
        var readerGeneric = new NonDateTimeDateTokenReader(12345);
        var genericConv = new DomainPrimitiveNewtonsoftJsonConverter<CustomerBirthDate, DateOnly>();
        var actGeneric = () => genericConv.ReadJson(readerGeneric, typeof(CustomerBirthDate), default, false, JsonSerializer.CreateDefault());
        actGeneric.Should().Throw<JsonSerializationException>();

        var readerUniversal = new NonDateTimeDateTokenReader(12345);
        var universalConv = new DomainPrimitiveUniversalNewtonsoftJsonConverter();
        var actUniversal = () => universalConv.ReadJson(readerUniversal, typeof(CustomerBirthDate), null, JsonSerializer.CreateDefault());
        actUniversal.Should().Throw<JsonSerializationException>();
    }

    [Fact]
    public void NoValuePropertyPrimitive_UniversalConverter_FallsBackToStringValueType()
    {
        var universalConv = new DomainPrimitiveUniversalNewtonsoftJsonConverter();
        var reader = new JsonTextReader(new StringReader("\"some-text\""));
        reader.Read();
        var act = () => universalConv.ReadJson(reader, typeof(NoValuePropertyPrimitive), null, JsonSerializer.CreateDefault());
        act.Should().Throw<JsonSerializationException>();
    }

    [Fact]
    public void DateOnly_WithDateTimeOffsetParseHandling_DeserializesSuccessfully()
    {
        var settings = new JsonSerializerSettings
        {
            DateParseHandling = DateParseHandling.DateTimeOffset
        }.AddDomainPrimitives();

        var json = "\"1990-05-15T00:00:00+00:00\"";
        var deserialized = JsonConvert.DeserializeObject<CustomerBirthDate>(json, settings);
        deserialized.Value.Should().Be(new DateOnly(1990, 5, 15));

        var genericSettings = new JsonSerializerSettings
        {
            DateParseHandling = DateParseHandling.DateTimeOffset
        };
        genericSettings.Converters.Add(new DomainPrimitiveNewtonsoftJsonConverter<CustomerBirthDate, DateOnly>());
        var deserializedGeneric = JsonConvert.DeserializeObject<CustomerBirthDate>(json, genericSettings);
        deserializedGeneric.Value.Should().Be(new DateOnly(1990, 5, 15));
    }

    [Fact]
    public void UniversalConverter_WriteJson_WithNullPropertyOrNoValueProperty_WritesNull()
    {
        var universal = new DomainPrimitiveUniversalNewtonsoftJsonConverter();
        var serializer = JsonSerializer.CreateDefault();

        var swNull = new StringWriter();
        universal.WriteJson(new JsonTextWriter(swNull), new NullValuePrimitive(), serializer);
        swNull.ToString().Should().Be("null");

        var swNoProp = new StringWriter();
        universal.WriteJson(new JsonTextWriter(swNoProp), new NoValuePropertyPrimitive(), serializer);
        swNoProp.ToString().Should().Be("null");
    }

    [Fact]
    public void GenericConverter_WriteJson_WithNullPropertyOrNoValueProperty_WritesNull()
    {
        var serializer = JsonSerializer.CreateDefault();

        var convNull = new DomainPrimitiveNewtonsoftJsonConverter<NullValuePrimitive, string>();
        var swNull = new StringWriter();
        convNull.WriteJson(new JsonTextWriter(swNull), new NullValuePrimitive(), serializer);
        swNull.ToString().Should().Be("null");

        var convNoProp = new DomainPrimitiveNewtonsoftJsonConverter<NoValuePropertyPrimitive, string>();
        var swNoProp = new StringWriter();
        convNoProp.WriteJson(new JsonTextWriter(swNoProp), new NoValuePropertyPrimitive(), serializer);
        swNoProp.ToString().Should().Be("null");
    }

    [Fact]
    public void GenericConverter_WriteJson_DateOnly_WritesIsoDateString()
    {
        var genericConv = new DomainPrimitiveNewtonsoftJsonConverter<CustomerBirthDate, DateOnly>();
        var sw = new StringWriter();
        var birthDate = CustomerBirthDate.Create(new DateOnly(1995, 8, 20));
        genericConv.WriteJson(new JsonTextWriter(sw), birthDate, JsonSerializer.CreateDefault());
        sw.ToString().Should().Be("\"1995-08-20\"");
    }

    [Fact]
    public void GenericConverter_WriteJson_TimeOnly_WritesIsoTimeString()
    {
        var genericConv = new DomainPrimitiveNewtonsoftJsonConverter<WorkShiftTime, TimeOnly>();
        var sw = new StringWriter();
        var shiftTime = WorkShiftTime.Create(new TimeOnly(14, 30, 0));
        genericConv.WriteJson(new JsonTextWriter(sw), shiftTime, JsonSerializer.CreateDefault());
        sw.ToString().Should().Be("\"14:30:00\"");
    }

    [Fact]
    public void GenericConverter_ReadJson_NullToken_ReturnsDefault()
    {
        var genericConv = new DomainPrimitiveNewtonsoftJsonConverter<EmailAddress, string>();
        var reader = new JsonTextReader(new StringReader("null"));
        reader.Read();
        var result = genericConv.ReadJson(reader, typeof(EmailAddress), default, false, JsonSerializer.CreateDefault());
        result.IsDefault.Should().BeTrue();
    }

    [Fact]
    public void UniversalConverter_WriteJson_DateOnly_WritesIsoDateString()
    {
        var universal = new DomainPrimitiveUniversalNewtonsoftJsonConverter();
        var sw = new StringWriter();
        var birthDate = CustomerBirthDate.Create(new DateOnly(1995, 8, 20));
        universal.WriteJson(new JsonTextWriter(sw), birthDate, JsonSerializer.CreateDefault());
        sw.ToString().Should().Be("\"1995-08-20\"");
    }

    [Fact]
    public void UniversalConverter_WriteJson_TimeOnly_WritesIsoTimeString()
    {
        var universal = new DomainPrimitiveUniversalNewtonsoftJsonConverter();
        var sw = new StringWriter();
        var shiftTime = WorkShiftTime.Create(new TimeOnly(14, 30, 0));
        universal.WriteJson(new JsonTextWriter(sw), shiftTime, JsonSerializer.CreateDefault());
        sw.ToString().Should().Be("\"14:30:00\"");
    }

    [Fact]
    public void UniversalConverter_ReadJson_NullToken_ReturnsDefault()
    {
        var universal = new DomainPrimitiveUniversalNewtonsoftJsonConverter();
        var reader = new JsonTextReader(new StringReader("null"));
        reader.Read();
        var result = universal.ReadJson(reader, typeof(EmailAddress), null, JsonSerializer.CreateDefault());
        ((EmailAddress)result!).IsDefault.Should().BeTrue();
    }

    [Fact]
    public void UniversalConverter_ReadJson_NonPrimitiveType_DeserializesNormally()
    {
        var universal = new DomainPrimitiveUniversalNewtonsoftJsonConverter();
        var reader = new JsonTextReader(new StringReader("123"));
        reader.Read();
        var result = universal.ReadJson(reader, typeof(int), null, JsonSerializer.CreateDefault());
        result.Should().Be(123);
    }

    #endregion

    #region Extension Methods

    [Fact]
    public void ExtensionMethods_AddDomainPrimitives_OnSettings_AddsConverter()
    {
        var settings = new JsonSerializerSettings();
        var returned = settings.AddDomainPrimitives();
        returned.Should().BeSameAs(settings);
        settings.Converters.Should().ContainSingle(c => c is DomainPrimitiveUniversalNewtonsoftJsonConverter);
    }

    [Fact]
    public void ExtensionMethods_AddDomainPrimitives_OnSerializer_AddsConverter()
    {
        var serializer = new JsonSerializer();
        var returned = serializer.AddDomainPrimitives();
        returned.Should().BeSameAs(serializer);
        serializer.Converters.Should().ContainSingle(c => c is DomainPrimitiveUniversalNewtonsoftJsonConverter);

        var sw = new StringWriter();
        serializer.Serialize(sw, EmailAddress.Create("ext@test.com"));
        sw.ToString().Should().Be("\"ext@test.com\"");
    }

    [Fact]
    public void ExtensionMethods_NullArguments_ThrowsArgumentNullException()
    {
        var actSettings = () => NewtonsoftJsonExtensions.AddDomainPrimitives((JsonSerializerSettings)null!);
        actSettings.Should().Throw<ArgumentNullException>();

        var actSerializer = () => NewtonsoftJsonExtensions.AddDomainPrimitives((JsonSerializer)null!);
        actSerializer.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GenericConverter_ReadJson_IntPrimitive_NullToken_ReturnsDefault()
    {
        var genericConv = new DomainPrimitiveNewtonsoftJsonConverter<OrderNumber, int>();
        var reader = new JsonTextReader(new StringReader("null"));
        reader.Read();
        var result = genericConv.ReadJson(reader, typeof(OrderNumber), default, false, JsonSerializer.CreateDefault());
        result.IsDefault.Should().BeTrue();
    }

    [Fact]
    public void TimeOnly_NonDateTimeDateTokenReader_ParsesSuccessfully()
    {
        var readerGen = new NonDateTimeDateTokenReader("08:30:00");
        var genericConv = new DomainPrimitiveNewtonsoftJsonConverter<WorkShiftTime, TimeOnly>();
        var resultGen = genericConv.ReadJson(readerGen, typeof(WorkShiftTime), default, false, JsonSerializer.CreateDefault());
        resultGen.Value.Should().Be(new TimeOnly(8, 30, 0));

        var readerUni = new NonDateTimeDateTokenReader("08:30:00");
        var universalConv = new DomainPrimitiveUniversalNewtonsoftJsonConverter();
        var resultUni = (WorkShiftTime)universalConv.ReadJson(readerUni, typeof(WorkShiftTime), null, JsonSerializer.CreateDefault())!;
        resultUni.Value.Should().Be(new TimeOnly(8, 30, 0));
    }

    [Fact]
    public void UniversalConverter_WriteJson_NullValue_WritesNull()
    {
        var universal = new DomainPrimitiveUniversalNewtonsoftJsonConverter();
        var sw = new StringWriter();
        universal.WriteJson(new JsonTextWriter(sw), null, JsonSerializer.CreateDefault());
        sw.ToString().Should().Be("null");
    }

    [Fact]
    public void UniversalConverter_WriteJson_NonPrimitiveStruct_WritesNull()
    {
        var universal = new DomainPrimitiveUniversalNewtonsoftJsonConverter();
        var sw = new StringWriter();
        universal.WriteJson(new JsonTextWriter(sw), new NonPrimitiveStruct(), JsonSerializer.CreateDefault());
        sw.ToString().Should().Be("null");
    }

    [Fact]
    public void ValueObject_Serialization_OmitsIsDefaultProperty()
    {
        var json = JsonConvert.SerializeObject(new ReadOnlyPropVo { Title = "Hello" }, _settings);
        json.Should().NotContain("IsDefault");
    }

    [Fact]
    public void Other3ParamMethodVo_DoesNotMatchTryCreateMethod()
    {
        DomainPrimitiveUniversalNewtonsoftJsonConverter.ClearCache();
        var item = JsonConvert.DeserializeObject<Other3ParamMethodVo>("\"my-val\"", _settings);
        item.Value.Should().Be("my-val");
    }

    [Fact]
    public void Other1ParamMethodVo_DoesNotMatchParseMethod()
    {
        DomainPrimitiveUniversalNewtonsoftJsonConverter.ClearCache();
        var vo = JsonConvert.DeserializeObject<Other1ParamMethodVo>("{\"Name\":\"Test\"}", _settings);
        vo.Name.Should().Be("Test");
    }

    [Fact]
    public void StringPrimitive_WithDateOrTimeString_DeserializesCorrectly()
    {
        var itemFromDate = JsonConvert.DeserializeObject<InterfaceOnlyPrimitive>("\"2023-05-15\"", _settings);
        itemFromDate.Value.Should().Be("2023-05-15");

        var itemFromTime = JsonConvert.DeserializeObject<InterfaceOnlyPrimitive>("\"14:30:00\"", _settings);
        itemFromTime.Value.Should().Be("14:30:00");

        var genericConv = new DomainPrimitiveNewtonsoftJsonConverter<InterfaceOnlyPrimitive, string>();
        var readerDate = new JsonTextReader(new StringReader("\"2023-05-15\""));
        readerDate.Read();
        var itemGenDate = genericConv.ReadJson(readerDate, typeof(InterfaceOnlyPrimitive), default, false, JsonSerializer.CreateDefault());
        itemGenDate.Value.Should().Be("2023-05-15");

        var readerTime = new JsonTextReader(new StringReader("\"14:30:00\""));
        readerTime.Read();
        var itemGenTime = genericConv.ReadJson(readerTime, typeof(InterfaceOnlyPrimitive), default, false, JsonSerializer.CreateDefault());
        itemGenTime.Value.Should().Be("14:30:00");
    }

    [Fact]
    public void TimeOnly_CustomStringReader_ParsesSuccessfully()
    {
        var readerGen = new CustomStringReader("14:30:00");
        var genericConv = new DomainPrimitiveNewtonsoftJsonConverter<WorkShiftTime, TimeOnly>();
        var resultGen = genericConv.ReadJson(readerGen, typeof(WorkShiftTime), default, false, JsonSerializer.CreateDefault());
        resultGen.Value.Should().Be(new TimeOnly(14, 30, 0));

        var readerUni = new CustomStringReader("14:30:00");
        var universalConv = new DomainPrimitiveUniversalNewtonsoftJsonConverter();
        var resultUni = (WorkShiftTime)universalConv.ReadJson(readerUni, typeof(WorkShiftTime), null, JsonSerializer.CreateDefault())!;
        resultUni.Value.Should().Be(new TimeOnly(14, 30, 0));
    }

    [Fact]
    public void TimeOnly_DateTimeReader_ParsesSuccessfully()
    {
        var dt = new DateTime(2026, 5, 20, 16, 45, 30);
        var readerGen = new DateTimeReader(dt);
        var genericConv = new DomainPrimitiveNewtonsoftJsonConverter<WorkShiftTime, TimeOnly>();
        var resultGen = genericConv.ReadJson(readerGen, typeof(WorkShiftTime), default, false, JsonSerializer.CreateDefault());
        resultGen.Value.Should().Be(new TimeOnly(16, 45, 30));

        var readerUni = new DateTimeReader(dt);
        var universalConv = new DomainPrimitiveUniversalNewtonsoftJsonConverter();
        var resultUni = (WorkShiftTime)universalConv.ReadJson(readerUni, typeof(WorkShiftTime), null, JsonSerializer.CreateDefault())!;
        resultUni.Value.Should().Be(new TimeOnly(16, 45, 30));
    }

    [Fact]
    public void NoValuePropertyPrimitive_WithTryCreateMethod_ThrowsBecauseNoValueProperty()
    {
        DomainPrimitiveUniversalNewtonsoftJsonConverter.ClearCache();
        var act = () => JsonConvert.DeserializeObject<NoValuePropertyPrimitive>("\"test\"", _settings);
        act.Should().Throw<JsonSerializationException>()
            .WithMessage("*does not define a suitable Create or TryCreate*");
    }

    [Fact]
    public void VoWithOtherStringParamMethod_DoesNotMatchParseMethod()
    {
        DomainPrimitiveUniversalNewtonsoftJsonConverter.ClearCache();
        var vo = JsonConvert.DeserializeObject<VoWithOtherStringParamMethod>("{\"Description\":\"MyDesc\"}", _settings);
        vo.Description.Should().Be("MyDesc");
    }

    [Fact]
    public void UniversalConverter_CanConvert_ChecksPrimitives()
    {
        var universal = new DomainPrimitiveUniversalNewtonsoftJsonConverter();
        DomainPrimitiveUniversalNewtonsoftJsonConverter.ClearCache();
        universal.CanConvert(typeof(EmailAddress)).Should().BeTrue();
        universal.CanConvert(typeof(NonPrimitiveStruct)).Should().BeFalse();
        universal.CanConvert(typeof(string)).Should().BeFalse();
        universal.CanConvert(typeof(int)).Should().BeFalse();
    }

    #endregion

    private static JsonSerializerSettings CreateSettingsWithGenericConverter<TPrimitive, TValue>()
        where TPrimitive : struct, IDomainPrimitive<TPrimitive, TValue>
        where TValue : notnull, IEquatable<TValue>, IComparable<TValue>
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new DomainPrimitiveNewtonsoftJsonConverter<TPrimitive, TValue>());
        return settings;
    }
}
