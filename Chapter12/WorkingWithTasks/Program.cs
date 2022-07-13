using System.Diagnostics;
using System.Runtime.CompilerServices;
using static System.Console;

internal class Program
{
    private static void Main(string[] args)
    {
        OutputThreadInfo();
        Stopwatch timer = Stopwatch.StartNew();
        // WriteLine("Run sync");
        // MethodA();
        // MethodB();
        // MethodC();

        // WriteLine("run async in multi thread");
        // Task taskA = new(MethodA);
        // taskA.Start();
        // Task taskB = Task.Factory.StartNew(MethodB);
        // Task taskC = Task.Run(MethodC);
        // Task[] tasks = { taskA, taskB, taskC };
        // Task.WaitAll(tasks);
        // WriteLine($"{timer.ElapsedMilliseconds:#,##0}ms elapsed");

        WriteLine("Passing the result of one task as an input into another");
        Task<string> taskServicethenSPro = Task.Factory
                                            .StartNew(CallWebService)
                                            .ContinueWith(r => CallStoredProcedure(r.Result));
        WriteLine($"Result: {taskServicethenSPro.Result}");
        Interlocked.Increment()
    }

    static void OutputThreadInfo() {
        Thread t = Thread.CurrentThread;
        WriteLine($"Thread id={t.ManagedThreadId}, Priority={t.Priority}, Background={t.IsBackground}, Name={t.Name ?? "null"}");
    }

    static void MethodA() {
        WriteLine("Start Method A...");
        OutputThreadInfo();
        Thread.Sleep(3000);
        WriteLine("Finish Method A");
    }

    static void MethodB() {
        WriteLine("Start Method B...");
        OutputThreadInfo();
        Thread.Sleep(2000);
        WriteLine("Finish Method B");
    }

    static void MethodC() {
        WriteLine("Start Method C...");
        OutputThreadInfo();
        Thread.Sleep(1000);
        WriteLine("Finish Method C");
    }

    static decimal CallWebService() {
        WriteLine("start call to web service");
        OutputThreadInfo();

        Thread.Sleep((new Random()).Next(2000, 4000));
        WriteLine("Finish call web service");
        return 89.99M;
    }

    static string CallStoredProcedure(decimal amount) {
        WriteLine("start call to stored proc");
        OutputThreadInfo();

        Thread.Sleep((new Random()).Next(2000, 4000));
        WriteLine("Finish call to stored proc");
        return $"12 products cost more than {amount:C}";
    }
}