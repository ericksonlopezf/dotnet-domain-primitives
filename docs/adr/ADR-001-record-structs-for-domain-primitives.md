# 1. Record Structs for Domain Primitives

Date: 2026-08-01

## Status

Accepted

## Context

We need a zero-allocation, immutable, and equality-comparable type for domain primitives.

## Decision

We will use 
eadonly partial record struct in C# 10+.

## Consequences

- Zero boxing allocations.
- Structural equality out of the box.
- Requires C# 10 or later.
