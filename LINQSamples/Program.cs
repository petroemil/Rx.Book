using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LINQSamples
{
    public class Program
    {
        public static void Main(string[] args)
        {
        }
    }

    public class MyEnumerable : IEnumerable<object>
    {
        public IEnumerator<object> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    public class MyEnumerator : IEnumerator<object>
    {
        public object Current
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool MoveNext()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }

    public class Samples
    {
        public int TheOldAproach()
        {
            List<int> numbers = new List<int>();
            numbers.Add(1);
            numbers.Add(2);
            numbers.Add(3);
            numbers.Add(4);
            numbers.Add(5);
            numbers.Add(6);
            numbers.Add(7);
            numbers.Add(8);
            numbers.Add(9);
            numbers.Add(10);

            int accumulator = 0;
            foreach (var number in numbers)
            {
                if (number % 2 == 0)
                {
                    accumulator += (number * number);
                }
            }

            return accumulator;
        }

        public void CollectionInitialiser()
        {
            var numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        }

        public void AnonymusTypes()
        {
            var person = new { Name = "Emil", Age = 27 };
        }

        public int LINQAproach()
        {
            var numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            return numbers
                .Where(num => num % 2 == 0)
                .Select(num => num * num)
                .Sum();
        }

        public void ExtensionMethod()
        {
            "Hello World".SpecialFormat(42);
        }

        public void FullLambdaSyntaxSample()
        {
            Func<int, string, bool> sampleFunc =
                (param1, param2) =>
                {
                    // ...
                    return true;
                };
        }

        public int PLINQSample()
        {
            var numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            return numbers
                .AsParallel()
                .Where(num => num % 2 == 0)
                .Select(num => num * num)
                .Sum();
        }
    }

    public static class ExtensionMethodExample
    {
        public static string SpecialFormat(this string s, int spaces)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<T> Where<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
