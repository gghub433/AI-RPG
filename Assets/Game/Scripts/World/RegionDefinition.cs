using System;
using UnityEngine;

namespace RebirthEclipse.World
{
    [Serializable]
    public class PointOfInterest
    {
        public string poiId;
        public string displayName;
        public Vector3 worldPosition;
        public bool isHidden; // revealed only through exploration/scanning
    }

    /// <summary>
    /// Metadata for one explorable region. The scene itself holds the actual geometry;
    /// this asset is what the world map, fast-travel menu, and quest system reference by id.
    /// </summary>
    [CreateAssetMenu(fileName = "NewRegion", menuName = "RebirthEclipse/Region Definition")]
    public class RegionDefinition : ScriptableObject
    {
        public string regionId;
        public string displayName;
        [TextArea(2, 6)] public string description;
        public string sceneName;
        public string requiredStoryFlagToUnlock;
        public int recommendedLevel;
        public PointOfInterest[] pointsOfInterest;
        public string[] uniqueMaterialIds;
    }
}
