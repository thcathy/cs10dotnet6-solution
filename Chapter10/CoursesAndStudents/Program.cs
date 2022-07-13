using CoursesAndStudents;
using Microsoft.EntityFrameworkCore;
using CoursesAndStudents;
using static System.Console;

using (Academy a = new()) {
    bool deleted = await a.Database.EnsureDeletedAsync();
    WriteLine($"db deleted: {deleted}");
    bool created = await a.Database.EnsureCreatedAsync();
    WriteLine($"db created: {created}");
    WriteLine("SQL script used to create db:");
    WriteLine(a.Database.GenerateCreateScript());
    
    foreach (Student s in a.Students.Include(s => s.Courses)) {
        WriteLine($"{s.FirstName} {s.LastName} attends the following {s.Courses.Count} courses:");
        foreach (Course c in s.Courses) {
            WriteLine($"    {c.Title}");
        }
    }
}