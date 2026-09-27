```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.401
  [Host]    : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v3
  .NET 8.0  : .NET 8.0.31 (8.0.31, 8.0.3126.42015), X64 RyuJIT x86-64-v3
  .NET 9.0  : .NET 9.0.20 (9.0.20, 9.0.2026.41315), X64 RyuJIT x86-64-v3


```
| Method                         | Job       | Runtime   | Mean        | Error     | StdDev    | Ratio    | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------------------------- |---------- |---------- |------------:|----------:|----------:|---------:|--------:|-------:|----------:|------------:|
| RawGuid                        | .NET 10.0 | .NET 10.0 |   0.3390 ns | 0.0260 ns | 0.0203 ns |     1.92 |    0.11 |      - |         - |          NA |
| PrimitiveGuid                  | .NET 10.0 | .NET 10.0 |   0.8820 ns | 0.0044 ns | 0.0041 ns |     4.99 |    0.06 |      - |         - |          NA |
| PrimitiveGuid_TryParse         | .NET 10.0 | .NET 10.0 |  26.8381 ns | 0.0338 ns | 0.0283 ns |   151.84 |    1.72 |      - |         - |          NA |
| PrimitiveEmail_Create          | .NET 10.0 | .NET 10.0 | 110.2937 ns | 0.0840 ns | 0.0656 ns |   624.00 |    7.05 |      - |         - |          NA |
| PrimitiveEmail_JsonSerialize   | .NET 10.0 | .NET 10.0 | 251.0044 ns | 0.5275 ns | 0.4676 ns | 1,420.08 |   16.23 | 0.0038 |      64 B |          NA |
| PrimitiveEmail_JsonDeserialize | .NET 10.0 | .NET 10.0 | 225.5240 ns | 0.2814 ns | 0.2632 ns | 1,275.92 |   14.47 | 0.0072 |     120 B |          NA |
| RawGuid                        | .NET 8.0  | .NET 8.0  |   0.1768 ns | 0.0023 ns | 0.0021 ns |     1.00 |    0.02 |      - |         - |          NA |
| PrimitiveGuid                  | .NET 8.0  | .NET 8.0  |   1.0531 ns | 0.0013 ns | 0.0012 ns |     5.96 |    0.07 |      - |         - |          NA |
| PrimitiveGuid_TryParse         | .NET 8.0  | .NET 8.0  |  28.2644 ns | 0.1056 ns | 0.0882 ns |   159.91 |    1.87 |      - |         - |          NA |
| PrimitiveEmail_Create          | .NET 8.0  | .NET 8.0  | 233.3725 ns | 0.6190 ns | 0.5790 ns | 1,320.33 |   15.23 |      - |         - |          NA |
| PrimitiveEmail_JsonSerialize   | .NET 8.0  | .NET 8.0  | 415.8780 ns | 0.6508 ns | 0.6088 ns | 2,352.87 |   26.76 | 0.0038 |      64 B |          NA |
| PrimitiveEmail_JsonDeserialize | .NET 8.0  | .NET 8.0  | 430.1694 ns | 0.5685 ns | 0.4747 ns | 2,433.72 |   27.59 | 0.0072 |     120 B |          NA |
| RawGuid                        | .NET 9.0  | .NET 9.0  |   0.3125 ns | 0.0015 ns | 0.0014 ns |     1.77 |    0.02 |      - |         - |          NA |
| PrimitiveGuid                  | .NET 9.0  | .NET 9.0  |   0.8766 ns | 0.0013 ns | 0.0011 ns |     4.96 |    0.06 |      - |         - |          NA |
| PrimitiveGuid_TryParse         | .NET 9.0  | .NET 9.0  |  28.2489 ns | 0.0893 ns | 0.0792 ns |   159.82 |    1.85 |      - |         - |          NA |
| PrimitiveEmail_Create          | .NET 9.0  | .NET 9.0  | 121.9847 ns | 0.5314 ns | 0.4711 ns |   690.14 |    8.20 |      - |         - |          NA |
| PrimitiveEmail_JsonSerialize   | .NET 9.0  | .NET 9.0  | 253.6428 ns | 0.3226 ns | 0.2860 ns | 1,435.01 |   16.27 | 0.0038 |      64 B |          NA |
| PrimitiveEmail_JsonDeserialize | .NET 9.0  | .NET 9.0  | 259.5744 ns | 0.6393 ns | 0.5668 ns | 1,468.57 |   16.86 | 0.0072 |     120 B |          NA |
