using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using System;

public class PlantGrowth : MonoBehaviour
{
    [Header("Requirements")]
    public int requiredWater = 3;
    public int requiredFertilize = 2;

    [Header("References")]
    public GameObject seedObject;     // your seed GO
    public GameObject fullPlantObject; // grown plant GO
    public GameObject potObject;
    public TextMeshProUGUI statusText;

    [Header("Runtime")]
    public bool isPlanted = false;
    public bool isMature = false;
    public int waterCount = 0;
    public int fertilizeCount = 0;

    [Header("UI Buttons")]
    public Button waterButton;
    public Button fertilizeButton;

    public event Action OnWatered;
    public event Action OnFertilized;
    public event Action OnMatured;


    void Start()
    {
        SetCareButtonsVisible(false);

        if (fullPlantObject) fullPlantObject.SetActive(false);
        
        // Disable buttons until seed planted
        if (waterButton) waterButton.interactable = false;
        if (fertilizeButton) fertilizeButton.interactable = false;

        if (statusText)
            statusText.gameObject.SetActive(true);

        UpdateStatus();
    }

    public void OnSeedPlanted()
    {
        if (isPlanted) return;
        isPlanted = true;
        if (seedObject) seedObject.SetActive(false);

        SetCareButtonsVisible(true);

        // Enable buttons now
        if (waterButton) waterButton.interactable = true;
        if (fertilizeButton) fertilizeButton.interactable = true;
        
        UpdateStatus();
        Debug.Log($"{name} planted. seedObject={(seedObject ? seedObject.name : "NULL")}");

    }

    public void Water()
    {
        if (!isPlanted || isMature) return;
        if (waterCount >= requiredWater) return;

        waterCount++;
        OnWatered?.Invoke();

        // Disable water button at max
        if (waterCount >= requiredWater && waterButton)
            waterButton.interactable = false;

        UpdateStatus();
        CheckMature();
    }

    public void Fertilize()
    {
        if (!isPlanted || isMature) return;
        if (fertilizeCount >= requiredFertilize) return;

        fertilizeCount++;
        OnFertilized?.Invoke();

        // Disable fertilize button at max
        if (fertilizeCount >= requiredFertilize && fertilizeButton)
            fertilizeButton.interactable = false;

        UpdateStatus();
        CheckMature();
    }

    public bool IsMature()
    {
        return isMature;
    }

    private void CheckMature()
    {
        if (waterCount >= requiredWater && fertilizeCount >= requiredFertilize)
        {
            isMature = true;
            OnMatured?.Invoke();
            SetCareButtonsVisible(false);

            if (potObject) potObject.SetActive(false);
            if (seedObject) seedObject.SetActive(false);
            if (fullPlantObject) fullPlantObject.SetActive(true);
            if (waterButton) waterButton.interactable = false;
            if (fertilizeButton) fertilizeButton.interactable = false;

            if (fullPlantObject) fullPlantObject.SetActive(true);

            // Enable grab (if you use XR Grab)
            var grab = fullPlantObject.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            if (grab != null) grab.enabled = true;


            UpdateStatus();

            // hide after 5 seconds
            StopAllCoroutines();
            StartCoroutine(HideStatusAfterDelay(5f));
        }
    }

    private void UpdateStatus()
    {
        if (!statusText) return;

        if (!isPlanted)
            statusText.text = "Drag seed into pot first";
        else if (!isMature)
            statusText.text = $"Water {waterCount}/{requiredWater}  Fertilize {fertilizeCount}/{requiredFertilize}";
        else
            statusText.text = "Mature! You can inspect it now.";
    }

    private void SetCareButtonsVisible(bool visible)
    {
        if (waterButton) waterButton.gameObject.SetActive(visible);
        if (fertilizeButton) fertilizeButton.gameObject.SetActive(visible);
    }

    private IEnumerator HideStatusAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        if (statusText != null)
            statusText.gameObject.SetActive(false);
    }

}

