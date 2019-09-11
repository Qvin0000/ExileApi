using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Helpers;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using JM.LinqFaster;
using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using MoreLinq.Extensions;

namespace ExileCore.Shared
{
    public class PluginManager
    {
        public const string PluginsDirectory = "Plugins";
        public const string CompiledPluginsDirectory = "Compiled";
        public const string SourcePluginsDirectory = "Source";
        private readonly GameController _gameController;
        private readonly Graphics _graphics;
        private readonly MultiThreadManager _multiThreadManager;
        private readonly Dictionary<string, string> Directories = new Dictionary<string, string>();
        private object initLoadLocker = new object();
        private readonly object lockerLoadAsm = new object();
        private readonly Dictionary<string, Stopwatch> PluginLoadTime = new Dictionary<string, Stopwatch>();

        public PluginManager(GameController gameController, Graphics graphics, MultiThreadManager multiThreadManager)
        {
            _gameController = gameController;
            _graphics = graphics;
            _multiThreadManager = multiThreadManager;
            RootDirectory = AppDomain.CurrentDomain.BaseDirectory;
            Directories["Temp"] = Path.Combine(RootDirectory, PluginsDirectory, "Temp");
            Directories[PluginsDirectory] = Path.Combine(RootDirectory, PluginsDirectory);

            Directories[CompiledPluginsDirectory] =
                Path.Combine(Directories[PluginsDirectory], CompiledPluginsDirectory);

            Directories[SourcePluginsDirectory] = Path.Combine(Directories[PluginsDirectory], SourcePluginsDirectory);
            _gameController.EntityListWrapper.EntityAdded += EntityListWrapperOnEntityAdded;
            _gameController.EntityListWrapper.EntityRemoved += EntityListWrapperOnEntityRemoved;
            _gameController.EntityListWrapper.EntityAddedAny += EntityListWrapperOnEntityAddedAny;
            _gameController.EntityListWrapper.EntityIgnored += EntityListWrapperOnEntityIgnored;
            _gameController.Area.OnAreaChange += AreaOnOnAreaChange;

            bool parallelLoading = _gameController.Settings.CoreSettings.MultiThreadLoadPlugins;

            foreach (var directory in Directories)
            {
                if (!Directory.Exists(directory.Value))
                {
                    DebugWindow.LogMsg($"{directory.Value} doesn't exists, but don't worry i created it for you.");
                    Directory.CreateDirectory(directory.Value);
                }
            }

            var (compiledPlugins, sourcePlugins) = SearchPlugins();
            List<(Assembly asm, bool source)> assemblies = new List<(Assembly, bool)>();
            var locker = new object();

            var task = Task.Run(() =>
            {
                var compilePluginsFromSource = CompilePluginsFromSource(sourcePlugins);

                lock (locker)
                {
                    compilePluginsFromSource.ForEach(assembly => assemblies.Add((assembly, true)));
                }
            });

            var task2 = Task.Run(() =>
            {
                var compiledAssemblies = GetCompiledAssemblies(compiledPlugins, parallelLoading);

                lock (locker)
                {
                    compiledAssemblies.Where(x => x != null).ForEach(assembly => assemblies.Add((assembly, false)));
                }
            });

            Task.WaitAll(task, task2);

            if (parallelLoading)
                Parallel.ForEach(assemblies, tuple => TryLoadAssemble(tuple.asm, tuple.source));
            else
                assemblies.ForEach(tuple => TryLoadAssemble(tuple.asm, tuple.source));

            Plugins = Plugins.OrderBy(x => x.Order).ThenByDescending(x => x.CanBeMultiThreading).ThenBy(x => x.Name)
                .ToList();

            if (parallelLoading)
            {
                //Pre init some general objects because with multi threading load they can null sometimes for some plugin
                var ingameStateIngameUi = gameController.IngameState.IngameUi;
                var ingameStateData = gameController.IngameState.Data;
                var ingameStateServerData = gameController.IngameState.ServerData;
                Parallel.ForEach(Plugins, wrapper => InitPlugin(wrapper.Plugin));
            }
            else
                Plugins.ForEach(wrapper => InitPlugin(wrapper.Plugin));

            AreaOnOnAreaChange(gameController.Area.CurrentArea);
            AllPluginsLoaded = true;
        }

