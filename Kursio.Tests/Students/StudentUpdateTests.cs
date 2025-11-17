using Alba;
using Kursio.Api.Contracts.Students;
using Kursio.Api.Extensions;
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
            x.WriteJson(new UpdateStudentRequest("Deniz Satır"), null);
            x.StatusCodeShouldBe(StatusCodes.Status404NotFound);
        });
    }
    
    [Fact]
    public async Task StudentUpdateShouldThrowWhenFullNameIsEmpty()
    {
        var createResult = await _host.Scenario(x =>
        {
            x.Post.Url("/students");
            x.WriteJson(new CreateStudentRequest("Deniz Satır"), null);
        });

        var student = await createResult.ReadAsJsonAsync<StudentResponse>();
        
        var updateResult = await _host.Scenario(x =>
        {
            x.Put.Url($"/students/{student.Id}");
            x.WriteJson(new UpdateStudentRequest(""), null);
            x.StatusCodeShouldBe(StatusCodes.Status400BadRequest);
        });
        
        var problemDetails = await updateResult.ReadAsJsonAsync<ProblemDetails>();
        
        var (errorCode, parameters) = problemDetails.GetError("fullName");
        
        errorCode.ShouldBe("REQUIRED");
        parameters.ShouldBeNull();
    }
    
    [Fact]
    public async Task StudentUpdateShouldThrowWhenFullNameIsLong()
    {
        var createResult = await _host.Scenario(x =>
        {
            x.Post.Url("/students");
            x.WriteJson(new CreateStudentRequest("Deniz Satır"), null);
        });

        var student = await createResult.ReadAsJsonAsync<StudentResponse>();
        
        var updateResult = await _host.Scenario(x =>
        {
            x.Put.Url($"/students/{student.Id}");
            x.WriteJson(new UpdateStudentRequest(new string('x', 129)), null);
            x.StatusCodeShouldBe(StatusCodes.Status400BadRequest);
        });
        
        var problemDetails = await updateResult.ReadAsJsonAsync<ProblemDetails>();
        
        var (errorCode, parameters) = problemDetails.GetError("fullName");
        
        errorCode.ShouldBe("MAX_LENGTH");
        parameters.ShouldNotBeNull().ShouldContainKeyAndValue("max", 128L);
    }
    
    [Fact]
    public async Task StudentUpdateShouldWork()
    {
        var createResult = await _host.Scenario(x =>
        {
            x.Post.Url("/students");
            x.WriteJson(new CreateStudentRequest("Deniz Satır"), null);
        });

        var student = await createResult.ReadAsJsonAsync<StudentResponse>();
        
        var updateResult = await _host.Scenario(x =>
        {
            x.Put.Url($"/students/{student.Id}");
            x.WriteJson(new UpdateStudentRequest("Deniz Satır2"), null);
        });
        
        var updatedStudent = await updateResult.ReadAsJsonAsync<StudentResponse>();
        
        updatedStudent.FullName.ShouldBe("Deniz Satır2");
        updatedStudent.Id.ShouldBe(student.Id);
    }
}