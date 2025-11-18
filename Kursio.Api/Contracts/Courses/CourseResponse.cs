namespace Kursio.Api.Contracts.Courses;

public record CourseResponse(Guid Id, TimeOnly StartTime, TimeOnly EndTime);