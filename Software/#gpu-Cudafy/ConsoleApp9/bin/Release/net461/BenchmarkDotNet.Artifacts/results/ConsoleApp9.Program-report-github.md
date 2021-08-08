``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.18363.1500 (1909/November2018Update/19H2)
Intel Core i5-9300H CPU 2.40GHz, 1 CPU, 8 logical and 4 physical cores
  [Host]        : .NET Framework 4.8 (4.8.4341.0), X64 RyuJIT
  .NET 4.6.2    : .NET Framework 4.8 (4.8.4341.0), X64 RyuJIT
  .NET Core 3.1 : .NET Core 3.1.14 (CoreCLR 4.700.21.16201, CoreFX 4.700.21.16208), X64 RyuJIT


```
|       Method |           Job |       Runtime |     Mean |     Error |    StdDev | Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------- |-------------- |-------------- |---------:|----------:|----------:|------:|------:|------:|----------:|
| StructToTest |    .NET 4.6.2 |    .NET 4.6.2 | 2.681 ns | 0.0174 ns | 0.0163 ns |     - |     - |     - |         - |
| StructToTest | .NET Core 3.1 | .NET Core 3.1 | 2.598 ns | 0.0225 ns | 0.0188 ns |     - |     - |     - |         - |
