using System.Collections.Generic;
using UnityEngine;
using RebirthEclipse.Quests;

namespace RebirthEclipse.World
{
    /// <summary>
    /// Tracks which regions and fast-travel points the player has unlocked. A region unlocks
    /// only when its story-flag requirement is actually set on QuestManager — never just by
    /// walking near it — so the world map can't reveal content ahead of the story.
    /// </summary>
    public class RegionProgressTracker : MonoBehaviour
    {
        [SerializeField] private QuestManager quests;
        [SerializeField] private RegionDefinition[] allRegions;

        private readonly HashSet<string> unlockedRegions = new HashSet<string>();
        private readonly HashSet<string> unlockedFastTravel = new HashSet<string>();

        public IReadOnlyCollection<string> UnlockedRegionIds => unlockedRegions;
        public IReadOnlyCollection<string> UnlockedFastTravelIds => unlockedFastTravel;

        private void OnEnable() => Core.GameEvents.OnStoryFlagSet += HandleStoryFlagSet;
        private void OnDisable() => Core.GameEvents.OnStoryFlagSet -= HandleStoryFlagSet;

        private void HandleStoryFlagSet(string flagId)
        {
            foreach (var region in allRegions)
            {
                if (region.requiredStoryFlagToUnlock == flagId)
                {
                    UnlockRegion(region.regionId);
                }
            }
        }

        public void UnlockRegion(string regionId)
        {
            if (unlockedRegions.Add(regionId))
            {
                UnlockFastTravel($"{regionId}_entrance");
            }
        }

        public bool IsRegionUnlocked(string regionId) => unlockedRegions.Contains(regionId);

        public void UnlockFastTravel(string pointId) => unlockedFastTravel.Add(pointId);
        public bool IsFastTravelUnlocked(string pointId) => unlockedFastTravel.Contains(pointId);

        public void LoadFromSave(IEnumerable<string> regions, IEnumerable<string> fastTravel)
        {
            unlockedRegions.Clear();
            foreach (var r in regions) unlockedRegions.Add(r);
            unlockedFastTravel.Clear();
            foreach (var f in fastTravel) unlockedFastTravel.Add(f);
        }
    }
}
