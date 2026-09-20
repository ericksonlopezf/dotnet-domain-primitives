// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.UnitTests.TestTypes;

/// <summary>
/// URL with custom allowed schemes.
/// </summary>
[Url(AllowedSchemes = new[] { "https", "ftp" })]
public readonly partial record struct SecureFtpUrl;
