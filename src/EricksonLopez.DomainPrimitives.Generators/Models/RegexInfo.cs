// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives.Generators.Models;

/// <summary>
/// Information about a regex pattern applied to a string primitive.
/// </summary>
internal sealed record RegexInfo(string Pattern, string? ErrorMessage);
