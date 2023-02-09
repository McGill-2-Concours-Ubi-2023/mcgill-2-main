using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
public class SceneListGenerator : MonoBehaviour
{
    public ClickSound cs; 
    public string sceneFolder = "Assets/Scenes";
    public Transform ScenesManager; 
    public GameObject sceneButtonFrefab;
    // Start is called before the first frame update
    private void Start()
    {
        
        // Get all the scenes in the folder.
        string[] scenePaths = GetScenePaths(sceneFolder);

        //instantiate the buttonlist.
        foreach (string scenePath in scenePaths)
        {
            // get the scene.
            Scene scene = EditorSceneManager.GetSceneByPath(scenePath);

            // Instantiate button
            GameObject instance = Instantiate(sceneButtonFrefab);
            instance.transform.SetParent(ScenesManager, false);
            instance.GetComponentInChildren<TMP_Text>().text = scenePath;
            instance.GetComponent<Button>().onClick.AddListener(delegate { LoadScene(scenePath); });
            instance.GetComponent<Button>().onClick.AddListener(Click);
        }
    }

    private void LoadScene(string path)
    {
        //EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
        SceneManager.LoadScene(path);
    }
    private void Click() {
        cs.Click();
    }

    private string[] GetScenePaths(string folderPath)
    {
        // Get all the assets in the folder.
        string[] assetPaths = AssetDatabase.FindAssets("t:Scene", new[] { folderPath });

        // Get the full path of each scene.
        string[] scenePaths = new string[assetPaths.Length];
        for (int i = 0; i < assetPaths.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(assetPaths[i]);
            scenePaths[i] = assetPath;
        }

        return scenePaths;
    }


}
