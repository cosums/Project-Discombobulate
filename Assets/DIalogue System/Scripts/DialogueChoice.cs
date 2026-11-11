using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue System/Dialogue Choice")]
public class DialogueChoice : ScriptableObject
{
    [Serializable]
    public struct DialogueOption
    {
        [TextArea(3, 5)]
        public string dialogLine;
        public AudioClip dialogAudio;

        public float clipLength;

        public Dialogue nextDialogue;
    }

    public DialogueOption[] dialogueChoices = new DialogueOption[2];

    public DialogueOption[] GetLines()
    {
        return dialogueChoices;
    }
}