using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RebirthEclipse.Core;

namespace RebirthEclipse.Inventory
{
    /// <summary>
    /// Owns the player's item collection. Stackable items (consumables/materials) merge by
    /// itemId up to maxStack; non-stackable gear gets a unique instance id on creation so the
    /// same physical item can never be "found" twice — this is what keeps loot and quest-reward
    /// grants from being exploitable into infinite duplication (see InventoryTests).
    /// </summary>
    public class InventorySystem : MonoBehaviour
    {
        [SerializeField] private ItemDatabase itemDatabase;
        [SerializeField] private int maxDistinctSlots = 200;

        private readonly List<ItemStack> stacks = new List<ItemStack>();
        private readonly HashSet<string> knownInstanceIds = new HashSet<string>();

        public IReadOnlyList<ItemStack> Stacks => stacks;

        public ItemDefinition GetDefinition(string itemId) => itemDatabase != null ? itemDatabase.Find(itemId) : null;

        /// <summary>
        /// Adds a stackable item by id/quantity. Returns the quantity actually added
        /// (may be less than requested if slots are full) — callers must not assume success.
        /// </summary>
        public int AddStackable(string itemId, int quantity)
        {
            if (quantity <= 0) return 0;
            var def = GetDefinition(itemId);
            if (def == null) { Debug.LogWarning($"Unknown item id {itemId}"); return 0; }

            int remaining = quantity;
            int added = 0;

            // Fill existing stacks first.
            for (int i = 0; i < stacks.Count && remaining > 0; i++)
            {
                if (stacks[i].itemId != itemId) continue;
                int space = def.maxStack - stacks[i].quantity;
                if (space <= 0) continue;
                int moved = Mathf.Min(space, remaining);
                var s = stacks[i];
                s.quantity += moved;
                stacks[i] = s;
                remaining -= moved;
                added += moved;
            }

            // Create new stacks for the remainder, bounded by slot cap.
            while (remaining > 0 && stacks.Count < maxDistinctSlots)
            {
                int qty = Mathf.Min(def.maxStack, remaining);
                stacks.Add(new ItemStack { itemId = itemId, quantity = qty, uniqueInstanceId = string.Empty });
                remaining -= qty;
                added += qty;
            }

            if (added > 0) GameEvents.RaiseItemAcquired(itemId, added);
            return added;
        }

        /// <summary>
        /// Adds exactly one non-stackable gear instance with a server/save-assigned unique id.
        /// Rejects duplicate instance ids outright — this is the hard guarantee against
        /// double-granting the same drop (e.g. a quest-completion race or a reload exploit).
        /// </summary>
        public bool AddUniqueInstance(string itemId, string instanceId, StatBlock rolledBonus)
        {
            if (string.IsNullOrEmpty(instanceId)) instanceId = Guid.NewGuid().ToString("N");
            if (knownInstanceIds.Contains(instanceId)) return false;
            if (stacks.Count >= maxDistinctSlots) return false;

            knownInstanceIds.Add(instanceId);
            stacks.Add(new ItemStack { itemId = itemId, quantity = 1, uniqueInstanceId = instanceId, rolledBonus = rolledBonus });
            GameEvents.RaiseItemAcquired(itemId, 1);
            return true;
        }

        public bool RemoveStackable(string itemId, int quantity)
        {
            if (quantity <= 0) return true;
            int totalAvailable = stacks.Where(s => s.itemId == itemId).Sum(s => s.quantity);
            if (totalAvailable < quantity) return false;

            int toRemove = quantity;
            for (int i = stacks.Count - 1; i >= 0 && toRemove > 0; i--)
            {
                if (stacks[i].itemId != itemId) continue;
                var s = stacks[i];
                int removeFromThis = Mathf.Min(s.quantity, toRemove);
                s.quantity -= removeFromThis;
                toRemove -= removeFromThis;
                if (s.quantity <= 0) stacks.RemoveAt(i);
                else stacks[i] = s;
            }
            return true;
        }

        public bool RemoveUniqueInstance(string instanceId)
        {
            int idx = stacks.FindIndex(s => s.uniqueInstanceId == instanceId);
            if (idx < 0) return false;
            stacks.RemoveAt(idx);
            knownInstanceIds.Remove(instanceId);
            return true;
        }

        public int CountOf(string itemId) => stacks.Where(s => s.itemId == itemId).Sum(s => s.quantity);

        public bool HasMaterialsFor(ItemDefinition recipe)
        {
            if (recipe.requiredMaterialIds == null) return true;
            for (int i = 0; i < recipe.requiredMaterialIds.Length; i++)
            {
                int needed = i < recipe.requiredMaterialCounts.Length ? recipe.requiredMaterialCounts[i] : 1;
                if (CountOf(recipe.requiredMaterialIds[i]) < needed) return false;
            }
            return true;
        }

        public bool TryCraft(ItemDefinition recipe)
        {
            if (recipe == null || !recipe.isCraftable || !HasMaterialsFor(recipe)) return false;
            for (int i = 0; i < recipe.requiredMaterialIds.Length; i++)
            {
                int needed = i < recipe.requiredMaterialCounts.Length ? recipe.requiredMaterialCounts[i] : 1;
                RemoveStackable(recipe.requiredMaterialIds[i], needed);
            }
            if (recipe.maxStack > 1)
                AddStackable(recipe.itemId, 1);
            else
                AddUniqueInstance(recipe.itemId, Guid.NewGuid().ToString("N"), StatBlock.Zero);
            return true;
        }

        public List<ItemStack> ExportForSave() => new List<ItemStack>(stacks);

        public void LoadFromSave(List<ItemStack> savedStacks)
        {
            stacks.Clear();
            knownInstanceIds.Clear();
            foreach (var s in savedStacks)
            {
                stacks.Add(s);
                if (!string.IsNullOrEmpty(s.uniqueInstanceId)) knownInstanceIds.Add(s.uniqueInstanceId);
            }
        }
    }
}
