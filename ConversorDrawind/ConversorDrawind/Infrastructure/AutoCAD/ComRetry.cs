using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace ConversorDrawind
{
    internal static class Class_ComRetry
    {
        private const int RPC_E_CALL_REJECTED = unchecked((int)0x80010001);
        private const int RPC_E_SERVERCALL_RETRYLATER = unchecked((int)0x8001010A);

        public static void Invoke(Action action, int maxRetries = 60, int retryDelayMs = 100)
        {
            Invoke<object>(() =>
            {
                action();
                return null;
            }, maxRetries, retryDelayMs);
        }

        public static T Invoke<T>(Func<T> action, int maxRetries = 60, int retryDelayMs = 100)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            int attempt = 0;

            while (true)
            {
                try
                {
                    return action();
                }
                catch (COMException ex)
                {
                    if (!IsTransientCom(ex) || attempt >= maxRetries)
                        throw;

                    attempt++;
                    Thread.Sleep(retryDelayMs);
                }
            }
        }

        public static bool IsTransientCom(COMException ex)
        {
            if (ex == null)
                return false;

            return ex.ErrorCode == RPC_E_CALL_REJECTED ||
                   ex.ErrorCode == RPC_E_SERVERCALL_RETRYLATER;
        }
    }
}

