using UnityEngine;

[ExecuteInEditMode]
public class RenderTextureGenerator : MonoBehaviour
{
    private Camera _camera;
    public RenderTexture _renderTexture;
    public Renderer _renderer;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _renderTexture = new RenderTexture(Camera.main.pixelWidth, Camera.main.pixelHeight, 24);
    }

    private void Update()
    {
        _camera.targetTexture = _renderTexture;
        if (_renderer) _renderer.material.SetTexture("_TextureMap", _renderTexture);
    }
}
