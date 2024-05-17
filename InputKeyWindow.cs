using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtwo.API.Inputs
{
    public static class InputKeyWindow
    {
        public class Listener
        {
            public DofusWindow DofusWindow;
            private Dictionary<int, Action> m_actions = new Dictionary<int, Action>();

            public Listener(DofusWindow dofusWindow)
            {
                DofusWindow = dofusWindow;
            }

            public bool AddInput(int key, Action action)
            {
                if (m_actions.ContainsKey(key))
                    return false;

                m_actions.Add(key, action);

                return true;
            }

            public bool RemoveInput(int key)
            {
                if (m_actions.ContainsKey(key) == false)
                    return false;

                m_actions.Remove(key);

                return true;
            }

            public void CallAction(int key)
            {
                if (m_actions.TryGetValue(key, out var action) == false)
                    return;

                action.Invoke();
            }
        }

        private static event Action<DofusWindow, int> m_onKeyDownInWindow;
        public static event Action<DofusWindow, int> OnKeyDownInWindow
        {
            add => m_onKeyDownInWindow += value;
            remove => m_onKeyDownInWindow -= value;
        }

        private static event Action<DofusWindow, int> m_onKeyUpInWindow;
        public static event Action<DofusWindow, int> OnKeyUpInWindow
        {
            add => m_onKeyUpInWindow += value;
            remove => m_onKeyUpInWindow -= value;
        }

        private static Dictionary<IntPtr, Listener> m_registeredKeyUpCallbacks = new Dictionary<IntPtr, Listener>();
        private static Dictionary<IntPtr, Listener> m_registeredKeyDownCallbacks = new Dictionary<IntPtr, Listener>();



        public static void Init()
        {
            InputKeyListener.Instance.KeyDown += OnKeyDown;
            InputKeyListener.Instance.KeyUp += OnKeyUp;
        }

        private static void OnKeyDown(int key)
        {
            Listener listener;
            var focused = SystemWindowInfos.FocusedDofusWindow;

            if (focused == null)
                return;

            if (m_registeredKeyDownCallbacks.TryGetValue(focused.WindowProcess.MainWindowHandle, out listener) == false)
            {
                return;
            }

            listener.CallAction(key);
            m_onKeyDownInWindow?.Invoke(focused, key);
        }

        private static void OnKeyUp(int key)
        {
            Listener listener;
            var focused = SystemWindowInfos.FocusedDofusWindow;

            if (focused == null)
                return;

            if (m_registeredKeyUpCallbacks.TryGetValue(focused.WindowProcess.MainWindowHandle, out listener) == false)
            {
                return;
            }

            listener.CallAction(key);
            m_onKeyUpInWindow?.Invoke(focused, key);
        }



        public static void SubscribeKeyUp(DofusWindow window, int key, Action callback)
        {
            var whnd = window.WindowProcess.MainWindowHandle;

            Listener listener;

            if (m_registeredKeyUpCallbacks.TryGetValue(whnd, out listener) == false)
            {
                listener = new Listener(window);
                m_registeredKeyDownCallbacks.Add(whnd, listener);
            }

            if (listener.AddInput(key, callback) == false)
            {
                return;
            }

            InputKeyListener.Instance.AddKey(key);
        }

        public static void UnsubscribeKeyUp(DofusWindow window, int key)
        {
            var whnd = window.WindowProcess.MainWindowHandle;

            Listener listener;
            if (m_registeredKeyDownCallbacks.TryGetValue(whnd, out listener) == false)
            {
                return;
            }

            if (listener.RemoveInput(key) == false)
                return;

            m_registeredKeyDownCallbacks.Remove(whnd);
        }

        public static void SubscribeKeyDown(DofusWindow window, int key, Action callback)
        {
            var whnd = window.WindowProcess.MainWindowHandle;

            Listener? listener;

            if (m_registeredKeyDownCallbacks.TryGetValue(whnd, out listener) == false)
            {
                listener = new Listener(window);
                m_registeredKeyDownCallbacks.Add(whnd, listener);
            }

            if (listener.AddInput(key, callback) == false)
            {
                return;
            }

            InputKeyListener.Instance.AddKey(key);
        }

        public static void UnsubscribeKeyDown(DofusWindow window, int key)
        {
            var whnd = window.WindowProcess.MainWindowHandle;

            Listener listener;
            if (m_registeredKeyDownCallbacks.TryGetValue(whnd, out listener) == false)
            {
                return;
            }

            if (listener.RemoveInput(key) == false)
                return;

            m_registeredKeyDownCallbacks.Remove(whnd);
        }
    }
}
