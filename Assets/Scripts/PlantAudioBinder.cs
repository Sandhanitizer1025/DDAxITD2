using UnityEngine;

public class PlantAudioBinder : MonoBehaviour
{
    public ImageTracker imageTracker;
    public AudioManager audioManager;

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
        if (growth == null) return;

        // Prevent double subscriptions
        growth.OnWatered -= HandleWatered;
        growth.OnFertilized -= HandleFertilized;
        growth.OnMatured -= HandleMatured;

        growth.OnWatered += HandleWatered;
        growth.OnFertilized += HandleFertilized;
        growth.OnMatured += HandleMatured;
    }

    private void HandleWatered()
    {
        audioManager.PlaySfx(audioManager.waterSfx);
    }

    private void HandleFertilized()
    {
        audioManager.PlaySfx(audioManager.fertilizeSfx);
    }

    private void HandleMatured()
    {
        audioManager.PlaySfx(audioManager.matureSfx);
    }
}
