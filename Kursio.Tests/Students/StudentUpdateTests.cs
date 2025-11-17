using Alba;
using Kursio.Api.Contracts.Students;
using Kursio.Tests.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shouldly;

namespace Kursio.Tests.Students;

public class StudentUpdateTests(Fixture fixture) : IClassFixture<Fixture>
{
    private readonly IAlbaHost _host = fixture.AlbaHost;
        
    [Fact]
    public async Task StudentUpdateShouldThrowWhenStudentNotExists()
    {
        await _host.Scenario(x =>
        {
            x.Put.Url($"/students/{Guid.NewGuid()}");
            x.WriteJson(new UpdateStudentRequest("Deniz Satır", 500), null);
            x.StatusCodeShouldBe(StatusCodes.Status404NotFound);
        });
    }
    
    [Fact]
    public async Task StudentUpdateShouldThrowWhenFullNameIsEmptyAndPaymentAmountIsNegative()
    {
        var createResult = await _host.Scenario(x =>
        {
            x.Post.Url("/students");
            x.WriteJson(new CreateStudentRequest("Deniz Satır", 500), null);
        });

        var student = await createResult.ReadAsJsonAsync<StudentResponse>();
        
        var updateResult = await _host.Scenario(x =>
        {
            x.Put.Url($"/students/{student.Id}");
            x.WriteJson(new UpdateStudentRequest("", -1), null);
            x.StatusCodeShouldBe(StatusCodes.Status400BadRequest);
        });
        
        var problemDetails = await updateResult.ReadAsJsonAsync<ProblemDetails>();

        problemDetails.ShouldContainError("fullName", "REQUIRED");
        problemDetails.ShouldContainError("paymentAmount", "MIN_VALUE", new Dictionary<string, object> {{"min", 0L}});
    }
    
    [Fact]
    public async Task StudentUpdateShouldThrowWhenFullNameIsLongAndPaymentAmountIsNegative()
    {
        var createResult = await _host.Scenario(x =>
        {
            x.Post.Url("/students");
            x.WriteJson(new CreateStudentRequest("Deniz Satır", 500), null);
        });

        var student = await createResult.ReadAsJsonAsync<StudentResponse>();
        
        var updateResult = await _host.Scenario(x =>
        {
            x.Put.Url($"/students/{student.Id}");
            x.WriteJson(new UpdateStudentRequest(new string('x', 129), -1), null);
            x.StatusCodeShouldBe(StatusCodes.Status400BadRequest);
        });
        
        var problemDetails = await updateResult.ReadAsJsonAsync<ProblemDetails>();

        problemDetails.ShouldContainError("fullName", "MAX_LENGTH", new Dictionary<string, object> {{"max", 128L}});
        problemDetails.ShouldContainError("paymentAmount", "MIN_VALUE", new Dictionary<string, object> {{"min", 0L}});
    }
    
    [Fact]
    public async Task StudentUpdateShouldWork()
    {
        var createResult = await _host.Scenario(x =>
        {
            x.Post.Url("/students");
            x.WriteJson(new CreateStudentRequest("Deniz Satır", 500), null);
        });

        var student = await createResult.ReadAsJsonAsync<StudentResponse>();
        
        var updateResult = await _host.Scenario(x =>
        {
            x.Put.Url($"/students/{student.Id}");
            x.WriteJson(new UpdateStudentRequest("Deniz Satır2", 400), null);
        });
        
        var updatedStudent = await updateResult.ReadAsJsonAsync<StudentResponse>();
        
        updatedStudent.FullName.ShouldBe("Deniz Satır2");
        updatedStudent.PaymentAmount.ShouldBe(400);
        updatedStudent.Id.ShouldBe(student.Id);
    }
}