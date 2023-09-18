namespace Brupper.AspNetCore.Models;

public class KeyValueListViewModel : ListViewModel<KeyValuePair<string, string>>
{
    public virtual string TitleLeft { get; set; } = default!;
    public virtual string TitleRight { get; set; } = default!;
}
