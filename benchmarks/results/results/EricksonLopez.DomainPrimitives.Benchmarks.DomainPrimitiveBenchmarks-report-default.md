
BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.401
  [Host]    : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v3
  .NET 8.0  : .NET 8.0.31 (8.0.31, 8.0.3126.42015), X64 RyuJIT x86-64-v3
  .NET 9.0  : .NET 9.0.20 (9.0.20, 9.0.2026.41315), X64 RyuJIT x86-64-v3


 Method                         | Job       | Runtime   | Mean        | Error     | StdDev    | Ratio    | RatioSD | Gen0   | Allocated | Alloc Ratio |
------------------------------- |---------- |---------- |------------:|----------:|----------:|---------:|--------:|-------:|----------:|------------:|
 RawGuid                        | .NET 10.0 | .NET 10.0 |   0.3144 ns | 0.0014 ns | 0.0013 ns |     0.97 |    0.03 |      - |         - |          NA |
 PrimitiveGuid                  | .NET 10.0 | .NET 10.0 |   0.8883 ns | 0.0139 ns | 0.0130 ns |     2.74 |    0.09 |      - |         - |          NA |
 PrimitiveGuid_TryParse         | .NET 10.0 | .NET 10.0 |  31.6576 ns | 0.1824 ns | 0.1617 ns |    97.82 |    3.12 |      - |         - |          NA |
 PrimitiveEmail_Create          | .NET 10.0 | .NET 10.0 | 110.6973 ns | 0.3320 ns | 0.2592 ns |   342.06 |   10.79 |      - |         - |          NA |
 PrimitiveEmail_JsonSerialize   | .NET 10.0 | .NET 10.0 | 224.5843 ns | 0.9490 ns | 0.7925 ns |   693.97 |   21.96 | 0.0038 |      64 B |          NA |
 PrimitiveEmail_JsonDeserialize | .NET 10.0 | .NET 10.0 | 215.3248 ns | 1.1278 ns | 0.9998 ns |   665.36 |   21.14 | 0.0072 |     120 B |          NA |
 RawGuid                        | .NET 8.0  | .NET 8.0  |   0.3239 ns | 0.0111 ns | 0.0104 ns |     1.00 |    0.04 |      - |         - |          NA |
 PrimitiveGuid                  | .NET 8.0  | .NET 8.0  |   0.8823 ns | 0.0074 ns | 0.0065 ns |     2.73 |    0.09 |      - |         - |          NA |
 PrimitiveGuid_TryParse         | .NET 8.0  | .NET 8.0  |  35.9053 ns | 0.3970 ns | 0.3714 ns |   110.95 |    3.66 |      - |         - |          NA |
 PrimitiveEmail_Create          | .NET 8.0  | .NET 8.0  | 243.5859 ns | 1.5480 ns | 1.3723 ns |   752.68 |   24.03 |      - |         - |          NA |
 PrimitiveEmail_JsonSerialize   | .NET 8.0  | .NET 8.0  | 405.1888 ns | 2.7075 ns | 2.5326 ns | 1,252.04 |   40.10 | 0.0038 |      64 B |          NA |
 PrimitiveEmail_JsonDeserialize | .NET 8.0  | .NET 8.0  | 412.7487 ns | 0.3665 ns | 0.3060 ns | 1,275.40 |   40.14 | 0.0072 |     120 B |          NA |
 RawGuid                        | .NET 9.0  | .NET 9.0  |   0.1142 ns | 0.0081 ns | 0.0075 ns |     0.35 |    0.03 |      - |         - |          NA |
 PrimitiveGuid                  | .NET 9.0  | .NET 9.0  |   0.8520 ns | 0.0069 ns | 0.0058 ns |     2.63 |    0.08 |      - |         - |          NA |
 PrimitiveGuid_TryParse         | .NET 9.0  | .NET 9.0  |  33.5394 ns | 0.2656 ns | 0.2484 ns |   103.64 |    3.34 |      - |         - |          NA |
 PrimitiveEmail_Create          | .NET 9.0  | .NET 9.0  | 119.2990 ns | 0.5995 ns | 0.5607 ns |   368.64 |   11.72 |      - |         - |          NA |
 PrimitiveEmail_JsonSerialize   | .NET 9.0  | .NET 9.0  | 283.8833 ns | 1.7314 ns | 1.6195 ns |   877.20 |   28.01 | 0.0038 |      64 B |          NA |
 PrimitiveEmail_JsonDeserialize | .NET 9.0  | .NET 9.0  | 256.8344 ns | 1.2895 ns | 1.1431 ns |   793.62 |   25.20 | 0.0072 |     120 B |          NA |
