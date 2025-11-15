using Kursio.Api.Contracts.Students;
using Kursio.Api.Domain;
using Kursio.Api.Extensions;
using Kursio.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;

namespace Kursio.Api.Endpoints.Students;

public static class AddStudentEndpoint
{
    public static ProblemDetails Validate(CreateStudentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            return new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
            }.AddError("fullName", "REQUIRED");
        }

        if (request.FullName.Length > 128)
        {
            return new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
            }.AddError("fullName", "MAXLENGTH,128");
        }

        return WolverineContinue.NoProblems;
    }
    
    [Tags("Students")]
    [EndpointSummary("Add Student")]
    [EndpointDescription("Add student")]
    [WolverinePost("/students")]
    public static async Task<StudentResponse> AddStudent(CreateStudentRequest request, KursioDbContext dbContext)
    {
        var student = Student.Create(request.FullName);
        
        await dbContext.Students.AddAsync(student);
        await dbContext.SaveChangesAsync();
        
        return new StudentResponse(student.Id, student.FullName);
    }
}