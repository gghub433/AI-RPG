using UnityEngine;

namespace RebirthEclipse.Inventory
{
    /// <summary>
    /// Flat registry of every ItemDefinition asset in the game, built by the editor tool
    /// (see Scripts/Editor/ItemDatabaseBuilder.cs) so runtime lookup by id is O(1) instead of
    /// a Resources.Load per item.
    /// </summary>
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "RebirthEclipse/Item Database")]
    public class ItemDatabase : ScriptableObject
    {
        public ItemDefinition[] allItems;

        private System.Collections.Generic.Dictionary<string, ItemDefinition> lookup;

        public ItemDefinition Find(string itemId)
        {
            if (lookup == null)
            {
                lookup = new System.Collections.Generic.Dictionary<string, ItemDefinition>();
                foreach (var item in allItems)
                {
                    if (item != null && !string.IsNullOrEmpty(item.itemId))
                        lookup[item.itemId] = item;
                }
            }
            lookup.TryGetValue(itemId, out var def);
            return def;
        }
    }
}
