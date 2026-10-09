using System;
using UnityEngine;

namespace RebirthEclipse.Dialogue
{
    [Serializable]
    public class DialogueChoice
    {
        public string choiceText;
        public string nextNodeId;           // empty = ends conversation
        public string requiredStoryFlag;    // empty = always available
        public string forbiddenStoryFlag;   // hides choice if this flag IS set
        public int minimumAffinityWithSpeaker; // for romance/relationship-gated lines
        public string setsStoryFlagOnSelect;
        public int affinityDeltaOnSelect;
    }

    [Serializable]
    public class DialogueNode
    {
        public string nodeId;
        public string speakerNpcId;         // empty = narrator/system
        [TextArea(2, 6)] public string text;
        public string portraitEmotion;      // "neutral","angry","sad","smirk", etc. — drives portrait sprite selection
        public string voiceClipKey;
        public DialogueChoice[] choices;    // empty = auto-advance to autoAdvanceNextNodeId
        public string autoAdvanceNextNodeId;
        public string setsStoryFlagOnShow;
    }

    /// <summary>
    /// One full conversation graph. Branches are resolved entirely from story flags and
    /// NPC affinity read at runtime — the same asset plays differently depending on what
    /// the player has already done, which is how companion loyalty/romance state actually
    /// changes dialogue rather than just being flavor text.
    /// </summary>
    [CreateAssetMenu(fileName = "NewDialogue", menuName = "RebirthEclipse/Dialogue Definition")]
    public class DialogueDefinition : ScriptableObject
    {
        public string dialogueId;
        public string startNodeId;
        public DialogueNode[] nodes;

        public DialogueNode GetNode(string nodeId)
        {
            foreach (var n in nodes)
                if (n.nodeId == nodeId) return n;
            return null;
        }
    }
}
