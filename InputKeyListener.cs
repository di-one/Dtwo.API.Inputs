using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Dtwo.API.Inputs.PInvoke;

namespace Dtwo.API.Inputs
{
    /// <summary>
    /// Listens to keyboard and mouse inputs and triggers events when a watched key is pressed or released.
    /// </summary>
    public class InputKeyListener
    {
        private static InputKeyListener? m_instance;
        public static InputKeyListener Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new InputKeyListener();
                }

                return m_instance;
            }
        }

        private IntPtr m_keyboardHookID = IntPtr.Zero;
        private IntPtr m_mouseHookID = IntPtr.Zero;
        private PInvoke.LowLevelKeyboardProc? m_keyboardProc;
        private PInvoke.LowLevelMouseProc? m_mouseProc;

        public bool IsStarted { get; private set; }

        private event Action<int>? m_keyDown;
        /// <summary>
        /// Called when a watched key is pressed.
        /// </summary>
        public event Action<int> KeyDown
        {
            add => m_keyDown += value;
            remove => m_keyDown -= value;
        }

        private event Action<int>? m_keyUp;
        /// <summary>
        /// Called when a watched key is released.
        /// 
        public event Action<int> KeyUp
        {
            add => m_keyUp += value;
            remove => m_keyUp -= value;
        }

        private ConcurrentDictionary<int, int> m_watchedKeys = new ConcurrentDictionary<int, int>();

        /// <summary>
        /// Adds a key to the list of watched keys.
        /// </summary>
        /// <param name="key"></param>
        public void AddKey(int key)
        {
            if (m_watchedKeys.ContainsKey(key))
            {
                m_watchedKeys[key]++;
            }
            else
            {
                m_watchedKeys.TryAdd(key, 1);
            }
        }

        /// <summary>
        /// Removes a key from the list of watched keys.
        /// </summary>
        /// <param name="key"></param>
        public void RemoveKey(int key)
        {
            if (m_watchedKeys.ContainsKey(key) == false)
                return;

            m_watchedKeys[key]--;
            if (m_watchedKeys[key] == 0)
            {
                m_watchedKeys.TryRemove(key, out int val);
            }
        }

        /// <summary>
        /// Starts listening to keyboard and mouse inputs.
        /// </summary>
        public void StartListen()
        {
            if (IsStarted)
                return;

            m_keyboardProc = KeyboardHookCallback;
            m_mouseProc = MouseHookCallback;

            m_keyboardHookID = SetKeyboardHook(m_keyboardProc);
            m_mouseHookID = SetMouseHook(m_mouseProc);


            Debug.WriteLine("HookID: " + m_keyboardHookID);

            if (m_keyboardHookID == IntPtr.Zero)
            {
                int errorCode = Marshal.GetLastWin32Error();
                Debug.WriteLine("Failed to install hook: " + errorCode);
            }

            IsStarted = true;
        }

        /// <summary>
        /// Stops listening to keyboard and mouse inputs.
        /// </summary>
        public void Stop()
        {
            if (IsStarted == false)
                return;

            PInvoke.UnhookWindowsHookEx(m_keyboardHookID);
        }

        private IntPtr SetKeyboardHook(PInvoke.LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule? curModule = curProcess.MainModule)
            {
                return PInvoke.SetWindowsHookEx(PInvoke.WH_KEYBOARD_LL, proc, PInvoke.GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private static IntPtr SetMouseHook(PInvoke.LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return PInvoke.SetWindowsHookEx(PInvoke.WH_MOUSE_LL, proc, PInvoke.GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                var key = vkCode;

                if (m_watchedKeys.ContainsKey(key))
                {
                    bool isKeyDown = wParam == (IntPtr)PInvoke.WM_KEYDOWN || wParam == (IntPtr)PInvoke.WM_SYSKEYDOWN;
                    bool isKeyUp = wParam == (IntPtr)PInvoke.WM_KEYUP || wParam == (IntPtr)PInvoke.WM_SYSKEYUP;

                    if (isKeyDown)
                    {
                        m_keyDown?.Invoke(key);
                    }
                    else if (isKeyUp)
                    {
                        m_keyUp?.Invoke(key);
                    }
                }
            }
            return PInvoke.CallNextHookEx(m_keyboardHookID, nCode, wParam, lParam);
        }

        private IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                MSLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
                int keyCode = MouseMessageToVirtualKeyCode(wParam, hookStruct, out bool isDown);

                if (keyCode != -1)
                {
                    if (isDown)
                    {
                        m_keyDown?.Invoke(keyCode);
                    }
                    else
                    {
                        m_keyUp?.Invoke(keyCode);
                    }
                }
            }
            return PInvoke.CallNextHookEx(m_mouseHookID, nCode, wParam, lParam);
        }

        public int MouseMessageToVirtualKeyCode(IntPtr wParam, MSLLHOOKSTRUCT hookStruct, out bool isDown)
        {
            int message = wParam.ToInt32();
            isDown = false; // Default to false, set to true if it's a "down" event

            switch (message)
            {
                case (int)PInvoke.WM_LBUTTONDOWN:
                    isDown = true;
                    return 0x01; // VK_LBUTTON
                case (int)PInvoke.WM_LBUTTONUP:
                    return 0x01; // VK_LBUTTON

                case (int)PInvoke.WM_RBUTTONDOWN:
                    isDown = true;
                    return 0x02; // VK_RBUTTON
                case (int)PInvoke.WM_RBUTTONUP:
                    return 0x02; // VK_RBUTTON

                case (int)PInvoke.WM_MBUTTONDOWN:
                    isDown = true;
                    return 0x04; // VK_MBUTTON
                case (int)PInvoke.WM_MBUTTONUP:
                    return 0x04; // VK_MBUTTON

                case (int)PInvoke.WM_XBUTTONDOWN:
                    isDown = true;
                    return DetermineXButton(hookStruct); // Determine which XBUTTON

                case (int)PInvoke.WM_XBUTTONUP:
                    return DetermineXButton(hookStruct); // Determine which XBUTTON

                default:
                    return -1; // No corresponding virtual key code or not a mouse button event
            }
        }

        private static int DetermineXButton(MSLLHOOKSTRUCT hookStruct)
        {
            if ((hookStruct.mouseData >> 16) == 0x0001)
                return 0x05; // VK_XBUTTON1
            else if ((hookStruct.mouseData >> 16) == 0x0002)
                return 0x06; // VK_XBUTTON2
            return -1; // Just in case
        }
    }
}
