namespace Brupper.AspNetCore.Models;

public class CreateIfNotExistResult<TEntity>
{
    public TEntity Entity { get; set; } = default!;

    public bool AlreadyExists { get; set; }
}
