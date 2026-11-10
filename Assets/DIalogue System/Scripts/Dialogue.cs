using System;
using System.Collections.Generic;
using UnityEngine;

public enum Speaker
{
    James = 0,
    Mallory = 1
}

public enum DialogueResult
{
    None = 0,
    Death = 1
}

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue System/Dialogue")]
public class Dialogue : ScriptableObject
{
    [Serializable]
    public struct DialogueLine
    {
        [TextArea(3, 5)]
        public string dialogLine;
        public AudioClip dialogAudio;

        public float clipLength;
        public Speaker speaker;
    }

    [SerializeField] List<DialogueLine> dialogLines = new List<DialogueLine>();
    public DialogueChoice endingChoice;
    public bool strictPlay;
    public DialogueResult result;

    public List<DialogueLine> GetLines()
    {
        return dialogLines;
    }
}