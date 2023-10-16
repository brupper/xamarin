namespace Brupper.Logic
{
    [System.Diagnostics.DebuggerDisplay("{Item,nq} - {Order,nq} - {Level,nq}")]
    public class TreeViewModel<TViewModel>
        where TViewModel : class
    {
        public int Order { get; set; } = default!;

        public Tree<TViewModel> Item { get; set; } = default!;
    }
}
