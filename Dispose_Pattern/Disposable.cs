using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispose_Pattern
{
    class Disposable : IDisposable
    {
        private bool _IsDisposed;

        public void Fun()
        {
            if (_IsDisposed)
                throw new ObjectDisposedException("Cannot use a disposed object");

            //Do some fun 
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                if (!_IsDisposed)
                {
                    //Release managed resources
                }
            }
            //Release Native resources
            _IsDisposed = true;
        }

        ~Disposable()
        {
            Dispose(false);
        }
    }
}
