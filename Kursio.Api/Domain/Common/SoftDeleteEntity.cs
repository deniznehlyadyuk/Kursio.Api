namespace Kursio.Api.Domain.Common;

public abstract class SoftDeleteEntity
{
    public bool IsDeleted { get; private set; }
    
    public void Delete()
    {
        IsDeleted = true;
    }
}