using UnityEngine;

[CreateAssetMenu(fileName = "PlanetList", menuName = "Scene Creator/Planet List")]
public class PlanetList : ScriptableObject
{
    public string[] planets = { "Earth", "Mars", "Venus", "Jupiter" };
}
