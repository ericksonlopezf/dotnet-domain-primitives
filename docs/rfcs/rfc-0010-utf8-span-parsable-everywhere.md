# RFC 0005: IUtf8SpanParsable Implementation Everywhere

## Context and Problem Statement
High-performance APIs (such as ASP.NET Core) are heavily moving towards UTF-8 byte processing to avoid string allocations when reading from network streams. If Domain Primitives only support string parsing, they become a bottleneck in high-throughput applications.

## Proposed Solution
All generated Domain Primitives will implement IUtf8SpanParsable<TSelf> and IUtf8SpanFormattable (introduced in .NET 8).
This allows frameworks like ASP.NET Core and System.Text.Json to parse and format Domain Primitives directly from/to UTF-8 byte buffers without allocating intermediate string objects.

## Decision Outcome
Approved. The source generators will unconditionally emit IUtf8SpanParsable<TSelf> and IUtf8SpanFormattable when compiling against 
et8.0 or greater.
