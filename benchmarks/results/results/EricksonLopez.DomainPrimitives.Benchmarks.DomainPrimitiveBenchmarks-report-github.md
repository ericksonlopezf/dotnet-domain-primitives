```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.400
  [Host]    : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  .NET 8.0  : .NET 8.0.30 (8.0.30, 8.0.3026.36720), X64 RyuJIT x86-64-v3
  .NET 9.0  : .NET 9.0.19 (9.0.19, 9.0.1926.36724), X64 RyuJIT x86-64-v3


```
| Method                         | Job       | Runtime   | Mean        | Error     | StdDev    | Ratio    | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------------------------- |---------- |---------- |------------:|----------:|----------:|---------:|--------:|-------:|----------:|------------:|
| RawGuid                        | .NET 10.0 | .NET 10.0 |   0.3743 ns | 0.0012 ns | 0.0010 ns |     1.06 |    0.00 |      - |         - |          NA |
| PrimitiveGuid                  | .NET 10.0 | .NET 10.0 |   1.3192 ns | 0.0013 ns | 0.0011 ns |     3.74 |    0.01 |      - |         - |          NA |
| PrimitiveGuid_TryParse         | .NET 10.0 | .NET 10.0 |  29.3916 ns | 0.0279 ns | 0.0248 ns |    83.40 |    0.13 |      - |         - |          NA |
| PrimitiveEmail_Create          | .NET 10.0 | .NET 10.0 | 119.1740 ns | 0.1732 ns | 0.1447 ns |   338.14 |    0.60 |      - |         - |          NA |
| PrimitiveEmail_JsonSerialize   | .NET 10.0 | .NET 10.0 | 228.0781 ns | 0.1506 ns | 0.1258 ns |   647.15 |    0.94 | 0.0038 |      64 B |          NA |
| PrimitiveEmail_JsonDeserialize | .NET 10.0 | .NET 10.0 | 222.5952 ns | 0.2883 ns | 0.2556 ns |   631.59 |    1.10 | 0.0072 |     120 B |          NA |
| RawGuid                        | .NET 8.0  | .NET 8.0  |   0.3524 ns | 0.0006 ns | 0.0005 ns |     1.00 |    0.00 |      - |         - |          NA |
| PrimitiveGuid                  | .NET 8.0  | .NET 8.0  |   0.9694 ns | 0.0028 ns | 0.0025 ns |     2.75 |    0.01 |      - |         - |          NA |
| PrimitiveGuid_TryParse         | .NET 8.0  | .NET 8.0  |  35.1576 ns | 0.0298 ns | 0.0264 ns |    99.76 |    0.15 |      - |         - |          NA |
| PrimitiveEmail_Create          | .NET 8.0  | .NET 8.0  | 232.1980 ns | 0.1168 ns | 0.1035 ns |   658.84 |    0.93 |      - |         - |          NA |
| PrimitiveEmail_JsonSerialize   | .NET 8.0  | .NET 8.0  | 380.0364 ns | 0.2871 ns | 0.2545 ns | 1,078.32 |    1.61 | 0.0038 |      64 B |          NA |
| PrimitiveEmail_JsonDeserialize | .NET 8.0  | .NET 8.0  | 387.8222 ns | 0.2756 ns | 0.2578 ns | 1,100.41 |    1.64 | 0.0072 |     120 B |          NA |
| RawGuid                        | .NET 9.0  | .NET 9.0  |   0.0719 ns | 0.0032 ns | 0.0028 ns |     0.20 |    0.01 |      - |         - |          NA |
| PrimitiveGuid                  | .NET 9.0  | .NET 9.0  |   0.9669 ns | 0.0010 ns | 0.0008 ns |     2.74 |    0.00 |      - |         - |          NA |
| PrimitiveGuid_TryParse         | .NET 9.0  | .NET 9.0  |  30.5752 ns | 0.0304 ns | 0.0269 ns |    86.75 |    0.14 |      - |         - |          NA |
| PrimitiveEmail_Create          | .NET 9.0  | .NET 9.0  | 127.3472 ns | 0.2654 ns | 0.2352 ns |   361.34 |    0.81 |      - |         - |          NA |
| PrimitiveEmail_JsonSerialize   | .NET 9.0  | .NET 9.0  | 248.0231 ns | 0.2065 ns | 0.1724 ns |   703.74 |    1.06 | 0.0038 |      64 B |          NA |
| PrimitiveEmail_JsonDeserialize | .NET 9.0  | .NET 9.0  | 242.0594 ns | 0.6390 ns | 0.5977 ns |   686.82 |    1.88 | 0.0072 |     120 B |          NA |
