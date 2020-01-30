using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Serilog;
using SharpDX;
using SharpDX.Windows;

namespace Loader
{
    public class Loader
    {
        private const string AttentionSign = "===============";

        private ILogger _logger;
        private Assembly _coreDll;
        private Stopwatch _stopwatch;
        private Type _coreType;
        private Type _performanceTimerType;
        private AppForm _form;
        private object _coreTypeInstance;

        public void Load(string[] args)
        {
            _stopwatch = Stopwatch.StartNew();
            try
            {
                LoadCoreDll();

                if (args.Length > 0)
                {
                    var arg = string.Join(" ", args.Select(x => x.ToLower()));
                    ExecuteCommand(arg);
                    return;
                }

                LoadLogger();
                LogStartMessage();
                LoadCoreType();
                LoadPerformanceTimerType();
                CreateOffsets();
                CreateForm();
                CreateCoreTypeInstance();
                SetUpFormFixImguiCapture();
                LogHudLoadedMessage();
                StartRenderLoop();
                DisposeCoreTypeInstance();
                LogCloseMessage();
            }
            catch (Exception e)
            {
                LogLoaderError(e);
            }
            finally
            {
                _form?.Dispose();
            }
        }

        private void LoadCoreDll()
        {
            _coreDll = Assembly.Load("ExileCore");
        }

        private void ExecuteCommand(string command)
        {
            var commandExecutorType = GetTypeFromCoreDll("ExileCore.CommandExecutor");
            var executeCommandMethodInfo = commandExecutorType.GetMethod("Execute");
            executeCommandMethodInfo.Invoke(null, new object[] { command });
        }

        private void LoadLogger()
        {
            var loggerType = GetTypeFromCoreDll("ExileCore.Logger");
            var loggerPropertyInfo = loggerType.GetProperty("Log") ??
                                     throw new InvalidOperationException("Not found Log property in Logger class.");
            _logger = loggerPropertyInfo.GetValue(null) as ILogger ?? throw new InvalidOperationException("Logger can't be null.");
        }

        private void LoadCoreType()
        {
            _coreType = GetTypeFromCoreDll("ExileCore.Core");
            _coreType.GetProperty("Logger")?.SetValue(null, _logger);
        }

        private void CreateOffsets()
        {
            MeasurePerformance("Create new offsets", () => ExecuteCommand("loader_offsets"));
        }

        private void CreateForm()
        {
            MeasurePerformance("Form Load", () => _form = new AppForm());
        }

        private void CreateCoreTypeInstance()
        {
            _coreTypeInstance = Activator.CreateInstance(_coreType, _form);
        }

        private void MeasurePerformance(string actionName, Action action)
        {
            var methodPerformanceTimerDispose = _performanceTimerType.GetMethod("Dispose");
            var instanceCreateNewOffsets = CreatePerformanceTimer(actionName);
            action();
            methodPerformanceTimerDispose.Invoke(instanceCreateNewOffsets, null);
        }

        private void LoadPerformanceTimerType()
        {
            _performanceTimerType = GetTypeFromCoreDll("ExileCore.Shared.Helpers.PerformanceTimer");
            _performanceTimerType.GetField("Logger").SetValue(null, _logger);
        }

        private object CreatePerformanceTimer(string debugText)
        {
            return Activator.CreateInstance(_performanceTimerType, debugText, 0, null, true);
        }

        private void SetUpFormFixImguiCapture()
        {
            var methodCoreFixImGui = _coreType.GetMethod("FixImGui");
            _form.FixImguiCapture = () => methodCoreFixImGui?.Invoke(_coreTypeInstance, null);
        }

        private void StartRenderLoop()
        {
            var renderMethodInfo = _coreType.GetMethod("Render");
            RenderLoop.Run(_form, () =>
            {
                try
                {
                    renderMethodInfo.Invoke(_coreTypeInstance, null);
                }
                catch (Exception e)
                {
                    LogError(e);
                }
            });
        }

        private void LogHudLoadedMessage()
        {
            var debugWindowType = GetTypeFromCoreDll("ExileCore.DebugWindow");
            var logMsgMethodInfo = debugWindowType.GetMethod("LogMsg", new[] { typeof(string), typeof(float), typeof(Color) });
            logMsgMethodInfo.Invoke(null,
                new object[] { $"HUD loaded in {_stopwatch.Elapsed.TotalMilliseconds} ms.", 7, Color.GreenYellow });
        }

        private void LogError(Exception e)
        {
            var debugWindowType = GetTypeFromCoreDll("ExileCore.DebugWindow");
            var logErrorMethodInfo = debugWindowType.GetMethod("LogError");
            logErrorMethodInfo.Invoke(null, new object[] { e.ToString(), 2 });
        }

        private void DisposeCoreTypeInstance()
        {
            var coreDispose = _coreType.GetMethod("Dispose");
            coreDispose.Invoke(_coreTypeInstance, null);
        }

        private Type GetTypeFromCoreDll(string name)
        {
            return _coreDll.GetType(name, true, true);
        }

        private void LogLoaderError(Exception e)
        {
            if (_logger != null)
            {
                _logger.Error($"Loader -> {e}");
                LogCloseMessage();
            }
            else
            {
                File.WriteAllText(@"Logs\Loader.txt", e.ToString());
            }

            MessageBox.Show(e.ToString(), "Error while launching program");
        }

        private void LogStartMessage()
        {
            _logger.Information($"{AttentionSign} Start hud at {DateTime.Now} {AttentionSign}");
        }

        private void LogCloseMessage()
        {
            _logger.Information($"{AttentionSign} Close hud at {DateTime.Now} {AttentionSign}");
        }
    }
}