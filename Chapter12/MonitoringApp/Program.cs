using System.Net.Http.Headers;
using System.Text;
using My.Shared;
using static System.Console;

internal class Program
{
    private static void Main(string[] args)
    {       
        // int[] largeArrayOfInts = Enumerable.Range(1, 10_000).ToArray();
        // Thread.Sleep(new Random().Next(5, 10) * 1000);

        int[] numbers = Enumerable.Range(1, 50_000).ToArray();
        WriteLine("Using string with +");
        Recorder.Start();
        string s = string.Empty;
        for (int i = 0; i < numbers.Length; i++) {
            s += numbers[i] + ", ";
        }
        Recorder.Stop();
        
        WriteLine("Using StringBuilder");
        Recorder.Start();
        StringBuilder sb = new();
        for (int i = 0; i < numbers.Length; i++) {
            sb.Append(numbers[i]).Append(", ");
        }
        Recorder.Stop();

        WriteLine("");

    }
}