        public bool AllPluginsLoaded { get; }
        public string RootDirectory { get; }
        public List<PluginWrapper> Plugins { get; } = new List<PluginWrapper>();

        private List<Assembly> GetCompiledAssemblies(DirectoryInfo[] compiledPlugins, bool parallel)
        {
            var results = new List<Assembly>(compiledPlugins.Length);

            if (parallel)
            {
                Parallel.ForEach(compiledPlugins, info =>
                {
                    var loadedAssembly = LoadAssembly(info);

                    lock (lockerLoadAsm)
                    {
                        results.Add(loadedAssembly);
                    }
                });
            }
            else
            {
                compiledPlugins.ForEach(info =>
                {
                    var loadedAssembly = LoadAssembly(info);
                    results.Add(loadedAssembly);
                });
            }

            return results;
        }

        private Assembly LoadAssembly(DirectoryInfo dir)
        {
            try
            {
                var directoryInfo = new DirectoryInfo(dir.FullName);

                if (!directoryInfo.Exists)
                {
                    LogError($"Directory - {dir} not found.");
                    return null;
                }

                var dll = directoryInfo.GetFiles($"{directoryInfo.Name}*.dll", SearchOption.TopDirectoryOnly)
                    .FirstOrDefault();

                if (dll == null)
                {
                    LogError($"Not found plugin dll. (Dll {dir.Name} should be like folder)");
                    return null;
                }

                PluginLoadTime[Path.Combine(Directories[CompiledPluginsDirectory], dir.Name)] = Stopwatch.StartNew();

                var pdbPath = dll.FullName.Replace(".dll", ".pdb");
                var pdbExists = File.Exists(pdbPath);

                var dllBytes = File.ReadAllBytes(dll.FullName);

                Assembly asm;

                if (pdbExists)
                {
                    var pdbBytes = File.ReadAllBytes(pdbPath);
                    asm = Assembly.Load(dllBytes, pdbBytes);
                }
                else
                    asm = Assembly.Load(dllBytes);

                return asm;
            }
            catch (Exception e)
            {
                LogError($"{nameof(LoadAssembly)} -> {e}");
                return null;
            }
        }

