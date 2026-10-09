#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace RebirthEclipse.EditorTools
{
    /// <summary>
    /// Applies mobile-friendly import settings to every model dropped under
    /// Assets/Game/Art/Models/ (where Tools/tripo_generate.py and manual Tripo3D exports
    /// land). Runs automatically on import so artists/scripts don't have to remember to
    /// tune each GLB/FBX by hand. Textures under the same tree get ASTC-friendly,
    /// mip-mapped settings sized for Android.
    ///
    /// Kept deliberately conservative: it only touches assets under the managed folder,
    /// and only sets fields that are safe defaults — anything project-specific (colliders,
    /// per-character rig overrides) is still done per-prefab.
    /// </summary>
    public class TripoModelPostprocessor : AssetPostprocessor
    {
        private const string ManagedRoot = "Assets/Game/Art/Models/";

        private bool IsManaged => assetPath != null && assetPath.StartsWith(ManagedRoot);

        private void OnPreprocessModel()
        {
            if (!IsManaged) return;
            var importer = (ModelImporter)assetImporter;

            // Mesh: compress and drop CPU-side copy (mobile memory) unless a mesh needs
            // runtime read access (none of ours do by default).
            importer.meshCompression = ModelImporterMeshCompression.Medium;
            importer.isReadable = false;
            importer.optimizeMeshPolygons = true;
            importer.optimizeMeshVertices = true;
            importer.weldVertices = true;

            // Characters/enemies/bosses are flagged rig:true in the manifest; default any
            // humanoid-ish model under Characters/Enemies/Bosses to Humanoid rig so Mecanim
            // retargeting works. Props/weapons/crystals/armor stay rig-less.
            bool wantsRig = assetPath.Contains("/Characters/") || assetPath.Contains("/Enemies/")
                            || assetPath.Contains("/Bosses/");
            importer.animationType = wantsRig ? ModelImporterAnimationType.Human : ModelImporterAnimationType.None;

            importer.importBlendShapes = assetPath.Contains("/Characters/"); // facial expressions only for cast
            importer.importCameras = false;
            importer.importLights = false;

            // Scale/units: GLB from Tripo is metric; keep useFileScale on so 1 unit = 1 m.
            importer.useFileScale = true;
        }

        private void OnPreprocessTexture()
        {
            if (!IsManaged) return;
            var importer = (TextureImporter)assetImporter;
            importer.mipmapEnabled = true;
            importer.streamingMipmaps = true;

            var androidSettings = new TextureImporterPlatformSettings
            {
                name = "Android",
                overridden = true,
                maxTextureSize = 1024,
                format = TextureImporterFormat.ASTC_6x6,
                compressionQuality = (int)TextureCompressionQuality.Normal
            };
            importer.SetPlatformTextureSettings(androidSettings);
        }
    }
}
#endif
