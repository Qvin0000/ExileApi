using System;
using System.Diagnostics;
using System.Threading;
using Serilog;

namespace ExileCore.Shared.Helpers
{
    public struct PerformanceTimer : IDisposable
    {
        private readonly string DebugText;
        private readonly Action<string, TimeSpan> FinishedCallback;
        private readonly int TriggerMs;
        private readonly bool Log;
        public static bool IgnoreTimer = false;
        public static ILogger Logger;
        private readonly Stopwatch sw;

        public PerformanceTimer(string debugText, int triggerMs = 0, Action<string, TimeSpan> callback = null, bool log = true)
        {
            FinishedCallback = callback;
            DebugText = debugText;
            TriggerMs = triggerMs;
            Log = log;
            sw = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            StopAndPrint();
        }

        public void StopAndPrint()
        {
            if (!sw.IsRunning) return;
            sw.Stop();

            if (sw.ElapsedMilliseconds >= TriggerMs && !IgnoreTimer)
            {
                var elapsed = sw.Elapsed;

                if (Log)
                {
                    Logger.Information(
                        $"PerfTimer =-> {DebugText} ({elapsed.TotalMilliseconds} ms) Thread #[{Thread.CurrentThread.ManagedThreadId}]");
                }

                FinishedCallback?.Invoke(DebugText, elapsed);
            }
        }
    }
}
