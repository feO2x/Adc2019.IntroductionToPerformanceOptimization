using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using BenchmarkDotNet.Attributes;

namespace Benchmarks
{
    public class MustNotBeEmptyGuidBenchmark
    {
        public Guid GuidValue = Guid.NewGuid();

        [Benchmark(Baseline = true)]
        public Guid OldVersion() => GuidValue.OldMustNotBeEmpty(nameof(GuidValue));

        [Benchmark]
        public Guid Imperative()
        {
            if (GuidValue == Guid.Empty) throw new EmptyGuidException(nameof(GuidValue));
            return GuidValue;
        }

        [Benchmark]
        public Guid LightGuardClausesWithParameterName() => GuidValue.MustNotBeEmpty(nameof(GuidValue));

        [Benchmark]
        public Guid LightGuardClausesWithCustomException() => GuidValue.MustNotBeEmpty(() => new Exception());
    }

    public static class MustNotBeEmptyGuidExtensions
    {
        public static Guid OldMustNotBeEmpty(this Guid parameter, string parameterName = null, string message = null, Func<Exception> exception = null)
        {
            if (parameter != Guid.Empty)
                return parameter;
            throw exception?.Invoke() ?? new EmptyGuidException(parameterName, message ?? $"{parameterName ?? "The value"} must be a valid GUID, but it actually is an empty one.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Guid MustNotBeEmpty(this Guid parameter, string parameterName = null, string message = null)
        {
            if (parameter == Guid.Empty)
                Throw.EmptyGuid(parameterName, message);
            return parameter;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Guid MustNotBeEmpty(this Guid parameter, Func<Exception> exceptionFactory)
        {
            if (parameter == Guid.Empty)
                Throw.CustomException(exceptionFactory);
            return parameter;
        }
    }

    public static partial class Throw
    {
        public static void EmptyGuid(string parameterName = null, string message = null) =>
            throw new EmptyGuidException(parameterName, message ?? $"{parameterName ?? "The value"} must be a valid GUID, but it actually is an empty one.");

        public static void CustomException(Func<Exception> exceptionFactory) =>
            throw exceptionFactory();
    }

    [Serializable]
    public class EmptyGuidException : ArgumentException
    {
        public EmptyGuidException(string parameterName = null, string message = null) : base(message, parameterName) { }
        protected EmptyGuidException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}