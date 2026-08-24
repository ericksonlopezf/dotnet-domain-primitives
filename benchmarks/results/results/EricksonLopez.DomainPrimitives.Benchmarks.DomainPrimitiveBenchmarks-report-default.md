
BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
INTEL XEON PLATINUM 8573C 2.30GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.400
  [Host]    : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v4
  .NET 8.0  : .NET 8.0.30 (8.0.30, 8.0.3026.36720), X64 RyuJIT x86-64-v4
  .NET 9.0  : .NET 9.0.19 (9.0.19, 9.0.1926.36724), X64 RyuJIT x86-64-v4


 Method                         | Job       | Runtime   | Mean        | Error     | StdDev    | Median      | Ratio     | RatioSD | Gen0   | Allocated | Alloc Ratio |
------------------------------- |---------- |---------- |------------:|----------:|----------:|------------:|----------:|--------:|-------:|----------:|------------:|
 RawGuid                        | .NET 10.0 | .NET 10.0 |   0.0023 ns | 0.0088 ns | 0.0098 ns |   0.0000 ns |     0.008 |    0.04 |      - |         - |          NA |
 PrimitiveGuid                  | .NET 10.0 | .NET 10.0 |   0.3186 ns | 0.0486 ns | 0.0477 ns |   0.3188 ns |     1.161 |    0.22 |      - |         - |          NA |
 PrimitiveGuid_TryParse         | .NET 10.0 | .NET 10.0 |  20.2692 ns | 0.4170 ns | 0.3900 ns |  20.1322 ns |    73.841 |    8.85 |      - |         - |          NA |
 PrimitiveEmail_Create          | .NET 10.0 | .NET 10.0 |  81.6516 ns | 1.3379 ns | 1.2514 ns |  81.7315 ns |   297.457 |   35.49 |      - |         - |          NA |
 PrimitiveEmail_JsonSerialize   | .NET 10.0 | .NET 10.0 | 171.2978 ns | 3.1486 ns | 2.9452 ns | 171.5351 ns |   624.038 |   74.60 | 0.0007 |      64 B |          NA |
 PrimitiveEmail_JsonDeserialize | .NET 10.0 | .NET 10.0 | 176.1892 ns | 3.3448 ns | 3.2850 ns | 176.0915 ns |   641.857 |   76.85 | 0.0014 |     120 B |          NA |
 RawGuid                        | .NET 8.0  | .NET 8.0  |   0.2783 ns | 0.0355 ns | 0.0332 ns |   0.2848 ns |     1.014 |    0.17 |      - |         - |          NA |
 PrimitiveGuid                  | .NET 8.0  | .NET 8.0  |   0.2823 ns | 0.0363 ns | 0.0340 ns |   0.2761 ns |     1.029 |    0.17 |      - |         - |          NA |
 PrimitiveGuid_TryParse         | .NET 8.0  | .NET 8.0  |  26.0644 ns | 0.3806 ns | 0.3560 ns |  26.2169 ns |    94.953 |   11.31 |      - |         - |          NA |
 PrimitiveEmail_Create          | .NET 8.0  | .NET 8.0  | 179.5954 ns | 3.1544 ns | 3.8739 ns | 179.3279 ns |   654.266 |   78.62 |      - |         - |          NA |
 PrimitiveEmail_JsonSerialize   | .NET 8.0  | .NET 8.0  | 287.4184 ns | 3.9239 ns | 3.6704 ns | 288.2607 ns | 1,047.065 |  124.61 | 0.0005 |      64 B |          NA |
 PrimitiveEmail_JsonDeserialize | .NET 8.0  | .NET 8.0  | 328.5293 ns | 6.3430 ns | 6.7870 ns | 328.2909 ns | 1,196.832 |  143.66 | 0.0014 |     120 B |          NA |
 RawGuid                        | .NET 9.0  | .NET 9.0  |   0.0027 ns | 0.0079 ns | 0.0077 ns |   0.0000 ns |     0.010 |    0.03 |      - |         - |          NA |
 PrimitiveGuid                  | .NET 9.0  | .NET 9.0  |   0.3116 ns | 0.0497 ns | 0.0510 ns |   0.3063 ns |     1.135 |    0.23 |      - |         - |          NA |
 PrimitiveGuid_TryParse         | .NET 9.0  | .NET 9.0  |  21.4301 ns | 0.4137 ns | 0.3667 ns |  21.4466 ns |    78.070 |    9.33 |      - |         - |          NA |
 PrimitiveEmail_Create          | .NET 9.0  | .NET 9.0  |  87.9859 ns | 1.8123 ns | 1.6065 ns |  87.5506 ns |   320.533 |   38.37 |      - |         - |          NA |
 PrimitiveEmail_JsonSerialize   | .NET 9.0  | .NET 9.0  | 188.6612 ns | 3.7038 ns | 3.6376 ns | 188.0449 ns |   687.293 |   82.36 | 0.0007 |      64 B |          NA |
 PrimitiveEmail_JsonDeserialize | .NET 9.0  | .NET 9.0  | 207.5604 ns | 4.1122 ns | 3.8465 ns | 208.3953 ns |   756.143 |   90.53 | 0.0014 |     120 B |          NA |
