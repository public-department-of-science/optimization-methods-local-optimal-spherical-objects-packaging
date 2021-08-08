``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.18363.1500 (1909/November2018Update/19H2)
Intel Core i5-9300H CPU 2.40GHz, 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=6.0.100-preview.2.21155.3
  [Host]        : .NET Core 3.1.14 (CoreCLR 4.700.21.16201, CoreFX 4.700.21.16208), X64 RyuJIT
  .NET Core 3.1 : .NET Core 3.1.14 (CoreCLR 4.700.21.16201, CoreFX 4.700.21.16208), X64 RyuJIT
  .NET Core 5.0 : .NET Core 5.0.5 (CoreCLR 5.0.521.16609, CoreFX 5.0.521.16609), X64 RyuJIT


```
|                       Method |           Job |       Runtime |     Mean |     Error |    StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|----------------------------- |-------------- |-------------- |---------:|----------:|----------:|-------:|------:|------:|----------:|
|        StringSortToBenchmark |    .NET 4.6.1 |    .NET 4.6.1 |       NA |        NA |        NA |      - |     - |     - |         - |
| StringBuilderSortToBenchmark |    .NET 4.6.1 |    .NET 4.6.1 |       NA |        NA |        NA |      - |     - |     - |         - |
|        StringSortToBenchmark |    .NET 4.6.2 |    .NET 4.6.2 |       NA |        NA |        NA |      - |     - |     - |         - |
| StringBuilderSortToBenchmark |    .NET 4.6.2 |    .NET 4.6.2 |       NA |        NA |        NA |      - |     - |     - |         - |
|        StringSortToBenchmark |      .NET 4.7 |      .NET 4.7 |       NA |        NA |        NA |      - |     - |     - |         - |
| StringBuilderSortToBenchmark |      .NET 4.7 |      .NET 4.7 |       NA |        NA |        NA |      - |     - |     - |         - |
|        StringSortToBenchmark |    .NET 4.7.1 |    .NET 4.7.1 |       NA |        NA |        NA |      - |     - |     - |         - |
| StringBuilderSortToBenchmark |    .NET 4.7.1 |    .NET 4.7.1 |       NA |        NA |        NA |      - |     - |     - |         - |
|        StringSortToBenchmark |    .NET 4.7.2 |    .NET 4.7.2 |       NA |        NA |        NA |      - |     - |     - |         - |
| StringBuilderSortToBenchmark |    .NET 4.7.2 |    .NET 4.7.2 |       NA |        NA |        NA |      - |     - |     - |         - |
|        StringSortToBenchmark |      .NET 4.8 |      .NET 4.8 |       NA |        NA |        NA |      - |     - |     - |         - |
| StringBuilderSortToBenchmark |      .NET 4.8 |      .NET 4.8 |       NA |        NA |        NA |      - |     - |     - |         - |
|        StringSortToBenchmark | .NET Core 2.0 | .NET Core 2.0 |       NA |        NA |        NA |      - |     - |     - |         - |
| StringBuilderSortToBenchmark | .NET Core 2.0 | .NET Core 2.0 |       NA |        NA |        NA |      - |     - |     - |         - |
|        StringSortToBenchmark | .NET Core 2.1 | .NET Core 2.1 |       NA |        NA |        NA |      - |     - |     - |         - |
| StringBuilderSortToBenchmark | .NET Core 2.1 | .NET Core 2.1 |       NA |        NA |        NA |      - |     - |     - |         - |
|        StringSortToBenchmark | .NET Core 2.2 | .NET Core 2.2 |       NA |        NA |        NA |      - |     - |     - |         - |
| StringBuilderSortToBenchmark | .NET Core 2.2 | .NET Core 2.2 |       NA |        NA |        NA |      - |     - |     - |         - |
|        StringSortToBenchmark | .NET Core 3.0 | .NET Core 3.0 |       NA |        NA |        NA |      - |     - |     - |         - |
| StringBuilderSortToBenchmark | .NET Core 3.0 | .NET Core 3.0 |       NA |        NA |        NA |      - |     - |     - |         - |
|        StringSortToBenchmark | .NET Core 3.1 | .NET Core 3.1 | 4.539 μs | 0.0288 μs | 0.0241 μs | 0.5341 |     - |     - |    2264 B |
| StringBuilderSortToBenchmark | .NET Core 3.1 | .NET Core 3.1 | 5.127 μs | 0.0918 μs | 0.0814 μs | 0.8392 |     - |     - |    3512 B |
|        StringSortToBenchmark | .NET Core 5.0 | .NET Core 5.0 | 3.736 μs | 0.0722 μs | 0.0640 μs | 0.5379 |     - |     - |    2264 B |
| StringBuilderSortToBenchmark | .NET Core 5.0 | .NET Core 5.0 | 4.359 μs | 0.0248 μs | 0.0232 μs | 0.8392 |     - |     - |    3512 B |

Benchmarks with issues:
  Program.StringSortToBenchmark: .NET 4.6.1(Runtime=.NET 4.6.1)
  Program.StringBuilderSortToBenchmark: .NET 4.6.1(Runtime=.NET 4.6.1)
  Program.StringSortToBenchmark: .NET 4.6.2(Runtime=.NET 4.6.2)
  Program.StringBuilderSortToBenchmark: .NET 4.6.2(Runtime=.NET 4.6.2)
  Program.StringSortToBenchmark: .NET 4.7(Runtime=.NET 4.7)
  Program.StringBuilderSortToBenchmark: .NET 4.7(Runtime=.NET 4.7)
  Program.StringSortToBenchmark: .NET 4.7.1(Runtime=.NET 4.7.1)
  Program.StringBuilderSortToBenchmark: .NET 4.7.1(Runtime=.NET 4.7.1)
  Program.StringSortToBenchmark: .NET 4.7.2(Runtime=.NET 4.7.2)
  Program.StringBuilderSortToBenchmark: .NET 4.7.2(Runtime=.NET 4.7.2)
  Program.StringSortToBenchmark: .NET 4.8(Runtime=.NET 4.8)
  Program.StringBuilderSortToBenchmark: .NET 4.8(Runtime=.NET 4.8)
  Program.StringSortToBenchmark: .NET Core 2.0(Runtime=.NET Core 2.0)
  Program.StringBuilderSortToBenchmark: .NET Core 2.0(Runtime=.NET Core 2.0)
  Program.StringSortToBenchmark: .NET Core 2.1(Runtime=.NET Core 2.1)
  Program.StringBuilderSortToBenchmark: .NET Core 2.1(Runtime=.NET Core 2.1)
  Program.StringSortToBenchmark: .NET Core 2.2(Runtime=.NET Core 2.2)
  Program.StringBuilderSortToBenchmark: .NET Core 2.2(Runtime=.NET Core 2.2)
  Program.StringSortToBenchmark: .NET Core 3.0(Runtime=.NET Core 3.0)
  Program.StringBuilderSortToBenchmark: .NET Core 3.0(Runtime=.NET Core 3.0)
