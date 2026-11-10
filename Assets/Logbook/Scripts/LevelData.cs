using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelItem", menuName = "Level Data")]
public class LevelData : ScriptableObject
{
    public int LevelNum;
    public LoreItemData[] LoreItems;
}

[System.Serializable]
public class RoomSet
{
    
}