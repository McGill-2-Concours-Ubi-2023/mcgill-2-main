#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[ExecuteAlways]
public class SceneRenderer : MonoBehaviour
{
    [SerializeField]
    private RenderTexture sceneTexture;
    private Camera sceneCamera;

    private void Awake()
    {
        sceneCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (sceneCamera == null) sceneCamera = GetComponent<Camera>();
        sceneCamera.targetTexture = sceneTexture;
    }
}
