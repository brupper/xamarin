using System;

namespace Brupper
{
    public sealed class SimpleStateHolder : IDisposable
    {
        private readonly Action? cleanUpAction;

        public bool IsOnState { get; private set; }

        public SimpleStateHolder(bool initialState = true, Action? cleanUpAction = null)
        {
            IsOnState = initialState;
            this.cleanUpAction = cleanUpAction;
        }
        public SimpleStateHolder(Action setUpAction, Action cleanUpAction)
        {
            setUpAction();
            this.cleanUpAction = cleanUpAction;
        }

        #region IDisposable implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                IsOnState = false;
                cleanUpAction?.Invoke();
            }
        }

        ~SimpleStateHolder()
        {
            Dispose(false);
        }

        #endregion
    }
}
