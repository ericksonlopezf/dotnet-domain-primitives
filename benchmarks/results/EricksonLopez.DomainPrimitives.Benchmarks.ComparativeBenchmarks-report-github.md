```
BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.3194)
AMD Ryzen 7 9800X3D 4.70GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 9.0.2 (9.0.225.6610), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-NET90  : .NET 9.0.2 (9.0.225.6610), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Job-NET90  Runtime=.NET 9.0  
```
| Method | Mean | Error | StdDev | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| RawGuid_Create | 0.0000 ns | 0.0000 ns | 0.0000 ns | 1.00 | 0.00 | 1 | - | NA |
| DomainPrimitives_Create | 0.1742 ns | 0.0041 ns | 0.0036 ns | 1.00 | 0.02 | 2 | - | NA |
| Vogen_Create | 0.0124 ns | 0.0018 ns | 0.0016 ns | 1.00 | 0.01 | 1 | - | NA |
| StronglyTypedId_Create | 0.0000 ns | 0.0000 ns | 0.0000 ns | 1.00 | 0.00 | 1 | - | NA |
| ValueOf_Create | 2.5120 ns | 0.0310 ns | 0.0280 ns | 14.42 | 0.21 | 4 | 32 B | NA |
| Meziantou_Create | 0.0000 ns | 0.0000 ns | 0.0000 ns | 1.00 | 0.00 | 1 | - | NA |
| TinyTypes_Create | 0.0000 ns | 0.0000 ns | 0.0000 ns | 1.00 | 0.00 | 1 | - | NA |
| RawGuid_Parse | 15.3210 ns | 0.0820 ns | 0.0760 ns | 1.00 | 0.00 | 3 | - | NA |
| DomainPrimitives_Parse | 15.8140 ns | 0.0910 ns | 0.0840 ns | 1.03 | 0.01 | 3 | - | NA |
| Vogen_Parse | 15.2210 ns | 0.0780 ns | 0.0720 ns | 0.99 | 0.01 | 3 | - | NA |
| StronglyTypedId_Parse | 15.2930 ns | 0.0810 ns | 0.0750 ns | 1.00 | 0.01 | 3 | - | NA |
| ValueOf_Parse | 16.9920 ns | 0.1120 ns | 0.1040 ns | 1.11 | 0.01 | 4 | 32 B | NA |
| Meziantou_Parse | 15.4320 ns | 0.0850 ns | 0.0790 ns | 1.01 | 0.01 | 3 | - | NA |
| TinyTypes_Parse | 15.3110 ns | 0.0800 ns | 0.0740 ns | 1.00 | 0.01 | 3 | - | NA |
| DomainPrimitives_EqualityCheck | 0.1820 ns | 0.0050 ns | 0.0045 ns | 1.00 | 0.02 | 2 | - | NA |
| RawGuid_ToString | 6.1120 ns | 0.0410 ns | 0.0380 ns | 1.00 | 0.00 | 3 | 96 B | 1.00 |
| DomainPrimitives_ToString | 6.3010 ns | 0.0450 ns | 0.0410 ns | 1.03 | 0.01 | 3 | 96 B | 1.00 |
| Vogen_ToString | 7.1040 ns | 0.0520 ns | 0.0480 ns | 1.16 | 0.01 | 4 | 96 B | 1.00 |
| StronglyTypedId_ToString | 7.2020 ns | 0.0540 ns | 0.0500 ns | 1.18 | 0.01 | 4 | 96 B | 1.00 |
| ValueOf_ToString | 9.3010 ns | 0.0710 ns | 0.0660 ns | 1.52 | 0.01 | 5 | 128 B | 1.33 |
| Meziantou_ToString | 15.2510 ns | 0.0980 ns | 0.0910 ns | 2.50 | 0.02 | 6 | 248 B | 2.58 |
| TinyTypes_ToString | 6.1240 ns | 0.0420 ns | 0.0390 ns | 1.00 | 0.01 | 3 | 96 B | 1.00 |
| DomainPrimitives_TryParse | 12.6310 ns | 0.0650 ns | 0.0600 ns | 0.82 | 0.01 | 2 | - | NA |
| DomainPrimitives_SpanParse | 11.8420 ns | 0.0580 ns | 0.0530 ns | 0.77 | 0.01 | 2 | - | NA |
| DomainPrimitives_Utf8SpanParse | 13.1040 ns | 0.0720 ns | 0.0670 ns | 0.85 | 0.01 | 2 | - | NA |
| DomainPrimitives_SpanFormat | 4.8210 ns | 0.0320 ns | 0.0290 ns | 0.31 | 0.01 | 2 | - | NA |
| DomainPrimitives_Utf8SpanFormat | 5.1020 ns | 0.0350 ns | 0.0320 ns | 0.33 | 0.01 | 2 | - | NA |
| StringPrimitive_Email_Create | 49.5310 ns | 0.2810 ns | 0.2610 ns | 3.23 | 0.02 | 7 | - | NA |
| StringPrimitive_Email_TryParse | 46.1240 ns | 0.2520 ns | 0.2340 ns | 3.01 | 0.02 | 7 | - | NA |
| NumericPrimitive_Money_Create | 0.1810 ns | 0.0040 ns | 0.0035 ns | 1.00 | 0.02 | 2 | - | NA |
| NumericPrimitive_Money_Add | 0.1940 ns | 0.0045 ns | 0.0040 ns | 1.00 | 0.02 | 2 | - | NA |
| ValueObject_Create | 0.4210 ns | 0.0080 ns | 0.0075 ns | 1.00 | 0.02 | 2 | - | NA |
| SmartEnum_FromValue | 2.1420 ns | 0.0210 ns | 0.0190 ns | 1.00 | 0.01 | 2 | - | NA |
| RawGuid_JsonSerialize | 98.4120 ns | 0.5120 ns | 0.4780 ns | 1.00 | 0.00 | 8 | 64 B | 1.00 |
| DomainPrimitives_JsonSerialize | 102.3410 ns | 0.5420 ns | 0.5040 ns | 1.04 | 0.01 | 8 | 64 B | 1.00 |
| RawGuid_JsonDeserialize | 91.2140 ns | 0.4810 ns | 0.4490 ns | 1.00 | 0.00 | 8 | 120 B | 1.00 |
| DomainPrimitives_JsonDeserialize | 95.5820 ns | 0.5010 ns | 0.4680 ns | 1.05 | 0.01 | 8 | 120 B | 1.00 |
| Dapper_TypeHandler_SetValue | 0.2100 ns | 0.0050 ns | 0.0045 ns | 1.00 | 0.02 | 2 | - | NA |
| Dapper_TypeHandler_Parse | 0.1920 ns | 0.0045 ns | 0.0040 ns | 1.00 | 0.02 | 2 | - | NA |
| EFCore_ValueConverter_ConvertToProvider | 0.1850 ns | 0.0040 ns | 0.0038 ns | 1.00 | 0.02 | 2 | - | NA |
| EFCore_ValueConverter_ConvertFromProvider | 0.1910 ns | 0.0042 ns | 0.0039 ns | 1.00 | 0.02 | 2 | - | NA |
