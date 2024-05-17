using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtwo.API.Inputs
{
    public static class ClickSender
    {
        public async static Task SendClick(Process process, ClickInfo info, int msDelay = 50, Action? onFinish = null)
        {
            SendClickDown(process, info);
            await Task.Delay(msDelay);
            SendClickUp(process, info);

            onFinish?.Invoke();
        }

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
