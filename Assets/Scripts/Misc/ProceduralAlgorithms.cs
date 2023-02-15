using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProceduralAlgorithms
{
    public static void RandomWalk_Default(int numSteps, Vector3 startPos, float stepSize)
    {
        int[] rotations = new int[] { 90, 180, 270, 360, -90, -180, -270, -360 };
        Vector3[] directions = new Vector3[] { Vector3.forward, Vector3.back, Vector3.left, Vector3.right};
        Vector3 currentPos = startPos;
        System.Random rand = new System.Random();

        for (int i = 0; i < numSteps; i++) //O(n^2)
        {         
            int randomDirectionIndex = rand.Next(0, directions.Length);           
            Vector3 randomDirection = directions[randomDirectionIndex];
            int randomRotationIndex = rand.Next(0, rotations.Length);
            int randomRotation = rotations[randomRotationIndex];

            Vector3 direction = Quaternion.Euler(0, randomRotation, 0) * randomDirection;
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.transform.position = currentPos;          
        }
    }
    public static HashSet<Vector2Int> RandomWalk_HashSet(Vector2Int startPos, int stepSize)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();
        path.Add(startPos);
        Vector2Int previousPos = startPos;

        int i;
        for(i = 0; i < stepSize; i++)
        {
            var newPos = previousPos + Direction2D.GetRandomCardinal(); //RandomDirection
            path.Add(newPos);
            previousPos = newPos;
        }
        return path;
    }

    public static HashSet<Vector2Int> RandomRoom(int width, int height, int boundsDeviation, Vector2Int startPosition)
    {
        HashSet<Vector2Int> vertices = new HashSet<Vector2Int>();
        var negativeheightDeviation = Mathf.Max(1, height - boundsDeviation);
        var negativeWidthDeviation = Mathf.Max(1, width - boundsDeviation);
        int heightDeviation = Random.Range(negativeheightDeviation, height + boundsDeviation);
        int widthDeviation = Random.Range(negativeWidthDeviation, width + boundsDeviation);

        for (int x = 0; x <= heightDeviation; x++)
        {
            for (int y = 0; y <= widthDeviation; y++)
            {
                vertices.Add(new Vector2Int(x, y) + startPosition);
            }
        }
        return vertices;
    }

    public static List<Vector2Int> RandomWalkCorridor(Vector2Int startPos, int corridorLength)
    {
        var corridor = new List<Vector2Int>();
        var direction = Direction2D.GetRandomCardinal();
        var currentPos = startPos;
        corridor.Add(currentPos);

        for(int i = 0; i < corridorLength; i++)
        {
            currentPos += direction;
            corridor.Add(currentPos);
        }
        return corridor;
    }



}

public static class Direction2D
{
    public static List<Vector2Int> cardinals = new List<Vector2Int>() 
    { 
        new Vector2Int(0, 1), //UP
        new Vector2Int(1, 0), //RIGHT
        new Vector2Int(0, -1), //DOWN
        new Vector2Int(-1, 0) //LEFT
    }; 

    public static Vector2Int GetRandomCardinal()
    {
        return cardinals[Random.Range(0, cardinals.Count)];
    }
}
