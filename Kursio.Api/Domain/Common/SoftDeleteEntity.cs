namespace Kursio.Api.Domain.Common;

public abstract class SoftDeleteEntity
{
    public Guid Id { get; protected init; }
    public bool IsDeleted { get; private set; }
    
    public void Delete()
    {
        IsDeleted = true;
    }
}