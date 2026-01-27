using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    /// <summary>
    /// Data structure for game save data.
    /// </summary>
    public class SaveData
    {
        public int energie;
        public int geld;
        public int stress;
    }

    public static SaveData saveData = new SaveData();                               // Current save data
    public static string saveFilePath = Application.persistentDataPath + "/save";   // Path to save files in

    /// <summary>
    /// Saves the current saveData to PlayerPrefs, a .json file, and an 'encrypted' .sav file.
    /// </summary>
    public static void SaveDataToFile()
    {
        string saveString = JsonUtility.ToJson(saveData);

        // Save to PlayerPrefs
        PlayerPrefs.SetString("SaveData", saveString);
        PlayerPrefs.Save();

        // Save to .json file
        File.WriteAllText(saveFilePath + ".json", saveString);

        // Save to encripted .sav file
        using MemoryStream stream = new MemoryStream();
        using BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(saveString);
        byte[] data = stream.ToArray();
        File.WriteAllBytes(saveFilePath + ".sav", data);
    }

    /// <summary>
    /// Loads the saveData from PlayerPrefs, a .json file, and an 'encrypted' .sav file. And checks for integrity.
    /// </summary>
    public static void LoadDataFromFile()
    {
        bool playerPrefsExists = PlayerPrefs.HasKey("SaveData");
        bool jsonFileExists = File.Exists(saveFilePath + ".json");
        bool savFileExists = File.Exists(saveFilePath + ".sav");

        int amountOfSaveStrings = (playerPrefsExists ? 1 : 0) + (jsonFileExists ? 1 : 0) + (savFileExists ? 1 : 0);
        string[] saveStrings = new string[amountOfSaveStrings];
        int index = 0;
        if (playerPrefsExists)
            saveStrings[index++] = PlayerPrefs.GetString("SaveData");
        if (jsonFileExists)
            saveStrings[index++] = File.ReadAllText(saveFilePath + ".json");
        if (savFileExists)
        {
            byte[] data = File.ReadAllBytes(saveFilePath + ".sav");
            using MemoryStream stream = new MemoryStream(data);
            using BinaryReader reader = new BinaryReader(stream);
            saveStrings[index++] = reader.ReadString();
        }

        // Check integrity and load data if valid
        if (CheckSaveIntegrity(saveStrings))
            saveData = JsonUtility.FromJson<SaveData>(saveStrings[0]);
        else
        {
            Debug.LogError("Save data is corrupted.");
        }
    }

    /// <summary>
    /// Checks if all provided save strings are identical.
    /// </summary>
    /// <param name="strings">The array of save strings to check.</param>
    /// <returns></returns>
    private static bool CheckSaveIntegrity(string[] strings)
    {
        // Check if all save strings are identical
        for (int i = 1; i < strings.Length; i++)
        {
            for (int j = 0; j < strings.Length; j++)
            {
                if (strings[i] != strings[j])
                    return false;
            }
        }
        return true;
    }
}
