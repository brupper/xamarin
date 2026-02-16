namespace Brupper.AspNetCore.Models;

public class PagedResult
{
    /// <summary> The count of all the entities to be returned by the search (without paging). </summary>
    public int? Count { get; set; }

    /// <summary> The arguments to retrieve the next page of items. The client needs to prepend the URI of the table to this. </summary>
    public string? NextLink { get; set; }
}

/// <summary> The model class for the response to a paged request. </summary>
public class PagedResult<TEntity>(IEnumerable<TEntity>? items = null)
    : PagedResult
    where TEntity : class
{
    /// <summary> The list of entities to include in the response. </summary>
    public IEnumerable<TEntity> Items { get; set; } = items ?? Array.Empty<TEntity>();

}