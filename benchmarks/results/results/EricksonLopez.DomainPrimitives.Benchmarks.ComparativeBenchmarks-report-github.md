```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 7763 3.14GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.401
  [Host]    : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v3
  .NET 8.0  : .NET 8.0.31 (8.0.31, 8.0.3126.42015), X64 RyuJIT x86-64-v3
  .NET 9.0  : .NET 9.0.20 (9.0.20, 9.0.2026.41315), X64 RyuJIT x86-64-v3


```
| Method                                    | Job       | Runtime   | Mean        | Error     | StdDev    | Ratio     | RatioSD | Rank | Gen0   | Allocated | Alloc Ratio |
|------------------------------------------ |---------- |---------- |------------:|----------:|----------:|----------:|--------:|-----:|-------:|----------:|------------:|
| RawGuid_Create                            | .NET 10.0 | .NET 10.0 |   0.3481 ns | 0.0050 ns | 0.0047 ns |     1.472 |    0.03 |    8 |      - |         - |          NA |
| DomainPrimitives_Create                   | .NET 10.0 | .NET 10.0 |   1.0131 ns | 0.0125 ns | 0.0098 ns |     4.285 |    0.08 |   16 |      - |         - |          NA |
| Vogen_Create                              | .NET 10.0 | .NET 10.0 |   0.4793 ns | 0.0166 ns | 0.0155 ns |     2.027 |    0.07 |    9 |      - |         - |          NA |
| StronglyTypedId_Create                    | .NET 10.0 | .NET 10.0 |   0.2024 ns | 0.0037 ns | 0.0029 ns |     0.856 |    0.02 |    5 |      - |         - |          NA |
| ValueOf_Create                            | .NET 10.0 | .NET 10.0 |   8.9333 ns | 0.0359 ns | 0.0300 ns |    37.788 |    0.60 |   32 | 0.0019 |      32 B |          NA |
| Meziantou_Create                          | .NET 10.0 | .NET 10.0 |   0.2383 ns | 0.0048 ns | 0.0045 ns |     1.008 |    0.02 |    6 |      - |         - |          NA |
| TinyTypes_Create                          | .NET 10.0 | .NET 10.0 |   0.3289 ns | 0.0064 ns | 0.0060 ns |     1.391 |    0.03 |    8 |      - |         - |          NA |
| RawGuid_Parse                             | .NET 10.0 | .NET 10.0 |  27.7238 ns | 0.0754 ns | 0.0668 ns |   117.270 |    1.85 |   46 |      - |         - |          NA |
| DomainPrimitives_Parse                    | .NET 10.0 | .NET 10.0 |  27.6771 ns | 0.0337 ns | 0.0299 ns |   117.073 |    1.83 |   46 |      - |         - |          NA |
| Vogen_Parse                               | .NET 10.0 | .NET 10.0 |  28.2582 ns | 0.0527 ns | 0.0440 ns |   119.531 |    1.88 |   46 |      - |         - |          NA |
| StronglyTypedId_Parse                     | .NET 10.0 | .NET 10.0 |  28.6880 ns | 0.0813 ns | 0.0760 ns |   121.349 |    1.92 |   46 |      - |         - |          NA |
| ValueOf_Parse                             | .NET 10.0 | .NET 10.0 |  29.6096 ns | 0.0921 ns | 0.0816 ns |   125.247 |    1.99 |   46 | 0.0019 |      32 B |          NA |
| Meziantou_Parse                           | .NET 10.0 | .NET 10.0 |  43.7825 ns | 0.0384 ns | 0.0340 ns |   185.197 |    2.90 |   50 |      - |         - |          NA |
| TinyTypes_Parse                           | .NET 10.0 | .NET 10.0 |  27.4060 ns | 0.0376 ns | 0.0314 ns |   115.926 |    1.82 |   46 |      - |         - |          NA |
| DomainPrimitives_EqualityCheck            | .NET 10.0 | .NET 10.0 |   1.1301 ns | 0.0106 ns | 0.0099 ns |     4.780 |    0.08 |   17 |      - |         - |          NA |
| RawGuid_ToString                          | .NET 10.0 | .NET 10.0 |   9.9915 ns | 0.0168 ns | 0.0140 ns |    42.263 |    0.66 |   34 | 0.0057 |      96 B |          NA |
| DomainPrimitives_ToString                 | .NET 10.0 | .NET 10.0 |   9.7481 ns | 0.1071 ns | 0.1002 ns |    41.234 |    0.76 |   34 | 0.0057 |      96 B |          NA |
| Vogen_ToString                            | .NET 10.0 | .NET 10.0 |  10.5905 ns | 0.1167 ns | 0.1091 ns |    44.797 |    0.83 |   35 | 0.0057 |      96 B |          NA |
| StronglyTypedId_ToString                  | .NET 10.0 | .NET 10.0 |   9.6937 ns | 0.0365 ns | 0.0323 ns |    41.004 |    0.65 |   34 | 0.0057 |      96 B |          NA |
| ValueOf_ToString                          | .NET 10.0 | .NET 10.0 |  19.6976 ns | 0.0501 ns | 0.0418 ns |    83.320 |    1.31 |   40 | 0.0076 |     128 B |          NA |
| Meziantou_ToString                        | .NET 10.0 | .NET 10.0 |  25.4995 ns | 0.0792 ns | 0.0662 ns |   107.862 |    1.71 |   45 | 0.0148 |     248 B |          NA |
| TinyTypes_ToString                        | .NET 10.0 | .NET 10.0 |   6.8747 ns | 0.1836 ns | 0.1718 ns |    29.079 |    0.84 |   30 | 0.0019 |      32 B |          NA |
| DomainPrimitives_TryParse                 | .NET 10.0 | .NET 10.0 |  27.3047 ns | 0.0603 ns | 0.0564 ns |   115.497 |    1.82 |   46 |      - |         - |          NA |
| DomainPrimitives_SpanParse                | .NET 10.0 | .NET 10.0 |  26.9769 ns | 0.0726 ns | 0.0644 ns |   114.111 |    1.80 |   46 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanParse            | .NET 10.0 | .NET 10.0 |  44.8281 ns | 0.1747 ns | 0.1634 ns |   189.620 |    3.04 |   51 | 0.0038 |      64 B |          NA |
| DomainPrimitives_SpanFormat               | .NET 10.0 | .NET 10.0 |   2.4874 ns | 0.0155 ns | 0.0145 ns |    10.522 |    0.17 |   22 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanFormat           | .NET 10.0 | .NET 10.0 |   2.3795 ns | 0.0071 ns | 0.0067 ns |    10.065 |    0.16 |   21 |      - |         - |          NA |
| StringPrimitive_Email_Create              | .NET 10.0 | .NET 10.0 | 159.7829 ns | 0.1356 ns | 0.1202 ns |   675.872 |   10.57 |   61 |      - |         - |          NA |
| StringPrimitive_Email_TryParse            | .NET 10.0 | .NET 10.0 | 164.9832 ns | 0.1478 ns | 0.1234 ns |   697.869 |   10.92 |   62 |      - |         - |          NA |
| NumericPrimitive_Money_Create             | .NET 10.0 | .NET 10.0 |   3.6430 ns | 0.0039 ns | 0.0032 ns |    15.410 |    0.24 |   23 |      - |         - |          NA |
| NumericPrimitive_Money_Add                | .NET 10.0 | .NET 10.0 |   7.8963 ns | 0.0047 ns | 0.0039 ns |    33.401 |    0.52 |   31 |      - |         - |          NA |
| ValueObject_Create                        | .NET 10.0 | .NET 10.0 |   0.3388 ns | 0.0015 ns | 0.0014 ns |     1.433 |    0.02 |    8 |      - |         - |          NA |
| SmartEnum_FromValue                       | .NET 10.0 | .NET 10.0 |   1.2502 ns | 0.0008 ns | 0.0007 ns |     5.288 |    0.08 |   18 |      - |         - |          NA |
| RawGuid_JsonSerialize                     | .NET 10.0 | .NET 10.0 |  93.3807 ns | 0.8865 ns | 0.8292 ns |   394.995 |    7.04 |   54 | 0.0062 |     104 B |          NA |
| RawGuid_JsonDeserialize                   | .NET 10.0 | .NET 10.0 | 102.2931 ns | 0.2321 ns | 0.2058 ns |   432.694 |    6.81 |   56 |      - |         - |          NA |
| DomainPrimitives_JsonSerialize            | .NET 10.0 | .NET 10.0 |  89.7656 ns | 0.0919 ns | 0.0767 ns |   379.703 |    5.94 |   53 | 0.0062 |     104 B |          NA |
| DomainPrimitives_JsonDeserialize          | .NET 10.0 | .NET 10.0 |  99.2656 ns | 0.0849 ns | 0.0794 ns |   419.888 |    6.57 |   55 |      - |         - |          NA |
| RawGuid_Create                            | .NET 8.0  | .NET 8.0  |   0.2365 ns | 0.0041 ns | 0.0038 ns |     1.000 |    0.02 |    6 |      - |         - |          NA |
| DomainPrimitives_Create                   | .NET 8.0  | .NET 8.0  |   0.8595 ns | 0.0015 ns | 0.0013 ns |     3.636 |    0.06 |   13 |      - |         - |          NA |
| Vogen_Create                              | .NET 8.0  | .NET 8.0  |   5.5641 ns | 0.0030 ns | 0.0028 ns |    23.536 |    0.37 |   29 |      - |         - |          NA |
| StronglyTypedId_Create                    | .NET 8.0  | .NET 8.0  |   0.3336 ns | 0.0014 ns | 0.0013 ns |     1.411 |    0.02 |    8 |      - |         - |          NA |
| ValueOf_Create                            | .NET 8.0  | .NET 8.0  |  13.0265 ns | 0.0288 ns | 0.0269 ns |    55.101 |    0.87 |   37 | 0.0019 |      32 B |          NA |
| Meziantou_Create                          | .NET 8.0  | .NET 8.0  |   0.1689 ns | 0.0053 ns | 0.0047 ns |     0.714 |    0.02 |    4 |      - |         - |          NA |
| TinyTypes_Create                          | .NET 8.0  | .NET 8.0  |   0.3317 ns | 0.0015 ns | 0.0014 ns |     1.403 |    0.02 |    8 |      - |         - |          NA |
| RawGuid_Parse                             | .NET 8.0  | .NET 8.0  |  29.3017 ns | 0.0539 ns | 0.0450 ns |   123.945 |    1.95 |   46 |      - |         - |          NA |
| DomainPrimitives_Parse                    | .NET 8.0  | .NET 8.0  |  28.9429 ns | 0.0660 ns | 0.0618 ns |   122.427 |    1.93 |   46 |      - |         - |          NA |
| Vogen_Parse                               | .NET 8.0  | .NET 8.0  |  33.5605 ns | 0.0996 ns | 0.0883 ns |   141.959 |    2.25 |   48 |      - |         - |          NA |
| StronglyTypedId_Parse                     | .NET 8.0  | .NET 8.0  |  29.2958 ns | 0.0702 ns | 0.0657 ns |   123.920 |    1.95 |   46 |      - |         - |          NA |
| ValueOf_Parse                             | .NET 8.0  | .NET 8.0  |  32.6487 ns | 0.0515 ns | 0.0430 ns |   138.102 |    2.17 |   48 | 0.0019 |      32 B |          NA |
| Meziantou_Parse                           | .NET 8.0  | .NET 8.0  |  29.4032 ns | 0.0749 ns | 0.0585 ns |   124.374 |    1.96 |   46 |      - |         - |          NA |
| TinyTypes_Parse                           | .NET 8.0  | .NET 8.0  |  29.1346 ns | 0.0585 ns | 0.0547 ns |   123.238 |    1.94 |   46 |      - |         - |          NA |
| DomainPrimitives_EqualityCheck            | .NET 8.0  | .NET 8.0  |   0.7925 ns | 0.0051 ns | 0.0048 ns |     3.352 |    0.06 |   11 |      - |         - |          NA |
| RawGuid_ToString                          | .NET 8.0  | .NET 8.0  |  14.0452 ns | 0.0481 ns | 0.0426 ns |    59.410 |    0.94 |   38 | 0.0057 |      96 B |          NA |
| DomainPrimitives_ToString                 | .NET 8.0  | .NET 8.0  |  14.1033 ns | 0.0431 ns | 0.0382 ns |    59.656 |    0.95 |   38 | 0.0057 |      96 B |          NA |
| Vogen_ToString                            | .NET 8.0  | .NET 8.0  |  23.4526 ns | 0.0289 ns | 0.0241 ns |    99.203 |    1.55 |   43 | 0.0057 |      96 B |          NA |
| StronglyTypedId_ToString                  | .NET 8.0  | .NET 8.0  |  14.4492 ns | 0.0427 ns | 0.0399 ns |    61.119 |    0.97 |   38 | 0.0057 |      96 B |          NA |
| ValueOf_ToString                          | .NET 8.0  | .NET 8.0  |  24.8575 ns | 0.0591 ns | 0.0553 ns |   105.146 |    1.66 |   44 | 0.0076 |     128 B |          NA |
| Meziantou_ToString                        | .NET 8.0  | .NET 8.0  |  34.9548 ns | 0.1386 ns | 0.1229 ns |   147.857 |    2.36 |   49 | 0.0148 |     248 B |          NA |
| TinyTypes_ToString                        | .NET 8.0  | .NET 8.0  |   9.2444 ns | 0.0182 ns | 0.0152 ns |    39.103 |    0.61 |   33 | 0.0019 |      32 B |          NA |
| DomainPrimitives_TryParse                 | .NET 8.0  | .NET 8.0  |  28.1388 ns | 0.1171 ns | 0.1095 ns |   119.025 |    1.91 |   46 |      - |         - |          NA |
| DomainPrimitives_SpanParse                | .NET 8.0  | .NET 8.0  |  28.4247 ns | 0.0374 ns | 0.0350 ns |   120.235 |    1.88 |   46 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanParse            | .NET 8.0  | .NET 8.0  |  61.1968 ns | 0.0856 ns | 0.0801 ns |   258.859 |    4.06 |   52 | 0.0038 |      64 B |          NA |
| DomainPrimitives_SpanFormat               | .NET 8.0  | .NET 8.0  |   5.0975 ns | 0.0048 ns | 0.0037 ns |    21.562 |    0.34 |   28 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanFormat           | .NET 8.0  | .NET 8.0  |   4.1846 ns | 0.0019 ns | 0.0015 ns |    17.700 |    0.28 |   26 |      - |         - |          NA |
| StringPrimitive_Email_Create              | .NET 8.0  | .NET 8.0  | 336.0033 ns | 1.2109 ns | 1.1326 ns | 1,421.274 |   22.68 |   63 |      - |         - |          NA |
| StringPrimitive_Email_TryParse            | .NET 8.0  | .NET 8.0  | 331.9862 ns | 0.3097 ns | 0.2746 ns | 1,404.282 |   21.97 |   63 |      - |         - |          NA |
| NumericPrimitive_Money_Create             | .NET 8.0  | .NET 8.0  |   4.3747 ns | 0.0016 ns | 0.0012 ns |    18.505 |    0.29 |   27 |      - |         - |          NA |
| NumericPrimitive_Money_Add                | .NET 8.0  | .NET 8.0  |   9.4931 ns | 0.0044 ns | 0.0034 ns |    40.155 |    0.63 |   34 |      - |         - |          NA |
| ValueObject_Create                        | .NET 8.0  | .NET 8.0  |   0.3552 ns | 0.0021 ns | 0.0019 ns |     1.503 |    0.02 |    8 |      - |         - |          NA |
| SmartEnum_FromValue                       | .NET 8.0  | .NET 8.0  |   3.7304 ns | 0.0066 ns | 0.0055 ns |    15.780 |    0.25 |   24 |      - |         - |          NA |
| RawGuid_JsonSerialize                     | .NET 8.0  | .NET 8.0  | 127.5464 ns | 0.0848 ns | 0.0708 ns |   539.514 |    8.44 |   59 | 0.0062 |     104 B |          NA |
| RawGuid_JsonDeserialize                   | .NET 8.0  | .NET 8.0  | 151.1792 ns | 0.1985 ns | 0.1658 ns |   639.479 |   10.02 |   60 |      - |         - |          NA |
| DomainPrimitives_JsonSerialize            | .NET 8.0  | .NET 8.0  | 129.9267 ns | 0.2689 ns | 0.2384 ns |   549.582 |    8.64 |   59 | 0.0062 |     104 B |          NA |
| DomainPrimitives_JsonDeserialize          | .NET 8.0  | .NET 8.0  | 166.3058 ns | 0.1229 ns | 0.1149 ns |   703.464 |   11.00 |   62 |      - |         - |          NA |
| RawGuid_Create                            | .NET 9.0  | .NET 9.0  |   0.2365 ns | 0.0051 ns | 0.0047 ns |     1.000 |    0.02 |    6 |      - |         - |          NA |
| DomainPrimitives_Create                   | .NET 9.0  | .NET 9.0  |   0.8400 ns | 0.0010 ns | 0.0009 ns |     3.553 |    0.06 |   12 |      - |         - |          NA |
| Vogen_Create                              | .NET 9.0  | .NET 9.0  |   0.4600 ns | 0.0111 ns | 0.0104 ns |     1.946 |    0.05 |    9 |      - |         - |          NA |
| StronglyTypedId_Create                    | .NET 9.0  | .NET 9.0  |   0.3144 ns | 0.0019 ns | 0.0017 ns |     1.330 |    0.02 |    7 |      - |         - |          NA |
| ValueOf_Create                            | .NET 9.0  | .NET 9.0  |  12.8560 ns | 0.2292 ns | 0.2144 ns |    54.380 |    1.22 |   37 | 0.0019 |      32 B |          NA |
| Meziantou_Create                          | .NET 9.0  | .NET 9.0  |   0.2117 ns | 0.0052 ns | 0.0049 ns |     0.895 |    0.02 |    5 |      - |         - |          NA |
| TinyTypes_Create                          | .NET 9.0  | .NET 9.0  |   0.1346 ns | 0.0026 ns | 0.0024 ns |     0.569 |    0.01 |    2 |      - |         - |          NA |
| RawGuid_Parse                             | .NET 9.0  | .NET 9.0  |  28.8696 ns | 0.0576 ns | 0.0481 ns |   122.117 |    1.92 |   46 |      - |         - |          NA |
| DomainPrimitives_Parse                    | .NET 9.0  | .NET 9.0  |  28.6850 ns | 0.0797 ns | 0.0706 ns |   121.336 |    1.92 |   46 |      - |         - |          NA |
| Vogen_Parse                               | .NET 9.0  | .NET 9.0  |  29.0723 ns | 0.0406 ns | 0.0339 ns |   122.974 |    1.93 |   46 |      - |         - |          NA |
| StronglyTypedId_Parse                     | .NET 9.0  | .NET 9.0  |  28.9623 ns | 0.0476 ns | 0.0398 ns |   122.509 |    1.92 |   46 |      - |         - |          NA |
| ValueOf_Parse                             | .NET 9.0  | .NET 9.0  |  31.2103 ns | 0.1874 ns | 0.1565 ns |   132.018 |    2.16 |   47 | 0.0019 |      32 B |          NA |
| Meziantou_Parse                           | .NET 9.0  | .NET 9.0  |  29.0431 ns | 0.0765 ns | 0.0715 ns |   122.851 |    1.94 |   46 |      - |         - |          NA |
| TinyTypes_Parse                           | .NET 9.0  | .NET 9.0  |  28.8706 ns | 0.0478 ns | 0.0447 ns |   122.121 |    1.92 |   46 |      - |         - |          NA |
| DomainPrimitives_EqualityCheck            | .NET 9.0  | .NET 9.0  |   0.9646 ns | 0.0054 ns | 0.0050 ns |     4.080 |    0.07 |   15 |      - |         - |          NA |
| RawGuid_ToString                          | .NET 9.0  | .NET 9.0  |  15.2686 ns | 0.1473 ns | 0.1378 ns |    64.585 |    1.16 |   38 | 0.0057 |      96 B |          NA |
| DomainPrimitives_ToString                 | .NET 9.0  | .NET 9.0  |  15.1262 ns | 0.3007 ns | 0.2665 ns |    63.983 |    1.48 |   38 | 0.0057 |      96 B |          NA |
| Vogen_ToString                            | .NET 9.0  | .NET 9.0  |  16.7640 ns | 0.3241 ns | 0.2873 ns |    70.911 |    1.61 |   39 | 0.0057 |      96 B |          NA |
| StronglyTypedId_ToString                  | .NET 9.0  | .NET 9.0  |  15.6126 ns | 0.3663 ns | 0.3597 ns |    66.040 |    1.80 |   38 | 0.0057 |      96 B |          NA |
| ValueOf_ToString                          | .NET 9.0  | .NET 9.0  |  23.1907 ns | 0.4833 ns | 0.4285 ns |    98.095 |    2.33 |   43 | 0.0076 |     128 B |          NA |
| Meziantou_ToString                        | .NET 9.0  | .NET 9.0  |  37.3982 ns | 0.8198 ns | 1.3697 ns |   158.192 |    6.23 |   49 | 0.0148 |     248 B |          NA |
| TinyTypes_ToString                        | .NET 9.0  | .NET 9.0  |  10.6530 ns | 0.2099 ns | 0.1964 ns |    45.062 |    1.07 |   35 | 0.0019 |      32 B |          NA |
| DomainPrimitives_TryParse                 | .NET 9.0  | .NET 9.0  |  27.7434 ns | 0.0617 ns | 0.0482 ns |   117.353 |    1.84 |   46 |      - |         - |          NA |
| DomainPrimitives_SpanParse                | .NET 9.0  | .NET 9.0  |  27.8919 ns | 0.0847 ns | 0.0707 ns |   117.981 |    1.87 |   46 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanParse            | .NET 9.0  | .NET 9.0  |  59.4268 ns | 0.6386 ns | 0.5661 ns |   251.372 |    4.56 |   52 | 0.0038 |      64 B |          NA |
| DomainPrimitives_SpanFormat               | .NET 9.0  | .NET 9.0  |   4.4448 ns | 0.0042 ns | 0.0039 ns |    18.801 |    0.29 |   27 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanFormat           | .NET 9.0  | .NET 9.0  |   4.0309 ns | 0.0056 ns | 0.0047 ns |    17.050 |    0.27 |   25 |      - |         - |          NA |
| StringPrimitive_Email_Create              | .NET 9.0  | .NET 9.0  | 169.9787 ns | 0.1503 ns | 0.1255 ns |   719.000 |   11.25 |   62 |      - |         - |          NA |
| StringPrimitive_Email_TryParse            | .NET 9.0  | .NET 9.0  | 168.7209 ns | 0.5969 ns | 0.4985 ns |   713.680 |   11.34 |   62 |      - |         - |          NA |
| NumericPrimitive_Money_Create             | .NET 9.0  | .NET 9.0  |   4.1452 ns | 0.0020 ns | 0.0017 ns |    17.534 |    0.27 |   26 |      - |         - |          NA |
| NumericPrimitive_Money_Add                | .NET 9.0  | .NET 9.0  |   8.7771 ns | 0.0065 ns | 0.0054 ns |    37.126 |    0.58 |   32 |      - |         - |          NA |
| ValueObject_Create                        | .NET 9.0  | .NET 9.0  |   0.3135 ns | 0.0016 ns | 0.0015 ns |     1.326 |    0.02 |    7 |      - |         - |          NA |
| SmartEnum_FromValue                       | .NET 9.0  | .NET 9.0  |   2.1134 ns | 0.0054 ns | 0.0048 ns |     8.939 |    0.14 |   20 |      - |         - |          NA |
| RawGuid_JsonSerialize                     | .NET 9.0  | .NET 9.0  | 106.7815 ns | 0.2726 ns | 0.2128 ns |   451.679 |    7.11 |   57 | 0.0062 |     104 B |          NA |
| RawGuid_JsonDeserialize                   | .NET 9.0  | .NET 9.0  | 121.0452 ns | 0.0716 ns | 0.0634 ns |   512.014 |    8.00 |   58 |      - |         - |          NA |
| DomainPrimitives_JsonSerialize            | .NET 9.0  | .NET 9.0  | 108.2633 ns | 0.1652 ns | 0.1465 ns |   457.948 |    7.18 |   57 | 0.0062 |     104 B |          NA |
| DomainPrimitives_JsonDeserialize          | .NET 9.0  | .NET 9.0  | 120.8200 ns | 0.0749 ns | 0.0625 ns |   511.062 |    7.99 |   58 |      - |         - |          NA |
| RawGuid_Create                            | .NET 9.0  | .NET 9.0  |   0.1525 ns | 0.0027 ns | 0.0025 ns |     0.645 |    0.01 |    3 |      - |         - |          NA |
| DomainPrimitives_Create                   | .NET 9.0  | .NET 9.0  |   0.8386 ns | 0.0010 ns | 0.0009 ns |     3.547 |    0.06 |   12 |      - |         - |          NA |
| Vogen_Create                              | .NET 9.0  | .NET 9.0  |   0.5268 ns | 0.0010 ns | 0.0009 ns |     2.228 |    0.04 |   10 |      - |         - |          NA |
| StronglyTypedId_Create                    | .NET 9.0  | .NET 9.0  |   0.2042 ns | 0.0041 ns | 0.0039 ns |     0.864 |    0.02 |    5 |      - |         - |          NA |
| ValueOf_Create                            | .NET 9.0  | .NET 9.0  |   8.7961 ns | 0.2141 ns | 0.2003 ns |    37.207 |    1.01 |   32 | 0.0019 |      32 B |          NA |
| Meziantou_Create                          | .NET 9.0  | .NET 9.0  |   0.1487 ns | 0.0059 ns | 0.0055 ns |     0.629 |    0.02 |    3 |      - |         - |          NA |
| TinyTypes_Create                          | .NET 9.0  | .NET 9.0  |   0.0000 ns | 0.0000 ns | 0.0000 ns |     0.000 |    0.00 |    1 |      - |         - |          NA |
| RawGuid_Parse                             | .NET 9.0  | .NET 9.0  |  28.9701 ns | 0.0904 ns | 0.0755 ns |   122.542 |    1.94 |   46 |      - |         - |          NA |
| DomainPrimitives_Parse                    | .NET 9.0  | .NET 9.0  |  28.6907 ns | 0.0705 ns | 0.0625 ns |   121.360 |    1.91 |   46 |      - |         - |          NA |
| Vogen_Parse                               | .NET 9.0  | .NET 9.0  |  29.6513 ns | 0.0649 ns | 0.0607 ns |   125.423 |    1.98 |   46 |      - |         - |          NA |
| StronglyTypedId_Parse                     | .NET 9.0  | .NET 9.0  |  28.9135 ns | 0.0880 ns | 0.0823 ns |   122.302 |    1.94 |   46 |      - |         - |          NA |
| ValueOf_Parse                             | .NET 9.0  | .NET 9.0  |  33.3665 ns | 0.1557 ns | 0.1381 ns |   141.138 |    2.28 |   48 | 0.0019 |      32 B |          NA |
| Meziantou_Parse                           | .NET 9.0  | .NET 9.0  |  29.2979 ns | 0.0717 ns | 0.0635 ns |   123.928 |    1.95 |   46 |      - |         - |          NA |
| TinyTypes_Parse                           | .NET 9.0  | .NET 9.0  |  28.9081 ns | 0.0730 ns | 0.0610 ns |   122.280 |    1.93 |   46 |      - |         - |          NA |
| DomainPrimitives_EqualityCheck            | .NET 9.0  | .NET 9.0  |   0.9539 ns | 0.0021 ns | 0.0018 ns |     4.035 |    0.06 |   15 |      - |         - |          NA |
| RawGuid_ToString                          | .NET 9.0  | .NET 9.0  |  14.9662 ns | 0.2241 ns | 0.2097 ns |    63.306 |    1.31 |   38 | 0.0057 |      96 B |          NA |
| DomainPrimitives_ToString                 | .NET 9.0  | .NET 9.0  |  14.6354 ns | 0.1664 ns | 0.1299 ns |    61.907 |    1.10 |   38 | 0.0057 |      96 B |          NA |
| Vogen_ToString                            | .NET 9.0  | .NET 9.0  |  20.7785 ns | 0.1095 ns | 0.0914 ns |    87.892 |    1.42 |   41 | 0.0057 |      96 B |          NA |
| StronglyTypedId_ToString                  | .NET 9.0  | .NET 9.0  |  15.0462 ns | 0.2872 ns | 0.2686 ns |    63.644 |    1.48 |   38 | 0.0057 |      96 B |          NA |
| ValueOf_ToString                          | .NET 9.0  | .NET 9.0  |  22.1763 ns | 0.1581 ns | 0.1479 ns |    93.805 |    1.59 |   42 | 0.0076 |     128 B |          NA |
| Meziantou_ToString                        | .NET 9.0  | .NET 9.0  |  35.5614 ns | 0.2341 ns | 0.1955 ns |   150.422 |    2.48 |   49 | 0.0148 |     248 B |          NA |
| TinyTypes_ToString                        | .NET 9.0  | .NET 9.0  |  10.3339 ns | 0.0295 ns | 0.0276 ns |    43.712 |    0.69 |   35 | 0.0019 |      32 B |          NA |
| DomainPrimitives_TryParse                 | .NET 9.0  | .NET 9.0  |  27.8188 ns | 0.0586 ns | 0.0548 ns |   117.672 |    1.85 |   46 |      - |         - |          NA |
| DomainPrimitives_SpanParse                | .NET 9.0  | .NET 9.0  |  27.9149 ns | 0.0676 ns | 0.0633 ns |   118.079 |    1.86 |   46 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanParse            | .NET 9.0  | .NET 9.0  |  60.0717 ns | 0.2070 ns | 0.1936 ns |   254.100 |    4.05 |   52 | 0.0038 |      64 B |          NA |
| DomainPrimitives_SpanFormat               | .NET 9.0  | .NET 9.0  |   4.4540 ns | 0.0054 ns | 0.0048 ns |    18.840 |    0.30 |   27 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanFormat           | .NET 9.0  | .NET 9.0  |   3.7813 ns | 0.0030 ns | 0.0028 ns |    15.995 |    0.25 |   24 |      - |         - |          NA |
| StringPrimitive_Email_Create              | .NET 9.0  | .NET 9.0  | 168.9335 ns | 0.1829 ns | 0.1622 ns |   714.579 |   11.19 |   62 |      - |         - |          NA |
| StringPrimitive_Email_TryParse            | .NET 9.0  | .NET 9.0  | 169.0189 ns | 0.1360 ns | 0.1136 ns |   714.940 |   11.18 |   62 |      - |         - |          NA |
| NumericPrimitive_Money_Create             | .NET 9.0  | .NET 9.0  |   4.1326 ns | 0.0030 ns | 0.0023 ns |    17.481 |    0.27 |   26 |      - |         - |          NA |
| NumericPrimitive_Money_Add                | .NET 9.0  | .NET 9.0  |   8.6989 ns | 0.0069 ns | 0.0054 ns |    36.796 |    0.58 |   32 |      - |         - |          NA |
| ValueObject_Create                        | .NET 9.0  | .NET 9.0  |   0.3107 ns | 0.0008 ns | 0.0007 ns |     1.314 |    0.02 |    7 |      - |         - |          NA |
| SmartEnum_FromValue                       | .NET 9.0  | .NET 9.0  |   2.1057 ns | 0.0022 ns | 0.0019 ns |     8.907 |    0.14 |   20 |      - |         - |          NA |
| RawGuid_JsonSerialize                     | .NET 9.0  | .NET 9.0  | 110.1133 ns | 0.2362 ns | 0.1972 ns |   465.773 |    7.32 |   57 | 0.0062 |     104 B |          NA |
| RawGuid_JsonDeserialize                   | .NET 9.0  | .NET 9.0  | 122.7010 ns | 0.1135 ns | 0.0948 ns |   519.018 |    8.12 |   58 |      - |         - |          NA |
| DomainPrimitives_JsonSerialize            | .NET 9.0  | .NET 9.0  | 109.2796 ns | 0.0887 ns | 0.0740 ns |   462.246 |    7.23 |   57 | 0.0062 |     104 B |          NA |
| DomainPrimitives_JsonDeserialize          | .NET 9.0  | .NET 9.0  | 123.4081 ns | 0.0834 ns | 0.0739 ns |   522.009 |    8.16 |   58 |      - |         - |          NA |
| Dapper_TypeHandler_SetValue               | .NET 10.0 | .NET 10.0 |   0.0000 ns | 0.0000 ns | 0.0000 ns |     0.000 |    0.00 |    1 |      - |         - |          NA |
| Dapper_TypeHandler_Parse                  | .NET 10.0 | .NET 10.0 |   0.9217 ns | 0.0033 ns | 0.0029 ns |     3.899 |    0.06 |   14 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertToProvider   | .NET 10.0 | .NET 10.0 |   1.1476 ns | 0.0063 ns | 0.0056 ns |     4.854 |    0.08 |   17 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertFromProvider | .NET 10.0 | .NET 10.0 |   0.9200 ns | 0.0027 ns | 0.0022 ns |     3.891 |    0.06 |   14 |      - |         - |          NA |
| Dapper_TypeHandler_SetValue               | .NET 8.0  | .NET 8.0  |   7.1132 ns | 0.0299 ns | 0.0280 ns |    30.088 |    0.48 |   30 | 0.0019 |      32 B |          NA |
| Dapper_TypeHandler_Parse                  | .NET 8.0  | .NET 8.0  |   0.8987 ns | 0.0014 ns | 0.0013 ns |     3.802 |    0.06 |   14 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertToProvider   | .NET 8.0  | .NET 8.0  |  11.5055 ns | 0.0094 ns | 0.0083 ns |    48.668 |    0.76 |   36 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertFromProvider | .NET 8.0  | .NET 8.0  |   0.9031 ns | 0.0032 ns | 0.0028 ns |     3.820 |    0.06 |   14 |      - |         - |          NA |
| Dapper_TypeHandler_SetValue               | .NET 9.0  | .NET 9.0  |   0.8316 ns | 0.0040 ns | 0.0036 ns |     3.518 |    0.06 |   12 |      - |         - |          NA |
| Dapper_TypeHandler_Parse                  | .NET 9.0  | .NET 9.0  |   0.9166 ns | 0.0046 ns | 0.0043 ns |     3.877 |    0.06 |   14 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertToProvider   | .NET 9.0  | .NET 9.0  |   1.1689 ns | 0.0062 ns | 0.0058 ns |     4.944 |    0.08 |   17 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertFromProvider | .NET 9.0  | .NET 9.0  |   1.1041 ns | 0.0027 ns | 0.0024 ns |     4.670 |    0.07 |   17 |      - |         - |          NA |
| Dapper_TypeHandler_SetValue               | .NET 9.0  | .NET 9.0  |   1.1594 ns | 0.0059 ns | 0.0055 ns |     4.904 |    0.08 |   17 |      - |         - |          NA |
| Dapper_TypeHandler_Parse                  | .NET 9.0  | .NET 9.0  |   0.9147 ns | 0.0037 ns | 0.0029 ns |     3.869 |    0.06 |   14 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertToProvider   | .NET 9.0  | .NET 9.0  |   1.1672 ns | 0.0066 ns | 0.0062 ns |     4.937 |    0.08 |   17 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertFromProvider | .NET 9.0  | .NET 9.0  |   1.8416 ns | 0.0031 ns | 0.0029 ns |     7.790 |    0.12 |   19 |      - |         - |          NA |
