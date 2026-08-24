```
BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.3194)
AMD Ryzen 7 9800X3D 4.70GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v4
  Job-NET10  : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v4

Job=Job-NET10  Runtime=.NET 10.0  
```
| Method | Mean | Error | StdDev | Ratio | RatioSD | Allocated | Alloc Ratio |
|---|---:|---:|---:|---:|---:|---:|---:|
| RawGuid | 0.0000 ns | 0.0000 ns | 0.0000 ns | 1.00 | 0.00 | - | NA |
| PrimitiveGuid | 0.0000 ns | 0.0000 ns | 0.0000 ns | 1.00 | 0.00 | - | NA |
| PrimitiveGuid_TryParse | 12.6310 ns | 0.0650 ns | 0.0600 ns | NA | NA | - | NA |
| PrimitiveEmail_Create | 49.5310 ns | 0.2810 ns | 0.2610 ns | NA | NA | - | NA |
| PrimitiveEmail_JsonSerialize | 102.3410 ns | 0.5420 ns | 0.5040 ns | NA | NA | 64 B | NA |
| PrimitiveEmail_JsonDeserialize | 95.5820 ns | 0.5010 ns | 0.4680 ns | NA | NA | 120 B | NA |
