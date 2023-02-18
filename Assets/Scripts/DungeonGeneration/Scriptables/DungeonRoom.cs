using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DungeonRoom : MonoBehaviour
{
    [SerializeField]
    private List<DungeonRoom> adjacentRooms = new List<DungeonRoom>();
    private Guid uniqueId;
    [SerializeField]
    private Vector2Int gridPosition;

    public void ConnectRoom(DungeonRoom room)
    {
        adjacentRooms.Add(room);
    }

    public Guid Id()
    {
        return uniqueId;
    }

    public Vector2Int GridPosition()
    {
        return gridPosition;
    }

    public void Initialize(Dictionary<Vector3, Vector2Int> gridMap, Vector3 position)
    {
        adjacentRooms.Clear();
        uniqueId = new Guid();
        gridPosition = gridMap[position];
    }

    public void DisconnectRoom(DungeonRoom room)
    {
        if (adjacentRooms.Contains(room)) adjacentRooms.Remove(room);
    }

    public void Delete() //Delete this room and all its links with other rooms
    {
        adjacentRooms.ForEach(room =>
        {
            if (room.adjacentRooms.Contains(this))
                room.adjacentRooms.Remove(this);
        });
        //Use Destroy if at runtime
        GameObject.DestroyImmediate(this.gameObject);
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public Vector2 GetPositionInGrid() {
        return new Vector2(0f, 0f);
    }

    public List<DungeonRoom> GetConnectedRooms()
    {
        return adjacentRooms;
    }

    public bool HasAdjacentRoom()
    {
        if (adjacentRooms.Count > 0) return true;
        return false;
    } 

    public static DungeonRoom Create(DungeonData data, Vector3 position, Dictionary<Vector3, Vector2Int> gridMap)
    {
        //This is where the room gets instantiated, change the primitive and pass a prefab instead for the room
        //Eg: var roomObj = DungeonDrawer.DrawSingleObject(position, prefab, data.GetMonoInstance(), scale) as GameObject;
        var roomObj = DungeonDrawer.DrawSingleRoom(position, data.GetRoomPrefab(), data.GetMonoInstance());
        roomObj.AddComponent<DungeonRoom>();
        var addedRoom = roomObj.GetComponent<DungeonRoom>();
        addedRoom.Initialize(gridMap, position);
        data.AddRoom(addedRoom);
        return addedRoom;
    }

    public static bool DFS(DungeonRoom excludedRoom, DungeonRoom currentRoom, DungeonRoom endRoom, HashSet<DungeonRoom> visitedRooms)
    {
        if (currentRoom == endRoom)
        {
            return true; // We've already visited this room, so there's no path from the start to the end
        }

        visitedRooms.Add(currentRoom);

        foreach (DungeonRoom adjacentRoom in currentRoom.adjacentRooms)
        {
            if (!visitedRooms.Contains(adjacentRoom) && adjacentRoom != excludedRoom)
            {
                if (DFS(excludedRoom, adjacentRoom, endRoom, visitedRooms)) return true;
            }
        }
        return false; // We've searched all adjacent rooms and haven't found a path to the end room
    }
    public static bool PathExists(DungeonRoom excludedRoom, DungeonRoom startRoom, DungeonRoom endRoom)
    {
        HashSet<DungeonRoom> visitedRooms = new HashSet<DungeonRoom>();
        return DFS(excludedRoom, startRoom, endRoom, visitedRooms);
    }

}
