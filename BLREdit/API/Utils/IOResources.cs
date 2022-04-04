using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BLREdit
{
    public class IOResources
    {
        public const string PROFILE_DIR = "Profiles\\";
        public const string SEPROFILE_DIR = "SEProfiles\\";
        public const string ASSET_DIR = "Assets\\";
        public const string JSON_DIR = "json\\";
        public const string GEAR_FILE = ASSET_DIR + JSON_DIR + "gear.json";
        public const string MOD_FILE = ASSET_DIR + JSON_DIR + "mods.json";
        public const string WEAPON_FILE = ASSET_DIR + JSON_DIR + "weapons.json";
        public const string SETTINGS_FILE = "settings.json";
        public const string CHECKSUM = "checksum.json";
        public const string IMG_CACHE = "Cache\\";

        private static ConcurrentDictionary<string, byte[]> fileChecksums = new();

        public static Encoding FILE_ENCODING { get; } = Encoding.UTF8;
        public static JsonSerializerOptions JSOFields { get; } = new JsonSerializerOptions() { WriteIndented = true, IncludeFields = true, Converters = { new JsonStringEnumConverter() } };
        public static JsonSerializerOptions JSOCompacted { get; } = new JsonSerializerOptions() { WriteIndented = false, IncludeFields = true, Converters = { new JsonStringEnumConverter() } };

        static IOResources()
        {
            if (File.Exists(CHECKSUM))
            {
                fileChecksums = DeserializeFile<ConcurrentDictionary<string, byte[]>>(CHECKSUM);
            }
        }

        public static void SaveChecksum()
        {
            SerializeFile(CHECKSUM, fileChecksums, true, false);
        }


        public static void CopyToBackup(string file)
        {
            FileInfo info = new(file);
            File.Copy(file, ExportSystem.CurrentBackupFolder.FullName + info.Name, true);
        }

        public static byte[] GetChecksum(string fileContent)
        {
            using var crypt = MD5.Create();
            return crypt.ComputeHash(FILE_ENCODING.GetBytes(fileContent));
        }

        public static byte[] GetChecksumForFile(string filePath)
        {
            fileChecksums.TryGetValue(filePath, out byte[] checksum);
            return checksum;
        }

        public static void SerializeFile<T>(string filePath, T obj, bool compact = false, bool doChecksum = true)
        {
            bool isSame = false;
            //if the object we want to serialize is null we can instantly exit this function as we dont have anything to do as well the filePath
            if (string.IsNullOrEmpty(filePath)) { LoggingSystem.Log("filePath was empty!", LogTypes.Warning); return; }
            string json = Serialize(obj);
            byte[] checksum = GetChecksum(json);
            byte[] fileChecksum = GetChecksumForFile(filePath);
            //remove file before we write to it to prevent resedue data
            if (File.Exists(filePath))
            {
                if (fileChecksum != null && fileChecksum.SequenceEqual(checksum))
                {
                    isSame = true;
                    LoggingSystem.Log("Same Contents!");
                }
                else
                {
                    try
                    {
                        File.Delete(filePath); }
                    catch { }
                }
            }

            if (!isSame)
            {
                fileChecksums.AddOrUpdate(filePath, checksum, (key, oldvalue) => checksum);
                using var file = File.CreateText(filePath);
                file.Write(json);
                file.Close();
                LoggingSystem.Log("Serialize Succes!");
            }
        }

        public static string Serialize<T>(T obj, bool compact = false)
        {
            //if the object we want to serialize is null we can instantly exit this function as we dont have anything to do as well the filePath
            if (obj == null) { LoggingSystem.Log("object was empty!", LogTypes.Warning); return ""; }
            if (compact)
            {
                return JsonSerializer.Serialize<T>(obj, JSOCompacted);
            }
            else
            {
                return JsonSerializer.Serialize<T>(obj, JSOFields);
            }
        }

        public static T DeserializeFile<T>(string filePath)
        {
            T temp = default;
            if (string.IsNullOrEmpty(filePath)) { return temp; }

            //check if file exist's before we try to read it if it doesn't exist return and Write an error to log
            if (!File.Exists(filePath))
            { LoggingSystem.Log("File:(" + filePath + ") was not found for Deserialization!", LogTypes.Error); return temp; }

            using (var file = File.OpenText(filePath))
            {
                temp = Deserialize<T>(file.ReadToEnd());
                file.Close();
            }
            return temp;
        }

        public static T Deserialize<T>(string json)
        {
            //LoggingSystem.LogInfo(json);
            return JsonSerializer.Deserialize<T>(json, JSOFields);
        }
    }
}