// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using EricksonLopez.DomainPrimitives;
using System.Threading.Tasks;

namespace EricksonLopez.DomainPrimitives.UnitTests.TestTypes;

[SmartEnum<int>]
public readonly partial record struct TestOrderStatus
{
    public static readonly TestOrderStatus Pending = new(1, nameof(Pending));
    public static readonly TestOrderStatus Processing = new(2, nameof(Processing));
    public static readonly TestOrderStatus Completed = new(3, nameof(Completed));
}


