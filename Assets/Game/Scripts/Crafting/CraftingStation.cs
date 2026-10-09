using System;
using UnityEngine;
using RebirthEclipse.Core;
using RebirthEclipse.Economy;
using RebirthEclipse.Inventory;

namespace RebirthEclipse.Crafting
{
    public enum CraftOutcome { Success, FailureProduced, InsufficientMaterials, InsufficientCurrency, InvalidRecipe, MaxLevelReached }

    /// <summary>
    /// Executes recipes against the player's inventory + wallet. The defining guarantee:
    /// inputs are validated in full BEFORE anything is consumed, so a craft either fully
    /// succeeds (or fully resolves an upgrade attempt) or changes nothing at all — no
    /// half-spent materials on a rejected craft. Upgrade attempts always consume their
    /// materials once started (that is the gamble), but the "can I even attempt this" checks
    /// still run first.
    /// </summary>
    public class CraftingStation : MonoBehaviour
    {
        [SerializeField] private RecipeDatabase recipeDatabase;
        [SerializeField] private InventorySystem inventory;
        [SerializeField] private CurrencyWallet wallet;

        /// <summary>Deterministic RNG seam so tests can force success/failure.</summary>
        public Func<float> RandomRoll = () => UnityEngine.Random.value;

        /// <summary>
        /// Authoritative per-instance upgrade levels for the whole game, persisted by
        /// SaveController. Callers may pass their own store to Craft (tests do), but the
        /// default is this shared one.
        /// </summary>
        public UpgradeableItemLevels UpgradeLevels { get; } = new UpgradeableItemLevels();

        public CraftOutcome Craft(string recipeId, UpgradeableItemLevels upgradeLevels = null, string upgradeTargetInstanceId = null)
        {
            upgradeLevels ??= UpgradeLevels;
            var recipe = recipeDatabase != null ? recipeDatabase.Find(recipeId) : null;
            if (recipe == null) return Fail(recipeId, CraftOutcome.InvalidRecipe);

            // --- Validate everything first ---
            if (!HasAllMaterials(recipe)) return Fail(recipeId, CraftOutcome.InsufficientMaterials);
            if (!HasAllCrystals(recipe)) return Fail(recipeId, CraftOutcome.InsufficientMaterials);

            if (recipe.eclipseTokenCost > 0 && wallet != null && wallet.EclipseTokens < recipe.eclipseTokenCost)
                return Fail(recipeId, CraftOutcome.InsufficientCurrency);
            if (recipe.currencyCostCopper > 0 && wallet != null && !wallet.CanAfford(recipe.currencyCostCopper))
                return Fail(recipeId, CraftOutcome.InsufficientCurrency);

            if (recipe.kind == CraftKind.Upgrade && upgradeLevels != null && upgradeTargetInstanceId != null)
            {
                int level = upgradeLevels.GetLevel(upgradeTargetInstanceId);
                if (level >= recipe.maxUpgradeLevel) return Fail(recipeId, CraftOutcome.MaxLevelReached);
            }

            // --- Consume inputs (validated, so these all succeed) ---
            ConsumeInputs(recipe);

            if (recipe.kind == CraftKind.Create)
            {
                ProduceCreatedItem(recipe);
                GameEvents.RaiseCraftResolved(recipeId, true);
                return CraftOutcome.Success;
            }

            // Upgrade: roll against success chance at the item's current level.
            int currentLevel = upgradeLevels != null && upgradeTargetInstanceId != null
                ? upgradeLevels.GetLevel(upgradeTargetInstanceId) : 0;
            float chance = recipe.SuccessChanceAtLevel(currentLevel);
            bool success = RandomRoll() < chance;

            if (success)
            {
                upgradeLevels?.SetLevel(upgradeTargetInstanceId, currentLevel + 1);
                GameEvents.RaiseCraftResolved(recipeId, true);
                return CraftOutcome.Success;
            }

            if (!recipe.safeFailure && currentLevel > 0)
            {
                upgradeLevels?.SetLevel(upgradeTargetInstanceId, currentLevel - 1);
            }
            GameEvents.RaiseCraftResolved(recipeId, false);
            return CraftOutcome.FailureProduced;
        }

        private bool HasAllMaterials(CraftingRecipe recipe)
        {
            if (recipe.materials == null) return true;
            foreach (var m in recipe.materials)
                if (inventory.CountOf(m.itemId) < m.count) return false;
            return true;
        }

        private bool HasAllCrystals(CraftingRecipe recipe)
        {
            if (recipe.requiredCrystalIds == null) return true;
            foreach (var cId in recipe.requiredCrystalIds)
                if (inventory.CountOf(cId) < 1) return false;
            return true;
        }

        private void ConsumeInputs(CraftingRecipe recipe)
        {
            if (recipe.materials != null)
                foreach (var m in recipe.materials) inventory.RemoveStackable(m.itemId, m.count);
            if (recipe.requiredCrystalIds != null)
                foreach (var cId in recipe.requiredCrystalIds) inventory.RemoveStackable(cId, 1);
            if (wallet != null)
            {
                if (recipe.currencyCostCopper > 0) wallet.TrySpendCopper(recipe.currencyCostCopper);
                if (recipe.eclipseTokenCost > 0) wallet.TrySpendEclipseTokens(recipe.eclipseTokenCost);
            }
        }

        private void ProduceCreatedItem(CraftingRecipe recipe)
        {
            var def = inventory.GetDefinition(recipe.producedItemId);
            if (def == null) return;
            if (def.maxStack > 1)
                inventory.AddStackable(recipe.producedItemId, 1);
            else
                inventory.AddUniqueInstance(recipe.producedItemId, Guid.NewGuid().ToString("N"), Character.StatBlock.Zero);
        }

        private CraftOutcome Fail(string recipeId, CraftOutcome reason)
        {
            GameEvents.RaiseCraftResolved(recipeId, false);
            return reason;
        }
    }

    /// <summary>
    /// Per-instance upgrade levels (e.g. "+3 Voidrend"). Separate from the item asset so each
    /// physical instance tracks its own upgrade progress; serialized by SaveController.
    /// </summary>
    [Serializable]
    public class UpgradeableItemLevels
    {
        private readonly System.Collections.Generic.Dictionary<string, int> levels =
            new System.Collections.Generic.Dictionary<string, int>();

        public int GetLevel(string instanceId) => levels.TryGetValue(instanceId, out var l) ? l : 0;
        public void SetLevel(string instanceId, int level) => levels[instanceId] = Mathf.Max(0, level);
        public System.Collections.Generic.Dictionary<string, int> Export() =>
            new System.Collections.Generic.Dictionary<string, int>(levels);
        public void Load(System.Collections.Generic.Dictionary<string, int> saved)
        {
            levels.Clear();
            foreach (var kv in saved) levels[kv.Key] = kv.Value;
        }
    }
}
