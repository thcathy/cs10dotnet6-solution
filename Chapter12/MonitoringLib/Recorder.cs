using System.Diagnostics;
using static System.Console;
using static System.Diagnostics.Process;

namespace My.Shared;

public static class Recorder {
    private static Stopwatch timer = new();
    private static long bytesPhysicalBefore = 0;
    private static long bytesVirtualBefore = 0;

    public static void Start() {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        bytesPhysicalBefore = GetCurrentProcess().WorkingSet64;
        bytesVirtualBefore = GetCurrentProcess().VirtualMemorySize64;
        timer.Restart();
    }

    public static void Stop() {
        timer.Stop();
        long bytesPhysicalAfter = GetCurrentProcess().WorkingSet64;
        long bytesVirtualAfter = GetCurrentProcess().VirtualMemorySize64;
        WriteLine("{0:N0} bytes physical used", bytesPhysicalAfter - bytesPhysicalBefore);
        WriteLine("{0:N0} bytes virtual used", bytesVirtualAfter - bytesVirtualBefore);
        WriteLine($"{timer.Elapsed} time span ellapsed");
        WriteLine("{0:N0} ms total ellapsed", timer.ElapsedMilliseconds);
        WriteLine();
    }
}