using System.IO;
using System.Text;
using UnityEngine;
using VoidspireStudio.FNATS.Core;

public static class SaveManager
{
    public static SaveData LastSavedData { get; private set; } = new();
    public static string SaveFilePath => Path.Combine(Application.persistentDataPath, "save.sav");

    public static void LoadGame()
    {
        if (!File.Exists(SaveFilePath))
        {
            LastSavedData = new SaveData();
            return;
        }

        byte[] encryptedData = File.ReadAllBytes(SaveFilePath);
        LastSavedData = LoadData(DecodeBytes(ProcessData(encryptedData)));
    }

    public static void SaveGame()
    {
        byte[] encryptedData = ProcessData(EncodeBytes(SaveData(LastSavedData)));
        File.WriteAllBytes(SaveFilePath, encryptedData);
    }

    private static byte[] ProcessData(byte[] dataToProcess)
    {
        byte[] result = new byte[dataToProcess.Length];
        for (int i = 0; i < dataToProcess.Length; i++)
            result[i] = (byte)(dataToProcess[i] ^ 0xBA);
        return result;
    }

    private static string DecodeBytes(byte[] data) => Encoding.UTF8.GetString(data, 0, data.Length);
    private static byte[] EncodeBytes(string data) => Encoding.UTF8.GetBytes(data);

    private static SaveData LoadData(string data) => JsonUtility.FromJson<SaveData>(data);
    private static string SaveData(SaveData data) => JsonUtility.ToJson(data);
}