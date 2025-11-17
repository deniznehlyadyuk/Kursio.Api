using Alba;
using Kursio.Api.Contracts.Students;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shouldly;
using Kursio.Tests.Extensions;

namespace Kursio.Tests.Students;

public class StudentCreateTests(Fixture fixture) : IClassFixture<Fixture>
{
    private readonly IAlbaHost _host = fixture.AlbaHost;
    
    [Fact]
    public async Task StudentCreateShouldThrowWhenFullNameIsEmptyAndPaymentAmountIsNegative()
    {
        var result = await _host.Scenario(x =>
        {
            x.Post.Url("/students");
            x.WriteJson(new CreateStudentRequest("", -1), null);
            x.StatusCodeShouldBe(StatusCodes.Status400BadRequest);
        });
        
        var problemDetails1 = await result.ReadAsJsonAsync<ProblemDetails>();

        problemDetails1.ShouldContainError("fullName", "REQUIRED");
        problemDetails1.ShouldContainError("paymentAmount", "MIN_VALUE", new Dictionary<string, object> {{"min", 0L}});
    }
    
    [Fact]
    public async Task StudentCreateShouldThrowWhenFullNameIsLongAndPaymentAmountIsNegative()
    {
        var result = await _host.Scenario(x =>
        {
            x.Post.Url("/students");
            x.WriteJson(new CreateStudentRequest(new string('x', 129), -1), null);
            x.StatusCodeShouldBe(StatusCodes.Status400BadRequest);
        });
        
        var problemDetails1 = await result.ReadAsJsonAsync<ProblemDetails>();

        problemDetails1.ShouldContainError("fullName", "MAX_LENGTH", new Dictionary<string, object> {{"max", 128L}});
        problemDetails1.ShouldContainError("paymentAmount", "MIN_VALUE", new Dictionary<string, object> {{"min", 0L}});
    }
    
    [Fact]
    public async Task StudentCreateShouldWork()
    {
        var result = await _host.Scenario(x =>
        {
            x.Post.Url("/students");
            x.WriteJson(new CreateStudentRequest("Deniz Satır", 500), null);
        });

        var student = await result.ReadAsJsonAsync<StudentResponse>();
        
        student.FullName.ShouldBe("Deniz Satır");
        student.PaymentAmount.ShouldBe(500);
        student.Id.ShouldNotBe(Guid.Empty);
    }
}