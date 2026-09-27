
BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.401
  [Host]    : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v3
  .NET 8.0  : .NET 8.0.31 (8.0.31, 8.0.3126.42015), X64 RyuJIT x86-64-v3
  .NET 9.0  : .NET 9.0.20 (9.0.20, 9.0.2026.41315), X64 RyuJIT x86-64-v3


 Method                                    | Job       | Runtime   | Mean        | Error     | StdDev    | Ratio     | RatioSD | Rank | Gen0   | Allocated | Alloc Ratio |
------------------------------------------ |---------- |---------- |------------:|----------:|----------:|----------:|--------:|-----:|-------:|----------:|------------:|
 RawGuid_Create                            | .NET 10.0 | .NET 10.0 |   0.3452 ns | 0.0031 ns | 0.0025 ns |     1.470 |    0.03 |    5 |      - |         - |          NA |
 DomainPrimitives_Create                   | .NET 10.0 | .NET 10.0 |   0.8383 ns | 0.0011 ns | 0.0010 ns |     3.570 |    0.06 |    9 |      - |         - |          NA |
 Vogen_Create                              | .NET 10.0 | .NET 10.0 |   0.4438 ns | 0.0117 ns | 0.0109 ns |     1.890 |    0.05 |    6 |      - |         - |          NA |
 StronglyTypedId_Create                    | .NET 10.0 | .NET 10.0 |   0.1489 ns | 0.0044 ns | 0.0039 ns |     0.634 |    0.02 |    2 |      - |         - |          NA |
 ValueOf_Create                            | .NET 10.0 | .NET 10.0 |  11.6411 ns | 0.5520 ns | 1.5927 ns |    49.575 |    6.80 |   30 | 0.0019 |      32 B |          NA |
 Meziantou_Create                          | .NET 10.0 | .NET 10.0 |   0.2410 ns | 0.0041 ns | 0.0036 ns |     1.027 |    0.02 |    4 |      - |         - |          NA |
 TinyTypes_Create                          | .NET 10.0 | .NET 10.0 |   0.2459 ns | 0.0066 ns | 0.0061 ns |     1.047 |    0.03 |    4 |      - |         - |          NA |
 RawGuid_Parse                             | .NET 10.0 | .NET 10.0 |  27.9131 ns | 0.0353 ns | 0.0331 ns |   118.872 |    1.90 |   33 |      - |         - |          NA |
 DomainPrimitives_Parse                    | .NET 10.0 | .NET 10.0 |  27.5631 ns | 0.0346 ns | 0.0289 ns |   117.381 |    1.87 |   33 |      - |         - |          NA |
 Vogen_Parse                               | .NET 10.0 | .NET 10.0 |  28.3400 ns | 0.0454 ns | 0.0403 ns |   120.690 |    1.93 |   33 |      - |         - |          NA |
 StronglyTypedId_Parse                     | .NET 10.0 | .NET 10.0 |  27.7464 ns | 0.0404 ns | 0.0337 ns |   118.162 |    1.88 |   33 |      - |         - |          NA |
 ValueOf_Parse                             | .NET 10.0 | .NET 10.0 |  32.6555 ns | 0.1806 ns | 0.1601 ns |   139.068 |    2.31 |   33 | 0.0019 |      32 B |          NA |
 Meziantou_Parse                           | .NET 10.0 | .NET 10.0 |  43.6027 ns | 0.0990 ns | 0.0827 ns |   185.688 |    2.97 |   38 |      - |         - |          NA |
 TinyTypes_Parse                           | .NET 10.0 | .NET 10.0 |  27.7676 ns | 0.0755 ns | 0.0706 ns |   118.252 |    1.90 |   33 |      - |         - |          NA |
 DomainPrimitives_EqualityCheck            | .NET 10.0 | .NET 10.0 |   1.1461 ns | 0.0096 ns | 0.0090 ns |     4.881 |    0.09 |   13 |      - |         - |          NA |
 RawGuid_ToString                          | .NET 10.0 | .NET 10.0 |  11.1364 ns | 0.2171 ns | 0.1924 ns |    47.426 |    1.09 |   30 | 0.0057 |      96 B |          NA |
 DomainPrimitives_ToString                 | .NET 10.0 | .NET 10.0 |  11.0527 ns | 0.2777 ns | 0.3611 ns |    47.069 |    1.68 |   30 | 0.0057 |      96 B |          NA |
 Vogen_ToString                            | .NET 10.0 | .NET 10.0 |  12.7462 ns | 0.2995 ns | 0.2655 ns |    54.282 |    1.39 |   30 | 0.0057 |      96 B |          NA |
 StronglyTypedId_ToString                  | .NET 10.0 | .NET 10.0 |  11.3163 ns | 0.1920 ns | 0.1702 ns |    48.192 |    1.04 |   30 | 0.0057 |      96 B |          NA |
 ValueOf_ToString                          | .NET 10.0 | .NET 10.0 |  23.2602 ns | 0.4296 ns | 0.4019 ns |    99.057 |    2.29 |   32 | 0.0076 |     128 B |          NA |
 Meziantou_ToString                        | .NET 10.0 | .NET 10.0 |  30.2613 ns | 0.6717 ns | 0.7466 ns |   128.872 |    3.72 |   33 | 0.0148 |     248 B |          NA |
 TinyTypes_ToString                        | .NET 10.0 | .NET 10.0 |  14.8070 ns | 0.6413 ns | 1.8908 ns |    63.058 |    8.08 |   31 | 0.0019 |      32 B |          NA |
 DomainPrimitives_TryParse                 | .NET 10.0 | .NET 10.0 |  27.1408 ns | 0.0536 ns | 0.0501 ns |   115.583 |    1.85 |   33 |      - |         - |          NA |
 DomainPrimitives_SpanParse                | .NET 10.0 | .NET 10.0 |  27.5963 ns | 0.0637 ns | 0.0596 ns |   117.523 |    1.88 |   33 |      - |         - |          NA |
 DomainPrimitives_Utf8SpanParse            | .NET 10.0 | .NET 10.0 |  47.0371 ns | 0.1320 ns | 0.1235 ns |   200.314 |    3.23 |   39 | 0.0038 |      64 B |          NA |
 DomainPrimitives_SpanFormat               | .NET 10.0 | .NET 10.0 |   2.4873 ns | 0.0104 ns | 0.0098 ns |    10.593 |    0.17 |   18 |      - |         - |          NA |
 DomainPrimitives_Utf8SpanFormat           | .NET 10.0 | .NET 10.0 |   2.3978 ns | 0.0044 ns | 0.0037 ns |    10.211 |    0.16 |   17 |      - |         - |          NA |
 StringPrimitive_Email_Create              | .NET 10.0 | .NET 10.0 | 158.6818 ns | 0.2077 ns | 0.1943 ns |   675.769 |   10.77 |   48 |      - |         - |          NA |
 StringPrimitive_Email_TryParse            | .NET 10.0 | .NET 10.0 | 169.2568 ns | 0.1175 ns | 0.1099 ns |   720.804 |   11.47 |   50 |      - |         - |          NA |
 NumericPrimitive_Money_Create             | .NET 10.0 | .NET 10.0 |   3.6448 ns | 0.0073 ns | 0.0065 ns |    15.522 |    0.25 |   19 |      - |         - |          NA |
 NumericPrimitive_Money_Add                | .NET 10.0 | .NET 10.0 |   7.9248 ns | 0.0147 ns | 0.0130 ns |    33.749 |    0.54 |   27 |      - |         - |          NA |
 ValueObject_Create                        | .NET 10.0 | .NET 10.0 |   0.3300 ns | 0.0011 ns | 0.0009 ns |     1.405 |    0.02 |    5 |      - |         - |          NA |
 SmartEnum_FromValue                       | .NET 10.0 | .NET 10.0 |   1.2490 ns | 0.0010 ns | 0.0009 ns |     5.319 |    0.08 |   14 |      - |         - |          NA |
 RawGuid_JsonSerialize                     | .NET 10.0 | .NET 10.0 |  99.4146 ns | 0.3116 ns | 0.2914 ns |   423.371 |    6.84 |   42 | 0.0062 |     104 B |          NA |
 RawGuid_JsonDeserialize                   | .NET 10.0 | .NET 10.0 | 100.8233 ns | 0.0990 ns | 0.0878 ns |   429.370 |    6.84 |   42 |      - |         - |          NA |
 DomainPrimitives_JsonSerialize            | .NET 10.0 | .NET 10.0 |  92.5793 ns | 0.2499 ns | 0.2338 ns |   394.262 |    6.34 |   41 | 0.0062 |     104 B |          NA |
 DomainPrimitives_JsonDeserialize          | .NET 10.0 | .NET 10.0 | 100.0364 ns | 0.1194 ns | 0.0997 ns |   426.019 |    6.79 |   42 |      - |         - |          NA |
 RawGuid_Create                            | .NET 8.0  | .NET 8.0  |   0.2349 ns | 0.0044 ns | 0.0039 ns |     1.000 |    0.02 |    4 |      - |         - |          NA |
 DomainPrimitives_Create                   | .NET 8.0  | .NET 8.0  |   0.8586 ns | 0.0013 ns | 0.0012 ns |     3.656 |    0.06 |   10 |      - |         - |          NA |
 Vogen_Create                              | .NET 8.0  | .NET 8.0  |   7.7265 ns | 0.0019 ns | 0.0017 ns |    32.904 |    0.52 |   26 |      - |         - |          NA |
 StronglyTypedId_Create                    | .NET 8.0  | .NET 8.0  |   0.3328 ns | 0.0009 ns | 0.0007 ns |     1.417 |    0.02 |    5 |      - |         - |          NA |
 ValueOf_Create                            | .NET 8.0  | .NET 8.0  |  14.1468 ns | 0.0795 ns | 0.0705 ns |    60.246 |    1.00 |   31 | 0.0019 |      32 B |          NA |
 Meziantou_Create                          | .NET 8.0  | .NET 8.0  |   0.1572 ns | 0.0029 ns | 0.0027 ns |     0.669 |    0.02 |    2 |      - |         - |          NA |
 TinyTypes_Create                          | .NET 8.0  | .NET 8.0  |   0.3410 ns | 0.0017 ns | 0.0015 ns |     1.452 |    0.02 |    5 |      - |         - |          NA |
 RawGuid_Parse                             | .NET 8.0  | .NET 8.0  |  29.1800 ns | 0.0751 ns | 0.0703 ns |   124.267 |    2.00 |   33 |      - |         - |          NA |
 DomainPrimitives_Parse                    | .NET 8.0  | .NET 8.0  |  28.9698 ns | 0.0336 ns | 0.0298 ns |   123.372 |    1.97 |   33 |      - |         - |          NA |
 Vogen_Parse                               | .NET 8.0  | .NET 8.0  |  32.8924 ns | 0.1323 ns | 0.1237 ns |   140.077 |    2.28 |   33 |      - |         - |          NA |
 StronglyTypedId_Parse                     | .NET 8.0  | .NET 8.0  |  29.2003 ns | 0.0748 ns | 0.0625 ns |   124.354 |    1.99 |   33 |      - |         - |          NA |
 ValueOf_Parse                             | .NET 8.0  | .NET 8.0  |  33.9334 ns | 0.0421 ns | 0.0351 ns |   144.510 |    2.30 |   34 | 0.0019 |      32 B |          NA |
 Meziantou_Parse                           | .NET 8.0  | .NET 8.0  |  29.4034 ns | 0.0373 ns | 0.0291 ns |   125.218 |    2.00 |   33 |      - |         - |          NA |
 TinyTypes_Parse                           | .NET 8.0  | .NET 8.0  |  29.3463 ns | 0.0435 ns | 0.0386 ns |   124.975 |    1.99 |   33 |      - |         - |          NA |
 DomainPrimitives_EqualityCheck            | .NET 8.0  | .NET 8.0  |   0.7852 ns | 0.0037 ns | 0.0033 ns |     3.344 |    0.05 |    8 |      - |         - |          NA |
 RawGuid_ToString                          | .NET 8.0  | .NET 8.0  |  14.8777 ns | 0.2173 ns | 0.2032 ns |    63.359 |    1.31 |   31 | 0.0057 |      96 B |          NA |
 DomainPrimitives_ToString                 | .NET 8.0  | .NET 8.0  |  15.1991 ns | 0.3434 ns | 0.3212 ns |    64.727 |    1.68 |   31 | 0.0057 |      96 B |          NA |
 Vogen_ToString                            | .NET 8.0  | .NET 8.0  |  24.7643 ns | 0.3656 ns | 0.3241 ns |   105.462 |    2.14 |   32 | 0.0057 |      96 B |          NA |
 StronglyTypedId_ToString                  | .NET 8.0  | .NET 8.0  |  15.4347 ns | 0.1142 ns | 0.1069 ns |    65.731 |    1.13 |   31 | 0.0057 |      96 B |          NA |
 ValueOf_ToString                          | .NET 8.0  | .NET 8.0  |  23.3370 ns | 0.3968 ns | 0.3711 ns |    99.384 |    2.20 |   32 | 0.0076 |     128 B |          NA |
 Meziantou_ToString                        | .NET 8.0  | .NET 8.0  |  35.7715 ns | 0.5588 ns | 0.5227 ns |   152.338 |    3.24 |   35 | 0.0148 |     248 B |          NA |
 TinyTypes_ToString                        | .NET 8.0  | .NET 8.0  |   9.7117 ns | 0.0870 ns | 0.0771 ns |    41.359 |    0.73 |   29 | 0.0019 |      32 B |          NA |
 DomainPrimitives_TryParse                 | .NET 8.0  | .NET 8.0  |  28.1750 ns | 0.0689 ns | 0.0538 ns |   119.987 |    1.92 |   33 |      - |         - |          NA |
 DomainPrimitives_SpanParse                | .NET 8.0  | .NET 8.0  |  28.1399 ns | 0.1025 ns | 0.0909 ns |   119.838 |    1.94 |   33 |      - |         - |          NA |
 DomainPrimitives_Utf8SpanParse            | .NET 8.0  | .NET 8.0  |  63.6410 ns | 0.4356 ns | 0.4074 ns |   271.024 |    4.63 |   40 | 0.0038 |      64 B |          NA |
 DomainPrimitives_SpanFormat               | .NET 8.0  | .NET 8.0  |   5.0025 ns | 0.0117 ns | 0.0104 ns |    21.304 |    0.34 |   24 |      - |         - |          NA |
 DomainPrimitives_Utf8SpanFormat           | .NET 8.0  | .NET 8.0  |   4.1970 ns | 0.0033 ns | 0.0029 ns |    17.873 |    0.28 |   21 |      - |         - |          NA |
 StringPrimitive_Email_Create              | .NET 8.0  | .NET 8.0  | 359.3821 ns | 1.4568 ns | 1.3627 ns | 1,530.480 |   24.98 |   52 |      - |         - |          NA |
 StringPrimitive_Email_TryParse            | .NET 8.0  | .NET 8.0  | 336.5112 ns | 0.3363 ns | 0.3145 ns | 1,433.081 |   22.82 |   51 |      - |         - |          NA |
 NumericPrimitive_Money_Create             | .NET 8.0  | .NET 8.0  |   4.3813 ns | 0.0075 ns | 0.0066 ns |    18.658 |    0.30 |   22 |      - |         - |          NA |
 NumericPrimitive_Money_Add                | .NET 8.0  | .NET 8.0  |   9.4964 ns | 0.0071 ns | 0.0060 ns |    40.442 |    0.64 |   29 |      - |         - |          NA |
 ValueObject_Create                        | .NET 8.0  | .NET 8.0  |   0.3543 ns | 0.0022 ns | 0.0019 ns |     1.509 |    0.03 |    5 |      - |         - |          NA |
 SmartEnum_FromValue                       | .NET 8.0  | .NET 8.0  |   3.7309 ns | 0.0053 ns | 0.0047 ns |    15.889 |    0.25 |   20 |      - |         - |          NA |
 RawGuid_JsonSerialize                     | .NET 8.0  | .NET 8.0  | 133.5231 ns | 0.3021 ns | 0.2825 ns |   568.627 |    9.12 |   46 | 0.0062 |     104 B |          NA |
 RawGuid_JsonDeserialize                   | .NET 8.0  | .NET 8.0  | 151.0261 ns | 0.0935 ns | 0.0730 ns |   643.166 |   10.24 |   47 |      - |         - |          NA |
 DomainPrimitives_JsonSerialize            | .NET 8.0  | .NET 8.0  | 135.2913 ns | 0.1765 ns | 0.1474 ns |   576.157 |    9.18 |   46 | 0.0062 |     104 B |          NA |
 DomainPrimitives_JsonDeserialize          | .NET 8.0  | .NET 8.0  | 164.0389 ns | 0.1557 ns | 0.1456 ns |   698.583 |   11.12 |   49 |      - |         - |          NA |
 RawGuid_Create                            | .NET 9.0  | .NET 9.0  |   0.2382 ns | 0.0065 ns | 0.0057 ns |     1.015 |    0.03 |    4 |      - |         - |          NA |
 DomainPrimitives_Create                   | .NET 9.0  | .NET 9.0  |   0.8395 ns | 0.0011 ns | 0.0009 ns |     3.575 |    0.06 |    9 |      - |         - |          NA |
 Vogen_Create                              | .NET 9.0  | .NET 9.0  |   0.5275 ns | 0.0015 ns | 0.0013 ns |     2.246 |    0.04 |    7 |      - |         - |          NA |
 StronglyTypedId_Create                    | .NET 9.0  | .NET 9.0  |   0.1328 ns | 0.0022 ns | 0.0020 ns |     0.565 |    0.01 |    2 |      - |         - |          NA |
 ValueOf_Create                            | .NET 9.0  | .NET 9.0  |   9.8836 ns | 0.1033 ns | 0.0967 ns |    42.091 |    0.78 |   29 | 0.0019 |      32 B |          NA |
 Meziantou_Create                          | .NET 9.0  | .NET 9.0  |   0.2124 ns | 0.0031 ns | 0.0029 ns |     0.905 |    0.02 |    3 |      - |         - |          NA |
 TinyTypes_Create                          | .NET 9.0  | .NET 9.0  |   0.2193 ns | 0.0061 ns | 0.0057 ns |     0.934 |    0.03 |    3 |      - |         - |          NA |
 RawGuid_Parse                             | .NET 9.0  | .NET 9.0  |  28.6978 ns | 0.0676 ns | 0.0632 ns |   122.214 |    1.96 |   33 |      - |         - |          NA |
 DomainPrimitives_Parse                    | .NET 9.0  | .NET 9.0  |  28.5654 ns | 0.0460 ns | 0.0384 ns |   121.650 |    1.94 |   33 |      - |         - |          NA |
 Vogen_Parse                               | .NET 9.0  | .NET 9.0  |  32.0591 ns | 0.0619 ns | 0.0579 ns |   136.528 |    2.18 |   33 |      - |         - |          NA |
 StronglyTypedId_Parse                     | .NET 9.0  | .NET 9.0  |  28.6722 ns | 0.0687 ns | 0.0609 ns |   122.105 |    1.96 |   33 |      - |         - |          NA |
 ValueOf_Parse                             | .NET 9.0  | .NET 9.0  |  32.3857 ns | 0.0945 ns | 0.0789 ns |   137.919 |    2.22 |   33 | 0.0019 |      32 B |          NA |
 Meziantou_Parse                           | .NET 9.0  | .NET 9.0  |  28.7173 ns | 0.0556 ns | 0.0520 ns |   122.297 |    1.96 |   33 |      - |         - |          NA |
 TinyTypes_Parse                           | .NET 9.0  | .NET 9.0  |  28.8110 ns | 0.0311 ns | 0.0243 ns |   122.696 |    1.95 |   33 |      - |         - |          NA |
 DomainPrimitives_EqualityCheck            | .NET 9.0  | .NET 9.0  |   0.9633 ns | 0.0045 ns | 0.0043 ns |     4.102 |    0.07 |   11 |      - |         - |          NA |
 RawGuid_ToString                          | .NET 9.0  | .NET 9.0  |  15.5693 ns | 0.2289 ns | 0.1911 ns |    66.304 |    1.31 |   31 | 0.0057 |      96 B |          NA |
 DomainPrimitives_ToString                 | .NET 9.0  | .NET 9.0  |  15.3275 ns | 0.3565 ns | 0.3335 ns |    65.274 |    1.72 |   31 | 0.0057 |      96 B |          NA |
 Vogen_ToString                            | .NET 9.0  | .NET 9.0  |  16.7971 ns | 0.4015 ns | 0.6251 ns |    71.533 |    2.86 |   31 | 0.0057 |      96 B |          NA |
 StronglyTypedId_ToString                  | .NET 9.0  | .NET 9.0  |  16.1151 ns | 0.3741 ns | 0.4158 ns |    68.628 |    2.04 |   31 | 0.0057 |      96 B |          NA |
 ValueOf_ToString                          | .NET 9.0  | .NET 9.0  |  24.5960 ns | 0.5426 ns | 0.5806 ns |   104.745 |    2.93 |   32 | 0.0076 |     128 B |          NA |
 Meziantou_ToString                        | .NET 9.0  | .NET 9.0  |  40.9252 ns | 0.7730 ns | 1.1570 ns |   174.286 |    5.58 |   37 | 0.0148 |     248 B |          NA |
 TinyTypes_ToString                        | .NET 9.0  | .NET 9.0  |  11.3149 ns | 0.0839 ns | 0.0785 ns |    48.186 |    0.83 |   30 | 0.0019 |      32 B |          NA |
 DomainPrimitives_TryParse                 | .NET 9.0  | .NET 9.0  |  27.6993 ns | 0.0242 ns | 0.0189 ns |   117.962 |    1.88 |   33 |      - |         - |          NA |
 DomainPrimitives_SpanParse                | .NET 9.0  | .NET 9.0  |  27.9404 ns | 0.0242 ns | 0.0227 ns |   118.988 |    1.89 |   33 |      - |         - |          NA |
 DomainPrimitives_Utf8SpanParse            | .NET 9.0  | .NET 9.0  |  62.0938 ns | 0.2224 ns | 0.1736 ns |   264.435 |    4.27 |   40 | 0.0038 |      64 B |          NA |
 DomainPrimitives_SpanFormat               | .NET 9.0  | .NET 9.0  |   4.4502 ns | 0.0077 ns | 0.0072 ns |    18.952 |    0.30 |   22 |      - |         - |          NA |
 DomainPrimitives_Utf8SpanFormat           | .NET 9.0  | .NET 9.0  |   3.7761 ns | 0.0056 ns | 0.0047 ns |    16.081 |    0.26 |   20 |      - |         - |          NA |
 StringPrimitive_Email_Create              | .NET 9.0  | .NET 9.0  | 169.0696 ns | 0.2763 ns | 0.2584 ns |   720.007 |   11.50 |   50 |      - |         - |          NA |
 StringPrimitive_Email_TryParse            | .NET 9.0  | .NET 9.0  | 170.4757 ns | 0.1734 ns | 0.1537 ns |   725.995 |   11.56 |   50 |      - |         - |          NA |
 NumericPrimitive_Money_Create             | .NET 9.0  | .NET 9.0  |   4.1502 ns | 0.0056 ns | 0.0052 ns |    17.674 |    0.28 |   21 |      - |         - |          NA |
 NumericPrimitive_Money_Add                | .NET 9.0  | .NET 9.0  |   8.7078 ns | 0.0071 ns | 0.0059 ns |    37.083 |    0.59 |   28 |      - |         - |          NA |
 ValueObject_Create                        | .NET 9.0  | .NET 9.0  |   0.3118 ns | 0.0026 ns | 0.0023 ns |     1.328 |    0.02 |    5 |      - |         - |          NA |
 SmartEnum_FromValue                       | .NET 9.0  | .NET 9.0  |   2.1083 ns | 0.0059 ns | 0.0052 ns |     8.979 |    0.14 |   16 |      - |         - |          NA |
 RawGuid_JsonSerialize                     | .NET 9.0  | .NET 9.0  | 112.5714 ns | 0.2635 ns | 0.2465 ns |   479.401 |    7.69 |   43 | 0.0062 |     104 B |          NA |
 RawGuid_JsonDeserialize                   | .NET 9.0  | .NET 9.0  | 122.6724 ns | 0.2622 ns | 0.2324 ns |   522.418 |    8.36 |   45 |      - |         - |          NA |
 DomainPrimitives_JsonSerialize            | .NET 9.0  | .NET 9.0  | 117.7178 ns | 0.4353 ns | 0.4072 ns |   501.318 |    8.15 |   44 | 0.0062 |     104 B |          NA |
 DomainPrimitives_JsonDeserialize          | .NET 9.0  | .NET 9.0  | 123.2363 ns | 0.1324 ns | 0.1105 ns |   524.819 |    8.36 |   45 |      - |         - |          NA |
 RawGuid_Create                            | .NET 9.0  | .NET 9.0  |   0.2453 ns | 0.0122 ns | 0.0108 ns |     1.045 |    0.05 |    4 |      - |         - |          NA |
 DomainPrimitives_Create                   | .NET 9.0  | .NET 9.0  |   0.9112 ns | 0.0164 ns | 0.0146 ns |     3.880 |    0.09 |   11 |      - |         - |          NA |
 Vogen_Create                              | .NET 9.0  | .NET 9.0  |   0.3141 ns | 0.0368 ns | 0.0378 ns |     1.338 |    0.16 |    5 |      - |         - |          NA |
 StronglyTypedId_Create                    | .NET 9.0  | .NET 9.0  |   0.1371 ns | 0.0036 ns | 0.0032 ns |     0.584 |    0.02 |    2 |      - |         - |          NA |
 ValueOf_Create                            | .NET 9.0  | .NET 9.0  |   8.7137 ns | 0.0935 ns | 0.0829 ns |    37.108 |    0.68 |   28 | 0.0019 |      32 B |          NA |
 Meziantou_Create                          | .NET 9.0  | .NET 9.0  |   0.1610 ns | 0.0086 ns | 0.0081 ns |     0.686 |    0.04 |    2 |      - |         - |          NA |
 TinyTypes_Create                          | .NET 9.0  | .NET 9.0  |   0.1426 ns | 0.0046 ns | 0.0043 ns |     0.607 |    0.02 |    2 |      - |         - |          NA |
 RawGuid_Parse                             | .NET 9.0  | .NET 9.0  |  28.8869 ns | 0.0597 ns | 0.0530 ns |   123.019 |    1.97 |   33 |      - |         - |          NA |
 DomainPrimitives_Parse                    | .NET 9.0  | .NET 9.0  |  28.7125 ns | 0.0860 ns | 0.0804 ns |   122.276 |    1.97 |   33 |      - |         - |          NA |
 Vogen_Parse                               | .NET 9.0  | .NET 9.0  |  29.7073 ns | 0.0267 ns | 0.0237 ns |   126.513 |    2.01 |   33 |      - |         - |          NA |
 StronglyTypedId_Parse                     | .NET 9.0  | .NET 9.0  |  28.9466 ns | 0.0970 ns | 0.0908 ns |   123.273 |    2.00 |   33 |      - |         - |          NA |
 ValueOf_Parse                             | .NET 9.0  | .NET 9.0  |  32.3541 ns | 0.1031 ns | 0.0914 ns |   137.785 |    2.22 |   33 | 0.0019 |      32 B |          NA |
 Meziantou_Parse                           | .NET 9.0  | .NET 9.0  |  29.2057 ns | 0.0660 ns | 0.0585 ns |   124.377 |    1.99 |   33 |      - |         - |          NA |
 TinyTypes_Parse                           | .NET 9.0  | .NET 9.0  |  28.9053 ns | 0.0812 ns | 0.0759 ns |   123.097 |    1.98 |   33 |      - |         - |          NA |
 DomainPrimitives_EqualityCheck            | .NET 9.0  | .NET 9.0  |   0.9626 ns | 0.0045 ns | 0.0042 ns |     4.100 |    0.07 |   11 |      - |         - |          NA |
 RawGuid_ToString                          | .NET 9.0  | .NET 9.0  |  15.5356 ns | 0.3554 ns | 0.4621 ns |    66.161 |    2.20 |   31 | 0.0057 |      96 B |          NA |
 DomainPrimitives_ToString                 | .NET 9.0  | .NET 9.0  |  15.3618 ns | 0.3617 ns | 0.3206 ns |    65.420 |    1.68 |   31 | 0.0057 |      96 B |          NA |
 Vogen_ToString                            | .NET 9.0  | .NET 9.0  |  16.4769 ns | 0.2317 ns | 0.2054 ns |    70.169 |    1.40 |   31 | 0.0057 |      96 B |          NA |
 StronglyTypedId_ToString                  | .NET 9.0  | .NET 9.0  |  15.2967 ns | 0.2687 ns | 0.2513 ns |    65.143 |    1.47 |   31 | 0.0057 |      96 B |          NA |
 ValueOf_ToString                          | .NET 9.0  | .NET 9.0  |  22.7916 ns | 0.1741 ns | 0.1628 ns |    97.061 |    1.68 |   32 | 0.0076 |     128 B |          NA |
 Meziantou_ToString                        | .NET 9.0  | .NET 9.0  |  37.7710 ns | 0.5882 ns | 0.5502 ns |   160.853 |    3.42 |   36 | 0.0148 |     248 B |          NA |
 TinyTypes_ToString                        | .NET 9.0  | .NET 9.0  |  10.7685 ns | 0.0751 ns | 0.0702 ns |    45.859 |    0.78 |   30 | 0.0019 |      32 B |          NA |
 DomainPrimitives_TryParse                 | .NET 9.0  | .NET 9.0  |  27.8915 ns | 0.0363 ns | 0.0340 ns |   118.780 |    1.89 |   33 |      - |         - |          NA |
 DomainPrimitives_SpanParse                | .NET 9.0  | .NET 9.0  |  27.7937 ns | 0.0672 ns | 0.0629 ns |   118.363 |    1.90 |   33 |      - |         - |          NA |
 DomainPrimitives_Utf8SpanParse            | .NET 9.0  | .NET 9.0  |  60.2485 ns | 0.5679 ns | 0.5034 ns |   256.577 |    4.58 |   40 | 0.0038 |      64 B |          NA |
 DomainPrimitives_SpanFormat               | .NET 9.0  | .NET 9.0  |   4.5930 ns | 0.0078 ns | 0.0073 ns |    19.560 |    0.31 |   23 |      - |         - |          NA |
 DomainPrimitives_Utf8SpanFormat           | .NET 9.0  | .NET 9.0  |   3.7769 ns | 0.0040 ns | 0.0035 ns |    16.084 |    0.26 |   20 |      - |         - |          NA |
 StringPrimitive_Email_Create              | .NET 9.0  | .NET 9.0  | 168.7531 ns | 0.1484 ns | 0.1388 ns |   718.659 |   11.44 |   50 |      - |         - |          NA |
 StringPrimitive_Email_TryParse            | .NET 9.0  | .NET 9.0  | 170.6855 ns | 0.2211 ns | 0.1846 ns |   726.889 |   11.59 |   50 |      - |         - |          NA |
 NumericPrimitive_Money_Create             | .NET 9.0  | .NET 9.0  |   4.1473 ns | 0.0053 ns | 0.0047 ns |    17.662 |    0.28 |   21 |      - |         - |          NA |
 NumericPrimitive_Money_Add                | .NET 9.0  | .NET 9.0  |   8.6963 ns | 0.0049 ns | 0.0041 ns |    37.034 |    0.59 |   28 |      - |         - |          NA |
 ValueObject_Create                        | .NET 9.0  | .NET 9.0  |   0.3126 ns | 0.0013 ns | 0.0010 ns |     1.331 |    0.02 |    5 |      - |         - |          NA |
 SmartEnum_FromValue                       | .NET 9.0  | .NET 9.0  |   2.1047 ns | 0.0028 ns | 0.0023 ns |     8.963 |    0.14 |   16 |      - |         - |          NA |
 RawGuid_JsonSerialize                     | .NET 9.0  | .NET 9.0  | 113.6221 ns | 0.7611 ns | 0.7119 ns |   483.876 |    8.24 |   43 | 0.0062 |     104 B |          NA |
 RawGuid_JsonDeserialize                   | .NET 9.0  | .NET 9.0  | 121.7390 ns | 0.3905 ns | 0.3048 ns |   518.443 |    8.34 |   45 |      - |         - |          NA |
 DomainPrimitives_JsonSerialize            | .NET 9.0  | .NET 9.0  | 112.8987 ns | 0.6877 ns | 0.6433 ns |   480.795 |    8.09 |   43 | 0.0062 |     104 B |          NA |
 DomainPrimitives_JsonDeserialize          | .NET 9.0  | .NET 9.0  | 120.9247 ns | 0.0805 ns | 0.0672 ns |   514.975 |    8.20 |   45 |      - |         - |          NA |
 Dapper_TypeHandler_SetValue               | .NET 10.0 | .NET 10.0 |   0.0000 ns | 0.0000 ns | 0.0000 ns |     0.000 |    0.00 |    1 |      - |         - |          NA |
 Dapper_TypeHandler_Parse                  | .NET 10.0 | .NET 10.0 |   0.9217 ns | 0.0029 ns | 0.0027 ns |     3.925 |    0.06 |   11 |      - |         - |          NA |
 EFCore_ValueConverter_ConvertToProvider   | .NET 10.0 | .NET 10.0 |   1.1482 ns | 0.0040 ns | 0.0033 ns |     4.890 |    0.08 |   13 |      - |         - |          NA |
 EFCore_ValueConverter_ConvertFromProvider | .NET 10.0 | .NET 10.0 |   0.8848 ns | 0.0031 ns | 0.0029 ns |     3.768 |    0.06 |   11 |      - |         - |          NA |
 Dapper_TypeHandler_SetValue               | .NET 8.0  | .NET 8.0  |   7.3183 ns | 0.0536 ns | 0.0475 ns |    31.166 |    0.53 |   25 | 0.0019 |      32 B |          NA |
 Dapper_TypeHandler_Parse                  | .NET 8.0  | .NET 8.0  |   0.9007 ns | 0.0027 ns | 0.0024 ns |     3.836 |    0.06 |   11 |      - |         - |          NA |
 EFCore_ValueConverter_ConvertToProvider   | .NET 8.0  | .NET 8.0  |  10.8168 ns | 0.0070 ns | 0.0066 ns |    46.065 |    0.73 |   30 |      - |         - |          NA |
 EFCore_ValueConverter_ConvertFromProvider | .NET 8.0  | .NET 8.0  |   0.9398 ns | 0.0027 ns | 0.0023 ns |     4.002 |    0.06 |   11 |      - |         - |          NA |
 Dapper_TypeHandler_SetValue               | .NET 9.0  | .NET 9.0  |   0.8300 ns | 0.0072 ns | 0.0060 ns |     3.535 |    0.06 |    9 |      - |         - |          NA |
 Dapper_TypeHandler_Parse                  | .NET 9.0  | .NET 9.0  |   0.9112 ns | 0.0021 ns | 0.0019 ns |     3.881 |    0.06 |   11 |      - |         - |          NA |
 EFCore_ValueConverter_ConvertToProvider   | .NET 9.0  | .NET 9.0  |   1.5262 ns | 0.0085 ns | 0.0079 ns |     6.499 |    0.11 |   15 |      - |         - |          NA |
 EFCore_ValueConverter_ConvertFromProvider | .NET 9.0  | .NET 9.0  |   0.9121 ns | 0.0033 ns | 0.0031 ns |     3.884 |    0.06 |   11 |      - |         - |          NA |
 Dapper_TypeHandler_SetValue               | .NET 9.0  | .NET 9.0  |   0.8324 ns | 0.0044 ns | 0.0037 ns |     3.545 |    0.06 |    9 |      - |         - |          NA |
 Dapper_TypeHandler_Parse                  | .NET 9.0  | .NET 9.0  |   0.9125 ns | 0.0024 ns | 0.0023 ns |     3.886 |    0.06 |   11 |      - |         - |          NA |
 EFCore_ValueConverter_ConvertToProvider   | .NET 9.0  | .NET 9.0  |   1.1625 ns | 0.0046 ns | 0.0043 ns |     4.951 |    0.08 |   13 |      - |         - |          NA |
 EFCore_ValueConverter_ConvertFromProvider | .NET 9.0  | .NET 9.0  |   1.1080 ns | 0.0023 ns | 0.0021 ns |     4.719 |    0.08 |   12 |      - |         - |          NA |
