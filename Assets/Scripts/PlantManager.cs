using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PlantManager : MonoBehaviour
{
    public ImageTracker imageTracker;
    public GameObject infoPanel;

    [Header("End Experience UI (Same Scene)")]
    public GameObject endPanel;
    public string startSceneName = "StartMenu";
    public int totalPlantsToComplete = 3;

    private HashSet<string> maturedPlantIds = new HashSet<string>();
    private HashSet<PlantGrowth> subscribedPlants = new HashSet<PlantGrowth>();

    // Auto-wired end panel buttons (no Inspector OnClick needed)
    private Button continueButton;
    private Button returnButton;

    void Awake()
    {
        if (imageTracker != null)
        {
            imageTracker.OnPlantActivated += SetupPlantUI;
            imageTracker.OnPlantDeactivated += HideUI;
        }

        if (endPanel != null)
            endPanel.SetActive(false);
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

        var pg = plant.GetComponent<PlantGrowth>();

        if (pg != null && !subscribedPlants.Contains(pg))
        {
            subscribedPlants.Add(pg);
            string plantId = GetPlantId(plant);

            // Track completion
            pg.OnMatured += () => MarkPlantMatured(plantId);
        }

        // Firebase: LOAD saved state
        if (FirebasePlantService.Instance != null)
        {
            string plantId = GetPlantId(plant);

            FirebasePlantService.Instance.LoadPlants((dict) =>
            {
                if (dict != null && dict.TryGetValue(plantId, out PlantState state))
                {
                    if (pg != null)
                    {
                        pg.isPlanted = state.isPlanted;
                        pg.isMature = state.isMature;

                        pg.waterCount = state.waterCount;
                        pg.fertilizeCount = state.fertilizeCount;

                        pg.requiredWater = state.requiredWater;
                        pg.requiredFertilize = state.requiredFertilize;

                        Debug.Log($"[PlantManager] Loaded {plantId} planted={pg.isPlanted} mature={pg.isMature} water={pg.waterCount}/{pg.requiredWater} fert={pg.fertilizeCount}/{pg.requiredFertilize}");

                        if (pg.isMature)
                            MarkPlantMatured(plantId);
                    }
                }
            });
        }

        // Water button
        Button water = canvas.Find("Water")?.GetComponent<Button>();
        if (water)
        {
            water.onClick.RemoveAllListeners();
            water.onClick.AddListener(() => WaterPlant(plant));
        }

        // Fertilize button
        Button fertilize = canvas.Find("Fertilize")?.GetComponent<Button>();
        if (fertilize)
        {
            fertilize.onClick.RemoveAllListeners();
            fertilize.onClick.AddListener(() => FertilizePlant(plant));
        }

        // Info button
        Button info = canvas.Find("Info")?.GetComponent<Button>();
        if (info)
        {
            info.onClick.RemoveAllListeners();
            info.onClick.AddListener(() => ShowInfo(plant));
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

    private void WaterPlant(GameObject plant)
    {
        var pg = plant.GetComponent<PlantGrowth>();
        if (!pg) return;

        pg.Water();
        Debug.Log($"Water pressed for {plant.name} → water {pg.waterCount}/{pg.requiredWater}");

        SaveCurrentPlant(plant, pg);
    }

    private void FertilizePlant(GameObject plant)
    {
        var pg = plant.GetComponent<PlantGrowth>();
        if (!pg) return;

        pg.Fertilize();
        Debug.Log($"Fertilize pressed for {plant.name} → fert {pg.fertilizeCount}/{pg.requiredFertilize}");

        SaveCurrentPlant(plant, pg);
    }

    private void ShowInfo(GameObject plant)
    {
        var pg = plant.GetComponent<PlantGrowth>();
        if (pg == null || !pg.IsMature())
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
    }

    // End experience logic
    private void MarkPlantMatured(string plantId)
    {
        if (string.IsNullOrEmpty(plantId)) return;

        if (!maturedPlantIds.Add(plantId))
            return;

        Debug.Log($"[PlantManager] Matured {maturedPlantIds.Count}/{totalPlantsToComplete} : {plantId}");

        if (maturedPlantIds.Count >= totalPlantsToComplete)
        {
            ShowEndPanel();
        }
    }

    private void ShowEndPanel()
    {
        if (endPanel != null)
        {
            endPanel.SetActive(true);

            AutoWireEndPanelButtons();
        }
    }

    private void AutoWireEndPanelButtons()
    {
        if (endPanel == null) return;

        continueButton = null;
        returnButton = null;

        Button[] buttons = endPanel.GetComponentsInChildren<Button>(true);

        foreach (Button b in buttons)
        {
            if (b.name == "ContinueButton") continueButton = b;
            if (b.name == "ReturnButton") returnButton = b;
        }

        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(() =>
            {
                Debug.Log("[EndPanel] Continue clicked");
                ContinueExploring();
            });
        }
        else
        {
            Debug.LogWarning("[PlantManager] ContinueButton not found under endPanel.");
        }

        if (returnButton != null)
        {
            returnButton.onClick.RemoveAllListeners();
            returnButton.onClick.AddListener(() =>
            {
                Debug.Log("[EndPanel] Return clicked");
                ReturnToStartMenu();
            });
        }
        else
        {
            Debug.LogWarning("[PlantManager] ReturnButton not found under endPanel.");
        }
    }

    public void ContinueExploring()
    {
        if (endPanel != null)
            endPanel.SetActive(false);
    }

    public void ReturnToStartMenu()
    {
        SceneManager.LoadScene(startSceneName);
    }

    // Firebase helper methods
    private string GetPlantId(GameObject plant)
    {
        return plant.name;
    }

    private void SaveCurrentPlant(GameObject plant, PlantGrowth pg)
    {
        if (FirebasePlantService.Instance == null) return;

        string plantId = GetPlantId(plant);

        PlantState state = new PlantState
        {
            plantId = plantId,
            speciesId = plantId,

            isPlanted = pg.isPlanted,
            isMature = pg.isMature,
            waterCount = pg.waterCount,
            fertilizeCount = pg.fertilizeCount,
            requiredWater = pg.requiredWater,
            requiredFertilize = pg.requiredFertilize
        };

        FirebasePlantService.Instance.SavePlant(plantId, state);
    }
}
