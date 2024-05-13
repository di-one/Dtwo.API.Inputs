using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtwo.API.Inputs
{
    public class WindowsEventsMessageDispatcher
    {
        private bool m_isStarted;
        private bool m_needStop;

        public void Start(Action threadCallbacks, Action stopCallbacks)
        {
            if (m_isStarted)
            {
                LogManager.LogWarning(
                            $"{nameof(WindowsEventsMessageDispatcher)}.{nameof(Start)}",
                            "WinEventMessageDispatcher already started", 1);
                return;
            }

            m_isStarted = true;

            Update(threadCallbacks, stopCallbacks);
        }

        public void Stop()
        {
            m_needStop = true;
        }

        private void Update(Action threadCallbacks, Action stopCallbacks)
        {
            PInvoke.MSG msg = new PInvoke.MSG();

            Task.Factory.StartNew(() =>
            {
                try
                {
                    threadCallbacks?.Invoke();

                    while (m_isStarted && PInvoke.GetMessage(ref msg, IntPtr.Zero, 0, 0))
                    {
                        if (m_needStop)
                        {
                            stopCallbacks.Invoke();
                            m_isStarted = false;
                            return;
                        }
                        //if (check_any_message_you_need == msg.message)
                        // you can for example check if Enter has been pressed to emulate Console.Readline();
                        PInvoke.TranslateMessage(ref msg);
                        PInvoke.DispatchMessage(ref msg);
                    }
                }
                catch (Exception ex)
                {
                    LogManager.LogError(ex.ToString(), 1);
                }
            }, TaskCreationOptions.LongRunning);

        }
    }
}
