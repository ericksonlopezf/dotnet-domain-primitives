
BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.401
  [Host]    : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v3
  .NET 8.0  : .NET 8.0.31 (8.0.31, 8.0.3126.42015), X64 RyuJIT x86-64-v3
  .NET 9.0  : .NET 9.0.20 (9.0.20, 9.0.2026.41315), X64 RyuJIT x86-64-v3


 Method         | Job       | Runtime   | ArraySize | Mean      | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
--------------- |---------- |---------- |---------- |----------:|----------:|----------:|------:|----------:|------------:|
 **IterateDefault** | **.NET 10.0** | **.NET 10.0** | **10000**     |  **3.643 μs** | **0.0067 μs** | **0.0059 μs** |  **0.97** |         **-** |          **NA** |
 IterateSize17  | .NET 10.0 | .NET 10.0 | 10000     |  3.640 μs | 0.0055 μs | 0.0048 μs |  0.97 |         - |          NA |
 IterateDefault | .NET 8.0  | .NET 8.0  | 10000     |  3.767 μs | 0.0062 μs | 0.0055 μs |  1.00 |         - |          NA |
 IterateSize17  | .NET 8.0  | .NET 8.0  | 10000     |  3.761 μs | 0.0020 μs | 0.0017 μs |  1.00 |         - |          NA |
 IterateDefault | .NET 9.0  | .NET 9.0  | 10000     |  3.631 μs | 0.0020 μs | 0.0018 μs |  0.96 |         - |          NA |
 IterateSize17  | .NET 9.0  | .NET 9.0  | 10000     |  3.633 μs | 0.0014 μs | 0.0013 μs |  0.96 |         - |          NA |
                |           |           |           |           |           |           |       |           |             |
 **IterateDefault** | **.NET 10.0** | **.NET 10.0** | **100000**    | **37.905 μs** | **0.0347 μs** | **0.0307 μs** |  **0.97** |         **-** |          **NA** |
 IterateSize17  | .NET 10.0 | .NET 10.0 | 100000    | 37.821 μs | 0.0777 μs | 0.0689 μs |  0.97 |         - |          NA |
 IterateDefault | .NET 8.0  | .NET 8.0  | 100000    | 39.173 μs | 0.0857 μs | 0.0802 μs |  1.00 |         - |          NA |
 IterateSize17  | .NET 8.0  | .NET 8.0  | 100000    | 39.240 μs | 0.0211 μs | 0.0187 μs |  1.00 |         - |          NA |
 IterateDefault | .NET 9.0  | .NET 9.0  | 100000    | 37.898 μs | 0.0319 μs | 0.0266 μs |  0.97 |         - |          NA |
 IterateSize17  | .NET 9.0  | .NET 9.0  | 100000    | 38.123 μs | 0.0194 μs | 0.0151 μs |  0.97 |         - |          NA |
