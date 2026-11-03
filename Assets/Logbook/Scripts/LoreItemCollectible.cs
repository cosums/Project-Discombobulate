using UnityEngine;
using UnityEngine.Events;

public class LoreItemCollectible : MonoBehaviour, IInteractable
{
    [SerializeField] LoreItemData loreItemData;

    [SerializeField] UnityEvent OnInteractEnter;
    [SerializeField] UnityEvent OnHoverEnter;
    [SerializeField] UnityEvent OnHoverExit;

    public void InteractEnter()
    {
        if (LevelManager.Instance.Collect(loreItemData))
        {
            OnInteractEnter.Invoke();
        }
    }

    public void HoverExit()
    {
        OnHoverExit.Invoke();
    }

    public void HoverEnter()
    {
        OnHoverEnter.Invoke();
    }

}
