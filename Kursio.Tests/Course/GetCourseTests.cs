using Alba;
using Kursio.Api.Contracts.Courses;
using Microsoft.AspNetCore.Http;
using Shouldly;

namespace Kursio.Tests.Course;

public class GetCourseTests(Fixture fixture) : IClassFixture<Fixture>
{
    private readonly IAlbaHost _host = fixture.AlbaHost;

    [Fact]
    public async Task GetCourseShouldThrowWhenCourseIdIsWrong()
    {
        await _host.Scenario(x =>
        {
            x.Get.Url($"/courses/{Guid.NewGuid()}");
            x.StatusCodeShouldBe(StatusCodes.Status404NotFound);
        });
    }
    
    [Fact]
    public async Task GetCourseShouldThrowWhenCourseNotExists()
    {
        var result = await _host.Scenario(x =>
        {
            x.Post.Url("/courses");
            x.WriteJson(new CreateCourseRequest(TimeOnly.Parse("09:00:00")), null);
        });
        
        var course = await result.ReadAsJsonAsync<CourseResponse>();
        
        await _host.Scenario(x =>
        {
            x.Delete.Url($"/courses")
                .QueryString("courseIds", course.Id.ToString());;
        });
        
        await _host.Scenario(x =>
        {
            x.Get.Url($"/courses/{course.Id}");
            x.StatusCodeShouldBe(StatusCodes.Status404NotFound);
        });
    }
    
    [Fact]
    public async Task GetCourseShouldWork()
    {
        var createResult = await _host.Scenario(x =>
        {
            x.Post.Url("/courses");
            x.WriteJson(new CreateCourseRequest(TimeOnly.Parse("08:00:00")), null);
        });
        
        var course = await createResult.ReadAsJsonAsync<CourseResponse>();
        
        var result = await _host.Scenario(x =>
        {
            x.Get.Url($"/courses/{course.Id}");
        });
        
        var returnedCourse = await result.ReadAsJsonAsync<CourseResponse>();
        
        returnedCourse.Id.ShouldBe(course.Id);
        returnedCourse.StartTime.ShouldBe(course.StartTime);
        returnedCourse.EndTime.ShouldBe(course.StartTime.AddHours(1));
    }
}