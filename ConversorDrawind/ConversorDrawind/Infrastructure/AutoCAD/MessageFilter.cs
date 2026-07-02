using System;
using System.Runtime.InteropServices;

namespace ConversorDrawind
{
    /// <summary>
    /// http://msdn.microsoft.com/pt-br/library/ms228772.aspx
    /// </summary>
    public class MessageFilter : IOleMessageFilter
    {
        private const int SERVERCALL_RETRYLATER = 2;
        private const int RETRY_TIMEOUT_MS = 15000;

        [ThreadStatic]
        private static int _registerCounter;

        //
        // Class containing the IOleMessageFilter
        // thread error-handling functions.

        // Start the filter.
        public static void Register()
        {
            if (_registerCounter == 0)
            {
                IOleMessageFilter newFilter = new MessageFilter();
                IOleMessageFilter oldFilter = null;
                CoRegisterMessageFilter(newFilter, out oldFilter);
            }

            _registerCounter++;
        }

        public static IDisposable ScopedRegistration()
        {
            Register();
            return new MessageFilterScope();
        }

        // Done with the filter, close it.
        public static void Revoke()
        {
            if (_registerCounter <= 0)
                return;

            _registerCounter--;

            if (_registerCounter == 0)
            {
                IOleMessageFilter oldFilter = null;
                CoRegisterMessageFilter(null, out oldFilter);
            }
        }

        //
        // IOleMessageFilter functions.
        // Handle incoming thread requests.
        int IOleMessageFilter.HandleInComingCall(int dwCallType, 
          System.IntPtr hTaskCaller, int dwTickCount, System.IntPtr 
          lpInterfaceInfo) 
        {
            //Return the flag SERVERCALL_ISHANDLED.
            return 0;
        }

        // Thread call was rejected, so try again.
        int IOleMessageFilter.RetryRejectedCall(System.IntPtr 
          hTaskCallee, int dwTickCount, int dwRejectType)
        {
            if (dwRejectType == SERVERCALL_RETRYLATER)
            {
                if (dwTickCount < RETRY_TIMEOUT_MS)
                {
                    // Wait 100 ms and retry while AutoCAD is busy.
                    return 100;
                }
            }

            return -1;
        }

        int IOleMessageFilter.MessagePending(System.IntPtr hTaskCallee, 
          int dwTickCount, int dwPendingType)
        {
            //Return the flag PENDINGMSG_WAITDEFPROCESS.
            return 2; 
        }

        // Implement the IOleMessageFilter interface.
        [DllImport("Ole32.dll")]
        private static extern int 
          CoRegisterMessageFilter(IOleMessageFilter newFilter, out 
          IOleMessageFilter oldFilter);

        private sealed class MessageFilterScope : IDisposable
        {
            private bool _disposed;

            public void Dispose()
            {
                if (_disposed)
                    return;

                _disposed = true;
                Revoke();
            }
        }
    }

    [ComImport(), Guid("00000016-0000-0000-C000-000000000046"), 
    InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    interface IOleMessageFilter 
    {
        [PreserveSig]
        int HandleInComingCall( 
            int dwCallType, 
            IntPtr hTaskCaller, 
            int dwTickCount, 
            IntPtr lpInterfaceInfo);

        [PreserveSig]
        int RetryRejectedCall( 
            IntPtr hTaskCallee, 
            int dwTickCount,
            int dwRejectType);

        [PreserveSig]
        int MessagePending( 
            IntPtr hTaskCallee, 
            int dwTickCount,
            int dwPendingType);
    }
}


