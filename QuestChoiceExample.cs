using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

public class QuestChoiceExample : MonoBehaviour
{
    public void ChooseQuest(string questId)
    {
        bool started = QuestManager.Instance.StartQuest(questId);
        Debug.Log(started ? $"Quest gestart: {questId}" : $"Quest kon niet starten: {questId}");
    }
}
