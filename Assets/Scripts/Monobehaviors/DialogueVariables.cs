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
                    GameObject animatorObject = new GameObject("DialogueVariables");
                    _instance = animatorObject.AddComponent<DialogueVariables>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private Dictionary<string, (Type, object, Action<object>)> dialogueVariables = new();

    public void CreateVariable<T>(string name, T value, Action<object> onChanged = null)
    {
        if (dialogueVariables.ContainsKey(name))
        {
            Debug.LogWarning($"Variable with name '{name}' already exists.");
            return;
        }
        dialogueVariables[name] = (typeof(T), value, onChanged);
    }

    public void SetVariable<T>(string name, T newValue)
    {
        if (dialogueVariables.TryGetValue(name, out var value))
        {
            if (value.Item1 == typeof(T) || typeof(T) == typeof(object))
            {
                dialogueVariables[name] = (value.Item1, newValue, value.Item3);
                value.Item3?.Invoke(newValue);
            }
            else
                throw new InvalidCastException($"Variable with name '{name}' is of type '{value.Item1}', not '{typeof(T)}'.");
        }
        else
            throw new KeyNotFoundException($"Variable with name '{name}' not found.");
    }

    public Type GetVariableType(string name)
    {
        if (dialogueVariables.TryGetValue(name, out var value))
            return value.Item1;
        Debug.LogError($"Variable with name '{name}' not found.");
        return null;
    }

    public T GetVariable<T>(string name)
    {
        if (dialogueVariables.TryGetValue(name, out var value))
        {
            if (value.Item1 == typeof(T) || typeof(T) == typeof(object))
                return (T)value.Item2;
            Debug.LogError($"Variable with name '{name}' is of type '{value.Item1}', not '{typeof(T)}'.");
        }
        Debug.LogError($"Variable with name '{name}' not found.");
        return default;
    }
}
