using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using Serilog;
using SharpDX;
using SharpDX.Windows;

namespace Loader
{
    internal class Program
    {
        public static void Main(string[] a) {
            ILogger logger = null;
            AppForm form;

            var sw = Stopwatch.StartNew();
            var stringWith15MinusChars = new string('-', 15);
            try
            {
                for (var i = 0; i < a.Length; i++)
                {
                    var arg = a[i].ToLower();
                    if (arg == "offset" || arg == "offsets")
                    {
                        CreateOffsets(true);
                        Application.Exit();
                        return;
                    }
                }

                form = new AppForm();

                //var CoreDll = Assembly.Load(File.ReadAllBytes($"{AppDomain.CurrentDomain.BaseDirectory}\\Core.dll"));

                var CoreDll = Assembly.LoadFrom("Core.dll");
                var coreType = CoreDll.GetType("Exile.Core", true, true);
                if (coreType == null) throw new NullReferenceException("Core not found.");
                var loggerType = CoreDll.GetType("Exile.Logger", true, true);
                var propertyInfo = loggerType.GetProperty("Log");

                if (propertyInfo != null)
                {
                    logger = propertyInfo.GetValue(null) as ILogger;
                    if (logger == null) throw new NullReferenceException("Logger cant be null.");

                    var perfomanceTimerType = CoreDll.GetType("Shared.Helpers.PerformanceTimer");
                    if (perfomanceTimerType != null)
                    {
                        var propertyPerfomanceTimerLogger = perfomanceTimerType.GetProperty("Logger");
                        if (propertyPerfomanceTimerLogger != null) propertyPerfomanceTimerLogger.SetValue(null, logger);
                    }

                    logger.Information($"{stringWith15MinusChars} Start hud at {DateTime.Now} {stringWith15MinusChars}");
                }
                else
                    throw new NullReferenceException("Not found Log property in Logger class.");
                var coreLogger = coreType.GetProperty("Logger");
                if (coreLogger != null)
                {
                    coreLogger.SetValue(null,logger);
                }
                var performanceTimerType = CoreDll.GetType("Shared.Helpers.PerformanceTimer", true, true);
                performanceTimerType.GetField("Logger").SetValue(null, logger);
                var methodPerfomanceTimerDispose = performanceTimerType.GetMethod("Dispose");
                var instanceCreateNewOffsets = Activator.CreateInstance(performanceTimerType, "Create new offsets", 0, null, true);
                CreateOffsets();
                methodPerfomanceTimerDispose.Invoke(instanceCreateNewOffsets, null);


                var instanceFormLoad = Activator.CreateInstance(performanceTimerType, "Form Load", 0, null, true);
                methodPerfomanceTimerDispose.Invoke(instanceFormLoad, null);
                var instanceCore = Activator.CreateInstance(coreType, form);


                var coreDispose = coreType.GetMethod("Dispose");


                var DebugWindowType = CoreDll.GetType("Exile.DebugWindow");

                var methodLogMsg = DebugWindowType.GetMethod("LogMsg", new[] {typeof(string), typeof(float), typeof(Color)});
                methodLogMsg.Invoke(null, new object[] {$"HUD loaded in {sw.Elapsed.TotalMilliseconds} ms.", 7, Color.GreenYellow});
                var methodCoreRender = coreType.GetMethod("Render");
                sw = null;

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
                form.Dispose();
                logger.Information($"{stringWith15MinusChars} Close hud at {DateTime.Now} {stringWith15MinusChars}");
            }
            catch (Exception e)
            {
                if (logger != null)
                {
                    logger.Error($"Loader -> {e}");
                    logger.Information($"{stringWith15MinusChars} Close hud at {DateTime.Now} {stringWith15MinusChars}");
                }
                else
                    File.WriteAllText("Logs\\Loader.txt", e.ToString());
            }
        }

        private static void CreateOffsets(bool force = false) {
            var dllName = "GameOffsets.dll";
            var dllInfo = new FileInfo(dllName);
            var dirInfo = new DirectoryInfo("GameOffsets");
            if (!dirInfo.Exists) MessageBox.Show("Offsets folder not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            var filesNames = dirInfo.GetFiles("*.cs", SearchOption.AllDirectories).Select(x => x.FullName).ToArray();
            var shouldCompile = force;
            if (!shouldCompile)
            {
                foreach (var filesName in filesNames)
                    if (new FileInfo(filesName).LastWriteTimeUtc > dllInfo.LastWriteTimeUtc)
                    {
                        shouldCompile = true;
                        // Logger.Log.Debug($"RECOMPILE: {filesName.Split('\\').Last()}");
                        //  break;
                    }
            }

            if (shouldCompile)
            {
                using (CodeDomProvider provider = new CSharpCodeProvider())
                {
                    var _compilerSettings = provider.GetType().GetField("_compilerSettings", BindingFlags.Instance | BindingFlags.NonPublic)
                                                    .GetValue(provider);
                    var _compilerFullPath = _compilerSettings
                                            .GetType().GetField("_compilerFullPath", BindingFlags.Instance | BindingFlags.NonPublic);
                    _compilerFullPath.SetValue(_compilerSettings,
                                               ((string) _compilerFullPath.GetValue(_compilerSettings))
                                               .Replace(@"bin\roslyn\", @"roslyn\"));

                    var RootDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    var csFiles = dirInfo.GetFiles("*.cs", SearchOption.AllDirectories).Select(x => x.FullName).ToArray();
                    var parameters = new CompilerParameters
                    {
                        GenerateExecutable = false,
                        OutputAssembly = dllName,
                        IncludeDebugInformation = true,
                        ReferencedAssemblies = {"System.dll", "Core.dll", "SharpDX.dll", "SharpDX.Mathematics.dll"},
                        GenerateInMemory = true,
                        CompilerOptions = "/optimize /unsafe"
                    };
                    var result = provider.CompileAssemblyFromFile(parameters, csFiles);

                    if (result.Errors.HasErrors)
                    {
                        var AllErrors = "";
                        foreach (CompilerError compilerError in result.Errors) AllErrors += compilerError + Environment.NewLine;

                        MessageBox.Show(AllErrors, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(1);
                    }

                    Assembly.Load(File.ReadAllBytes(dllName));
                }
            }
        }
    }
}