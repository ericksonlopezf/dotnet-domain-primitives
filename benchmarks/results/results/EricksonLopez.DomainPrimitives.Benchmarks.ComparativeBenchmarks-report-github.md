```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.401
  [Host]    : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v3
  .NET 8.0  : .NET 8.0.31 (8.0.31, 8.0.3126.42015), X64 RyuJIT x86-64-v3
  .NET 9.0  : .NET 9.0.20 (9.0.20, 9.0.2026.41315), X64 RyuJIT x86-64-v3


```
| Method                                    | Job       | Runtime   | Mean        | Error     | StdDev    | Median      | Ratio     | RatioSD | Rank | Gen0   | Allocated | Alloc Ratio |
|------------------------------------------ |---------- |---------- |------------:|----------:|----------:|------------:|----------:|--------:|-----:|-------:|----------:|------------:|
| RawGuid_Create                            | .NET 10.0 | .NET 10.0 |   0.3382 ns | 0.0084 ns | 0.0070 ns |   0.3357 ns |     1.342 |    0.08 |    6 |      - |         - |          NA |
| DomainPrimitives_Create                   | .NET 10.0 | .NET 10.0 |   0.8703 ns | 0.0116 ns | 0.0097 ns |   0.8701 ns |     3.455 |    0.19 |   11 |      - |         - |          NA |
| Vogen_Create                              | .NET 10.0 | .NET 10.0 |   0.3837 ns | 0.0160 ns | 0.0142 ns |   0.3803 ns |     1.523 |    0.10 |    7 |      - |         - |          NA |
| StronglyTypedId_Create                    | .NET 10.0 | .NET 10.0 |   0.1343 ns | 0.0033 ns | 0.0029 ns |   0.1333 ns |     0.533 |    0.03 |    2 |      - |         - |          NA |
| ValueOf_Create                            | .NET 10.0 | .NET 10.0 |  17.9136 ns | 0.9283 ns | 2.7370 ns |  17.7611 ns |    71.112 |   11.49 |   29 | 0.0019 |      32 B |          NA |
| Meziantou_Create                          | .NET 10.0 | .NET 10.0 |   0.3093 ns | 0.0160 ns | 0.0150 ns |   0.3118 ns |     1.228 |    0.09 |    6 |      - |         - |          NA |
| TinyTypes_Create                          | .NET 10.0 | .NET 10.0 |   0.2471 ns | 0.0071 ns | 0.0067 ns |   0.2468 ns |     0.981 |    0.06 |    5 |      - |         - |          NA |
| RawGuid_Parse                             | .NET 10.0 | .NET 10.0 |  27.7105 ns | 0.0744 ns | 0.0621 ns |  27.7282 ns |   110.002 |    5.96 |   32 |      - |         - |          NA |
| DomainPrimitives_Parse                    | .NET 10.0 | .NET 10.0 |  28.9595 ns | 0.2377 ns | 0.2223 ns |  28.9470 ns |   114.961 |    6.28 |   32 |      - |         - |          NA |
| Vogen_Parse                               | .NET 10.0 | .NET 10.0 |  28.2829 ns | 0.0465 ns | 0.0412 ns |  28.2912 ns |   112.275 |    6.08 |   32 |      - |         - |          NA |
| StronglyTypedId_Parse                     | .NET 10.0 | .NET 10.0 |  27.8615 ns | 0.1522 ns | 0.1349 ns |  27.8438 ns |   110.602 |    6.01 |   32 |      - |         - |          NA |
| ValueOf_Parse                             | .NET 10.0 | .NET 10.0 |  30.9750 ns | 0.2095 ns | 0.1857 ns |  31.0031 ns |   122.962 |    6.69 |   32 | 0.0019 |      32 B |          NA |
| Meziantou_Parse                           | .NET 10.0 | .NET 10.0 |  44.6831 ns | 0.3025 ns | 0.2681 ns |  44.6357 ns |   177.379 |    9.65 |   35 |      - |         - |          NA |
| TinyTypes_Parse                           | .NET 10.0 | .NET 10.0 |  27.3966 ns | 0.0920 ns | 0.0816 ns |  27.3522 ns |   108.757 |    5.89 |   32 |      - |         - |          NA |
| DomainPrimitives_EqualityCheck            | .NET 10.0 | .NET 10.0 |   1.1657 ns | 0.0144 ns | 0.0135 ns |   1.1662 ns |     4.628 |    0.26 |   12 |      - |         - |          NA |
| RawGuid_ToString                          | .NET 10.0 | .NET 10.0 |  10.8119 ns | 0.2912 ns | 0.3682 ns |  10.7903 ns |    42.920 |    2.73 |   26 | 0.0057 |      96 B |          NA |
| DomainPrimitives_ToString                 | .NET 10.0 | .NET 10.0 |  11.1914 ns | 0.1903 ns | 0.1780 ns |  11.1918 ns |    44.427 |    2.50 |   26 | 0.0057 |      96 B |          NA |
| Vogen_ToString                            | .NET 10.0 | .NET 10.0 |  12.1484 ns | 0.2990 ns | 0.3199 ns |  12.1455 ns |    48.225 |    2.89 |   27 | 0.0057 |      96 B |          NA |
| StronglyTypedId_ToString                  | .NET 10.0 | .NET 10.0 |  10.6807 ns | 0.2743 ns | 0.2694 ns |  10.6441 ns |    42.399 |    2.52 |   26 | 0.0057 |      96 B |          NA |
| ValueOf_ToString                          | .NET 10.0 | .NET 10.0 |  21.9276 ns | 0.2347 ns | 0.2195 ns |  21.9859 ns |    87.046 |    4.78 |   30 | 0.0076 |     128 B |          NA |
| Meziantou_ToString                        | .NET 10.0 | .NET 10.0 |  29.0373 ns | 0.6354 ns | 0.6240 ns |  29.1121 ns |   115.270 |    6.68 |   32 | 0.0148 |     248 B |          NA |
| TinyTypes_ToString                        | .NET 10.0 | .NET 10.0 |  10.9570 ns | 0.7436 ns | 2.1925 ns |  11.0159 ns |    43.496 |    8.99 |   26 | 0.0019 |      32 B |          NA |
| DomainPrimitives_TryParse                 | .NET 10.0 | .NET 10.0 |  25.9970 ns | 0.0548 ns | 0.0428 ns |  25.9900 ns |   103.201 |    5.59 |   31 |      - |         - |          NA |
| DomainPrimitives_SpanParse                | .NET 10.0 | .NET 10.0 |  25.8474 ns | 0.1469 ns | 0.1374 ns |  25.8835 ns |   102.607 |    5.58 |   31 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanParse            | .NET 10.0 | .NET 10.0 |  46.1411 ns | 0.4110 ns | 0.3644 ns |  46.0313 ns |   183.167 |   10.01 |   35 | 0.0038 |      64 B |          NA |
| DomainPrimitives_SpanFormat               | .NET 10.0 | .NET 10.0 |   2.6712 ns | 0.0286 ns | 0.0254 ns |   2.6753 ns |    10.604 |    0.58 |   15 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanFormat           | .NET 10.0 | .NET 10.0 |   2.4213 ns | 0.0086 ns | 0.0072 ns |   2.4200 ns |     9.612 |    0.52 |   14 |      - |         - |          NA |
| StringPrimitive_Email_Create              | .NET 10.0 | .NET 10.0 | 164.7018 ns | 0.2943 ns | 0.2458 ns | 164.6690 ns |   653.818 |   35.39 |   45 |      - |         - |          NA |
| StringPrimitive_Email_TryParse            | .NET 10.0 | .NET 10.0 | 164.6576 ns | 0.1146 ns | 0.1016 ns | 164.6566 ns |   653.643 |   35.36 |   45 |      - |         - |          NA |
| NumericPrimitive_Money_Create             | .NET 10.0 | .NET 10.0 |   3.6476 ns | 0.0078 ns | 0.0069 ns |   3.6454 ns |    14.480 |    0.78 |   16 |      - |         - |          NA |
| NumericPrimitive_Money_Add                | .NET 10.0 | .NET 10.0 |   7.8993 ns | 0.0079 ns | 0.0070 ns |   7.8975 ns |    31.358 |    1.70 |   23 |      - |         - |          NA |
| ValueObject_Create                        | .NET 10.0 | .NET 10.0 |   0.3254 ns | 0.0095 ns | 0.0088 ns |   0.3272 ns |     1.292 |    0.08 |    6 |      - |         - |          NA |
| SmartEnum_FromValue                       | .NET 10.0 | .NET 10.0 |   0.9760 ns | 0.0104 ns | 0.0087 ns |   0.9775 ns |     3.874 |    0.21 |   11 |      - |         - |          NA |
| RawGuid_JsonSerialize                     | .NET 10.0 | .NET 10.0 |  96.5518 ns | 1.2284 ns | 1.1490 ns |  96.4851 ns |   383.282 |   21.20 |   38 | 0.0062 |     104 B |          NA |
| RawGuid_JsonDeserialize                   | .NET 10.0 | .NET 10.0 | 102.9192 ns | 0.6736 ns | 0.5971 ns | 102.9319 ns |   408.559 |   22.22 |   39 |      - |         - |          NA |
| DomainPrimitives_JsonSerialize            | .NET 10.0 | .NET 10.0 | 141.6499 ns | 0.9531 ns | 0.8449 ns | 141.4821 ns |   562.308 |   30.59 |   43 | 0.0062 |     104 B |          NA |
| DomainPrimitives_JsonDeserialize          | .NET 10.0 | .NET 10.0 | 105.4743 ns | 0.3432 ns | 0.3042 ns | 105.5043 ns |   418.702 |   22.68 |   39 |      - |         - |          NA |
| RawGuid_Create                            | .NET 8.0  | .NET 8.0  |   0.2527 ns | 0.0156 ns | 0.0146 ns |   0.2511 ns |     1.003 |    0.08 |    5 |      - |         - |          NA |
| DomainPrimitives_Create                   | .NET 8.0  | .NET 8.0  |   0.9564 ns | 0.0072 ns | 0.0068 ns |   0.9574 ns |     3.797 |    0.21 |   11 |      - |         - |          NA |
| Vogen_Create                              | .NET 8.0  | .NET 8.0  |   5.5684 ns | 0.0047 ns | 0.0042 ns |   5.5687 ns |    22.105 |    1.20 |   22 |      - |         - |          NA |
| StronglyTypedId_Create                    | .NET 8.0  | .NET 8.0  |   0.1916 ns | 0.0102 ns | 0.0085 ns |   0.1903 ns |     0.761 |    0.05 |    3 |      - |         - |          NA |
| ValueOf_Create                            | .NET 8.0  | .NET 8.0  |  10.1618 ns | 0.0706 ns | 0.0660 ns |  10.1835 ns |    40.339 |    2.20 |   26 | 0.0019 |      32 B |          NA |
| Meziantou_Create                          | .NET 8.0  | .NET 8.0  |   0.1673 ns | 0.0191 ns | 0.0169 ns |   0.1598 ns |     0.664 |    0.07 |    3 |      - |         - |          NA |
| TinyTypes_Create                          | .NET 8.0  | .NET 8.0  |   0.1709 ns | 0.0092 ns | 0.0086 ns |   0.1734 ns |     0.678 |    0.05 |    3 |      - |         - |          NA |
| RawGuid_Parse                             | .NET 8.0  | .NET 8.0  |  29.7793 ns | 0.1925 ns | 0.1801 ns |  29.8082 ns |   118.215 |    6.43 |   32 |      - |         - |          NA |
| DomainPrimitives_Parse                    | .NET 8.0  | .NET 8.0  |  36.0371 ns | 0.1910 ns | 0.1786 ns |  36.0171 ns |   143.057 |    7.77 |   33 |      - |         - |          NA |
| Vogen_Parse                               | .NET 8.0  | .NET 8.0  |  32.6183 ns | 0.0850 ns | 0.0754 ns |  32.6311 ns |   129.485 |    7.01 |   32 |      - |         - |          NA |
| StronglyTypedId_Parse                     | .NET 8.0  | .NET 8.0  |  29.6311 ns | 0.1302 ns | 0.1154 ns |  29.6335 ns |   117.627 |    6.38 |   32 |      - |         - |          NA |
| ValueOf_Parse                             | .NET 8.0  | .NET 8.0  |  33.8954 ns | 0.2120 ns | 0.1983 ns |  33.8530 ns |   134.555 |    7.32 |   32 | 0.0019 |      32 B |          NA |
| Meziantou_Parse                           | .NET 8.0  | .NET 8.0  |  29.2744 ns | 0.0262 ns | 0.0232 ns |  29.2825 ns |   116.211 |    6.29 |   32 |      - |         - |          NA |
| TinyTypes_Parse                           | .NET 8.0  | .NET 8.0  |  29.5430 ns | 0.1548 ns | 0.1448 ns |  29.5629 ns |   117.277 |    6.37 |   32 |      - |         - |          NA |
| DomainPrimitives_EqualityCheck            | .NET 8.0  | .NET 8.0  |   0.7984 ns | 0.0147 ns | 0.0137 ns |   0.8020 ns |     3.169 |    0.18 |   10 |      - |         - |          NA |
| RawGuid_ToString                          | .NET 8.0  | .NET 8.0  |  15.5755 ns | 0.3603 ns | 0.3539 ns |  15.5100 ns |    61.830 |    3.61 |   29 | 0.0057 |      96 B |          NA |
| DomainPrimitives_ToString                 | .NET 8.0  | .NET 8.0  |  16.0031 ns | 0.3839 ns | 0.4108 ns |  15.9718 ns |    63.528 |    3.79 |   29 | 0.0057 |      96 B |          NA |
| Vogen_ToString                            | .NET 8.0  | .NET 8.0  |  25.8253 ns | 0.4991 ns | 0.4669 ns |  25.8374 ns |   102.519 |    5.83 |   31 | 0.0057 |      96 B |          NA |
| StronglyTypedId_ToString                  | .NET 8.0  | .NET 8.0  |  16.7796 ns | 0.4049 ns | 0.3977 ns |  16.8665 ns |    66.610 |    3.92 |   29 | 0.0057 |      96 B |          NA |
| ValueOf_ToString                          | .NET 8.0  | .NET 8.0  |  24.9425 ns | 0.5409 ns | 0.5313 ns |  25.1083 ns |    99.014 |    5.73 |   31 | 0.0076 |     128 B |          NA |
| Meziantou_ToString                        | .NET 8.0  | .NET 8.0  |  39.1003 ns | 0.8468 ns | 0.7921 ns |  39.1849 ns |   155.217 |    8.93 |   34 | 0.0148 |     248 B |          NA |
| TinyTypes_ToString                        | .NET 8.0  | .NET 8.0  |  10.5585 ns | 0.1481 ns | 0.1385 ns |  10.5745 ns |    41.914 |    2.33 |   26 | 0.0019 |      32 B |          NA |
| DomainPrimitives_TryParse                 | .NET 8.0  | .NET 8.0  |  26.5972 ns | 0.0396 ns | 0.0370 ns |  26.5872 ns |   105.583 |    5.71 |   31 |      - |         - |          NA |
| DomainPrimitives_SpanParse                | .NET 8.0  | .NET 8.0  |  26.5298 ns | 0.1009 ns | 0.0895 ns |  26.5154 ns |   105.316 |    5.71 |   31 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanParse            | .NET 8.0  | .NET 8.0  |  59.9569 ns | 0.5369 ns | 0.4759 ns |  60.0568 ns |   238.011 |   13.00 |   36 | 0.0038 |      64 B |          NA |
| DomainPrimitives_SpanFormat               | .NET 8.0  | .NET 8.0  |   5.1153 ns | 0.0517 ns | 0.0484 ns |   5.1264 ns |    20.306 |    1.11 |   21 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanFormat           | .NET 8.0  | .NET 8.0  |   4.4981 ns | 0.0299 ns | 0.0280 ns |   4.4909 ns |    17.856 |    0.97 |   19 |      - |         - |          NA |
| StringPrimitive_Email_Create              | .NET 8.0  | .NET 8.0  | 330.0972 ns | 2.9912 ns | 2.7979 ns | 329.9382 ns | 1,310.389 |   71.69 |   47 |      - |         - |          NA |
| StringPrimitive_Email_TryParse            | .NET 8.0  | .NET 8.0  | 324.6042 ns | 0.7686 ns | 0.6813 ns | 324.3698 ns | 1,288.584 |   69.76 |   47 |      - |         - |          NA |
| NumericPrimitive_Money_Create             | .NET 8.0  | .NET 8.0  |   4.3827 ns | 0.0042 ns | 0.0035 ns |   4.3821 ns |    17.398 |    0.94 |   19 |      - |         - |          NA |
| NumericPrimitive_Money_Add                | .NET 8.0  | .NET 8.0  |   9.4895 ns | 0.0075 ns | 0.0063 ns |   9.4886 ns |    37.671 |    2.04 |   25 |      - |         - |          NA |
| ValueObject_Create                        | .NET 8.0  | .NET 8.0  |   0.3313 ns | 0.0019 ns | 0.0016 ns |   0.3310 ns |     1.315 |    0.07 |    6 |      - |         - |          NA |
| SmartEnum_FromValue                       | .NET 8.0  | .NET 8.0  |  14.4748 ns | 0.1485 ns | 0.1389 ns |  14.4534 ns |    57.461 |    3.15 |   28 | 0.0019 |      32 B |          NA |
| RawGuid_JsonSerialize                     | .NET 8.0  | .NET 8.0  | 135.7164 ns | 1.0157 ns | 0.9501 ns | 135.5785 ns |   538.754 |   29.37 |   42 | 0.0062 |     104 B |          NA |
| RawGuid_JsonDeserialize                   | .NET 8.0  | .NET 8.0  | 159.8444 ns | 0.8824 ns | 0.8254 ns | 159.6292 ns |   634.536 |   34.47 |   44 |      - |         - |          NA |
| DomainPrimitives_JsonSerialize            | .NET 8.0  | .NET 8.0  | 198.7955 ns | 1.1750 ns | 1.0416 ns | 198.5270 ns |   789.160 |   42.88 |   46 | 0.0062 |     104 B |          NA |
| DomainPrimitives_JsonDeserialize          | .NET 8.0  | .NET 8.0  | 168.0304 ns | 1.4403 ns | 1.3473 ns | 168.0841 ns |   667.031 |   36.45 |   45 |      - |         - |          NA |
| RawGuid_Create                            | .NET 9.0  | .NET 9.0  |   0.2163 ns | 0.0065 ns | 0.0061 ns |   0.2147 ns |     0.859 |    0.05 |    4 |      - |         - |          NA |
| DomainPrimitives_Create                   | .NET 9.0  | .NET 9.0  |   1.0171 ns | 0.0149 ns | 0.0125 ns |   1.0140 ns |     4.038 |    0.22 |   11 |      - |         - |          NA |
| Vogen_Create                              | .NET 9.0  | .NET 9.0  |   0.5474 ns | 0.0018 ns | 0.0016 ns |   0.5471 ns |     2.173 |    0.12 |    8 |      - |         - |          NA |
| StronglyTypedId_Create                    | .NET 9.0  | .NET 9.0  |   0.1730 ns | 0.0071 ns | 0.0063 ns |   0.1733 ns |     0.687 |    0.04 |    3 |      - |         - |          NA |
| ValueOf_Create                            | .NET 9.0  | .NET 9.0  |   9.3206 ns | 0.2064 ns | 0.1931 ns |   9.3643 ns |    37.000 |    2.13 |   25 | 0.0019 |      32 B |          NA |
| Meziantou_Create                          | .NET 9.0  | .NET 9.0  |   0.1636 ns | 0.0054 ns | 0.0045 ns |   0.1639 ns |     0.650 |    0.04 |    3 |      - |         - |          NA |
| TinyTypes_Create                          | .NET 9.0  | .NET 9.0  |  14.4091 ns | 0.0048 ns | 0.0040 ns |  14.4087 ns |    57.200 |    3.09 |   28 |      - |         - |          NA |
| RawGuid_Parse                             | .NET 9.0  | .NET 9.0  |  30.0271 ns | 0.2099 ns | 0.1861 ns |  30.0060 ns |   119.199 |    6.49 |   32 |      - |         - |          NA |
| DomainPrimitives_Parse                    | .NET 9.0  | .NET 9.0  |  29.7629 ns | 0.2577 ns | 0.2284 ns |  29.8071 ns |   118.150 |    6.45 |   32 |      - |         - |          NA |
| Vogen_Parse                               | .NET 9.0  | .NET 9.0  |  29.3997 ns | 0.2219 ns | 0.1967 ns |  29.3566 ns |   116.708 |    6.36 |   32 |      - |         - |          NA |
| StronglyTypedId_Parse                     | .NET 9.0  | .NET 9.0  |  29.2305 ns | 0.3001 ns | 0.2807 ns |  29.2978 ns |   116.037 |    6.37 |   32 |      - |         - |          NA |
| ValueOf_Parse                             | .NET 9.0  | .NET 9.0  |  32.1073 ns | 0.1810 ns | 0.1693 ns |  32.1130 ns |   127.457 |    6.92 |   32 | 0.0019 |      32 B |          NA |
| Meziantou_Parse                           | .NET 9.0  | .NET 9.0  |  29.3802 ns | 0.1490 ns | 0.1321 ns |  29.3532 ns |   116.631 |    6.33 |   32 |      - |         - |          NA |
| TinyTypes_Parse                           | .NET 9.0  | .NET 9.0  |  29.3598 ns | 0.2170 ns | 0.1924 ns |  29.2873 ns |   116.550 |    6.35 |   32 |      - |         - |          NA |
| DomainPrimitives_EqualityCheck            | .NET 9.0  | .NET 9.0  |   1.0066 ns | 0.0148 ns | 0.0138 ns |   1.0055 ns |     3.996 |    0.22 |   11 |      - |         - |          NA |
| RawGuid_ToString                          | .NET 9.0  | .NET 9.0  |  15.6466 ns | 0.3742 ns | 0.4003 ns |  15.5760 ns |    62.112 |    3.70 |   29 | 0.0057 |      96 B |          NA |
| DomainPrimitives_ToString                 | .NET 9.0  | .NET 9.0  |  16.8844 ns | 0.4093 ns | 0.4872 ns |  16.8569 ns |    67.026 |    4.09 |   29 | 0.0057 |      96 B |          NA |
| Vogen_ToString                            | .NET 9.0  | .NET 9.0  |  18.8952 ns | 0.4380 ns | 0.3883 ns |  18.8633 ns |    75.008 |    4.32 |   29 | 0.0057 |      96 B |          NA |
| StronglyTypedId_ToString                  | .NET 9.0  | .NET 9.0  |  17.2483 ns | 0.3891 ns | 0.3640 ns |  17.3493 ns |    68.471 |    3.96 |   29 | 0.0057 |      96 B |          NA |
| ValueOf_ToString                          | .NET 9.0  | .NET 9.0  |  24.2102 ns | 0.5500 ns | 1.2525 ns |  24.0356 ns |    96.107 |    7.17 |   31 | 0.0076 |     128 B |          NA |
| Meziantou_ToString                        | .NET 9.0  | .NET 9.0  |  37.8789 ns | 0.8102 ns | 0.9331 ns |  37.8195 ns |   150.368 |    8.90 |   34 | 0.0148 |     248 B |          NA |
| TinyTypes_ToString                        | .NET 9.0  | .NET 9.0  |  11.1985 ns | 0.1061 ns | 0.0992 ns |  11.2174 ns |    44.455 |    2.43 |   26 | 0.0019 |      32 B |          NA |
| DomainPrimitives_TryParse                 | .NET 9.0  | .NET 9.0  |  26.2740 ns | 0.0194 ns | 0.0162 ns |  26.2767 ns |   104.300 |    5.64 |   31 |      - |         - |          NA |
| DomainPrimitives_SpanParse                | .NET 9.0  | .NET 9.0  |  26.3006 ns | 0.2637 ns | 0.2467 ns |  26.1767 ns |   104.406 |    5.73 |   31 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanParse            | .NET 9.0  | .NET 9.0  |  63.4667 ns | 0.6686 ns | 0.6254 ns |  63.1772 ns |   251.944 |   13.84 |   37 | 0.0038 |      64 B |          NA |
| DomainPrimitives_SpanFormat               | .NET 9.0  | .NET 9.0  |   4.7219 ns | 0.0471 ns | 0.0440 ns |   4.7214 ns |    18.744 |    1.03 |   20 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanFormat           | .NET 9.0  | .NET 9.0  |   3.7563 ns | 0.0073 ns | 0.0061 ns |   3.7550 ns |    14.911 |    0.81 |   17 |      - |         - |          NA |
| StringPrimitive_Email_Create              | .NET 9.0  | .NET 9.0  | 167.3326 ns | 1.0227 ns | 0.9066 ns | 166.9330 ns |   664.261 |   36.10 |   45 |      - |         - |          NA |
| StringPrimitive_Email_TryParse            | .NET 9.0  | .NET 9.0  | 173.1821 ns | 1.3722 ns | 1.1458 ns | 173.4657 ns |   687.482 |   37.46 |   45 |      - |         - |          NA |
| NumericPrimitive_Money_Create             | .NET 9.0  | .NET 9.0  |   4.2533 ns | 0.0346 ns | 0.0324 ns |   4.2476 ns |    16.884 |    0.92 |   18 |      - |         - |          NA |
| NumericPrimitive_Money_Add                | .NET 9.0  | .NET 9.0  |   8.8450 ns | 0.0601 ns | 0.0562 ns |   8.8229 ns |    35.112 |    1.91 |   24 |      - |         - |          NA |
| ValueObject_Create                        | .NET 9.0  | .NET 9.0  |   0.0009 ns | 0.0016 ns | 0.0014 ns |   0.0000 ns |     0.003 |    0.01 |    1 |      - |         - |          NA |
| SmartEnum_FromValue                       | .NET 9.0  | .NET 9.0  |  15.4689 ns | 0.1511 ns | 0.1413 ns |  15.4928 ns |    61.407 |    3.37 |   29 | 0.0019 |      32 B |          NA |
| RawGuid_JsonSerialize                     | .NET 9.0  | .NET 9.0  | 111.8361 ns | 1.5127 ns | 1.4150 ns | 111.5344 ns |   443.956 |   24.62 |   40 | 0.0062 |     104 B |          NA |
| RawGuid_JsonDeserialize                   | .NET 9.0  | .NET 9.0  | 122.1818 ns | 0.9443 ns | 0.8833 ns | 122.1693 ns |   485.026 |   26.45 |   41 |      - |         - |          NA |
| DomainPrimitives_JsonSerialize            | .NET 9.0  | .NET 9.0  | 157.3967 ns | 1.2098 ns | 1.1316 ns | 157.0473 ns |   624.819 |   34.08 |   44 | 0.0062 |     104 B |          NA |
| DomainPrimitives_JsonDeserialize          | .NET 9.0  | .NET 9.0  | 123.3342 ns | 1.6671 ns | 1.5594 ns | 123.3636 ns |   489.601 |   27.15 |   41 |      - |         - |          NA |
| RawGuid_Create                            | .NET 9.0  | .NET 9.0  |   0.1494 ns | 0.0099 ns | 0.0088 ns |   0.1458 ns |     0.593 |    0.05 |    3 |      - |         - |          NA |
| DomainPrimitives_Create                   | .NET 9.0  | .NET 9.0  |   0.8960 ns | 0.0191 ns | 0.0160 ns |   0.8916 ns |     3.557 |    0.20 |   11 |      - |         - |          NA |
| Vogen_Create                              | .NET 9.0  | .NET 9.0  |   0.6510 ns | 0.0130 ns | 0.0122 ns |   0.6492 ns |     2.584 |    0.15 |    9 |      - |         - |          NA |
| StronglyTypedId_Create                    | .NET 9.0  | .NET 9.0  |   0.1765 ns | 0.0060 ns | 0.0050 ns |   0.1772 ns |     0.701 |    0.04 |    3 |      - |         - |          NA |
| ValueOf_Create                            | .NET 9.0  | .NET 9.0  |   9.3048 ns | 0.1394 ns | 0.1304 ns |   9.3281 ns |    36.937 |    2.06 |   25 | 0.0019 |      32 B |          NA |
| Meziantou_Create                          | .NET 9.0  | .NET 9.0  |   0.1721 ns | 0.0095 ns | 0.0089 ns |   0.1734 ns |     0.683 |    0.05 |    3 |      - |         - |          NA |
| TinyTypes_Create                          | .NET 9.0  | .NET 9.0  |   0.1691 ns | 0.0078 ns | 0.0073 ns |   0.1701 ns |     0.671 |    0.05 |    3 |      - |         - |          NA |
| RawGuid_Parse                             | .NET 9.0  | .NET 9.0  |  28.9310 ns | 0.0760 ns | 0.0634 ns |  28.9290 ns |   114.847 |    6.22 |   32 |      - |         - |          NA |
| DomainPrimitives_Parse                    | .NET 9.0  | .NET 9.0  |  29.4722 ns | 0.0509 ns | 0.0451 ns |  29.4580 ns |   116.996 |    6.33 |   32 |      - |         - |          NA |
| Vogen_Parse                               | .NET 9.0  | .NET 9.0  |  29.5154 ns | 0.0540 ns | 0.0478 ns |  29.5150 ns |   117.167 |    6.34 |   32 |      - |         - |          NA |
| StronglyTypedId_Parse                     | .NET 9.0  | .NET 9.0  |  28.5785 ns | 0.0198 ns | 0.0185 ns |  28.5803 ns |   113.448 |    6.14 |   32 |      - |         - |          NA |
| ValueOf_Parse                             | .NET 9.0  | .NET 9.0  |  33.2797 ns | 0.3199 ns | 0.2992 ns |  33.2829 ns |   132.111 |    7.24 |   32 | 0.0019 |      32 B |          NA |
| Meziantou_Parse                           | .NET 9.0  | .NET 9.0  |  28.8583 ns | 0.0784 ns | 0.0612 ns |  28.8410 ns |   114.559 |    6.20 |   32 |      - |         - |          NA |
| TinyTypes_Parse                           | .NET 9.0  | .NET 9.0  |  29.3724 ns | 0.2017 ns | 0.1788 ns |  29.3370 ns |   116.600 |    6.35 |   32 |      - |         - |          NA |
| DomainPrimitives_EqualityCheck            | .NET 9.0  | .NET 9.0  |   0.9502 ns | 0.0021 ns | 0.0017 ns |   0.9506 ns |     3.772 |    0.20 |   11 |      - |         - |          NA |
| RawGuid_ToString                          | .NET 9.0  | .NET 9.0  |  15.7895 ns | 0.3123 ns | 0.2608 ns |  15.8136 ns |    62.680 |    3.54 |   29 | 0.0057 |      96 B |          NA |
| DomainPrimitives_ToString                 | .NET 9.0  | .NET 9.0  |  15.8339 ns | 0.3776 ns | 0.5415 ns |  16.0306 ns |    62.856 |    4.00 |   29 | 0.0057 |      96 B |          NA |
| Vogen_ToString                            | .NET 9.0  | .NET 9.0  |  17.0048 ns | 0.2564 ns | 0.2398 ns |  17.0098 ns |    67.504 |    3.77 |   29 | 0.0057 |      96 B |          NA |
| StronglyTypedId_ToString                  | .NET 9.0  | .NET 9.0  |  17.7214 ns | 0.4149 ns | 0.5095 ns |  17.7541 ns |    70.349 |    4.29 |   29 | 0.0057 |      96 B |          NA |
| ValueOf_ToString                          | .NET 9.0  | .NET 9.0  |  32.0355 ns | 0.7092 ns | 0.9222 ns |  31.9503 ns |   127.171 |    7.76 |   32 | 0.0076 |     128 B |          NA |
| Meziantou_ToString                        | .NET 9.0  | .NET 9.0  |  40.3602 ns | 0.8598 ns | 0.8043 ns |  40.4675 ns |   160.218 |    9.20 |   34 | 0.0148 |     248 B |          NA |
| TinyTypes_ToString                        | .NET 9.0  | .NET 9.0  |  11.3551 ns | 0.1979 ns | 0.1851 ns |  11.3417 ns |    45.076 |    2.54 |   26 | 0.0019 |      32 B |          NA |
| DomainPrimitives_TryParse                 | .NET 9.0  | .NET 9.0  |  26.6066 ns | 0.1720 ns | 0.1609 ns |  26.6127 ns |   105.621 |    5.75 |   31 |      - |         - |          NA |
| DomainPrimitives_SpanParse                | .NET 9.0  | .NET 9.0  |  26.1233 ns | 0.0626 ns | 0.0523 ns |  26.1012 ns |   103.702 |    5.61 |   31 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanParse            | .NET 9.0  | .NET 9.0  |  60.9826 ns | 0.5154 ns | 0.4821 ns |  60.8469 ns |   242.083 |   13.22 |   36 | 0.0038 |      64 B |          NA |
| DomainPrimitives_SpanFormat               | .NET 9.0  | .NET 9.0  |   4.4896 ns | 0.0493 ns | 0.0461 ns |   4.4829 ns |    17.822 |    0.98 |   19 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanFormat           | .NET 9.0  | .NET 9.0  |   3.7582 ns | 0.0031 ns | 0.0024 ns |   3.7577 ns |    14.919 |    0.81 |   17 |      - |         - |          NA |
| StringPrimitive_Email_Create              | .NET 9.0  | .NET 9.0  | 166.7745 ns | 0.9454 ns | 0.8843 ns | 166.8452 ns |   662.046 |   35.97 |   45 |      - |         - |          NA |
| StringPrimitive_Email_TryParse            | .NET 9.0  | .NET 9.0  | 169.3939 ns | 0.3416 ns | 0.3028 ns | 169.2972 ns |   672.444 |   36.40 |   45 |      - |         - |          NA |
| NumericPrimitive_Money_Create             | .NET 9.0  | .NET 9.0  |   4.1675 ns | 0.0081 ns | 0.0063 ns |   4.1658 ns |    16.544 |    0.90 |   18 |      - |         - |          NA |
| NumericPrimitive_Money_Add                | .NET 9.0  | .NET 9.0  |   8.7525 ns | 0.1020 ns | 0.0955 ns |   8.6824 ns |    34.745 |    1.91 |   24 |      - |         - |          NA |
| ValueObject_Create                        | .NET 9.0  | .NET 9.0  |   0.0000 ns | 0.0001 ns | 0.0001 ns |   0.0000 ns |     0.000 |    0.00 |    1 |      - |         - |          NA |
| SmartEnum_FromValue                       | .NET 9.0  | .NET 9.0  |  15.7416 ns | 0.2194 ns | 0.1945 ns |  15.6594 ns |    62.489 |    3.46 |   29 | 0.0019 |      32 B |          NA |
| RawGuid_JsonSerialize                     | .NET 9.0  | .NET 9.0  | 110.3020 ns | 0.3770 ns | 0.3342 ns | 110.3531 ns |   437.867 |   23.72 |   40 | 0.0062 |     104 B |          NA |
| RawGuid_JsonDeserialize                   | .NET 9.0  | .NET 9.0  | 122.5750 ns | 0.1864 ns | 0.1652 ns | 122.5258 ns |   486.587 |   26.33 |   41 |      - |         - |          NA |
| DomainPrimitives_JsonSerialize            | .NET 9.0  | .NET 9.0  | 169.5850 ns | 1.0686 ns | 0.9996 ns | 169.3467 ns |   673.203 |   36.62 |   45 | 0.0062 |     104 B |          NA |
| DomainPrimitives_JsonDeserialize          | .NET 9.0  | .NET 9.0  | 122.3745 ns | 0.5451 ns | 0.4552 ns | 122.5637 ns |   485.791 |   26.34 |   41 |      - |         - |          NA |
| Dapper_TypeHandler_SetValue               | .NET 10.0 | .NET 10.0 |   0.3448 ns | 0.0104 ns | 0.0087 ns |   0.3446 ns |     1.369 |    0.08 |    6 |      - |         - |          NA |
| Dapper_TypeHandler_Parse                  | .NET 10.0 | .NET 10.0 |   0.8982 ns | 0.0115 ns | 0.0107 ns |   0.8981 ns |     3.566 |    0.20 |   11 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertToProvider   | .NET 10.0 | .NET 10.0 |   1.5192 ns | 0.0176 ns | 0.0165 ns |   1.5199 ns |     6.031 |    0.33 |   13 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertFromProvider | .NET 10.0 | .NET 10.0 |   0.8793 ns | 0.0027 ns | 0.0024 ns |   0.8803 ns |     3.490 |    0.19 |   11 |      - |         - |          NA |
| Dapper_TypeHandler_SetValue               | .NET 8.0  | .NET 8.0  |   9.3076 ns | 0.0739 ns | 0.0655 ns |   9.3038 ns |    36.948 |    2.01 |   25 | 0.0019 |      32 B |          NA |
| Dapper_TypeHandler_Parse                  | .NET 8.0  | .NET 8.0  |   0.9405 ns | 0.0029 ns | 0.0022 ns |   0.9401 ns |     3.734 |    0.20 |   11 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertToProvider   | .NET 8.0  | .NET 8.0  |  11.1317 ns | 0.0162 ns | 0.0151 ns |  11.1304 ns |    44.190 |    2.39 |   26 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertFromProvider | .NET 8.0  | .NET 8.0  |   0.9604 ns | 0.0100 ns | 0.0093 ns |   0.9596 ns |     3.812 |    0.21 |   11 |      - |         - |          NA |
| Dapper_TypeHandler_SetValue               | .NET 9.0  | .NET 9.0  |   1.0148 ns | 0.0016 ns | 0.0015 ns |   1.0149 ns |     4.029 |    0.22 |   11 |      - |         - |          NA |
| Dapper_TypeHandler_Parse                  | .NET 9.0  | .NET 9.0  |   0.9004 ns | 0.0042 ns | 0.0037 ns |   0.9001 ns |     3.574 |    0.19 |   11 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertToProvider   | .NET 9.0  | .NET 9.0  |   1.1535 ns | 0.0069 ns | 0.0065 ns |   1.1531 ns |     4.579 |    0.25 |   12 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertFromProvider | .NET 9.0  | .NET 9.0  |   1.1122 ns | 0.0113 ns | 0.0106 ns |   1.1130 ns |     4.415 |    0.24 |   11 |      - |         - |          NA |
| Dapper_TypeHandler_SetValue               | .NET 9.0  | .NET 9.0  |   1.0573 ns | 0.0180 ns | 0.0168 ns |   1.0500 ns |     4.197 |    0.24 |   11 |      - |         - |          NA |
| Dapper_TypeHandler_Parse                  | .NET 9.0  | .NET 9.0  |   1.0808 ns | 0.0021 ns | 0.0019 ns |   1.0802 ns |     4.291 |    0.23 |   11 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertToProvider   | .NET 9.0  | .NET 9.0  |   0.8587 ns | 0.0107 ns | 0.0095 ns |   0.8585 ns |     3.409 |    0.19 |   11 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertFromProvider | .NET 9.0  | .NET 9.0  |   0.9142 ns | 0.0127 ns | 0.0119 ns |   0.9150 ns |     3.629 |    0.20 |   11 |      - |         - |          NA |
