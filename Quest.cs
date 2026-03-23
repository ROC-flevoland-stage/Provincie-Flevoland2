using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Game.Quests
{
    public class Quest
    {
        public string Id { get; }
        public string Title { get; }
        public string Description { get; }

        private readonly List<IQuestObjective> _objectives;
        private int _index;

        public bool IsCompleted => _index >= _objectives.Count;
        public IQuestObjective? CurrentObjective => IsCompleted ? null : _objectives[_index];

        public event Action<Quest>? OnCompleted;

        public Quest(string id, string title, string description, List<IQuestObjective> objectives)
        {
            Id = id;
            Title = title;
            Description = description;
            _objectives = objectives;
            _index = 0;
        }

        public void StartCurrent(QuestContext ctx)
        {
            CurrentObjective?.Start(ctx);
        }


        public void Tick(QuestContext ctx)
        {
            if (IsCompleted) return;

            var obj = CurrentObjective;
            if (obj != null && obj.IsCompleted)
            {
                obj.Stop(ctx);
                _index++;

                if (IsCompleted) OnCompleted?.Invoke(this);
                else StartCurrent(ctx);
            }
        }
    }
}
