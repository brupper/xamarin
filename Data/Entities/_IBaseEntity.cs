using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Brupper.Data.Entities
{
    public interface IBaseEntity : INotifyPropertyChanged
    {
        string Id { get; set; }
    }

    public class BaseEntity : IBaseEntity
    {
        public string Id { get; set; }

        #region INotifyPropertyChanged

        private static readonly PropertyChangedEventArgs AllPropertiesChanged = new PropertyChangedEventArgs(string.Empty);


        public virtual void RaisePropertyChanged([CallerMemberName] string whichProperty = "")
        {
            var changedArgs = new PropertyChangedEventArgs(whichProperty);
            RaisePropertyChanged(changedArgs);
        }

        public virtual void RaiseAllPropertiesChanged()
        {
            RaisePropertyChanged(AllPropertiesChanged);
        }

        public virtual void RaisePropertyChanged(PropertyChangedEventArgs changedArgs)
        {
            PropertyChanged?.Invoke(this, changedArgs);
        }

        protected virtual void SetProperty<T>(ref T storage, T value, Action<bool> action, [CallerMemberName] string propertyName = null)
        {
            if (action == null)
            {
                throw new ArgumentException($"{nameof(action)} should not be null", nameof(action));
            }

            action.Invoke(SetProperty(ref storage, value, propertyName));
        }

        protected virtual bool SetProperty<T>(ref T storage, T value, Action afterAction, [CallerMemberName] string propertyName = null)
        {
            if (SetProperty(ref storage, value, propertyName))
            {
                afterAction?.Invoke();
                return true;
            }

            return false;
        }

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            RaisePropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #endregion
    }
}
