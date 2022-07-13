using static System.Console;
internal class Program
{
    private static void Main(string[] args)
    {
        ConsoleKeyInfo key;
        do {
            Write("Press any key: ");
            key = ReadKey();
            WriteLine($"Key: {key.Key}, Char: {key.KeyChar}, Modifiers: {key.Modifiers}");
        } while (key.KeyChar != 'q');
        
    }
}