using System;
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
        public const string IMG_CACHE = "Cache\\";

        public static Encoding FILE_ENCODING { get; } = Encoding.UTF8;
        public static JsonSerializerOptions JSOFields { get; } = new JsonSerializerOptions() { WriteIndented = true, IncludeFields = true, Converters = { new JsonStringEnumConverter() } };
        public static JsonSerializerOptions JSOCompacted { get; } = new JsonSerializerOptions() { WriteIndented = false, IncludeFields = true, Converters = { new JsonStringEnumConverter() } };


        public static void CopyToBackup(string file)
        {
            FileInfo info = new FileInfo(file);
            File.Copy(file, ExportSystem.CurrentBackupFolder.FullName + info.Name, true);
        }

        public static byte[] GetChecksum(string fileContent)
        {
            using (var crypt = MD5.Create())
            {
                return crypt.ComputeHash(FILE_ENCODING.GetBytes(fileContent));
            }
        }

        public static byte[] ReadChecksumFromFile(Stream stream)
        { 
            stream.Position = 0;
            byte[] checksum = new byte[16];
            stream.Read(checksum, 0, 16);
            stream.Position = 0;
            return checksum;
        }

        public static void SerializeFile<T>(string filePath, T obj, bool compact = false)
        {
            bool isSame = false;
            //if the object we want to serialize is null we can instantly exit this function as we dont have anything to do as well the filePath
            if (string.IsNullOrEmpty(filePath)) { LoggingSystem.LogWarning("filePath was empty!"); return; }
            string json = Serialize(obj);
            byte[] checksum = GetChecksum(json);
            //remove file before we write to it to prevent resedue data
            if (File.Exists(filePath))
            {
                var checkFile = File.Open(filePath, FileMode.Open);

                byte[] fileChecksum = ReadChecksumFromFile(checkFile);
                checkFile.Close();

                if (fileChecksum.SequenceEqual(checksum))
                {
                    isSame = true;
                    LoggingSystem.LogInfo("Same Contents!");
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
                using (var file = File.CreateText(filePath))
                {
                    file.BaseStream.Write(checksum, 0, checksum.Length);
                    file.Write(Environment.NewLine + json);
                    file.Close();
                    LoggingSystem.LogInfo("Serialize Succes!");
                }
            }
        }

        public static string Serialize<T>(T obj, bool compact = false)
        {
            //if the object we want to serialize is null we can instantly exit this function as we dont have anything to do as well the filePath
            if (obj == null) { LoggingSystem.LogWarning("object were empty!"); return ""; }
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
            { LoggingSystem.LogError("File:(" + filePath + ") was not found for Deserialization!"); return temp; }

            byte[] checksum;

            using (var file = File.OpenRead(filePath))
            {
                checksum = ReadChecksumFromFile(file);

                string checksumString = Encoding.UTF8.GetString(checksum);
                LoggingSystem.LogInfo(checksumString);

                var text = new StreamReader(file);

                if (checksumString.StartsWith("{" + Environment.NewLine) || checksumString.StartsWith("[" + Environment.NewLine))
                {
                    text.DiscardBufferedData();
                    file.Seek(0, SeekOrigin.Begin);
                }
                else
                {
                    text.DiscardBufferedData();
                    file.Seek(16, SeekOrigin.Begin);
                }

                

                temp = Deserialize<T>(text.ReadToEnd());
                text.Dispose();
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