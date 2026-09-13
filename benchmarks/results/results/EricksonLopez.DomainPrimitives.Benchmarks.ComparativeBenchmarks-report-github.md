```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.401
  [Host]    : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v3
  .NET 8.0  : .NET 8.0.31 (8.0.31, 8.0.3126.42015), X64 RyuJIT x86-64-v3
  .NET 9.0  : .NET 9.0.20 (9.0.20, 9.0.2026.41315), X64 RyuJIT x86-64-v3


```
| Method                                    | Job       | Runtime   | Mean        | Error     | StdDev    | Median      | Ratio     | RatioSD | Rank | Gen0   | Allocated | Alloc Ratio |
|------------------------------------------ |---------- |---------- |------------:|----------:|----------:|------------:|----------:|--------:|-----:|-------:|----------:|------------:|
| RawGuid_Create                            | .NET 10.0 | .NET 10.0 |   0.1144 ns | 0.0049 ns | 0.0046 ns |   0.1151 ns |      4.84 |    0.28 |    7 |      - |         - |          NA |
| DomainPrimitives_Create                   | .NET 10.0 | .NET 10.0 |   0.9474 ns | 0.0019 ns | 0.0018 ns |   0.9471 ns |     40.08 |    1.75 |   11 |      - |         - |          NA |
| Vogen_Create                              | .NET 10.0 | .NET 10.0 |   0.3536 ns | 0.0019 ns | 0.0018 ns |   0.3535 ns |     14.96 |    0.66 |    8 |      - |         - |          NA |
| StronglyTypedId_Create                    | .NET 10.0 | .NET 10.0 |   0.0035 ns | 0.0015 ns | 0.0013 ns |   0.0032 ns |      0.15 |    0.05 |    4 |      - |         - |          NA |
| ValueOf_Create                            | .NET 10.0 | .NET 10.0 |   8.2882 ns | 0.2316 ns | 0.3394 ns |   8.2702 ns |    350.61 |   20.83 |   24 | 0.0019 |      32 B |          NA |
| Meziantou_Create                          | .NET 10.0 | .NET 10.0 |   0.0633 ns | 0.0129 ns | 0.0115 ns |   0.0605 ns |      2.68 |    0.48 |    6 |      - |         - |          NA |
| TinyTypes_Create                          | .NET 10.0 | .NET 10.0 |   0.0644 ns | 0.0051 ns | 0.0045 ns |   0.0655 ns |      2.72 |    0.22 |    6 |      - |         - |          NA |
| RawGuid_Parse                             | .NET 10.0 | .NET 10.0 |  27.2641 ns | 0.0554 ns | 0.0519 ns |  27.2649 ns |  1,153.34 |   50.40 |   32 |      - |         - |          NA |
| DomainPrimitives_Parse                    | .NET 10.0 | .NET 10.0 |  27.6259 ns | 0.0265 ns | 0.0235 ns |  27.6327 ns |  1,168.64 |   51.04 |   32 |      - |         - |          NA |
| Vogen_Parse                               | .NET 10.0 | .NET 10.0 |  29.1669 ns | 0.0457 ns | 0.0382 ns |  29.1621 ns |  1,233.83 |   53.91 |   32 |      - |         - |          NA |
| StronglyTypedId_Parse                     | .NET 10.0 | .NET 10.0 |  27.2846 ns | 0.0406 ns | 0.0380 ns |  27.2747 ns |  1,154.20 |   50.41 |   32 |      - |         - |          NA |
| ValueOf_Parse                             | .NET 10.0 | .NET 10.0 |  29.9064 ns | 0.0441 ns | 0.0344 ns |  29.9093 ns |  1,265.11 |   55.28 |   32 | 0.0019 |      32 B |          NA |
| Meziantou_Parse                           | .NET 10.0 | .NET 10.0 |  42.1361 ns | 0.0400 ns | 0.0375 ns |  42.1363 ns |  1,782.46 |   77.83 |   35 |      - |         - |          NA |
| TinyTypes_Parse                           | .NET 10.0 | .NET 10.0 |  27.7337 ns | 0.0173 ns | 0.0145 ns |  27.7349 ns |  1,173.20 |   51.24 |   32 |      - |         - |          NA |
| DomainPrimitives_EqualityCheck            | .NET 10.0 | .NET 10.0 |   0.9594 ns | 0.0040 ns | 0.0037 ns |   0.9592 ns |     40.59 |    1.78 |   11 |      - |         - |          NA |
| RawGuid_ToString                          | .NET 10.0 | .NET 10.0 |  10.2651 ns | 0.1444 ns | 0.1350 ns |  10.3217 ns |    434.24 |   19.75 |   24 | 0.0057 |      96 B |          NA |
| DomainPrimitives_ToString                 | .NET 10.0 | .NET 10.0 |   9.6517 ns | 0.0970 ns | 0.0908 ns |   9.6442 ns |    408.29 |   18.21 |   24 | 0.0057 |      96 B |          NA |
| Vogen_ToString                            | .NET 10.0 | .NET 10.0 |  11.2578 ns | 0.0264 ns | 0.0221 ns |  11.2532 ns |    476.23 |   20.82 |   25 | 0.0057 |      96 B |          NA |
| StronglyTypedId_ToString                  | .NET 10.0 | .NET 10.0 |   9.4312 ns | 0.0997 ns | 0.0933 ns |   9.4738 ns |    398.96 |   17.83 |   24 | 0.0057 |      96 B |          NA |
| ValueOf_ToString                          | .NET 10.0 | .NET 10.0 |  21.3475 ns | 0.4958 ns | 0.4869 ns |  21.1280 ns |    903.05 |   44.21 |   29 | 0.0076 |     128 B |          NA |
| Meziantou_ToString                        | .NET 10.0 | .NET 10.0 |  25.2547 ns | 0.2227 ns | 0.2083 ns |  25.2995 ns |  1,068.33 |   47.42 |   31 | 0.0148 |     248 B |          NA |
| TinyTypes_ToString                        | .NET 10.0 | .NET 10.0 |   6.8600 ns | 0.1650 ns | 0.1543 ns |   6.9526 ns |    290.20 |   14.16 |   23 | 0.0019 |      32 B |          NA |
| DomainPrimitives_TryParse                 | .NET 10.0 | .NET 10.0 |  27.0218 ns | 0.0220 ns | 0.0195 ns |  27.0194 ns |  1,143.09 |   49.92 |   32 |      - |         - |          NA |
| DomainPrimitives_SpanParse                | .NET 10.0 | .NET 10.0 |  27.1886 ns | 0.0165 ns | 0.0146 ns |  27.1881 ns |  1,150.14 |   50.23 |   32 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanParse            | .NET 10.0 | .NET 10.0 |  47.7522 ns | 0.0634 ns | 0.0530 ns |  47.7610 ns |  2,020.03 |   88.25 |   36 | 0.0038 |      64 B |          NA |
| DomainPrimitives_SpanFormat               | .NET 10.0 | .NET 10.0 |   2.9267 ns | 0.0135 ns | 0.0126 ns |   2.9257 ns |    123.81 |    5.43 |   17 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanFormat           | .NET 10.0 | .NET 10.0 |   2.4178 ns | 0.0209 ns | 0.0195 ns |   2.4155 ns |    102.28 |    4.54 |   16 |      - |         - |          NA |
| StringPrimitive_Email_Create              | .NET 10.0 | .NET 10.0 | 170.5562 ns | 0.1015 ns | 0.0900 ns | 170.5447 ns |  7,214.94 |  315.07 |   46 |      - |         - |          NA |
| StringPrimitive_Email_TryParse            | .NET 10.0 | .NET 10.0 | 174.4778 ns | 0.2408 ns | 0.2135 ns | 174.4407 ns |  7,380.83 |  322.41 |   46 |      - |         - |          NA |
| NumericPrimitive_Money_Create             | .NET 10.0 | .NET 10.0 |   4.1144 ns | 0.0026 ns | 0.0022 ns |   4.1138 ns |    174.05 |    7.60 |   18 |      - |         - |          NA |
| NumericPrimitive_Money_Add                | .NET 10.0 | .NET 10.0 |   9.6672 ns | 0.0042 ns | 0.0037 ns |   9.6675 ns |    408.95 |   17.86 |   24 |      - |         - |          NA |
| ValueObject_Create                        | .NET 10.0 | .NET 10.0 |   0.3517 ns | 0.0012 ns | 0.0011 ns |   0.3514 ns |     14.88 |    0.65 |    8 |      - |         - |          NA |
| SmartEnum_FromValue                       | .NET 10.0 | .NET 10.0 |   0.9702 ns | 0.0026 ns | 0.0023 ns |   0.9694 ns |     41.04 |    1.79 |   11 |      - |         - |          NA |
| RawGuid_JsonSerialize                     | .NET 10.0 | .NET 10.0 |  93.3856 ns | 0.0946 ns | 0.0838 ns |  93.3727 ns |  3,950.44 |  172.53 |   38 | 0.0062 |     104 B |          NA |
| RawGuid_JsonDeserialize                   | .NET 10.0 | .NET 10.0 | 108.0948 ns | 0.2804 ns | 0.2623 ns | 108.0830 ns |  4,572.67 |  199.92 |   40 |      - |         - |          NA |
| DomainPrimitives_JsonSerialize            | .NET 10.0 | .NET 10.0 | 135.9151 ns | 0.1286 ns | 0.1074 ns | 135.9282 ns |  5,749.54 |  251.14 |   44 | 0.0062 |     104 B |          NA |
| DomainPrimitives_JsonDeserialize          | .NET 10.0 | .NET 10.0 | 105.3718 ns | 0.1420 ns | 0.1186 ns | 105.3533 ns |  4,457.48 |  194.74 |   39 |      - |         - |          NA |
| RawGuid_Create                            | .NET 8.0  | .NET 8.0  |   0.0237 ns | 0.0012 ns | 0.0011 ns |   0.0235 ns |      1.00 |    0.06 |    5 |      - |         - |          NA |
| DomainPrimitives_Create                   | .NET 8.0  | .NET 8.0  |   0.9681 ns | 0.0012 ns | 0.0010 ns |   0.9682 ns |     40.95 |    1.79 |   11 |      - |         - |          NA |
| Vogen_Create                              | .NET 8.0  | .NET 8.0  |   6.5882 ns | 0.0020 ns | 0.0016 ns |   6.5875 ns |    278.70 |   12.17 |   23 |      - |         - |          NA |
| StronglyTypedId_Create                    | .NET 8.0  | .NET 8.0  |   0.0245 ns | 0.0010 ns | 0.0008 ns |   0.0245 ns |      1.04 |    0.06 |    5 |      - |         - |          NA |
| ValueOf_Create                            | .NET 8.0  | .NET 8.0  |   9.4599 ns | 0.2053 ns | 0.1920 ns |   9.4902 ns |    400.18 |   19.16 |   24 | 0.0019 |      32 B |          NA |
| Meziantou_Create                          | .NET 8.0  | .NET 8.0  |   0.0231 ns | 0.0011 ns | 0.0010 ns |   0.0230 ns |      0.98 |    0.06 |    5 |      - |         - |          NA |
| TinyTypes_Create                          | .NET 8.0  | .NET 8.0  |   0.0232 ns | 0.0007 ns | 0.0005 ns |   0.0232 ns |      0.98 |    0.05 |    5 |      - |         - |          NA |
| RawGuid_Parse                             | .NET 8.0  | .NET 8.0  |  28.7820 ns | 0.0225 ns | 0.0199 ns |  28.7818 ns |  1,217.55 |   53.17 |   32 |      - |         - |          NA |
| DomainPrimitives_Parse                    | .NET 8.0  | .NET 8.0  |  35.3984 ns | 0.0238 ns | 0.0198 ns |  35.4026 ns |  1,497.44 |   65.40 |   34 |      - |         - |          NA |
| Vogen_Parse                               | .NET 8.0  | .NET 8.0  |  32.7412 ns | 0.0274 ns | 0.0256 ns |  32.7379 ns |  1,385.03 |   60.48 |   33 |      - |         - |          NA |
| StronglyTypedId_Parse                     | .NET 8.0  | .NET 8.0  |  28.4989 ns | 0.0276 ns | 0.0230 ns |  28.4936 ns |  1,205.57 |   52.66 |   32 |      - |         - |          NA |
| ValueOf_Parse                             | .NET 8.0  | .NET 8.0  |  34.2312 ns | 0.0262 ns | 0.0205 ns |  34.2312 ns |  1,448.06 |   63.26 |   34 | 0.0019 |      32 B |          NA |
| Meziantou_Parse                           | .NET 8.0  | .NET 8.0  |  29.2519 ns | 0.0212 ns | 0.0188 ns |  29.2519 ns |  1,237.43 |   54.04 |   32 |      - |         - |          NA |
| TinyTypes_Parse                           | .NET 8.0  | .NET 8.0  |  28.7504 ns | 0.0265 ns | 0.0235 ns |  28.7485 ns |  1,216.21 |   53.12 |   32 |      - |         - |          NA |
| DomainPrimitives_EqualityCheck            | .NET 8.0  | .NET 8.0  |   0.9043 ns | 0.0016 ns | 0.0014 ns |   0.9040 ns |     38.26 |    1.67 |   10 |      - |         - |          NA |
| RawGuid_ToString                          | .NET 8.0  | .NET 8.0  |  13.6639 ns | 0.3357 ns | 0.2976 ns |  13.5878 ns |    578.02 |   28.02 |   27 | 0.0057 |      96 B |          NA |
| DomainPrimitives_ToString                 | .NET 8.0  | .NET 8.0  |  13.7059 ns | 0.1626 ns | 0.1358 ns |  13.6656 ns |    579.79 |   25.92 |   27 | 0.0057 |      96 B |          NA |
| Vogen_ToString                            | .NET 8.0  | .NET 8.0  |  26.9586 ns | 0.0222 ns | 0.0185 ns |  26.9528 ns |  1,140.41 |   49.81 |   32 | 0.0057 |      96 B |          NA |
| StronglyTypedId_ToString                  | .NET 8.0  | .NET 8.0  |  15.3120 ns | 0.3799 ns | 0.4523 ns |  15.5285 ns |    647.74 |   33.90 |   28 | 0.0057 |      96 B |          NA |
| ValueOf_ToString                          | .NET 8.0  | .NET 8.0  |  23.7279 ns | 0.5407 ns | 0.7755 ns |  24.0948 ns |  1,003.75 |   54.39 |   30 | 0.0076 |     128 B |          NA |
| Meziantou_ToString                        | .NET 8.0  | .NET 8.0  |  34.7149 ns | 0.1570 ns | 0.1469 ns |  34.7109 ns |  1,468.53 |   64.40 |   34 | 0.0148 |     248 B |          NA |
| TinyTypes_ToString                        | .NET 8.0  | .NET 8.0  |   9.8397 ns | 0.0374 ns | 0.0312 ns |   9.8296 ns |    416.24 |   18.22 |   24 | 0.0019 |      32 B |          NA |
| DomainPrimitives_TryParse                 | .NET 8.0  | .NET 8.0  |  28.0141 ns | 0.0195 ns | 0.0173 ns |  28.0140 ns |  1,185.06 |   51.75 |   32 |      - |         - |          NA |
| DomainPrimitives_SpanParse                | .NET 8.0  | .NET 8.0  |  28.1624 ns | 0.0442 ns | 0.0413 ns |  28.1572 ns |  1,191.34 |   52.04 |   32 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanParse            | .NET 8.0  | .NET 8.0  |  60.1712 ns | 0.0454 ns | 0.0379 ns |  60.1781 ns |  2,545.39 |  111.18 |   37 | 0.0038 |      64 B |          NA |
| DomainPrimitives_SpanFormat               | .NET 8.0  | .NET 8.0  |   5.4787 ns | 0.0032 ns | 0.0027 ns |   5.4778 ns |    231.76 |   10.12 |   22 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanFormat           | .NET 8.0  | .NET 8.0  |   4.4232 ns | 0.0028 ns | 0.0025 ns |   4.4229 ns |    187.11 |    8.17 |   19 |      - |         - |          NA |
| StringPrimitive_Email_Create              | .NET 8.0  | .NET 8.0  | 326.2026 ns | 0.1307 ns | 0.1020 ns | 326.2426 ns | 13,799.16 |  602.82 |   48 |      - |         - |          NA |
| StringPrimitive_Email_TryParse            | .NET 8.0  | .NET 8.0  | 327.4170 ns | 0.1261 ns | 0.1118 ns | 327.4398 ns | 13,850.54 |  604.81 |   48 |      - |         - |          NA |
| NumericPrimitive_Money_Create             | .NET 8.0  | .NET 8.0  |   4.8376 ns | 0.0044 ns | 0.0042 ns |   4.8358 ns |    204.64 |    8.94 |   20 |      - |         - |          NA |
| NumericPrimitive_Money_Add                | .NET 8.0  | .NET 8.0  |  10.2959 ns | 0.0035 ns | 0.0027 ns |  10.2953 ns |    435.54 |   19.03 |   24 |      - |         - |          NA |
| ValueObject_Create                        | .NET 8.0  | .NET 8.0  |   0.3739 ns | 0.0006 ns | 0.0005 ns |   0.3739 ns |     15.82 |    0.69 |    9 |      - |         - |          NA |
| SmartEnum_FromValue                       | .NET 8.0  | .NET 8.0  |  16.6418 ns | 0.2528 ns | 0.2364 ns |  16.7227 ns |    703.99 |   32.23 |   28 | 0.0019 |      32 B |          NA |
| RawGuid_JsonSerialize                     | .NET 8.0  | .NET 8.0  | 122.5081 ns | 0.2040 ns | 0.1704 ns | 122.4849 ns |  5,182.39 |  226.44 |   42 | 0.0062 |     104 B |          NA |
| RawGuid_JsonDeserialize                   | .NET 8.0  | .NET 8.0  | 171.5540 ns | 0.1513 ns | 0.1415 ns | 171.5328 ns |  7,257.15 |  316.89 |   46 |      - |         - |          NA |
| DomainPrimitives_JsonSerialize            | .NET 8.0  | .NET 8.0  | 181.5334 ns | 0.2230 ns | 0.1977 ns | 181.5530 ns |  7,679.30 |  335.42 |   46 | 0.0062 |     104 B |          NA |
| DomainPrimitives_JsonDeserialize          | .NET 8.0  | .NET 8.0  | 188.9960 ns | 0.1710 ns | 0.1516 ns | 188.9228 ns |  7,994.99 |  349.16 |   47 |      - |         - |          NA |
| RawGuid_Create                            | .NET 9.0  | .NET 9.0  |   0.0026 ns | 0.0015 ns | 0.0013 ns |   0.0026 ns |      0.11 |    0.05 |    3 |      - |         - |          NA |
| DomainPrimitives_Create                   | .NET 9.0  | .NET 9.0  |   0.9888 ns | 0.0015 ns | 0.0013 ns |   0.9887 ns |     41.83 |    1.83 |   11 |      - |         - |          NA |
| Vogen_Create                              | .NET 9.0  | .NET 9.0  |   0.3748 ns | 0.0006 ns | 0.0005 ns |   0.3747 ns |     15.85 |    0.69 |    9 |      - |         - |          NA |
| StronglyTypedId_Create                    | .NET 9.0  | .NET 9.0  |   0.0278 ns | 0.0024 ns | 0.0023 ns |   0.0272 ns |      1.18 |    0.11 |    5 |      - |         - |          NA |
| ValueOf_Create                            | .NET 9.0  | .NET 9.0  |   9.2385 ns | 0.1642 ns | 0.1536 ns |   9.1337 ns |    390.81 |   18.19 |   24 | 0.0019 |      32 B |          NA |
| Meziantou_Create                          | .NET 9.0  | .NET 9.0  |   0.0283 ns | 0.0016 ns | 0.0015 ns |   0.0284 ns |      1.20 |    0.08 |    5 |      - |         - |          NA |
| TinyTypes_Create                          | .NET 9.0  | .NET 9.0  |  16.8319 ns | 0.0048 ns | 0.0038 ns |  16.8313 ns |    712.03 |   31.10 |   28 |      - |         - |          NA |
| RawGuid_Parse                             | .NET 9.0  | .NET 9.0  |  28.4302 ns | 0.0298 ns | 0.0249 ns |  28.4294 ns |  1,202.67 |   52.54 |   32 |      - |         - |          NA |
| DomainPrimitives_Parse                    | .NET 9.0  | .NET 9.0  |  29.5471 ns | 0.0258 ns | 0.0215 ns |  29.5506 ns |  1,249.91 |   54.60 |   32 |      - |         - |          NA |
| Vogen_Parse                               | .NET 9.0  | .NET 9.0  |  29.1222 ns | 0.0374 ns | 0.0350 ns |  29.1271 ns |  1,231.94 |   53.80 |   32 |      - |         - |          NA |
| StronglyTypedId_Parse                     | .NET 9.0  | .NET 9.0  |  28.3450 ns | 0.0201 ns | 0.0178 ns |  28.3409 ns |  1,199.06 |   52.36 |   32 |      - |         - |          NA |
| ValueOf_Parse                             | .NET 9.0  | .NET 9.0  |  32.5710 ns | 0.0386 ns | 0.0322 ns |  32.5663 ns |  1,377.83 |   60.19 |   33 | 0.0019 |      32 B |          NA |
| Meziantou_Parse                           | .NET 9.0  | .NET 9.0  |  28.9606 ns | 0.0603 ns | 0.0564 ns |  28.9501 ns |  1,225.10 |   53.54 |   32 |      - |         - |          NA |
| TinyTypes_Parse                           | .NET 9.0  | .NET 9.0  |  28.0182 ns | 0.0229 ns | 0.0203 ns |  28.0152 ns |  1,185.24 |   51.76 |   32 |      - |         - |          NA |
| DomainPrimitives_EqualityCheck            | .NET 9.0  | .NET 9.0  |   0.9070 ns | 0.0012 ns | 0.0011 ns |   0.9064 ns |     38.37 |    1.68 |   10 |      - |         - |          NA |
| RawGuid_ToString                          | .NET 9.0  | .NET 9.0  |  13.7371 ns | 0.1711 ns | 0.1429 ns |  13.7866 ns |    581.11 |   26.04 |   27 | 0.0057 |      96 B |          NA |
| DomainPrimitives_ToString                 | .NET 9.0  | .NET 9.0  |  13.6820 ns | 0.1524 ns | 0.1351 ns |  13.7433 ns |    578.78 |   25.87 |   27 | 0.0057 |      96 B |          NA |
| Vogen_ToString                            | .NET 9.0  | .NET 9.0  |  15.1740 ns | 0.0308 ns | 0.0289 ns |  15.1689 ns |    641.90 |   28.05 |   28 | 0.0057 |      96 B |          NA |
| StronglyTypedId_ToString                  | .NET 9.0  | .NET 9.0  |  13.9331 ns | 0.0592 ns | 0.0525 ns |  13.9199 ns |    589.40 |   25.83 |   27 | 0.0057 |      96 B |          NA |
| ValueOf_ToString                          | .NET 9.0  | .NET 9.0  |  22.6892 ns | 0.1701 ns | 0.1591 ns |  22.7655 ns |    959.81 |   42.41 |   29 | 0.0076 |     128 B |          NA |
| Meziantou_ToString                        | .NET 9.0  | .NET 9.0  |  34.1620 ns | 0.1291 ns | 0.1144 ns |  34.1695 ns |  1,445.14 |   63.28 |   34 | 0.0148 |     248 B |          NA |
| TinyTypes_ToString                        | .NET 9.0  | .NET 9.0  |  11.0945 ns | 0.1858 ns | 0.1738 ns |  11.1960 ns |    469.32 |   21.69 |   25 | 0.0019 |      32 B |          NA |
| DomainPrimitives_TryParse                 | .NET 9.0  | .NET 9.0  |  27.9826 ns | 0.0504 ns | 0.0421 ns |  27.9787 ns |  1,183.73 |   51.73 |   32 |      - |         - |          NA |
| DomainPrimitives_SpanParse                | .NET 9.0  | .NET 9.0  |  28.0452 ns | 0.0241 ns | 0.0201 ns |  28.0507 ns |  1,186.38 |   51.82 |   32 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanParse            | .NET 9.0  | .NET 9.0  |  60.2880 ns | 0.0687 ns | 0.0537 ns |  60.2770 ns |  2,550.33 |  111.43 |   37 | 0.0038 |      64 B |          NA |
| DomainPrimitives_SpanFormat               | .NET 9.0  | .NET 9.0  |   5.1260 ns | 0.0030 ns | 0.0028 ns |   5.1256 ns |    216.84 |    9.47 |   21 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanFormat           | .NET 9.0  | .NET 9.0  |   4.0684 ns | 0.0020 ns | 0.0017 ns |   4.0677 ns |    172.10 |    7.52 |   18 |      - |         - |          NA |
| StringPrimitive_Email_Create              | .NET 9.0  | .NET 9.0  | 174.3835 ns | 0.2340 ns | 0.2074 ns | 174.3720 ns |  7,376.85 |  322.23 |   46 |      - |         - |          NA |
| StringPrimitive_Email_TryParse            | .NET 9.0  | .NET 9.0  | 179.8502 ns | 0.2645 ns | 0.2345 ns | 179.8209 ns |  7,608.10 |  332.35 |   46 |      - |         - |          NA |
| NumericPrimitive_Money_Create             | .NET 9.0  | .NET 9.0  |   4.8797 ns | 0.0028 ns | 0.0023 ns |   4.8806 ns |    206.42 |    9.02 |   20 |      - |         - |          NA |
| NumericPrimitive_Money_Add                | .NET 9.0  | .NET 9.0  |  10.2878 ns | 0.0054 ns | 0.0045 ns |  10.2881 ns |    435.20 |   19.01 |   24 |      - |         - |          NA |
| ValueObject_Create                        | .NET 9.0  | .NET 9.0  |   0.0005 ns | 0.0005 ns | 0.0004 ns |   0.0005 ns |      0.02 |    0.01 |    1 |      - |         - |          NA |
| SmartEnum_FromValue                       | .NET 9.0  | .NET 9.0  |  15.9367 ns | 0.0348 ns | 0.0272 ns |  15.9330 ns |    674.16 |   29.47 |   28 | 0.0019 |      32 B |          NA |
| RawGuid_JsonSerialize                     | .NET 9.0  | .NET 9.0  | 108.1583 ns | 0.1200 ns | 0.1064 ns | 108.1468 ns |  4,575.36 |  199.83 |   40 | 0.0062 |     104 B |          NA |
| RawGuid_JsonDeserialize                   | .NET 9.0  | .NET 9.0  | 127.3677 ns | 0.1364 ns | 0.1065 ns | 127.3518 ns |  5,387.96 |  235.41 |   43 |      - |         - |          NA |
| DomainPrimitives_JsonSerialize            | .NET 9.0  | .NET 9.0  | 158.9513 ns | 0.3021 ns | 0.2523 ns | 158.8610 ns |  6,724.03 |  293.85 |   45 | 0.0062 |     104 B |          NA |
| DomainPrimitives_JsonDeserialize          | .NET 9.0  | .NET 9.0  | 125.7885 ns | 0.0832 ns | 0.0738 ns | 125.7913 ns |  5,321.16 |  232.37 |   43 |      - |         - |          NA |
| RawGuid_Create                            | .NET 9.0  | .NET 9.0  |   0.0581 ns | 0.0033 ns | 0.0031 ns |   0.0573 ns |      2.46 |    0.17 |    6 |      - |         - |          NA |
| DomainPrimitives_Create                   | .NET 9.0  | .NET 9.0  |   0.9871 ns | 0.0020 ns | 0.0017 ns |   0.9872 ns |     41.76 |    1.83 |   11 |      - |         - |          NA |
| Vogen_Create                              | .NET 9.0  | .NET 9.0  |   0.3752 ns | 0.0014 ns | 0.0011 ns |   0.3748 ns |     15.87 |    0.69 |    9 |      - |         - |          NA |
| StronglyTypedId_Create                    | .NET 9.0  | .NET 9.0  |   0.0248 ns | 0.0024 ns | 0.0023 ns |   0.0247 ns |      1.05 |    0.10 |    5 |      - |         - |          NA |
| ValueOf_Create                            | .NET 9.0  | .NET 9.0  |   8.6042 ns | 0.2502 ns | 0.2569 ns |   8.4347 ns |    363.98 |   19.08 |   24 | 0.0019 |      32 B |          NA |
| Meziantou_Create                          | .NET 9.0  | .NET 9.0  |   0.0282 ns | 0.0031 ns | 0.0029 ns |   0.0283 ns |      1.19 |    0.13 |    5 |      - |         - |          NA |
| TinyTypes_Create                          | .NET 9.0  | .NET 9.0  |   0.0265 ns | 0.0020 ns | 0.0016 ns |   0.0261 ns |      1.12 |    0.08 |    5 |      - |         - |          NA |
| RawGuid_Parse                             | .NET 9.0  | .NET 9.0  |  29.4437 ns | 0.0718 ns | 0.0599 ns |  29.4235 ns |  1,245.54 |   54.45 |   32 |      - |         - |          NA |
| DomainPrimitives_Parse                    | .NET 9.0  | .NET 9.0  |  29.3199 ns | 0.0257 ns | 0.0214 ns |  29.3253 ns |  1,240.30 |   54.18 |   32 |      - |         - |          NA |
| Vogen_Parse                               | .NET 9.0  | .NET 9.0  |  29.6921 ns | 0.0375 ns | 0.0332 ns |  29.6867 ns |  1,256.05 |   54.86 |   32 |      - |         - |          NA |
| StronglyTypedId_Parse                     | .NET 9.0  | .NET 9.0  |  28.3911 ns | 0.0479 ns | 0.0400 ns |  28.3849 ns |  1,201.01 |   52.48 |   32 |      - |         - |          NA |
| ValueOf_Parse                             | .NET 9.0  | .NET 9.0  |  32.2678 ns | 0.0350 ns | 0.0310 ns |  32.2675 ns |  1,365.01 |   59.62 |   33 | 0.0019 |      32 B |          NA |
| Meziantou_Parse                           | .NET 9.0  | .NET 9.0  |  29.5262 ns | 0.0308 ns | 0.0273 ns |  29.5241 ns |  1,249.03 |   54.55 |   32 |      - |         - |          NA |
| TinyTypes_Parse                           | .NET 9.0  | .NET 9.0  |  27.8749 ns | 0.0369 ns | 0.0345 ns |  27.8646 ns |  1,179.18 |   51.50 |   32 |      - |         - |          NA |
| DomainPrimitives_EqualityCheck            | .NET 9.0  | .NET 9.0  |   0.9089 ns | 0.0028 ns | 0.0026 ns |   0.9078 ns |     38.45 |    1.68 |   10 |      - |         - |          NA |
| RawGuid_ToString                          | .NET 9.0  | .NET 9.0  |  13.1575 ns | 0.0173 ns | 0.0144 ns |  13.1596 ns |    556.59 |   24.32 |   27 | 0.0057 |      96 B |          NA |
| DomainPrimitives_ToString                 | .NET 9.0  | .NET 9.0  |  14.0599 ns | 0.1665 ns | 0.1476 ns |  14.1185 ns |    594.77 |   26.66 |   27 | 0.0057 |      96 B |          NA |
| Vogen_ToString                            | .NET 9.0  | .NET 9.0  |  15.0095 ns | 0.1232 ns | 0.1152 ns |  15.0284 ns |    634.94 |   28.12 |   28 | 0.0057 |      96 B |          NA |
| StronglyTypedId_ToString                  | .NET 9.0  | .NET 9.0  |  13.8726 ns | 0.0143 ns | 0.0119 ns |  13.8738 ns |    586.85 |   25.63 |   27 | 0.0057 |      96 B |          NA |
| ValueOf_ToString                          | .NET 9.0  | .NET 9.0  |  22.1517 ns | 0.3218 ns | 0.3010 ns |  22.0952 ns |    937.07 |   42.73 |   29 | 0.0076 |     128 B |          NA |
| Meziantou_ToString                        | .NET 9.0  | .NET 9.0  |  34.8958 ns | 0.1549 ns | 0.1449 ns |  34.8990 ns |  1,476.18 |   64.72 |   34 | 0.0148 |     248 B |          NA |
| TinyTypes_ToString                        | .NET 9.0  | .NET 9.0  |  11.2554 ns | 0.0228 ns | 0.0191 ns |  11.2577 ns |    476.13 |   20.81 |   25 | 0.0019 |      32 B |          NA |
| DomainPrimitives_TryParse                 | .NET 9.0  | .NET 9.0  |  28.0873 ns | 0.0623 ns | 0.0486 ns |  28.0709 ns |  1,188.16 |   51.94 |   32 |      - |         - |          NA |
| DomainPrimitives_SpanParse                | .NET 9.0  | .NET 9.0  |  27.7466 ns | 0.0149 ns | 0.0139 ns |  27.7437 ns |  1,173.75 |   51.25 |   32 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanParse            | .NET 9.0  | .NET 9.0  |  60.4862 ns | 0.0741 ns | 0.0619 ns |  60.4720 ns |  2,558.71 |  111.78 |   37 | 0.0038 |      64 B |          NA |
| DomainPrimitives_SpanFormat               | .NET 9.0  | .NET 9.0  |   5.1297 ns | 0.0037 ns | 0.0033 ns |   5.1288 ns |    217.00 |    9.48 |   21 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanFormat           | .NET 9.0  | .NET 9.0  |   4.4225 ns | 0.0025 ns | 0.0022 ns |   4.4228 ns |    187.08 |    8.17 |   19 |      - |         - |          NA |
| StringPrimitive_Email_Create              | .NET 9.0  | .NET 9.0  | 174.5112 ns | 0.1329 ns | 0.1110 ns | 174.4724 ns |  7,382.25 |  322.45 |   46 |      - |         - |          NA |
| StringPrimitive_Email_TryParse            | .NET 9.0  | .NET 9.0  | 176.0733 ns | 0.2052 ns | 0.1919 ns | 176.0279 ns |  7,448.33 |  325.28 |   46 |      - |         - |          NA |
| NumericPrimitive_Money_Create             | .NET 9.0  | .NET 9.0  |   4.5083 ns | 0.0036 ns | 0.0032 ns |   4.5071 ns |    190.71 |    8.33 |   19 |      - |         - |          NA |
| NumericPrimitive_Money_Add                | .NET 9.0  | .NET 9.0  |   9.9929 ns | 0.0082 ns | 0.0076 ns |   9.9910 ns |    422.72 |   18.46 |   24 |      - |         - |          NA |
| ValueObject_Create                        | .NET 9.0  | .NET 9.0  |   0.0009 ns | 0.0016 ns | 0.0014 ns |   0.0001 ns |      0.04 |    0.06 |    2 |      - |         - |          NA |
| SmartEnum_FromValue                       | .NET 9.0  | .NET 9.0  |  17.3539 ns | 0.2178 ns | 0.2037 ns |  17.3956 ns |    734.11 |   33.12 |   28 | 0.0019 |      32 B |          NA |
| RawGuid_JsonSerialize                     | .NET 9.0  | .NET 9.0  | 116.3068 ns | 0.1965 ns | 0.1641 ns | 116.2908 ns |  4,920.06 |  214.98 |   41 | 0.0062 |     104 B |          NA |
| RawGuid_JsonDeserialize                   | .NET 9.0  | .NET 9.0  | 126.9483 ns | 0.0880 ns | 0.0735 ns | 126.9328 ns |  5,370.22 |  234.56 |   43 |      - |         - |          NA |
| DomainPrimitives_JsonSerialize            | .NET 9.0  | .NET 9.0  | 159.8441 ns | 0.2371 ns | 0.2217 ns | 159.7849 ns |  6,761.79 |  295.35 |   45 | 0.0062 |     104 B |          NA |
| DomainPrimitives_JsonDeserialize          | .NET 9.0  | .NET 9.0  | 125.3582 ns | 0.1689 ns | 0.1498 ns | 125.3003 ns |  5,302.96 |  231.64 |   43 |      - |         - |          NA |
| Dapper_TypeHandler_SetValue               | .NET 10.0 | .NET 10.0 |   0.3522 ns | 0.0006 ns | 0.0005 ns |   0.3521 ns |     14.90 |    0.65 |    8 |      - |         - |          NA |
| Dapper_TypeHandler_Parse                  | .NET 10.0 | .NET 10.0 |   0.9428 ns | 0.0009 ns | 0.0008 ns |   0.9428 ns |     39.88 |    1.74 |   11 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertToProvider   | .NET 10.0 | .NET 10.0 |   1.1682 ns | 0.0120 ns | 0.0112 ns |   1.1646 ns |     49.42 |    2.21 |   13 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertFromProvider | .NET 10.0 | .NET 10.0 |   0.9444 ns | 0.0012 ns | 0.0010 ns |   0.9444 ns |     39.95 |    1.75 |   11 |      - |         - |          NA |
| Dapper_TypeHandler_SetValue               | .NET 8.0  | .NET 8.0  |   8.3842 ns | 0.0070 ns | 0.0062 ns |   8.3836 ns |    354.67 |   15.49 |   24 | 0.0019 |      32 B |          NA |
| Dapper_TypeHandler_Parse                  | .NET 8.0  | .NET 8.0  |   1.3205 ns | 0.0016 ns | 0.0013 ns |   1.3204 ns |     55.86 |    2.44 |   14 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertToProvider   | .NET 8.0  | .NET 8.0  |  12.4444 ns | 0.0150 ns | 0.0140 ns |  12.4400 ns |    526.43 |   22.99 |   26 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertFromProvider | .NET 8.0  | .NET 8.0  |   0.9690 ns | 0.0014 ns | 0.0012 ns |   0.9686 ns |     40.99 |    1.79 |   11 |      - |         - |          NA |
| Dapper_TypeHandler_SetValue               | .NET 9.0  | .NET 9.0  |   1.3842 ns | 0.0071 ns | 0.0067 ns |   1.3846 ns |     58.56 |    2.57 |   15 |      - |         - |          NA |
| Dapper_TypeHandler_Parse                  | .NET 9.0  | .NET 9.0  |   1.2995 ns | 0.0010 ns | 0.0010 ns |   1.2995 ns |     54.97 |    2.40 |   14 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertToProvider   | .NET 9.0  | .NET 9.0  |  23.4489 ns | 0.0028 ns | 0.0024 ns |  23.4486 ns |    991.94 |   43.31 |   30 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertFromProvider | .NET 9.0  | .NET 9.0  |   1.2988 ns | 0.0017 ns | 0.0015 ns |   1.2982 ns |     54.94 |    2.40 |   14 |      - |         - |          NA |
| Dapper_TypeHandler_SetValue               | .NET 9.0  | .NET 9.0  |   1.0467 ns | 0.0045 ns | 0.0042 ns |   1.0450 ns |     44.28 |    1.94 |   12 |      - |         - |          NA |
| Dapper_TypeHandler_Parse                  | .NET 9.0  | .NET 9.0  |   0.9445 ns | 0.0014 ns | 0.0013 ns |   0.9446 ns |     39.96 |    1.75 |   11 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertToProvider   | .NET 9.0  | .NET 9.0  |   1.1624 ns | 0.0085 ns | 0.0080 ns |   1.1597 ns |     49.17 |    2.17 |   13 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertFromProvider | .NET 9.0  | .NET 9.0  |   1.2986 ns | 0.0007 ns | 0.0006 ns |   1.2986 ns |     54.93 |    2.40 |   14 |      - |         - |          NA |
