using UnityEngine;

namespace RebirthEclipse.World
{
    /// <summary>
    /// Base class for anything the player can interact with in the world: doors, chests,
    /// NPCs that aren't full dialogue triggers, lootable corpses, levers. Concrete behavior
    /// lives in subclasses/derived components, not in branching if/else here.
    /// </summary>
    public abstract class InteractableObject : MonoBehaviour
    {
        [SerializeField] protected string interactionPrompt = "Interact";
        [SerializeField] protected float interactionRange = 2.5f;
        [SerializeField] protected bool oneTimeUse;
        protected bool hasBeenUsed;

        public string Prompt => interactionPrompt;
        public float Range => interactionRange;
        public bool IsAvailable => !(oneTimeUse && hasBeenUsed);

        public void Interact(GameObject interactor)
        {
            if (!IsAvailable) return;
            OnInteract(interactor);
            if (oneTimeUse) hasBeenUsed = true;
        }

        protected abstract void OnInteract(GameObject interactor);
    }

    public class LootableContainer : InteractableObject
    {
        [SerializeField] private string lootTableId;
        [SerializeField] private Inventory.InventorySystem targetInventory;

        protected override void OnInteract(GameObject interactor)
        {
            // Resolved by LootTableService at runtime (data-driven; see Docs/Design/LootTables.md).
            LootTableService.GrantLoot(lootTableId, targetInventory);
        }
    }

    public class FastTravelPoint : InteractableObject
    {
        [SerializeField] private string fastTravelPointId;
        [SerializeField] private RegionProgressTracker progressTracker;

        private void Start()
        {
            progressTracker?.UnlockFastTravel(fastTravelPointId);
        }

        protected override void OnInteract(GameObject interactor)
        {
            // Hands off to a scene-level FastTravelMenuController (UI layer) to pick a destination.
        }
    }

    /// <summary>Static entry point so InteractableObject subclasses don't need a direct loot-table manager reference.</summary>
    public static class LootTableService
    {
        public static void GrantLoot(string lootTableId, Inventory.InventorySystem inventory)
        {
            if (string.IsNullOrEmpty(lootTableId) || inventory == null) return;
            // Real implementation resolves lootTableId against a LootTableDatabase asset and
            // rolls rarity-weighted drops; stubbed here pending that data asset being authored.
        }
    }
}
