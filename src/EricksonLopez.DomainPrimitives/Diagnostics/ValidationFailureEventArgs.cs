// Copyright © Erickson Lopez. MIT License.

namespace EricksonLopez.DomainPrimitives.Diagnostics;

/// <summary>Represents event arguments describing a domain primitive validation failure.</summary>
/// <param name="PrimitiveName">The name of the domain primitive type that failed validation (e.g., <c>"EmailAddress"</c>).</param>
/// <param name="ErrorType">The error code identifying the failure category (e.g., <c>"FORMAT"</c>, <c>"LENGTH"</c>).</param>
/// <param name="ErrorMessage">The human-readable description of the validation failure.</param>
public readonly record struct ValidationFailureEventArgs(string PrimitiveName, string ErrorType, string ErrorMessage);
