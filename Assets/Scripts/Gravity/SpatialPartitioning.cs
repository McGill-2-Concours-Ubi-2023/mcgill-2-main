using System.Collections.Generic;
using UnityEngine;

public class SpatialPartitioning : MonoBehaviour
{
    public float gridSize = 2f;
    private Dictionary<Vector2Int, List<Collider>> grid = new Dictionary<Vector2Int, List<Collider>>();

    public void RegisterCollider(Collider collider)
    {
        // Get the bounds of the collider
        Bounds bounds = collider.bounds;

        // Calculate the grid coordinates of the collider
        Vector2Int min = new Vector2Int(
            Mathf.FloorToInt(bounds.min.x / gridSize),
            Mathf.FloorToInt(bounds.min.z / gridSize)
        );
        Vector2Int max = new Vector2Int(
            Mathf.FloorToInt(bounds.max.x / gridSize),
            Mathf.FloorToInt(bounds.max.z / gridSize)
        );

        // Add the collider to each grid cell it overlaps
        for (int x = min.x; x <= max.x; x++)
        {
            for (int y = min.y; y <= max.y; y++)
            {
                Vector2Int cell = new Vector2Int(x, y);
                if (!grid.ContainsKey(cell))
                {
                    grid[cell] = new List<Collider>();
                }
                grid[cell].Add(collider);
            }
        }
    }

    public List<Collider> GetNearbyColliders(Vector3 position, float radius)
    {
        // Calculate the grid coordinates of the sphere
        int minX = Mathf.FloorToInt((position.x - radius) / gridSize);
        int maxX = Mathf.FloorToInt((position.x + radius) / gridSize);
        int minY = Mathf.FloorToInt((position.z - radius) / gridSize);
        int maxY = Mathf.FloorToInt((position.z + radius) / gridSize);

        // Get the colliders from each grid cell that the sphere overlaps
        List<Collider> nearbyColliders = new List<Collider>();
        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                Vector2Int cell = new Vector2Int(x, y);
                if (grid.ContainsKey(cell))
                {
                    foreach (Collider collider in grid[cell])
                    {
                        // Check if the collider is within the sphere's radius
                        if ((collider.transform.position - position).sqrMagnitude <= radius * radius)
                        {
                            nearbyColliders.Add(collider);
                        }
                    }
                }
            }
        }
        return nearbyColliders;
    }

    public void ClearGrid()
    {
        grid.Clear();
    }
}
