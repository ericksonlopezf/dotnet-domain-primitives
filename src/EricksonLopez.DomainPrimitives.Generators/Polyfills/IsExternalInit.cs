// Copyright © Erickson Lopez. MIT License.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Polyfill for netstandard2.0 — enables C# 9+ record types and init properties.
// This type is required by the compiler but not available in netstandard2.0.

#if !NET5_0_OR_GREATER
namespace System.Runtime.CompilerServices;

internal static class IsExternalInit { }
#endif
