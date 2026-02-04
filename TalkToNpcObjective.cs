using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TalkToNpcObjective : IQuestObjective
{
    public string Description { get; }
    public bool IsCompleted { get; private set; }

    private readonly string _targetNpcId;

    public TalkToNpcObjective(string targetNpcId, string description)
    {
        _targetNpcId = targetNpcId;
        Description = description;
    }

    public void Start(QuestContext context)
    {
        IsCompleted = false;
        context.EventBus.OnTalkedToNpc += Handle;
    }

    public void Stop(QuestContext context)
    {
        context.EventBus.OnTalkedToNpc -= Handle;
    }

    private void Handle(string npcId)
    {
        if (!IsCompleted && npcId == _targetNpcId)
            IsCompleted = true;
    }
}
