using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtwo.API.Inputs
{
    // Cette classe permet de globalement de capturer les événements liés aux fenêtre Windows
    // Dans notre cas, notre utilité est moindre, nous nous en servons uniquement pour détecter la fenêtre focus
    // Cela est déjà possible avec GetForegroundWindow(), et nous pourrions l'utiliser, mais nous ne l'utilsons pas car :
    // Dans certains cas, nous avons besoin de détecter des événements qui ont été émis dans une des fenêtres du jeu,
    //     prenons l'exemple d'inputs, si nous voulons détecter les inputs, nous devons être sûr que la fenêtre du jeu est focus
    //     dans le cas d'inputs c'est des actions répétés (imaginons quelqu'un qui écrit un mail en même temps que l'application tourne ...)
    //     utiliser GetForegroundWindow() a chaque inputs semlblerait trop grourmand en performance, nous préferons donc utiliser les événements
    //     de Windows pour détecter la fenêtre focus.

    // Il faudrait effectuer un benchmark, ou une étude de performance pour savoir si c'est vraiment plus performant
    // Dans le cas où cette méthode est interessante, nous pourrions migrer la plus part des détections d'inputs vers cette méthode
    //     et ainsi faire une détection en one shot (sans détection de la fenêtre avec ForeGround() au préalable)
    public static class WindowsEventsListener
    {
        public static bool IsStarted { get; private set; }

        private static event Action<IntPtr>? m_onWindowFocused;
        public static event Action<IntPtr> OnWindowFocused
        {
            add => m_onWindowFocused += value;
            remove => m_onWindowFocused -= value;
        }

        private static IntPtr m_hweHook = IntPtr.Zero;
        private static WindowsEventsMessageDispatcher? m_dispatcher;
        private static PInvoke.WinEventDelegate? m_focusCallback;

        public static void StartListen()
        {
            try
            {
                if (IsStarted)
                {
                    LogManager.LogWarning(
                            $"{nameof(WindowsEventsListener)}.{nameof(StartListen)}",
                            "WindowEventListener already started", 1);
                    return;
                }

                IsStarted = true;

                m_dispatcher = new WindowsEventsMessageDispatcher();
                m_dispatcher.Start(
                () => // start
                {
                    // Event qui sera appelé par le systeme Windows lorsqu'une fenêtre est focus
                    m_focusCallback = new PInvoke.WinEventDelegate(OnWindowFocus);

                    // On s'abonne à l'event en définissant la plage d'event que l'on veut écouter (EVENT_SYSTEM_FOREGROUND)
                    m_hweHook = PInvoke.SetWinEventHook(
                        PInvoke.WinEvents.EVENT_SYSTEM_FOREGROUND, PInvoke.WinEvents.EVENT_SYSTEM_FOREGROUND,
                        IntPtr.Zero, m_focusCallback, 0, 0,
                        PInvoke.WinEventFlags.WINEVENT_OUTOFCONTEXT);
                },
                () => // stop
                {
                    if (m_hweHook != IntPtr.Zero)
                    {
                        if (PInvoke.UnhookWinEvent(m_hweHook) == false)
                        {
                            LogManager.LogWarning(
                            $"{nameof(WindowsEventsListener)}.{nameof(StartListen)}",
                            "Unhook window event fail", 1);
                        }

                        m_hweHook = IntPtr.Zero;
                    }
                });
            }
            catch (Exception ex)
            {
                LogManager.LogError(
                     $"{nameof(WindowsEventsListener)}.{nameof(StartListen)}",
                     ex.ToString(), 1);
            }
        }

        public static void Stop()
        {
            try
            {
                if (IsStarted)
                {
                    IsStarted = false;

                    if (m_dispatcher != null)
                    {
                        m_dispatcher.Stop();
                        m_dispatcher = null;
                    }

                    m_onWindowFocused = null;
                    m_focusCallback = null;
                }
            }
            catch (Exception ex)
            {
                LogManager.LogError(ex.ToString(), 1);
            }
        }

        private static void OnWindowFocus(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            SystemWindowInfos.SetFocusedWindow(hwnd);
            m_onWindowFocused?.Invoke(hwnd);
        }
    }
}
