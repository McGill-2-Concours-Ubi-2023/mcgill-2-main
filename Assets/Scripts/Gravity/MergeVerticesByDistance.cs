using System;
using System.Collections.Generic;
using UnityEngine;

public class MergeVerticesByDistance : MonoBehaviour
{
    internal static void MergeVertices(float mergeDistance, MeshFilter meshFilter)
    {
        // Get the mesh filter
        if (meshFilter == null)
        {
            Debug.LogError("MergeVerticesByDistance: MeshFilter not found.");
            return;
        }

        // Get the mesh
        Mesh mesh = meshFilter.sharedMesh;
        if (mesh == null)
        {
            Debug.LogError("MergeVerticesByDistance: Mesh not found.");
            return;
        }

        // Create new arrays to store the merged vertices and triangles
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        // Create a dictionary to map old vertex indices to new vertex indices
        Dictionary<int, int> indexMap = new Dictionary<int, int>();

        // Merge vertices that are closer than the threshold distance
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            Vector3 vertex = mesh.vertices[i];
            Vector2 uv = mesh.uv[i];
            int newIndex = vertices.Count;

            // Check if the vertex is close enough to an existing vertex
            foreach (int index in indexMap.Keys)
            {
                Vector3 existingVertex = vertices[indexMap[index]];
                if (Vector3.Distance(vertex, existingVertex) <= mergeDistance)
                {
                    newIndex = indexMap[index];
                    break;
                }
            }

            // Map the old index to the new index
            indexMap[i] = newIndex;

            // Add the vertex to the new list if it hasn't been merged
            if (newIndex == vertices.Count)
            {
                vertices.Add(vertex);
                uvs.Add(uv);
            }
            // Otherwise, average the UVs of the merged vertices
            else
            {
                Vector2 averagedUV = (uvs[newIndex] + uv) / 2f;
                uvs[newIndex] = averagedUV;
            }
        }

        // Update the triangles to use the new vertex indices
        for (int i = 0; i < mesh.triangles.Length; i++)
        {
            int oldIndex = mesh.triangles[i];
            int newIndex = indexMap[oldIndex];
            triangles.Add(newIndex);
        }

        // Create a new mesh with the merged vertices and triangles
        Mesh newMesh = new Mesh();
        newMesh.SetVertices(vertices);
        newMesh.SetUVs(0, uvs);
        newMesh.SetTriangles(triangles, 0);
        newMesh.RecalculateNormals();

        // Assign the new mesh to the object
        meshFilter.sharedMesh = newMesh;
    }
}
