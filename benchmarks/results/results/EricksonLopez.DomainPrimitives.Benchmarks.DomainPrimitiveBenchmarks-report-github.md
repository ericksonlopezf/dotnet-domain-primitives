```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.401
  [Host]    : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v3
  .NET 8.0  : .NET 8.0.31 (8.0.31, 8.0.3126.42015), X64 RyuJIT x86-64-v3
  .NET 9.0  : .NET 9.0.20 (9.0.20, 9.0.2026.41315), X64 RyuJIT x86-64-v3


```
| Method                         | Job       | Runtime   | Mean        | Error     | StdDev    | Ratio    | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------------------------- |---------- |---------- |------------:|----------:|----------:|---------:|--------:|-------:|----------:|------------:|
| RawGuid                        | .NET 10.0 | .NET 10.0 |   0.3745 ns | 0.0007 ns | 0.0006 ns |     1.06 |    0.00 |      - |         - |          NA |
| PrimitiveGuid                  | .NET 10.0 | .NET 10.0 |   0.9673 ns | 0.0010 ns | 0.0008 ns |     2.74 |    0.01 |      - |         - |          NA |
| PrimitiveGuid_TryParse         | .NET 10.0 | .NET 10.0 |  29.1964 ns | 0.0555 ns | 0.0463 ns |    82.72 |    0.22 |      - |         - |          NA |
| PrimitiveEmail_Create          | .NET 10.0 | .NET 10.0 | 118.8642 ns | 0.0648 ns | 0.0606 ns |   336.76 |    0.75 |      - |         - |          NA |
| PrimitiveEmail_JsonSerialize   | .NET 10.0 | .NET 10.0 | 222.8771 ns | 0.2140 ns | 0.1897 ns |   631.45 |    1.46 | 0.0038 |      64 B |          NA |
| PrimitiveEmail_JsonDeserialize | .NET 10.0 | .NET 10.0 | 218.6638 ns | 0.5385 ns | 0.4773 ns |   619.51 |    1.87 | 0.0072 |     120 B |          NA |
| RawGuid                        | .NET 8.0  | .NET 8.0  |   0.3530 ns | 0.0008 ns | 0.0008 ns |     1.00 |    0.00 |      - |         - |          NA |
| PrimitiveGuid                  | .NET 8.0  | .NET 8.0  |   0.9689 ns | 0.0018 ns | 0.0016 ns |     2.75 |    0.01 |      - |         - |          NA |
| PrimitiveGuid_TryParse         | .NET 8.0  | .NET 8.0  |  35.4624 ns | 0.0482 ns | 0.0427 ns |   100.47 |    0.25 |      - |         - |          NA |
| PrimitiveEmail_Create          | .NET 8.0  | .NET 8.0  | 231.8991 ns | 0.0920 ns | 0.0718 ns |   657.01 |    1.44 |      - |         - |          NA |
| PrimitiveEmail_JsonSerialize   | .NET 8.0  | .NET 8.0  | 383.9389 ns | 0.5107 ns | 0.4527 ns | 1,087.76 |    2.66 | 0.0038 |      64 B |          NA |
| PrimitiveEmail_JsonDeserialize | .NET 8.0  | .NET 8.0  | 388.1881 ns | 0.4674 ns | 0.4143 ns | 1,099.80 |    2.64 | 0.0072 |     120 B |          NA |
| RawGuid                        | .NET 9.0  | .NET 9.0  |   0.0237 ns | 0.0009 ns | 0.0008 ns |     0.07 |    0.00 |      - |         - |          NA |
| PrimitiveGuid                  | .NET 9.0  | .NET 9.0  |   0.9684 ns | 0.0012 ns | 0.0011 ns |     2.74 |    0.01 |      - |         - |          NA |
| PrimitiveGuid_TryParse         | .NET 9.0  | .NET 9.0  |  30.3195 ns | 0.0649 ns | 0.0607 ns |    85.90 |    0.25 |      - |         - |          NA |
| PrimitiveEmail_Create          | .NET 9.0  | .NET 9.0  | 126.4010 ns | 0.0803 ns | 0.0671 ns |   358.11 |    0.80 |      - |         - |          NA |
| PrimitiveEmail_JsonSerialize   | .NET 9.0  | .NET 9.0  | 254.2953 ns | 0.5144 ns | 0.4811 ns |   720.46 |    2.04 | 0.0038 |      64 B |          NA |
| PrimitiveEmail_JsonDeserialize | .NET 9.0  | .NET 9.0  | 244.2091 ns | 0.1822 ns | 0.1615 ns |   691.88 |    1.56 | 0.0072 |     120 B |          NA |
