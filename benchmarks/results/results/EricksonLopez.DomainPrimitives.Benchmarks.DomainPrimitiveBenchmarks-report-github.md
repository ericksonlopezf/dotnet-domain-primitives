```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.400
  [Host]    : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  .NET 8.0  : .NET 8.0.30 (8.0.30, 8.0.3026.36720), X64 RyuJIT x86-64-v3
  .NET 9.0  : .NET 9.0.19 (9.0.19, 9.0.1926.36724), X64 RyuJIT x86-64-v3


```
| Method                         | Job       | Runtime   | Mean        | Error     | StdDev    | Ratio    | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------------------------- |---------- |---------- |------------:|----------:|----------:|---------:|--------:|-------:|----------:|------------:|
| RawGuid                        | .NET 10.0 | .NET 10.0 |   0.3136 ns | 0.0015 ns | 0.0013 ns |     4.70 |    0.29 |      - |         - |          NA |
| PrimitiveGuid                  | .NET 10.0 | .NET 10.0 |   0.8590 ns | 0.0007 ns | 0.0006 ns |    12.86 |    0.79 |      - |         - |          NA |
| PrimitiveGuid_TryParse         | .NET 10.0 | .NET 10.0 |  31.7030 ns | 0.1345 ns | 0.1193 ns |   474.75 |   29.29 |      - |         - |          NA |
| PrimitiveEmail_Create          | .NET 10.0 | .NET 10.0 | 109.6696 ns | 0.0708 ns | 0.0591 ns | 1,642.28 |  101.18 |      - |         - |          NA |
| PrimitiveEmail_JsonSerialize   | .NET 10.0 | .NET 10.0 | 230.3407 ns | 0.4668 ns | 0.4138 ns | 3,449.31 |  212.55 | 0.0038 |      64 B |          NA |
| PrimitiveEmail_JsonDeserialize | .NET 10.0 | .NET 10.0 | 209.5667 ns | 0.4347 ns | 0.4066 ns | 3,138.22 |  193.36 | 0.0072 |     120 B |          NA |
| RawGuid                        | .NET 8.0  | .NET 8.0  |   0.0670 ns | 0.0047 ns | 0.0042 ns |     1.00 |    0.09 |      - |         - |          NA |
| PrimitiveGuid                  | .NET 8.0  | .NET 8.0  |   0.8789 ns | 0.0011 ns | 0.0010 ns |    13.16 |    0.81 |      - |         - |          NA |
| PrimitiveGuid_TryParse         | .NET 8.0  | .NET 8.0  |  35.1141 ns | 0.1474 ns | 0.1378 ns |   525.83 |   32.44 |      - |         - |          NA |
| PrimitiveEmail_Create          | .NET 8.0  | .NET 8.0  | 255.4106 ns | 0.6286 ns | 0.5880 ns | 3,824.72 |  235.70 |      - |         - |          NA |
| PrimitiveEmail_JsonSerialize   | .NET 8.0  | .NET 8.0  | 408.7723 ns | 0.5238 ns | 0.4643 ns | 6,121.28 |  377.10 | 0.0038 |      64 B |          NA |
| PrimitiveEmail_JsonDeserialize | .NET 8.0  | .NET 8.0  | 419.7210 ns | 0.7683 ns | 0.7187 ns | 6,285.24 |  387.22 | 0.0072 |     120 B |          NA |
| RawGuid                        | .NET 9.0  | .NET 9.0  |   0.3141 ns | 0.0025 ns | 0.0021 ns |     4.70 |    0.29 |      - |         - |          NA |
| PrimitiveGuid                  | .NET 9.0  | .NET 9.0  |   0.8801 ns | 0.0024 ns | 0.0020 ns |    13.18 |    0.81 |      - |         - |          NA |
| PrimitiveGuid_TryParse         | .NET 9.0  | .NET 9.0  |  32.4293 ns | 0.1590 ns | 0.1487 ns |   485.62 |   29.99 |      - |         - |          NA |
| PrimitiveEmail_Create          | .NET 9.0  | .NET 9.0  | 116.0498 ns | 0.1300 ns | 0.1085 ns | 1,737.82 |  107.07 |      - |         - |          NA |
| PrimitiveEmail_JsonSerialize   | .NET 9.0  | .NET 9.0  | 276.5058 ns | 0.3709 ns | 0.3288 ns | 4,140.62 |  255.09 | 0.0038 |      64 B |          NA |
| PrimitiveEmail_JsonDeserialize | .NET 9.0  | .NET 9.0  | 243.2555 ns | 0.5136 ns | 0.4553 ns | 3,642.70 |  224.47 | 0.0072 |     120 B |          NA |
