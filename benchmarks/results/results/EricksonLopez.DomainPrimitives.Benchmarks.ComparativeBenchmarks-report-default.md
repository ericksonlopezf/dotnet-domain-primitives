
BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.400
  [Host]    : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  .NET 8.0  : .NET 8.0.30 (8.0.30, 8.0.3026.36720), X64 RyuJIT x86-64-v3
  .NET 9.0  : .NET 9.0.19 (9.0.19, 9.0.1926.36724), X64 RyuJIT x86-64-v3


 Method                                    | Job       | Runtime   | Mean        | Error     | StdDev    | Median      | Ratio     | RatioSD | Rank | Gen0   | Allocated | Alloc Ratio |
------------------------------------------ |---------- |---------- |------------:|----------:|----------:|------------:|----------:|--------:|-----:|-------:|----------:|------------:|
 RawGuid_Create                            | .NET 10.0 | .NET 10.0 |   0.1131 ns | 0.0066 ns | 0.0062 ns |   0.1135 ns |      4.75 |    0.30 |    5 |      - |         - |          NA |
 DomainPrimitives_Create                   | .NET 10.0 | .NET 10.0 |   1.2979 ns | 0.0013 ns | 0.0012 ns |   1.2979 ns |     54.56 |    1.84 |   14 |      - |         - |          NA |
 Vogen_Create                              | .NET 10.0 | .NET 10.0 |   0.3535 ns | 0.0014 ns | 0.0013 ns |   0.3535 ns |     14.86 |    0.50 |    8 |      - |         - |          NA |
 StronglyTypedId_Create                    | .NET 10.0 | .NET 10.0 |   0.0033 ns | 0.0021 ns | 0.0019 ns |   0.0029 ns |      0.14 |    0.08 |    2 |      - |         - |          NA |
 ValueOf_Create                            | .NET 10.0 | .NET 10.0 |   7.6833 ns | 0.0895 ns | 0.0837 ns |   7.6855 ns |    322.97 |   11.40 |   27 | 0.0019 |      32 B |          NA |
 Meziantou_Create                          | .NET 10.0 | .NET 10.0 |   0.1786 ns | 0.0096 ns | 0.0090 ns |   0.1772 ns |      7.51 |    0.44 |    6 |      - |         - |          NA |
 TinyTypes_Create                          | .NET 10.0 | .NET 10.0 |   0.0667 ns | 0.0052 ns | 0.0049 ns |   0.0665 ns |      2.81 |    0.22 |    4 |      - |         - |          NA |
 RawGuid_Parse                             | .NET 10.0 | .NET 10.0 |  27.6806 ns | 0.0280 ns | 0.0234 ns |  27.6806 ns |  1,163.57 |   39.22 |   36 |      - |         - |          NA |
 DomainPrimitives_Parse                    | .NET 10.0 | .NET 10.0 |  27.4179 ns | 0.0266 ns | 0.0236 ns |  27.4213 ns |  1,152.53 |   38.84 |   36 |      - |         - |          NA |
 Vogen_Parse                               | .NET 10.0 | .NET 10.0 |  27.3671 ns | 0.0220 ns | 0.0206 ns |  27.3663 ns |  1,150.39 |   38.76 |   36 |      - |         - |          NA |
 StronglyTypedId_Parse                     | .NET 10.0 | .NET 10.0 |  27.3035 ns | 0.0149 ns | 0.0125 ns |  27.3064 ns |  1,147.72 |   38.68 |   36 |      - |         - |          NA |
 ValueOf_Parse                             | .NET 10.0 | .NET 10.0 |  30.7924 ns | 0.0399 ns | 0.0334 ns |  30.7925 ns |  1,294.38 |   43.64 |   37 | 0.0019 |      32 B |          NA |
 Meziantou_Parse                           | .NET 10.0 | .NET 10.0 |  42.0296 ns | 0.1242 ns | 0.1037 ns |  41.9882 ns |  1,766.74 |   59.68 |   41 |      - |         - |          NA |
 TinyTypes_Parse                           | .NET 10.0 | .NET 10.0 |  27.3115 ns | 0.0334 ns | 0.0296 ns |  27.3084 ns |  1,148.05 |   38.70 |   36 |      - |         - |          NA |
 DomainPrimitives_EqualityCheck            | .NET 10.0 | .NET 10.0 |   0.9573 ns | 0.0023 ns | 0.0019 ns |   0.9573 ns |     40.24 |    1.36 |   11 |      - |         - |          NA |
 RawGuid_ToString                          | .NET 10.0 | .NET 10.0 |   9.6194 ns | 0.1240 ns | 0.1160 ns |   9.5662 ns |    404.36 |   14.42 |   28 | 0.0057 |      96 B |          NA |
 DomainPrimitives_ToString                 | .NET 10.0 | .NET 10.0 |   9.9275 ns | 0.1204 ns | 0.1126 ns |   9.8994 ns |    417.31 |   14.79 |   28 | 0.0057 |      96 B |          NA |
 Vogen_ToString                            | .NET 10.0 | .NET 10.0 |  10.6422 ns | 0.0371 ns | 0.0310 ns |  10.6326 ns |    447.35 |   15.13 |   29 | 0.0057 |      96 B |          NA |
 StronglyTypedId_ToString                  | .NET 10.0 | .NET 10.0 |   9.6392 ns | 0.2588 ns | 0.2421 ns |   9.6102 ns |    405.19 |   16.84 |   28 | 0.0057 |      96 B |          NA |
 ValueOf_ToString                          | .NET 10.0 | .NET 10.0 |  20.4961 ns | 0.2666 ns | 0.2363 ns |  20.5045 ns |    861.56 |   30.57 |   33 | 0.0076 |     128 B |          NA |
 Meziantou_ToString                        | .NET 10.0 | .NET 10.0 |  25.3029 ns | 0.3121 ns | 0.2920 ns |  25.3115 ns |  1,063.62 |   37.75 |   35 | 0.0148 |     248 B |          NA |
 TinyTypes_ToString                        | .NET 10.0 | .NET 10.0 |   7.3738 ns | 0.0285 ns | 0.0267 ns |   7.3764 ns |    309.96 |   10.50 |   26 | 0.0019 |      32 B |          NA |
 DomainPrimitives_TryParse                 | .NET 10.0 | .NET 10.0 |  27.0374 ns | 0.0225 ns | 0.0188 ns |  27.0365 ns |  1,136.53 |   38.30 |   36 |      - |         - |          NA |
 DomainPrimitives_SpanParse                | .NET 10.0 | .NET 10.0 |  27.2367 ns | 0.0273 ns | 0.0242 ns |  27.2338 ns |  1,144.91 |   38.58 |   36 |      - |         - |          NA |
 DomainPrimitives_Utf8SpanParse            | .NET 10.0 | .NET 10.0 |  47.7645 ns | 0.0524 ns | 0.0465 ns |  47.7454 ns |  2,007.81 |   67.67 |   42 | 0.0038 |      64 B |          NA |
 DomainPrimitives_SpanFormat               | .NET 10.0 | .NET 10.0 |   2.9301 ns | 0.0074 ns | 0.0058 ns |   2.9311 ns |    123.17 |    4.16 |   19 |      - |         - |          NA |
 DomainPrimitives_Utf8SpanFormat           | .NET 10.0 | .NET 10.0 |   2.7194 ns | 0.0138 ns | 0.0122 ns |   2.7175 ns |    114.31 |    3.88 |   18 |      - |         - |          NA |
 StringPrimitive_Email_Create              | .NET 10.0 | .NET 10.0 | 171.9724 ns | 0.2375 ns | 0.1983 ns | 171.9144 ns |  7,228.95 |  243.72 |   51 |      - |         - |          NA |
 StringPrimitive_Email_TryParse            | .NET 10.0 | .NET 10.0 | 196.6874 ns | 0.5555 ns | 0.4639 ns | 196.8070 ns |  8,267.87 |  279.23 |   54 |      - |         - |          NA |
 NumericPrimitive_Money_Create             | .NET 10.0 | .NET 10.0 |   4.1133 ns | 0.0048 ns | 0.0040 ns |   4.1120 ns |    172.90 |    5.83 |   20 |      - |         - |          NA |
 NumericPrimitive_Money_Add                | .NET 10.0 | .NET 10.0 |   9.6672 ns | 0.0061 ns | 0.0048 ns |   9.6678 ns |    406.36 |   13.70 |   28 |      - |         - |          NA |
 ValueObject_Create                        | .NET 10.0 | .NET 10.0 |   0.3524 ns | 0.0011 ns | 0.0009 ns |   0.3524 ns |     14.81 |    0.50 |    8 |      - |         - |          NA |
 SmartEnum_FromValue                       | .NET 10.0 | .NET 10.0 |   0.9686 ns | 0.0018 ns | 0.0015 ns |   0.9686 ns |     40.72 |    1.37 |   11 |      - |         - |          NA |
 RawGuid_JsonSerialize                     | .NET 10.0 | .NET 10.0 |  91.5496 ns | 0.1220 ns | 0.1082 ns |  91.5563 ns |  3,848.34 |  129.72 |   45 | 0.0062 |     104 B |          NA |
 RawGuid_JsonDeserialize                   | .NET 10.0 | .NET 10.0 | 107.5959 ns | 0.2157 ns | 0.1912 ns | 107.5303 ns |  4,522.85 |  152.57 |   46 |      - |         - |          NA |
 DomainPrimitives_JsonSerialize            | .NET 10.0 | .NET 10.0 | 136.6560 ns | 0.7130 ns | 0.6321 ns | 136.5084 ns |  5,744.41 |  195.22 |   50 | 0.0062 |     104 B |          NA |
 DomainPrimitives_JsonDeserialize          | .NET 10.0 | .NET 10.0 | 105.1486 ns | 0.2206 ns | 0.1955 ns | 105.0905 ns |  4,419.98 |  149.12 |   46 |      - |         - |          NA |
 RawGuid_Create                            | .NET 8.0  | .NET 8.0  |   0.0238 ns | 0.0009 ns | 0.0008 ns |   0.0237 ns |      1.00 |    0.05 |    3 |      - |         - |          NA |
 DomainPrimitives_Create                   | .NET 8.0  | .NET 8.0  |   0.9680 ns | 0.0009 ns | 0.0008 ns |   0.9680 ns |     40.69 |    1.37 |   11 |      - |         - |          NA |
 Vogen_Create                              | .NET 8.0  | .NET 8.0  |   6.5900 ns | 0.0023 ns | 0.0021 ns |   6.5900 ns |    277.01 |    9.33 |   25 |      - |         - |          NA |
 StronglyTypedId_Create                    | .NET 8.0  | .NET 8.0  |   0.0227 ns | 0.0006 ns | 0.0005 ns |   0.0226 ns |      0.96 |    0.04 |    3 |      - |         - |          NA |
 ValueOf_Create                            | .NET 8.0  | .NET 8.0  |  13.8098 ns | 0.0794 ns | 0.0704 ns |  13.7907 ns |    580.50 |   19.77 |   31 | 0.0019 |      32 B |          NA |
 Meziantou_Create                          | .NET 8.0  | .NET 8.0  |   0.0572 ns | 0.0048 ns | 0.0045 ns |   0.0557 ns |      2.41 |    0.20 |    4 |      - |         - |          NA |
 TinyTypes_Create                          | .NET 8.0  | .NET 8.0  |   0.0245 ns | 0.0019 ns | 0.0015 ns |   0.0243 ns |      1.03 |    0.07 |    3 |      - |         - |          NA |
 RawGuid_Parse                             | .NET 8.0  | .NET 8.0  |  28.7834 ns | 0.0293 ns | 0.0260 ns |  28.7760 ns |  1,209.93 |   40.78 |   36 |      - |         - |          NA |
 DomainPrimitives_Parse                    | .NET 8.0  | .NET 8.0  |  34.7374 ns | 0.0346 ns | 0.0306 ns |  34.7382 ns |  1,460.21 |   49.21 |   39 |      - |         - |          NA |
 Vogen_Parse                               | .NET 8.0  | .NET 8.0  |  32.5557 ns | 0.0481 ns | 0.0401 ns |  32.5611 ns |  1,368.50 |   46.14 |   38 |      - |         - |          NA |
 StronglyTypedId_Parse                     | .NET 8.0  | .NET 8.0  |  28.7713 ns | 0.0172 ns | 0.0153 ns |  28.7702 ns |  1,209.42 |   40.75 |   36 |      - |         - |          NA |
 ValueOf_Parse                             | .NET 8.0  | .NET 8.0  |  34.6165 ns | 0.0869 ns | 0.0770 ns |  34.5914 ns |  1,455.12 |   49.12 |   39 | 0.0019 |      32 B |          NA |
 Meziantou_Parse                           | .NET 8.0  | .NET 8.0  |  29.3948 ns | 0.0779 ns | 0.0690 ns |  29.3729 ns |  1,235.63 |   41.72 |   36 |      - |         - |          NA |
 TinyTypes_Parse                           | .NET 8.0  | .NET 8.0  |  28.9994 ns | 0.0456 ns | 0.0404 ns |  29.0002 ns |  1,219.00 |   41.10 |   36 |      - |         - |          NA |
 DomainPrimitives_EqualityCheck            | .NET 8.0  | .NET 8.0  |   0.9071 ns | 0.0030 ns | 0.0027 ns |   0.9061 ns |     38.13 |    1.29 |   10 |      - |         - |          NA |
 RawGuid_ToString                          | .NET 8.0  | .NET 8.0  |  14.4261 ns | 0.3082 ns | 0.2883 ns |  14.3998 ns |    606.41 |   23.56 |   31 | 0.0057 |      96 B |          NA |
 DomainPrimitives_ToString                 | .NET 8.0  | .NET 8.0  |  13.9296 ns | 0.1855 ns | 0.1735 ns |  13.9218 ns |    585.54 |   20.95 |   31 | 0.0057 |      96 B |          NA |
 Vogen_ToString                            | .NET 8.0  | .NET 8.0  |  26.7191 ns | 0.1023 ns | 0.0957 ns |  26.7305 ns |  1,123.15 |   38.03 |   36 | 0.0057 |      96 B |          NA |
 StronglyTypedId_ToString                  | .NET 8.0  | .NET 8.0  |  13.7529 ns | 0.1232 ns | 0.1153 ns |  13.7175 ns |    578.11 |   20.03 |   31 | 0.0057 |      96 B |          NA |
 ValueOf_ToString                          | .NET 8.0  | .NET 8.0  |  22.6065 ns | 0.2933 ns | 0.2449 ns |  22.6292 ns |    950.28 |   33.52 |   34 | 0.0076 |     128 B |          NA |
 Meziantou_ToString                        | .NET 8.0  | .NET 8.0  |  36.3310 ns | 0.2197 ns | 0.1948 ns |  36.2940 ns |  1,527.20 |   52.06 |   40 | 0.0148 |     248 B |          NA |
 TinyTypes_ToString                        | .NET 8.0  | .NET 8.0  |  10.1878 ns | 0.1685 ns | 0.1407 ns |  10.2366 ns |    428.25 |   15.52 |   28 | 0.0019 |      32 B |          NA |
 DomainPrimitives_TryParse                 | .NET 8.0  | .NET 8.0  |  28.0772 ns | 0.0640 ns | 0.0599 ns |  28.0769 ns |  1,180.24 |   39.83 |   36 |      - |         - |          NA |
 DomainPrimitives_SpanParse                | .NET 8.0  | .NET 8.0  |  27.8788 ns | 0.0392 ns | 0.0366 ns |  27.8786 ns |  1,171.90 |   39.50 |   36 |      - |         - |          NA |
 DomainPrimitives_Utf8SpanParse            | .NET 8.0  | .NET 8.0  |  63.0133 ns | 0.1416 ns | 0.1255 ns |  62.9800 ns |  2,648.80 |   89.38 |   44 | 0.0038 |      64 B |          NA |
 DomainPrimitives_SpanFormat               | .NET 8.0  | .NET 8.0  |   5.4808 ns | 0.0043 ns | 0.0038 ns |   5.4797 ns |    230.39 |    7.76 |   24 |      - |         - |          NA |
 DomainPrimitives_Utf8SpanFormat           | .NET 8.0  | .NET 8.0  |   4.4216 ns | 0.0073 ns | 0.0069 ns |   4.4198 ns |    185.86 |    6.27 |   21 |      - |         - |          NA |
 StringPrimitive_Email_Create              | .NET 8.0  | .NET 8.0  | 327.7533 ns | 0.2119 ns | 0.1769 ns | 327.7792 ns | 13,777.30 |  464.30 |   55 |      - |         - |          NA |
 StringPrimitive_Email_TryParse            | .NET 8.0  | .NET 8.0  | 326.0099 ns | 0.2974 ns | 0.2636 ns | 325.9798 ns | 13,704.01 |  461.81 |   55 |      - |         - |          NA |
 NumericPrimitive_Money_Create             | .NET 8.0  | .NET 8.0  |   4.8383 ns | 0.0063 ns | 0.0053 ns |   4.8366 ns |    203.38 |    6.86 |   22 |      - |         - |          NA |
 NumericPrimitive_Money_Add                | .NET 8.0  | .NET 8.0  |  10.2936 ns | 0.0049 ns | 0.0041 ns |  10.2919 ns |    432.70 |   14.58 |   28 |      - |         - |          NA |
 ValueObject_Create                        | .NET 8.0  | .NET 8.0  |   0.3735 ns | 0.0010 ns | 0.0009 ns |   0.3734 ns |     15.70 |    0.53 |    9 |      - |         - |          NA |
 SmartEnum_FromValue                       | .NET 8.0  | .NET 8.0  |  16.9675 ns | 0.3667 ns | 0.3766 ns |  17.0516 ns |    713.24 |   28.53 |   32 | 0.0019 |      32 B |          NA |
 RawGuid_JsonSerialize                     | .NET 8.0  | .NET 8.0  | 124.2277 ns | 0.2560 ns | 0.2394 ns | 124.2252 ns |  5,221.98 |  176.17 |   49 | 0.0062 |     104 B |          NA |
 RawGuid_JsonDeserialize                   | .NET 8.0  | .NET 8.0  | 168.8367 ns | 0.1954 ns | 0.1828 ns | 168.7750 ns |  7,097.14 |  239.18 |   51 |      - |         - |          NA |
 DomainPrimitives_JsonSerialize            | .NET 8.0  | .NET 8.0  | 190.6194 ns | 0.2646 ns | 0.2346 ns | 190.5807 ns |  8,012.79 |  270.12 |   53 | 0.0062 |     104 B |          NA |
 DomainPrimitives_JsonDeserialize          | .NET 8.0  | .NET 8.0  | 190.5346 ns | 0.1185 ns | 0.1108 ns | 190.5068 ns |  8,009.23 |  269.82 |   53 |      - |         - |          NA |
 RawGuid_Create                            | .NET 9.0  | .NET 9.0  |   0.0545 ns | 0.0061 ns | 0.0057 ns |   0.0524 ns |      2.29 |    0.24 |    4 |      - |         - |          NA |
 DomainPrimitives_Create                   | .NET 9.0  | .NET 9.0  |   1.3416 ns | 0.0012 ns | 0.0010 ns |   1.3415 ns |     56.40 |    1.90 |   15 |      - |         - |          NA |
 Vogen_Create                              | .NET 9.0  | .NET 9.0  |   0.3744 ns | 0.0012 ns | 0.0010 ns |   0.3746 ns |     15.74 |    0.53 |    9 |      - |         - |          NA |
 StronglyTypedId_Create                    | .NET 9.0  | .NET 9.0  |   0.0277 ns | 0.0019 ns | 0.0018 ns |   0.0276 ns |      1.17 |    0.08 |    3 |      - |         - |          NA |
 ValueOf_Create                            | .NET 9.0  | .NET 9.0  |  13.2224 ns | 0.1747 ns | 0.1634 ns |  13.1475 ns |    555.81 |   19.87 |   31 | 0.0019 |      32 B |          NA |
 Meziantou_Create                          | .NET 9.0  | .NET 9.0  |   0.2051 ns | 0.0048 ns | 0.0045 ns |   0.2042 ns |      8.62 |    0.34 |    7 |      - |         - |          NA |
 TinyTypes_Create                          | .NET 9.0  | .NET 9.0  |   0.0256 ns | 0.0019 ns | 0.0017 ns |   0.0248 ns |      1.08 |    0.08 |    3 |      - |         - |          NA |
 RawGuid_Parse                             | .NET 9.0  | .NET 9.0  |  28.7368 ns | 0.0223 ns | 0.0198 ns |  28.7308 ns |  1,207.97 |   40.70 |   36 |      - |         - |          NA |
 DomainPrimitives_Parse                    | .NET 9.0  | .NET 9.0  |  29.1308 ns | 0.0761 ns | 0.0636 ns |  29.1169 ns |  1,224.53 |   41.34 |   36 |      - |         - |          NA |
 Vogen_Parse                               | .NET 9.0  | .NET 9.0  |  29.2081 ns | 0.0341 ns | 0.0302 ns |  29.2049 ns |  1,227.78 |   41.38 |   36 |      - |         - |          NA |
 StronglyTypedId_Parse                     | .NET 9.0  | .NET 9.0  |  28.3639 ns | 0.0139 ns | 0.0123 ns |  28.3647 ns |  1,192.29 |   40.17 |   36 |      - |         - |          NA |
 ValueOf_Parse                             | .NET 9.0  | .NET 9.0  |  32.5587 ns | 0.0698 ns | 0.0618 ns |  32.5738 ns |  1,368.62 |   46.18 |   38 | 0.0019 |      32 B |          NA |
 Meziantou_Parse                           | .NET 9.0  | .NET 9.0  |  29.3583 ns | 0.0630 ns | 0.0526 ns |  29.3371 ns |  1,234.09 |   41.64 |   36 |      - |         - |          NA |
 TinyTypes_Parse                           | .NET 9.0  | .NET 9.0  |  27.9946 ns | 0.0372 ns | 0.0311 ns |  27.9980 ns |  1,176.77 |   39.67 |   36 |      - |         - |          NA |
 DomainPrimitives_EqualityCheck            | .NET 9.0  | .NET 9.0  |   0.9085 ns | 0.0017 ns | 0.0016 ns |   0.9081 ns |     38.19 |    1.29 |   10 |      - |         - |          NA |
 RawGuid_ToString                          | .NET 9.0  | .NET 9.0  |  13.4007 ns | 0.1533 ns | 0.1434 ns |  13.3474 ns |    563.31 |   19.85 |   31 | 0.0057 |      96 B |          NA |
 DomainPrimitives_ToString                 | .NET 9.0  | .NET 9.0  |  14.0777 ns | 0.0866 ns | 0.0676 ns |  14.0956 ns |    591.77 |   20.13 |   31 | 0.0057 |      96 B |          NA |
 Vogen_ToString                            | .NET 9.0  | .NET 9.0  |  15.2188 ns | 0.1668 ns | 0.1479 ns |  15.2324 ns |    639.73 |   22.37 |   31 | 0.0057 |      96 B |          NA |
 StronglyTypedId_ToString                  | .NET 9.0  | .NET 9.0  |  14.1587 ns | 0.1154 ns | 0.0964 ns |  14.1770 ns |    595.17 |   20.43 |   31 | 0.0057 |      96 B |          NA |
 ValueOf_ToString                          | .NET 9.0  | .NET 9.0  |  22.5670 ns | 0.0759 ns | 0.0634 ns |  22.5553 ns |    948.62 |   32.07 |   34 | 0.0076 |     128 B |          NA |
 Meziantou_ToString                        | .NET 9.0  | .NET 9.0  |  34.5372 ns | 0.2982 ns | 0.2644 ns |  34.4812 ns |  1,451.79 |   50.08 |   39 | 0.0148 |     248 B |          NA |
 TinyTypes_ToString                        | .NET 9.0  | .NET 9.0  |  12.2855 ns | 0.0456 ns | 0.0405 ns |  12.2657 ns |    516.43 |   17.48 |   30 | 0.0019 |      32 B |          NA |
 DomainPrimitives_TryParse                 | .NET 9.0  | .NET 9.0  |  28.0101 ns | 0.0307 ns | 0.0272 ns |  28.0104 ns |  1,177.42 |   39.68 |   36 |      - |         - |          NA |
 DomainPrimitives_SpanParse                | .NET 9.0  | .NET 9.0  |  27.7506 ns | 0.0151 ns | 0.0142 ns |  27.7502 ns |  1,166.51 |   39.30 |   36 |      - |         - |          NA |
 DomainPrimitives_Utf8SpanParse            | .NET 9.0  | .NET 9.0  |  59.8793 ns | 0.1243 ns | 0.1102 ns |  59.8574 ns |  2,517.06 |   84.92 |   43 | 0.0038 |      64 B |          NA |
 DomainPrimitives_SpanFormat               | .NET 9.0  | .NET 9.0  |   5.1283 ns | 0.0051 ns | 0.0048 ns |   5.1269 ns |    215.57 |    7.26 |   23 |      - |         - |          NA |
 DomainPrimitives_Utf8SpanFormat           | .NET 9.0  | .NET 9.0  |   4.0692 ns | 0.0021 ns | 0.0017 ns |   4.0684 ns |    171.05 |    5.76 |   20 |      - |         - |          NA |
 StringPrimitive_Email_Create              | .NET 9.0  | .NET 9.0  | 184.3376 ns | 0.1175 ns | 0.1099 ns | 184.3524 ns |  7,748.74 |  261.05 |   52 |      - |         - |          NA |
 StringPrimitive_Email_TryParse            | .NET 9.0  | .NET 9.0  | 175.4955 ns | 0.1397 ns | 0.1090 ns | 175.4975 ns |  7,377.05 |  248.68 |   51 |      - |         - |          NA |
 NumericPrimitive_Money_Create             | .NET 9.0  | .NET 9.0  |   4.5063 ns | 0.0022 ns | 0.0020 ns |   4.5055 ns |    189.42 |    6.38 |   21 |      - |         - |          NA |
 NumericPrimitive_Money_Add                | .NET 9.0  | .NET 9.0  |   9.9881 ns | 0.0030 ns | 0.0026 ns |   9.9879 ns |    419.85 |   14.15 |   28 |      - |         - |          NA |
 ValueObject_Create                        | .NET 9.0  | .NET 9.0  |   0.0007 ns | 0.0006 ns | 0.0005 ns |   0.0005 ns |      0.03 |    0.02 |    1 |      - |         - |          NA |
 SmartEnum_FromValue                       | .NET 9.0  | .NET 9.0  |  16.5187 ns | 0.1582 ns | 0.1321 ns |  16.4841 ns |    694.37 |   24.00 |   32 | 0.0019 |      32 B |          NA |
 RawGuid_JsonSerialize                     | .NET 9.0  | .NET 9.0  | 114.5807 ns | 1.1941 ns | 1.1169 ns | 114.5260 ns |  4,816.46 |  168.49 |   48 | 0.0062 |     104 B |          NA |
 RawGuid_JsonDeserialize                   | .NET 9.0  | .NET 9.0  | 126.8748 ns | 0.1541 ns | 0.1366 ns | 126.8778 ns |  5,333.26 |  179.76 |   49 |      - |         - |          NA |
 DomainPrimitives_JsonSerialize            | .NET 9.0  | .NET 9.0  | 164.2092 ns | 0.8547 ns | 0.7995 ns | 164.1581 ns |  6,902.63 |  234.78 |   51 | 0.0062 |     104 B |          NA |
 DomainPrimitives_JsonDeserialize          | .NET 9.0  | .NET 9.0  | 124.8818 ns | 0.1917 ns | 0.1793 ns | 124.9020 ns |  5,249.48 |  176.97 |   49 |      - |         - |          NA |
 RawGuid_Create                            | .NET 9.0  | .NET 9.0  |   0.0551 ns | 0.0084 ns | 0.0070 ns |   0.0585 ns |      2.32 |    0.29 |    4 |      - |         - |          NA |
 DomainPrimitives_Create                   | .NET 9.0  | .NET 9.0  |   0.9905 ns | 0.0023 ns | 0.0019 ns |   0.9895 ns |     41.63 |    1.41 |   11 |      - |         - |          NA |
 Vogen_Create                              | .NET 9.0  | .NET 9.0  |   0.3745 ns | 0.0007 ns | 0.0006 ns |   0.3745 ns |     15.74 |    0.53 |    9 |      - |         - |          NA |
 StronglyTypedId_Create                    | .NET 9.0  | .NET 9.0  |   0.0263 ns | 0.0026 ns | 0.0024 ns |   0.0259 ns |      1.10 |    0.11 |    3 |      - |         - |          NA |
 ValueOf_Create                            | .NET 9.0  | .NET 9.0  |   9.1427 ns | 0.2559 ns | 0.3143 ns |   9.0872 ns |    384.32 |   18.29 |   28 | 0.0019 |      32 B |          NA |
 Meziantou_Create                          | .NET 9.0  | .NET 9.0  |   0.0254 ns | 0.0015 ns | 0.0013 ns |   0.0253 ns |      1.07 |    0.07 |    3 |      - |         - |          NA |
 TinyTypes_Create                          | .NET 9.0  | .NET 9.0  |   0.0276 ns | 0.0025 ns | 0.0022 ns |   0.0278 ns |      1.16 |    0.10 |    3 |      - |         - |          NA |
 RawGuid_Parse                             | .NET 9.0  | .NET 9.0  |  28.7695 ns | 0.0367 ns | 0.0344 ns |  28.7650 ns |  1,209.34 |   40.76 |   36 |      - |         - |          NA |
 DomainPrimitives_Parse                    | .NET 9.0  | .NET 9.0  |  29.0021 ns | 0.0321 ns | 0.0285 ns |  29.0005 ns |  1,219.12 |   41.09 |   36 |      - |         - |          NA |
 Vogen_Parse                               | .NET 9.0  | .NET 9.0  |  28.8470 ns | 0.0581 ns | 0.0515 ns |  28.8542 ns |  1,212.60 |   40.91 |   36 |      - |         - |          NA |
 StronglyTypedId_Parse                     | .NET 9.0  | .NET 9.0  |  28.3681 ns | 0.0328 ns | 0.0256 ns |  28.3748 ns |  1,192.47 |   40.20 |   36 |      - |         - |          NA |
 ValueOf_Parse                             | .NET 9.0  | .NET 9.0  |  32.6398 ns | 0.0731 ns | 0.0648 ns |  32.6436 ns |  1,372.03 |   46.30 |   38 | 0.0019 |      32 B |          NA |
 Meziantou_Parse                           | .NET 9.0  | .NET 9.0  |  29.4545 ns | 0.0425 ns | 0.0397 ns |  29.4654 ns |  1,238.14 |   41.74 |   36 |      - |         - |          NA |
 TinyTypes_Parse                           | .NET 9.0  | .NET 9.0  |  28.3680 ns | 0.0262 ns | 0.0204 ns |  28.3704 ns |  1,192.46 |   40.20 |   36 |      - |         - |          NA |
 DomainPrimitives_EqualityCheck            | .NET 9.0  | .NET 9.0  |   0.9070 ns | 0.0012 ns | 0.0011 ns |   0.9067 ns |     38.13 |    1.29 |   10 |      - |         - |          NA |
 RawGuid_ToString                          | .NET 9.0  | .NET 9.0  |  13.5419 ns | 0.1822 ns | 0.1704 ns |  13.4712 ns |    569.24 |   20.39 |   31 | 0.0057 |      96 B |          NA |
 DomainPrimitives_ToString                 | .NET 9.0  | .NET 9.0  |  14.4252 ns | 0.2366 ns | 0.2213 ns |  14.4992 ns |    606.37 |   22.33 |   31 | 0.0057 |      96 B |          NA |
 Vogen_ToString                            | .NET 9.0  | .NET 9.0  |  14.9631 ns | 0.1333 ns | 0.1181 ns |  14.9615 ns |    628.98 |   21.73 |   31 | 0.0057 |      96 B |          NA |
 StronglyTypedId_ToString                  | .NET 9.0  | .NET 9.0  |  14.3684 ns | 0.1990 ns | 0.1764 ns |  14.3539 ns |    603.98 |   21.57 |   31 | 0.0057 |      96 B |          NA |
 ValueOf_ToString                          | .NET 9.0  | .NET 9.0  |  22.1279 ns | 0.1722 ns | 0.1526 ns |  22.1071 ns |    930.16 |   31.94 |   34 | 0.0076 |     128 B |          NA |
 Meziantou_ToString                        | .NET 9.0  | .NET 9.0  |  35.0470 ns | 0.3164 ns | 0.2960 ns |  35.1156 ns |  1,473.22 |   51.07 |   39 | 0.0148 |     248 B |          NA |
 TinyTypes_ToString                        | .NET 9.0  | .NET 9.0  |  15.6304 ns | 0.1258 ns | 0.1116 ns |  15.6111 ns |    657.03 |   22.59 |   31 | 0.0019 |      32 B |          NA |
 DomainPrimitives_TryParse                 | .NET 9.0  | .NET 9.0  |  28.0170 ns | 0.0430 ns | 0.0402 ns |  28.0227 ns |  1,177.71 |   39.70 |   36 |      - |         - |          NA |
 DomainPrimitives_SpanParse                | .NET 9.0  | .NET 9.0  |  27.7505 ns | 0.0292 ns | 0.0273 ns |  27.7394 ns |  1,166.51 |   39.31 |   36 |      - |         - |          NA |
 DomainPrimitives_Utf8SpanParse            | .NET 9.0  | .NET 9.0  |  60.7987 ns | 0.2141 ns | 0.1898 ns |  60.8389 ns |  2,555.71 |   86.45 |   43 | 0.0038 |      64 B |          NA |
 DomainPrimitives_SpanFormat               | .NET 9.0  | .NET 9.0  |   5.1265 ns | 0.0055 ns | 0.0046 ns |   5.1252 ns |    215.49 |    7.26 |   23 |      - |         - |          NA |
 DomainPrimitives_Utf8SpanFormat           | .NET 9.0  | .NET 9.0  |   4.0705 ns | 0.0036 ns | 0.0028 ns |   4.0698 ns |    171.11 |    5.77 |   20 |      - |         - |          NA |
 StringPrimitive_Email_Create              | .NET 9.0  | .NET 9.0  | 176.2394 ns | 0.2081 ns | 0.1845 ns | 176.2485 ns |  7,408.32 |  249.70 |   51 |      - |         - |          NA |
 StringPrimitive_Email_TryParse            | .NET 9.0  | .NET 9.0  | 181.6981 ns | 0.2896 ns | 0.2418 ns | 181.6707 ns |  7,637.78 |  257.55 |   52 |      - |         - |          NA |
 NumericPrimitive_Money_Create             | .NET 9.0  | .NET 9.0  |   4.5097 ns | 0.0039 ns | 0.0036 ns |   4.5086 ns |    189.57 |    6.39 |   21 |      - |         - |          NA |
 NumericPrimitive_Money_Add                | .NET 9.0  | .NET 9.0  |  10.2924 ns | 0.0058 ns | 0.0045 ns |  10.2916 ns |    432.65 |   14.58 |   28 |      - |         - |          NA |
 ValueObject_Create                        | .NET 9.0  | .NET 9.0  |   0.0006 ns | 0.0005 ns | 0.0004 ns |   0.0006 ns |      0.03 |    0.02 |    1 |      - |         - |          NA |
 SmartEnum_FromValue                       | .NET 9.0  | .NET 9.0  |  17.4989 ns | 0.3800 ns | 0.4224 ns |  17.4871 ns |    735.58 |   30.22 |   32 | 0.0019 |      32 B |          NA |
 RawGuid_JsonSerialize                     | .NET 9.0  | .NET 9.0  | 110.2044 ns | 0.1382 ns | 0.1293 ns | 110.1857 ns |  4,632.50 |  156.13 |   47 | 0.0062 |     104 B |          NA |
 RawGuid_JsonDeserialize                   | .NET 9.0  | .NET 9.0  | 127.6597 ns | 0.1770 ns | 0.1478 ns | 127.6283 ns |  5,366.25 |  180.92 |   49 |      - |         - |          NA |
 DomainPrimitives_JsonSerialize            | .NET 9.0  | .NET 9.0  | 160.9003 ns | 0.3308 ns | 0.2763 ns | 160.8415 ns |  6,763.53 |  228.18 |   51 | 0.0062 |     104 B |          NA |
 DomainPrimitives_JsonDeserialize          | .NET 9.0  | .NET 9.0  | 125.8694 ns | 0.0724 ns | 0.0605 ns | 125.8545 ns |  5,290.99 |  178.30 |   49 |      - |         - |          NA |
 Dapper_TypeHandler_SetValue               | .NET 10.0 | .NET 10.0 |   0.3524 ns | 0.0017 ns | 0.0015 ns |   0.3518 ns |     14.81 |    0.50 |    8 |      - |         - |          NA |
 Dapper_TypeHandler_Parse                  | .NET 10.0 | .NET 10.0 |   0.9434 ns | 0.0004 ns | 0.0003 ns |   0.9435 ns |     39.65 |    1.34 |   11 |      - |         - |          NA |
 EFCore_ValueConverter_ConvertToProvider   | .NET 10.0 | .NET 10.0 |   1.5222 ns | 0.0073 ns | 0.0068 ns |   1.5231 ns |     63.99 |    2.17 |   16 |      - |         - |          NA |
 EFCore_ValueConverter_ConvertFromProvider | .NET 10.0 | .NET 10.0 |   0.9455 ns | 0.0010 ns | 0.0008 ns |   0.9453 ns |     39.74 |    1.34 |   11 |      - |         - |          NA |
 Dapper_TypeHandler_SetValue               | .NET 8.0  | .NET 8.0  |   7.6394 ns | 0.0102 ns | 0.0091 ns |   7.6389 ns |    321.13 |   10.82 |   27 | 0.0019 |      32 B |          NA |
 Dapper_TypeHandler_Parse                  | .NET 8.0  | .NET 8.0  |   2.0057 ns | 0.0102 ns | 0.0090 ns |   2.0059 ns |     84.31 |    2.86 |   17 |      - |         - |          NA |
 EFCore_ValueConverter_ConvertToProvider   | .NET 8.0  | .NET 8.0  |  12.0321 ns | 0.0063 ns | 0.0053 ns |  12.0314 ns |    505.78 |   17.04 |   30 |      - |         - |          NA |
 EFCore_ValueConverter_ConvertFromProvider | .NET 8.0  | .NET 8.0  |  11.8646 ns | 0.0048 ns | 0.0037 ns |  11.8631 ns |    498.73 |   16.81 |   30 |      - |         - |          NA |
 Dapper_TypeHandler_SetValue               | .NET 9.0  | .NET 9.0  |   1.0471 ns | 0.0070 ns | 0.0065 ns |   1.0443 ns |     44.01 |    1.51 |   12 |      - |         - |          NA |
 Dapper_TypeHandler_Parse                  | .NET 9.0  | .NET 9.0  |   1.2972 ns | 0.0011 ns | 0.0008 ns |   1.2973 ns |     54.53 |    1.84 |   14 |      - |         - |          NA |
 EFCore_ValueConverter_ConvertToProvider   | .NET 9.0  | .NET 9.0  |   1.5265 ns | 0.0069 ns | 0.0061 ns |   1.5255 ns |     64.17 |    2.18 |   16 |      - |         - |          NA |
 EFCore_ValueConverter_ConvertFromProvider | .NET 9.0  | .NET 9.0  |   0.9436 ns | 0.0006 ns | 0.0005 ns |   0.9437 ns |     39.67 |    1.34 |   11 |      - |         - |          NA |
 Dapper_TypeHandler_SetValue               | .NET 9.0  | .NET 9.0  |   1.2066 ns | 0.0075 ns | 0.0066 ns |   1.2063 ns |     50.72 |    1.73 |   13 |      - |         - |          NA |
 Dapper_TypeHandler_Parse                  | .NET 9.0  | .NET 9.0  |   0.9442 ns | 0.0007 ns | 0.0006 ns |   0.9441 ns |     39.69 |    1.34 |   11 |      - |         - |          NA |
 EFCore_ValueConverter_ConvertToProvider   | .NET 9.0  | .NET 9.0  |   1.5289 ns | 0.0059 ns | 0.0052 ns |   1.5284 ns |     64.27 |    2.18 |   16 |      - |         - |          NA |
 EFCore_ValueConverter_ConvertFromProvider | .NET 9.0  | .NET 9.0  |   0.9454 ns | 0.0027 ns | 0.0025 ns |   0.9442 ns |     39.74 |    1.34 |   11 |      - |         - |          NA |
