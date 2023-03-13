using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashMeshTrail : MonoBehaviour
{
    public float activeTime = 5.0f;
    public bool isTrailActive = false;
    [Header("Trail Mesh")][Range(0.01f, 1.0f)]
    public float refreshRate = 0.1f;
    [SerializeField]
    private SkinnedMeshRenderer[] skinnedMeshRenderers;
    // Start is called before the first frame update
    void Start()
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
            }
            
            yield return new WaitForSeconds(refreshRate);
        }
        isTrailActive = false;
    }
}
