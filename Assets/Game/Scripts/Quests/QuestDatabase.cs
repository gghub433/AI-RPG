using UnityEngine;

namespace RebirthEclipse.Quests
{
    [CreateAssetMenu(fileName = "QuestDatabase", menuName = "RebirthEclipse/Quest Database")]
    public class QuestDatabase : ScriptableObject
    {
        public QuestDefinition[] allQuests;
        private System.Collections.Generic.Dictionary<string, QuestDefinition> lookup;

        public QuestDefinition Find(string questId)
        {
            if (lookup == null)
            {
                lookup = new System.Collections.Generic.Dictionary<string, QuestDefinition>();
                foreach (var q in allQuests)
                    if (q != null) lookup[q.questId] = q;
            }
            lookup.TryGetValue(questId, out var def);
            return def;
        }
    }
}
