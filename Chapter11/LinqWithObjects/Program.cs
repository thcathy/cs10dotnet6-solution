using System.IO.Pipes;
using static System.Console;

internal class Program
{
    private static void Main(string[] args)
    {
        //queryNames();
        //queryType();
        PlaySet();
    }

    static void PlaySet() {
        string[] cohort1 = new[] { "Rachel", "Gareth", "Jonathan", "George" };
        string[] cohort2 = new[] { "Jack", "Stephen", "Daniel", "Jack", "Jared" };
        string[] cohort3 = new[] { "Declan", "Jack", "Jack", "Jasmine", "Conor" };
        Output(cohort1, "Cohort 1");
        Output(cohort2, "Cohort 2");
        Output(cohort3, "Cohort 3");
        Output(cohort2.DistinctBy(n => n.Substring(0, 2)), "cohor2.DistinctBy");
        Output(cohort2.Concat(cohort3), "2 concat 3");
        Output(cohort1.Zip(cohort2, (c1, c2) => $"{c1} matched with {c2}"), "1 zip 2");
    }

    static void Output(IEnumerable<string> cohort, string description = "") {
        if (!string.IsNullOrEmpty(description)) {
            WriteLine(description);
        }
        Write(" ");
        WriteLine(string.Join(", ", cohort.ToArray()));
        WriteLine();
    }

    static void queryType() {
        WriteLine("Filter by type");
        List<Exception> exceptions = new() {
            new ArgumentException(),
            new SystemException(),
            new IndexOutOfRangeException(),
            new InvalidOperationException(),
            new NullReferenceException(),
            new InvalidCastException(),
            new OverflowException(),
            new DivideByZeroException(),
            new ApplicationException()
        };
        var arithmeticExceptionQuery = exceptions.OfType<ArithmeticException>();
        foreach (ArithmeticException e in arithmeticExceptionQuery) {
            WriteLine(e);
        }
    }

    static void queryNames() {
        string[] names = new[] { "Michael", "Pam", "Jim", "Dwight", "Angela", "Kevin", "Toby", "Creed" };
        var query = names
            .Where(n => n.Length > 4)
            .OrderBy(n => n.Length)
            .ThenBy(n => n);
        foreach (string item in query) {
            WriteLine(item);
        }
    }

}