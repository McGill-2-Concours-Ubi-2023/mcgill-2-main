using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class StandardCorridor: DungeonComponent
{
    [SerializeField]
    private Vector2Int startPos;
    [SerializeField]
    private int corridorLength, corridorCount;
    [SerializeField]
    private bool randomizeLength;
    [SerializeField]
    [Range(0.1f, 1f)]
    public float roomPercent = 0.8f;
    private HashSet<Vector2Int> corridorPositions;

    public void GenerateCorridors(RandomWalk walkGenerator)
    {
        corridorPositions = new HashSet<Vector2Int>();
        HashSet<Vector2Int> potentialRoomPositions = new HashSet<Vector2Int>();
        CreateCorridors(corridorPositions, startPos, potentialRoomPositions);
        HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPositions, walkGenerator);
        corridorPositions.UnionWith(roomPositions); //Now everything is merge and no conflicting points
        foreach (var position in corridorPositions)
        {
            positionsBuffer.Add(new Vector3(position.x, mono.transform.position.y, position.y));
        }
    }

    public HashSet<Vector2Int> GetCorridorsLayout()
    {
        return corridorPositions;
    }

    private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomPositions, RandomWalk walkGenerator)
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        int numRooms = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent);
        List<Vector2Int> roomsGenerationPoints = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(numRooms).ToList();
        foreach(var point in roomsGenerationPoints)
        {
            var roomFloor = walkGenerator.GenerateRoom(point);
            roomPositions.UnionWith(roomFloor); //roomPosition[i] is part of the corridor          
        }
        //At this point, we only drew the rooms and we still have to merge the existing
        //corridors positions with the newly created data for the rooms
        return roomPositions; 
    }

    private void CreateCorridors(HashSet<Vector2Int> floorPositions, Vector2Int startPos, HashSet<Vector2Int> potentialRoomPositions)
    {
        var currentPosition = startPos;
        potentialRoomPositions.Add(currentPosition);
        for(int i = 0; i < corridorCount; i++)
        {
            var corridor = ProceduralAlgorithms.RandomWalkCorridor(currentPosition, corridorLength);
            currentPosition = corridor[corridor.Count - 1]; //last position in list is the new position to create a corridor from
            potentialRoomPositions.Add(currentPosition);
            floorPositions.UnionWith(corridor);         
        }
    }
}
