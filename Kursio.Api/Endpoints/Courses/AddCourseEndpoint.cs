using Kursio.Api.Contracts.Courses;
using Kursio.Api.Domain;
using Kursio.Api.Extensions;
using Kursio.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wolverine.Http;

namespace Kursio.Api.Endpoints.Courses;

public class AddCourseEndpoint
{
    public static async Task<ProblemDetails> ValidateAsync(CreateCourseRequest request, KursioDbContext dbContext)
    {
        var overlapCourse = await dbContext.Courses.FirstOrDefaultAsync(course => course.EndTime > request.StartTime && request.StartTime.AddHours(1) > course.StartTime);

        if (overlapCourse != null)
        {
            return new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest
            }.AddError("startTime", "OVERLAP", new Dictionary<string, object>{{"startTime", overlapCourse.StartTime}, {"endTime", overlapCourse.EndTime}});
        }
        
        return WolverineContinue.NoProblems;
    }
    
    [Tags("Courses")]
    [EndpointSummary("Add Course")]
    [EndpointDescription("Add course")]
    [WolverinePost("/courses")]
    public static async Task<CourseResponse> CreateCourse(CreateCourseRequest request, KursioDbContext dbContext)
    {
        var course = Course.Create(request.StartTime);
        
        await dbContext.Courses.AddAsync(course);

        await dbContext.SaveChangesAsync();
        
        return new CourseResponse(course.Id, course.StartTime, course.EndTime);
    }
}