using Alba;
using Kursio.Api.Contracts.Common;
using Kursio.Api.Contracts.Students;
using Shouldly;

namespace Kursio.Tests.Students;

public class StudentDeleteTests(Fixture fixture) : IClassFixture<Fixture>
{
    private readonly IAlbaHost _host = fixture.AlbaHost;
    
    [Fact]
    public async Task StudentDeleteShouldReturnZeroWhenAllStudentIdsWrong()
    {
        var result = await _host.Scenario(x =>
        {
            x.Delete.Url($"/students")
                .QueryString("studentIds", Guid.NewGuid().ToString())
                .QueryString("studentIds", Guid.NewGuid().ToString());
        });
        
        var response = await result.ReadAsJsonAsync<DeleteResponse>();

        response.Count.ShouldBe(0);
    }
    
    [Fact]
    public async Task StudentDeleteShouldReturnOneWhenOnlyOneStudentIdCorrect()
    {
        var createResult = await _host.Scenario(x =>
        {
            x.Post.Url("/students");
            x.WriteJson(new CreateStudentRequest("Deniz Satır1", 500), null);
        });
        
        var student = await createResult.ReadAsJsonAsync<StudentResponse>();
        
        var result = await _host.Scenario(x =>
        {
            x.Delete.Url($"/students")
                .QueryString("studentIds", Guid.NewGuid().ToString())
                .QueryString("studentIds", student.Id.ToString());;
        });
        
        var response = await result.ReadAsJsonAsync<DeleteResponse>();

        response.Count.ShouldBe(1);
    }
    
    [Fact]
    public async Task StudentDeleteShouldReturnOneWhenAllStudentIdsCorrect()
    {
        var createResult1 = await _host.Scenario(x =>
        {
            x.Post.Url("/students");
            x.WriteJson(new CreateStudentRequest("Deniz Satır1", 500), null);
        });
        
        var createResult2 = await _host.Scenario(x =>
        {
            x.Post.Url("/students");
            x.WriteJson(new CreateStudentRequest("Deniz Satır2", 500), null);
        });
        
        var student1 = await createResult1.ReadAsJsonAsync<StudentResponse>();
        var student2 = await createResult2.ReadAsJsonAsync<StudentResponse>();
        
        var result = await _host.Scenario(x =>
        {
            x.Delete.Url($"/students")
                .QueryString("studentIds", student1.Id.ToString())
                .QueryString("studentIds", student2.Id.ToString());
        });
        
        var response = await result.ReadAsJsonAsync<DeleteResponse>();

        response.Count.ShouldBe(2);
    }
}