using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RebirthEclipse.Core;

namespace RebirthEclipse.UI
{
    public struct SystemNotification
    {
        public string headerLine;   // e.g. "[НОВОЕ ДОСТИЖЕНИЕ]"
        public string bodyLine;
        public float displaySeconds;
    }

    /// <summary>
    /// Queues and displays "System message" popups (the bracketed [...] notification style
    /// from the design doc). Every notification here is pushed in response to a real
    /// GameEvents callback — level up, quest complete, item acquired, sync rate milestone —
    /// never fabricated for a cosmetic pop. Add new trigger points by subscribing another
    /// GameEvents handler here, not by calling Enqueue() from arbitrary UI code.
    /// </summary>
    public class SystemNotificationQueue : MonoBehaviour
    {
        [SerializeField] private TMP_Text headerText;
        [SerializeField] private TMP_Text bodyText;
        [SerializeField] private CanvasGroup panelGroup;
        [SerializeField] private Animator popAnimator;
        [SerializeField] private float defaultDisplaySeconds = 3f;
        [SerializeField] private float fadeSeconds = 0.25f;

        private readonly Queue<SystemNotification> queue = new Queue<SystemNotification>();
        private bool isShowing;

        private void OnEnable()
        {
            GameEvents.OnLevelUp += HandleLevelUp;
            GameEvents.OnQuestCompleted += HandleQuestCompleted;
            GameEvents.OnItemAcquired += HandleItemAcquired;
            GameEvents.OnClassAssigned += HandleClassAssigned;
            GameEvents.OnClassEvolutionStage += HandleClassEvolution;
            GameEvents.OnMemoryFragmentUnlocked += HandleMemoryFragment;
            GameEvents.OnRomanceUnlocked += HandleRomanceUnlocked;
        }

        private void OnDisable()
        {
            GameEvents.OnLevelUp -= HandleLevelUp;
            GameEvents.OnQuestCompleted -= HandleQuestCompleted;
            GameEvents.OnItemAcquired -= HandleItemAcquired;
            GameEvents.OnClassAssigned -= HandleClassAssigned;
            GameEvents.OnClassEvolutionStage -= HandleClassEvolution;
            GameEvents.OnMemoryFragmentUnlocked -= HandleMemoryFragment;
            GameEvents.OnRomanceUnlocked -= HandleRomanceUnlocked;
        }

        private void HandleLevelUp(int newLevel) =>
            Enqueue("[ПОВЫШЕНИЕ УРОВНЯ]", $"Вы достигли уровня {newLevel}.");

        private void HandleQuestCompleted(string questId) =>
            Enqueue("[ЗАДАНИЕ ВЫПОЛНЕНО]", questId);

        private void HandleItemAcquired(string itemId, int qty) =>
            Enqueue("[ПРЕДМЕТ ПОЛУЧЕН]", qty > 1 ? $"{itemId} x{qty}" : itemId);

        private void HandleClassAssigned(string classId) =>
            Enqueue("[КЛАСС ОПРЕДЕЛЁН]", classId);

        private void HandleClassEvolution(string classId, int stage) =>
            Enqueue("[ЭВОЛЮЦИЯ КЛАССА]", $"{classId}: этап {stage}");

        private void HandleMemoryFragment(string fragmentId) =>
            Enqueue("[ФРАГМЕНТ ПАМЯТИ ОБНАРУЖЕН]", fragmentId);

        private void HandleRomanceUnlocked(string npcId) =>
            Enqueue("[НОВАЯ СВЯЗЬ]", npcId);

        public void Enqueue(string header, string body, float duration = -1f)
        {
            queue.Enqueue(new SystemNotification
            {
                headerLine = header,
                bodyLine = body,
                displaySeconds = duration > 0f ? duration : defaultDisplaySeconds
            });
            if (!isShowing) StartCoroutine(ProcessQueue());
        }

        private IEnumerator ProcessQueue()
        {
            isShowing = true;
            while (queue.Count > 0)
            {
                var note = queue.Dequeue();
                headerText.text = note.headerLine;
                bodyText.text = note.bodyLine;
                popAnimator?.SetTrigger("Show");

                yield return Fade(0f, 1f);
                yield return new WaitForSeconds(note.displaySeconds);
                yield return Fade(1f, 0f);
            }
            isShowing = false;
        }

        private IEnumerator Fade(float from, float to)
        {
            float t = 0f;
            while (t < fadeSeconds)
            {
                t += Time.deltaTime;
                panelGroup.alpha = Mathf.Lerp(from, to, t / fadeSeconds);
                yield return null;
            }
            panelGroup.alpha = to;
        }
    }
}
