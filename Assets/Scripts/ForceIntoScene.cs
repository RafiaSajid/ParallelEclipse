using UnityEngine;
using UnityEngine.SceneManagement;

public class ForceIntoScene : MonoBehaviour
{
    // The target scene name should match the name of the file this script is in
    [SerializeField]
    private string targetSceneName = "MapAdditive";

    void Start()
    {
        // 1. Get a reference to the scene this object is SUPPOSED to belong to.
        Scene mapScene = SceneManager.GetSceneByName(targetSceneName);

        // 2. Critical Check: Ensure the scene is actually valid and loaded.
        if (mapScene.IsValid() && mapScene.isLoaded)
        {
            // 3. Move the object (and all its children) into the correct scene.
            SceneManager.MoveGameObjectToScene(gameObject, mapScene);
            Debug.Log($"Moved '{gameObject.name}' into its correct scene: {targetSceneName}");
        }
        else
        {
            // This happens if the scene loading failed or the name is wrong.
            Debug.LogError($"Cannot find or move object. Scene '{targetSceneName}' is not valid or not loaded.");
        }
    }
}
//16 6.5 2.57 60.2 -0.88 0.36