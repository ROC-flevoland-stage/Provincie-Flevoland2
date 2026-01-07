using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Quests
{
    public class CollectItemObjective : IQuestObjective
    {
        public string Description { get; }
        public bool IsCompleted { get; private set; }

        private readonly string _itemId;
        private readonly int _required;
        private int _current;

        public CollectItemObjective(string itemId, int requiredAmount, string description)
        {
            _itemId = itemId;
            _required = requiredAmount;
            Description = description;
        }

        public void Start(QuestContext context)
        {
            IsCompleted = false;
            _current = 0;
            context.EventBus.OnItemCollected += Handle;
        }

        public void Stop(QuestContext context)
        {
            context.EventBus.OnItemCollected -= Handle;
        }

        private void Handle(string itemId, int amount)
        {
            if (IsCompleted || itemId != _itemId) return;

            _current += amount;
            if (_current >= _required)
                IsCompleted = true;
        }
    }
}
