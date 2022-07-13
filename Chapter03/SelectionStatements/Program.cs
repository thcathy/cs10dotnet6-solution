using static System.Console;

internal class Program
{
    private static void Main(string[] args)
    {        
        CardinalToOrdinal(10);
    }

    /// <summary>
    ///  Car Car Car ar ho la 
    /// </summary>
    /// <param name="number">give me a integer la</param>
    /// <returns>a string lor</returns>
    static string CardinalToOrdinal(int number) {
        switch (number) {
            case 11:
            case 12:
            case 13:
                return $"{number}th";
            default:
                int lastDigit = number % 10;
                string suffix = lastDigit switch {
                    1 => "st",
                    2 => "nd",
                    3 => "rd",
                    _ => "th"
                };
                return $"{number}{suffix}";
        }
    }
}