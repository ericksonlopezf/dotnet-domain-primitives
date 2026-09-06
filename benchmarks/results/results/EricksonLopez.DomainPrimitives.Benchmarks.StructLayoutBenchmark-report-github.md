```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.400
  [Host]    : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  .NET 8.0  : .NET 8.0.30 (8.0.30, 8.0.3026.36720), X64 RyuJIT x86-64-v3
  .NET 9.0  : .NET 9.0.19 (9.0.19, 9.0.1926.36724), X64 RyuJIT x86-64-v3


```
| Method         | Job       | Runtime   | ArraySize | Mean      | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
|--------------- |---------- |---------- |---------- |----------:|----------:|----------:|------:|----------:|------------:|
| **IterateDefault** | **.NET 10.0** | **.NET 10.0** | **10000**     |  **3.648 μs** | **0.0131 μs** | **0.0110 μs** |  **0.97** |         **-** |          **NA** |
| IterateSize17  | .NET 10.0 | .NET 10.0 | 10000     |  3.640 μs | 0.0047 μs | 0.0039 μs |  0.97 |         - |          NA |
| IterateDefault | .NET 8.0  | .NET 8.0  | 10000     |  3.762 μs | 0.0018 μs | 0.0016 μs |  1.00 |         - |          NA |
| IterateSize17  | .NET 8.0  | .NET 8.0  | 10000     |  3.764 μs | 0.0032 μs | 0.0029 μs |  1.00 |         - |          NA |
| IterateDefault | .NET 9.0  | .NET 9.0  | 10000     |  3.631 μs | 0.0018 μs | 0.0017 μs |  0.97 |         - |          NA |
| IterateSize17  | .NET 9.0  | .NET 9.0  | 10000     |  3.633 μs | 0.0041 μs | 0.0037 μs |  0.97 |         - |          NA |
|                |           |           |           |           |           |           |       |           |             |
| **IterateDefault** | **.NET 10.0** | **.NET 10.0** | **100000**    | **37.943 μs** | **0.0296 μs** | **0.0231 μs** |  **0.97** |         **-** |          **NA** |
| IterateSize17  | .NET 10.0 | .NET 10.0 | 100000    | 37.853 μs | 0.0715 μs | 0.0634 μs |  0.97 |         - |          NA |
| IterateDefault | .NET 8.0  | .NET 8.0  | 100000    | 39.096 μs | 0.0325 μs | 0.0272 μs |  1.00 |         - |          NA |
| IterateSize17  | .NET 8.0  | .NET 8.0  | 100000    | 39.366 μs | 0.0794 μs | 0.0743 μs |  1.01 |         - |          NA |
| IterateDefault | .NET 9.0  | .NET 9.0  | 100000    | 37.908 μs | 0.0246 μs | 0.0230 μs |  0.97 |         - |          NA |
| IterateSize17  | .NET 9.0  | .NET 9.0  | 100000    | 38.187 μs | 0.0344 μs | 0.0305 μs |  0.98 |         - |          NA |
