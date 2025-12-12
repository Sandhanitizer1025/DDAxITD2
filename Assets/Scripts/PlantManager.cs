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

        Transform canvas = plant.transform.Find("Canvas");
        if (canvas == null) return;

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

        // auto-bind Info button
        Button info = canvas.Find("Info")?.GetComponent<Button>();
        if (info)
        {
            info.onClick.RemoveAllListeners();
            info.onClick.AddListener(() => ShowInfo(plant));
        }

        // ensure info panel disabled at start
        Transform p = canvas.Find("InfoPanel");
        if (p) p.gameObject.SetActive(false);
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
    }

    private void FertilizePlant(GameObject plant)
    {
        var growth = plant.GetComponent<PlantGrowth>();
        if (!growth) return;

        growth.Grow();
        growth.Grow();
        Debug.Log($"Fertilized {plant.name} → growth {growth.growth}");
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
    }
}




