using System.ComponentModel.DataAnnotations;

namespace Kursio.Api.Domain;

public class Student
{
    public Guid Id { get; private set; }
    
    [MaxLength(128)]
    public string FullName { get; private set; } = null!;

    protected Student()
    {
    }

    public static Student Create(string fullName)
    {
        return new Student
        {
            Id = Guid.NewGuid(),
            FullName = fullName
        };
    }

    public void Update(string fullName)
    {
        FullName = fullName;
    }
}