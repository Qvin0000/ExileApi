using System;
using System.IO;
using System.Threading;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using Newtonsoft.Json;

namespace ExileCore
{
    public class SettingsContainer
    {
        private const string SETTINGS_FILE_NAME = "config/settings.json";
        private const string DEFAULT_PROFILE_NAME = "global";
        private const string CFG_DIR = "config";
        public static readonly JsonSerializerSettings jsonSettings;
        private string _currentProfileName = "";
        public CoreSettings CoreSettings;

        static SettingsContainer()
        {
            jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new SortContractResolver(),
                Converters = new JsonConverter[] {new ColorNodeConverter(), new ToggleNodeConverter(), new FileNodeConverter()}
            };
        }

        public SettingsContainer()
        {
            if (!Directory.Exists(CFG_DIR)) Directory.CreateDirectory(CFG_DIR);

            if (!Directory.Exists($"{CFG_DIR}\\{DEFAULT_PROFILE_NAME}")) Directory.CreateDirectory($"{CFG_DIR}\\{DEFAULT_PROFILE_NAME}");

            LoadCoreSettings();
        }

        private static ReaderWriterLockSlim rwLock { get; } = new ReaderWriterLockSlim();

        private string CurrentProfileName
        {
            get => _currentProfileName;
            set
            {
                OnProfileChange?.Invoke(this, value);
                _currentProfileName = value;
            }
        }

        public event EventHandler<string> OnProfileChange;

        public void LoadCoreSettings()
        {
            try
            {
                if (!File.Exists(SETTINGS_FILE_NAME))
                {
                    var coreSettings = new CoreSettings();
                    File.AppendAllText(SETTINGS_FILE_NAME, JsonConvert.SerializeObject(coreSettings, Formatting.Indented));
                }
                else
                {
                    var readAllText = File.ReadAllText(SETTINGS_FILE_NAME);
                    CoreSettings = JsonConvert.DeserializeObject<CoreSettings>(readAllText);
                }

                CurrentProfileName = CoreSettings.Profiles.Value;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void SaveCoreSettings()
        {
            try
            {
                rwLock.EnterWriteLock();
                var serializeObject = JsonConvert.SerializeObject(CoreSettings, Formatting.Indented);
                var info = new FileInfo(SETTINGS_FILE_NAME);
                if (info.Length > 1) File.Copy(SETTINGS_FILE_NAME, $"{CFG_DIR}\\dumpSettings.json", true);
                File.WriteAllText(SETTINGS_FILE_NAME, serializeObject);
                rwLock.ExitWriteLock();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void SaveSettings(IPlugin plugin)
        {
            if (plugin == null) return;
            if (string.IsNullOrWhiteSpace(CurrentProfileName)) CurrentProfileName = DEFAULT_PROFILE_NAME;
            rwLock.EnterWriteLock();

            if (!Directory.Exists($"{CFG_DIR}\\{CurrentProfileName}")) Directory.CreateDirectory($"{CFG_DIR}\\{CurrentProfileName}");

            File.WriteAllText($"{CFG_DIR}\\{CurrentProfileName}\\{plugin.InternalName}_settings.json",
                JsonConvert.SerializeObject(plugin._Settings, Formatting.Indented, jsonSettings));

            rwLock.ExitWriteLock();
        }

        public string LoadSettings(IPlugin plugin)
        {
            if (!Directory.Exists($"{CFG_DIR}\\{CurrentProfileName}"))
                throw new DirectoryNotFoundException($"{CurrentProfileName} not found in {CFG_DIR}");

            var formattableString = $"{CFG_DIR}\\{CurrentProfileName}\\{plugin.Name}_settings.json";
            if (!File.Exists(formattableString)) return default;

            var readAllText = File.ReadAllText(formattableString);
            return readAllText.Length == 0 ? null : readAllText;
        }

        public static TSettingType LoadSettingFile<TSettingType>(string fileName)
        {
            if (!File.Exists(fileName))
            {
                Logger.Log.Error("Cannot find " + fileName + " file.");
                return default;
            }

            return JsonConvert.DeserializeObject<TSettingType>(File.ReadAllText(fileName));
        }

        public static void SaveSettingFile<TSettingType>(string fileName, TSettingType setting)
        {
            var serialized = JsonConvert.SerializeObject(setting, Formatting.Indented);

            File.WriteAllText(fileName, serialized);
        }
    }
}
