using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    int currentGroupIndex = 0;
    [SerializeField] private List<ItemGroup> itemGroups = new List<ItemGroup>();

    public static LevelManager Instance;

    public static Action OnCollectionChanged; // calls when player unlocks a new lore item


    /// <summary>
    /// Each level has a list of lore items that need to be collected
    /// </summary>
    [Serializable]
    public class ItemGroup
    {
        public string name;
        public List<LoreItemData> items = new List<LoreItemData>();
        [NonSerialized] public HashSet<LoreItemData> collected = new HashSet<LoreItemData>();

        public bool IsComplete => collected.Count >= items.Count;
    }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;


        AssignItemDataGroupNumbers();
    }

    /// <summary>
    /// Each <see cref="LoreItemData"/> has a itemGroup int representing the item group they are apart of
    /// Based on the itemGroups, assign the correct group number to the item data
    /// </summary>
    private void AssignItemDataGroupNumbers()
    {
        for (int i = 0; i < itemGroups.Count; i++)
        {
            foreach (LoreItemData loreItem in itemGroups[i].items)
            {
                loreItem.groupIndex = i;
            }
        }
    }

    /// <summary>
    /// Try to collect lore item and mark it as collected.
    /// Check for level completion after.
    /// </summary>
    /// <param name="loreItem"></param>
    /// <returns></returns>
    public bool Collect(LoreItemData loreItem)
    {
        if (!CanCollect(loreItem)) return false;

        // add item to collected hash set
        var collected = itemGroups[loreItem.groupIndex].collected;
        collected.Add(loreItem); 

        Debug.Log($"Collected {loreItem.name} from {itemGroups[loreItem.groupIndex].name}");

        OnCollectionChanged?.Invoke();

        return true;
    }

    /// <summary>
    /// Checks if the lore item can be collected.
    /// Cannot collect if groupIndex doesn't match or item has been colleccted already
    /// </summary>
    /// <param name="loreItem"></param>
    /// <returns></returns>
    public bool CanCollect(LoreItemData loreItem)
    {
        int groupIndex = loreItem.groupIndex;

        if (groupIndex != currentGroupIndex)
        {
            Debug.Log($"Can't collect {loreItem.name} yet!");
            return false;
        }

        var collected = itemGroups[groupIndex].collected;

        if (collected.Contains(loreItem))
        {
            Debug.Log($"{loreItem.name} has already been collected!");
            return false;
        }

        return true;
    }

    private bool TryIncrementGroupIndex()
    {
        return itemGroups[currentGroupIndex].IsComplete;
    }
}
