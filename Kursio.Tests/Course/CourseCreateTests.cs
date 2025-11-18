using System.Net;
using Alba;
using Kursio.Api.Contracts.Courses;
using Kursio.Tests.Extensions;
using Microsoft.AspNetCore.Mvc;
using Shouldly;

namespace Kursio.Tests.Course;

public class CourseCreateTests(Fixture fixture) : IClassFixture<Fixture>
{
    private readonly IAlbaHost _host = fixture.AlbaHost;
    
    [Fact]
    public async Task CourseCreateShouldThrowWhenOverlap()
    {
        await _host.Scenario(x =>
        {
            x.Post.Url("/courses");
            x.WriteJson(new CreateCourseRequest(TimeOnly.Parse("09:00:00")), null);
        });
        
        var result = await _host.Scenario(x =>
        {
            x.Post.Url("/courses");
            x.WriteJson(new CreateCourseRequest(TimeOnly.Parse("09:30:00")), null);
            x.StatusCodeShouldBe(HttpStatusCode.BadRequest);
        });
        
        var problemDetails = await result.ReadAsJsonAsync<ProblemDetails>();

        problemDetails.ShouldContainError("startTime", "OVERLAP", new Dictionary<string, object>
        {
            {"startTime", "09:00:00"},
            {"endTime", "10:00:00"}
        });
    }
    
    [Fact]
    public async Task CourseCreateShouldWork()
    {
        var result = await _host.Scenario(x =>
        {
            x.Post.Url("/courses");
            x.WriteJson(new CreateCourseRequest(TimeOnly.Parse("12:00:00")), null);
        });

        var course = await result.ReadAsJsonAsync<CourseResponse>();
        
        course.StartTime.ShouldBe(TimeOnly.Parse("12:00:00"));
        course.EndTime.ShouldBe(TimeOnly.Parse("13:00:00"));
        course.Id.ShouldNotBe(Guid.Empty);
    }
}