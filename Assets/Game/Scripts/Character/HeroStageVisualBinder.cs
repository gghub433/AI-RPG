using UnityEngine;
using RebirthEclipse.Core;
using RebirthEclipse.World;

namespace RebirthEclipse.Character
{
    /// <summary>
    /// Presentation-layer listener that swaps Arden's visible model when his hero stage
    /// changes. Keeps HeroStageController free of any art/prefab knowledge: the controller
    /// owns gameplay state and raises OnHeroStageChanged; this binder resolves the stage's
    /// armorSetId/weaponId against the ModelRegistry and instantiates the matching prefab.
    ///
    /// With an empty ModelRegistry (no art yet) this safely does nothing, so the character
    /// keeps whatever placeholder visual the scene already has.
    /// </summary>
    public class HeroStageVisualBinder : MonoBehaviour
    {
        [SerializeField] private HeroStageController heroStage;
        [SerializeField] private ModelRegistry modelRegistry;
        [SerializeField] private Transform modelAttachPoint; // where the body model is parented
        [SerializeField] private Transform weaponAttachPoint;

        private GameObject currentBody;
        private GameObject currentWeapon;

        private void OnEnable() => GameEvents.OnHeroStageChanged += HandleStageChanged;
        private void OnDisable() => GameEvents.OnHeroStageChanged -= HandleStageChanged;

        private void Start()
        {
            // Apply whatever the starting stage is (e.g. after a load) once references are set.
            if (heroStage != null) ApplyStage(heroStage.CurrentStageIndex);
        }

        private void HandleStageChanged(int stageIndex) => ApplyStage(stageIndex);

        private void ApplyStage(int stageIndex)
        {
            if (heroStage == null || modelRegistry == null) return;
            var stage = heroStage.CurrentStage;
            if (stage == null) return;

            SwapBody(stage.armorSetId);
            SwapWeapon(stage.weaponId);
            // auraVfxId is handled by a separate VFX binder; kept out of here on purpose.
        }

        private void SwapBody(string armorSetId)
        {
            if (string.IsNullOrEmpty(armorSetId) || modelAttachPoint == null) return;
            var prefab = modelRegistry.Resolve(armorSetId);
            if (prefab == null) return; // no art yet — leave the existing placeholder
            if (currentBody != null) Destroy(currentBody);
            currentBody = Instantiate(prefab, modelAttachPoint);
            currentBody.transform.localPosition = Vector3.zero;
            currentBody.transform.localRotation = Quaternion.identity;
        }

        private void SwapWeapon(string weaponId)
        {
            if (string.IsNullOrEmpty(weaponId) || weaponAttachPoint == null) return;
            var prefab = modelRegistry.Resolve(weaponId);
            if (prefab == null) return;
            if (currentWeapon != null) Destroy(currentWeapon);
            currentWeapon = Instantiate(prefab, weaponAttachPoint);
            currentWeapon.transform.localPosition = Vector3.zero;
            currentWeapon.transform.localRotation = Quaternion.identity;
        }
    }
}
