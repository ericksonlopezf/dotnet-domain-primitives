using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
// TD-001: NativeAOT trimming probe for EricksonLopez.DomainPrimitives.Abstractions
using EricksonLopez.DomainPrimitives;
using EricksonLopez.DomainPrimitives.Validation;

Console.WriteLine("NativeAOT probe started.");

var error = new PrimitiveError("TEST", "Test message");
Console.WriteLine($"PrimitiveError: {error.Code} / IsError: {error.IsError}");

var noError = PrimitiveError.None;
Console.WriteLine($"None.IsError: {noError.IsError}");

var stringAttr = typeof(StringPrimitiveAttribute);
var strongIdAttr = typeof(StrongIdAttribute<System.Guid>);
Console.WriteLine($"Attributes: {stringAttr.Name}, {strongIdAttr.Name}");

Console.WriteLine("NativeAOT probe completed.");
