using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    public enum StepType { TalkToNpc, CollectItem }

    [Serializable]
    public class Step
    {
        public StepType type;

        [Header("TalkToNpc")]
        public string targetNpcId;

        [Header("CollectItem")]
        public string itemId;
        public int amount = 1;
    }

    [Serializable]
    public class Quest
    {
        public string questId = "quest_1";
        public string title = "Mijn Quest";
        [TextArea] public string description;
        public List<Step> steps = new List<Step>();
    }

    [Serializable]
    public class NpcQuestOffer
    {
        public string npcId = "NPC_1";
        public List<string> questIdsToOffer = new List<string>();
    }

    [Header("Database (instellen in Inspector)")]
    [SerializeField] private List<Quest> quests = new List<Quest>();

    [Header("Welke NPC biedt welke quest(s) aan?")]
    [SerializeField] private List<NpcQuestOffer> npcOffers = new List<NpcQuestOffer>();

    [Header("UI (optioneel, onderin links)")]
    [SerializeField] private TextMeshProUGUI questText;
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private float notificationDuration = 2f;

    // Runtime (maar 1 actieve quest)
    private Quest activeQuest;
    private int stepIndex = -1;
    private int collectedCount = 0;

    private Coroutine notifRoutine;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        UpdateQuestUI();
        HideNotification();
    }

    // =========================
    //  NPC INTERACTION ENTRY
    // =========================

    /// <summary>
    /// Roep dit aan wanneer je met een NPC praat (geeft npcId mee).
    /// Dit doet 2 dingen:
    /// 1) Als er een talk-step actief is en dit is de juiste NPC: step completen
    /// 2) Als er geen actieve quest is: deze NPC kan een quest aanbieden/starten
    /// </summary>
    public void InteractWithNpc(string npcId)
    {
        // 1) Probeer eerst step progress
        if (HasActiveQuest())
        {
            Step step = GetCurrentStep();
            if (step != null && step.type == StepType.TalkToNpc && step.targetNpcId == npcId)
            {
                CompleteCurrentStep();
                ShowNotification("✅ Stap voltooid: gesprek");
                return;
            }

            ShowNotification("ℹ️ Geen quest-stap bij deze NPC.");
            return;
        }

        // 2) Geen actieve quest -> NPC kan een quest starten
        string questToStart = GetFirstQuestOfferForNpc(npcId);
        if (string.IsNullOrEmpty(questToStart))
        {
            ShowNotification("❌ Deze NPC heeft geen quest.");
            return;
        }

        StartQuest(questToStart);
    }

    /// <summary>
    /// Als jij later een quest-keuze UI wil: hiermee kun je exact kiezen welke quest je start.
    /// </summary>
    public void StartQuest(string questId)
    {
        Quest q = FindQuest(questId);
        if (q == null)
        {
            ShowNotification("⚠️ QuestId niet gevonden: " + questId);
            return;
        }

        activeQuest = q;
        stepIndex = 0;
        collectedCount = 0;

        ShowNotification("📜 Quest gestart: " + q.title);
        UpdateQuestUI();
    }

    // =========================
    //  ITEM COLLECT ENTRY
    // =========================

    /// <summary>
    /// Roep dit aan wanneer speler een item oppakt.
    /// amountAdded = 1 meestal.
    /// </summary>
    public void CollectItem(string itemId, int amountAdded = 1)
    {
        if (!HasActiveQuest()) return;

        Step step = GetCurrentStep();
        if (step == null) return;

        if (step.type != StepType.CollectItem) return;
        if (step.itemId != itemId) return;

        collectedCount += amountAdded;

        // Clamp zodat UI niet 6/5 toont
        int required = Mathf.Max(1, step.amount);
        if (collectedCount > required) collectedCount = required;

        UpdateQuestUI();

        if (collectedCount >= required)
        {
            CompleteCurrentStep();
            ShowNotification("✅ Items compleet!");
        }
        else
        {
            ShowNotification($"📦 {itemId}: {collectedCount}/{required}");
        }
    }

    // =========================
    //  INTERNAL HELPERS
    // =========================

    private bool HasActiveQuest()
    {
        return activeQuest != null && stepIndex >= 0 && stepIndex < activeQuest.steps.Count;
    }

    private Step GetCurrentStep()
    {
        if (!HasActiveQuest()) return null;
        return activeQuest.steps[stepIndex];
    }

    private void CompleteCurrentStep()
    {
        stepIndex++;
        collectedCount = 0;

        if (activeQuest != null && stepIndex >= activeQuest.steps.Count)
        {
            ShowNotification("🏆 Quest voltooid: " + activeQuest.title);
            // Quest klaar -> geen actieve quest meer (of je kunt hem bewaren als completed)
            activeQuest = null;
            stepIndex = -1;
        }

        UpdateQuestUI();
    }

    private Quest FindQuest(string questId)
    {
        for (int i = 0; i < quests.Count; i++)
        {
            if (quests[i] != null && quests[i].questId == questId)
                return quests[i];
        }
        return null;
    }

    private string GetFirstQuestOfferForNpc(string npcId)
    {
        foreach (var offer in npcOffers)
        {
            if (offer != null && offer.npcId == npcId && offer.questIdsToOffer != null && offer.questIdsToOffer.Count > 0)
                return offer.questIdsToOffer[0]; // eerste quest
        }
        return null;
    }

    private void UpdateQuestUI()
    {
        if (questText == null) return;

        if (activeQuest == null)
        {
            questText.text = "Quest:\nGeen actieve quest";
            return;
        }

        // Als hij nog bezig is:
        Step step = GetCurrentStep();
        if (step == null)
        {
            questText.text = $"Quest:\n{activeQuest.title}\n(Klaar)";
            return;
        }

        if (step.type == StepType.TalkToNpc)
        {
            questText.text =
                $"Quest:\n{activeQuest.title}\n" +
                $"Stap {stepIndex + 1}/{activeQuest.steps.Count}: Praat met {step.targetNpcId}";
        }
        else // CollectItem
        {
            int required = Mathf.Max(1, step.amount);
            questText.text =
                $"Quest:\n{activeQuest.title}\n" +
                $"Stap {stepIndex + 1}/{activeQuest.steps.Count}: Verzamel {step.itemId} ({collectedCount}/{required})";
        }
    }

    private void ShowNotification(string msg)
    {
        if (notificationText == null) return;

        if (notifRoutine != null) StopCoroutine(notifRoutine);
        notifRoutine = StartCoroutine(NotifRoutine(msg));
    }

    private IEnumerator NotifRoutine(string msg)
    {
        notificationText.gameObject.SetActive(true);
        notificationText.text = msg;

        yield return new WaitForSeconds(notificationDuration);

        HideNotification();
    }

    private void HideNotification()
    {
        if (notificationText == null) return;
        notificationText.gameObject.SetActive(false);
    }
}
