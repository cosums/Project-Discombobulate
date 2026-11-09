using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LogbookUI : MonoBehaviour
{
    [SerializeField] GameObject parent;
    [SerializeField] private List<LoreItemButtonUI> loreItemButtons = new List<LoreItemButtonUI>();
    private bool showUI = false;


    private void Awake()
    {
        PopulateItemButtonList();
    }

    private void Update()
    {
        // TODO: move input logic to a centralized script
        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            showUI = !showUI;
            SetActive(showUI);
            PlayerController.CanMove = !showUI;
            if (showUI) UIManager.EnableCursor();
            else UIManager.DisableCursor();
        }
    }

    private void OnEnable()
    {
        LoreItemManager.OnCollectionChanged += UpdateLogbookUI;
        UpdateLogbookUI();
    }

    private void OnDisable()
    {
        LoreItemManager.OnCollectionChanged -= UpdateLogbookUI;
    }

    private void PopulateItemButtonList()
    {
        loreItemButtons.Clear();
        loreItemButtons.AddRange(GetComponentsInChildren<LoreItemButtonUI>(true));
    }

    private void UpdateLogbookUI()
    {
        if (LoreItemManager.Instance == null) return;

        foreach (var button in loreItemButtons)
        {
            bool collected = LoreItemManager.Instance.HasCollected(button.ItemData);
            button.UpdateButton(button.ItemData, collected);
        }
    }

    public void SetActive(bool enabled)
    {
        parent.SetActive(enabled);
    }
}