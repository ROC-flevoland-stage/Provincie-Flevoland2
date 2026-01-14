using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

public class NpcQuestLogic : MonoBehaviour
{
    [SerializeField] private string npcId = NpcIds.Npc1; // alleen ID, geen quests in inspector

    public void Interact()
    {
        // 1) altijd talk-event voor objectives
        QuestManager.Instance.Bus.RaiseTalkedToNpc(npcId);

        // 2) ALLE quest-logica alleen via code:
        if (npcId == NpcIds.Npc1)
        {
            // voorbeeld: NPC1 geeft keuze tussen 2 quests (via code)
            OfferChoiceAndAutoPickExample();
        }
        else if (npcId == NpcIds.Npc2)
        {
            // NPC2 start niks, maar praat-event kan objectives voltooien
            Debug.Log("NPC2: Hey!");
        }
    }

    private void OfferChoiceAndAutoPickExample()
    {
        // Hier hoort jouw UI te zitten. Omdat je “via code” wil, demo ik met auto-pick:
        // Stel: als TalkChain nog niet gestart is, start die, anders start collect apples.
        if (!QuestManager.Instance.IsActive(QuestIds.TalkChain) && !QuestManager.Instance.IsCompleted(QuestIds.TalkChain))
        {
            QuestManager.Instance.StartQuest(QuestIds.TalkChain);
            Debug.Log("NPC1: Quest gestart (TalkChain).");
        }
        else if (!QuestManager.Instance.IsActive(QuestIds.CollectApples) && !QuestManager.Instance.IsCompleted(QuestIds.CollectApples))
        {
            QuestManager.Instance.StartQuest(QuestIds.CollectApples);
            Debug.Log("NPC1: Quest gestart (CollectApples).");
        }
        else
        {
            Debug.Log("NPC1: Ik heb geen nieuwe quests voor je.");
        }
    }
}
