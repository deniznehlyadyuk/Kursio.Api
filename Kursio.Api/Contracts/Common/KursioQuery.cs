using Gridify;

namespace Kursio.Api.Contracts.Common;

public class KursioQuery
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string? OrderBy { get; set; }
    public string? Filter { get; set; }

    public GridifyQuery ToGridifyQuery() => new GridifyQuery(Page, PageSize, Filter, OrderBy);
}