using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTracker : MonoBehaviour
{
    public GameObject CurrentActiveObject;

    [SerializeField] private ARTrackedImageManager trackedImageManager;
    [SerializeField] private GameObject[] placeablePrefabs;

    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();

    public event Action<GameObject> OnPlantActivated;
    public event Action<GameObject> OnPlantDeactivated;

    void OnEnable()
    {
        if (trackedImageManager != null)
            trackedImageManager.trackablesChanged.AddListener(OnImageChanged);
    }

    void OnDisable()
    {
        if (trackedImageManager != null)
            trackedImageManager.trackablesChanged.RemoveListener(OnImageChanged);
    }

    void Start()
    {
        SetupPrefabs();
    }

    private void SetupPrefabs()
    {
        spawnedPrefabs.Clear();

        foreach (GameObject prefab in placeablePrefabs)
        {
            GameObject newObj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            newObj.name = prefab.name;
            newObj.SetActive(false);

            spawnedPrefabs.Add(prefab.name, newObj);
        }
    }

    private void OnImageChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        foreach (var added in args.added)
            UpdateImage(added);

        foreach (var updated in args.updated)
            UpdateImage(updated);

        foreach (var removed in args.removed)
            UpdateImage(removed.Value);
    }

    private void UpdateImage(ARTrackedImage trackedImage)
    {
        if (trackedImage == null || trackedImage.referenceImage == null)
            return;

        string key = trackedImage.referenceImage.name;

        if (!spawnedPrefabs.TryGetValue(key, out GameObject obj))
            return;

        // tracking lost → hide plant
        if (trackedImage.trackingState == TrackingState.Limited ||
            trackedImage.trackingState == TrackingState.None)
        {
            if (obj.activeSelf)
            {
                obj.SetActive(false);

                if (CurrentActiveObject == obj)
                {
                    CurrentActiveObject = null;
                    OnPlantDeactivated?.Invoke(obj);
                }
            }
            return;
        }

        // tracking good → show/update
        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            obj.transform.SetPositionAndRotation(
                trackedImage.transform.position,
                trackedImage.transform.rotation);

            if (!obj.activeSelf)
                obj.SetActive(true);

            if (CurrentActiveObject != obj)
            {
                CurrentActiveObject = obj;
                OnPlantActivated?.Invoke(obj);
            }
        }
    }
}


