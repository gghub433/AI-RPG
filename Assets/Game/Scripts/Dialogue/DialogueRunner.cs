using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RebirthEclipse.Core;
using RebirthEclipse.Quests;
using RebirthEclipse.Relationships;

namespace RebirthEclipse.Dialogue
{
    /// <summary>
    /// Drives one active conversation. The UI layer (DialogueUIController) only ever calls
    /// Begin / SelectChoice / Advance and reads CurrentNode / CurrentChoices — all branching
    /// logic lives here so writers/designers only touch DialogueDefinition data.
    /// </summary>
    public class DialogueRunner : MonoBehaviour
    {
        [SerializeField] private QuestManager questManager;
        [SerializeField] private RelationshipManager relationships;

        private DialogueDefinition currentDialogue;
        public DialogueNode CurrentNode { get; private set; }
        public bool IsActive { get; private set; }

        public void Begin(DialogueDefinition dialogue)
        {
            currentDialogue = dialogue;
            IsActive = true;
            GoToNode(dialogue.startNodeId);
            if (GameManager_Core_SafeGetState() == Core.GameManager.GameState.Gameplay)
                Core.GameManager.Instance?.SetState(Core.GameManager.GameState.Dialogue);
        }

        private static Core.GameManager.GameState GameManager_Core_SafeGetState() =>
            Core.GameManager.Instance != null ? Core.GameManager.Instance.CurrentState : Core.GameManager.GameState.Gameplay;

        private void GoToNode(string nodeId)
        {
            if (string.IsNullOrEmpty(nodeId)) { End(); return; }
            var node = currentDialogue.GetNode(nodeId);
            if (node == null) { Debug.LogWarning($"Dialogue node {nodeId} not found in {currentDialogue.dialogueId}"); End(); return; }

            CurrentNode = node;
            if (!string.IsNullOrEmpty(node.setsStoryFlagOnShow)) questManager?.SetStoryFlag(node.setsStoryFlagOnShow);
        }

        public List<DialogueChoice> GetAvailableChoices()
        {
            if (CurrentNode?.choices == null) return new List<DialogueChoice>();
            return CurrentNode.choices.Where(IsChoiceAvailable).ToList();
        }

        private bool IsChoiceAvailable(DialogueChoice choice)
        {
            if (!string.IsNullOrEmpty(choice.requiredStoryFlag) && !(questManager?.HasStoryFlag(choice.requiredStoryFlag) ?? false))
                return false;
            if (!string.IsNullOrEmpty(choice.forbiddenStoryFlag) && (questManager?.HasStoryFlag(choice.forbiddenStoryFlag) ?? false))
                return false;
            if (choice.minimumAffinityWithSpeaker > 0 && CurrentNode != null)
            {
                int affinity = relationships != null ? relationships.GetAffinity(CurrentNode.speakerNpcId) : 0;
                if (affinity < choice.minimumAffinityWithSpeaker) return false;
            }
            return true;
        }

        public void SelectChoice(DialogueChoice choice)
        {
            if (!string.IsNullOrEmpty(choice.setsStoryFlagOnSelect)) questManager?.SetStoryFlag(choice.setsStoryFlagOnSelect);
            if (choice.affinityDeltaOnSelect != 0 && CurrentNode != null)
                relationships?.ChangeAffinity(CurrentNode.speakerNpcId, choice.affinityDeltaOnSelect);

            if (!string.IsNullOrEmpty(CurrentNode?.speakerNpcId))
                questManager?.ReportTalkToNpc(CurrentNode.speakerNpcId);

            GoToNode(choice.nextNodeId);
            if (!IsActive) return;
        }

        public void Advance()
        {
            if (CurrentNode == null) { End(); return; }
            if (CurrentNode.choices != null && CurrentNode.choices.Length > 0) return; // waiting for a choice
            GoToNode(CurrentNode.autoAdvanceNextNodeId);
        }

        private void End()
        {
            IsActive = false;
            CurrentNode = null;
            currentDialogue = null;
            if (Core.GameManager.Instance != null && Core.GameManager.Instance.CurrentState == Core.GameManager.GameState.Dialogue)
                Core.GameManager.Instance.SetState(Core.GameManager.GameState.Gameplay);
        }
    }
}
