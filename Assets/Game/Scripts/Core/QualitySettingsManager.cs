using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace RebirthEclipse.Core
{
    public enum QualityPreset { Low, Medium, High, Ultra }

    /// <summary>
    /// Applies one of four graphics presets (per design spec section 19/20: never default
    /// every phone to max settings). Reads/writes the URP asset's render scale and the
    /// built-in QualitySettings level, so a single toggle changes shadows, render scale,
    /// and post-processing budget together instead of leaving them inconsistent.
    /// </summary>
    public class QualitySettingsManager : MonoBehaviour
    {
        [SerializeField] private UniversalRenderPipelineAsset lowAsset;
        [SerializeField] private UniversalRenderPipelineAsset mediumAsset;
        [SerializeField] private UniversalRenderPipelineAsset highAsset;
        [SerializeField] private UniversalRenderPipelineAsset ultraAsset;

        public QualityPreset CurrentPreset { get; private set; } = QualityPreset.Medium;

        public void ApplyPreset(QualityPreset preset)
        {
            CurrentPreset = preset;
            QualitySettings.SetQualityLevel((int)preset, applyExpensiveChanges: true);

            UniversalRenderPipelineAsset asset = preset switch
            {
                QualityPreset.Low => lowAsset,
                QualityPreset.Medium => mediumAsset,
                QualityPreset.High => highAsset,
                QualityPreset.Ultra => ultraAsset,
                _ => mediumAsset
            };
            if (asset != null)
            {
                GraphicsSettings.renderPipelineAsset = asset;
            }

            Application.targetFrameRate = preset == QualityPreset.Low ? 30 : 60;
        }

        /// <summary>
        /// Heuristic for first-run: picks Medium by default, Low on devices with little
        /// system memory, so a fresh install on a budget phone doesn't start by stuttering.
        /// </summary>
        public QualityPreset RecommendPresetForDevice()
        {
            int systemMemoryMb = SystemInfo.systemMemorySize;
            if (systemMemoryMb <= 3072) return QualityPreset.Low;
            if (systemMemoryMb <= 6144) return QualityPreset.Medium;
            if (SystemInfo.graphicsMemorySize >= 6144) return QualityPreset.Ultra;
            return QualityPreset.High;
        }
    }
}
