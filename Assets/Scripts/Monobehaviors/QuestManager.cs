using UnityEngine;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    private static QuestManager _instance;

    public static QuestManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<QuestManager>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("QuestManager");
                    _instance = singletonObject.AddComponent<QuestManager>();
                }
                else
                    DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }

    private Dictionary<string, Quest> _activeQuest = new();                 // Dictionary to hold active quests
    private Dictionary<string, Quest> _completedQuest = new();              // Dictionary to hold completed quests

    public List<Quest> ActiveQuests => new(_activeQuest.Values);            // List of active quests
    public List<Quest> CompletedQuests => new(_completedQuest.Values);      // List of completed quests

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
            Destroy(gameObject);
    }

    /// <summary>
    /// Adds a new quest to the active quests list if it is not already active or completed.
    /// </summary>
    /// <param name="quest">The quest to be added. </param>
    public void AddActiveQuest(Quest quest)
    {
        if (!_activeQuest.ContainsKey(quest.Name) && !_completedQuest.ContainsKey(quest.Name))
            _activeQuest.Add(quest.Name, quest);
    }

    /// <summary>
    /// Marks a quest as completed, moving it from active to completed quests.
    /// </summary>
    /// <param name="quest">The quest to be marked as completed. </param>
    public void CompleteQuest(Quest quest)
    {
        if (!_completedQuest.ContainsKey(quest.Name))
            _completedQuest.Add(quest.Name, quest);
        if (_activeQuest.ContainsKey(quest.Name))
            _activeQuest.Remove(quest.Name);
    }
}
