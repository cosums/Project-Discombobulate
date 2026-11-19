using UnityEngine;
using UnityEngine.Events;

public class LoreItemCollectible : MonoBehaviour, IInteractable
{
    [SerializeField] LoreItemData loreItemData;
    [SerializeField] Dialogue dialogueOnPickup;

    [SerializeField] UnityEvent OnInteractEnter;
    [SerializeField] UnityEvent OnHoverEnter;
    [SerializeField] UnityEvent OnHoverExit;

    private HighlightObject highlightObject;

    private void Awake()
    {
        highlightObject = GetComponent<HighlightObject>();
    }

    public void InteractEnter()
    {
        if (LoreItemManager.Instance.Collect(loreItemData))
        {
            OnInteractEnter.Invoke();
            DialogueSystem.Instance.PlayDialogue(dialogueOnPickup);
        }

        //InspectObjectUI.Instance.Show(loreItemData); // TODO UNCOMMENT ME
        // commented out to test if subtitles work at base
    }

    public void HoverExit()
    {
        OnHoverExit.Invoke();
        highlightObject?.DeHighlight();
    }

    public void HoverEnter()
    {
        OnHoverEnter.Invoke();
        highlightObject?.Highlight();
    }

}
