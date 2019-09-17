using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using JM.LinqFaster;
using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;

namespace ExileCore
{
    public static class CommandExecutor
    {
        public const string Offset = "offset";
        public const string OffsetS = "offsets";
        public const string LoaderOffsets = "loader_offsets";
        public const string CompilePlugins = "compile_plugins";
        public const string GameOffsets = "GameOffsets.dll";


        public static void Execute(string cmd)
        {
            switch (cmd)
            {
                case Offset:
                case OffsetS:
                    CreateOffsets(true);
                    return;
                case CompilePlugins:
                    CompilePluginsIntoDll();
                    return;
                case LoaderOffsets:
                    CreateOffsets();
                    return;
                default:
                    if (cmd.StartsWith("compile_"))
                    {
                        var directoryInfo = new DirectoryInfo(Path.Combine("Plugins", "Source"));
                        var plugin = cmd.Replace("compile_", "");
                        if (directoryInfo.GetDirectories().FirstOrDefaultF(x => x.Name.Equals(plugin,StringComparison.OrdinalIgnoreCase)) != null)
                            CompilePluginIntoDll(plugin);
                    }

                    return;
            }
        }

        private static void CompilePluginIntoDll(string plugin)
        {
            var rootDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var pathToSources = Path.Combine(rootDirectory, "Plugins", "Source");
            var directories = new DirectoryInfo(pathToSources).GetDirectories();
            var pluginDir = directories.FirstOrDefaultF(x=>x.Name.Equals(plugin,StringComparison.OrdinalIgnoreCase));
            if (pluginDir == null)
            {
                DebugWindow.LogError($"{plugin} directory not found.");
                return;
            }

            using (var provider = PrepareProvider())
            {
                CompileSourceIntoDll(provider,pluginDir);
            }
            
        }

        private static CodeDomProvider PrepareProvider()
        {
            CodeDomProvider provider = new CSharpCodeProvider();
            var _compilerSettings = provider.GetType()
                .GetField("_compilerSettings", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(provider);

            var _compilerFullPath = _compilerSettings
                .GetType().GetField("_compilerFullPath", BindingFlags.Instance | BindingFlags.NonPublic);

            _compilerFullPath.SetValue(_compilerSettings,
                ((string) _compilerFullPath.GetValue(_compilerSettings)).Replace(@"bin\roslyn\", @"roslyn\"));


            return provider;
        }

        private static string[] _dllFiles;

        private static string[] GetAllDllFilesFromRootDirectory()
        {
            if (_dllFiles == null)
            {
                var rootDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var rootDirInfo = new DirectoryInfo(rootDirectory);
                _dllFiles = rootDirInfo.GetFiles("*.dll", SearchOption.TopDirectoryOnly)
                    .Where(x => !x.Name.Equals("cimgui.dll") && x.Name.Count(c => c == '-' || c == '_') != 5)
                    .Select(x => x.FullName).ToArray();
            }

            return _dllFiles;
        }

        private static void CompileSourceIntoDll(CodeDomProvider provider, DirectoryInfo info)
        {
            var sw = Stopwatch.StartNew();

            var csFiles = info.GetFiles("*.cs", SearchOption.AllDirectories).Select(x => x.FullName)
                .ToArray();
            var csProj = info.GetFiles("*.csproj", SearchOption.AllDirectories).FirstOrDefault();
            if (csProj == null)
            {
                MessageBox.Show($".csproj for plugin {info.Name} not found.", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
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

            parameters.ReferencedAssemblies.AddRange(GetAllDllFilesFromRootDirectory());
            var csprojPath = csProj.FullName;

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

            var errorsCount = 0;
            if (result.Errors.HasErrors)
            {
                var AllErrors = "";

                foreach (CompilerError compilerError in result.Errors)
                {
                    AllErrors += compilerError + Environment.NewLine;
                    errorsCount++;
                }

                MessageBox.Show($"{info.Name} -> Failed (Errors: {errorsCount}) look in {info.FullName}/Errors.txt",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                File.WriteAllText(Path.Combine(info.FullName, "Errors.txt"), AllErrors);
            }
            else
            {
                MessageBox.Show($"{info.Name}  >>> Successful <<< (Working time: {sw.ElapsedMilliseconds} ms.)");
            }
        }


        private static void CreateOffsets(bool force = false)
        {
            var dllInfo = new FileInfo(GameOffsets);
            var dirInfo = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GameOffsets"));

            if (!dllInfo.Exists && !dirInfo.Exists)
            {
                MessageBox.Show("Offsets dll and folder not found.", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Environment.Exit(0);
                return;
            }

            if (!dirInfo.Exists)
            {
                if (force)
                    MessageBox.Show("Offsets folder not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            var filesNames = dirInfo.GetFiles("*.cs", SearchOption.AllDirectories).Select(x => x.FullName).ToArray();
            var shouldCompile = force;

            if (!shouldCompile)
                foreach (var filesName in filesNames)
                    if (new FileInfo(filesName).LastWriteTimeUtc > dllInfo.LastWriteTimeUtc)
                    {
                        shouldCompile = true;
                        break;
                    }

            if (shouldCompile)
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
                        OutputAssembly = GameOffsets,
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
                            AllErrors += compilerError + Environment.NewLine;

                        MessageBox.Show(AllErrors, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(1);
                    }

                    Assembly.Load(File.ReadAllBytes(GameOffsets));
                }
        }

        private static void CompilePluginsIntoDll()
        {
            var rootDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var pathToSources = Path.Combine(rootDirectory, "Plugins", "Source");
            var directories = new DirectoryInfo(pathToSources).GetDirectories();
            var directoryInfos = directories.Where(x => (x.Attributes & FileAttributes.Hidden) == 0).ToList();
            if (directoryInfos.Count == 0)
            {
                MessageBox.Show("Plugins/Source/ is empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            using (var provider = PrepareProvider())
            {
                Parallel.ForEach(directoryInfos, info => { CompileSourceIntoDll(provider, info); });
            }
            
        }
    }
}