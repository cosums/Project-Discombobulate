using UnityEngine;

[CreateAssetMenu(fileName = "NewGameData", menuName = "Game Data")]
public class GameData : ScriptableObject
{
    public LevelData[] Levels;
}
