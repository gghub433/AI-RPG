using System;
using UnityEngine;

namespace RebirthEclipse.Relationships
{
    public enum RelationshipTrack { None, Friendship, Rivalry, Romance }

    /// <summary>
    /// Static profile data for one significant NPC (see Docs/Characters for the full written
    /// biography). Gameplay-relevant fields only — personality/history prose lives in the
    /// design docs, not duplicated here, so writers edit one place.
    /// </summary>
    [CreateAssetMenu(fileName = "NewNPC", menuName = "RebirthEclipse/NPC Definition")]
    public class NPCDefinition : ScriptableObject
    {
        public string npcId;
        public string displayName;
        public int age;
        public string faction;
        public bool isRomanceable;
        public bool isAdult = true; // romance content is gated on this flag being true; never bypassed
        public RelationshipTrack defaultTrack;

        [Header("Affinity thresholds")]
        public int friendshipThreshold = 30;
        public int trustedAllyThreshold = 70;
        public int romanceUnlockThreshold = 60;

        [TextArea(2, 6)] public string shortBio;
    }
}
