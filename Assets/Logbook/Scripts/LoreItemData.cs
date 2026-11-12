using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLoreItem", menuName = "Lore Items System/Lore Item Data")]
public class LoreItemData : ScriptableObject
{
    public string ItemName;
    [TextArea(5, 10)]
    public string Description;
    public Sprite Icon; // shown in the log book
    public GameObject ModelPrefab; // to be inspected by the player

    [HideInInspector] public int groupIndex = -1;
}
