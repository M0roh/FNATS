using Newtonsoft.Json;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Localization.Settings;
using VoidspireStudio.FNATS.Sounds;

namespace VoidspireStudio.FNATS.Saves
{
    public static class SaveManager
    {
        public static bool HasSavedGame => (LastSavedData?.lastNight ?? 0) > 0;
        public static SaveData LastSavedData { get; private set; } = new();

        private static string SaveFilePath => Path.Combine(Application.persistentDataPath, "GameSaves.sav");

        public static void LoadGame()
        {
            if (!File.Exists(SaveFilePath))
            {
                LastSavedData = new SaveData();
                LastSavedData.graphics.resolutionIndex = Screen.resolutions.Length - 1;
                Debug.Log("Save not found");
                return;
            }

            byte[] encryptedData = File.ReadAllBytes(SaveFilePath);
            string loadedData = DecodeBytes(ProcessData(encryptedData, 0xBA));
            LastSavedData = LoadData(loadedData);

            UpdateSettings();
        }

        public static void UpdateSettings()
        {
            int resolutionIndex = (LastSavedData.graphics.resolutionIndex < 0 || LastSavedData.graphics.resolutionIndex >= Screen.resolutions.Length) ? (Screen.resolutions.Length - 1) : LastSavedData.graphics.resolutionIndex;
            Resolution resolution = Screen.resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, LastSavedData.graphics.isFullscreen);
            QualitySettings.vSyncCount = LastSavedData.graphics.vSync ? 1 : 0;
            Application.targetFrameRate = LastSavedData.graphics.vSync ? -1 : LastSavedData.graphics.fpsCap;

            AudioManager.Instance.UpdateSettings(LastSavedData.audio);
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[LastSavedData.gameplay.languageIndex];
        }

        public static void SaveGame()
        {
            string dataToSave = SaveData(LastSavedData);
            byte[] encryptedData = ProcessData(EncodeBytes(dataToSave), 0xBA);
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

        private static SaveData LoadData(string data) => JsonConvert.DeserializeObject<SaveData>(data);
        private static string SaveData(SaveData data) => JsonConvert.SerializeObject(data);
    }
}