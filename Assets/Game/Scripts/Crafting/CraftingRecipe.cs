using System;
using UnityEngine;

namespace RebirthEclipse.Crafting
{
    [Serializable]
    public struct MaterialRequirement
    {
        public string itemId;
        public int count;
    }

    public enum CraftKind { Create, Upgrade }

    /// <summary>
    /// A craft or upgrade recipe. "Create" produces a new item from materials + currency;
    /// "Upgrade" consumes materials to raise an existing item's upgrade level, and may carry
    /// a failure chance at higher levels (Gaarret Stone's forge gets riskier the further you
    /// push a weapon). All costs are data; CraftingStation is the only code that executes them.
    /// </summary>
    [CreateAssetMenu(fileName = "NewRecipe", menuName = "RebirthEclipse/Crafting Recipe")]
    public class CraftingRecipe : ScriptableObject
    {
        public string recipeId;
        public string displayName;
        public CraftKind kind = CraftKind.Create;

        [Header("Inputs")]
        public MaterialRequirement[] materials;
        public string[] requiredCrystalIds; // consumed crystals, if any
        public long currencyCostCopper;
        public int eclipseTokenCost;

        [Header("Create output")]
        public string producedItemId;        // for CraftKind.Create

        [Header("Upgrade settings")]
        [Tooltip("For CraftKind.Upgrade: base success chance at upgrade level 0, 0..1.")]
        [Range(0f, 1f)] public float baseSuccessChance = 1f;
        [Tooltip("Success chance lost per existing upgrade level (diminishing safety).")]
        [Range(0f, 0.2f)] public float successChanceFalloffPerLevel = 0.08f;
        [Tooltip("If true, a failed upgrade consumes materials but does NOT reduce the item's level (safe fail). If false, failure can drop a level.")]
        public bool safeFailure = true;
        public int maxUpgradeLevel = 10;

        public float SuccessChanceAtLevel(int currentLevel)
        {
            if (kind != CraftKind.Upgrade) return 1f;
            return Mathf.Clamp01(baseSuccessChance - successChanceFalloffPerLevel * currentLevel);
        }
    }

    [CreateAssetMenu(fileName = "RecipeDatabase", menuName = "RebirthEclipse/Recipe Database")]
    public class RecipeDatabase : ScriptableObject
    {
        public CraftingRecipe[] allRecipes;
        private System.Collections.Generic.Dictionary<string, CraftingRecipe> lookup;

        public CraftingRecipe Find(string recipeId)
        {
            if (lookup == null)
            {
                lookup = new System.Collections.Generic.Dictionary<string, CraftingRecipe>();
                foreach (var r in allRecipes)
                    if (r != null) lookup[r.recipeId] = r;
            }
            lookup.TryGetValue(recipeId, out var def);
            return def;
        }
    }
}
