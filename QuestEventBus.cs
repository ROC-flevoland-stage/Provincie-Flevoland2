using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace Game.Quests
{
    public class QuestEventBus
    {
        public event Action<string>? OnTalkedToNpc;
        public event Action<string, int>? OnItemCollected;

        public void RaiseTalkedToNpc(string npcId) => OnTalkedToNpc?.Invoke(npcId);
        public void RaiseItemCollected(string itemId, int amount) => OnItemCollected?.Invoke(itemId, amount);
    }
}