        private List<Assembly> CompilePluginsFromSource(DirectoryInfo[] sourcePlugins, bool parallel = true)
        {
            using (CodeDomProvider provider =
                new CSharpCodeProvider())
            using (new PerformanceTimer("Compile and load source plugins"))
            {
                var _compilerSettings = provider.GetType()
                    .GetField("_compilerSettings", BindingFlags.Instance | BindingFlags.NonPublic)
                    .GetValue(provider);

                var _compilerFullPath = _compilerSettings
                    .GetType().GetField("_compilerFullPath", BindingFlags.Instance | BindingFlags.NonPublic);

                _compilerFullPath.SetValue(_compilerSettings,
                    ((string) _compilerFullPath.GetValue(_compilerSettings)).Replace(@"bin\roslyn\", @"roslyn\"));

                var rootDirInfo = new DirectoryInfo(RootDirectory);

                var dllFiles = rootDirInfo.GetFiles("*.dll", SearchOption.TopDirectoryOnly)
                    .WhereF(x => !x.Name.Equals("cimgui.dll") && x.Name.Count(c => c == '-' || c == '_') != 5)
                    .SelectF(x => x.FullName).ToArray();

                var results = new List<Assembly>(sourcePlugins.Length);

                if (parallel)
                {
                    var innerLocker = new object();
                    ;

                    Parallel.ForEach(sourcePlugins, info =>
                    {
                        using (new PerformanceTimer($"Compile and load source plugin: {info.Name}"))
                        {
                            var csFiles = info.GetFiles("*.cs", SearchOption.AllDirectories).Select(x => x.FullName)
                                .ToArray();

                            var parameters = new CompilerParameters
                            {
                                GenerateExecutable = false, GenerateInMemory = true,
                                CompilerOptions = "/optimize /unsafe"
                            };

                            parameters.ReferencedAssemblies.AddRange(dllFiles);
                            var csprojPath = Path.Combine(info.FullName, $"{info.Name}.csproj");

                            if (File.Exists(csprojPath))
                            {
                                var readAllLines = File.ReadAllLines(csprojPath);

                                var refer = readAllLines
                                    .WhereF(x =>
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

                            //   parameters.TempFiles = new TempFileCollection($"{MainDir}\\{PluginsDirectory}\\Temp");
                            //  parameters.CoreAssemblyFileName = info.Name;
                            var result = provider.CompileAssemblyFromFile(parameters, csFiles);

                            if (result.Errors.HasErrors)
                            {
                                var AllErrors = "";

                                foreach (CompilerError compilerError in result.Errors)
                                {
                                    AllErrors += compilerError + Environment.NewLine;
                                    Logger.Log.Error($"{info.Name} -> {compilerError}");
                                }

                                File.WriteAllText(Path.Combine(info.FullName, "Errors.txt"), AllErrors);

                                // throw new Exception("Offsets file corrupted");
                            }
                            else
                            {
                                lock (innerLocker)
                                {
                                    results.Add(result.CompiledAssembly);
                                }
                            }
                        }
                    });
                }
                else
                {
                    foreach (var info in sourcePlugins)
                    {
                        using (new PerformanceTimer($"Compile and load source plugin: {info.Name}"))
                        {
                            var csFiles = info.GetFiles("*.cs", SearchOption.AllDirectories).Select(x => x.FullName)
                                .ToArray();

                            var parameters = new CompilerParameters
                            {
                                GenerateExecutable = false, GenerateInMemory = true,
                                CompilerOptions = "/optimize /unsafe"
                            };

                            parameters.ReferencedAssemblies.AddRange(dllFiles);
                            var csprojPath = Path.Combine(info.FullName, $"{info.Name}.csproj");

                            if (File.Exists(csprojPath))
                            {
                                var readAllLines = File.ReadAllLines(csprojPath);

                                var refer = readAllLines
                                    .WhereF(x =>
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

                            //   parameters.TempFiles = new TempFileCollection($"{MainDir}\\{PluginsDirectory}\\Temp");
                            //  parameters.CoreAssemblyFileName = info.Name;
                            var result = provider.CompileAssemblyFromFile(parameters, csFiles);

                            if (result.Errors.HasErrors)
                            {
                                var AllErrors = "";

                                foreach (CompilerError compilerError in result.Errors)
                                {
                                    AllErrors += compilerError + Environment.NewLine;
                                    Logger.Log.Error($"{info.Name} -> {compilerError}");
                                }

                                File.WriteAllText(Path.Combine(info.FullName, "Errors.txt"), AllErrors);

                                // throw new Exception("Offsets file corrupted");
                            }
                            else
                                results.Add(result.CompiledAssembly);
                        }
                    }
                }

                return results;
            }
        }

        private void TryLoadAssemble(Assembly asm, bool fromSource)
        {
            try
            {
                var dir = asm.ManifestModule.ScopeName.Replace(".dll", "");

                var fullPath =
                    Path.Combine(
                        fromSource ? Directories[SourcePluginsDirectory] : Directories[CompiledPluginsDirectory], dir);

                PluginLoadTime[fullPath] = Stopwatch.StartNew();
                var types = asm.GetTypes();

                if (types.Length == 0)
                {
                    LogError($"Not found any types in plugin {fullPath}");
                    return;
                }

                var classesWithIPlugin = types.WhereF(type => typeof(IPlugin).IsAssignableFrom(type));
                var settings = types.FirstOrDefaultF(type => typeof(ISettings).IsAssignableFrom(type));

                if (settings == null)
                {
                    LogError("Not found setting class");
                    return;
                }

                foreach (var type in classesWithIPlugin)
                {
                    if (type.IsAbstract) continue;

                    if (Activator.CreateInstance(type) is IPlugin instance)
                    {
                        var pluginWrapper = new PluginWrapper(instance);
                        instance.DirectoryName = dir;
                        instance.DirectoryFullName = fullPath;
                        pluginWrapper.SetApi(_gameController, _graphics);
                        pluginWrapper.LoadSettings();
                        OnLoad(instance);
                        Plugins.Add(pluginWrapper);
                    }
                }
            }
            catch (Exception e)
            {
                LogError($"Error when load plugin ({asm.ManifestModule.ScopeName}): {e})");
            }
        }

        private void OnLoad(IPlugin plugin)
        {
            // lock (initLoadLocker)
            {
                try
                {
                    plugin.OnLoad();
                }
                catch (Exception e)
                {
                    LogError($"{plugin.Name} OnLoad-> {e}");
                }
            }
        }

        private bool InitPlugin(IPlugin plugin)
        {
            // lock (initLoadLocker)
            {
                try
                {
                    if (plugin._Settings == null)
                    {
                        DebugWindow.LogError($"Cant load plugin ({plugin.Name}) because settings is null.");
                        plugin._SaveSettings();
                        return false;
                    }

                    if (plugin.Initialized) return true;

                    plugin._Settings.Enable.OnValueChanged += (obj, value) =>
                    {
                        if (plugin.Initialized)
                        {
                            if (value)
                                plugin.AreaChange(_gameController.Area.CurrentArea);
                            else
                                return;
                        }

                        if (value)
                        {
                            plugin.Initialized = plugin.Initialise();
                            if (plugin.Initialized) plugin.AreaChange(_gameController.Area.CurrentArea);
                        }

                        if (!plugin.Initialized) plugin._Settings.Enable = new ToggleNode(false);
                    };

                    if (plugin._Settings.Enable)
                    {
                        if (plugin.Initialized) return true;
                        plugin.Initialized = plugin.Initialise();

                        if (PluginLoadTime.TryGetValue(plugin.DirectoryFullName, out var sw))
                            DebugWindow.LogMsg($"{plugin.Name} loaded in {sw.Elapsed.TotalMilliseconds} ms.", 2);
                        else
                            DebugWindow.LogError($"{plugin.Name} problem with load timer.");
                    }
                }
                catch (Exception ex)
                {
                    DebugWindow.LogError($"Plugin {plugin.Name} error init {ex}", 5);
                    Logger.Log.Error($"Plugin {plugin.Name} error init {ex}");
                    return false;
                }

                return true;
            }
        }

        //By default priority - Compiled 
        private (DirectoryInfo[] CompiledDirectories, DirectoryInfo[] SourceDirectories) SearchPlugins()
        {
            var CompiledDirectories = new DirectoryInfo(Directories[CompiledPluginsDirectory]).GetDirectories();

            var SourceDirectories = new DirectoryInfo(Directories[SourcePluginsDirectory]).GetDirectories()
                .ExceptBy(CompiledDirectories, info => info.Name)
                .ToArray();

            return (CompiledDirectories, SourceDirectories);
        }

        public void CloseAllPlugin()
        {
            foreach (var plugin in Plugins)
            {
                plugin.Close();
            }
        }

        private void AreaOnOnAreaChange(AreaInstance area)
        {
            foreach (var plugin in Plugins)
            {
                if (plugin.IsEnable)
                    plugin.AreaChange(area);
            }
        }

        private void EntityListWrapperOnEntityIgnored(Entity entity)
        {
            foreach (var plugin in Plugins)
            {
                if (plugin.IsEnable)
                    plugin.EntityIgnored(entity);
            }
        }

        private void EntityListWrapperOnEntityAddedAny(Entity entity)
        {
            foreach (var plugin in Plugins)
            {
                if (plugin.IsEnable)
                    plugin.EntityAddedAny(entity);
            }
        }

        private void EntityListWrapperOnEntityAdded(Entity entity)
        {
            if (_gameController.Settings.CoreSettings.AddedMultiThread && _multiThreadManager.ThreadsCount > 0)
            {
                var listJob = new List<Job>();

                Plugins.WhereF(x => x.IsEnable).Batch(_multiThreadManager.ThreadsCount)
                    .ForEach(wrappers =>
                        listJob.Add(_multiThreadManager.AddJob(() => wrappers.ForEach(x => x.EntityAdded(entity)),
                            "Entity added")));

                _multiThreadManager.Process(this);
                SpinWait.SpinUntil(() => listJob.AllF(x => x.IsCompleted), 500);
            }
            else
            {
                foreach (var plugin in Plugins)
                {
                    if (plugin.IsEnable)
                        plugin.EntityAdded(entity);
                }
            }
        }

        private void EntityListWrapperOnEntityRemoved(Entity entity)
        {
            foreach (var plugin in Plugins)
            {
                if (plugin.IsEnable)
                    plugin.EntityRemoved(entity);
            }
        }

        private void LogError(string msg)
        {
            DebugWindow.LogError(msg, 5);
        }
    }
}
