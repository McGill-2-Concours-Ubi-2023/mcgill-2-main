using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonWallGenerator : DungeonComponent
{
    public void CreateWalls(HashSet<Vector2Int> floorPositions)
    {
        FindWallsInCardinals(floorPositions);
    }
  
    private void FindWallsInCardinals(HashSet<Vector2Int> floorPositions)
    {
        foreach (var position in floorPositions)
        {
            foreach(var direction in Direction2D.cardinals)
            {
                Vector2Int neighbourPosition = position + direction;
                if (!floorPositions.Contains(neighbourPosition))
                {
                    positionsBuffer.Add(new Vector3(neighbourPosition.x, mono.transform.position.y, neighbourPosition.y));
                }
            }
        }
    }
}
