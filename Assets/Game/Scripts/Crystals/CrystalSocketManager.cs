using System.Collections.Generic;
using UnityEngine;
using RebirthEclipse.Core;
using RebirthEclipse.Inventory;

namespace RebirthEclipse.Crystals
{
    /// <summary>
    /// Tracks which crystals are socketed into which specific gear instances (keyed by the
    /// item's unique instance id, so two copies of the same sword can carry different
    /// crystals). Enforces the socket-count limit declared on the ItemDefinition and the
    /// per-crystal compatibility gate. Socketing consumes the crystal from the inventory;
    /// unsocketing returns it — crystals are never created or destroyed by this system, which
    /// is what keeps them on the same anti-duplication footing as the rest of the economy.
    /// </summary>
    public class CrystalSocketManager : MonoBehaviour
    {
        [SerializeField] private InventorySystem inventory;
        [SerializeField] private CrystalDatabase crystalDatabase;
        [SerializeField] private EquipmentManager equipment; // to recompute stats after a change

        // itemInstanceId -> list of socketed crystal ids
        private readonly Dictionary<string, List<string>> socketed = new Dictionary<string, List<string>>();

        private static readonly string[] NoCrystals = System.Array.Empty<string>();

        public IReadOnlyList<string> GetSocketedCrystals(string itemInstanceId) =>
            socketed.TryGetValue(itemInstanceId, out var list) ? (IReadOnlyList<string>)list : NoCrystals;

        public int FreeSockets(string itemInstanceId)
        {
            var def = FindItemDefinitionForInstance(itemInstanceId);
            if (def == null) return 0;
            int used = socketed.TryGetValue(itemInstanceId, out var list) ? list.Count : 0;
            return Mathf.Max(0, def.socketCount - used);
        }

        /// <summary>
        /// Sockets a crystal (consumed from inventory) into a gear instance.
        /// Fails — changing nothing — if the item is unknown, has no free socket, or the
        /// crystal is restricted and the gear is not aligned to its school.
        /// </summary>
        public bool TrySocket(string itemInstanceId, string crystalId)
        {
            var itemDef = FindItemDefinitionForInstance(itemInstanceId);
            if (itemDef == null || itemDef.socketCount <= 0) return false;
            if (FreeSockets(itemInstanceId) <= 0) return false;

            var crystal = crystalDatabase != null ? crystalDatabase.Find(crystalId) : null;
            if (crystal == null) return false;

            if (crystal.restrictedToAlignedGear && !GearAcceptsSchool(itemDef, crystal.school))
                return false;

            // The crystal must actually be in the player's inventory to socket it.
            if (!inventory.RemoveStackable(crystalId, 1)) return false;

            if (!socketed.TryGetValue(itemInstanceId, out var list))
            {
                list = new List<string>();
                socketed[itemInstanceId] = list;
            }
            list.Add(crystalId);

            equipment?.SendMessage("RecomputeBonuses", SendMessageOptions.DontRequireReceiver);
            GameEvents.RaiseCrystalSocketed(itemInstanceId, crystalId);
            return true;
        }

        /// <summary>Removes the crystal at the given socket index and returns it to inventory.</summary>
        public bool TryUnsocket(string itemInstanceId, int socketIndex)
        {
            if (!socketed.TryGetValue(itemInstanceId, out var list)) return false;
            if (socketIndex < 0 || socketIndex >= list.Count) return false;

            string crystalId = list[socketIndex];
            list.RemoveAt(socketIndex);
            inventory.AddStackable(crystalId, 1);
            equipment?.SendMessage("RecomputeBonuses", SendMessageOptions.DontRequireReceiver);
            return true;
        }

        private bool GearAcceptsSchool(ItemDefinition itemDef, CrystalSchool school)
        {
            // Convention: an item declares aligned schools via tags in its description-less
            // compatibility field. For the data-light first pass, weapons accept any school
            // and armor accepts defensive schools; EclipseCore only accepts onto Unique gear.
            if (school == CrystalSchool.EclipseCore)
                return itemDef.rarity == ItemRarity.Unique || itemDef.rarity == ItemRarity.Mythical;
            return true;
        }

        private ItemDefinition FindItemDefinitionForInstance(string itemInstanceId)
        {
            foreach (var stack in inventory.Stacks)
            {
                if (stack.uniqueInstanceId == itemInstanceId)
                    return inventory.GetDefinition(stack.itemId);
            }
            return null;
        }

        public Dictionary<string, List<string>> ExportForSave() =>
            new Dictionary<string, List<string>>(socketed);

        public void LoadFromSave(Dictionary<string, List<string>> saved)
        {
            socketed.Clear();
            foreach (var kv in saved) socketed[kv.Key] = new List<string>(kv.Value);
        }
    }
}
