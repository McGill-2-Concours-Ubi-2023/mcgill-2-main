using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DungeonRoom : MonoBehaviour
{
    [SerializeField]
    private List<DungeonRoom> adjacentRooms = new List<DungeonRoom>();
    private Guid uniqueId = new Guid();

    public void ConnectRoom(DungeonRoom room)
    {
        adjacentRooms.Add(room);
    }

    public Guid Id()
    {
        return uniqueId;
    }

    public void DisconnectRoom(DungeonRoom room)
    {
        if (adjacentRooms.Contains(room)) adjacentRooms.Remove(room);
    }

    public void Delete()
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

    public static DungeonRoom Create(DungeonData data, Vector3 position)
    {
        //This is where the room gets instantiated, change the primitive and pass a prefab instead for the room
        //Eg: var roomObj = DungeonDrawer.DrawSingleObject(position, prefab, data.GetMonoInstance(), scale) as GameObject;
        var roomObj = DungeonDrawer.DrawSingleRoom(position, PrimitiveType.Cube,
            data.GetMonoInstance(), new Vector3(data.GetGrid().RoomSize(), 1 ,data.GetGrid().RoomSize())) as GameObject;
        roomObj.AddComponent<DungeonRoom>();
        var addedRoom = roomObj.GetComponent<DungeonRoom>();
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
