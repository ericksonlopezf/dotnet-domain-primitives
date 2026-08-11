using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.Tests.TestTypes;

/// <summary>
/// Test strong ID backed by Guid.
/// </summary>
[StrongId<Guid>]
public readonly partial record struct CustomerId;

/// <summary>
/// Test strong ID backed by int.
/// </summary>
[StrongId<int>]
public readonly partial record struct OrderNumber;

/// <summary>
/// Test strong ID backed by long.
/// </summary>
[StrongId<long>]
public readonly partial record struct TransactionId;

/// <summary>
/// Test strong ID backed by string with required length constraints.
/// A Sku without bounds would be flagged by DP0009 (MED-001).
/// </summary>
[StrongId<string>]
[MinLength(1), MaxLength(50)]
public readonly partial record struct Sku;
