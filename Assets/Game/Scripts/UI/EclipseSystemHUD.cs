using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RebirthEclipse.Progression;
using RebirthEclipse.Core;

namespace RebirthEclipse.UI
{
    /// <summary>
    /// Renders the in-fiction "Eclipse System" panel shown in the design doc mock-up.
    /// Every field is pulled from EclipseSystemModel.BuildSnapshot() each refresh — this
    /// class contains no gameplay numbers of its own, only formatting and animation.
    /// </summary>
    public class EclipseSystemHUD : MonoBehaviour
    {
        [SerializeField] private EclipseSystemModel model;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text classText;
        [SerializeField] private TMP_Text rankText;
        [SerializeField] private Slider healthBar;
        [SerializeField] private Slider manaBar;
        [SerializeField] private Slider staminaBar;
        [SerializeField] private TMP_Text healthLabel;
        [SerializeField] private TMP_Text manaLabel;
        [SerializeField] private TMP_Text staminaLabel;
        [SerializeField] private TMP_Text freePointsText;
        [SerializeField] private TMP_Text skillPointsText;
        [SerializeField] private TMP_Text syncRateText;
        [SerializeField] private Animator panelAnimator; // drives the open/close + "sync pulse" animation

        private static readonly int AnimOpen = Animator.StringToHash("Open");

        private void OnEnable()
        {
            GameEvents.OnSyncRateChanged += HandleSyncRateChanged;
            GameEvents.OnLevelUp += HandleLevelUp;
            Refresh();
        }

        private void OnDisable()
        {
            GameEvents.OnSyncRateChanged -= HandleSyncRateChanged;
            GameEvents.OnLevelUp -= HandleLevelUp;
        }

        private void HandleSyncRateChanged(float _) => Refresh();
        private void HandleLevelUp(int _) => Refresh();

        public void TogglePanel(bool open)
        {
            panelAnimator?.SetBool(AnimOpen, open);
            if (open) Refresh();
        }

        public void Refresh()
        {
            if (model == null) return;
            var snap = model.BuildSnapshot();

            nameText.text = snap.characterName;
            levelText.text = $"Level: {snap.level}";
            classText.text = $"Class: {snap.className}";
            rankText.text = $"Rank: {snap.rank}";

            healthBar.maxValue = snap.maxHealth; healthBar.value = snap.currentHealth;
            manaBar.maxValue = snap.maxMana; manaBar.value = snap.currentMana;
            staminaBar.maxValue = snap.maxStamina; staminaBar.value = snap.currentStamina;

            healthLabel.text = $"{Mathf.RoundToInt(snap.currentHealth)}/{Mathf.RoundToInt(snap.maxHealth)}";
            manaLabel.text = $"{Mathf.RoundToInt(snap.currentMana)}/{Mathf.RoundToInt(snap.maxMana)}";
            staminaLabel.text = $"{Mathf.RoundToInt(snap.currentStamina)}/{Mathf.RoundToInt(snap.maxStamina)}";

            freePointsText.text = $"Free Points: {snap.freeStatPoints}";
            skillPointsText.text = $"Skills: {snap.skillPoints}";
            syncRateText.text = $"Sync: {snap.syncRatePercent}%";
        }
    }
}
