[System.Serializable]
public class PlantState
{
    public string plantId;
    public string speciesId;
    public int growth;        
    public int matureLevel;   // default = 3
    public bool isMatured;
    public long lastUpdated;
}
