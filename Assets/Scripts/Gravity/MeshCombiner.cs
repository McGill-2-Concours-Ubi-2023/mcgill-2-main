using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshCombiner : MonoBehaviour
{
    public MeshFilter[] meshFilters;
    public float mergeDistance = 0.01f;

    void Start()
    {
        // Combine all the meshes into a single mesh
        Mesh combinedMesh = new Mesh();
        CombineInstance[] combineInstances = new CombineInstance[meshFilters.Length];
        for (int i = 0; i < meshFilters.Length; i++)
        {
            combineInstances[i].mesh = meshFilters[i].sharedMesh;
            combineInstances[i].transform = meshFilters[i].transform.localToWorldMatrix;
        }
        combinedMesh.CombineMeshes(combineInstances);

        // Merge vertices that are close together
        Vector3[] oldVertices = combinedMesh.vertices;
        List<Vector3> newVertices = new List<Vector3>();
        int[] map = new int[oldVertices.Length];
        for (int i = 0; i < oldVertices.Length; i++)
        {
            bool found = false;
            for (int j = 0; j < newVertices.Count; j++)
            {
                if (Vector3.Distance(oldVertices[i], newVertices[j]) < mergeDistance)
                {
                    map[i] = j;
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                map[i] = newVertices.Count;
                newVertices.Add(oldVertices[i]);
            }
        }
        combinedMesh.vertices = newVertices.ToArray();

        // Find the overlapping surface and remove it
        List<int> triangles = new List<int>(combinedMesh.triangles);
        int triangleCount = triangles.Count / 3;
        for (int i = 0; i < triangleCount; i++)
        {
            int i1 = triangles[i * 3];
            int i2 = triangles[i * 3 + 1];
            int i3 = triangles[i * 3 + 2];
            Vector3 v1 = combinedMesh.vertices[i1];
            Vector3 v2 = combinedMesh.vertices[i2];
            Vector3 v3 = combinedMesh.vertices[i3];
            Vector3 normal = Vector3.Cross(v2 - v1, v3 - v1).normalized;
            for (int j = i + 1; j < triangleCount; j++)
            {
                int j1 = triangles[j * 3];
                int j2 = triangles[j * 3 + 1];
                int j3 = triangles[j * 3 + 2];
                Vector3 w1 = combinedMesh.vertices[j1];
                Vector3 w2 = combinedMesh.vertices[j2];
                Vector3 w3 = combinedMesh.vertices[j3];
                if (Vector3.Dot(normal, Vector3.Cross(w2 - w1, w3 - w1).normalized) > 0.9f)
                {
                    // Found overlapping triangles
                    int[] overlapIndices = new int[] { i1, i2, i3, j1, j2, j3 };
                    List<int> indicesToRemove = new List<int>();
                    for (int k = 0; k < overlapIndices.Length; k++)
                    {
                        int index = overlapIndices[k];
                        int count = indicesToRemove.Count;
                        for (int l = 0; l < count; l++)
                        {
                            if (indicesToRemove[l] == map[index])
                            {
                                index = -1;
                                break;
                            }
                        }
                        if (index >= 0)
                        {
                            indicesToRemove.Add(map[index]);
                        }
                    }
                    if (indicesToRemove.Count >= 3)
                    {
                        // Remove overlapping triangles
                        for (int k = 0; k < indicesToRemove.Count; k++)
                        {
                            int index = indicesToRemove[k];
                            for (int l = 0; l < triangles.Count; l++)
                            {
                                if (triangles[l] == index)
                                {
                                    triangles.RemoveAt(l + 2);
                                    triangles.RemoveAt(l + 1);
                                    triangles.RemoveAt(l);
                                    l -= 3;
                                }
                                else if (triangles[l] > index)
                                {
                                    triangles[l]--;
                                }
                            }
                            for (int l = 0; l < map.Length; l++)
                            {
                                if (map[l] > index)
                                {
                                    map[l]--;
                                }
                            }
                        }
                    }
                }
            }
        }
        combinedMesh.triangles = triangles.ToArray();
        combinedMesh.RecalculateNormals();

        // Assign the combined mesh to the object
        GetComponent<MeshFilter>().sharedMesh = combinedMesh;
    }
}
