using UnityEngine;
using System.Collections.Generic;

public class CompletionTracker : MonoBehaviour
{
    public EndExperienceUI endUI;
    public int totalPlantsToComplete = 3;

    private HashSet<PlantGrowth> maturedPlants = new HashSet<PlantGrowth>();
    private bool endShown = false;

    // Called once per plant to register it
    public void RegisterPlant(PlantGrowth plant)
    {
        if (plant == null) return;

        // Listen for maturity
        plant.OnMatured += () => OnPlantMatured(plant);
    }

    private void OnPlantMatured(PlantGrowth plant)
    {
        if (endShown) return;

        maturedPlants.Add(plant);
        Debug.Log($"[CompletionTracker] {maturedPlants.Count}/{totalPlantsToComplete} plants matured");

        if (maturedPlants.Count >= totalPlantsToComplete)
        {
            endShown = true;
            if (endUI != null)
                endUI.ShowEnd();
        }
    }
}
