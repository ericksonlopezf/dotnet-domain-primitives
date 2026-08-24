// Copyright © Erickson Lopez. MIT License.
using System;
using System.Text.Json;
using AwesomeAssertions;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.DomainPrimitives.Testing;
using Xunit;

namespace EricksonLopez.DomainPrimitives.EndToEndTests;

[StrongId<Guid>]
public readonly partial record struct AccountId;

[Email]
public readonly partial record struct AccountEmail;

[Money]
public readonly partial record struct AccountBalance;

[BirthDate]
public readonly partial record struct OwnerBirthDate;

[SmartEnum<int>]
public readonly partial record struct AccountTier
{
    public static readonly AccountTier Bronze = new(1, "Bronze");
    public static readonly AccountTier Silver = new(2, "Silver");
    public static readonly AccountTier Gold = new(3, "Gold");
}

public sealed record UserAccountAggregate
{
    public AccountId Id { get; init; }
    public AccountEmail Email { get; init; }
    public AccountBalance Balance { get; init; }
    public OwnerBirthDate BirthDate { get; init; }
    public AccountTier Tier { get; init; }
}

public class DomainPrimitiveLifecycleTests
{
    [Fact]
    public void SerializeAndDeserialize_WithSinglePrimitive_RoundtripsSuccessfully()
    {
        // Arrange: Raw JSON string simulating payload
        string expectedEmail = DomainPrimitiveFakeFactory.Strings.ValidEmail;
        string jsonPayload = $"\"{expectedEmail}\"";

        // Act 1: Deserialization
        var userEmail = JsonSerializer.Deserialize<AccountEmail>(jsonPayload);

        // Assert 1: Valid instance created with correct value
        userEmail.Value.Should().Be(expectedEmail);
        userEmail.IsDefault.Should().BeFalse();

        // Act 2: Serialization
        string serializedJson = JsonSerializer.Serialize(userEmail);

        // Assert 2: Matches expected JSON string
        serializedJson.Should().Be(jsonPayload);
    }

    [Fact]
    public void SerializeAndDeserialize_WithComplexAggregate_PreservesAllDomainPrimitives()
    {
        // Arrange: Construct an aggregate holding 5 distinct Domain Primitive types
        var id = AccountId.Create(DomainPrimitiveFakeFactory.Identifiers.ValidGuid);
        var email = AccountEmail.Create(DomainPrimitiveFakeFactory.Strings.ValidEmail);
        var balance = AccountBalance.Create(1500.50m);
        var birthDate = OwnerBirthDate.Create(DomainPrimitiveFakeFactory.Dates.ValidBirthDate);
        var tier = AccountTier.Gold;

        var originalAggregate = new UserAccountAggregate
        {
            Id = id,
            Email = email,
            Balance = balance,
            BirthDate = birthDate,
            Tier = tier
        };

        // Act 1: Serialize aggregate to JSON payload
        string json = JsonSerializer.Serialize(originalAggregate);
        json.Should().NotBeNullOrWhiteSpace();

        // Act 2: Deserialize back to aggregate
        var restoredAggregate = JsonSerializer.Deserialize<UserAccountAggregate>(json);

        // Assert: Full roundtrip equality and invariance guarantee
        restoredAggregate.Should().NotBeNull();
        restoredAggregate!.Id.Should().Be(id);
        restoredAggregate.Email.Should().Be(email);
        restoredAggregate.Balance.Should().Be(balance);
        restoredAggregate.BirthDate.Should().Be(birthDate);
        restoredAggregate.Tier.Should().Be(tier);
    }

    [Fact]
    public void DeserializeAndSerialize_WithUnnormalizedInput_PreservesNormalizationThroughRoundtrip()
    {
        // Arrange: Raw input with irregular formatting (whitespace and uppercase)
        string rawInput = "  USER.NAME@EXAMPLE.COM  ";
        string expectedNormalized = "user.name@example.com";
        string inputJson = $"\"{rawInput}\"";

        // Act: Deserialize un-normalized JSON
        var email = JsonSerializer.Deserialize<AccountEmail>(inputJson);

        // Assert: Normalization occurred automatically
        email.Value.Should().Be(expectedNormalized);

        // Act: Re-serialize
        string outputJson = JsonSerializer.Serialize(email);

        // Assert: Output contains strictly normalized form
        outputJson.Should().Be($"\"{expectedNormalized}\"");
    }

    [Fact]
    public void Deserialize_WithInvalidPayload_ThrowsJsonExceptionPreservingInvariants()
    {
        // Arrange: Invalid email JSON payload
        string invalidEmailJson = "\"not-a-valid-email-address\"";

        // Act: Attempt deserialization
        Action act = () => JsonSerializer.Deserialize<AccountEmail>(invalidEmailJson);

        // Assert: Throws JsonException preventing invalid state creation
        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void EqualsAndHashCode_WithIdenticalPrimitives_GuaranteesValueSemantics()
    {
        // Arrange
        var guid = DomainPrimitiveFakeFactory.Identifiers.ValidGuid;
        var id1 = AccountId.Create(guid);
        var id2 = AccountId.Create(guid);

        // Assert: Value semantics
        id1.Should().Be(id2);
        (id1 == id2).Should().BeTrue();
        id1.GetHashCode().Should().Be(id2.GetHashCode());

        // Default instance protection
        AccountId defaultId = default;
        defaultId.IsDefault.Should().BeTrue();
    }
}



