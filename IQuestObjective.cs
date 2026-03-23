using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

public interface IQuestObjective
{
    string Description { get; }
    bool IsCompleted { get; }

    void Start(QuestContext context);
    void Stop(QuestContext context);
}
