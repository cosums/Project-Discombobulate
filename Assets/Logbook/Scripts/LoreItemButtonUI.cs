using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the UI for an instance of a lore item button (in the logbook UI)
/// </summary>
public class LoreItemButtonUI : MonoBehaviour
{
    [SerializeField] Image mainImage;
    [SerializeField] Image outlineImage;
    [SerializeField] TextMeshProUGUI itemNameTmp;
    [SerializeField] Button button;
    [SerializeField] LoreItemData itemData;

    public LoreItemData ItemData => itemData;

    private void Start()
    {
        SetImage(itemData.Icon);
    }

    /// <summary>
    /// Updates the button visuals and data
    /// </summary>
    /// <param name="data"></param>
    /// <param name="collected">Whether the lore item has been collected by player.</param>
    public void UpdateButton(LoreItemData data, bool collected)
    {
        SetImage(data.Icon);
        SetItemData(data);
        itemNameTmp.text = data.ItemName;

        if (!collected) mainImage.enabled = false;
        else mainImage.enabled = true; 
    }

    private void SetImage(Sprite sprite)
    {
        mainImage.sprite = sprite;
        outlineImage.sprite = sprite;
    }

    private void SetItemData(LoreItemData data)
    {
        itemData = data;
    }
}
