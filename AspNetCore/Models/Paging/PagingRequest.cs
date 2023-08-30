namespace Brupper.AspNetCore.Models;

public class PagingRequest
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
}

public class PagingRequest<T> : PagingRequest
{
    public T? Filter { get; set; }
}
