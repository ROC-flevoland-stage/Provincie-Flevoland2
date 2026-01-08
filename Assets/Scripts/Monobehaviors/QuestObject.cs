using System;
using UnityEngine;

public class QuestObject : MonoBehaviour
{
    public event Action OnQuestObjectInteracted;

    /// <summary>
    /// Triggers the interaction event for this quest object.
    /// </summary>
    public void Interact()
    {
        OnQuestObjectInteracted?.Invoke();
    }
}
