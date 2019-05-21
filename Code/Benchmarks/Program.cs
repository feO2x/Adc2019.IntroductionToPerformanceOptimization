﻿using System;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace Benchmarks
{
    public static class Program
    {
        public static void Main(string[] args) =>
            BenchmarkSwitcher
                .FromAssembly(typeof(Program).Assembly)
                .Run(args, CreateDefaultConfig());

        private static IConfig CreateDefaultConfig() =>
            DefaultConfig
                .Instance
                .With(Job.Core, Job.Clr)
                .With(MemoryDiagnoser.Default)
                .With(DisassemblyDiagnoser.Create(DisassemblyDiagnoserConfig.Asm));
    }
}
