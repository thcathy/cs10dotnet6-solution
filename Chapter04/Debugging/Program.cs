using static System.Console;
using System.Diagnostics;

internal class Program
{
    private static void Main(string[] args)
    {
        Trace.Listeners.Add(new TextWriterTraceListener(
            File.CreateText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "log.txt"))
        ));
        Trace.AutoFlush = true;
        Debug.WriteLine("Debug !");
        Trace.WriteLine("Trace !");
    }

    static double Add(double a, double b) {
        return a * b;
    }
}