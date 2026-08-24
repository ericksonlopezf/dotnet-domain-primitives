```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
INTEL XEON PLATINUM 8573C 2.30GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.400
  [Host]    : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v4
  .NET 8.0  : .NET 8.0.30 (8.0.30, 8.0.3026.36720), X64 RyuJIT x86-64-v4
  .NET 9.0  : .NET 9.0.19 (9.0.19, 9.0.1926.36724), X64 RyuJIT x86-64-v4


```
| Method         | Job       | Runtime   | ArraySize | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
|--------------- |---------- |---------- |---------- |----------:|----------:|----------:|------:|--------:|----------:|------------:|
| **IterateDefault** | **.NET 10.0** | **.NET 10.0** | **10000**     |  **3.112 μs** | **0.0471 μs** | **0.0441 μs** |  **1.01** |    **0.02** |         **-** |          **NA** |
| IterateSize17  | .NET 10.0 | .NET 10.0 | 10000     |  3.127 μs | 0.0624 μs | 0.0613 μs |  1.01 |    0.03 |         - |          NA |
| IterateDefault | .NET 8.0  | .NET 8.0  | 10000     |  3.091 μs | 0.0547 μs | 0.0630 μs |  1.00 |    0.03 |         - |          NA |
| IterateSize17  | .NET 8.0  | .NET 8.0  | 10000     |  3.087 μs | 0.0549 μs | 0.0514 μs |  1.00 |    0.03 |         - |          NA |
| IterateDefault | .NET 9.0  | .NET 9.0  | 10000     |  3.132 μs | 0.0618 μs | 0.0578 μs |  1.01 |    0.03 |         - |          NA |
| IterateSize17  | .NET 9.0  | .NET 9.0  | 10000     |  3.128 μs | 0.0591 μs | 0.0580 μs |  1.01 |    0.03 |         - |          NA |
|                |           |           |           |           |           |           |       |         |           |             |
| **IterateDefault** | **.NET 10.0** | **.NET 10.0** | **100000**    | **55.199 μs** | **0.4087 μs** | **0.3823 μs** |  **0.86** |    **0.02** |         **-** |          **NA** |
| IterateSize17  | .NET 10.0 | .NET 10.0 | 100000    | 55.610 μs | 1.0915 μs | 1.3405 μs |  0.86 |    0.03 |         - |          NA |
| IterateDefault | .NET 8.0  | .NET 8.0  | 100000    | 64.355 μs | 1.2564 μs | 1.4469 μs |  1.00 |    0.03 |         - |          NA |
| IterateSize17  | .NET 8.0  | .NET 8.0  | 100000    | 61.206 μs | 0.8273 μs | 0.6459 μs |  0.95 |    0.02 |         - |          NA |
| IterateDefault | .NET 9.0  | .NET 9.0  | 100000    | 55.315 μs | 0.4192 μs | 0.3921 μs |  0.86 |    0.02 |         - |          NA |
| IterateSize17  | .NET 9.0  | .NET 9.0  | 100000    | 54.342 μs | 0.8590 μs | 0.7615 μs |  0.84 |    0.02 |         - |          NA |
