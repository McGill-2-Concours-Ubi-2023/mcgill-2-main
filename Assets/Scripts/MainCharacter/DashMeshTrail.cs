using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashMeshTrail : MonoBehaviour
{
    public float activeTime = 0.5f;
    public bool isTrailActive = false;
    [Header("Trail Mesh")][Range(0.01f, 1.0f)]
    public float refreshRate = 0.1f;
    [SerializeField]
    private SkinnedMeshRenderer[] skinnedMeshRenderers;
    [Header("Shader related")]
    public Material trailMaterial;
    public float alphaDecreaseRate = 0.1f;
    public float alphaDecreaseRefreshRate = 0.05f;

    public void ActivateTrail()
    {
        StartCoroutine(Activate(activeTime));
    }

    IEnumerator Activate(float cooldown)
    {
        isTrailActive = true;
        while(cooldown > 0)
        {
            cooldown -= refreshRate;

            if(skinnedMeshRenderers == null)
                skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

            for(int i = 0; i < skinnedMeshRenderers.Length; i++)
            {
                GameObject obj = new GameObject();
                MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
                MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
                Mesh mesh = new Mesh();
                skinnedMeshRenderers[i].BakeMesh(mesh);
                meshFilter.mesh = mesh;
                var _transform = skinnedMeshRenderers[i].transform;
                obj.transform.rotation = _transform.rotation;
                obj.transform.position = _transform.position;
                meshRenderer.material = trailMaterial;
                StartCoroutine(FadeMaterial(meshRenderer.material, 0, alphaDecreaseRate, alphaDecreaseRefreshRate));
                Destroy(obj, activeTime * 2);
            }           
            yield return new WaitForSeconds(refreshRate);
        }
        isTrailActive = false;
    }

    IEnumerator FadeMaterial(Material mat, float goal, float rate, float refreshRate)
    {
        float valueToAnimate = mat.GetFloat("_Alpha");
        while(valueToAnimate > goal)
        {
            valueToAnimate -= rate;
            mat.SetFloat("_Alpha", valueToAnimate);
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
