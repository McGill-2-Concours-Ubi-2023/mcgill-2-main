using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class RandomWalk : DungeonComponent
{
    [SerializeField]
    private Vector2Int startPos;
    [SerializeField]
    private int iterations;
    [SerializeField]
    private int stepSize;
    [SerializeField]
    private bool randomStartLocation = true;
    private HashSet<Vector2Int> floorPositions;

    public void ExecuteRandomWalk()
    {
        GenerateRoom(startPos);
    }

    //Walls need a reference to generate
    public HashSet<Vector2Int> GetRooms2DLayout()
    {
        return floorPositions;
    }

    public HashSet<Vector2Int> GenerateRoom(Vector2Int startPos)
    {
        floorPositions = new HashSet<Vector2Int>();
        var currentPos = startPos;
        for(int i = 0; i < iterations; i++)
        {
            var path = ProceduralAlgorithms.RandomWalk_HashSet(currentPos, stepSize);
            floorPositions.UnionWith(path); //Do not want to copy duplicates
            if (randomStartLocation)
            {
                //We don't want to start walking outside of our current islands (i.e. we don't lose continuity)
                currentPos = floorPositions.ElementAt(UnityEngine.Random.Range(0, floorPositions.Count));
            }           
        }
        foreach (var position in floorPositions)
        {
            positionsBuffer.Add(new Vector3(position.x, mono.transform.position.y, position.y));
        }
        return floorPositions;
    }
    public HashSet<Vector2Int> GenerateRoomStandard(Vector2Int startPos)
    {
        floorPositions = new HashSet<Vector2Int>();
        var currentPos = startPos;

        for (int i = 0; i < iterations; i++)
        {
            var path = ProceduralAlgorithms.RandomRoom(10, 8, 6, currentPos);
            if (randomStartLocation)
            {
                //We don't want to start walking outside of our current islands (i.e. we don't lose continuity)
                var nextDirection = Direction2D.GetRandomCardinal();
                currentPos += nextDirection * stepSize;
                if (floorPositions.Contains(currentPos))
                {
                    currentPos += nextDirection * stepSize;
                } else
                {
                    floorPositions.UnionWith(path); //Do not want to copy duplicates
                }
            }
        }

        foreach (var position in floorPositions)
        {
            positionsBuffer.Add(new Vector3(position.x, mono.transform.position.y, position.y));
        }
        return floorPositions;
    }

}
