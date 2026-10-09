using UnityEngine;
using UnityEngine.UI;
using RebirthEclipse.Core;
using RebirthEclipse.SaveSystem;

namespace RebirthEclipse.UI
{
    /// <summary>
    /// Wires the main menu's real buttons to real actions: New Game clears no existing save
    /// and starts the prologue scene; Continue only enables when a save/autosave actually
    /// exists; Settings opens the settings panel bound to GameSettings fields that actually
    /// affect gameplay (volume sliders drive AudioManager mixers, not just UI state).
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private Button continueButton;
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject slotSelectPanel;
        [SerializeField] private string prologueSceneName = "Prologue";

        private void Start()
        {
            bool hasAnySave = SaveManager.SlotExists(0) || SaveManager.SlotExists(1) || SaveManager.SlotExists(2)
                               || SaveManager.SlotExists(3) || SaveManager.SlotExists(4);
            continueButton.interactable = hasAnySave;

            newGameButton.onClick.AddListener(StartNewGame);
            continueButton.onClick.AddListener(OpenSlotSelect);
            settingsButton.onClick.AddListener(() => settingsPanel.SetActive(true));
            quitButton.onClick.AddListener(Application.Quit);
        }

        private void StartNewGame()
        {
            GameManager.Instance?.LoadSceneAsync(prologueSceneName, GameManager.GameState.Cutscene);
        }

        private void OpenSlotSelect()
        {
            slotSelectPanel.SetActive(true);
        }
    }
}
