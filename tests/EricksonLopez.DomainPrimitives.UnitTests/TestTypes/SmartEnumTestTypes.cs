using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.UnitTests.TestTypes;

[SmartEnum<int>]
public readonly partial record struct TestOrderStatus
{
    public static readonly TestOrderStatus Pending = new(1, nameof(Pending));
    public static readonly TestOrderStatus Processing = new(2, nameof(Processing));
    public static readonly TestOrderStatus Completed = new(3, nameof(Completed));
}
