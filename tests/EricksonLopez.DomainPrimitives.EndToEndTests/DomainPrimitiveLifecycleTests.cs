using System;
using System.Text.Json;
using FluentAssertions;
using EricksonLopez.DomainPrimitives.Testing;
using Xunit;

namespace EricksonLopez.DomainPrimitives.EndToEndTests;

[Email]
public readonly partial record struct UserEmail;

public class DomainPrimitiveLifecycleTests
{
    [Fact]
    public void Should_Complete_Full_Lifecycle_Successfully()
    {
        // Arrange: A raw JSON payload (simulating an HTTP Request or DB record)
        string expectedEmail = DomainPrimitiveFakeFactory.ValidEmail;
        string jsonPayload = $"\"{expectedEmail}\"";

        // Act 1: Deserialization (Simulating ASP.NET Core Model Binding or EF Core JSON)
        var userEmail = JsonSerializer.Deserialize<UserEmail>(jsonPayload);

        // Assert 1
        userEmail.Value.Should().Be(expectedEmail);

        // Act 2: Operations and Serialization (Simulating HTTP Response)
        string serializedJson = JsonSerializer.Serialize(userEmail);

        // Assert 2
        serializedJson.Should().Be(jsonPayload);
    }
}
