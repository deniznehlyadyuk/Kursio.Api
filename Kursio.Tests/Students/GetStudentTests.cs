using Alba;
using Kursio.Api.Contracts.Students;
using Microsoft.AspNetCore.Http;
using Shouldly;

namespace Kursio.Tests.Students;

public class GetStudentTests(Fixture fixture) : IClassFixture<Fixture>
{
    private readonly IAlbaHost _host = fixture.AlbaHost;

    [Fact]
    public async Task GetStudentShouldThrowWhenStudentIdIsWrong()
    {
        await _host.Scenario(x =>
        {
            x.Get.Url($"/students/{Guid.NewGuid()}");
            x.StatusCodeShouldBe(StatusCodes.Status404NotFound);
        });
    }
    
    [Fact]
    public async Task GetStudentShouldThrowWhenStudentNotExists()
    {
        var result = await _host.Scenario(x =>
        {
            x.Post.Url("/students");
            x.WriteJson(new CreateStudentRequest("Deniz Satır", 500), null);
        });
        
        var student = await result.ReadAsJsonAsync<StudentResponse>();
        
        await _host.Scenario(x =>
        {
            x.Delete.Url($"/students")
                .QueryString("studentIds", student.Id.ToString());;
        });
        
        await _host.Scenario(x =>
        {
            x.Get.Url($"/students/{student.Id}");
            x.StatusCodeShouldBe(StatusCodes.Status404NotFound);
        });
    }
    
    [Fact]
    public async Task GetStudentShouldWork()
    {
        var createResult = await _host.Scenario(x =>
        {
            x.Post.Url("/students");
            x.WriteJson(new CreateStudentRequest("Deniz Satır", 500), null);
        });
        
        var student = await createResult.ReadAsJsonAsync<StudentResponse>();
        
        var result = await _host.Scenario(x =>
        {
            x.Get.Url($"/students/{student.Id}");
        });
        
        var returnedStudent = await result.ReadAsJsonAsync<StudentResponse>();
        
        returnedStudent.FullName.ShouldBe(student.FullName);
        returnedStudent.Id.ShouldBe(student.Id);
        returnedStudent.PaymentAmount.ShouldBe(student.PaymentAmount);
    }
}