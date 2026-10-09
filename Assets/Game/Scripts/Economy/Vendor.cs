using System;
using System.Collections.Generic;
using UnityEngine;
using RebirthEclipse.Inventory;
using RebirthEclipse.Relationships;

namespace RebirthEclipse.Economy
{
    [Serializable]
    public class VendorStockEntry
    {
        public string itemId;
        public long basePriceCopper;
        public int quantityAvailable;    // -1 = unlimited
        public bool costsEclipseTokens;  // premium goods
        public int eclipseTokenPrice;
        public string requiredStoryFlag; // gated unique stock
    }

    /// <summary>
    /// A merchant. Buy/sell prices scale with the player's standing: higher affinity/faction
    /// reputation lowers buy prices and raises sell values, within clamped bounds, so
    /// relationships have a concrete economic payoff. Limited-stock entries decrement on
    /// purchase and do not silently refill within a session (restock is a separate,
    /// time/flag-driven concern), preventing a single rare item from being bought infinitely.
    /// </summary>
    public class Vendor : MonoBehaviour
    {
        [SerializeField] private string vendorId;
        [SerializeField] private string affinityNpcId; // whose standing affects pricing (optional)
        [SerializeField] private VendorStockEntry[] stock;
        [SerializeField, Range(0f, 0.5f)] private float maxDiscountAtMaxReputation = 0.2f;
        [SerializeField, Range(0f, 1f)] private float sellbackFraction = 0.4f; // base fraction of buy price when selling to vendor

        public IReadOnlyList<VendorStockEntry> Stock => stock;

        public long GetBuyPrice(VendorStockEntry entry, RelationshipManager relationships)
        {
            float repFactor = ReputationFactor(relationships); // 0..1
            float discount = maxDiscountAtMaxReputation * repFactor;
            return (long)Mathf.Round(entry.basePriceCopper * (1f - discount));
        }

        public long GetSellValue(ItemDefinition item, RelationshipManager relationships)
        {
            float repFactor = ReputationFactor(relationships);
            // Selling improves toward (but never reaches) parity as reputation rises.
            float fraction = Mathf.Lerp(sellbackFraction, Mathf.Min(0.75f, sellbackFraction + 0.3f), repFactor);
            return (long)Mathf.Round(item.sellValue * fraction);
        }

        private float ReputationFactor(RelationshipManager relationships)
        {
            if (relationships == null || string.IsNullOrEmpty(affinityNpcId)) return 0f;
            return Mathf.Clamp01(relationships.GetAffinity(affinityNpcId) / 100f);
        }

        public bool TryBuy(VendorStockEntry entry, CurrencyWallet wallet, InventorySystem inventory, RelationshipManager relationships)
        {
            int idx = Array.IndexOf(stock, entry);
            if (idx < 0) return false;
            if (entry.quantityAvailable == 0) return false;

            var def = inventory.GetDefinition(entry.itemId);
            if (def == null) return false;

            if (entry.costsEclipseTokens)
            {
                if (!wallet.TrySpendEclipseTokens(entry.eclipseTokenPrice)) return false;
            }
            else
            {
                long price = GetBuyPrice(entry, relationships);
                if (!wallet.TrySpendCopper(price)) return false;
            }

            // Grant the item: stackables merge, non-stackables get a fresh unique instance.
            if (def.maxStack > 1)
                inventory.AddStackable(entry.itemId, 1);
            else
                inventory.AddUniqueInstance(entry.itemId, Guid.NewGuid().ToString("N"), Character.StatBlock.Zero);

            if (entry.quantityAvailable > 0) entry.quantityAvailable--;
            return true;
        }

        public bool TrySellStackable(string itemId, int quantity, CurrencyWallet wallet, InventorySystem inventory, RelationshipManager relationships)
        {
            var def = inventory.GetDefinition(itemId);
            if (def == null) return false;
            if (!inventory.RemoveStackable(itemId, quantity)) return false;
            wallet.AddCopper(GetSellValue(def, relationships) * quantity);
            return true;
        }
    }
}
