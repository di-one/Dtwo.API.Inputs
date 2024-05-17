using System.Diagnostics;

namespace Dtwo.API.Inputs
{
    /// <summary>
    /// Functions to send clicks
    /// </summary>
    public static class ClickSender
    {
        /// <summary>
        /// Send a complete click (down and up)
        /// </summary>
        /// <param name="process"></param>
        /// <param name="info"></param>
        /// <param name="msDelay"></param>
        /// <param name="onFinish"></param>
        /// <returns></returns>
        public async static Task SendClick(Process process, ClickInfo info, int msDelay = 50, Action? onFinish = null)
        {
            SendClickDown(process, info);
            await Task.Delay(msDelay);
            SendClickUp(process, info);

            onFinish?.Invoke();
        }

        /// <summary>
        /// Send a click down
        /// </summary>
        /// <param name="process"></param>
        /// <param name="info"></param>
        public static void SendClickDown(Process process, ClickInfo info)
        {
            if (info.IsrightClick)
            {
                SendClick(process.MainWindowHandle, info.PosX, info.PosY, PInvoke.WM_RBUTTONDOWN);
            }
            else
            {
                SendClick(process.MainWindowHandle, info.PosX, info.PosY, PInvoke.WM_LBUTTONDOWN);
            }
        }

        /// <summary>
        /// Send a click up
        /// </summary>
        /// <param name="process"></param>
        /// <param name="info"></param>
        public static void SendClickUp(Process process, ClickInfo info)
        {
            if (info.IsrightClick)
            {
                SendClick(process.MainWindowHandle, info.PosX, info.PosY, PInvoke.WM_RBUTTONUP);
            }
            else
            {
                SendClick(process.MainWindowHandle, info.PosX, info.PosY, PInvoke.WM_LBUTTONUP);
            }
        }

        private static void SendClick(IntPtr hwnd, int x, int y, uint key)
        {
            PInvoke.PostMessage(hwnd, key, 1, PInvoke.MakeLParam(x, y));
        }
    }
}
