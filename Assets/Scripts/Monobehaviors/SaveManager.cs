using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class SaveManager : MonoBehaviour
{
    private static Dictionary<string, (object value, bool isPersistent)> SaveData = new(); // Key: Name, Value: (Value, IsPersistent)
    private static JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.Auto,
        Formatting = Formatting.Indented,
        Converters = new List<JsonConverter>
        {
            new Vector3Converter(),
            new Vector2Converter(),
            new QuaternionConverter(),
            new ColorConverter()
        }
    };

    public static string saveFilePath { get { return Application.persistentDataPath + "/save"; } }  // Path to save file

    /// <summary>
    /// Creates or updates a value in the save data.<para></para>
    /// <c>isPersistant</c> does not update if the key already exists.
    /// </summary>
    /// <typeparam name="T">Type of the value to store.</typeparam>
    /// <param name="key">The name of the value.</param>
    /// <param name="value">The value to store.</param>
    /// <param name="isPersistant">If the value should persist across sessions.</param>
    public static void CreateOrSetValue<T>(string key, T value, bool isPersistant = false)
    {
        if (SaveData.ContainsKey(key))
        {
            var existing = SaveData[key];
            SaveData[key] = (value, existing.isPersistent);
        }
        else
            SaveData.Add(key, (value, isPersistant));
    }

    /// <summary>
    /// Gets a value from the save data.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve.</typeparam>
    /// <param name="key">The name of the value.</param>
    /// <returns>The value associated with the specified key.</returns>
    public static T GetValue<T>(string key)
    {
        return (T)SaveData[key].value;
    }

    /// <summary>
    /// Tries to get a value from the save data.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve.</typeparam>
    /// <param name="key">The name of the value.</param>
    /// <param name="value">The output value if found.</param>
    /// <returns>True if the value was found, false otherwise.</returns>
    public static bool TryGetValue<T>(string key, out T value)
    {
        if (SaveData.ContainsKey(key))
        {
            value = (T)SaveData[key].value;
            return true;
        }
        value = default;
        return false;
    }

    /// <summary>
    /// Deletes a value from the save data if it exists.
    /// </summary>
    /// <param name="key">The name of the value to delete.</param>
    public static void DeleteValue(string key)
    {
        if (SaveData.ContainsKey(key))
            SaveData.Remove(key);
    }

    /// <summary>
    /// Saves the current saveData to PlayerPrefs, a .json file, and an 'encrypted' .sav file.
    /// </summary>
    public static void SaveDataToFile()
    {
        var persistentData = GetPersistantData();
        string saveString = JsonConvert.SerializeObject(persistentData, JsonSerializerSettings);

        // Save to PlayerPrefs
        PlayerPrefs.SetString("SaveData", saveString);
        PlayerPrefs.Save();

        // Save to .json file
        System.IO.File.WriteAllText(saveFilePath + ".json", saveString);

        // Save to encripted .sav file
        using MemoryStream stream = new MemoryStream();
        using BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(saveString);
        byte[] data = stream.ToArray();
        System.IO.File.WriteAllBytes(saveFilePath + ".sav", data);

        Debug.Log($"Saved to {saveFilePath}");
    }

    /// <summary>
    /// Loads the saveData from PlayerPrefs, a .json file, and an 'encrypted' .sav file. And checks for integrity.
    /// </summary>
    public static void LoadDataFromFile()
    {
        bool playerPrefsExists = PlayerPrefs.HasKey("SaveData");
        bool jsonFileExists = System.IO.File.Exists(saveFilePath + ".json");
        bool savFileExists = System.IO.File.Exists(saveFilePath + ".sav");

        int amountOfSaveStrings = (playerPrefsExists ? 1 : 0) + (jsonFileExists ? 1 : 0) + (savFileExists ? 1 : 0);
        string[] saveStrings = new string[amountOfSaveStrings];
        int index = 0;
        if (playerPrefsExists)
            saveStrings[index++] = PlayerPrefs.GetString("SaveData");
        if (jsonFileExists)
            saveStrings[index++] = System.IO.File.ReadAllText(saveFilePath + ".json");
        if (savFileExists)
        {
            byte[] data = System.IO.File.ReadAllBytes(saveFilePath + ".sav");
            using MemoryStream stream = new MemoryStream(data);
            using BinaryReader reader = new BinaryReader(stream);
            saveStrings[index++] = reader.ReadString();
        }

        // No save data found
        if (amountOfSaveStrings == 0)
        {
            Debug.Log("No save data found.");
            return;
        }

        // Check integrity and load data if valid
        if (CheckSaveIntegrity(saveStrings))
        {
            // Convert the save string back to dictionary
            var persistentData = JsonConvert.DeserializeObject<Dictionary<string, object>>(saveStrings[0]);
            SaveData.Clear();
            // Repopulate SaveData with persistent entries
            foreach (var kvp in persistentData)
                SaveData.Add(kvp.Key, (kvp.Value, true));
            Debug.Log($"Save data loaded successfully.\n{saveStrings[0]}");
        }
        else
        {
            Debug.LogError("Save data is corrupted.");
            // Delete System32 or something, idk. ¯\_(^.^)_/¯
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

    private static Dictionary<string, object> GetPersistantData()
    {
        Dictionary<string, object> persistentData = new Dictionary<string, object>();
        foreach (var kvp in SaveData)
        {
            if (kvp.Value.isPersistent)
                persistentData.Add(kvp.Key, kvp.Value.value);
        }
        return persistentData;
    }
}

// Json Converters for Unity types
public class Vector3Converter : JsonConverter<Vector3>
{
    public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("x"); writer.WriteValue(value.x);
        writer.WritePropertyName("y"); writer.WriteValue(value.y);
        writer.WritePropertyName("z"); writer.WriteValue(value.z);
        writer.WriteEndObject();
    }

    public override Vector3 ReadJson(JsonReader reader, System.Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var obj = serializer.Deserialize<JObject>(reader);
        return new Vector3(
            obj["x"].Value<float>(),
            obj["y"].Value<float>(),
            obj["z"].Value<float>()
        );
    }
}

