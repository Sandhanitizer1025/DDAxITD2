using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FirebaseManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_Text statusText;
    public Button loginButton;
    public Button registerButton;

    [Header("Scene")]
    public string sceneToLoad = "StartMenu";

    private FirebaseAuth auth;
    private bool isFirebaseReady = false;

    async void Start()
    {
        if (loginButton != null)
        {
            loginButton.onClick.AddListener(LoginUser);
            Debug.Log("Login button connected");
        }
        else
        {
            Debug.LogWarning("Login button not assigned in Inspector!");
        }

        if (registerButton != null)
        {
            registerButton.onClick.AddListener(RegisterUser);
            Debug.Log("Register button connected");
        }
        else
        {
            Debug.LogWarning("Register button not assigned in Inspector!");
        }

        if (statusText != null)
        {
            statusText.text = "Initializing Firebase...";
        }
        else
        {
            Debug.LogWarning("Status text not assigned in Inspector!");
        }

        await InitializeFirebase();
    }

    private async Task InitializeFirebase()
    {
        Debug.Log("Starting Firebase initialization...");

        var result = await FirebaseApp.CheckAndFixDependenciesAsync();

        Debug.Log($"Firebase dependency check result: {result}");

        if (result == DependencyStatus.Available)
        {
            auth = FirebaseAuth.DefaultInstance;
            isFirebaseReady = true;
            Debug.Log("Firebase Auth Ready!");
            if (statusText != null)
                statusText.text = "Firebase Ready - Enter credentials";
        }
        else
        {
            Debug.LogError($"Firebase not ready: {result}");
            if (statusText != null)
                statusText.text = $"Firebase Error: {result}";
        }
    }

    // ================= REGISTER =================
    public async void RegisterUser()
    {
        if (!isFirebaseReady)
        {
            if (statusText != null)
                statusText.text = "Firebase not initialized!";
            Debug.LogError("Attempted to register before Firebase was ready");
            return;
        }

        string email = emailInput.text.Trim();
        string password = passwordInput.text.Trim();

        Debug.Log($"Attempting registration for: {email}");
        if (statusText != null)
            statusText.text = "Registering...";

        try
        {
            AuthResult authResult = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            FirebaseUser user = authResult.User;

            Debug.Log($"Registered! UID = {user.UserId}");
            if (statusText != null)
                statusText.text = $"Registered as: {user.Email}";

            Debug.Log($"Loading scene: {sceneToLoad}");
            SceneManager.LoadScene(sceneToLoad);
        }
        catch (FirebaseException fe)
        {
            Debug.LogError($"Firebase Register Error: {fe.ErrorCode} - {fe.Message}");
            if (statusText != null)
                statusText.text = $"Error: {fe.Message}";
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Register Error: {e.Message}");
            if (statusText != null)
                statusText.text = $"Error: {e.Message}";
        }
    }

    // ================= LOGIN =================
    public async void LoginUser()
    {
        if (!isFirebaseReady)
        {
            if (statusText != null)
                statusText.text = "Firebase not initialized!";
            Debug.LogError("Attempted to login before Firebase was ready");
            return;
        }

        string email = emailInput.text.Trim();
        string password = passwordInput.text.Trim();

        Debug.Log($"Attempting login for: {email}");
        if (statusText != null)
            statusText.text = "Logging in...";

        try
        {
            AuthResult authResult = await auth.SignInWithEmailAndPasswordAsync(email, password);
            FirebaseUser user = authResult.User;

            Debug.Log($"Logged in! UID = {user.UserId}");
            if (statusText != null)
                statusText.text = $"Logged in as: {user.Email}";

            // ✅ NEW: go to StartMenu after login too
            Debug.Log($"Loading scene: {sceneToLoad}");
            SceneManager.LoadScene(sceneToLoad);
        }
        catch (FirebaseException fe)
        {
            Debug.LogError($"Firebase Login Error: {fe.ErrorCode} - {fe.Message}");
            if (statusText != null)
                statusText.text = $"Error: {fe.Message}";
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Login Error: {e.Message}");
            if (statusText != null)
                statusText.text = $"Error: {e.Message}";
        }
    }
}
