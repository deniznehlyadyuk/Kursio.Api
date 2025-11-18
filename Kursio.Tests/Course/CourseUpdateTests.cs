using Alba;
using Kursio.Api.Contracts.Courses;
using Kursio.Tests.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shouldly;

namespace Kursio.Tests.Course;

public class CourseUpdateTests(Fixture fixture) : IClassFixture<Fixture>
{
    private readonly IAlbaHost _host = fixture.AlbaHost;
    
    [Fact]
    public async Task CourseUpdateShouldThrowWhenCourseNotExists()
    {
        await _host.Scenario(x =>
        {
            x.Put.Url($"/courses/{Guid.NewGuid()}");
            x.WriteJson(new UpdateCourseRequest(TimeOnly.Parse("08:00:00")), null);
            x.StatusCodeShouldBe(StatusCodes.Status404NotFound);
        });
    }
    
    [Fact]
    public async Task CourseUpdateShouldThrowWhenOverlap()
    {
        await _host.Scenario(x =>
        {
            x.Post.Url("/courses");
            x.WriteJson(new CreateCourseRequest(TimeOnly.Parse("08:00:00")), null);
        });
        
        var createResult = await _host.Scenario(x =>
        {
            x.Post.Url("/courses");
            x.WriteJson(new CreateCourseRequest(TimeOnly.Parse("09:00:00")), null);
        });

        var course = await createResult.ReadAsJsonAsync<CourseResponse>();
        
        var updateResult = await _host.Scenario(x =>
        {
            x.Put.Url($"/courses/{course.Id}");
            x.WriteJson(new UpdateCourseRequest(TimeOnly.Parse("08:59:59")), null);
            x.StatusCodeShouldBe(StatusCodes.Status400BadRequest);
        });
        
        var problemDetails = await updateResult.ReadAsJsonAsync<ProblemDetails>();

        problemDetails.ShouldContainError("startTime", "OVERLAP", new Dictionary<string, object>()
        {
            { "startTime", "08:00:00" },
            { "endTime", "09:00:00" },
        });
    }
    
    [Fact]
    public async Task CourseUpdateShouldWork()
    {
        var createResult = await _host.Scenario(x =>
        {
            x.Post.Url("/courses");
            x.WriteJson(new CreateCourseRequest(TimeOnly.Parse("10:00:00")), null);
        });

        var course = await createResult.ReadAsJsonAsync<CourseResponse>();
        
        var updateResult = await _host.Scenario(x =>
        {
            x.Put.Url($"/courses/{course.Id}");
            x.WriteJson(new UpdateCourseRequest(TimeOnly.Parse("10:10:00")), null);
        });
        
        var updatedCourse = await updateResult.ReadAsJsonAsync<CourseResponse>();
        
        updatedCourse.StartTime.ShouldBe(TimeOnly.Parse("10:10:00"));
        updatedCourse.EndTime.ShouldBe(TimeOnly.Parse("11:10:00"));
        updatedCourse.Id.ShouldBe(course.Id);
    }
}