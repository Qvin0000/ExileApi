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
        private MethodInfo _executeCommandMethodInfo;
        private Type _coreType;
        private Type _performanceTimerType;
        private AppForm _form;
        private object _coreTypeInstance;
        private MethodInfo _renderMethodInfo;
        private MethodInfo _logMsgMethodInfo;
        private MethodInfo _logErrorMethodInfo;

        public void Load(string[] args)
        {
            _stopwatch = Stopwatch.StartNew();
            try
            {
                LoadCoreDll();
                LoadExecuteCommandMethod();

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

                LoadLogMsgMethod();
                LoadLogErrorMethod();
                LogHudLoadedMessage();

                LoadRenderMethod();

                RenderLoop.Run(_form, Render);

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

        private void LoadExecuteCommandMethod()
        {
            var commandExecutorType = _coreDll.GetType("ExileCore.CommandExecutor");
            _executeCommandMethodInfo = commandExecutorType.GetMethod("Execute");
        }

        private void ExecuteCommand(string command)
        {
            _executeCommandMethodInfo.Invoke(null, new object[] { command });
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

        private void LoadRenderMethod()
        {
            _renderMethodInfo = _coreType.GetMethod("Render");
        }

        private void Render()
        {
            try
            {
                _renderMethodInfo.Invoke(_coreTypeInstance, null);
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }

        private void LoadLogMsgMethod()
        {
            var debugWindowType = GetTypeFromCoreDll("ExileCore.DebugWindow");
            _logMsgMethodInfo = debugWindowType.GetMethod("LogMsg", new[] { typeof(string), typeof(float), typeof(Color) });
        }

        private void LoadLogErrorMethod()
        {
            var debugWindowType = GetTypeFromCoreDll("ExileCore.DebugWindow");
            _logErrorMethodInfo = debugWindowType.GetMethod("LogError");
        }

        private void LogHudLoadedMessage()
        {
            _logMsgMethodInfo.Invoke(null,
                new object[] { $"HUD loaded in {_stopwatch.Elapsed.TotalMilliseconds} ms.", 7, Color.GreenYellow });
        }

        private void LogError(Exception e)
        {
            _logErrorMethodInfo.Invoke(null, new object[] { e.ToString(), 2 });
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