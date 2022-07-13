using Microsoft.EntityFrameworkCore;
using static System.Console;

namespace CoursesAndStudents;

public class Academy: DbContext {
    public DbSet<Student>? Students { get; set; }
    public DbSet<Course>? Courses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        string path = Path.Combine(Environment.CurrentDirectory, "Academy.db");
        WriteLine($"using {path} db file");
        optionsBuilder.UseSqlite($"Filename={path}");

        optionsBuilder.EnableSensitiveDataLogging(true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Student>()
            .Property(s => s.LastName).HasMaxLength(30).IsRequired();
        Student alice = new() { StudentId = 1, FirstName = "Alice", LastName = "Jones" };
        Student bob = new() { StudentId = 2, FirstName = "Bob", LastName = "Smith" };
        Student cecilia = new() { StudentId = 3, FirstName = "Cecilia", LastName = "Ramirez" };

        Course csharp = new() { CourseId = 1, Title = "C# 10 and .NET 6" };
        Course webdev = new() { CourseId = 2, Title = "Web development" };
        Course python = new() { CourseId = 3, Title = "Python for dummy" };

        modelBuilder.Entity<Student>().HasData(alice, bob, cecilia);
        modelBuilder.Entity<Course>().HasData(csharp, webdev, python);
        modelBuilder.Entity<Course>()
            .HasMany(c => c.Students)
            .WithMany(s => s.Courses)
            .UsingEntity(e => e.HasData(
                new { CoursesCourseId = 1, StudentsStudentId = 1},
                new { CoursesCourseId = 1, StudentsStudentId = 2},
                new { CoursesCourseId = 1, StudentsStudentId = 3},
                new { CoursesCourseId = 2, StudentsStudentId = 2},
                new { CoursesCourseId = 3, StudentsStudentId = 3}
            ));
    }
}