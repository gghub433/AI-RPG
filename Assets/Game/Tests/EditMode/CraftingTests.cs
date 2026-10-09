using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using RebirthEclipse.Crafting;
using RebirthEclipse.Economy;
using RebirthEclipse.Inventory;

namespace RebirthEclipse.Tests
{
    public class CraftingTests
    {
        private GameObject go;
        private InventorySystem inventory;
        private CurrencyWallet wallet;
        private CraftingStation station;
        private ItemDatabase itemDb;
        private RecipeDatabase recipeDb;

        private ItemDefinition ore, ingot, sword;
        private CraftingRecipe createIngot, upgradeSword;

        [SetUp]
        public void SetUp()
        {
            ore = MakeItem("mat_ore", 99, ItemCategory.Material);
            ingot = MakeItem("mat_ingot", 99, ItemCategory.Material);
            sword = MakeItem("sword_craft", 1, ItemCategory.Weapon);

            itemDb = ScriptableObject.CreateInstance<ItemDatabase>();
            itemDb.allItems = new[] { ore, ingot, sword };

            createIngot = ScriptableObject.CreateInstance<CraftingRecipe>();
            createIngot.recipeId = "recipe_ingot";
            createIngot.kind = CraftKind.Create;
            createIngot.producedItemId = "mat_ingot";
            createIngot.materials = new[] { new MaterialRequirement { itemId = "mat_ore", count = 3 } };
            createIngot.currencyCostCopper = 50;

            upgradeSword = ScriptableObject.CreateInstance<CraftingRecipe>();
            upgradeSword.recipeId = "recipe_upgrade_sword";
            upgradeSword.kind = CraftKind.Upgrade;
            upgradeSword.materials = new[] { new MaterialRequirement { itemId = "mat_ingot", count = 1 } };
            upgradeSword.baseSuccessChance = 1f;
            upgradeSword.successChanceFalloffPerLevel = 0.5f; // level 0 -> 100%, level 1 -> 50%, level 2 -> 0%
            upgradeSword.safeFailure = true;
            upgradeSword.maxUpgradeLevel = 10;

            recipeDb = ScriptableObject.CreateInstance<RecipeDatabase>();
            recipeDb.allRecipes = new[] { createIngot, upgradeSword };

            go = new GameObject("CraftTest");
            inventory = go.AddComponent<InventorySystem>();
            wallet = go.AddComponent<CurrencyWallet>();
            station = go.AddComponent<CraftingStation>();

            SetPrivate(inventory, "itemDatabase", itemDb);
            SetPrivate(station, "recipeDatabase", recipeDb);
            SetPrivate(station, "inventory", inventory);
            SetPrivate(station, "wallet", wallet);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(go);
            foreach (var o in new Object[] { ore, ingot, sword, itemDb, recipeDb, createIngot, upgradeSword })
                Object.DestroyImmediate(o);
        }

        private ItemDefinition MakeItem(string id, int stack, ItemCategory cat)
        {
            var i = ScriptableObject.CreateInstance<ItemDefinition>();
            i.itemId = id; i.maxStack = stack; i.category = cat;
            return i;
        }

        private static void SetPrivate(object t, string f, object v) =>
            t.GetType().GetField(f, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(t, v);

        [Test]
        public void Create_WithoutEnoughMaterials_FailsAndConsumesNothing()
        {
            inventory.AddStackable("mat_ore", 2); // need 3
            wallet.AddCopper(1000);
            var outcome = station.Craft("recipe_ingot");
            Assert.AreEqual(CraftOutcome.InsufficientMaterials, outcome);
            Assert.AreEqual(2, inventory.CountOf("mat_ore"));
            Assert.AreEqual(1000, wallet.TotalCopper, "Currency must not be spent on a rejected craft.");
        }

        [Test]
        public void Create_WithoutEnoughCurrency_FailsAndConsumesNothing()
        {
            inventory.AddStackable("mat_ore", 3);
            wallet.AddCopper(10); // need 50
            var outcome = station.Craft("recipe_ingot");
            Assert.AreEqual(CraftOutcome.InsufficientCurrency, outcome);
            Assert.AreEqual(3, inventory.CountOf("mat_ore"), "Materials must not be consumed when currency check fails.");
        }

        [Test]
        public void Create_Success_ConsumesInputs_AndProducesOutput()
        {
            inventory.AddStackable("mat_ore", 3);
            wallet.AddCopper(50);
            var outcome = station.Craft("recipe_ingot");
            Assert.AreEqual(CraftOutcome.Success, outcome);
            Assert.AreEqual(0, inventory.CountOf("mat_ore"));
            Assert.AreEqual(1, inventory.CountOf("mat_ingot"));
            Assert.AreEqual(0, wallet.TotalCopper);
        }

        [Test]
        public void Upgrade_GuaranteedSuccess_RaisesLevel()
        {
            inventory.AddStackable("mat_ingot", 1);
            inventory.AddUniqueInstance("sword_craft", "sword-1", default);
            station.RandomRoll = () => 0f; // always below any positive chance

            var outcome = station.Craft("recipe_upgrade_sword", upgradeTargetInstanceId: "sword-1");
            Assert.AreEqual(CraftOutcome.Success, outcome);
            Assert.AreEqual(1, station.UpgradeLevels.GetLevel("sword-1"));
        }

        [Test]
        public void Upgrade_ForcedFailure_WithSafeFailure_KeepsLevel_ButConsumesMaterials()
        {
            inventory.AddStackable("mat_ingot", 2);
            inventory.AddUniqueInstance("sword_craft", "sword-1", default);
            station.UpgradeLevels.SetLevel("sword-1", 1);
            station.RandomRoll = () => 0.99f; // above the 50% chance at level 1 -> failure

            var outcome = station.Craft("recipe_upgrade_sword", upgradeTargetInstanceId: "sword-1");
            Assert.AreEqual(CraftOutcome.FailureProduced, outcome);
            Assert.AreEqual(1, station.UpgradeLevels.GetLevel("sword-1"), "safeFailure must not drop the upgrade level.");
            Assert.AreEqual(1, inventory.CountOf("mat_ingot"), "An upgrade attempt always consumes its material, success or fail.");
        }

        [Test]
        public void Upgrade_AtMaxLevel_IsRejected()
        {
            inventory.AddStackable("mat_ingot", 1);
            inventory.AddUniqueInstance("sword_craft", "sword-1", default);
            station.UpgradeLevels.SetLevel("sword-1", upgradeSword.maxUpgradeLevel);

            var outcome = station.Craft("recipe_upgrade_sword", upgradeTargetInstanceId: "sword-1");
            Assert.AreEqual(CraftOutcome.MaxLevelReached, outcome);
            Assert.AreEqual(1, inventory.CountOf("mat_ingot"), "A rejected max-level upgrade consumes nothing.");
        }
    }
}
