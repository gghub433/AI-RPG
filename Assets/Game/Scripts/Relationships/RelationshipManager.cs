using System.Collections.Generic;
using UnityEngine;
using RebirthEclipse.Core;

namespace RebirthEclipse.Relationships
{
    /// <summary>
    /// Tracks numeric affinity per NPC and raises romance-unlock events the moment a
    /// romanceable NPC's affinity crosses its threshold — never unlocked by player age alone;
    /// NPCDefinition.isAdult gates it as a hard requirement checked here.
    /// </summary>
    public class RelationshipManager : MonoBehaviour
    {
        [SerializeField] private NPCDatabase npcDatabase;

        private readonly Dictionary<string, int> affinity = new Dictionary<string, int>();
        private readonly HashSet<string> romanceUnlocked = new HashSet<string>();

        public int GetAffinity(string npcId) => affinity.TryGetValue(npcId, out var v) ? v : 0;

        public void ChangeAffinity(string npcId, int delta)
        {
            if (string.IsNullOrEmpty(npcId) || delta == 0) return;
            int current = GetAffinity(npcId);
            int updated = Mathf.Clamp(current + delta, -100, 100);
            affinity[npcId] = updated;
            GameEvents.RaiseAffinityChanged(npcId, updated);

            var def = npcDatabase != null ? npcDatabase.Find(npcId) : null;
            if (def != null && def.isRomanceable && def.isAdult &&
                updated >= def.romanceUnlockThreshold && !romanceUnlocked.Contains(npcId))
            {
                romanceUnlocked.Add(npcId);
                GameEvents.RaiseRomanceUnlocked(npcId);
            }
        }

        public bool IsRomanceUnlocked(string npcId) => romanceUnlocked.Contains(npcId);

        public Dictionary<string, int> ExportForSave() => new Dictionary<string, int>(affinity);

        public void LoadFromSave(Dictionary<string, int> saved, IEnumerable<string> savedRomanceUnlocked)
        {
            affinity.Clear();
            foreach (var kv in saved) affinity[kv.Key] = kv.Value;
            romanceUnlocked.Clear();
            foreach (var id in savedRomanceUnlocked) romanceUnlocked.Add(id);
        }
    }

    [CreateAssetMenu(fileName = "NPCDatabase", menuName = "RebirthEclipse/NPC Database")]
    public class NPCDatabase : ScriptableObject
    {
        public NPCDefinition[] allNpcs;
        private Dictionary<string, NPCDefinition> lookup;

        public NPCDefinition Find(string npcId)
        {
            if (lookup == null)
            {
                lookup = new Dictionary<string, NPCDefinition>();
                foreach (var n in allNpcs)
                    if (n != null) lookup[n.npcId] = n;
            }
            lookup.TryGetValue(npcId, out var def);
            return def;
        }
    }
}
