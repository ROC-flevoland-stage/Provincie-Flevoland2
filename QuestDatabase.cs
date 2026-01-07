using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;



namespace Game.Quests
{
    public static class QuestDatabase
    {
        private static readonly Dictionary<string, Func<Quest>> _factories = new();

        public static void RegisterAll()
        {
            _factories.Clear();

            Register(QuestIds.TalkChain, () =>
                new Quest(
                    QuestIds.TalkChain,
                    "De Boodschap",
                    "Praat met NPC2 en ga terug naar NPC1.",
                    new List<IQuestObjective>
                    {
                        new TalkToNpcObjective(NpcIds.Npc2, "Praat met NPC2."),
                        new TalkToNpcObjective(NpcIds.Npc1, "Ga terug naar NPC1.")
                    }
                )
            );

            Register(QuestIds.CollectApples, () =>
                new Quest(
                    QuestIds.CollectApples,
                    "Appels voor de markt",
                    "Verzamel 5 appels.",
                    new List<IQuestObjective>
                    {
                        new CollectItemObjective(ItemIds.Apple, 5, "Verzamel 5 appels.")
                    }
                )
            );
        }

        private static void Register(string id, Func<Quest> factory) => _factories[id] = factory;

        public static Quest Create(string id)
        {
            if (!_factories.TryGetValue(id, out var f))
                throw new Exception($"Quest '{id}' is niet geregistreerd.");
            return f();
        }
    }
}
