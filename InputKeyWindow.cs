using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtwo.API.Inputs
{
    /// <summary>
    /// Methods to listen to key inputs in a specific window
    /// </summary>
    public class InputKeyWindow : ThreadSafeSingleton<InputKeyWindow>
    {
        private class Listener
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


		/// <summary>
		/// Register a callback when a subscribed key is pressed in a window
		/// </summary>
		public event Action<DofusWindow, int> WindowKeyDown
		{
			add => m_windowKeyDown += value;
			remove => m_windowKeyDown -= value;
		}
		private event Action<DofusWindow, int>? m_windowKeyDown;

        /// <summary>
        /// Register a callback when a subscribed key is released in a window
        /// </summary>
        public event Action<DofusWindow, int> WindowKeyUp
        {
            add => m_windowKeyUp += value;
            remove => m_windowKeyUp -= value;
        }
		private event Action<DofusWindow, int>? m_windowKeyUp;

        private readonly Dictionary<IntPtr, Listener> m_registeredKeyUpCallbacks = new();
        private readonly Dictionary<IntPtr, Listener> m_registeredKeyDownCallbacks = new();

        private bool m_isInitialized = false;

        /// <summary>
        /// Initialize the InputKeyWindow
        /// </summary>
        public void Init()
        {
            if (m_isInitialized)
            {
                LogManager.LogWarning("InputKeyWindow.Init", "InputKeyWindow is already initialized");
                return;
            }

            InputKeyListener.Instance.KeyDown += OnKeyDown;
            InputKeyListener.Instance.KeyUp += OnKeyUp;

            m_isInitialized = true;
        }

        private void OnKeyDown(int key)
        {
            Listener? listener = null;
            var focused = SystemWindowInfos.FocusedDofusWindow;

            if (focused == null)
                return;

            if (m_registeredKeyDownCallbacks.TryGetValue(focused.WindowProcess.MainWindowHandle, out listener) == false)
            {
                return;
            }

            listener.CallAction(key);
            m_windowKeyDown?.Invoke(focused, key);
        }

        private void OnKeyUp(int key)
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
            m_windowKeyUp?.Invoke(focused, key);
        }


        /// <summary>
        /// Subscribe to a key up event
        /// </summary>
        /// <param name="window"></param>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        public void SubscribeKeyUp(DofusWindow window, int key, Action callback)
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

        /// <summary>
        /// Unsubscribe to a key up event
        /// </summary>
        /// <param name="window"></param>
        /// <param name="key"></param>
        public void UnsubscribeKeyUp(DofusWindow window, int key)
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

        /// <summary>
        /// Subscribe to a key down event
        /// </summary>
        /// <param name="window"></param>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        public void SubscribeKeyDown(DofusWindow window, int key, Action callback)
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

        /// <summary>
        /// Unsubscribe to a key down event
        /// </summary>
        /// <param name="window"></param>
        /// <param name="key"></param>
        public void UnsubscribeKeyDown(DofusWindow window, int key)
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
