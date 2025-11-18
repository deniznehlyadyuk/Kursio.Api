using Kursio.Api.Domain.Common;

namespace Kursio.Api.Domain;

public class Course : SoftDeleteEntity
{
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }

    private Course()
    {
    }

    public static Course Create(TimeOnly startTime)
    {   
        return new Course
        {
            StartTime = startTime,
            EndTime = startTime.AddHours(1)
        };
    }
}