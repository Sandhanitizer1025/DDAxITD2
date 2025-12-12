using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuController : MonoBehaviour
{
    [Header("Buttons")]
    public Button enterMuseumButton;
    public Button growPlantsButton;
    public Button viewCollectionButton;
    public Button logoutButton;

    [Header("Scene Names")]
    public string museumSceneName = "MainGarden";    // museum scene
    public string growSceneName = "ARGrowScene";     // placeholder
    public string collectionSceneName = "Collection"; // placeholder
    public string loginSceneName = "Login";          // your login scene

    private void Awake()
    {
        if (enterMuseumButton != null)
            enterMuseumButton.onClick.AddListener(OnEnterMuseum);

        if (growPlantsButton != null)
            growPlantsButton.onClick.AddListener(OnGrowPlants);

        if (viewCollectionButton != null)
            viewCollectionButton.onClick.AddListener(OnViewCollection);

        if (logoutButton != null)
            logoutButton.onClick.AddListener(OnLogout);
    }

    private void OnEnterMuseum()
    {
        SceneManager.LoadScene(museumSceneName);
    }

    private void OnGrowPlants()
    {
        Debug.Log("Grow Plants scene not implemented yet.");
        // Later: SceneManager.LoadScene(growSceneName);
    }

    private void OnViewCollection()
    {
        Debug.Log("Collection scene not implemented yet.");
        // Later: SceneManager.LoadScene(collectionSceneName);
    }

    private void OnLogout()
    {
        // Optional: you can also sign out from Firebase here if you want
        SceneManager.LoadScene(loginSceneName);
    }
}
