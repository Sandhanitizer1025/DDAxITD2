using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;
using System.Collections.Generic;

public class FirebasePlantService : MonoBehaviour
{
    public static FirebasePlantService Instance;

    private DatabaseReference dbRoot;

    private void Awake()
    {
        // Simple singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Get DB root reference
        dbRoot = FirebaseDatabase.DefaultInstance.RootReference;
        Debug.Log("[FirebasePlantService] Database reference ready.");
    }

    // ========================
    // SAVE PLANT STATE
    // ========================
    public void SavePlant(string plantId, PlantState state)
    {
        if (FirebaseAuth.DefaultInstance.CurrentUser == null)
        {
            Debug.LogWarning("[FirebasePlantService] Cannot save plant, user not logged in.");
            return;
        }

        string uid = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        state.plantId = plantId;
        state.lastUpdated = GetUnixTime();

        string json = JsonUtility.ToJson(state);

        dbRoot.Child("plants").Child(uid).Child(plantId)
            .SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("[FirebasePlantService] SavePlant failed: " + task.Exception);
                }
                else
                {
                    Debug.Log("[FirebasePlantService] Saved plant " + plantId + " for user " + uid);
                }
            });
    }

    // ========================
    // LOAD ALL PLANTS FOR USER
    // ========================
    public void LoadPlants(System.Action<Dictionary<string, PlantState>> onLoaded)
    {
        if (FirebaseAuth.DefaultInstance.CurrentUser == null)
        {
            Debug.LogWarning("[FirebasePlantService] Cannot load plants, user not logged in.");
            onLoaded?.Invoke(new Dictionary<string, PlantState>());
            return;
        }

        string uid = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        dbRoot.Child("plants").Child(uid)
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                var result = new Dictionary<string, PlantState>();

                if (task.IsFaulted)
                {
                    Debug.LogError("[FirebasePlantService] LoadPlants failed: " + task.Exception);
                    onLoaded?.Invoke(result);
                    return;
                }

                DataSnapshot snapshot = task.Result;

                if (snapshot.Exists && snapshot.ChildrenCount > 0)
                {
                    foreach (var child in snapshot.Children)
                    {
                        string plantId = child.Key;
                        string json = child.GetRawJsonValue();
                        PlantState state = JsonUtility.FromJson<PlantState>(json);
                        result[plantId] = state;
                    }
                }

                Debug.Log("[FirebasePlantService] Loaded " + result.Count + " plants for user " + uid);
                onLoaded?.Invoke(result);
            });
    }

    // ========================
    // LOAD SPECIES INFO
    // ========================
    public void LoadSpecies(System.Action<Dictionary<string, SpeciesData>> onLoaded)
    {
        dbRoot.Child("species")
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                var result = new Dictionary<string, SpeciesData>();

                if (task.IsFaulted)
                {
                    Debug.LogError("[FirebasePlantService] LoadSpecies failed: " + task.Exception);
                    onLoaded?.Invoke(result);
                    return;
                }

                DataSnapshot snapshot = task.Result;

                if (snapshot.Exists && snapshot.ChildrenCount > 0)
                {
                    foreach (var child in snapshot.Children)
                    {
                        string speciesId = child.Key;
                        string json = child.GetRawJsonValue();
                        SpeciesData data = JsonUtility.FromJson<SpeciesData>(json);
                        result[speciesId] = data;
                    }
                }

                Debug.Log("[FirebasePlantService] Loaded " + result.Count + " species entries.");
                onLoaded?.Invoke(result);
            });
    }

    private long GetUnixTime()
    {
        return (long)System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
}
