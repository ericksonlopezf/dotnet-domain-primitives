// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives.Generators.Models;

/// <summary>
/// Global assembly-level defaults extracted from [assembly: DomainPrimitivesDefaults].
/// </summary>
internal readonly record struct AssemblyDefaultsInfo(
    bool Trim,
    bool NotEmpty,
    int? MaxLength,
    string? ExceptionTypeFullName);
