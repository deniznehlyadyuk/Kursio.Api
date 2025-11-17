namespace Kursio.Api.Contracts.Students;

public record StudentResponse(Guid Id, string FullName, int PaymentAmount);