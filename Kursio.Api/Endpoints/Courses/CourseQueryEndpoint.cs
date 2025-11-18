using Gridify;
using Gridify.EntityFramework;
using Kursio.Api.Contracts.Common;
using Kursio.Api.Domain;
using Kursio.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;

namespace Kursio.Api.Endpoints.Courses;

public class CourseQueryEndpoint
{
    [Tags("Courses")]
    [EndpointSummary("Course Query")]
    [EndpointDescription("Course query")]
    [WolverineGet("/courses")]
    public static async Task<Paging<Course>> CourseQuery([FromQuery] KursioQuery query, KursioDbContext dbContext)
    {
        return await dbContext.Courses.GridifyAsync(query.ToGridifyQuery());
    }
}