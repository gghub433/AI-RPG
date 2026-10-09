using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using RebirthEclipse.Crystals;
using RebirthEclipse.Inventory;

namespace RebirthEclipse.Tests
{
    public class CrystalSocketTests
    {
        private GameObject go;
        private InventorySystem inventory;
        private CrystalSocketManager sockets;
        private ItemDatabase itemDb;
        private CrystalDatabase crystalDb;
        private ItemDefinition socketedSword;   // 2 sockets
        private ItemDefinition noSocketDagger;   // 0 sockets
        private CrystalDefinition flameCrystal;

        [SetUp]
        public void SetUp()
        {
            socketedSword = ScriptableObject.CreateInstance<ItemDefinition>();
            socketedSword.itemId = "sword_socketed";
            socketedSword.maxStack = 1;
            socketedSword.socketCount = 2;
            socketedSword.rarity = ItemRarity.Epic;

            noSocketDagger = ScriptableObject.CreateInstance<ItemDefinition>();
            noSocketDagger.itemId = "dagger_plain";
            noSocketDagger.maxStack = 1;
            noSocketDagger.socketCount = 0;

            var flameItem = ScriptableObject.CreateInstance<ItemDefinition>();
            flameItem.itemId = "crystal_flame";
            flameItem.maxStack = 20;
            flameItem.category = ItemCategory.Material;

            itemDb = ScriptableObject.CreateInstance<ItemDatabase>();
            itemDb.allItems = new[] { socketedSword, noSocketDagger, flameItem };

            flameCrystal = ScriptableObject.CreateInstance<CrystalDefinition>();
            flameCrystal.crystalId = "crystal_flame";
            flameCrystal.school = CrystalSchool.FlameCore;

            crystalDb = ScriptableObject.CreateInstance<CrystalDatabase>();
            crystalDb.allCrystals = new[] { flameCrystal };

            go = new GameObject("SocketTest");
            inventory = go.AddComponent<InventorySystem>();
            sockets = go.AddComponent<CrystalSocketManager>();

            SetPrivate(inventory, "itemDatabase", itemDb);
            SetPrivate(sockets, "inventory", inventory);
            SetPrivate(sockets, "crystalDatabase", crystalDb);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(go);
            foreach (var o in new Object[] { socketedSword, noSocketDagger, itemDb, crystalDb, flameCrystal })
                Object.DestroyImmediate(o);
        }

        private static void SetPrivate(object target, string field, object value) =>
            target.GetType().GetField(field, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(target, value);

        [Test]
        public void Socket_IntoItemWithNoSockets_Fails()
        {
            inventory.AddUniqueInstance("dagger_plain", "dagger-1", default);
            inventory.AddStackable("crystal_flame", 1);
            Assert.IsFalse(sockets.TrySocket("dagger-1", "crystal_flame"));
            Assert.AreEqual(1, inventory.CountOf("crystal_flame"), "A failed socket must not consume the crystal.");
        }

        [Test]
        public void Socket_ConsumesCrystalFromInventory_OnSuccess()
        {
            inventory.AddUniqueInstance("sword_socketed", "sword-1", default);
            inventory.AddStackable("crystal_flame", 1);
            Assert.IsTrue(sockets.TrySocket("sword-1", "crystal_flame"));
            Assert.AreEqual(0, inventory.CountOf("crystal_flame"));
            Assert.AreEqual(1, sockets.GetSocketedCrystals("sword-1").Count);
        }

        [Test]
        public void Socket_BeyondSocketCount_Fails()
        {
            inventory.AddUniqueInstance("sword_socketed", "sword-1", default);
            inventory.AddStackable("crystal_flame", 3); // sword only has 2 sockets

            Assert.IsTrue(sockets.TrySocket("sword-1", "crystal_flame"));
            Assert.IsTrue(sockets.TrySocket("sword-1", "crystal_flame"));
            Assert.IsFalse(sockets.TrySocket("sword-1", "crystal_flame"), "Third socket on a 2-socket item must fail.");
            Assert.AreEqual(0, sockets.FreeSockets("sword-1"));
            Assert.AreEqual(1, inventory.CountOf("crystal_flame"), "The un-socketable third crystal stays in inventory.");
        }

        [Test]
        public void Unsocket_ReturnsCrystalToInventory()
        {
            inventory.AddUniqueInstance("sword_socketed", "sword-1", default);
            inventory.AddStackable("crystal_flame", 1);
            sockets.TrySocket("sword-1", "crystal_flame");

            Assert.IsTrue(sockets.TryUnsocket("sword-1", 0));
            Assert.AreEqual(1, inventory.CountOf("crystal_flame"));
            Assert.AreEqual(0, sockets.GetSocketedCrystals("sword-1").Count);
        }

        [Test]
        public void SaveLoadRoundTrip_PreservesSocketedCrystals()
        {
            inventory.AddUniqueInstance("sword_socketed", "sword-1", default);
            inventory.AddStackable("crystal_flame", 1);
            sockets.TrySocket("sword-1", "crystal_flame");

            var exported = sockets.ExportForSave();

            var go2 = new GameObject("SocketReload");
            var sockets2 = go2.AddComponent<CrystalSocketManager>();
            SetPrivate(sockets2, "inventory", inventory);
            SetPrivate(sockets2, "crystalDatabase", crystalDb);
            sockets2.LoadFromSave(exported);

            Assert.AreEqual(1, sockets2.GetSocketedCrystals("sword-1").Count);
            Object.DestroyImmediate(go2);
        }
    }
}
