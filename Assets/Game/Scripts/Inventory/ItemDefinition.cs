using System;
using UnityEngine;
using RebirthEclipse.Character;

namespace RebirthEclipse.Inventory
{
    public enum ItemRarity { Common, Uncommon, Rare, Epic, Legendary, Mythical, Unique }

    public enum ItemCategory { Weapon, Armor, Accessory, Consumable, Material, QuestItem, CraftingComponent }

    public enum EquipmentSlot { None, MainHand, OffHand, TwoHanded, Head, Chest, Legs, Ring1, Ring2, Amulet }

    /// <summary>
    /// Data-driven item. New gear/consumables are added as new assets of this type — no
    /// item-specific code is ever required, matching the "every item has real stats" rule:
    /// nothing here is cosmetic-only unless explicitly a cosmetic item.
    /// </summary>
    [CreateAssetMenu(fileName = "NewItem", menuName = "RebirthEclipse/Item Definition")]
    public class ItemDefinition : ScriptableObject
    {
        public string itemId;
        public string displayName;
        [TextArea(2, 6)] public string description;
        public ItemCategory category;
        public ItemRarity rarity;
        public EquipmentSlot slot;
        public Sprite icon;
        public int maxStack = 1;
        public int sellValue;

        [Header("Stat Bonuses (gear only)")]
        public StatBlock flatStatBonus;
        public DerivedStats derivedStatBonus;

        [Header("Consumable Effect")]
        public float healAmount;
        public float restoreMana;
        public float restoreStamina;
        public string buffEffectId;
        public float buffDurationSeconds;

        [Header("Crafting")]
        public bool isCraftable;
        public string[] requiredMaterialIds;
        public int[] requiredMaterialCounts;

        [Header("Random Affix Roll (gear)")]
        public bool hasRandomAffixes;
        [Range(0, 3)] public int maxRandomAffixes = 2;
    }

    [Serializable]
    public struct ItemStack
    {
        public string itemId;
        public int quantity;
        // Rolled affix values for this specific instance (gear with hasRandomAffixes).
        // Empty for stackable consumables/materials.
        public StatBlock rolledBonus;
        public string uniqueInstanceId;
    }
}
