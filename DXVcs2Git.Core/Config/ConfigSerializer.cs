using System;
using System.IO;
using System.Reflection;

namespace DXVcs2Git.Core.Configuration {
    public static class ConfigSerializer {
        public static readonly string AppPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static readonly string SettingsPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\GitTools\\";
        static readonly string SettingsFile = "ui_settings.config";
        static readonly string SettingsFile2 = "ui_settings_2.config";
        static string SettingsFilePath {
            get { return SettingsPath + (Version == 1 ? SettingsFile2 : SettingsFile); }
        }

        public static int Version { get; set; } = 0;

        public static Config GetConfig() {
            if (!File.Exists(SettingsFilePath))
                return Config.GenerateDefault();
            try {
                return Config.Validate(Serializer.Deserialize<Config>(SettingsFilePath));
            }
            catch {
                return Config.GenerateDefault();
            }
        }
        public static void SaveConfig(Config options) {
            try {
                Serializer.Serialize(SettingsFilePath, options);
            }
            catch {
            }
        }
        public static bool IsConfigEquals(Config left, Config right) {
            return GetConfigAsString(left) == GetConfigAsString(right);
        }
        static string GetConfigAsString(Config config) {
            using(MemoryStream stream = new MemoryStream()) {
                Serializer.Serialize(stream, config);
                stream.Seek(0, SeekOrigin.Begin);
                using(StreamReader reader = new StreamReader(stream))
                    return reader.ReadToEnd();
            }
        }
    }
}
