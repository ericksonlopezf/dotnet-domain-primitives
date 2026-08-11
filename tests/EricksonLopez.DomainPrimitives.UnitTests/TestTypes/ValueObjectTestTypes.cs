using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.Tests.TestTypes;

[ValueObject]
public readonly partial record struct Address
{
    public required string Street { get; init; }
    public required string City { get; init; }
    public required string State { get; init; }
    public required string ZipCode { get; init; }

    static partial void Validate(ref Address value, ref EricksonLopez.DomainPrimitives.Validation.PrimitiveError errors)
    {
        if (string.IsNullOrWhiteSpace(value.Street))
            errors = new EricksonLopez.DomainPrimitives.Validation.PrimitiveError("Address", "Street cannot be empty.");
    }
}
