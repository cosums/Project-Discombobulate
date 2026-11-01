using UnityEngine;

[CreateAssetMenu(fileName = "NewLoreItem", menuName = "Lore Items System/Lore Item")]
public class LoreItem : ScriptableObject
{
    public string Name;
    [TextArea(5, 10)]
    public string Description;
    public Sprite Icon; // shown in the log book
    public GameObject modelPrefab; // to be inspected by the player
}
