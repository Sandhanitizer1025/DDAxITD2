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


        // =========================
        // Firebase: LOAD saved state
        // =========================
        if (FirebasePlantService.Instance != null)
        {
            string plantId = GetPlantId(plant);
            FirebasePlantService.Instance.LoadPlants((dict) =>
            {
                if (dict != null && dict.TryGetValue(plantId, out PlantState state))
                {
                    var growth = plant.GetComponent<PlantGrowth>();
                    if (growth != null)
                    {
                        growth.growth = state.growth;
                        growth.matureLevel = state.matureLevel;
                        Debug.Log($"[PlantManager] Loaded {plantId} growth={growth.growth}");
                    }
                }
            });
        }

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


        growth.Grow();
        Debug.Log($"Watered {plant.name} → growth {growth.growth}");

        // =========================
        // Firebase: SAVE state
        // =========================
        SaveCurrentPlant(plant, growth);

        growth.Water();
        Debug.Log($"Water pressed for {plant.name} → water {growth.waterCount}/{growth.requiredWater}");

    }

    private void FertilizePlant(GameObject plant)
    {
        var growth = plant.GetComponent<PlantGrowth>();
        if (!growth) return;


        growth.Grow();
        growth.Grow();
        Debug.Log($"Fertilized {plant.name} → growth {growth.growth}");

        // =========================
        // Firebase: SAVE state
        // =========================
        SaveCurrentPlant(plant, growth);
    }

    private void ShowInfo(GameObject plant)
    {
        var growth = plant.GetComponent<PlantGrowth>();
        if (growth == null || !growth.IsMature())
        {
            Debug.Log("Plant is not mature yet.");
            return;
        }

        Transform localPanel = plant.transform.Find("Canvas/InfoPanel");
        if (localPanel)
        {
            localPanel.gameObject.SetActive(true);
            return;
        }

        if (infoPanel)
            infoPanel.SetActive(true);
    }

    public void HideInfo(GameObject plant)
    {
        if (plant == null) return;

        Transform panel = plant.transform.Find("Canvas/InfoPanel");
        if (panel)
            panel.gameObject.SetActive(false);

        if (infoPanel)
            infoPanel.SetActive(false);

        growth.Fertilize();
        Debug.Log($"Fertilize pressed for {plant.name} → fert {growth.fertilizeCount}/{growth.requiredFertilize}");

    }

    // =========================
    // Firebase helper methods
    // =========================
    private string GetPlantId(GameObject plant)
    {
        // Uses prefab name (ImageTracker sets obj.name = prefab.name)
        return plant.name;
    }

    private void SaveCurrentPlant(GameObject plant, PlantGrowth growth)
    {
        if (FirebasePlantService.Instance == null) return;

        string plantId = GetPlantId(plant);

        PlantState state = new PlantState
        {
            plantId = plantId,
            speciesId = plantId,                 // minimal mapping; same as plantId
            growth = growth.growth,
            matureLevel = growth.matureLevel,
            isMatured = growth.IsMature()
        };

        FirebasePlantService.Instance.SavePlant(plantId, state);
    }
}
