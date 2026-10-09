using System;
using System.Collections.Generic;
using UnityEngine;

namespace RebirthEclipse.World
{
    /// <summary>
    /// Maps a model KEY (the same ids used in Assets/Game/Art/model_manifest.json and by
    /// gameplay data like HeroStage.armorSetId / EnemyArchetype.enemyId) to an actual prefab.
    /// This is the single seam between "code/data that refers to a model by name" and "the
    /// Tripo3D-generated art". Populate it in the Inspector by dragging in the prefabs built
    /// from the imported GLBs; nothing else in the game loads models by path.
    ///
    /// Until the art exists, Resolve() simply returns null and callers fall back to whatever
    /// placeholder they already use — so the game runs with an empty registry.
    /// </summary>
    [CreateAssetMenu(fileName = "ModelRegistry", menuName = "RebirthEclipse/Model Registry")]
    public class ModelRegistry : ScriptableObject
    {
        [Serializable]
        public struct Entry
        {
            public string modelKey;   // must match a manifest key, e.g. "char_arden_eclipse_sovereign"
            public GameObject prefab;
        }

        [SerializeField] private Entry[] entries = Array.Empty<Entry>();

        private Dictionary<string, GameObject> lookup;

        private void BuildLookup()
        {
            lookup = new Dictionary<string, GameObject>();
            foreach (var e in entries)
            {
                if (!string.IsNullOrEmpty(e.modelKey) && e.prefab != null)
                    lookup[e.modelKey] = e.prefab;
            }
        }

        /// <summary>Returns the prefab for a model key, or null if not registered yet.</summary>
        public GameObject Resolve(string modelKey)
        {
            if (string.IsNullOrEmpty(modelKey)) return null;
            if (lookup == null) BuildLookup();
            lookup.TryGetValue(modelKey, out var prefab);
            return prefab;
        }

        public bool Has(string modelKey) => Resolve(modelKey) != null;

        /// <summary>Instantiates the resolved prefab as a child of parent, or returns null.</summary>
        public GameObject Spawn(string modelKey, Transform parent)
        {
            var prefab = Resolve(modelKey);
            return prefab != null ? UnityEngine.Object.Instantiate(prefab, parent) : null;
        }
    }
}
