using System;

namespace pCloud
{
    internal class Disposable : IDisposable
    {
        private readonly Action action;

        public Disposable(Action action)
        {
            this.action = action;
        }

        void IDisposable.Dispose()
        {
            this.action();
        }
    }
}
