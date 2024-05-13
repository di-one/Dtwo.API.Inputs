using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtwo.API.Inputs
{
    /// <summary>
    /// Get informations about the system windows
    /// </summary>
    public static class SystemWindowInfos
    {
        /// <summary>
        /// Get the handle of the focused window
        /// </summary>
        public static IntPtr FocusedWindowHwnd { get; private set; }

        private static DofusWindow? m_LastFocusedDofusWindow;

        /// <summary>
        /// Get the focused Dofus window (return null if no Dofus window is focused)
        /// </summary>
        public static DofusWindow? FocusedDofusWindow
        {
            get
            {
                if (FocusedWindowHwnd == IntPtr.Zero)
                    return null;

                if (m_LastFocusedDofusWindow == null)
                {
                    m_LastFocusedDofusWindow = FindDofusWindow();
                }

                else if (FocusedWindowHwnd != m_LastFocusedDofusWindow.WindowProcess.MainWindowHandle)
                {
                    m_LastFocusedDofusWindow = FindDofusWindow();
                }

                return m_LastFocusedDofusWindow;
            }
        }

        public static void SetFocusedWindow(IntPtr hwnd)
        {
            FocusedWindowHwnd = hwnd;
        }

        private static DofusWindow? FindDofusWindow()
        {
            var allDofusWindow = DofusWindow.WindowsList;
            foreach (var dofusWindow in allDofusWindow)
            {
                if (dofusWindow.WindowProcess.MainWindowHandle == FocusedWindowHwnd)
                {
                    return dofusWindow;
                }
            }

            return null;
        }
    }
}
