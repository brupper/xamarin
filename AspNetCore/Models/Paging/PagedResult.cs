namespace Brupper.AspNetCore.Models;

public class PagedResult
{
    public int TotalCount { get; set; }
}

public class PagedResult<T> : PagedResult
{
    public List<T> Items { get; set; } = new();
}
