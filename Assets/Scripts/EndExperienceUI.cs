using UnityEngine;
using UnityEngine.SceneManagement;

public class EndExperienceUI : MonoBehaviour
{
    public GameObject endPanel;
    public string startSceneName = "StartMenu";

    void Start()
    {
        if (endPanel != null)
            endPanel.SetActive(false);
    }

    // Called when ALL plants are matured
    public void ShowEnd()
    {
        if (endPanel != null)
            endPanel.SetActive(true);
    }

    // Button: Continue Exploring
    public void ContinueExploring()
    {
        if (endPanel != null)
            endPanel.SetActive(false);
    }

    // Button: Return to Start Menu
    public void ReturnToStart()
    {
        SceneManager.LoadScene(startSceneName);
    }
}
