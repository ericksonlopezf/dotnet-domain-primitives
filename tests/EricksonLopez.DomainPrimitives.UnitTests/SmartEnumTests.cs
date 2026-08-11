using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.DomainPrimitives.UnitTests.TestTypes;
using FluentAssertions;
using Xunit;

namespace EricksonLopez.DomainPrimitives.UnitTests;

public class SmartEnumTests
{
    [Fact]
    public void Match_ExecutesCorrectCase()
    {
        var status = TestOrderStatus.Processing;

        var result = status.Match(
            whenPending: () => "PendingCase",
            whenProcessing: () => "ProcessingCase",
            whenCompleted: () => "CompletedCase"
        );

        result.Should().Be("ProcessingCase");
    }

    [Fact]
    public void Switch_ExecutesCorrectAction()
    {
        var status = TestOrderStatus.Completed;
        var executed = false;

        status.Switch(
            whenPending: () => { },
            whenProcessing: () => { },
            whenCompleted: () => { executed = true; }
        );

        executed.Should().BeTrue();
    }

    [Fact]
    public void TryFromValue_ReturnsTrue_ForValidValue()
    {
        TestOrderStatus.TryFromValue(1, out var status).Should().BeTrue();
        status.Should().Be(TestOrderStatus.Pending);
    }

    [Fact]
    public void TryFromName_ReturnsTrue_ForValidName()
    {
        TestOrderStatus.TryFromName("Completed", out var status).Should().BeTrue();
        status.Should().Be(TestOrderStatus.Completed);
    }
}
