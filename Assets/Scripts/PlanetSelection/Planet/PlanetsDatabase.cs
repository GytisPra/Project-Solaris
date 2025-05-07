using UnityEngine;

[CreateAssetMenu(fileName = "PlanetsDatabase", menuName = "Planets/Planets Database")]
public class PlanetsDatabase : ScriptableObject
{
    public Planet[] planets;
}
