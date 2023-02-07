using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SceneListGenerator : MonoBehaviour
{
    public string sceneFolder = "Assets/Scenes";
    [FormerlySerializedAs("ScenesManager")]
    public Transform sceneManager;
    [FormerlySerializedAs("sceneButtonFrefab")]
    public GameObject sceneButtonPrefab;

    // Start is called before the first frame update
    private void Start()
    {
        // Get all the scenes in the folder.
        IEnumerable<string> scenes = GetAllScenePaths();

        //instantiate the buttonlist.
        foreach (string scenePath in scenes)
        {
            // Instantiate button
            GameObject instance = Instantiate(sceneButtonPrefab);
            instance.transform.SetParent(sceneManager, false);
            instance.GetComponentInChildren<TMP_Text>().text = scenePath;
            instance.GetComponent<Button>().onClick.AddListener(() =>
            {
                SceneManager.LoadScene(scenePath);
            });
        }
    }
    
    private IEnumerable<string> GetAllScenePaths()
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        for( int i = 0; i < sceneCount; i++ )
        {
            yield return SceneUtility.GetScenePathByBuildIndex(i);
        }
    }
}
