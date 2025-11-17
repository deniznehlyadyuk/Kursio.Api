using System.ComponentModel.DataAnnotations;

namespace Kursio.Api.Domain;

public class Student
{
    public Guid Id { get; private set; }
    
    [MaxLength(128)]
    public string FullName { get; private set; } = null!;

    [Range(0, int.MaxValue)]
    public int PaymentAmount { get; private set; }

    protected Student()
    {
    }

    public static Student Create(string fullName, int paymentAmount)
    {
        return new Student
        {
            Id = Guid.NewGuid(),
            FullName = fullName,
            PaymentAmount =  paymentAmount
        };
    }

    public void Update(string fullName, int paymentAmount)
    {
        FullName = fullName;
        PaymentAmount = paymentAmount;
    }
}