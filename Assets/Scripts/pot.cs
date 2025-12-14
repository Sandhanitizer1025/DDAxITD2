using UnityEngine;

public class PotTrigger : MonoBehaviour
{
    private PlantGrowth growth;

    void Awake()
    {
        growth = GetComponentInParent<PlantGrowth>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"PotTrigger hit by: {other.name}, tag={other.tag}");

        if (!other.CompareTag("Seed")) return;

        Debug.Log("Seed detected -> planting!");
        growth.OnSeedPlanted();
        GetComponent<Collider>().enabled = false;
    }
}

