using System;
using UnityEngine;
using RebirthEclipse.Character;
using RebirthEclipse.Core;

namespace RebirthEclipse.Crystals
{
    /// <summary>
    /// The seven crystal schools. Mirrors the magic/damage schools so a socketed crystal's
    /// school lines up with the DamageType it empowers; EclipseCore is the rarest and the
    /// only one tied to Arden's own progression rather than being found/bought freely.
    /// </summary>
    public enum CrystalSchool { FlameCore, FrostShard, StormCrystal, Earthheart, SpiritGem, VoidCrystal, EclipseCore }

    public enum CrystalQuality { Flawed, Standard, Radiant, Perfect }

    /// <summary>
    /// Data-driven magic crystal. Socketing one into a weapon/armor applies its
    /// derived-stat bonus and, on weapons, tags outgoing hits with its school's DamageType.
    /// Compatibility is a hard gate: a crystal only sockets into gear whose
    /// <see cref="ItemCompatibilityMask"/> allows its school, so (for example) a VoidCrystal
    /// can be restricted to Abyss-aligned gear rather than droppable into anything.
    /// </summary>
    [CreateAssetMenu(fileName = "NewCrystal", menuName = "RebirthEclipse/Crystal Definition")]
    public class CrystalDefinition : ScriptableObject
    {
        public string crystalId;
        public string displayName;
        [TextArea(2, 5)] public string description;
        public CrystalSchool school;
        public CrystalQuality quality = CrystalQuality.Standard;

        [Header("Socket Effect")]
        public DerivedStats socketBonus;
        [Tooltip("If true and socketed into a weapon, outgoing hits adopt this school's damage type.")]
        public bool overridesWeaponDamageType = true;

        [Header("Compatibility")]
        [Tooltip("Gear must declare it accepts this school (see CrystalSocketManager.TrySocket).")]
        public bool restrictedToAlignedGear;

        /// <summary>Maps a crystal school to the damage type its empowered hits deal.</summary>
        public DamageType ToDamageType() => school switch
        {
            CrystalSchool.FlameCore => DamageType.Magical,
            CrystalSchool.FrostShard => DamageType.Magical,
            CrystalSchool.StormCrystal => DamageType.Magical,
            CrystalSchool.Earthheart => DamageType.Physical,
            CrystalSchool.SpiritGem => DamageType.Magical,
            CrystalSchool.VoidCrystal => DamageType.Void,
            CrystalSchool.EclipseCore => DamageType.Void,
            _ => DamageType.Physical
        };

        /// <summary>Quality multiplies the raw socket bonus so one asset family scales cleanly.</summary>
        public float QualityMultiplier() => quality switch
        {
            CrystalQuality.Flawed => 0.6f,
            CrystalQuality.Standard => 1.0f,
            CrystalQuality.Radiant => 1.5f,
            CrystalQuality.Perfect => 2.2f,
            _ => 1.0f
        };
    }

    [CreateAssetMenu(fileName = "CrystalDatabase", menuName = "RebirthEclipse/Crystal Database")]
    public class CrystalDatabase : ScriptableObject
    {
        public CrystalDefinition[] allCrystals;
        private System.Collections.Generic.Dictionary<string, CrystalDefinition> lookup;

        public CrystalDefinition Find(string crystalId)
        {
            if (lookup == null)
            {
                lookup = new System.Collections.Generic.Dictionary<string, CrystalDefinition>();
                foreach (var c in allCrystals)
                    if (c != null) lookup[c.crystalId] = c;
            }
            lookup.TryGetValue(crystalId, out var def);
            return def;
        }
    }
}
