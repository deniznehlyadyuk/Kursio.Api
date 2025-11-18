using Kursio.Api.Contracts.Courses;
using Kursio.Api.Domain;
using Kursio.Api.Extensions;
using Kursio.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wolverine.Http;

namespace Kursio.Api.Endpoints.Courses;

public class UpdateCourseEndpoint
{
    public static async Task<(Course?, ProblemDetails)> LoadAsync(HttpRequest httpRequest, KursioDbContext dbContext)
    {
        var courseId = httpRequest.GetGuid("courseId");
        
        var course = await dbContext.Courses.FindAsync(courseId);

        if (course is null)
        {
            return (null, new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Detail = "Course not found"
            });
        }
        
        return (course, WolverineContinue.NoProblems);
    }
    
    public static async Task<ProblemDetails> ValidateAsync(UpdateCourseRequest request, Course updatingCourse,
        KursioDbContext dbContext)
    {
        var overlapCourse = await dbContext.Courses.FirstOrDefaultAsync(
            course => course.Id != updatingCourse.Id &&
                      course.EndTime > request.StartTime && request.StartTime.AddHours(1) > course.StartTime);

        if (overlapCourse != null)
        {
            return new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest
            }.AddError("startTime", "OVERLAP", new Dictionary<string, object>
            {
                {"startTime", overlapCourse.StartTime},
                {"endTime", overlapCourse.EndTime}
            });
        }
        
        return WolverineContinue.NoProblems;
    }
    
    [Tags("Courses")]
    [EndpointSummary("Update Course")]
    [EndpointDescription("Update course")]
    [WolverinePut("/courses/{courseId}")]
    public static async Task<CourseResponse> UpdateCourse(UpdateCourseRequest request, Course course,
        KursioDbContext dbContext)
    {
        course.Update(request.StartTime);
        
        dbContext.Courses.Update(course);

        await dbContext.SaveChangesAsync();
        
        return new CourseResponse(course.Id, course.StartTime, course.EndTime);
    }
}