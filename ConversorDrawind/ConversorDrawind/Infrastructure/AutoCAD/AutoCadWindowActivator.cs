using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace ConversorDrawind
{
    internal static class AutoCadWindowActivator
    {
        private const int VkEnter = 0x0D;
        private const uint KeyEventFlagKeyUp = 0x0002;

        internal static void CancelPendingInput(int applicationWindowHandle)
        {
            int processId;
            GetWindowThreadProcessId(applicationWindowHandle, out processId);

            Process process = Process.GetProcessById(processId);
            IntPtr windowHandle = FindWindowInProcess(
                process,
                title => title.EndsWith("acad", StringComparison.OrdinalIgnoreCase));

            if (windowHandle == IntPtr.Zero)
                return;

            SetForegroundWindow(windowHandle);
            Thread.Sleep(100);
            KeybdEvent(VkEnter, 0, 0, UIntPtr.Zero);
            KeybdEvent(VkEnter, 0, KeyEventFlagKeyUp, UIntPtr.Zero);
        }

        private static IntPtr FindWindowInProcess(Process process, Func<string, bool> compareTitle)
        {
            foreach (ProcessThread thread in process.Threads)
            {
                IntPtr windowHandle = FindWindowInThread(thread.Id, compareTitle);
                if (windowHandle != IntPtr.Zero)
                    return windowHandle;
            }

            return IntPtr.Zero;
        }

        private static IntPtr FindWindowInThread(int threadId, Func<string, bool> compareTitle)
        {
            IntPtr windowHandle = IntPtr.Zero;
            EnumThreadWindows(threadId, (handle, _) =>
            {
                StringBuilder title = new StringBuilder(200);
                GetWindowText(handle, title, title.Capacity);

                if (!compareTitle(title.ToString()))
                    return true;

                windowHandle = handle;
                return false;
            }, IntPtr.Zero);

            return windowHandle;
        }

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern void KeybdEvent(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern int GetWindowThreadProcessId(int handle, out int processId);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumThreadWindows(int threadId, EnumWindowsProc callback, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int maxCount);
    }
}
