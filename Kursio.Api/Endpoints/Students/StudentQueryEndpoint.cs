using Gridify;
using Gridify.EntityFramework;
using Kursio.Api.Contracts.Common;
using Kursio.Api.Domain;
using Kursio.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;

namespace Kursio.Api.Endpoints.Students;

public static class StudentQueryEndpoint
{
    [Tags("Students")]
    [EndpointSummary("Student Query")]
    [EndpointDescription("Student query")]
    [WolverineGet("/students")]
    public static async Task<Paging<Student>> StudentQuery([FromQuery] KursioQuery query, KursioDbContext dbContext)
    {
        return await dbContext.Students.GridifyAsync(query.ToGridifyQuery());
    }
}