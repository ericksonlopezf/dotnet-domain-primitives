```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 7763 3.14GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.401
  [Host]    : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v3
  .NET 8.0  : .NET 8.0.31 (8.0.31, 8.0.3126.42015), X64 RyuJIT x86-64-v3
  .NET 9.0  : .NET 9.0.20 (9.0.20, 9.0.2026.41315), X64 RyuJIT x86-64-v3


```
| Method                         | Job       | Runtime   | Mean        | Error     | StdDev    | Ratio     | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------------------------- |---------- |---------- |------------:|----------:|----------:|----------:|--------:|-------:|----------:|------------:|
| RawGuid                        | .NET 10.0 | .NET 10.0 |   0.3155 ns | 0.0030 ns | 0.0027 ns |     10.90 |    0.40 |      - |         - |          NA |
| PrimitiveGuid                  | .NET 10.0 | .NET 10.0 |   0.8794 ns | 0.0008 ns | 0.0007 ns |     30.39 |    1.08 |      - |         - |          NA |
| PrimitiveGuid_TryParse         | .NET 10.0 | .NET 10.0 |  26.7735 ns | 0.0276 ns | 0.0245 ns |    925.07 |   32.87 |      - |         - |          NA |
| PrimitiveEmail_Create          | .NET 10.0 | .NET 10.0 | 116.1969 ns | 0.4481 ns | 0.3741 ns |  4,014.80 |  143.20 |      - |         - |          NA |
| PrimitiveEmail_JsonSerialize   | .NET 10.0 | .NET 10.0 | 228.7931 ns | 0.6433 ns | 0.5703 ns |  7,905.19 |  281.48 | 0.0038 |      64 B |          NA |
| PrimitiveEmail_JsonDeserialize | .NET 10.0 | .NET 10.0 | 210.3196 ns | 1.2537 ns | 1.1727 ns |  7,266.90 |  261.08 | 0.0072 |     120 B |          NA |
| RawGuid                        | .NET 8.0  | .NET 8.0  |   0.0290 ns | 0.0012 ns | 0.0011 ns |      1.00 |    0.05 |      - |         - |          NA |
| PrimitiveGuid                  | .NET 8.0  | .NET 8.0  |   1.1691 ns | 0.0006 ns | 0.0004 ns |     40.39 |    1.44 |      - |         - |          NA |
| PrimitiveGuid_TryParse         | .NET 8.0  | .NET 8.0  |  28.2996 ns | 0.0562 ns | 0.0469 ns |    977.80 |   34.78 |      - |         - |          NA |
| PrimitiveEmail_Create          | .NET 8.0  | .NET 8.0  | 241.4511 ns | 0.2811 ns | 0.2492 ns |  8,342.54 |  296.49 |      - |         - |          NA |
| PrimitiveEmail_JsonSerialize   | .NET 8.0  | .NET 8.0  | 406.4846 ns | 0.1718 ns | 0.1341 ns | 14,044.73 |  499.17 | 0.0038 |      64 B |          NA |
| PrimitiveEmail_JsonDeserialize | .NET 8.0  | .NET 8.0  | 426.7878 ns | 0.9463 ns | 0.8851 ns | 14,746.24 |  524.61 | 0.0072 |     120 B |          NA |
| RawGuid                        | .NET 9.0  | .NET 9.0  |   0.3133 ns | 0.0012 ns | 0.0010 ns |     10.82 |    0.39 |      - |         - |          NA |
| PrimitiveGuid                  | .NET 9.0  | .NET 9.0  |   1.1632 ns | 0.0020 ns | 0.0017 ns |     40.19 |    1.43 |      - |         - |          NA |
| PrimitiveGuid_TryParse         | .NET 9.0  | .NET 9.0  |  28.1631 ns | 0.1012 ns | 0.0946 ns |    973.08 |   34.71 |      - |         - |          NA |
| PrimitiveEmail_Create          | .NET 9.0  | .NET 9.0  | 125.2067 ns | 0.2167 ns | 0.1921 ns |  4,326.10 |  153.82 |      - |         - |          NA |
| PrimitiveEmail_JsonSerialize   | .NET 9.0  | .NET 9.0  | 259.1299 ns | 0.2169 ns | 0.1922 ns |  8,953.37 |  318.13 | 0.0038 |      64 B |          NA |
| PrimitiveEmail_JsonDeserialize | .NET 9.0  | .NET 9.0  | 259.4831 ns | 0.3441 ns | 0.2686 ns |  8,965.58 |  318.76 | 0.0072 |     120 B |          NA |
