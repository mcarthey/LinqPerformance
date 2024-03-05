using BenchmarkDotNet.Attributes;
using System;
using System.Linq;
using System.Numerics;
using BenchmarkDotNet.Jobs;
using SimdLinq;

namespace LinqPerformance
{
    // This class is used to benchmark different methods of summing numbers
    // BenchmarkDotNet will measure how long the methods takes to execute
    [SimpleJob(RuntimeMoniker.Net60)]
    [SimpleJob(RuntimeMoniker.Net70)]
    [SimpleJob(RuntimeMoniker.Net80)]
    public class Benchmark
    {
        private int[] numbers;

        public Benchmark()
        {
            numbers = Enumerable.Range(1, 10000).ToArray();
        }

        // This method uses LINQ's Sum method to sum the numbers
        [Benchmark]
        public int MeasureLinqSum()
        {
            return numbers.Sum();
        }

        // This method sums the numbers manually using a for loop
        [Benchmark]
        public int MeasureManualSum()
        {
            int sum = 0;
            for (int i = 0; i < numbers.Length; i++)
            {
                sum += numbers[i];
            }
            return sum;
        }

        // This method uses Span<T> to sum the numbers
        // Span<T> is a new type in .NET that provides a lightweight, flexible slice of a sequence
        // It can improve performance in certain scenarios
        [Benchmark]
        public int MeasureSpanSum()
        {
            Span<int> span = numbers.AsSpan();
            int sum = 0;
            for (int i = 0; i < span.Length; i++)
            {
                sum += span[i];
            }
            return sum;
        }


        [Benchmark]
        public int MeasureVectorizedSum()
        {
            // Vector<T> can perform multiple operations at once using SIMD
            // We'll use it to sum the numbers in the array
            var sumVector = Vector<int>.Zero;
            int i = 0;

            // Sum up vectors as long as we have a full vector's worth of numbers left
            for (; i <= numbers.Length - Vector<int>.Count; i += Vector<int>.Count)
            {
                var v = new Vector<int>(numbers, i);
                sumVector += v;
            }

            // Sum up the remaining numbers
            int remainingSum = 0;
            for (; i < numbers.Length; i++)
            {
                remainingSum += numbers[i];
            }

            // Combine the sums
            int vectorSum = Vector.Dot(sumVector, Vector<int>.One);
            return vectorSum + remainingSum;
        }

        [Benchmark]
        public int MeasureSimdLinqSum()
        {
            // SimdLinq provides LINQ-style extensions for SIMD operations
            // We'll use it to sum the numbers in the array
            return SimdLinqExtensions.Sum(numbers);
        }
    }
}
