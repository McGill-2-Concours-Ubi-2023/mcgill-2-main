using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FractalCorridor : DungeonComponent
{
    [SerializeField]
    private Vector2Int startPos;
    [SerializeField][Range(1, 500)]
    private int radius = 20;
    [SerializeField]
    private int numberOfSegments = 10;
    [SerializeField]
    private float angleVariation = 45f;
    [SerializeField]
    private float innerRadiusFactor = 10f; //how close or far the child branch is from the parent branch
    [SerializeField]
    int treeDepth = 10;

    public void CreateFractalCorridor()
    {
        Vector3 origin = mono.transform.position;
        Vector3 globalStartPosition = new Vector3(origin.x + startPos.x, origin.y, origin.z + startPos.y);
        GenerateTree(globalStartPosition, radius, numberOfSegments, Random.Range(-angleVariation, angleVariation),
            innerRadiusFactor, treeDepth, positionsBuffer);
    }

    private List<Vector3> GenerateTree(Vector3 center, float radius, int numSegments, float angle, float innerRadiusFactor, int depth, List<Vector3> vertices)
    {
        if (depth <= 0)
        {
            return vertices;
        }

        // Generate vertices for the current segment
        for (int i = 0; i < numSegments; i++)
        {
            Vector3 outerVertex = center + Quaternion.Euler(0, i * angle, 0) * Vector3.forward * radius;
            Vector3 innerVertex = center + Quaternion.Euler(0, i * angle, 0) * Vector3.forward * radius * innerRadiusFactor;
            vertices.Add(outerVertex);
            vertices.Add(innerVertex);
        }

        // Generate child branches
        for (int i = 0; i < numSegments; i++)
        {
            Vector3 childCenter = center + Quaternion.Euler(0, i * angle, 0) * Vector3.forward * radius * innerRadiusFactor;
            float childRadius = radius * innerRadiusFactor;
            List<Vector3> childVertices = new List<Vector3>();
            GenerateTree(childCenter, childRadius, numSegments, angle, innerRadiusFactor, depth - 1, childVertices);
            vertices.AddRange(childVertices);
        }
        return vertices;
    }
}
