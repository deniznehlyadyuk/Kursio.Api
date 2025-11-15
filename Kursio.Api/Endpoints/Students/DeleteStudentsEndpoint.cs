using Kursio.Api.Contracts.Common;
using Kursio.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Wolverine.Http;

namespace Kursio.Api.Endpoints.Students;

public static class DeleteStudentsEndpoint
{
    [Tags("Students")]
    [EndpointSummary("Delete Student")]
    [EndpointDescription("Delete student")]
    [WolverineDelete("/students")]
    public static async Task<DeleteResponse> DeleteStudents(IEnumerable<Guid> studentIds, [NotBody] KursioDbContext dbContext)
    {
        var count = await dbContext.Students.Where(student => studentIds.Contains(student.Id)).ExecuteDeleteAsync();
        
        return new DeleteResponse(count);
    }
}