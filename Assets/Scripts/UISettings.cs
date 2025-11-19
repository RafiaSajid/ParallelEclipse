using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;


// ===================================================================
// JSON Config Class (MUST NOT BE SEALED) + Serializable
// ===================================================================
[System.Serializable]
public class UISettings
{
    public string additiveMapScene = "MapAdditive";
    public KeyCode mapToggleKey = KeyCode.LeftControl;
    public float roomButtonDelay = 10f;
    public string roomSceneToLoad = "Post-apocalyptic Scene";
}



// ===================================================================
// UI Manager Singleton (NOT sealed – Unity requires this!)
// ===================================================================
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject roomButton;

    private UISettings settings;
    private string settingsPath;
    private bool mapLoaded = false;


    // ===================================================================
    private void Awake()
    {
        // ---------------- Singleton ----------------
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);

        // JSON setup
        settingsPath = Path.Combine(Application.persistentDataPath, "ui_settings.json");
        LoadSettings();

        if (roomButton != null)
            roomButton.SetActive(false);
    }


    private void Start()
    {
        StartCoroutine(RoomButtonReveal());
    }


    private void Update()
    {
        HandleMapInput();
    }



    // ===================================================================
    // MAP TOGGLE LOGIC
    // ===================================================================
    private void HandleMapInput()
    {
        if (Input.GetKeyDown(settings.mapToggleKey) ||
            (settings.mapToggleKey == KeyCode.LeftControl &&
             Input.GetKeyDown(KeyCode.RightControl)))
        {
            ToggleMap();
        }
    }


    private void ToggleMap()
    {
        if (mapLoaded)
            UnloadMap();
        else
            LoadMap();
    }


    private void LoadMap()
    {
        if (mapLoaded) return;

        SceneManager.LoadSceneAsync(settings.additiveMapScene, LoadSceneMode.Additive);
        mapLoaded = true;

        Debug.Log("Map scene loaded additively.");
    }


    private void UnloadMap()
    {
        if (!mapLoaded) return;

        SceneManager.UnloadSceneAsync(settings.additiveMapScene);
        mapLoaded = false;

        Debug.Log("Map scene unloaded.");
    }



    // ===================================================================
    // ROOM BUTTON REVEAL
    // ===================================================================
    private IEnumerator RoomButtonReveal()
    {
        yield return new WaitForSeconds(settings.roomButtonDelay);

        if (roomButton != null)
            roomButton.SetActive(true);
    }


    public void OnRoomButtonClick()
    {
        SceneManager.LoadScene(settings.roomSceneToLoad);
    }



    // ===================================================================
    // JSON HANDLING
    // ===================================================================
    private void LoadSettings()
    {
        if (!File.Exists(settingsPath))
        {
            settings = new UISettings();
            File.WriteAllText(settingsPath, JsonUtility.ToJson(settings, true));
            return;
        }

        string json = File.ReadAllText(settingsPath);
        settings = JsonUtility.FromJson<UISettings>(json);

        if (settings == null)
            settings = new UISettings();
    }
}
