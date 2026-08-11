# Benchmark Results

This directory contains BenchmarkDotNet JSON and Markdown exports.

Run from the benchmarks project directory:

    dotnet run -c Release -- --filter "*" --exporters json md

Results will appear in BenchmarkDotNet.Artifacts/results/; copy them here.
See docs/benchmark-results.md for the full results table and status.
