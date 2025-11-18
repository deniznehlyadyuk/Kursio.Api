using Alba;
using Kursio.Api.Contracts.Common;
using Kursio.Api.Contracts.Courses;
using Shouldly;

namespace Kursio.Tests.Course;

public class CourseDeleteTests(Fixture fixture) : IClassFixture<Fixture>
{
    private readonly IAlbaHost _host = fixture.AlbaHost;
    
    [Fact]
    public async Task CourseDeleteShouldReturnZeroWhenAllCourseIdsWrong()
    {
        var result = await _host.Scenario(x =>
        {
            x.Delete.Url($"/courses")
                .QueryString("courseIds", Guid.NewGuid().ToString())
                .QueryString("courseIds", Guid.NewGuid().ToString());
        });
        
        var response = await result.ReadAsJsonAsync<DeleteResponse>();

        response.Count.ShouldBe(0);
    }
    
    [Fact]
    public async Task CourseDeleteShouldReturnOneWhenOnlyOneCourseIdCorrect()
    {
        var createResult = await _host.Scenario(x =>
        {
            x.Post.Url("/courses");
            x.WriteJson(new CreateCourseRequest(TimeOnly.Parse("08:00:00")), null);
        });
        
        var course = await createResult.ReadAsJsonAsync<CourseResponse>();
        
        var result = await _host.Scenario(x =>
        {
            x.Delete.Url($"/courses")
                .QueryString("courseIds", Guid.NewGuid().ToString())
                .QueryString("courseIds", course.Id.ToString());;
        });
        
        var response = await result.ReadAsJsonAsync<DeleteResponse>();

        response.Count.ShouldBe(1);
    }
    
    [Fact]
    public async Task CourseDeleteShouldReturnOneWhenAllCourseIdsCorrect()
    {
        var createResult1 = await _host.Scenario(x =>
        {
            x.Post.Url("/courses");
            x.WriteJson(new CreateCourseRequest(TimeOnly.Parse("09:00:00")), null);
        });
        
        var createResult2 = await _host.Scenario(x =>
        {
            x.Post.Url("/courses");
            x.WriteJson(new CreateCourseRequest(TimeOnly.Parse("10:00:00")), null);
        });
        
        var course1 = await createResult1.ReadAsJsonAsync<CourseResponse>();
        var course2 = await createResult2.ReadAsJsonAsync<CourseResponse>();
        
        var result = await _host.Scenario(x =>
        {
            x.Delete.Url($"/courses")
                .QueryString("courseIds", course1.Id.ToString())
                .QueryString("courseIds", course2.Id.ToString());
        });
        
        var response = await result.ReadAsJsonAsync<DeleteResponse>();

        response.Count.ShouldBe(2);
    }
}