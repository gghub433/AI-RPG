using UnityEngine;
using UnityEngine.SceneManagement;

namespace RebirthEclipse.Core
{
    /// <summary>
    /// Top-level scene/session orchestrator. Survives scene loads, owns async scene
    /// transitions, and is the single place that knows which "mode" the game is in
    /// (menu, gameplay, cutscene, paused). Does not hold gameplay data itself —
    /// that lives in Progression/Inventory/Quest managers, which this class wires together
    /// on each scene load.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public enum GameState { Boot, MainMenu, Loading, Gameplay, Cutscene, Paused, Dialogue }

        public GameState CurrentState { get; private set; } = GameState.Boot;

        [SerializeField] private string bootSceneName = "Boot";
        [SerializeField] private string mainMenuSceneName = "MainMenu";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Application.targetFrameRate = 60;
        }

        public void SetState(GameState newState)
        {
            if (CurrentState == newState) return;
            CurrentState = newState;
            Time.timeScale = (newState == GameState.Paused) ? 0f : 1f;
        }

        public void LoadSceneAsync(string sceneName, GameState stateAfterLoad)
        {
            SetState(GameState.Loading);
            StartCoroutine(LoadRoutine(sceneName, stateAfterLoad));
        }

        private System.Collections.IEnumerator LoadRoutine(string sceneName, GameState stateAfterLoad)
        {
            var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            op.allowSceneActivation = false;
            while (op.progress < 0.9f)
                yield return null;
            op.allowSceneActivation = true;
            yield return op;
            SetState(stateAfterLoad);
        }

        public void QuitToMainMenu()
        {
            LoadSceneAsync(mainMenuSceneName, GameState.MainMenu);
        }

        public void TogglePause()
        {
            if (CurrentState == GameState.Paused)
            {
                SetState(GameState.Gameplay);
            }
            else if (CurrentState == GameState.Gameplay)
            {
                SetState(GameState.Paused);
            }
        }
    }
}
