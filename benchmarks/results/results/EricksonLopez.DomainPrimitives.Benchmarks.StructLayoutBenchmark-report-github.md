```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.400
  [Host]    : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  .NET 8.0  : .NET 8.0.30 (8.0.30, 8.0.3026.36720), X64 RyuJIT x86-64-v3
  .NET 9.0  : .NET 9.0.19 (9.0.19, 9.0.1926.36724), X64 RyuJIT x86-64-v3


```
| Method         | Job       | Runtime   | ArraySize | Mean      | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
|--------------- |---------- |---------- |---------- |----------:|----------:|----------:|------:|----------:|------------:|
| **IterateDefault** | **.NET 10.0** | **.NET 10.0** | **10000**     |  **3.448 μs** | **0.0039 μs** | **0.0035 μs** |  **1.04** |         **-** |          **NA** |
| IterateSize17  | .NET 10.0 | .NET 10.0 | 10000     |  3.311 μs | 0.0235 μs | 0.0220 μs |  1.00 |         - |          NA |
| IterateDefault | .NET 8.0  | .NET 8.0  | 10000     |  3.300 μs | 0.0107 μs | 0.0095 μs |  1.00 |         - |          NA |
| IterateSize17  | .NET 8.0  | .NET 8.0  | 10000     |  3.293 μs | 0.0035 μs | 0.0029 μs |  1.00 |         - |          NA |
| IterateDefault | .NET 9.0  | .NET 9.0  | 10000     |  3.183 μs | 0.0037 μs | 0.0031 μs |  0.96 |         - |          NA |
| IterateSize17  | .NET 9.0  | .NET 9.0  | 10000     |  3.187 μs | 0.0041 μs | 0.0038 μs |  0.97 |         - |          NA |
|                |           |           |           |           |           |           |       |           |             |
| **IterateDefault** | **.NET 10.0** | **.NET 10.0** | **100000**    | **35.041 μs** | **0.0429 μs** | **0.0358 μs** |  **0.97** |         **-** |          **NA** |
| IterateSize17  | .NET 10.0 | .NET 10.0 | 100000    | 35.827 μs | 0.0461 μs | 0.0432 μs |  0.99 |         - |          NA |
| IterateDefault | .NET 8.0  | .NET 8.0  | 100000    | 36.199 μs | 0.0825 μs | 0.0689 μs |  1.00 |         - |          NA |
| IterateSize17  | .NET 8.0  | .NET 8.0  | 100000    | 37.080 μs | 0.0256 μs | 0.0227 μs |  1.02 |         - |          NA |
| IterateDefault | .NET 9.0  | .NET 9.0  | 100000    | 35.000 μs | 0.0463 μs | 0.0410 μs |  0.97 |         - |          NA |
| IterateSize17  | .NET 9.0  | .NET 9.0  | 100000    | 36.218 μs | 0.0610 μs | 0.0541 μs |  1.00 |         - |          NA |
