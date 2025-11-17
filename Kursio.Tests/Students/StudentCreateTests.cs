using Alba;
using Kursio.Api.Contracts.Students;
using Kursio.Api.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shouldly;

namespace Kursio.Tests.Students;

public class StudentCreateTests(Fixture fixture) : IClassFixture<Fixture>
{
    private readonly IAlbaHost _host = fixture.AlbaHost;
    
    [Fact]
    public async Task StudentCreateShouldThrowWhenFullNameIsEmpty()
    {
        var result = await _host.Scenario(x =>
        {
            x.Post.Url("/students");
            x.WriteJson(new CreateStudentRequest(""), null);
            x.StatusCodeShouldBe(StatusCodes.Status400BadRequest);
        });
        
        var problemDetails = await result.ReadAsJsonAsync<ProblemDetails>();

        var (errorCode, parameters) = problemDetails.GetError("fullName");
        
        errorCode.ShouldBe("REQUIRED");
        parameters.ShouldBeNull();
    }
    
    [Fact]
    public async Task StudentCreateShouldThrowWhenFullNameIsLong()
    {
        var result = await _host.Scenario(x =>
        {
            x.Post.Url("/students");
            x.WriteJson(new CreateStudentRequest(new string('x', 129)), null);
            x.StatusCodeShouldBe(StatusCodes.Status400BadRequest);
        });
        
        var problemDetails = await result.ReadAsJsonAsync<ProblemDetails>();
        
        var (errorCode, parameters) = problemDetails.GetError("fullName");
        
        errorCode.ShouldBe("MAX_LENGTH");
        parameters.ShouldNotBeNull().ShouldContainKeyAndValue("max", 128L);
    }
    
    [Fact]
    public async Task StudentCreateShouldWork()
    {
        var result = await _host.Scenario(x =>
        {
            x.Post.Url("/students");
            x.WriteJson(new CreateStudentRequest("Deniz Satır"), null);
        });

        var student = await result.ReadAsJsonAsync<StudentResponse>();
        
        student.FullName.ShouldBe("Deniz Satır");
        student.Id.ShouldNotBe(Guid.Empty);
    }
}