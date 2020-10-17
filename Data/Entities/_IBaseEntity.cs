using System.ComponentModel;

namespace Brupper.Data.Entities
{
    public interface IBaseEntity : INotifyPropertyChanged
    {
        string Id { get; set; }
    }
}
