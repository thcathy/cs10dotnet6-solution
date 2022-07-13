using System.Diagnostics;
using static System.Console;

internal class Program
{
    static int max = 45;

    private static void Main(string[] args)
    {
        Stopwatch watch = new();
        Write("Press ENTER to start. ");
        ReadLine();

        watch.Start();
        var numbers = Enumerable.Range(1, max);
        WriteLine($"calculating Fibonacci seq up to {max}.......");
        int[] fibonacciNumbners = numbers.AsParallel().Select(number => Fibonacci(number)).ToArray();
        watch.Stop();

        WriteLine($"{watch.ElapsedMilliseconds:#,##0}ms");
        WriteLine($"Results: {string.Join(" ", fibonacciNumbners)}");
    }

    static int Fibonacci(int term) =>
        term switch
        {
            1 => 0,
            2 => 1,
            _ => Fibonacci(term - 1) + Fibonacci(term - 2)
        };

}