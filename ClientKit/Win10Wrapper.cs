#if WINDOWS_UWP
using System;

namespace OSVR.ClientKit
{
    public class SafeHandleZeroOrMinusOneIsInvalid : IDisposable
    {
        public bool IsClosed;
        public bool IsInvalid;

        public SafeHandleZeroOrMinusOneIsInvalid(bool value) { }

        public void Close() { }
        public void Dispose() { }
        protected virtual bool ReleaseHandle() { return true; }
    }

    public class ApplicationException : Exception
    {
        public ApplicationException(string message)
            : base(message) { }
    }
}
#endif