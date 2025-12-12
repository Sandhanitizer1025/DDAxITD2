using UnityEngine;

public class PlantGrowth : MonoBehaviour
{
    public int growth = 0;
    public int matureLevel = 3;

    public void Grow()
    {
        growth++;

        Debug.Log("Plant grew to level: " + growth);
    }

    public bool IsMature()
    {
        return growth >= matureLevel;
    }
}
