[System.Serializable]
public class Level
{
    public int ID;
    public string title;
    public float latitude;
    public float longitude;
    public string description;
    public bool completed;  
}

[System.Serializable]
public class Levels
{
    public Level[] levels;
}