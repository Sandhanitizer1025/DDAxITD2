using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuController : MonoBehaviour
{
    [Header("Button")]
    public Button enterGardenButton;

    [Header("Scene Name")]
    public string gardenSceneName = "MainGarden";   
    private void Awake()
    {
        if (enterGardenButton != null)
        {
            enterGardenButton.onClick.AddListener(OnEnterGarden);
        }
        else
        {
            Debug.LogWarning("Enter Garden button not assigned!");
        }
    }

    private void OnEnterGarden()
    {
        SceneManager.LoadScene(gardenSceneName);
    }
}
