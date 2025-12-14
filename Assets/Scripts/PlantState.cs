[System.Serializable]
public class PlantState
{
    public string plantId;
    public string speciesId;
    public bool isPlanted;
    public bool isMature;
    public int waterCount;
    public int fertilizeCount;
    public int requiredWater;
    public int requiredFertilize;

    public long lastUpdated;
}
