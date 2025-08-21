// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;
using Mittons.DiffMatchPatch.Benchmark;
using Mittons.DiffMatchPatch.Benchmark.CommonMatches;
using Mittons.DiffMatchPatch.Benchmark.LineEncoding;

Console.WriteLine("Hello, World!");
// BenchmarkRunner.Run<StringBuildingBenchmark>();
// BenchmarkRunner.Run<DeltaBenchmarks>();
// BenchmarkRunner.Run<BuildArrayBenchmarks>();
// BenchmarkRunner.Run<CommonBenchmarks>();
BenchmarkRunner.Run<CommonPrefixBenchmarks>();
BenchmarkRunner.Run<CommonSuffixBenchmarks>();
BenchmarkRunner.Run<CommonHalfMiddleBenchmarks>();
BenchmarkRunner.Run<CommonOverlapBenchmarks>();
BenchmarkRunner.Run<EncodingBenchmarks>();
