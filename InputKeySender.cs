using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtwo.API.Inputs
{
    /// <summary>
    /// Send keys to a process
    /// </summary>
    public class InputKeySender
    {
        public async static Task SendKey(Process process, int keyCode, int msDelay = 50, Action? onFinish = null)
        {
            SendKeyDown(process, keyCode);
            await Task.Delay(msDelay);
            SendKeyUp(process, keyCode);

            onFinish?.Invoke();
        }

        /// <summary>
        /// Send a char to a process
        /// </summary>
        /// <param name="process"></param>
        /// <param name="c"></param>
        public static void SendChar(Process process, char c)
        {
            PInvoke.PostMessage(process.MainWindowHandle, PInvoke.WM_CHAR, c, 0);
        }

        /// <summary>
        /// Send a key down to a process
        /// </summary>
        /// <param name="process"></param>
        /// <param name="keyCode"></param>
        public static void SendKeyDown(Process process, int keyCode)
        {
            SendKey(process.MainWindowHandle, keyCode, PInvoke.WM_KEYDOWN);
        }

        /// <summary>
        /// Send a key up to a process
        /// </summary>
        /// <param name="process"></param>
        /// <param name="keyCode"></param>
        public static void SendKeyUp(Process process, int keyCode)
        {
            SendKey(process.MainWindowHandle, keyCode, PInvoke.WM_KEYUP);
        }

        private static void SendKey(IntPtr hwnd, int keyCode, uint msg)
        {
            PInvoke.PostMessage(hwnd, msg, keyCode, 0);
        }
    }
}
