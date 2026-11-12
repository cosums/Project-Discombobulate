using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] DialogueUIPanel dialogUI;
    [SerializeField] Dialogue redDialogue;

    Coroutine currentRoutine;
    AudioSource currentVoice;

    public static DialogueSystem Instance { get; private set; }
    public bool IsPlaying => currentRoutine != null;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(transform.root.gameObject);
            return;
        }

        Instance = this;
    }

    void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (Instance != this) return;
        if (mode != LoadSceneMode.Single) return;
        StopDialogue();
    }

    public void PlayDialogue(Dialogue dialogue)
    {
        if (dialogue == null) return;

        StopDialogue(clearUI: false);
        currentRoutine = StartCoroutine(PlayRoutine(dialogue));
    }

    public void StopDialogue(bool clearUI = true)
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }

        if (currentVoice != null)
        {
            Destroy(currentVoice.gameObject);
            currentVoice = null;
        }

        if (clearUI && dialogUI != null)
            dialogUI.ClearUI();
    }

    IEnumerator PlayRoutine(Dialogue dialogue)
    {
        foreach (var line in dialogue.GetLines())
        {
            if (dialogUI == null) break;
            GameObject lineObj = dialogUI.AddLine(line.dialogLine);

            if (line.dialogAudio != null)
            {
                AudioManager.Instance.PlayOneShot(line.dialogAudio, 2);

                yield return new WaitForSeconds(line.clipLength == 0 ? line.dialogAudio.length : line.clipLength);
            }
            else
            {
                yield return new WaitForSeconds(3);
            }

            Destroy(lineObj);
        }

        currentVoice = null;
        currentRoutine = null;

        if (dialogue.endingChoice != null) {
           HandleChoiceSetup(dialogue.endingChoice);
        }

        if (dialogue.result == DialogueResult.Death)
        {
            AttackHandler.KillPlayer();
            GameManager.Instance.ChangeState(GameState.Playing);
            yield break;
        }

        if (dialogue.result == DialogueResult.HappyHappyHappy)
        {
            GameManager.Instance.ChangeState(GameState.Playing);
            yield break;
        }
    }

    private void HandleChoiceSetup(DialogueChoice choice)
    {
        foreach (var option in choice.dialogueChoices)
        {
            dialogUI.AddOption(option.dialogLine, () => HandleChoice(option.nextDialogue));
        }
    }

    private void HandleChoice(Dialogue next)
    {
        dialogUI.ClearUI();
        PlayDialogue(next);
    }
}