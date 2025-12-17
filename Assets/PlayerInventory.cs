using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private List<string> items = new List<string>();

    public event Action<string> OnItemAdded;

    public void AddItem(string itemName)
    {
        items.Add(itemName);
        Debug.Log("Item toegevoegd: " + itemName);

        OnItemAdded?.Invoke(itemName);
    }

    public bool HasItem(string itemName)
    {
        return items.Contains(itemName);
    }

    public bool RemoveItem(string itemName)
    {
        return items.Remove(itemName);
    }
}
