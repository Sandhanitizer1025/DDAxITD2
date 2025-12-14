using UnityEngine;
using UnityEngine.UI;

public class PlantManager : MonoBehaviour
{
    public ImageTracker imageTracker;
    public GameObject infoPanel; // optional global override panel

    void Awake()
    {
        if (imageTracker != null)
        {
            imageTracker.OnPlantActivated += SetupPlantUI;
            imageTracker.OnPlantDeactivated += HideUI;
        }
    }

    void OnDestroy()
    {
        if (imageTracker != null)
        {
            imageTracker.OnPlantActivated -= SetupPlantUI;
            imageTracker.OnPlantDeactivated -= HideUI;
        }
    }

    // Automatically called when the plant becomes visible
    private void SetupPlantUI(GameObject plant)
    {
        if (plant == null) return;

        Canvas c = plant.GetComponentInChildren<Canvas>(true);
        if (c == null) return;
        Transform canvas = c.transform;


        // auto-bind Water button
        Button water = canvas.Find("Water")?.GetComponent<Button>();
        if (water)
        {
            water.onClick.RemoveAllListeners();
            water.onClick.AddListener(() => WaterPlant(plant));
        }

        // auto-bind Fertilize button
        Button fertilize = canvas.Find("Fertilize")?.GetComponent<Button>();
        if (fertilize)
        {
            fertilize.onClick.RemoveAllListeners();
            fertilize.onClick.AddListener(() => FertilizePlant(plant));
        }
    }

    private void HideUI(GameObject plant)
    {
        if (plant == null) return;

        Transform localPanel = plant.transform.Find("Canvas/InfoPanel");
        if (localPanel)
            localPanel.gameObject.SetActive(false);

        if (infoPanel)
            infoPanel.SetActive(false);
    }

    // growth logic
    private void WaterPlant(GameObject plant)
    {
        var growth = plant.GetComponent<PlantGrowth>();
        if (!growth) return;

        growth.Water();
        Debug.Log($"Water pressed for {plant.name} → water {growth.waterCount}/{growth.requiredWater}");
    }

    private void FertilizePlant(GameObject plant)
    {
        var growth = plant.GetComponent<PlantGrowth>();
        if (!growth) return;

        growth.Fertilize();
        Debug.Log($"Fertilize pressed for {plant.name} → fert {growth.fertilizeCount}/{growth.requiredFertilize}");
    }
}




