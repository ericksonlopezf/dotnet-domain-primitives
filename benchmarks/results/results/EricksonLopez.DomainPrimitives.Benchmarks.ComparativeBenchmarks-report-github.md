```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.401
  [Host]    : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v3
  .NET 8.0  : .NET 8.0.31 (8.0.31, 8.0.3126.42015), X64 RyuJIT x86-64-v3
  .NET 9.0  : .NET 9.0.20 (9.0.20, 9.0.2026.41315), X64 RyuJIT x86-64-v3


```
| Method                                    | Job       | Runtime   | Mean        | Error     | StdDev    | Ratio     | RatioSD | Rank | Gen0   | Allocated | Alloc Ratio |
|------------------------------------------ |---------- |---------- |------------:|----------:|----------:|----------:|--------:|-----:|-------:|----------:|------------:|
| RawGuid_Create                            | .NET 10.0 | .NET 10.0 |   0.2624 ns | 0.0030 ns | 0.0027 ns |     1.260 |    0.02 |    7 |      - |         - |          NA |
| DomainPrimitives_Create                   | .NET 10.0 | .NET 10.0 |   0.8397 ns | 0.0030 ns | 0.0025 ns |     4.032 |    0.04 |   14 |      - |         - |          NA |
| Vogen_Create                              | .NET 10.0 | .NET 10.0 |   0.5462 ns | 0.0025 ns | 0.0021 ns |     2.623 |    0.03 |   11 |      - |         - |          NA |
| StronglyTypedId_Create                    | .NET 10.0 | .NET 10.0 |   0.2026 ns | 0.0025 ns | 0.0022 ns |     0.973 |    0.01 |    5 |      - |         - |          NA |
| ValueOf_Create                            | .NET 10.0 | .NET 10.0 |  10.3672 ns | 0.2737 ns | 0.7718 ns |    49.780 |    3.72 |   34 | 0.0019 |      32 B |          NA |
| Meziantou_Create                          | .NET 10.0 | .NET 10.0 |   0.2377 ns | 0.0026 ns | 0.0021 ns |     1.141 |    0.02 |    6 |      - |         - |          NA |
| TinyTypes_Create                          | .NET 10.0 | .NET 10.0 |   0.2412 ns | 0.0025 ns | 0.0023 ns |     1.158 |    0.02 |    6 |      - |         - |          NA |
| RawGuid_Parse                             | .NET 10.0 | .NET 10.0 |  27.4109 ns | 0.0607 ns | 0.0568 ns |   131.618 |    1.41 |   40 |      - |         - |          NA |
| DomainPrimitives_Parse                    | .NET 10.0 | .NET 10.0 |  27.5421 ns | 0.0980 ns | 0.0917 ns |   132.248 |    1.46 |   40 |      - |         - |          NA |
| Vogen_Parse                               | .NET 10.0 | .NET 10.0 |  28.2579 ns | 0.0772 ns | 0.0685 ns |   135.685 |    1.46 |   40 |      - |         - |          NA |
| StronglyTypedId_Parse                     | .NET 10.0 | .NET 10.0 |  27.7806 ns | 0.0293 ns | 0.0260 ns |   133.393 |    1.41 |   40 |      - |         - |          NA |
| ValueOf_Parse                             | .NET 10.0 | .NET 10.0 |  30.2971 ns | 0.1863 ns | 0.1743 ns |   145.476 |    1.73 |   40 | 0.0019 |      32 B |          NA |
| Meziantou_Parse                           | .NET 10.0 | .NET 10.0 |  43.8016 ns | 0.1148 ns | 0.1074 ns |   210.321 |    2.27 |   44 |      - |         - |          NA |
| TinyTypes_Parse                           | .NET 10.0 | .NET 10.0 |  27.4118 ns | 0.0427 ns | 0.0333 ns |   131.622 |    1.40 |   40 |      - |         - |          NA |
| DomainPrimitives_EqualityCheck            | .NET 10.0 | .NET 10.0 |   1.1101 ns | 0.0072 ns | 0.0064 ns |     5.330 |    0.06 |   17 |      - |         - |          NA |
| RawGuid_ToString                          | .NET 10.0 | .NET 10.0 |  10.3897 ns | 0.0754 ns | 0.0706 ns |    49.888 |    0.62 |   34 | 0.0057 |      96 B |          NA |
| DomainPrimitives_ToString                 | .NET 10.0 | .NET 10.0 |   9.9136 ns | 0.0347 ns | 0.0290 ns |    47.602 |    0.52 |   34 | 0.0057 |      96 B |          NA |
| Vogen_ToString                            | .NET 10.0 | .NET 10.0 |  10.6806 ns | 0.0998 ns | 0.0885 ns |    51.285 |    0.68 |   34 | 0.0057 |      96 B |          NA |
| StronglyTypedId_ToString                  | .NET 10.0 | .NET 10.0 |   9.5116 ns | 0.0944 ns | 0.0837 ns |    45.672 |    0.62 |   34 | 0.0057 |      96 B |          NA |
| ValueOf_ToString                          | .NET 10.0 | .NET 10.0 |  20.2103 ns | 0.1211 ns | 0.1074 ns |    97.043 |    1.14 |   37 | 0.0076 |     128 B |          NA |
| Meziantou_ToString                        | .NET 10.0 | .NET 10.0 |  26.2210 ns | 0.1647 ns | 0.1375 ns |   125.904 |    1.47 |   39 | 0.0148 |     248 B |          NA |
| TinyTypes_ToString                        | .NET 10.0 | .NET 10.0 |  16.7265 ns | 0.4710 ns | 1.3888 ns |    80.315 |    6.69 |   36 | 0.0019 |      32 B |          NA |
| DomainPrimitives_TryParse                 | .NET 10.0 | .NET 10.0 |  27.6534 ns | 0.0753 ns | 0.0705 ns |   132.782 |    1.44 |   40 |      - |         - |          NA |
| DomainPrimitives_SpanParse                | .NET 10.0 | .NET 10.0 |  27.5907 ns | 0.1153 ns | 0.1078 ns |   132.481 |    1.48 |   40 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanParse            | .NET 10.0 | .NET 10.0 |  46.0805 ns | 0.2820 ns | 0.2638 ns |   221.263 |    2.63 |   45 | 0.0038 |      64 B |          NA |
| DomainPrimitives_SpanFormat               | .NET 10.0 | .NET 10.0 |   3.5469 ns | 0.0041 ns | 0.0035 ns |    17.031 |    0.18 |   23 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanFormat           | .NET 10.0 | .NET 10.0 |   2.3778 ns | 0.0063 ns | 0.0056 ns |    11.417 |    0.12 |   22 |      - |         - |          NA |
| StringPrimitive_Email_Create              | .NET 10.0 | .NET 10.0 | 158.6510 ns | 0.1714 ns | 0.1431 ns |   761.789 |    8.06 |   55 |      - |         - |          NA |
| StringPrimitive_Email_TryParse            | .NET 10.0 | .NET 10.0 | 171.3455 ns | 0.1852 ns | 0.1641 ns |   822.743 |    8.70 |   57 |      - |         - |          NA |
| NumericPrimitive_Money_Create             | .NET 10.0 | .NET 10.0 |   3.6433 ns | 0.0034 ns | 0.0030 ns |    17.494 |    0.18 |   24 |      - |         - |          NA |
| NumericPrimitive_Money_Add                | .NET 10.0 | .NET 10.0 |   8.0969 ns | 0.0023 ns | 0.0019 ns |    38.878 |    0.41 |   32 |      - |         - |          NA |
| ValueObject_Create                        | .NET 10.0 | .NET 10.0 |   0.3316 ns | 0.0021 ns | 0.0020 ns |     1.592 |    0.02 |    9 |      - |         - |          NA |
| SmartEnum_FromValue                       | .NET 10.0 | .NET 10.0 |   1.2511 ns | 0.0018 ns | 0.0015 ns |     6.007 |    0.06 |   19 |      - |         - |          NA |
| RawGuid_JsonSerialize                     | .NET 10.0 | .NET 10.0 |  95.5570 ns | 0.8413 ns | 0.7870 ns |   458.832 |    6.06 |   48 | 0.0062 |     104 B |          NA |
| RawGuid_JsonDeserialize                   | .NET 10.0 | .NET 10.0 | 101.0040 ns | 0.0970 ns | 0.0907 ns |   484.987 |    5.13 |   49 |      - |         - |          NA |
| DomainPrimitives_JsonSerialize            | .NET 10.0 | .NET 10.0 |  90.8614 ns | 0.3221 ns | 0.2855 ns |   436.286 |    4.78 |   47 | 0.0062 |     104 B |          NA |
| DomainPrimitives_JsonDeserialize          | .NET 10.0 | .NET 10.0 | 101.7128 ns | 0.1027 ns | 0.0961 ns |   488.391 |    5.16 |   49 |      - |         - |          NA |
| RawGuid_Create                            | .NET 8.0  | .NET 8.0  |   0.2083 ns | 0.0026 ns | 0.0023 ns |     1.000 |    0.01 |    5 |      - |         - |          NA |
| DomainPrimitives_Create                   | .NET 8.0  | .NET 8.0  |   0.8594 ns | 0.0013 ns | 0.0010 ns |     4.127 |    0.04 |   14 |      - |         - |          NA |
| Vogen_Create                              | .NET 8.0  | .NET 8.0  |   5.5639 ns | 0.0043 ns | 0.0040 ns |    26.716 |    0.28 |   30 |      - |         - |          NA |
| StronglyTypedId_Create                    | .NET 8.0  | .NET 8.0  |   0.3342 ns | 0.0014 ns | 0.0012 ns |     1.605 |    0.02 |    9 |      - |         - |          NA |
| ValueOf_Create                            | .NET 8.0  | .NET 8.0  |   9.3948 ns | 0.1980 ns | 0.1852 ns |    45.111 |    0.98 |   34 | 0.0019 |      32 B |          NA |
| Meziantou_Create                          | .NET 8.0  | .NET 8.0  |   0.1730 ns | 0.0037 ns | 0.0034 ns |     0.831 |    0.02 |    3 |      - |         - |          NA |
| TinyTypes_Create                          | .NET 8.0  | .NET 8.0  |   0.3338 ns | 0.0008 ns | 0.0007 ns |     1.603 |    0.02 |    9 |      - |         - |          NA |
| RawGuid_Parse                             | .NET 8.0  | .NET 8.0  |  29.2512 ns | 0.0586 ns | 0.0489 ns |   140.454 |    1.50 |   40 |      - |         - |          NA |
| DomainPrimitives_Parse                    | .NET 8.0  | .NET 8.0  |  28.7747 ns | 0.0596 ns | 0.0557 ns |   138.166 |    1.48 |   40 |      - |         - |          NA |
| Vogen_Parse                               | .NET 8.0  | .NET 8.0  |  33.8449 ns | 0.0425 ns | 0.0377 ns |   162.512 |    1.72 |   42 |      - |         - |          NA |
| StronglyTypedId_Parse                     | .NET 8.0  | .NET 8.0  |  31.5423 ns | 0.0523 ns | 0.0464 ns |   151.456 |    1.61 |   41 |      - |         - |          NA |
| ValueOf_Parse                             | .NET 8.0  | .NET 8.0  |  32.4844 ns | 0.0787 ns | 0.0697 ns |   155.979 |    1.67 |   41 | 0.0019 |      32 B |          NA |
| Meziantou_Parse                           | .NET 8.0  | .NET 8.0  |  29.5409 ns | 0.1044 ns | 0.0977 ns |   141.846 |    1.56 |   40 |      - |         - |          NA |
| TinyTypes_Parse                           | .NET 8.0  | .NET 8.0  |  29.2588 ns | 0.1428 ns | 0.1336 ns |   140.491 |    1.61 |   40 |      - |         - |          NA |
| DomainPrimitives_EqualityCheck            | .NET 8.0  | .NET 8.0  |   0.7859 ns | 0.0031 ns | 0.0026 ns |     3.774 |    0.04 |   13 |      - |         - |          NA |
| RawGuid_ToString                          | .NET 8.0  | .NET 8.0  |  13.8554 ns | 0.0553 ns | 0.0517 ns |    66.529 |    0.74 |   35 | 0.0057 |      96 B |          NA |
| DomainPrimitives_ToString                 | .NET 8.0  | .NET 8.0  |  14.2028 ns | 0.0803 ns | 0.0712 ns |    68.197 |    0.79 |   35 | 0.0057 |      96 B |          NA |
| Vogen_ToString                            | .NET 8.0  | .NET 8.0  |  24.0851 ns | 0.1732 ns | 0.1535 ns |   115.649 |    1.41 |   38 | 0.0057 |      96 B |          NA |
| StronglyTypedId_ToString                  | .NET 8.0  | .NET 8.0  |  14.7178 ns | 0.0903 ns | 0.0845 ns |    70.670 |    0.84 |   36 | 0.0057 |      96 B |          NA |
| ValueOf_ToString                          | .NET 8.0  | .NET 8.0  |  22.8450 ns | 0.1754 ns | 0.1640 ns |   109.694 |    1.38 |   38 | 0.0076 |     128 B |          NA |
| Meziantou_ToString                        | .NET 8.0  | .NET 8.0  |  35.7133 ns | 0.3185 ns | 0.2660 ns |   171.483 |    2.19 |   43 | 0.0148 |     248 B |          NA |
| TinyTypes_ToString                        | .NET 8.0  | .NET 8.0  |   9.6200 ns | 0.1010 ns | 0.0945 ns |    46.192 |    0.66 |   34 | 0.0019 |      32 B |          NA |
| DomainPrimitives_TryParse                 | .NET 8.0  | .NET 8.0  |  28.4829 ns | 0.0657 ns | 0.0583 ns |   136.765 |    1.47 |   40 |      - |         - |          NA |
| DomainPrimitives_SpanParse                | .NET 8.0  | .NET 8.0  |  29.1257 ns | 0.0458 ns | 0.0428 ns |   139.852 |    1.49 |   40 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanParse            | .NET 8.0  | .NET 8.0  |  61.9241 ns | 0.2078 ns | 0.1735 ns |   297.339 |    3.23 |   46 | 0.0038 |      64 B |          NA |
| DomainPrimitives_SpanFormat               | .NET 8.0  | .NET 8.0  |   4.9992 ns | 0.0055 ns | 0.0046 ns |    24.004 |    0.25 |   29 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanFormat           | .NET 8.0  | .NET 8.0  |   4.2006 ns | 0.0043 ns | 0.0041 ns |    20.170 |    0.21 |   26 |      - |         - |          NA |
| StringPrimitive_Email_Create              | .NET 8.0  | .NET 8.0  | 332.1675 ns | 1.0375 ns | 0.9705 ns | 1,594.957 |   17.40 |   58 |      - |         - |          NA |
| StringPrimitive_Email_TryParse            | .NET 8.0  | .NET 8.0  | 334.0396 ns | 0.2929 ns | 0.2596 ns | 1,603.946 |   16.94 |   58 |      - |         - |          NA |
| NumericPrimitive_Money_Create             | .NET 8.0  | .NET 8.0  |   4.3779 ns | 0.0050 ns | 0.0044 ns |    21.021 |    0.22 |   27 |      - |         - |          NA |
| NumericPrimitive_Money_Add                | .NET 8.0  | .NET 8.0  |   9.5235 ns | 0.0082 ns | 0.0069 ns |    45.729 |    0.48 |   34 |      - |         - |          NA |
| ValueObject_Create                        | .NET 8.0  | .NET 8.0  |   0.3573 ns | 0.0034 ns | 0.0032 ns |     1.716 |    0.02 |   10 |      - |         - |          NA |
| SmartEnum_FromValue                       | .NET 8.0  | .NET 8.0  |   3.7279 ns | 0.0030 ns | 0.0028 ns |    17.900 |    0.19 |   25 |      - |         - |          NA |
| RawGuid_JsonSerialize                     | .NET 8.0  | .NET 8.0  | 120.8415 ns | 0.8258 ns | 0.7725 ns |   580.240 |    7.09 |   51 | 0.0062 |     104 B |          NA |
| RawGuid_JsonDeserialize                   | .NET 8.0  | .NET 8.0  | 153.9877 ns | 0.1027 ns | 0.0858 ns |   739.397 |    7.80 |   54 |      - |         - |          NA |
| DomainPrimitives_JsonSerialize            | .NET 8.0  | .NET 8.0  | 136.4104 ns | 0.4496 ns | 0.4205 ns |   654.997 |    7.17 |   53 | 0.0062 |     104 B |          NA |
| DomainPrimitives_JsonDeserialize          | .NET 8.0  | .NET 8.0  | 164.1428 ns | 0.0969 ns | 0.0859 ns |   788.158 |    8.31 |   56 |      - |         - |          NA |
| RawGuid_Create                            | .NET 9.0  | .NET 9.0  |   0.1908 ns | 0.0031 ns | 0.0029 ns |     0.916 |    0.02 |    4 |      - |         - |          NA |
| DomainPrimitives_Create                   | .NET 9.0  | .NET 9.0  |   0.8376 ns | 0.0006 ns | 0.0004 ns |     4.022 |    0.04 |   14 |      - |         - |          NA |
| Vogen_Create                              | .NET 9.0  | .NET 9.0  |   0.6192 ns | 0.0087 ns | 0.0081 ns |     2.973 |    0.05 |   12 |      - |         - |          NA |
| StronglyTypedId_Create                    | .NET 9.0  | .NET 9.0  |   0.2160 ns | 0.0114 ns | 0.0089 ns |     1.037 |    0.04 |    5 |      - |         - |          NA |
| ValueOf_Create                            | .NET 9.0  | .NET 9.0  |   8.7878 ns | 0.0505 ns | 0.0422 ns |    42.196 |    0.49 |   34 | 0.0019 |      32 B |          NA |
| Meziantou_Create                          | .NET 9.0  | .NET 9.0  |   0.1368 ns | 0.0021 ns | 0.0019 ns |     0.657 |    0.01 |    2 |      - |         - |          NA |
| TinyTypes_Create                          | .NET 9.0  | .NET 9.0  |   0.1367 ns | 0.0036 ns | 0.0034 ns |     0.657 |    0.02 |    2 |      - |         - |          NA |
| RawGuid_Parse                             | .NET 9.0  | .NET 9.0  |  28.9645 ns | 0.0728 ns | 0.0681 ns |   139.078 |    1.50 |   40 |      - |         - |          NA |
| DomainPrimitives_Parse                    | .NET 9.0  | .NET 9.0  |  28.6941 ns | 0.0480 ns | 0.0449 ns |   137.779 |    1.47 |   40 |      - |         - |          NA |
| Vogen_Parse                               | .NET 9.0  | .NET 9.0  |  29.2201 ns | 0.0496 ns | 0.0414 ns |   140.305 |    1.49 |   40 |      - |         - |          NA |
| StronglyTypedId_Parse                     | .NET 9.0  | .NET 9.0  |  29.2135 ns | 0.0810 ns | 0.0757 ns |   140.273 |    1.52 |   40 |      - |         - |          NA |
| ValueOf_Parse                             | .NET 9.0  | .NET 9.0  |  31.4214 ns | 0.3446 ns | 0.3223 ns |   150.875 |    2.18 |   41 | 0.0019 |      32 B |          NA |
| Meziantou_Parse                           | .NET 9.0  | .NET 9.0  |  29.0322 ns | 0.0570 ns | 0.0533 ns |   139.403 |    1.49 |   40 |      - |         - |          NA |
| TinyTypes_Parse                           | .NET 9.0  | .NET 9.0  |  28.8547 ns | 0.1275 ns | 0.1130 ns |   138.551 |    1.55 |   40 |      - |         - |          NA |
| DomainPrimitives_EqualityCheck            | .NET 9.0  | .NET 9.0  |   0.9656 ns | 0.0039 ns | 0.0035 ns |     4.637 |    0.05 |   16 |      - |         - |          NA |
| RawGuid_ToString                          | .NET 9.0  | .NET 9.0  |  14.8986 ns | 0.3231 ns | 0.2698 ns |    71.538 |    1.46 |   36 | 0.0057 |      96 B |          NA |
| DomainPrimitives_ToString                 | .NET 9.0  | .NET 9.0  |  14.9517 ns | 0.3060 ns | 0.2862 ns |    71.793 |    1.53 |   36 | 0.0057 |      96 B |          NA |
| Vogen_ToString                            | .NET 9.0  | .NET 9.0  |  16.1984 ns | 0.3831 ns | 0.4561 ns |    77.779 |    2.29 |   36 | 0.0057 |      96 B |          NA |
| StronglyTypedId_ToString                  | .NET 9.0  | .NET 9.0  |  15.0669 ns | 0.1366 ns | 0.1278 ns |    72.346 |    0.97 |   36 | 0.0057 |      96 B |          NA |
| ValueOf_ToString                          | .NET 9.0  | .NET 9.0  |  22.5483 ns | 0.1712 ns | 0.1601 ns |   108.269 |    1.36 |   38 | 0.0076 |     128 B |          NA |
| Meziantou_ToString                        | .NET 9.0  | .NET 9.0  |  37.2362 ns | 0.8077 ns | 1.0783 ns |   178.796 |    5.42 |   43 | 0.0148 |     248 B |          NA |
| TinyTypes_ToString                        | .NET 9.0  | .NET 9.0  |  10.4017 ns | 0.0832 ns | 0.0778 ns |    49.946 |    0.64 |   34 | 0.0019 |      32 B |          NA |
| DomainPrimitives_TryParse                 | .NET 9.0  | .NET 9.0  |  27.8868 ns | 0.0385 ns | 0.0360 ns |   133.903 |    1.42 |   40 |      - |         - |          NA |
| DomainPrimitives_SpanParse                | .NET 9.0  | .NET 9.0  |  27.9826 ns | 0.0541 ns | 0.0506 ns |   134.363 |    1.43 |   40 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanParse            | .NET 9.0  | .NET 9.0  |  60.0055 ns | 0.3764 ns | 0.3521 ns |   288.126 |    3.45 |   46 | 0.0038 |      64 B |          NA |
| DomainPrimitives_SpanFormat               | .NET 9.0  | .NET 9.0  |   4.5713 ns | 0.0067 ns | 0.0060 ns |    21.950 |    0.23 |   28 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanFormat           | .NET 9.0  | .NET 9.0  |   3.7984 ns | 0.0062 ns | 0.0052 ns |    18.239 |    0.19 |   25 |      - |         - |          NA |
| StringPrimitive_Email_Create              | .NET 9.0  | .NET 9.0  | 169.0084 ns | 0.1637 ns | 0.1278 ns |   811.522 |    8.57 |   57 |      - |         - |          NA |
| StringPrimitive_Email_TryParse            | .NET 9.0  | .NET 9.0  | 170.3293 ns | 0.1477 ns | 0.1381 ns |   817.864 |    8.64 |   57 |      - |         - |          NA |
| NumericPrimitive_Money_Create             | .NET 9.0  | .NET 9.0  |   4.1476 ns | 0.0038 ns | 0.0036 ns |    19.915 |    0.21 |   26 |      - |         - |          NA |
| NumericPrimitive_Money_Add                | .NET 9.0  | .NET 9.0  |   8.8229 ns | 0.0068 ns | 0.0060 ns |    42.365 |    0.45 |   34 |      - |         - |          NA |
| ValueObject_Create                        | .NET 9.0  | .NET 9.0  |   0.3105 ns | 0.0015 ns | 0.0013 ns |     1.491 |    0.02 |    8 |      - |         - |          NA |
| SmartEnum_FromValue                       | .NET 9.0  | .NET 9.0  |   2.1046 ns | 0.0028 ns | 0.0025 ns |    10.106 |    0.11 |   21 |      - |         - |          NA |
| RawGuid_JsonSerialize                     | .NET 9.0  | .NET 9.0  | 112.7318 ns | 0.3557 ns | 0.3327 ns |   541.300 |    5.91 |   50 | 0.0062 |     104 B |          NA |
| RawGuid_JsonDeserialize                   | .NET 9.0  | .NET 9.0  | 121.1462 ns | 0.0686 ns | 0.0573 ns |   581.703 |    6.14 |   51 |      - |         - |          NA |
| DomainPrimitives_JsonSerialize            | .NET 9.0  | .NET 9.0  | 108.2499 ns | 0.9988 ns | 0.9343 ns |   519.779 |    6.99 |   50 | 0.0062 |     104 B |          NA |
| DomainPrimitives_JsonDeserialize          | .NET 9.0  | .NET 9.0  | 129.6564 ns | 0.0933 ns | 0.0827 ns |   622.566 |    6.57 |   52 |      - |         - |          NA |
| RawGuid_Create                            | .NET 9.0  | .NET 9.0  |   0.1601 ns | 0.0062 ns | 0.0055 ns |     0.769 |    0.03 |    3 |      - |         - |          NA |
| DomainPrimitives_Create                   | .NET 9.0  | .NET 9.0  |   0.8419 ns | 0.0029 ns | 0.0024 ns |     4.043 |    0.04 |   14 |      - |         - |          NA |
| Vogen_Create                              | .NET 9.0  | .NET 9.0  |   0.2148 ns | 0.0010 ns | 0.0008 ns |     1.031 |    0.01 |    5 |      - |         - |          NA |
| StronglyTypedId_Create                    | .NET 9.0  | .NET 9.0  |   0.1601 ns | 0.0048 ns | 0.0044 ns |     0.769 |    0.02 |    3 |      - |         - |          NA |
| ValueOf_Create                            | .NET 9.0  | .NET 9.0  |   8.9241 ns | 0.1387 ns | 0.1297 ns |    42.850 |    0.75 |   34 | 0.0019 |      32 B |          NA |
| Meziantou_Create                          | .NET 9.0  | .NET 9.0  |   0.1530 ns | 0.0057 ns | 0.0054 ns |     0.735 |    0.03 |    3 |      - |         - |          NA |
| TinyTypes_Create                          | .NET 9.0  | .NET 9.0  |   0.1334 ns | 0.0028 ns | 0.0026 ns |     0.641 |    0.01 |    2 |      - |         - |          NA |
| RawGuid_Parse                             | .NET 9.0  | .NET 9.0  |  28.8755 ns | 0.0290 ns | 0.0272 ns |   138.651 |    1.47 |   40 |      - |         - |          NA |
| DomainPrimitives_Parse                    | .NET 9.0  | .NET 9.0  |  28.5844 ns | 0.0340 ns | 0.0302 ns |   137.253 |    1.45 |   40 |      - |         - |          NA |
| Vogen_Parse                               | .NET 9.0  | .NET 9.0  |  29.5689 ns | 0.0699 ns | 0.0620 ns |   141.980 |    1.52 |   40 |      - |         - |          NA |
| StronglyTypedId_Parse                     | .NET 9.0  | .NET 9.0  |  28.9430 ns | 0.0536 ns | 0.0502 ns |   138.974 |    1.48 |   40 |      - |         - |          NA |
| ValueOf_Parse                             | .NET 9.0  | .NET 9.0  |  31.7749 ns | 0.1343 ns | 0.1191 ns |   152.572 |    1.70 |   41 | 0.0019 |      32 B |          NA |
| Meziantou_Parse                           | .NET 9.0  | .NET 9.0  |  29.2549 ns | 0.0557 ns | 0.0494 ns |   140.472 |    1.50 |   40 |      - |         - |          NA |
| TinyTypes_Parse                           | .NET 9.0  | .NET 9.0  |  28.9446 ns | 0.0784 ns | 0.0695 ns |   138.982 |    1.50 |   40 |      - |         - |          NA |
| DomainPrimitives_EqualityCheck            | .NET 9.0  | .NET 9.0  |   0.9698 ns | 0.0050 ns | 0.0047 ns |     4.657 |    0.05 |   16 |      - |         - |          NA |
| RawGuid_ToString                          | .NET 9.0  | .NET 9.0  |  15.3910 ns | 0.3383 ns | 0.3164 ns |    73.903 |    1.66 |   36 | 0.0057 |      96 B |          NA |
| DomainPrimitives_ToString                 | .NET 9.0  | .NET 9.0  |  15.1777 ns | 0.2508 ns | 0.2346 ns |    72.878 |    1.33 |   36 | 0.0057 |      96 B |          NA |
| Vogen_ToString                            | .NET 9.0  | .NET 9.0  |  16.4495 ns | 0.3175 ns | 0.2969 ns |    78.985 |    1.61 |   36 | 0.0057 |      96 B |          NA |
| StronglyTypedId_ToString                  | .NET 9.0  | .NET 9.0  |  15.5622 ns | 0.2487 ns | 0.2326 ns |    74.724 |    1.34 |   36 | 0.0057 |      96 B |          NA |
| ValueOf_ToString                          | .NET 9.0  | .NET 9.0  |  23.1319 ns | 0.5146 ns | 0.4814 ns |   111.071 |    2.53 |   38 | 0.0076 |     128 B |          NA |
| Meziantou_ToString                        | .NET 9.0  | .NET 9.0  |  37.8194 ns | 0.7004 ns | 0.6552 ns |   181.596 |    3.60 |   43 | 0.0148 |     248 B |          NA |
| TinyTypes_ToString                        | .NET 9.0  | .NET 9.0  |  10.7230 ns | 0.1970 ns | 0.1842 ns |    51.488 |    1.01 |   34 | 0.0019 |      32 B |          NA |
| DomainPrimitives_TryParse                 | .NET 9.0  | .NET 9.0  |  27.8236 ns | 0.0571 ns | 0.0506 ns |   133.600 |    1.43 |   40 |      - |         - |          NA |
| DomainPrimitives_SpanParse                | .NET 9.0  | .NET 9.0  |  27.7170 ns | 0.0213 ns | 0.0189 ns |   133.088 |    1.40 |   40 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanParse            | .NET 9.0  | .NET 9.0  |  60.6899 ns | 0.1391 ns | 0.1161 ns |   291.413 |    3.12 |   46 | 0.0038 |      64 B |          NA |
| DomainPrimitives_SpanFormat               | .NET 9.0  | .NET 9.0  |   4.4471 ns | 0.0071 ns | 0.0066 ns |    21.353 |    0.23 |   27 |      - |         - |          NA |
| DomainPrimitives_Utf8SpanFormat           | .NET 9.0  | .NET 9.0  |   4.0860 ns | 0.0062 ns | 0.0055 ns |    19.620 |    0.21 |   26 |      - |         - |          NA |
| StringPrimitive_Email_Create              | .NET 9.0  | .NET 9.0  | 169.6010 ns | 0.1462 ns | 0.1221 ns |   814.367 |    8.60 |   57 |      - |         - |          NA |
| StringPrimitive_Email_TryParse            | .NET 9.0  | .NET 9.0  | 168.6906 ns | 0.1393 ns | 0.1163 ns |   809.996 |    8.55 |   57 |      - |         - |          NA |
| NumericPrimitive_Money_Create             | .NET 9.0  | .NET 9.0  |   4.3406 ns | 0.0030 ns | 0.0025 ns |    20.842 |    0.22 |   27 |      - |         - |          NA |
| NumericPrimitive_Money_Add                | .NET 9.0  | .NET 9.0  |   8.7086 ns | 0.0105 ns | 0.0087 ns |    41.816 |    0.44 |   34 |      - |         - |          NA |
| ValueObject_Create                        | .NET 9.0  | .NET 9.0  |   0.3111 ns | 0.0012 ns | 0.0011 ns |     1.494 |    0.02 |    8 |      - |         - |          NA |
| SmartEnum_FromValue                       | .NET 9.0  | .NET 9.0  |   2.1054 ns | 0.0019 ns | 0.0018 ns |    10.109 |    0.11 |   21 |      - |         - |          NA |
| RawGuid_JsonSerialize                     | .NET 9.0  | .NET 9.0  | 110.2116 ns | 0.2384 ns | 0.2114 ns |   529.199 |    5.66 |   50 | 0.0062 |     104 B |          NA |
| RawGuid_JsonDeserialize                   | .NET 9.0  | .NET 9.0  | 121.2263 ns | 0.0725 ns | 0.0605 ns |   582.088 |    6.14 |   51 |      - |         - |          NA |
| DomainPrimitives_JsonSerialize            | .NET 9.0  | .NET 9.0  | 107.4399 ns | 0.4027 ns | 0.3363 ns |   515.890 |    5.65 |   50 | 0.0062 |     104 B |          NA |
| DomainPrimitives_JsonDeserialize          | .NET 9.0  | .NET 9.0  | 122.4345 ns | 0.1498 ns | 0.1328 ns |   587.890 |    6.22 |   51 |      - |         - |          NA |
| Dapper_TypeHandler_SetValue               | .NET 10.0 | .NET 10.0 |   0.0000 ns | 0.0000 ns | 0.0000 ns |     0.000 |    0.00 |    1 |      - |         - |          NA |
| Dapper_TypeHandler_Parse                  | .NET 10.0 | .NET 10.0 |   0.9214 ns | 0.0033 ns | 0.0027 ns |     4.424 |    0.05 |   15 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertToProvider   | .NET 10.0 | .NET 10.0 |   1.2939 ns | 0.0036 ns | 0.0032 ns |     6.213 |    0.07 |   20 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertFromProvider | .NET 10.0 | .NET 10.0 |   0.9168 ns | 0.0031 ns | 0.0029 ns |     4.402 |    0.05 |   15 |      - |         - |          NA |
| Dapper_TypeHandler_SetValue               | .NET 8.0  | .NET 8.0  |   7.8425 ns | 0.0485 ns | 0.0454 ns |    37.657 |    0.45 |   31 | 0.0019 |      32 B |          NA |
| Dapper_TypeHandler_Parse                  | .NET 8.0  | .NET 8.0  |   8.3409 ns | 0.0024 ns | 0.0019 ns |    40.050 |    0.42 |   33 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertToProvider   | .NET 8.0  | .NET 8.0  |  10.8096 ns | 0.0054 ns | 0.0051 ns |    51.904 |    0.55 |   34 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertFromProvider | .NET 8.0  | .NET 8.0  |   1.0950 ns | 0.0011 ns | 0.0009 ns |     5.258 |    0.06 |   17 |      - |         - |          NA |
| Dapper_TypeHandler_SetValue               | .NET 9.0  | .NET 9.0  |   1.1574 ns | 0.0061 ns | 0.0057 ns |     5.557 |    0.06 |   18 |      - |         - |          NA |
| Dapper_TypeHandler_Parse                  | .NET 9.0  | .NET 9.0  |   1.1009 ns | 0.0029 ns | 0.0027 ns |     5.286 |    0.06 |   17 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertToProvider   | .NET 9.0  | .NET 9.0  |   1.1660 ns | 0.0035 ns | 0.0032 ns |     5.599 |    0.06 |   18 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertFromProvider | .NET 9.0  | .NET 9.0  |   0.9500 ns | 0.0023 ns | 0.0020 ns |     4.562 |    0.05 |   16 |      - |         - |          NA |
| Dapper_TypeHandler_SetValue               | .NET 9.0  | .NET 9.0  |   1.1529 ns | 0.0047 ns | 0.0044 ns |     5.536 |    0.06 |   18 |      - |         - |          NA |
| Dapper_TypeHandler_Parse                  | .NET 9.0  | .NET 9.0  |   0.9132 ns | 0.0034 ns | 0.0029 ns |     4.385 |    0.05 |   15 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertToProvider   | .NET 9.0  | .NET 9.0  |   1.1675 ns | 0.0057 ns | 0.0053 ns |     5.606 |    0.06 |   18 |      - |         - |          NA |
| EFCore_ValueConverter_ConvertFromProvider | .NET 9.0  | .NET 9.0  |   0.9164 ns | 0.0025 ns | 0.0021 ns |     4.400 |    0.05 |   15 |      - |         - |          NA |
