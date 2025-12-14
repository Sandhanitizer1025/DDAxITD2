using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Collections;


public class ObjectiveManager : MonoBehaviour
{
    [Header("Objective UI")]
    public TextMeshProUGUI objectiveText;
    public GameObject completePanel; // optional
    private bool objectiveCompleted = false;

    [Header("Objective Settings")]
    public int totalPlantsToGrow = 3;

    private HashSet<PlantGrowth> maturedPlants = new HashSet<PlantGrowth>();
    private HashSet<PlantGrowth> registered = new HashSet<PlantGrowth>();


    public ImageTracker imageTracker;

    public void RegisterPlant(PlantGrowth plant)
    {
        if (plant == null) return;

        if (!registered.Contains(plant))
        {
            registered.Add(plant);
            plant.OnMatured += () => OnPlantMatured(plant);
        }

        if (plant.IsMature())
            OnPlantMatured(plant);

        UpdateUI();
    }

    private void OnPlantMatured(PlantGrowth plant)
    {
        if (plant == null) return;

        maturedPlants.Add(plant);
        UpdateUI();

        if (!objectiveCompleted && maturedPlants.Count >= totalPlantsToGrow)
        {
            objectiveCompleted = true;

            if (completePanel != null)
            {
                completePanel.SetActive(true);
                StartCoroutine(HideCompletePanelAfterDelay(3f));
            }
        }
    }

    private void UpdateUI()
    {
        if (objectiveText == null) return;
        objectiveText.text = $"Grow all succulents: {maturedPlants.Count}/{totalPlantsToGrow}";
    }

    void Awake()
    {
        if (imageTracker != null)
            imageTracker.OnPlantActivated += OnPlantActivated;
    }

    void OnDestroy()
    {
        if (imageTracker != null)
            imageTracker.OnPlantActivated -= OnPlantActivated;
    }

    private void OnPlantActivated(GameObject plantObj)
    {
        var growth = plantObj.GetComponent<PlantGrowth>();
        if (growth != null)
            RegisterPlant(growth);
    }

    private IEnumerator HideCompletePanelAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        if (completePanel != null)
            completePanel.SetActive(false);
    }

}

