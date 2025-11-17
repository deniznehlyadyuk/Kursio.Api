using Kursio.Api.Contracts.Students;
using Kursio.Api.Domain;
using Kursio.Api.Extensions;
using Kursio.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;

namespace Kursio.Api.Endpoints.Students;

public static class GetStudentEndpoint
{
    public static async Task<(Student?, ProblemDetails)> LoadAsync(HttpRequest httpRequest, KursioDbContext dbContext)
    {
        var studentId = httpRequest.GetGuid("studentId");
        
        var student = await dbContext.Students.FindAsync(studentId);

        if (student == null)
        {
            return (null, new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Detail = "Student not found"
            });
        }

        return (student, WolverineContinue.NoProblems);
    }
    
    [Tags("Students")]
    [EndpointSummary("Get Student")]
    [EndpointDescription("Get student")]
    [WolverineGet("/students/{studentId}")]
    public static StudentResponse GetStudent(Student student)
    {
        return new StudentResponse(student.Id, student.FullName, student.PaymentAmount);
    }
}