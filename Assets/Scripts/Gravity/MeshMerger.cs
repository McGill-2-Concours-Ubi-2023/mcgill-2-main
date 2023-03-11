using UnityEngine;

public class MeshMerger : MonoBehaviour
{
    public GameObject object1;
    public GameObject object2;
    public bool destroyOriginals = true;
    public float mergeDistance = 0.01f;

    void Start()
    {
        // Get the meshes from the two objects
        Mesh mesh1 = object1.GetComponent<MeshFilter>().mesh;
        Mesh mesh2 = object2.GetComponent<MeshFilter>().mesh;

        // Combine the meshes into a single mesh
        CombineInstance[] combine = new CombineInstance[2];
        combine[0].mesh = mesh1;
        combine[0].transform = object1.transform.localToWorldMatrix;
        combine[1].mesh = mesh2;
        combine[1].transform = object2.transform.localToWorldMatrix;
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine, true, true);

        // Create a new game object to hold the combined mesh
        GameObject combinedObject = new GameObject("Combined Mesh");
        combinedObject.transform.position = Vector3.zero;
        combinedObject.transform.rotation = Quaternion.identity;

        // Add a mesh filter and mesh renderer to the new game object
        MeshFilter meshFilter = combinedObject.AddComponent<MeshFilter>();
        meshFilter.mesh = combinedMesh;
        MeshRenderer meshRenderer = combinedObject.AddComponent<MeshRenderer>();
        meshRenderer.material = object1.GetComponent<MeshRenderer>().material;

        // Destroy the original objects if desired
        if (destroyOriginals)
        {
            Destroy(object1);
            Destroy(object2);
        }
        MergeVerticesByDistance.MergeVertices(mergeDistance, meshFilter);
    }
}
