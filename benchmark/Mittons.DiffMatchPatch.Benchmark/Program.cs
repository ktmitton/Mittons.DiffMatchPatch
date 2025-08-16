// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;
using Mittons.DiffMatchPatch.Benchmark;

Console.WriteLine("Hello, World!");
// BenchmarkRunner.Run<StringBuildingBenchmark>();
BenchmarkRunner.Run<DeltaBenchmarks>();
