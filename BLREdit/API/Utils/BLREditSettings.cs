using System.IO;
using System.Windows;

namespace BLREdit
{
    public class BLREditSettings
    {
#if DEBUG
        public bool EnableDebugging { get; set; } = true;
        public LogTypes LogLevel { get; set; } = LogTypes.All | LogTypes.Info | LogTypes.Warning | LogTypes.Error | LogTypes.Fatal | LogTypes.Timing;
#else
        public bool EnableDebugging { get; set; } = false;
        public LogTypes LogLevel { get; set; } = LogTypes.None;
#endif
        public bool ShowUpdateNotice { get; set; } = true;
        public bool DoRuntimeCheck { get; set; } = true;
        public bool ForceRuntimeCheck { get; set; } = false;

        public static BLREditSettings LoadSettings()
        {
            if (File.Exists(IOResources.SETTINGS_FILE))
            {
                BLREditSettings settings = IOResources.DeserializeFile<BLREditSettings>(IOResources.SETTINGS_FILE); //Load settings file
                IOResources.SerializeFile(IOResources.SETTINGS_FILE, settings);                                     //write it back to disk to clean out settings that don't exist anymore from old builds/versions
                return settings;
            }
            else
            {
                var tmp = new BLREditSettings();
                IOResources.SerializeFile(IOResources.SETTINGS_FILE, tmp);
                return tmp;
            }
        }
    }
}