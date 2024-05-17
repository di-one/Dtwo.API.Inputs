using Dtwo.API.Inputs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Dtwo.API.Inputs.Extensions
{
    /// <summary>
    /// Extensions for DofusWindow
    /// </summary>
    public static class DofusWindowExtensions
    {
        /// <summary>
        /// Send a click (up and down) to the window
        /// </summary>
        /// <param name="window"></param>
        /// <param name="clickInfo"></param>
        /// <param name="msDelay"></param>
        /// <param name="onFinish"></param>
        /// <returns></returns>
        public static async Task SendClick(this DofusWindow window, ClickInfo clickInfo, int msDelay = 50, Action? onFinish = null)
        {
            if (window.WindowProcess == null)
            {
                // Error
                return;
            }

            await ClickSender.SendClick(window.WindowProcess, clickInfo, msDelay, onFinish);
        }

        /// <summary>
        /// Send a click (up and down) to the window at the specified position
        /// </summary>
        /// <param name="window"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="right"></param>
        /// <param name="msDelay"></param>
        /// <param name="onFinish"></param>
        /// <returns></returns>
        public static async Task SendClick(this DofusWindow window, int x, int y, bool right, int msDelay = 50, Action? onFinish = null)
        {
            await SendClick(window, x, y, right, msDelay, onFinish);
        }

        /// <summary>
        /// Send a click down to the window at the specified position
        /// </summary>
        /// <param name="window"></param>
        /// <param name="clickInfo"></param>
        public static void SendClickDown(this DofusWindow window, ClickInfo clickInfo)
        {
            if (window.WindowProcess == null)
            {
                // Error
                return;
            }

            ClickSender.SendClickDown(window.WindowProcess, clickInfo);
        }

        /// <summary>
        /// Send a click down to the window at the specified position
        /// </summary>
        /// <param name="window"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="right"></param>
        public static void SendClickDown(this DofusWindow window, int x, int y, bool right)
        {
            SendClickDown(window, new ClickInfo(x, y, right));
        }

        /// <summary>
        /// Send a click up to the window at the specified position
        /// </summary>
        /// <param name="window"></param>
        /// <param name="clickInfo"></param>
        public static void SendClickUp(this DofusWindow window, ClickInfo clickInfo)
        {
            if (window.WindowProcess == null)
            {
                // Error
                return;
            }

            ClickSender.SendClickUp(window.WindowProcess, clickInfo);
        }

        /// <summary>
        /// Send a click up to the window at the specified position
        /// </summary>
        /// <param name="window"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="right"></param>
        public static void SendClickUp(this DofusWindow window, int x, int y, bool right)
        {
            SendClickUp(window, new ClickInfo(x, y, right));
        }

        /// <summary>
        /// Send a keyboard key (up and down) to the window
        /// </summary>
        /// <param name="window"></param>
        /// <param name="input"></param>
        /// <param name="msDelay"></param>
        /// <param name="onFinish"></param>
        /// <returns></returns>
        public static async Task SendKey(this DofusWindow window, int input, int msDelay = 50, Action? onFinish = null)
        {
            if (window.WindowProcess == null)
            {
                // Error
                return;
            }

            await InputKeySender.SendKey(window.WindowProcess, input, msDelay, onFinish);
        }

        /// <summary>
        /// Send a keyboard key (up and down) to the window
        /// </summary>
        /// <param name="window"></param>
        /// <param name="input"></param>
        /// <param name="msDelay"></param>
        /// <param name="onFinish"></param>
        /// <returns></returns>
        public static async Task SendKey(this DofusWindow window, InputKey input, int msDelay = 50, Action? onFinish = null)
        {
            await SendKey(window, input.KeyId, msDelay, onFinish);
        }

        /// <summary>
        /// Send a keyboard key down to the window
        /// </summary>
        /// <param name="window"></param>
        /// <param name="input"></param>
        public static void SendKeyDown(this DofusWindow window, int input)
        {
            if (window.WindowProcess == null)
            {
                // Error
                return;
            }

            InputKeySender.SendKeyDown(window.WindowProcess, input);
        }

        /// <summary>
        /// Send a keyboard key down to the window
        /// </summary>
        /// <param name="window"></param>
        /// <param name="input"></param>
        public static void SendKeyDown(this DofusWindow window, InputKey input)
        {
            SendKeyDown(window, input.KeyId);
        }

        /// <summary>
        /// Send a keyboard key up to the window
        /// </summary>
        /// <param name="window"></param>
        /// <param name="input"></param>
        public static void SendKeyUp(this DofusWindow window, int input)
        {
            if (window.WindowProcess == null)
            {
                // Error
                return;
            }

            InputKeySender.SendKeyUp(window.WindowProcess, input);
        }

        /// <summary>
        /// Send a keyboard key up to the window
        /// </summary>
        /// <param name="window"></param>
        /// <param name="input"></param>
        public static void SendKeyUp(this DofusWindow window, InputKey input)
        {
            SendKeyUp(window, input.KeyId);
        }

        /// <summary>
        /// Send a char to the window
        /// </summary>
        /// <param name="window"></param>
        /// <param name="c"></param>
        public static void SendChar(this DofusWindow window, char c)
        {
            if (window.WindowProcess == null)
            {
                // Error
                return;
            }

            InputKeySender.SendChar(window.WindowProcess, c);
        }

        /// <summary>
        /// Subscribe to a key down event for the window with the specified key
        /// </summary>
        /// <param name="window"></param>
        /// <param name="key"></param>
        /// <param name="action"></param>
        public static void SubscribeOnKeyDown(this DofusWindow window, int key, Action action)
        {
            InputKeyWindow.Instance.SubscribeKeyDown(window, key, action);
        }


        /// <summary>
        /// Unsubscribe to a key down event for the window with the specified key
        /// </summary>
        /// <param name="window"></param>
        /// <param name="key"></param>
        public static void UnsubscribeOnKeyDown(this DofusWindow window, int key)
        {
            InputKeyWindow.Instance.UnsubscribeKeyDown(window, key);
        }

        /// <summary>
        /// Subscribe to a key up event for the window with the specified key
        /// </summary>
        /// <param name="window"></param>
        /// <param name="key"></param>
        /// <param name="action"></param>
        public static void SubscribeOnKeyUp(this DofusWindow window, int key, Action action)
        {
            InputKeyWindow.Instance.SubscribeKeyUp(window, key, action);
        }

        /// <summary>
        /// Unsubscribe to a key up event for the window with the specified key
        /// </summary>
        /// <param name="window"></param>
        /// <param name="key"></param>
        public static void UnsubscribeOnKeyUp(this DofusWindow window, int key)
        {
            InputKeyWindow.Instance.UnsubscribeKeyUp(window, key);
        }


        /// <summary>
        /// Get the relative mouse position to the window
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public static (int, int) GetRelativeMousePosition(this DofusWindow window)
        {
            PInvoke.POINT pos;
            PInvoke.GetCursorPos(out pos);
            PInvoke.ScreenToClient(PInvoke.GetForegroundWindow(), ref pos);

            return (pos.X, pos.Y);
        }

        /// <summary>
        ///  Get if the window is focused
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public static bool IsFocused(this DofusWindow window)
        {
            return SystemWindowInfos.FocusedDofusWindow == window;
        }

        /// <summary>
        /// Focus the window
        /// </summary>
        /// <param name="window"></param>
        public static void Focus(this DofusWindow window)
        {
            PInvoke.FocusProcess(window.WindowProcess);
        }

        /// <summary>
        /// Maximize the window
        /// </summary>
        /// <param name="window"></param>
        public static void Maximize(this DofusWindow window)
        {
            var hwm = window.WindowProcess.MainWindowHandle;
            PInvoke.ShowWindow(hwm, 3);
        }

        /// <summary>
        /// Set the position of the window
        /// </summary>
        /// <param name="window"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void SetPosition(this DofusWindow window, int x, int y)
        {
            PInvoke.RECT baseRect = new PInvoke.RECT();
            PInvoke.GetWindowRect(window.WindowProcess.MainWindowHandle, ref baseRect);

            var rect = new PInvoke.RECT(x, y, baseRect.Width, baseRect.Height);
            SetPosition(window.WindowProcess, rect);
        }

        /// <summary>
        /// Set the size of the window
        /// </summary>
        /// <param name="window"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void SetSize(this DofusWindow window, int width, int height)
        {
            PInvoke.RECT baseRect = new PInvoke.RECT();
            PInvoke.GetWindowRect(window.WindowProcess.MainWindowHandle, ref baseRect);

            var rect = new PInvoke.RECT(baseRect.X, baseRect.Y, width, height);
            SetPosition(window.WindowProcess, rect);
        }

        /// <summary>
        /// Set the size and position of the window
        /// </summary>
        /// <param name="window"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void SetSizeAndPosition(this DofusWindow window, int x, int y, int width, int height)
        {
            PInvoke.RECT rect = new PInvoke.RECT()
            {
                X = x,
                Y = y,
                Width = width,
                Height = height
            };

            SetPosition(window.WindowProcess, rect);
        }

        /// <summary>
        /// Set the position of the window
        /// </summary>
        /// <param name="process"></param>
        /// <param name="rect"></param>
        private static void SetPosition(Process process, PInvoke.RECT rect)
        {
            PInvoke.SetWindowPos((int)process.MainWindowHandle, 0, rect.Left, rect.Top, rect.Right, rect.Bottom, 0);
        }

        /// <summary>
        /// Get if the window is minimized
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public static bool IsMinimized(this DofusWindow window)
        {
            var hwm = window.WindowProcess.MainWindowHandle;

            return PInvoke.IsIconic(hwm);
        }
    }
}
