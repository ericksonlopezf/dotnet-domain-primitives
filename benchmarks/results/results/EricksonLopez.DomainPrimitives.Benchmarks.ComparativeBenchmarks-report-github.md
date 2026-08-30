```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.400
  [Host]    : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  .NET 8.0  : .NET 8.0.30 (8.0.30, 8.0.3026.36720), X64 RyuJIT x86-64-v3
  .NET 9.0  : .NET 9.0.19 (9.0.19, 9.0.1926.36724), X64 RyuJIT x86-64-v3


```
| Method                                    | Job       | Runtime   | Mean        | Error     | StdDev    | Median      | Ratio      | RatioSD | Rank | Gen0   | Allocated | Alloc Ratio |
|------------------------------------------ |---------- |---------- |------------:|----------:|----------:|------------:|-----------:|--------:|-----:|-------:|----------:|------------:|
| RawGuid_Create                            | .NET 10.0 | .NET 10.0 |   0.1118 ns | 0.0024 ns | 0.0021 ns |   0.1117 ns |      4.835 |    0.17 |    9 |      - |         - |          NA |
| DomainPrimitives_Create                   | .NET 10.0 | .NET 10.0 |   0.9470 ns | 0.0016 ns | 0.0014 ns |   0.9469 ns |     40.956 |    1.19 |   13 |      - |         - |          NA |
| Vogen_Create                              | .NET 10.0 | .NET 10.0 |   0.3548 ns | 0.0024 ns | 0.0022 ns |   0.3544 ns |     15.346 |    0.46 |   10 |      - |         - |          NA |
| StronglyTypedId_Create                    | .NET 10.0 | .NET 10.0 |   0.0024 ns | 0.0018 ns | 0.0017 ns |   0.0020 ns |      0.102 |    0.07 |    4 |      - |         - |          NA |
| ValueOf_Create                            | .NET 10.0 | .NET 10.0 |   9.2919 ns | 0.1261 ns | 0.1180 ns |   9.3210 ns |    401.845 |   12.67 |   28 | 0.0019 |      32 B |          NA |
| Meziantou_Create                          | .NET 10.0 | .NET 10.0 |   0.3524 ns | 0.0014 ns | 0.0011 ns |   0.3526 ns |     15.238 |    0.44 |   10 |      - |         - |          NA |
| TinyTypes_Create                          | .NET 10.0 | .NET 10.0 |   0.3532 ns | 0.0013 ns | 0.0012 ns |   0.3536 ns |     15.276 |    0.45 |   10 |      - |         - |          NA |
| RawGuid_Parse                             | .NET 10.0 | .NET 10.0 |  27.2884 ns | 0.0206 ns | 0.0172 ns |  27.2830 ns |  1,180.139 |   34.27 |   35 |      - |         - |          NA |
| DomainPrimitives_Parse                    | .NET 10.0 | .NET 10.0 |  27.4480 ns | 0.0235 ns | 0.0208 ns |  27.4455 ns |  1,187.042 |   34.47 |   35 |      - |         - |          NA |
| Vogen_Parse                               | .NET 10.0 | .NET 10.0 |  27.4756 ns | 0.0387 ns | 0.0362 ns |  27.4759 ns |  1,188.235 |   34.52 |   35 |      - |         - |          NA |
| StronglyTypedId_Parse                     | .NET 10.0 | .NET 10.0 |  27.2978 ns | 0.0223 ns | 0.0208 ns |  27.3021 ns |  1,180.545 |   34.27 |   35 |      - |         - |          NA |
| ValueOf_Parse                             | .NET 10.0 | .NET 10.0 |  29.9297 ns | 0.0531 ns | 0.0444 ns |  29.9251 ns |  1,294.364 |   37.63 |   35 | 0.0019 |      32 B |          NA |
| Meziantou_Parse                           | .NET 10.0 | .NET 10.0 |  41.9702 ns | 0.0273 ns | 0.0213 ns |  41.9709 ns |  1,815.080 |   52.72 |   39 |      - |         - |          NA |
| TinyTypes_Parse                           | .NET 10.0 | .NET 10.0 |  27.2925 ns | 0.0311 ns | 0.0276 ns |  27.2912 ns |  1,180.315 |   34.28 |   35 |      - |         - |          NA |
| DomainPrimitives_EqualityCheck            | .NET 10.0 | .NET 10.0 |   0.9557 ns | 0.0030 ns | 0.0025 ns |   0.9568 ns |     41.333 |    1.20 |   13 |      - |         - |          NA |
| RawGuid_ToString                          | .NET 10.0 | .NET 10.0 |   9.5432 ns | 0.1463 ns | 0.1368 ns |   9.5237 ns |    412.715 |   13.28 |   28 | 0.0057 |      96 B |          NA |
| DomainPrimitives_ToString                 | .NET 10.0 | .NET 10.0 |  10.8802 ns | 0.2433 ns | 0.2276 ns |  10.8279 ns |    470.536 |   16.66 |   30 | 0.0057 |      96 B |          NA |
| Vogen_ToString                            | .NET 10.0 | .NET 10.0 |  10.7962 ns | 0.0459 ns | 0.0429 ns |  10.7756 ns |    466.900 |   13.67 |   30 | 0.0057 |      96 B |          NA |
| StronglyTypedId_ToString                  | .NET 10.0 | .NET 10.0 |   9.8643 ns | 0.1198 ns | 0.1121 ns |   9.8242 ns |    426.602 |   13.24 |   28 | 0.0057 |      96 B |          NA |
| ValueOf_ToString                          | .NET 10.0 | .NET 10.0 |  20.8914 ns | 0.2281 ns | 0.2133 ns |  20.9599 ns |    903.488 |   27.70 |   32 | 0.0076 |     128 B |          NA |
| Meziantou_ToString                        | .NET 10.0 | .NET 10.0 |  25.3233 ns | 0.3760 ns | 0.3517 ns |  25.3870 ns |  1,095.155 |   35.03 |   34 | 0.0148 |     248 B |          NA |
| TinyTypes_ToString                        | .NET 10.0 | .NET 10.0 |   7.1441 ns | 0.1872 ns | 0.1751 ns |   7.0573 ns |    308.959 |   11.59 |   25 | 0.0019 |      32 B |          NA |
| DomainPrimitives_TryParse                 | .NET 10.0 | .NET 10.0 |  27.3224 ns | 0.0272 ns | 0.0254 ns |  27.3233 ns |  1,181.607 |   34.31 |   35 |      - |         - |          NA |
| DomainPrimitives_SpanParse                | .NET 10.0 | .NET 10.0 |  26.8684 ns | 0.0285 ns | 0.0266 ns |  26.8719 ns |  1,161.974 |   33.74 |   35 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanParse            | .NET 10.0 | .NET 10.0 |  46.2465 ns | 0.0585 ns | 0.0488 ns |  46.2465 ns |  2,000.016 |   58.10 |   40 | 0.0038 |      64 B |          NA |
| DomainPrimitives_SpanFormat               | .NET 10.0 | .NET 10.0 |   2.9259 ns | 0.0128 ns | 0.0119 ns |   2.9252 ns |    126.535 |    3.71 |   18 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanFormat           | .NET 10.0 | .NET 10.0 |   2.7196 ns | 0.0150 ns | 0.0140 ns |   2.7155 ns |    117.614 |    3.46 |   17 |      - |         - |          NA |
| StringPrimitive_Email_Create              | .NET 10.0 | .NET 10.0 | 171.0846 ns | 0.2476 ns | 0.2316 ns | 171.0302 ns |  7,398.876 |  214.96 |   48 |      - |         - |          NA |
| StringPrimitive_Email_TryParse            | .NET 10.0 | .NET 10.0 | 174.8161 ns | 0.1529 ns | 0.1277 ns | 174.8263 ns |  7,560.251 |  219.57 |   48 |      - |         - |          NA |
| NumericPrimitive_Money_Create             | .NET 10.0 | .NET 10.0 |   4.4645 ns | 0.0014 ns | 0.0013 ns |   4.4650 ns |    193.076 |    5.60 |   20 |      - |         - |          NA |
| NumericPrimitive_Money_Add                | .NET 10.0 | .NET 10.0 |  10.0649 ns | 0.0068 ns | 0.0060 ns |  10.0658 ns |    435.277 |   12.64 |   28 |      - |         - |          NA |
| ValueObject_Create                        | .NET 10.0 | .NET 10.0 |   0.3520 ns | 0.0012 ns | 0.0010 ns |   0.3520 ns |     15.223 |    0.44 |   10 |      - |         - |          NA |
| SmartEnum_FromValue                       | .NET 10.0 | .NET 10.0 |   0.9690 ns | 0.0009 ns | 0.0007 ns |   0.9690 ns |     41.907 |    1.22 |   13 |      - |         - |          NA |
| RawGuid_JsonSerialize                     | .NET 10.0 | .NET 10.0 | 100.9987 ns | 0.1516 ns | 0.1266 ns | 100.9837 ns |  4,367.877 |  126.93 |   42 | 0.0062 |     104 B |          NA |
| RawGuid_JsonDeserialize                   | .NET 10.0 | .NET 10.0 | 111.6364 ns | 0.0801 ns | 0.0749 ns | 111.6472 ns |  4,827.924 |  140.16 |   44 |      - |         - |          NA |
| DomainPrimitives_JsonSerialize            | .NET 10.0 | .NET 10.0 | 135.8819 ns | 0.1556 ns | 0.1300 ns | 135.8882 ns |  5,876.469 |  170.70 |   46 | 0.0062 |     104 B |          NA |
| DomainPrimitives_JsonDeserialize          | .NET 10.0 | .NET 10.0 | 106.6362 ns | 0.0551 ns | 0.0515 ns | 106.6543 ns |  4,611.681 |  133.86 |   43 |      - |         - |          NA |
| RawGuid_Create                            | .NET 8.0  | .NET 8.0  |   0.0231 ns | 0.0008 ns | 0.0007 ns |   0.0230 ns |      1.001 |    0.04 |    6 |      - |         - |          NA |
| DomainPrimitives_Create                   | .NET 8.0  | .NET 8.0  |   0.9714 ns | 0.0029 ns | 0.0027 ns |   0.9706 ns |     42.010 |    1.22 |   13 |      - |         - |          NA |
| Vogen_Create                              | .NET 8.0  | .NET 8.0  |   6.5918 ns | 0.0017 ns | 0.0014 ns |   6.5917 ns |    285.075 |    8.28 |   24 |      - |         - |          NA |
| StronglyTypedId_Create                    | .NET 8.0  | .NET 8.0  |   0.0521 ns | 0.0040 ns | 0.0038 ns |   0.0525 ns |      2.253 |    0.17 |    7 |      - |         - |          NA |
| ValueOf_Create                            | .NET 8.0  | .NET 8.0  |  13.7106 ns | 0.2316 ns | 0.2667 ns |  13.6815 ns |    592.940 |   20.56 |   31 | 0.0019 |      32 B |          NA |
| Meziantou_Create                          | .NET 8.0  | .NET 8.0  |   0.0565 ns | 0.0058 ns | 0.0055 ns |   0.0563 ns |      2.442 |    0.24 |    7 |      - |         - |          NA |
| TinyTypes_Create                          | .NET 8.0  | .NET 8.0  |   0.0600 ns | 0.0020 ns | 0.0018 ns |   0.0604 ns |      2.596 |    0.11 |    7 |      - |         - |          NA |
| RawGuid_Parse                             | .NET 8.0  | .NET 8.0  |  28.9928 ns | 0.0202 ns | 0.0188 ns |  28.9910 ns |  1,253.850 |   36.40 |   35 |      - |         - |          NA |
| DomainPrimitives_Parse                    | .NET 8.0  | .NET 8.0  |  36.0850 ns | 0.0358 ns | 0.0317 ns |  36.0830 ns |  1,560.565 |   45.32 |   38 |      - |         - |          NA |
| Vogen_Parse                               | .NET 8.0  | .NET 8.0  |  31.7760 ns | 0.0417 ns | 0.0348 ns |  31.7714 ns |  1,374.211 |   39.93 |   36 |      - |         - |          NA |
| StronglyTypedId_Parse                     | .NET 8.0  | .NET 8.0  |  28.7861 ns | 0.0260 ns | 0.0243 ns |  28.7875 ns |  1,244.910 |   36.15 |   35 |      - |         - |          NA |
| ValueOf_Parse                             | .NET 8.0  | .NET 8.0  |  36.2376 ns | 0.0322 ns | 0.0269 ns |  36.2453 ns |  1,567.161 |   45.51 |   38 | 0.0019 |      32 B |          NA |
| Meziantou_Parse                           | .NET 8.0  | .NET 8.0  |  29.9692 ns | 0.0542 ns | 0.0480 ns |  29.9557 ns |  1,296.076 |   37.68 |   35 |      - |         - |          NA |
| TinyTypes_Parse                           | .NET 8.0  | .NET 8.0  |  28.7467 ns | 0.0157 ns | 0.0140 ns |  28.7463 ns |  1,243.206 |   36.09 |   35 |      - |         - |          NA |
| DomainPrimitives_EqualityCheck            | .NET 8.0  | .NET 8.0  |   0.9057 ns | 0.0013 ns | 0.0011 ns |   0.9058 ns |     39.169 |    1.14 |   12 |      - |         - |          NA |
| RawGuid_ToString                          | .NET 8.0  | .NET 8.0  |  14.5320 ns | 0.3540 ns | 0.5716 ns |  14.7125 ns |    628.464 |   30.44 |   31 | 0.0057 |      96 B |          NA |
| DomainPrimitives_ToString                 | .NET 8.0  | .NET 8.0  |  13.4680 ns | 0.0975 ns | 0.0912 ns |  13.4796 ns |    582.447 |   17.33 |   31 | 0.0057 |      96 B |          NA |
| Vogen_ToString                            | .NET 8.0  | .NET 8.0  |  27.0584 ns | 0.0327 ns | 0.0290 ns |  27.0521 ns |  1,170.190 |   33.99 |   35 | 0.0057 |      96 B |          NA |
| StronglyTypedId_ToString                  | .NET 8.0  | .NET 8.0  |  14.3247 ns | 0.1785 ns | 0.1670 ns |  14.3966 ns |    619.500 |   19.29 |   31 | 0.0057 |      96 B |          NA |
| ValueOf_ToString                          | .NET 8.0  | .NET 8.0  |  23.3317 ns | 0.4020 ns | 0.3760 ns |  23.1552 ns |  1,009.023 |   33.25 |   33 | 0.0076 |     128 B |          NA |
| Meziantou_ToString                        | .NET 8.0  | .NET 8.0  |  34.2735 ns | 0.1352 ns | 0.1129 ns |  34.2583 ns |  1,482.222 |   43.29 |   37 | 0.0148 |     248 B |          NA |
| TinyTypes_ToString                        | .NET 8.0  | .NET 8.0  |  11.7395 ns | 0.2123 ns | 0.1986 ns |  11.8253 ns |    507.697 |   16.92 |   30 | 0.0019 |      32 B |          NA |
| DomainPrimitives_TryParse                 | .NET 8.0  | .NET 8.0  |  28.0367 ns | 0.0167 ns | 0.0148 ns |  28.0311 ns |  1,212.499 |   35.20 |   35 |      - |         - |          NA |
| DomainPrimitives_SpanParse                | .NET 8.0  | .NET 8.0  |  28.6558 ns | 0.0224 ns | 0.0199 ns |  28.6511 ns |  1,239.275 |   35.98 |   35 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanParse            | .NET 8.0  | .NET 8.0  |  60.4107 ns | 0.0463 ns | 0.0387 ns |  60.4022 ns |  2,612.574 |   75.87 |   41 | 0.0038 |      64 B |          NA |
| DomainPrimitives_SpanFormat               | .NET 8.0  | .NET 8.0  |   5.4809 ns | 0.0040 ns | 0.0031 ns |   5.4808 ns |    237.032 |    6.88 |   23 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanFormat           | .NET 8.0  | .NET 8.0  |   4.4242 ns | 0.0025 ns | 0.0021 ns |   4.4238 ns |    191.335 |    5.56 |   20 |      - |         - |          NA |
| StringPrimitive_Email_Create              | .NET 8.0  | .NET 8.0  | 327.9662 ns | 0.1670 ns | 0.1304 ns | 327.9939 ns | 14,183.514 |  411.93 |   50 |      - |         - |          NA |
| StringPrimitive_Email_TryParse            | .NET 8.0  | .NET 8.0  | 342.6830 ns | 0.7305 ns | 0.6476 ns | 342.8765 ns | 14,819.968 |  431.05 |   51 |      - |         - |          NA |
| NumericPrimitive_Money_Create             | .NET 8.0  | .NET 8.0  |   4.8402 ns | 0.0055 ns | 0.0049 ns |   4.8384 ns |    209.324 |    6.08 |   21 |      - |         - |          NA |
| NumericPrimitive_Money_Add                | .NET 8.0  | .NET 8.0  |  10.2945 ns | 0.0044 ns | 0.0041 ns |  10.2944 ns |    445.204 |   12.92 |   29 |      - |         - |          NA |
| ValueObject_Create                        | .NET 8.0  | .NET 8.0  |   0.3738 ns | 0.0007 ns | 0.0006 ns |   0.3737 ns |     16.164 |    0.47 |   11 |      - |         - |          NA |
| SmartEnum_FromValue                       | .NET 8.0  | .NET 8.0  |  15.5849 ns | 0.3154 ns | 0.2950 ns |  15.5813 ns |    673.999 |   23.14 |   31 | 0.0019 |      32 B |          NA |
| RawGuid_JsonSerialize                     | .NET 8.0  | .NET 8.0  | 123.2045 ns | 0.1516 ns | 0.1266 ns | 123.2041 ns |  5,328.211 |  154.79 |   45 | 0.0062 |     104 B |          NA |
| RawGuid_JsonDeserialize                   | .NET 8.0  | .NET 8.0  | 168.2229 ns | 0.1510 ns | 0.1338 ns | 168.1714 ns |  7,275.116 |  211.26 |   48 |      - |         - |          NA |
| DomainPrimitives_JsonSerialize            | .NET 8.0  | .NET 8.0  | 180.6866 ns | 0.2617 ns | 0.2320 ns | 180.6520 ns |  7,814.130 |  227.04 |   48 | 0.0062 |     104 B |          NA |
| DomainPrimitives_JsonDeserialize          | .NET 8.0  | .NET 8.0  | 190.0999 ns | 0.1955 ns | 0.1829 ns | 190.1442 ns |  8,221.228 |  238.73 |   49 |      - |         - |          NA |
| RawGuid_Create                            | .NET 9.0  | .NET 9.0  |   0.0038 ns | 0.0027 ns | 0.0025 ns |   0.0025 ns |      0.166 |    0.11 |    5 |      - |         - |          NA |
| DomainPrimitives_Create                   | .NET 9.0  | .NET 9.0  |   0.9862 ns | 0.0017 ns | 0.0015 ns |   0.9860 ns |     42.649 |    1.24 |   13 |      - |         - |          NA |
| Vogen_Create                              | .NET 9.0  | .NET 9.0  |   0.3738 ns | 0.0010 ns | 0.0009 ns |   0.3739 ns |     16.167 |    0.47 |   11 |      - |         - |          NA |
| StronglyTypedId_Create                    | .NET 9.0  | .NET 9.0  |   0.0995 ns | 0.0040 ns | 0.0037 ns |   0.1006 ns |      4.302 |    0.20 |    8 |      - |         - |          NA |
| ValueOf_Create                            | .NET 9.0  | .NET 9.0  |  13.0344 ns | 0.2518 ns | 0.2900 ns |  13.0193 ns |    563.696 |   20.43 |   31 | 0.0019 |      32 B |          NA |
| Meziantou_Create                          | .NET 9.0  | .NET 9.0  |   0.0965 ns | 0.0029 ns | 0.0027 ns |   0.0963 ns |      4.172 |    0.17 |    8 |      - |         - |          NA |
| TinyTypes_Create                          | .NET 9.0  | .NET 9.0  |   0.0255 ns | 0.0015 ns | 0.0014 ns |   0.0258 ns |      1.105 |    0.07 |    6 |      - |         - |          NA |
| RawGuid_Parse                             | .NET 9.0  | .NET 9.0  |  28.4178 ns | 0.0382 ns | 0.0357 ns |  28.4072 ns |  1,228.979 |   35.70 |   35 |      - |         - |          NA |
| DomainPrimitives_Parse                    | .NET 9.0  | .NET 9.0  |  29.3539 ns | 0.0366 ns | 0.0342 ns |  29.3363 ns |  1,269.466 |   36.87 |   35 |      - |         - |          NA |
| Vogen_Parse                               | .NET 9.0  | .NET 9.0  |  29.1929 ns | 0.0274 ns | 0.0229 ns |  29.1886 ns |  1,262.500 |   36.67 |   35 |      - |         - |          NA |
| StronglyTypedId_Parse                     | .NET 9.0  | .NET 9.0  |  28.6703 ns | 0.0202 ns | 0.0169 ns |  28.6666 ns |  1,239.899 |   36.01 |   35 |      - |         - |          NA |
| ValueOf_Parse                             | .NET 9.0  | .NET 9.0  |  32.5159 ns | 0.0591 ns | 0.0553 ns |  32.5048 ns |  1,406.212 |   40.88 |   36 | 0.0019 |      32 B |          NA |
| Meziantou_Parse                           | .NET 9.0  | .NET 9.0  |  29.3205 ns | 0.0317 ns | 0.0281 ns |  29.3162 ns |  1,268.020 |   36.83 |   35 |      - |         - |          NA |
| TinyTypes_Parse                           | .NET 9.0  | .NET 9.0  |  28.7155 ns | 0.0225 ns | 0.0200 ns |  28.7114 ns |  1,241.854 |   36.06 |   35 |      - |         - |          NA |
| DomainPrimitives_EqualityCheck            | .NET 9.0  | .NET 9.0  |   0.9084 ns | 0.0021 ns | 0.0020 ns |   0.9085 ns |     39.287 |    1.14 |   12 |      - |         - |          NA |
| RawGuid_ToString                          | .NET 9.0  | .NET 9.0  |  13.1487 ns | 0.2097 ns | 0.1859 ns |  13.1713 ns |    568.640 |   18.24 |   31 | 0.0057 |      96 B |          NA |
| DomainPrimitives_ToString                 | .NET 9.0  | .NET 9.0  |  14.7482 ns | 0.1625 ns | 0.1441 ns |  14.8155 ns |    637.814 |   19.47 |   31 | 0.0057 |      96 B |          NA |
| Vogen_ToString                            | .NET 9.0  | .NET 9.0  |  14.9847 ns | 0.0519 ns | 0.0460 ns |  14.9734 ns |    648.040 |   18.91 |   31 | 0.0057 |      96 B |          NA |
| StronglyTypedId_ToString                  | .NET 9.0  | .NET 9.0  |  14.2623 ns | 0.1833 ns | 0.1714 ns |  14.3825 ns |    616.798 |   19.29 |   31 | 0.0057 |      96 B |          NA |
| ValueOf_ToString                          | .NET 9.0  | .NET 9.0  |  22.5767 ns | 0.1180 ns | 0.0986 ns |  22.5777 ns |    976.373 |   28.64 |   33 | 0.0076 |     128 B |          NA |
| Meziantou_ToString                        | .NET 9.0  | .NET 9.0  |  34.1155 ns | 0.1501 ns | 0.1331 ns |  34.1096 ns |  1,475.387 |   43.19 |   37 | 0.0148 |     248 B |          NA |
| TinyTypes_ToString                        | .NET 9.0  | .NET 9.0  |  11.4818 ns | 0.2729 ns | 0.2553 ns |  11.4300 ns |    496.553 |   17.95 |   30 | 0.0019 |      32 B |          NA |
| DomainPrimitives_TryParse                 | .NET 9.0  | .NET 9.0  |  27.9896 ns | 0.0235 ns | 0.0219 ns |  27.9914 ns |  1,210.461 |   35.14 |   35 |      - |         - |          NA |
| DomainPrimitives_SpanParse                | .NET 9.0  | .NET 9.0  |  27.7380 ns | 0.0227 ns | 0.0201 ns |  27.7437 ns |  1,199.583 |   34.83 |   35 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanParse            | .NET 9.0  | .NET 9.0  |  60.5609 ns | 0.0513 ns | 0.0428 ns |  60.5717 ns |  2,619.071 |   76.06 |   41 | 0.0038 |      64 B |          NA |
| DomainPrimitives_SpanFormat               | .NET 9.0  | .NET 9.0  |   5.1275 ns | 0.0037 ns | 0.0035 ns |   5.1270 ns |    221.749 |    6.44 |   22 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanFormat           | .NET 9.0  | .NET 9.0  |   4.0716 ns | 0.0014 ns | 0.0012 ns |   4.0717 ns |    176.085 |    5.11 |   19 |      - |         - |          NA |
| StringPrimitive_Email_Create              | .NET 9.0  | .NET 9.0  | 173.1836 ns | 0.1855 ns | 0.1645 ns | 173.1972 ns |  7,489.650 |  217.52 |   48 |      - |         - |          NA |
| StringPrimitive_Email_TryParse            | .NET 9.0  | .NET 9.0  | 176.3092 ns | 0.1656 ns | 0.1468 ns | 176.3026 ns |  7,624.824 |  221.42 |   48 |      - |         - |          NA |
| NumericPrimitive_Money_Create             | .NET 9.0  | .NET 9.0  |   4.5088 ns | 0.0040 ns | 0.0037 ns |   4.5072 ns |    194.993 |    5.66 |   20 |      - |         - |          NA |
| NumericPrimitive_Money_Add                | .NET 9.0  | .NET 9.0  |  10.0353 ns | 0.0038 ns | 0.0033 ns |  10.0353 ns |    433.995 |   12.60 |   28 |      - |         - |          NA |
| ValueObject_Create                        | .NET 9.0  | .NET 9.0  |   0.0006 ns | 0.0005 ns | 0.0004 ns |   0.0005 ns |      0.026 |    0.02 |    2 |      - |         - |          NA |
| SmartEnum_FromValue                       | .NET 9.0  | .NET 9.0  |  16.4835 ns | 0.0993 ns | 0.0829 ns |  16.5061 ns |    712.861 |   20.98 |   31 | 0.0019 |      32 B |          NA |
| RawGuid_JsonSerialize                     | .NET 9.0  | .NET 9.0  | 109.0131 ns | 0.2140 ns | 0.1897 ns | 108.9744 ns |  4,714.474 |  137.08 |   44 | 0.0062 |     104 B |          NA |
| RawGuid_JsonDeserialize                   | .NET 9.0  | .NET 9.0  | 127.7055 ns | 0.1782 ns | 0.1579 ns | 127.6831 ns |  5,522.865 |  160.46 |   45 |      - |         - |          NA |
| DomainPrimitives_JsonSerialize            | .NET 9.0  | .NET 9.0  | 157.6396 ns | 0.1090 ns | 0.0851 ns | 157.6165 ns |  6,817.419 |  198.01 |   47 | 0.0062 |     104 B |          NA |
| DomainPrimitives_JsonDeserialize          | .NET 9.0  | .NET 9.0  | 126.3555 ns | 0.2335 ns | 0.2070 ns | 126.3031 ns |  5,464.482 |  158.86 |   45 |      - |         - |          NA |
| RawGuid_Create                            | .NET 9.0  | .NET 9.0  |   0.0018 ns | 0.0010 ns | 0.0009 ns |   0.0018 ns |      0.078 |    0.04 |    3 |      - |         - |          NA |
| DomainPrimitives_Create                   | .NET 9.0  | .NET 9.0  |   1.3411 ns | 0.0007 ns | 0.0006 ns |   1.3412 ns |     58.000 |    1.68 |   16 |      - |         - |          NA |
| Vogen_Create                              | .NET 9.0  | .NET 9.0  |   0.3752 ns | 0.0005 ns | 0.0004 ns |   0.3753 ns |     16.228 |    0.47 |   11 |      - |         - |          NA |
| StronglyTypedId_Create                    | .NET 9.0  | .NET 9.0  |   0.0237 ns | 0.0014 ns | 0.0013 ns |   0.0236 ns |      1.023 |    0.06 |    6 |      - |         - |          NA |
| ValueOf_Create                            | .NET 9.0  | .NET 9.0  |   8.7857 ns | 0.0181 ns | 0.0151 ns |   8.7831 ns |    379.955 |   11.05 |   27 | 0.0019 |      32 B |          NA |
| Meziantou_Create                          | .NET 9.0  | .NET 9.0  |   0.0243 ns | 0.0009 ns | 0.0007 ns |   0.0246 ns |      1.053 |    0.04 |    6 |      - |         - |          NA |
| TinyTypes_Create                          | .NET 9.0  | .NET 9.0  |   0.0266 ns | 0.0021 ns | 0.0019 ns |   0.0257 ns |      1.151 |    0.08 |    6 |      - |         - |          NA |
| RawGuid_Parse                             | .NET 9.0  | .NET 9.0  |  28.4916 ns | 0.0440 ns | 0.0412 ns |  28.4758 ns |  1,232.172 |   35.80 |   35 |      - |         - |          NA |
| DomainPrimitives_Parse                    | .NET 9.0  | .NET 9.0  |  29.6345 ns | 0.0368 ns | 0.0344 ns |  29.6339 ns |  1,281.598 |   37.22 |   35 |      - |         - |          NA |
| Vogen_Parse                               | .NET 9.0  | .NET 9.0  |  29.6861 ns | 0.0522 ns | 0.0488 ns |  29.6689 ns |  1,283.832 |   37.32 |   35 |      - |         - |          NA |
| StronglyTypedId_Parse                     | .NET 9.0  | .NET 9.0  |  28.1594 ns | 0.0221 ns | 0.0196 ns |  28.1577 ns |  1,217.805 |   35.36 |   35 |      - |         - |          NA |
| ValueOf_Parse                             | .NET 9.0  | .NET 9.0  |  32.0996 ns | 0.0319 ns | 0.0249 ns |  32.0998 ns |  1,388.206 |   40.33 |   36 | 0.0019 |      32 B |          NA |
| Meziantou_Parse                           | .NET 9.0  | .NET 9.0  |  29.3244 ns | 0.0455 ns | 0.0380 ns |  29.3216 ns |  1,268.190 |   36.85 |   35 |      - |         - |          NA |
| TinyTypes_Parse                           | .NET 9.0  | .NET 9.0  |  27.9705 ns | 0.0244 ns | 0.0204 ns |  27.9692 ns |  1,209.639 |   35.13 |   35 |      - |         - |          NA |
| DomainPrimitives_EqualityCheck            | .NET 9.0  | .NET 9.0  |   0.9096 ns | 0.0030 ns | 0.0027 ns |   0.9088 ns |     39.336 |    1.15 |   12 |      - |         - |          NA |
| RawGuid_ToString                          | .NET 9.0  | .NET 9.0  |  14.7574 ns | 0.3688 ns | 0.5633 ns |  15.0804 ns |    638.212 |   30.30 |   31 | 0.0057 |      96 B |          NA |
| DomainPrimitives_ToString                 | .NET 9.0  | .NET 9.0  |  14.2159 ns | 0.0219 ns | 0.0205 ns |  14.2124 ns |    614.792 |   17.86 |   31 | 0.0057 |      96 B |          NA |
| Vogen_ToString                            | .NET 9.0  | .NET 9.0  |  14.9181 ns | 0.0428 ns | 0.0401 ns |  14.9088 ns |    645.162 |   18.80 |   31 | 0.0057 |      96 B |          NA |
| StronglyTypedId_ToString                  | .NET 9.0  | .NET 9.0  |  14.0769 ns | 0.2242 ns | 0.2097 ns |  13.9619 ns |    608.782 |   19.73 |   31 | 0.0057 |      96 B |          NA |
| ValueOf_ToString                          | .NET 9.0  | .NET 9.0  |  22.2005 ns | 0.2795 ns | 0.2615 ns |  22.1144 ns |    960.101 |   29.94 |   33 | 0.0076 |     128 B |          NA |
| Meziantou_ToString                        | .NET 9.0  | .NET 9.0  |  34.4038 ns | 0.1470 ns | 0.1375 ns |  34.3827 ns |  1,487.858 |   43.57 |   37 | 0.0148 |     248 B |          NA |
| TinyTypes_ToString                        | .NET 9.0  | .NET 9.0  |  11.1519 ns | 0.2417 ns | 0.2018 ns |  11.2355 ns |    482.283 |   16.33 |   30 | 0.0019 |      32 B |          NA |
| DomainPrimitives_TryParse                 | .NET 9.0  | .NET 9.0  |  28.2576 ns | 0.0331 ns | 0.0276 ns |  28.2603 ns |  1,222.052 |   35.50 |   35 |      - |         - |          NA |
| DomainPrimitives_SpanParse                | .NET 9.0  | .NET 9.0  |  28.0774 ns | 0.0370 ns | 0.0346 ns |  28.0823 ns |  1,214.259 |   35.27 |   35 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanParse            | .NET 9.0  | .NET 9.0  |  61.3370 ns | 0.0759 ns | 0.0673 ns |  61.3272 ns |  2,652.635 |   77.05 |   41 | 0.0038 |      64 B |          NA |
| DomainPrimitives_SpanFormat               | .NET 9.0  | .NET 9.0  |   5.1313 ns | 0.0034 ns | 0.0028 ns |   5.1313 ns |    221.914 |    6.44 |   22 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanFormat           | .NET 9.0  | .NET 9.0  |   4.0724 ns | 0.0019 ns | 0.0016 ns |   4.0720 ns |    176.120 |    5.11 |   19 |      - |         - |          NA |
| StringPrimitive_Email_Create              | .NET 9.0  | .NET 9.0  | 175.1257 ns | 0.1858 ns | 0.1647 ns | 175.0934 ns |  7,573.640 |  219.96 |   48 |      - |         - |          NA |
| StringPrimitive_Email_TryParse            | .NET 9.0  | .NET 9.0  | 177.0175 ns | 0.1853 ns | 0.1642 ns | 177.0394 ns |  7,655.453 |  222.33 |   48 |      - |         - |          NA |
| NumericPrimitive_Money_Create             | .NET 9.0  | .NET 9.0  |   4.5080 ns | 0.0015 ns | 0.0013 ns |   4.5079 ns |    194.958 |    5.66 |   20 |      - |         - |          NA |
| NumericPrimitive_Money_Add                | .NET 9.0  | .NET 9.0  |   9.9930 ns | 0.0084 ns | 0.0074 ns |   9.9911 ns |    432.165 |   12.55 |   28 |      - |         - |          NA |
| ValueObject_Create                        | .NET 9.0  | .NET 9.0  |   0.0002 ns | 0.0003 ns | 0.0002 ns |   0.0002 ns |      0.009 |    0.01 |    1 |      - |         - |          NA |
| SmartEnum_FromValue                       | .NET 9.0  | .NET 9.0  |  16.4604 ns | 0.3214 ns | 0.3007 ns |  16.4817 ns |    711.860 |   24.20 |   31 | 0.0019 |      32 B |          NA |
| RawGuid_JsonSerialize                     | .NET 9.0  | .NET 9.0  | 109.4170 ns | 0.1456 ns | 0.1362 ns | 109.3793 ns |  4,731.943 |  137.46 |   44 | 0.0062 |     104 B |          NA |
| RawGuid_JsonDeserialize                   | .NET 9.0  | .NET 9.0  | 127.0539 ns | 0.3083 ns | 0.2733 ns | 127.1301 ns |  5,494.683 |  159.91 |   45 |      - |         - |          NA |
| DomainPrimitives_JsonSerialize            | .NET 9.0  | .NET 9.0  | 169.5950 ns | 0.6778 ns | 0.6340 ns | 169.4396 ns |  7,334.456 |  214.52 |   48 | 0.0062 |     104 B |          NA |
| DomainPrimitives_JsonDeserialize          | .NET 9.0  | .NET 9.0  | 125.3679 ns | 0.1284 ns | 0.1201 ns | 125.3288 ns |  5,421.770 |  157.44 |   45 |      - |         - |          NA |
| Dapper_TypeHandler_SetValue               | .NET 10.0 | .NET 10.0 |   0.3534 ns | 0.0019 ns | 0.0018 ns |   0.3526 ns |     15.285 |    0.45 |   10 |      - |         - |          NA |
| Dapper_TypeHandler_Parse                  | .NET 10.0 | .NET 10.0 |   0.9444 ns | 0.0016 ns | 0.0014 ns |   0.9446 ns |     40.842 |    1.19 |   13 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertToProvider   | .NET 10.0 | .NET 10.0 |   1.1727 ns | 0.0066 ns | 0.0062 ns |   1.1726 ns |     50.715 |    1.49 |   14 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertFromProvider | .NET 10.0 | .NET 10.0 |   1.2975 ns | 0.0008 ns | 0.0007 ns |   1.2975 ns |     56.113 |    1.63 |   16 |      - |         - |          NA |
| Dapper_TypeHandler_SetValue               | .NET 8.0  | .NET 8.0  |   7.6542 ns | 0.0060 ns | 0.0053 ns |   7.6533 ns |    331.020 |    9.61 |   26 | 0.0019 |      32 B |          NA |
| Dapper_TypeHandler_Parse                  | .NET 8.0  | .NET 8.0  |   0.9700 ns | 0.0014 ns | 0.0012 ns |   0.9696 ns |     41.949 |    1.22 |   13 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertToProvider   | .NET 8.0  | .NET 8.0  |  12.4485 ns | 0.0062 ns | 0.0058 ns |  12.4491 ns |    538.359 |   15.63 |   31 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertFromProvider | .NET 8.0  | .NET 8.0  |   1.3199 ns | 0.0007 ns | 0.0006 ns |   1.3202 ns |     57.082 |    1.66 |   16 |      - |         - |          NA |
| Dapper_TypeHandler_SetValue               | .NET 9.0  | .NET 9.0  |   1.2101 ns | 0.0056 ns | 0.0052 ns |   1.2096 ns |     52.332 |    1.53 |   15 |      - |         - |          NA |
| Dapper_TypeHandler_Parse                  | .NET 9.0  | .NET 9.0  |   0.9447 ns | 0.0022 ns | 0.0020 ns |   0.9441 ns |     40.857 |    1.19 |   13 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertToProvider   | .NET 9.0  | .NET 9.0  |   1.1680 ns | 0.0055 ns | 0.0051 ns |   1.1673 ns |     50.512 |    1.48 |   14 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertFromProvider | .NET 9.0  | .NET 9.0  |   0.9444 ns | 0.0017 ns | 0.0014 ns |   0.9440 ns |     40.841 |    1.19 |   13 |      - |         - |          NA |
| Dapper_TypeHandler_SetValue               | .NET 9.0  | .NET 9.0  |   1.2083 ns | 0.0046 ns | 0.0038 ns |   1.2081 ns |     52.256 |    1.53 |   15 |      - |         - |          NA |
| Dapper_TypeHandler_Parse                  | .NET 9.0  | .NET 9.0  |   0.9455 ns | 0.0005 ns | 0.0005 ns |   0.9454 ns |     40.889 |    1.19 |   13 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertToProvider   | .NET 9.0  | .NET 9.0  |   1.1677 ns | 0.0057 ns | 0.0053 ns |   1.1676 ns |     50.501 |    1.48 |   14 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertFromProvider | .NET 9.0  | .NET 9.0  |   1.2974 ns | 0.0009 ns | 0.0008 ns |   1.2971 ns |     56.107 |    1.63 |   16 |      - |         - |          NA |
