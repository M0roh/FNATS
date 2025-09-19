using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Localization.Settings;
using VoidspireStudio.FNATS.Core;

namespace VoidspireStudio.FNATS.Saves
{
    public static class SaveManager
    {
        public static bool HasSavedGame => (LastSavedData?.lastNight ?? 0) > 0;
        public static SaveData LastSavedData { get; private set; } = new();

        private static string SaveFilePath => Path.Combine(Application.persistentDataPath, "save.sav");

        public static void LoadGame()
        {
            if (!File.Exists(SaveFilePath))
            {
                LastSavedData = new SaveData();
                return;
            }

            byte[] encryptedData = File.ReadAllBytes(SaveFilePath);
            LastSavedData = LoadData(DecodeBytes(ProcessData(encryptedData, 0xBA)));

            UpdateSettings();
        }

        public static void UpdateSettings()
        {
            AudioManager.Instance.UpdateMusicVolume(LastSavedData.volumeMusic);
            AudioManager.Instance.UpdateSFXVolume(LastSavedData.volumeSFX);
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[LastSavedData.languageIndex];
        }

        public static void SaveGame()
        {
            byte[] encryptedData = ProcessData(EncodeBytes(SaveData(LastSavedData)), 0xBA);
            File.WriteAllBytes(SaveFilePath, encryptedData);
        }

        public static byte[] EncodeData(string data) => ProcessData(EncodeBytes(data), 0x5C);
        public static string DecodeData(byte[] data) => DecodeBytes(ProcessData(data, 0x5C));

        private static byte[] ProcessData(byte[] dataToProcess, byte key)
        {
            byte[] result = new byte[dataToProcess.Length];
            for (int i = 0; i < dataToProcess.Length; i++)
                result[i] = (byte)(dataToProcess[i] ^ key);
            return result;
        }

        private static string DecodeBytes(byte[] data) => Encoding.UTF8.GetString(data, 0, data.Length);
        private static byte[] EncodeBytes(string data) => Encoding.UTF8.GetBytes(data);

        private static SaveData LoadData(string data) => JsonUtility.FromJson<SaveData>(data);
        private static string SaveData(SaveData data) => JsonUtility.ToJson(data);
    }
}