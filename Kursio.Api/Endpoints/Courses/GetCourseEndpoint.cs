using Kursio.Api.Contracts.Courses;
using Kursio.Api.Domain;
using Kursio.Api.Extensions;
using Kursio.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;

namespace Kursio.Api.Endpoints.Courses;

public class GetCourseEndpoint
{
    public static async Task<(Course?, ProblemDetails)> LoadAsync(HttpRequest httpRequest, KursioDbContext dbContext)
    {
        var courseId = httpRequest.GetGuid("courseId");
        
        var course = await dbContext.Courses.FindAsync(courseId);

        if (course == null)
        {
            return (null, new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Detail = "Course not found"
            });
        }

        return (course, WolverineContinue.NoProblems);
    }
    
    [Tags("Courses")]
    [EndpointSummary("Get Course")]
    [EndpointDescription("Get course")]
    [WolverineGet("/courses/{courseId}")]
    public static CourseResponse GetCourse(Course course)
    {
        return new CourseResponse(course.Id, course.StartTime, course.EndTime);
    }
}