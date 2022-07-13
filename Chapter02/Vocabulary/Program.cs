using System.Reflection;



internal class Program
{
    System.Data.DataSet ds;
    HttpClient client;

    private static void Main(string[] args)
    {
        double height = 1.88;
        Console.WriteLine($"The variable {nameof(height)} has the value {height}");
        // Assembly? assembly = Assembly.GetEntryAssembly();
        // if (assembly == null) return;

        // foreach (AssemblyName name in assembly.GetReferencedAssemblies()) {
        //     Assembly a = Assembly.Load(name);
        //     int methodCount = 0;
        //     foreach (TypeInfo t in a.DefinedTypes) {
        //         methodCount += t.GetMethods().Count();
        //     }
        //     Console.WriteLine("{0} types with {1} methods in {2} assembly", arg0: a.DefinedTypes.Count(), arg1: methodCount, arg2: name.Name);
        // }                
    }
}