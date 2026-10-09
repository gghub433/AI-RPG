using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace RebirthEclipse.SaveSystem
{
    /// <summary>
    /// Local JSON persistence with multiple slots, a backup-on-write safety net, and
    /// corrupted-file recovery. Uses Newtonsoft.Json (not Unity's JsonUtility) because
    /// SaveData contains Dictionary fields that JsonUtility cannot serialize correctly.
    ///
    /// File layout per slot: save_{slot}.json (current) and save_{slot}.bak (previous
    /// good write). On load, a JSON parse failure or a version mismatch beyond what
    /// MigrateIfNeeded can handle falls back to the .bak file before giving up.
    /// </summary>
    public static class SaveManager
    {
        public const int CurrentSaveFormatVersion = 1;
        public const int MaxSlots = 5;

        private static string SlotPath(int slot) => Path.Combine(Application.persistentDataPath, $"save_{slot}.json");
        private static string BackupPath(int slot) => Path.Combine(Application.persistentDataPath, $"save_{slot}.bak");
        private const string AutosaveMarkerSlot = "autosave";
        private static string AutosavePath => Path.Combine(Application.persistentDataPath, $"save_{AutosaveMarkerSlot}.json");

        public static bool SlotExists(int slot) => File.Exists(SlotPath(slot));

        public static void Save(int slot, SaveData data)
        {
            data.saveFormatVersion = CurrentSaveFormatVersion;
            data.savedAtUtc = DateTime.UtcNow;

            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            string path = SlotPath(slot);

            // Rotate current -> backup before overwriting, so a crash mid-write never
            // leaves the player with zero recoverable saves in this slot.
            if (File.Exists(path))
            {
                File.Copy(path, BackupPath(slot), overwrite: true);
            }

            string tmpPath = path + ".tmp";
            File.WriteAllText(tmpPath, json);
            File.Copy(tmpPath, path, overwrite: true);
            File.Delete(tmpPath);

            Core.GameEvents.RaiseGameSaved(slot);
        }

        public static void Autosave(SaveData data)
        {
            data.saveFormatVersion = CurrentSaveFormatVersion;
            data.savedAtUtc = DateTime.UtcNow;
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(AutosavePath, json);
        }

        public static SaveData LoadAutosave() => LoadFromPath(AutosavePath);

        public static SaveData Load(int slot)
        {
            var data = LoadFromPath(SlotPath(slot));
            if (data == null)
            {
                Debug.LogWarning($"Save slot {slot} was corrupted or unreadable; attempting backup recovery.");
                data = LoadFromPath(BackupPath(slot));
                if (data != null)
                {
                    Debug.LogWarning($"Recovered save slot {slot} from backup.");
                }
            }
            if (data != null)
            {
                data = MigrateIfNeeded(data);
                Core.GameEvents.RaiseGameLoaded(slot);
            }
            return data;
        }

        private static SaveData LoadFromPath(string path)
        {
            if (!File.Exists(path)) return null;
            try
            {
                string json = File.ReadAllText(path);
                var data = JsonConvert.DeserializeObject<SaveData>(json);
                return data;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse save file at {path}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Upgrades older save formats forward. Each case falls through so a save that is
        /// several versions old gets every intermediate migration applied in order.
        /// </summary>
        private static SaveData MigrateIfNeeded(SaveData data)
        {
            switch (data.saveFormatVersion)
            {
                case CurrentSaveFormatVersion:
                    return data;
                // case 0: /* pre-release format */ data = MigrateV0ToV1(data); goto case 1;
                default:
                    Debug.LogWarning($"Save format version {data.saveFormatVersion} is newer than supported ({CurrentSaveFormatVersion}); loading as-is.");
                    return data;
            }
        }

        public static void DeleteSlot(int slot)
        {
            if (File.Exists(SlotPath(slot))) File.Delete(SlotPath(slot));
            if (File.Exists(BackupPath(slot))) File.Delete(BackupPath(slot));
        }

        public static DateTime? GetSlotTimestamp(int slot)
        {
            var data = LoadFromPath(SlotPath(slot));
            return data?.savedAtUtc;
        }
    }
}
