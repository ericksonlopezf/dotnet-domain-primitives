// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using AwesomeAssertions;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.DomainPrimitives.OpenApi.Generated;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Xunit;

namespace EricksonLopez.DomainPrimitives.OpenApi.Tests;

[Email]
public readonly partial record struct OpenApiTestEmail;

[StrongId<Guid>]
public readonly partial record struct OpenApiTestUserId;

[StrongId<int>]
public readonly partial record struct OpenApiTestOrderId;

[Money]
public readonly partial record struct OpenApiTestPrice;

[Percentage]
public readonly partial record struct OpenApiTestTax;

[DatePrimitive(Kind = 0)]
public readonly partial record struct OpenApiTestDate;

[SmartEnum<int>]
public readonly partial record struct OpenApiTestStatus
{
    public static readonly OpenApiTestStatus Active = new(1, "Active");
    public static readonly OpenApiTestStatus Inactive = new(2, "Inactive");
}

public class OpenApiSchemaFilterTests
{
    private readonly DomainPrimitivesSchemaFilter _filter = new();

    private static SchemaFilterContext CreateContext(Type type)
    {
        return new SchemaFilterContext(type, null, null);
    }

    [Fact]
    public void Apply_WithEmailPrimitive_SetsTypeStringAndFormatEmail()
    {
        var schema = new OpenApiSchema();
        var context = CreateContext(typeof(OpenApiTestEmail));

        _filter.Apply(schema, context);

        schema.Type.Should().Be("string");
        schema.Format.Should().Be("email");
    }

    [Fact]
    public void Apply_WithGuidStrongId_SetsTypeStringAndFormatUuid()
    {
        var schema = new OpenApiSchema();
        var context = CreateContext(typeof(OpenApiTestUserId));

        _filter.Apply(schema, context);

        schema.Type.Should().Be("string");
        schema.Format.Should().Be("uuid");
    }

    [Fact]
    public void Apply_WithIntStrongId_SetsTypeInteger()
    {
        var schema = new OpenApiSchema();
        var context = CreateContext(typeof(OpenApiTestOrderId));

        _filter.Apply(schema, context);

        schema.Type.Should().Be("integer");
    }

    [Fact]
    public void Apply_WithMoney_SetsTypeNumberAndFormatDouble()
    {
        var schema = new OpenApiSchema();
        var context = CreateContext(typeof(OpenApiTestPrice));

        _filter.Apply(schema, context);

        schema.Type.Should().Be("number");
        schema.Format.Should().Be("double");
    }

    [Fact]
    public void Apply_WithPercentage_SetsTypeNumber()
    {
        var schema = new OpenApiSchema();
        var context = CreateContext(typeof(OpenApiTestTax));

        _filter.Apply(schema, context);

        schema.Type.Should().Be("number");
    }

    [Fact]
    public void Apply_WithDatePrimitive_SetsTypeStringAndFormatDate()
    {
        var schema = new OpenApiSchema();
        var context = CreateContext(typeof(OpenApiTestDate));

        _filter.Apply(schema, context);

        schema.Type.Should().Be("string");
        schema.Format.Should().Be("date");
    }

    [Fact]
    public void Apply_WithSmartEnum_PopulatesEnumValues()
    {
        var schema = new OpenApiSchema();
        var context = CreateContext(typeof(OpenApiTestStatus));

        _filter.Apply(schema, context);

        schema.Type.Should().Be("integer");
        schema.Enum.Should().NotBeNull();
        schema.Enum.Should().HaveCount(2);
    }

    [Fact]
    public void Apply_WithUnregisteredType_DoesNotModifySchema()
    {
        var schema = new OpenApiSchema { Title = "OriginalTitle", Type = "object" };
        var context = CreateContext(typeof(string));

        _filter.Apply(schema, context);

        schema.Title.Should().Be("OriginalTitle");
        schema.Type.Should().Be("object");
    }
}


