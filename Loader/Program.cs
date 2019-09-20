using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using Serilog;
using SharpDX;
using SharpDX.Windows;

namespace Loader
{
    internal class Program
    {
        public static void Main(string[] a)
        {
            ILogger logger = null;
            var sw = Stopwatch.StartNew();
            var stringWith15MinusChars = new string('-', 15);
            try
            {
                var CoreDll = Assembly.Load("ExileCore");
                var CommandExecutorType = CoreDll.GetType("ExileCore.CommandExecutor");
                var ExecuteMethod = CommandExecutorType.GetMethod("Execute");
                if (a.Length > 0)
                {
                    var arg = a.Aggregate((s, s1) => $"{s.ToLower()} {s1.ToLower()}");
                    ExecuteMethod.Invoke(null, new object[] {arg});
                    return;
                }
                

                var currentProcess = Process.GetCurrentProcess();
                var processes = Process.GetProcessesByName(currentProcess.ProcessName);

                if (processes.Length > 1)
                {
                    var msgBoxResult = MessageBox.Show("Kill already running HUD process? (program configs will not be saved)",
                        "Hud process is already running",
                        MessageBoxButtons.OKCancel);

                    if (msgBoxResult == DialogResult.OK)
                    {
                        foreach (var process in processes)
                        {
                            if (process.Id != currentProcess.Id)
                            {
                                process.Kill();
                            }
                        }
                    }
                    else if (msgBoxResult == DialogResult.Cancel)
                    {
                        Application.Exit();
                        return;
                    }
                }

                using (var form = new AppForm())
                {
                   
                    var coreType = CoreDll.GetType("ExileCore.Core", true, true);
                    if (coreType == null) throw new NullReferenceException("Core not found.");
                    var loggerType = CoreDll.GetType("ExileCore.Logger", true, true);
                    var propertyInfo = loggerType.GetProperty("Log");

                    if (propertyInfo != null)
                    {
                        logger = propertyInfo.GetValue(null) as ILogger;
                        if (logger == null) throw new NullReferenceException("Logger cant be null.");

                        var perfomanceTimerType = CoreDll.GetType("ExileCore.Shared.Helpers.PerformanceTimer");

                        if (perfomanceTimerType != null)
                        {
                            var propertyPerfomanceTimerLogger = perfomanceTimerType.GetProperty("Logger");
                            if (propertyPerfomanceTimerLogger != null) propertyPerfomanceTimerLogger.SetValue(null, logger);
                        }

                        logger.Information(
                            $"{stringWith15MinusChars} Start hud at {DateTime.Now} {stringWith15MinusChars}");
                    }
                    else
                        throw new NullReferenceException("Not found Log property in Logger class.");

                    var coreLogger = coreType.GetProperty("Logger");

                    if (coreLogger != null)
                        coreLogger.SetValue(null, logger);

                    var performanceTimerType = CoreDll.GetType("ExileCore.Shared.Helpers.PerformanceTimer", true, true);
                    performanceTimerType.GetField("Logger").SetValue(null, logger);
                    var methodPerfomanceTimerDispose = performanceTimerType.GetMethod("Dispose");

                    var instanceCreateNewOffsets =
                        Activator.CreateInstance(performanceTimerType, "Create new offsets", 0, null, true);

                    ExecuteMethod.Invoke(null, new object[] {"loader_offsets"});
                    methodPerfomanceTimerDispose.Invoke(instanceCreateNewOffsets, null);
                    var instanceFormLoad = Activator.CreateInstance(performanceTimerType, "Form Load", 0, null, true);
                    methodPerfomanceTimerDispose.Invoke(instanceFormLoad, null);
                    var instanceCore = Activator.CreateInstance(coreType, form);
                    var coreDispose = coreType.GetMethod("Dispose");
                    var DebugWindowType = CoreDll.GetType("ExileCore.DebugWindow");

                    var methodLogMsg =
                        DebugWindowType.GetMethod("LogMsg", new[] {typeof(string), typeof(float), typeof(Color)});

                    methodLogMsg.Invoke(null,
                        new object[] {$"HUD loaded in {sw.Elapsed.TotalMilliseconds} ms.", 7, Color.GreenYellow});

                    var methodCoreRender = coreType.GetMethod("Render");
                    var methodInfo = coreType.GetMethod("FixImGui");
                    form.FixImguiCapture = () => methodInfo?.Invoke(instanceCore, null);

                    RenderLoop.Run(form, () =>
                    {
                        try
                        {
                            methodCoreRender.Invoke(instanceCore, null);
                        }
                        catch (Exception e)
                        {
                            var methodLogError = DebugWindowType.GetMethod("LogError");
                            methodLogError.Invoke(null, new object[] {e.ToString(), 2});
                        }
                    });

                    coreDispose.Invoke(instanceCore, null);
                }

                logger.Information($"{stringWith15MinusChars} Close hud at {DateTime.Now} {stringWith15MinusChars}");
            }
            catch (Exception e)
            {
                if (logger != null)
                {
                    logger.Error($"Loader -> {e}");

                    logger.Information(
                        $"{stringWith15MinusChars} Close hud at {DateTime.Now} {stringWith15MinusChars}");
                }
                else
                    File.WriteAllText("Logs\\Loader.txt", e.ToString());

                MessageBox.Show(e.ToString(), "Error while launching program");
            }
        }
    }
}
