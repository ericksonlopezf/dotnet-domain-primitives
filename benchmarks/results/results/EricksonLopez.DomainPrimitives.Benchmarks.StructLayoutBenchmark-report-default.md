
BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.400
  [Host]    : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  .NET 8.0  : .NET 8.0.30 (8.0.30, 8.0.3026.36720), X64 RyuJIT x86-64-v3
  .NET 9.0  : .NET 9.0.19 (9.0.19, 9.0.1926.36724), X64 RyuJIT x86-64-v3


 Method         | Job       | Runtime   | ArraySize | Mean      | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
--------------- |---------- |---------- |---------- |----------:|----------:|----------:|------:|----------:|------------:|
 **IterateDefault** | **.NET 10.0** | **.NET 10.0** | **10000**     |  **3.641 μs** | **0.0049 μs** | **0.0041 μs** |  **0.97** |         **-** |          **NA** |
 IterateSize17  | .NET 10.0 | .NET 10.0 | 10000     |  3.642 μs | 0.0049 μs | 0.0043 μs |  0.97 |         - |          NA |
 IterateDefault | .NET 8.0  | .NET 8.0  | 10000     |  3.767 μs | 0.0039 μs | 0.0035 μs |  1.00 |         - |          NA |
 IterateSize17  | .NET 8.0  | .NET 8.0  | 10000     |  3.763 μs | 0.0012 μs | 0.0010 μs |  1.00 |         - |          NA |
 IterateDefault | .NET 9.0  | .NET 9.0  | 10000     |  3.629 μs | 0.0013 μs | 0.0010 μs |  0.96 |         - |          NA |
 IterateSize17  | .NET 9.0  | .NET 9.0  | 10000     |  3.634 μs | 0.0025 μs | 0.0021 μs |  0.96 |         - |          NA |
                |           |           |           |           |           |           |       |           |             |
 **IterateDefault** | **.NET 10.0** | **.NET 10.0** | **100000**    | **38.002 μs** | **0.0203 μs** | **0.0170 μs** |  **0.97** |         **-** |          **NA** |
 IterateSize17  | .NET 10.0 | .NET 10.0 | 100000    | 37.924 μs | 0.1005 μs | 0.0940 μs |  0.97 |         - |          NA |
 IterateDefault | .NET 8.0  | .NET 8.0  | 100000    | 39.148 μs | 0.0135 μs | 0.0119 μs |  1.00 |         - |          NA |
 IterateSize17  | .NET 8.0  | .NET 8.0  | 100000    | 39.305 μs | 0.0186 μs | 0.0165 μs |  1.00 |         - |          NA |
 IterateDefault | .NET 9.0  | .NET 9.0  | 100000    | 37.954 μs | 0.0248 μs | 0.0220 μs |  0.97 |         - |          NA |
 IterateSize17  | .NET 9.0  | .NET 9.0  | 100000    | 38.279 μs | 0.0987 μs | 0.0923 μs |  0.98 |         - |          NA |
