using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlantInfoDisplay : MonoBehaviour
{
    [Header("IDs")]
    [Tooltip("Must match the species ID in Firebase, e.g. 'cactus', 'echeveria', 'aloe_vera'")]
    public string speciesId;

    [Header("UI References")]
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public TMP_Text careTipsText;
    public TMP_Text funFactsText;

    private void Start()
    {
        // Safety checks
        if (FirebasePlantService.Instance == null)
        {
            Debug.LogWarning("[PlantInfoDisplay] FirebasePlantService.Instance is null.");
            return;
        }

        if (string.IsNullOrEmpty(speciesId))
        {
            Debug.LogWarning("[PlantInfoDisplay] speciesId is empty on " + gameObject.name);
            return;
        }

        // Ask Firebase for species data
        FirebasePlantService.Instance.LoadSpecies(OnSpeciesLoaded);
    }

    private void OnSpeciesLoaded(Dictionary<string, SpeciesData> speciesDict)
    {
        if (speciesDict == null || !speciesDict.ContainsKey(speciesId))
        {
            Debug.LogWarning($"[PlantInfoDisplay] No species found for id '{speciesId}'");
            return;
        }

        SpeciesData data = speciesDict[speciesId];

        if (titleText != null)       titleText.text       = data.name;
        if (descriptionText != null) descriptionText.text = data.description;
        if (careTipsText != null)   careTipsText.text   = data.careTips;
        if (funFactsText != null)     funFactsText.text     = data.funFacts;
    }
}
