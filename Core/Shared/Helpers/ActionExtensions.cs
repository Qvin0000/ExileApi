using System;

namespace ExileCore.Shared.Helpers
{
    public static class ActionExtensions
    {
        public static void SafeTryInvoke(this Action action)
        {
            if (action != null)
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    DebugWindow.LogError(e.ToString());
                }
            }
        }

        public static void SafeTryInvoke<T>(this Action<T> action, T arg)
        {
            if (action != null)
            {
                try
                {
                    action(arg);
                }
                catch (Exception e)
                {
                    DebugWindow.LogError(e.ToString());
                }
            }
        }

        public static void SafeTryInvoke<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            if (action != null)
            {
                try
                {
                    action(arg1, arg2);
                }
                catch (Exception e)
                {
                    DebugWindow.LogError(e.ToString());
                }
            }
        }

        public static void SafeTryInvoke<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
        {
            if (action != null)
            {
                try
                {
                    action(arg1, arg2, arg3);
                }
                catch (Exception e)
                {
                    DebugWindow.LogError(e.ToString());
                }
            }
        }
    }
}