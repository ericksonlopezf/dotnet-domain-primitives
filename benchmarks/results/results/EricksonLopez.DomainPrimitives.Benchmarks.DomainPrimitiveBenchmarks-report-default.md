
BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.400
  [Host]    : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  .NET 8.0  : .NET 8.0.30 (8.0.30, 8.0.3026.36720), X64 RyuJIT x86-64-v3
  .NET 9.0  : .NET 9.0.19 (9.0.19, 9.0.1926.36724), X64 RyuJIT x86-64-v3


 Method                         | Job       | Runtime   | Mean        | Error     | StdDev    | Ratio    | RatioSD | Gen0   | Allocated | Alloc Ratio |
------------------------------- |---------- |---------- |------------:|----------:|----------:|---------:|--------:|-------:|----------:|------------:|
 RawGuid                        | .NET 10.0 | .NET 10.0 |   0.3754 ns | 0.0008 ns | 0.0007 ns |     1.06 |    0.00 |      - |         - |          NA |
 PrimitiveGuid                  | .NET 10.0 | .NET 10.0 |   0.9683 ns | 0.0010 ns | 0.0008 ns |     2.74 |    0.00 |      - |         - |          NA |
 PrimitiveGuid_TryParse         | .NET 10.0 | .NET 10.0 |  29.5231 ns | 0.0989 ns | 0.0877 ns |    83.68 |    0.26 |      - |         - |          NA |
 PrimitiveEmail_Create          | .NET 10.0 | .NET 10.0 | 117.5161 ns | 0.1464 ns | 0.1298 ns |   333.09 |    0.55 |      - |         - |          NA |
 PrimitiveEmail_JsonSerialize   | .NET 10.0 | .NET 10.0 | 226.2902 ns | 0.2583 ns | 0.2289 ns |   641.41 |    1.02 | 0.0038 |      64 B |          NA |
 PrimitiveEmail_JsonDeserialize | .NET 10.0 | .NET 10.0 | 240.9266 ns | 0.2418 ns | 0.2144 ns |   682.90 |    1.03 | 0.0072 |     120 B |          NA |
 RawGuid                        | .NET 8.0  | .NET 8.0  |   0.3528 ns | 0.0006 ns | 0.0005 ns |     1.00 |    0.00 |      - |         - |          NA |
 PrimitiveGuid                  | .NET 8.0  | .NET 8.0  |   1.3200 ns | 0.0015 ns | 0.0013 ns |     3.74 |    0.01 |      - |         - |          NA |
 PrimitiveGuid_TryParse         | .NET 8.0  | .NET 8.0  |  35.1199 ns | 0.0883 ns | 0.0738 ns |    99.55 |    0.24 |      - |         - |          NA |
 PrimitiveEmail_Create          | .NET 8.0  | .NET 8.0  | 231.4505 ns | 0.1401 ns | 0.1242 ns |   656.04 |    0.89 |      - |         - |          NA |
 PrimitiveEmail_JsonSerialize   | .NET 8.0  | .NET 8.0  | 380.7493 ns | 0.3129 ns | 0.2774 ns | 1,079.22 |    1.55 | 0.0038 |      64 B |          NA |
 PrimitiveEmail_JsonDeserialize | .NET 8.0  | .NET 8.0  | 396.2637 ns | 0.5022 ns | 0.4452 ns | 1,123.19 |    1.86 | 0.0072 |     120 B |          NA |
 RawGuid                        | .NET 9.0  | .NET 9.0  |   0.0734 ns | 0.0066 ns | 0.0059 ns |     0.21 |    0.02 |      - |         - |          NA |
 PrimitiveGuid                  | .NET 9.0  | .NET 9.0  |   0.9681 ns | 0.0010 ns | 0.0008 ns |     2.74 |    0.00 |      - |         - |          NA |
 PrimitiveGuid_TryParse         | .NET 9.0  | .NET 9.0  |  31.0363 ns | 0.0437 ns | 0.0409 ns |    87.97 |    0.16 |      - |         - |          NA |
 PrimitiveEmail_Create          | .NET 9.0  | .NET 9.0  | 122.5614 ns | 0.2286 ns | 0.1785 ns |   347.40 |    0.65 |      - |         - |          NA |
 PrimitiveEmail_JsonSerialize   | .NET 9.0  | .NET 9.0  | 250.7668 ns | 0.3290 ns | 0.2747 ns |   710.79 |    1.16 | 0.0038 |      64 B |          NA |
 PrimitiveEmail_JsonDeserialize | .NET 9.0  | .NET 9.0  | 246.5670 ns | 0.1512 ns | 0.1263 ns |   698.88 |    0.94 | 0.0072 |     120 B |          NA |
