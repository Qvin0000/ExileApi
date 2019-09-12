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
                for (var i = 0; i < a.Length; i++)
                {
                    var arg = a[i].ToLower();

                    switch (arg)
                    {
                        case "offset":
                        case "offsets":
                            CreateOffsets(true);
                            Application.Exit();
                            return;
                        case "compile_plugins":
                            CompilePluginsIntoDll();
                            Application.Exit();
                            return;
                    }
                }

                using (var form = new AppForm())
                {
                    var CoreDll = Assembly.Load("ExileCore");
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

                    CreateOffsets();
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

        private static void CreateOffsets(bool force = false)
        {
            var dllName = "GameOffsets.dll";
            var dllInfo = new FileInfo(dllName);
            var dirInfo = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GameOffsets"));

            if (!dllInfo.Exists && !dirInfo.Exists)
            {
                MessageBox.Show("Offsets dll and folder not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
                return;
            }
            if (!dirInfo.Exists)
            {
                if (force)
                {
                    MessageBox.Show("Offsets folder not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }

            var filesNames = dirInfo.GetFiles("*.cs", SearchOption.AllDirectories).Select(x => x.FullName).ToArray();
            var shouldCompile = force;

            if (!shouldCompile)
            {
                foreach (var filesName in filesNames)
                {
                    if (new FileInfo(filesName).LastWriteTimeUtc > dllInfo.LastWriteTimeUtc)
                    {
                        shouldCompile = true;
                        break;
                    }
                }
            }

            if (shouldCompile)
            {
                using (CodeDomProvider provider = new CSharpCodeProvider())
                {
                    var _compilerSettings = provider.GetType().GetField("_compilerSettings",
                            BindingFlags.Instance | BindingFlags.NonPublic)
                        .GetValue(provider);

                    var _compilerFullPath = _compilerSettings
                        .GetType().GetField("_compilerFullPath", BindingFlags.Instance | BindingFlags.NonPublic);

                    _compilerFullPath.SetValue(_compilerSettings,
                        ((string) _compilerFullPath.GetValue(_compilerSettings))
                        .Replace(@"bin\roslyn\", @"roslyn\"));

                    var RootDirectory = AppDomain.CurrentDomain.BaseDirectory;

                    var csFiles = dirInfo.GetFiles("*.cs", SearchOption.AllDirectories).Select(x => x.FullName)
                        .ToArray();

                    var parameters = new CompilerParameters
                    {
                        GenerateExecutable = false,
                        OutputAssembly = dllName,
                        IncludeDebugInformation = true,
                        ReferencedAssemblies = {"System.dll", "SharpDX.dll", "SharpDX.Mathematics.dll"},
                        GenerateInMemory = true,
                        CompilerOptions = "/optimize /unsafe"
                    };

                    var result = provider.CompileAssemblyFromFile(parameters, csFiles);

                    if (result.Errors.HasErrors)
                    {
                        var AllErrors = "";

                        foreach (CompilerError compilerError in result.Errors)
                        {
                            AllErrors += compilerError + Environment.NewLine;
                        }

                        MessageBox.Show(AllErrors, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(1);
                    }

                    Assembly.Load(File.ReadAllBytes(dllName));
                }
            }
        }

        private static void CompilePluginsIntoDll()
        {
            var list = new List<(string source, string dist)>();
            var rootDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var pathToSources = Path.Combine(rootDirectory, "Plugins", "Source");
            var directoryInfos = new DirectoryInfo(pathToSources).GetDirectories();

            if (directoryInfos.Length == 0)
            {
                MessageBox.Show("Plugins/Source/ is empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            using (CodeDomProvider provider = new CSharpCodeProvider())
            {
                var _compilerSettings = provider.GetType()
                    .GetField("_compilerSettings", BindingFlags.Instance | BindingFlags.NonPublic)
                    .GetValue(provider);

                var _compilerFullPath = _compilerSettings
                    .GetType().GetField("_compilerFullPath", BindingFlags.Instance | BindingFlags.NonPublic);

                _compilerFullPath.SetValue(_compilerSettings,
                    ((string) _compilerFullPath.GetValue(_compilerSettings)).Replace(@"bin\roslyn\", @"roslyn\"));

                var rootDirInfo = new DirectoryInfo(rootDirectory);

                var dllFiles = rootDirInfo.GetFiles("*.dll", SearchOption.TopDirectoryOnly)
                    .Where(x => !x.Name.Equals("cimgui.dll") && x.Name.Count(c => c == '-' || c == '_') != 5)
                    .Select(x => x.FullName).ToArray();

             

                Parallel.ForEach(directoryInfos, info =>
                {
                    /*foreach (var info in directoryInfos)
                    {*/
                    var sw = Stopwatch.StartNew();
                    var csFiles = info.GetFiles("*.cs", SearchOption.AllDirectories).Select(x => x.FullName)
                        .ToArray();
                    var csProj = info.GetFiles("*.csproj", SearchOption.AllDirectories).FirstOrDefault();
                    if (csProj == null)
                    {
                        MessageBox.Show($".csproj for plugin {info.Name} not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    var compiledDir = info.FullName.Replace("\\Source\\", "\\Compiled\\");

                    if (!Directory.Exists(compiledDir))
                        Directory.CreateDirectory(compiledDir);

                    var parameters = new CompilerParameters
                    {
                        GenerateExecutable = false, GenerateInMemory = false,
                        CompilerOptions = "/optimize /unsafe",
                        OutputAssembly = Path.Combine(compiledDir, $"{info.Name}.dll"),
                        IncludeDebugInformation = true
                    };

                    parameters.ReferencedAssemblies.AddRange(dllFiles);
                    var csprojPath = Path.Combine(info.FullName, $"{info.Name}.csproj");

                    if (File.Exists(csprojPath))
                    {
                        var readAllLines = File.ReadAllLines(csprojPath);

                        var refer = readAllLines
                            .Where(x =>
                                x.TrimStart().StartsWith("<Reference Include=") && x.TrimEnd().EndsWith("/>"));

                        var refer2 = readAllLines.Where(x =>
                            x.TrimStart().StartsWith("<Reference Include=") && x.TrimEnd().EndsWith("\">") &&
                            x.Contains(","));

                        foreach (var r in refer)
                        {
                            var arr = new int[2] {0, 0};
                            var j = 0;

                            for (var i = 0; i < r.Length; i++)
                            {
                                if (r[i] == '"')
                                {
                                    arr[j] = i;
                                    j++;
                                }

                                if (j == 2)
                                    break;
                            }

                            if (arr[1] != 0)
                            {
                                var dll = $"{r.Substring(arr[0] + 1, arr[1] - arr[0] - 1)}.dll";
                                parameters.ReferencedAssemblies.Add(dll);
                            }
                        }

                        foreach (var r in refer2)
                        {
                            var arr = new int[2] {0, 0};
                            var j = 0;

                            for (var i = 0; i < r.Length; i++)
                            {
                                if (r[i] == '"' && j == 0)
                                {
                                    arr[0] = i;
                                    j++;
                                }
                                else if (r[i] == ',')
                                {
                                    arr[1] = i;
                                    j++;
                                }

                                if (j == 2)
                                    break;
                            }

                            if (arr[1] != 0)
                            {
                                var dll = $"{r.Substring(arr[0] + 1, arr[1] - arr[0] - 1)}.dll";
                                parameters.ReferencedAssemblies.Add(dll);
                            }
                        }
                    }

                    var libsFolder = Path.Combine(info.FullName, "libs");

                    if (Directory.Exists(libsFolder))
                    {
                        var libsDll = Directory.GetFiles(libsFolder, "*.dll");
                        parameters.ReferencedAssemblies.AddRange(libsDll);
                    }

                    var result = provider.CompileAssemblyFromFile(parameters, csFiles);

                    if (result.Errors.HasErrors)
                    {
                        var AllErrors = "";

                        foreach (CompilerError compilerError in result.Errors)
                        {
                            AllErrors += compilerError + Environment.NewLine;
                        }

                        MessageBox.Show($"{info.Name} -> Failed look in Source/{info.Name}/Errors.txt");
                        File.AppendAllText(Path.Combine(info.FullName, "Errors.txt"), AllErrors);
                    }
                    else
                        MessageBox.Show($"{info.Name}  >>> Successful <<< (Working time: {sw.ElapsedMilliseconds} ms.)");

                    //}
                });
            }
        }
    }
}
