using UnityEngine;
using Firebase;

public class FirebaseInitializer : MonoBehaviour
{
    private FirebaseApp app;

    private void Awake()
    {
        InitializeFirebase();
    }

    private async void InitializeFirebase()
    {
        Debug.Log("Initializing Firebase...");

        try
        {
            // Check that all dependencies are available
            var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
            if (dependencyStatus != DependencyStatus.Available)
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                return;
            }

            // Initialize the default app
            app = FirebaseApp.DefaultInstance;

            Debug.Log("Firebase initialized successfully.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Firebase initialization failed: {ex.Message}");
        }
    }
}

[System.Serializable]
public class FirebaseConfig
{
    public string type;
    public string project_id;
    public string private_key_id;
    public string private_key;
    public string client_email;
    public string client_id;
    public string auth_uri;
    public string token_uri;
    public string auth_provider_x509_cert_url;
    public string client_x509_cert_url;
    public string universe_domain;
}