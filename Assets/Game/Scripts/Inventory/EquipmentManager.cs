using System.Collections.Generic;
using UnityEngine;
using RebirthEclipse.Character;
using RebirthEclipse.Core;

namespace RebirthEclipse.Inventory
{
    /// <summary>
    /// Tracks which item instance occupies each equipment slot and re-applies the combined
    /// gear bonus to CharacterStats whenever the loadout changes. Equipping always goes
    /// through the inventory (an item must exist there) so gear can't be "equipped" out of
    /// thin air.
    /// </summary>
    public class EquipmentManager : MonoBehaviour
    {
        [SerializeField] private InventorySystem inventory;
        [SerializeField] private CharacterStats stats;

        private readonly Dictionary<EquipmentSlot, ItemStack> equipped = new Dictionary<EquipmentSlot, ItemStack>();

        public bool TryEquip(string instanceId)
        {
            var stackList = inventory.Stacks;
            int idx = -1;
            for (int i = 0; i < stackList.Count; i++)
                if (stackList[i].uniqueInstanceId == instanceId) { idx = i; break; }
            if (idx < 0) return false;

            var stack = stackList[idx];
            var def = inventory.GetDefinition(stack.itemId);
            if (def == null || def.slot == EquipmentSlot.None) return false;

            // Two-handed weapons occupy both MainHand and OffHand.
            if (def.slot == EquipmentSlot.TwoHanded)
            {
                Unequip(EquipmentSlot.MainHand);
                Unequip(EquipmentSlot.OffHand);
                equipped[EquipmentSlot.MainHand] = stack;
            }
            else
            {
                Unequip(def.slot);
                equipped[def.slot] = stack;
            }

            RecomputeBonuses();
            GameEvents.RaiseItemEquipped(stack.itemId);
            return true;
        }

        public void Unequip(EquipmentSlot slot)
        {
            if (equipped.Remove(slot))
            {
                RecomputeBonuses();
            }
        }

        public ItemStack? GetEquipped(EquipmentSlot slot) =>
            equipped.TryGetValue(slot, out var s) ? s : (ItemStack?)null;

        private void RecomputeBonuses()
        {
            stats.ResetEquipmentBonuses();
            StatBlock totalFlat = StatBlock.Zero;
            DerivedStats totalDerived = default;

            foreach (var kv in equipped)
            {
                var def = inventory.GetDefinition(kv.Value.itemId);
                if (def == null) continue;
                totalFlat += def.flatStatBonus;
                totalFlat += kv.Value.rolledBonus;
                totalDerived = AddDerived(totalDerived, def.derivedStatBonus);
            }
            stats.AddEquipmentBonus(totalFlat, totalDerived);
        }

        private static DerivedStats AddDerived(DerivedStats a, DerivedStats b)
        {
            a.maxHealth += b.maxHealth; a.maxMana += b.maxMana; a.maxStamina += b.maxStamina;
            a.physicalPower += b.physicalPower; a.magicPower += b.magicPower;
            a.physicalDefense += b.physicalDefense; a.magicDefense += b.magicDefense; a.curseDefense += b.curseDefense;
            a.critChance += b.critChance; a.critDamageMult += b.critDamageMult;
            a.attackSpeedMult += b.attackSpeedMult; a.cooldownReduction += b.cooldownReduction;
            a.manaCostReduction += b.manaCostReduction; a.moveSpeed += b.moveSpeed;
            return a;
        }

        public Dictionary<EquipmentSlot, string> ExportForSave()
        {
            var export = new Dictionary<EquipmentSlot, string>();
            foreach (var kv in equipped) export[kv.Key] = kv.Value.uniqueInstanceId;
            return export;
        }

        public void LoadFromSave(Dictionary<EquipmentSlot, string> saved)
        {
            equipped.Clear();
            foreach (var kv in saved)
            {
                if (string.IsNullOrEmpty(kv.Value)) continue;
                TryEquip(kv.Value);
            }
        }
    }
}
