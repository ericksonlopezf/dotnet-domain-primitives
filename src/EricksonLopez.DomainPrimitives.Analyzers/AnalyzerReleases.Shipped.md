; Shipped analyzer release
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

## Release 1.0.0-alpha.1

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
DP0001 | EricksonLopez.DomainPrimitives | Error | Domain primitive must be partial
DP0002 | EricksonLopez.DomainPrimitives | Error | Domain primitive must be readonly
DP0003 | EricksonLopez.DomainPrimitives | Error | Domain primitive must be a record struct
DP0004 | EricksonLopez.DomainPrimitives | Error | Invalid Regex Pattern
DP0005 | EricksonLopez.DomainPrimitives | Error | Conflicting normalization attributes
DP0006 | EricksonLopez.DomainPrimitives | Error | Invalid constraint bounds
DP0007 | EricksonLopez.DomainPrimitives | Warning | Avoid uninitialized domain primitive
DP0008 | EricksonLopez.DomainPrimitives | Error | Value object properties must use 'init'
DP0009 | EricksonLopez.DomainPrimitives | Warning | Missing validation
DP0010 | EricksonLopez.DomainPrimitives | Warning | String compared directly with domain primitive
DP0011 | EricksonLopez.DomainPrimitives | Warning | String assigned directly from domain primitive without accessing .Value
DP0012 | EricksonLopez.DomainPrimitives | Warning | Public constructor bypasses domain primitive validation
DP0013 | EricksonLopez.DomainPrimitives | Info | Possible duplicate domain primitive logic detected

## Release 1.1.0

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
DP0014 | EricksonLopez.DomainPrimitives | Warning | API Surface Budget Exceeded
DP0015 | EricksonLopez.DomainPrimitives | Warning | Missing XML Documentation
DP0016 | EricksonLopez.DomainPrimitives | Warning | Invalid Factory Method Name
