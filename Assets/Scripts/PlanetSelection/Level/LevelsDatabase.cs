using UnityEngine;

[CreateAssetMenu(fileName = "LevelsDatabase", menuName = "Levels/Levels Database")]
public class LevelsDatabase : ScriptableObject
{
    public Level[] levels;
}