public class Vector2Converter : JsonConverter<Vector2>
{
    public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("x"); writer.WriteValue(value.x);
        writer.WritePropertyName("y"); writer.WriteValue(value.y);
        writer.WriteEndObject();
    }

    public override Vector2 ReadJson(JsonReader reader, System.Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var obj = serializer.Deserialize<JObject>(reader);
        return new Vector2(
            obj["x"].Value<float>(),
            obj["y"].Value<float>()
        );
    }
}

public class QuaternionConverter : JsonConverter<Quaternion>
{
    public override void WriteJson(JsonWriter writer, Quaternion value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("x"); writer.WriteValue(value.x);
        writer.WritePropertyName("y"); writer.WriteValue(value.y);
        writer.WritePropertyName("z"); writer.WriteValue(value.z);
        writer.WritePropertyName("w"); writer.WriteValue(value.w);
        writer.WriteEndObject();
    }

    public override Quaternion ReadJson(JsonReader reader, System.Type objectType, Quaternion existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var obj = serializer.Deserialize<JObject>(reader);
        return new Quaternion(
            obj["x"].Value<float>(),
            obj["y"].Value<float>(),
            obj["z"].Value<float>(),
            obj["w"].Value<float>()
        );
    }
}

public class ColorConverter : JsonConverter<Color>
{
    public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("r"); writer.WriteValue(value.r);
        writer.WritePropertyName("g"); writer.WriteValue(value.g);
        writer.WritePropertyName("b"); writer.WriteValue(value.b);
        writer.WritePropertyName("a"); writer.WriteValue(value.a);
        writer.WriteEndObject();
    }
    public override Color ReadJson(JsonReader reader, System.Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var obj = serializer.Deserialize<JObject>(reader);
        return new Color(
            obj["r"].Value<float>(),
            obj["g"].Value<float>(),
            obj["b"].Value<float>(),
            obj["a"].Value<float>()
        );
    }
}
