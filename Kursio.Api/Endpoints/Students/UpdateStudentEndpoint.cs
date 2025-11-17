using Kursio.Api.Contracts.Students;
using Kursio.Api.Domain;
using Kursio.Api.Extensions;
using Kursio.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;

namespace Kursio.Api.Endpoints.Students;

public static class UpdateStudentEndpoint
{
    public static async Task<(Student?, ProblemDetails)> LoadAsync(UpdateStudentRequest request,
        HttpRequest httpRequest,
        KursioDbContext dbContext)
    {
        var studentId = httpRequest.GetGuid("studentId");
        
        var student = await dbContext.Students.FindAsync(studentId);

        if (student is null)
        {
            return (null, new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Detail = "Student not found"
            });
        }
        
        return (student, WolverineContinue.NoProblems);
    }
    
    public static ProblemDetails Validate(UpdateStudentRequest request)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
        };
        
        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            problemDetails.AddError("fullName", "REQUIRED");
        }
        
        if (request.FullName.Length > 128)
        {
            problemDetails.AddError("fullName", "MAX_LENGTH", new Dictionary<string, object> {{"max", 128}});
        }

        if (request.PaymentAmount < 0)
        {
            problemDetails.AddError("paymentAmount", "MIN_VALUE", new Dictionary<string, object> { { "min", 0 } });
        }

        if (problemDetails.HasErrors())
        {
            return problemDetails;
        }

        return WolverineContinue.NoProblems;
    }
    
    [Tags("Students")]
    [EndpointSummary("Update Student")]
    [EndpointDescription("Update student")]
    [WolverinePut("/students/{studentId:guid}")]
    public static async Task<StudentResponse> UpdateStudent(UpdateStudentRequest request, Student student,
        KursioDbContext dbContext)
    {
        student.Update(request.FullName, request.PaymentAmount);
        
        dbContext.Students.Update(student);

        await dbContext.SaveChangesAsync();
        
        return new StudentResponse(student.Id, student.FullName, student.PaymentAmount);
    }
}