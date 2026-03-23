using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Quest
{
    [SerializeField] private string _name;                              // The name of the quest
    [SerializeField] private string _description;                       // The description of the quest
    [SerializeField] private List<QuestObject> _associatedObjects;      // The objects associated with the quest
    private List<Func<bool>> _clearConditions;                          // The conditions to clear the quest

    public string Name => _name;                                        // The name of the quest
    public string Description => _description;                          // The description of the quest
    public List<QuestObject> AssociatedObjects => _associatedObjects;   // The objects associated with the quest


    public event Action OnQuestCompleted;                               // Event triggered when the quest is completed

    public Quest(string name, string description, List<QuestObject> associatedObjects)
    {
        // Initialize quest properties
        _name = name;
        _description = description;
        _associatedObjects = associatedObjects;
        _clearConditions = new();

        // Subscribe to quest object interactions
        foreach (var questObject in _associatedObjects)
            questObject.OnQuestObjectInteracted += CheckQuestCompletion;
    }

    public Quest(string name, string description, List<QuestObject> associatedObjects, List<Func<bool>> clearConditions)
    {
        // Initialize quest properties
        _name = name;
        _description = description;
        _associatedObjects = associatedObjects;
        _clearConditions = clearConditions;

        // Subscribe to quest object interactions
        foreach (var questObject in _associatedObjects)
            questObject.OnQuestObjectInteracted += CheckQuestCompletion;

        // Initial check for quest completion
        CheckQuestCompletion();
    }

    /// <summary>
    /// Sets the conditions required to clear the quest.
    /// </summary>
    /// <param name="conditions">The list of conditions to clear the quest. </param>
    public void SetClearConditions(List<Func<bool>> conditions) => _clearConditions = conditions;

    /// <summary>
    /// Adds a new condition to clear the quest.
    /// </summary>
    /// <param name="condition">The condition to be added.</param>
    public void AddClearCondition(Func<bool> condition) => _clearConditions.Add(condition);

    /// <summary>
    /// Checks if the quest completion condition is met and triggers the completion event if so.
    /// </summary>
    private void CheckQuestCompletion()
    {
        // Check if all clear conditions are met
        foreach (var condition in _clearConditions)
            if (!condition.Invoke())
                return;

        // Trigger the quest completed event
        OnQuestCompleted?.Invoke();

        // Mark the quest as completed in the QuestManager
        QuestManager.Instance.CompleteQuest(this);
    }
}
