namespace Brupper.AspNetCore.Models;

public class ListViewModel<TViewModel>
{
    public virtual List<TViewModel> Items { get; set; }
        = new List<TViewModel>();
}
