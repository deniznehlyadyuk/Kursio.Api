using Kursio.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Kursio.Api.Infrastructure;

public class KursioDbContext(DbContextOptions<KursioDbContext> options) : DbContext(options)
{
    public DbSet<Student> Students { get; set; }
    public DbSet<Course> Courses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>().HasQueryFilter("SoftDelete", student => !student.IsDeleted);
        modelBuilder.Entity<Course>().HasQueryFilter("SoftDelete", course => !course.IsDeleted);

        var timeOnlyConverter = new ValueConverter<TimeOnly, TimeSpan>(
            toDb => toDb.ToTimeSpan(),
            fromDb => TimeOnly.FromTimeSpan(fromDb));
        
        modelBuilder.Entity<Course>()
            .Property(course => course.StartTime)
            .HasConversion(timeOnlyConverter);
        
        modelBuilder.Entity<Course>()
            .Property(course => course.EndTime)
            .HasConversion(timeOnlyConverter);
        
        base.OnModelCreating(modelBuilder);
    }
}