using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Brupper.Core.Models
{
    public abstract class NotifyPropertyChanged : INotifyPropertyChanged
    {
        private static readonly PropertyChangedEventArgs AllPropertiesChanged = new PropertyChangedEventArgs(string.Empty);


        public virtual void OnPropertyChanged([CallerMemberName] string whichProperty = "")
        {
            var changedArgs = new PropertyChangedEventArgs(whichProperty);
            OnPropertyChanged(changedArgs);
        }

        public virtual void RaiseAllPropertiesChanged()
        {
            OnPropertyChanged(AllPropertiesChanged);
        }

        public virtual void OnPropertyChanged(PropertyChangedEventArgs changedArgs)
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
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }

}
