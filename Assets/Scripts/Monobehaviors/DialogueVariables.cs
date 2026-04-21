using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueVariables : MonoBehaviour
{
    private static DialogueVariables _instance;

    public static DialogueVariables Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<DialogueVariables>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("DialogueVariables");
                    _instance = go.AddComponent<DialogueVariables>();
                }
            }
            return _instance;
        }
    }

    private Dictionary<string, (Type type, object value, Action<object> callback)> dialogueVariables = new();

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeVariables();
        }
        else Destroy(gameObject);
    }

    private void InitializeVariables()
    {
        // Hier maken we alle variabelen aan die de game nodig heeft
        CreateVariable<bool>("wil je dit geven", false);
        CreateVariable<bool>("portemonnee_geaccepteerd", false);
        CreateVariable<bool>("portemonnee_teruggegeven", false);
        CreateVariable<bool>("help_portemonnee_zoeken", false);
        CreateVariable<int>("financiele_situatie", 0);
    }

    public void CreateVariable<T>(string name, T value, Action<object> onChanged = null)
    {
        if (!dialogueVariables.ContainsKey(name))
            dialogueVariables[name] = (typeof(T), value, onChanged);
    }

    public void SetVariable<T>(string name, T newValue)
    {
        if (dialogueVariables.TryGetValue(name, out var varData))
        {
            dialogueVariables[name] = (varData.type, newValue, varData.callback);
            varData.callback?.Invoke(newValue);
        }
        else Debug.LogError($"Variable '{name}' niet gevonden!");
    }

    public T GetVariable<T>(string name)
    {
        if (dialogueVariables.TryGetValue(name, out var varData))
            return (T)varData.value;

        Debug.LogError($"Variable '{name}' niet gevonden!");
        return default;
    }

    public Type GetVariableType(string name) => dialogueVariables.ContainsKey(name) ? dialogueVariables[name].type : null;

    // Handige methode om een callback (zoals item weggooien) later toe te voegen
    public void RegisterCallback(string name, Action<object> callback)
    {
        if (dialogueVariables.TryGetValue(name, out var varData))
            dialogueVariables[name] = (varData.type, varData.value, callback);
    }
}
