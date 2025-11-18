using Kursio.Api.Contracts.Common;
using Kursio.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Wolverine.Http;

namespace Kursio.Api.Endpoints.Courses;

public class DeleteCourseEndpoint
{
    [Tags("Courses")]
    [EndpointSummary("Delete Courses")]
    [EndpointDescription("Delete courses")]
    [WolverineDelete("/courses")]
    public static async Task<DeleteResponse> DeleteCourses(IEnumerable<Guid> courseIds, [NotBody] KursioDbContext dbContext)
    {
        var count = await dbContext.Courses.Where(course => courseIds.Contains(course.Id))
            .ExecuteUpdateAsync(setter => setter.SetProperty(course => course.IsDeleted, true));
        
        return new DeleteResponse(count);
    }
}