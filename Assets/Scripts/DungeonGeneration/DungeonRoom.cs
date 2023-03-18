using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

[System.Serializable]
public class DungeonRoom : MonoBehaviour
{
    [SerializeField]
    private List<DungeonRoom> adjacentRooms = new List<DungeonRoom>();
    private Guid uniqueId;
    [SerializeField]
    private Vector2Int gridPosition;
    [SerializeField]
    public static DungeonRoom activeRoom;
    [SerializeField]
    public static DungeonDoor lastEnteredDoor;
    [SerializeField]
    private List<DungeonDoor> doors = new List<DungeonDoor>();
    [SerializeField]
    private string layout;
    [SerializeField]
    private RoomTypes.RoomType type;
    [SerializeField]
    private List<OccludableWall> occludableWalls;
    private List<OccludableWall> northWalls;
    private GameObject walls;
    private void Awake()
    {
        if(type == RoomTypes.RoomType.Start)
        {
            occludableWalls.ForEach(wall => wall.Hide());
        }
    }

    public static DungeonRoom GetActiveRoom()
    {
        return activeRoom;
    }

    public void GetBottomRoomOccludableWalls()
    {
        foreach(DungeonRoom room in adjacentRooms)
        {
            if ((room.transform.position - transform.position).normalized == -Vector3.forward)
            {
                room.FindNorthWalls();
                foreach (OccludableWall wall in room.northWalls)
                {
                    if (wall != null)
                    {
                        occludableWalls.Add(wall);
                    }
                    else room.northWalls.Remove(wall);
                }
            }
        }
    }

    public string GetLayout()
    {
        return layout;
    }

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

    public void Initialize(Dictionary<Vector3, Vector2Int> gridMap, Vector3 position, string layout, RoomTypes.RoomType type)
    {
        this.layout = layout;
        adjacentRooms.Clear();
        uniqueId = new Guid();
        gridPosition = gridMap[position];
        this.type = type;
    }

    public static void Cleanup()
    {
        var rooms = FindObjectOfType<DungeonGenerator>().data.AllRooms();
        foreach(DungeonRoom room in rooms)
        {
            if(room.northWalls != null)
            {
                room.northWalls.RemoveAll(wall => wall == null);
            }
            Debug.Assert(room.occludableWalls != null);
            room.occludableWalls.RemoveAll(wall => wall == null);
        }     
    }

    public void DisconnectRoom(DungeonRoom room)
    {
        if (adjacentRooms.Contains(room)) adjacentRooms.Remove(room);
    }

    public RoomTypes.RoomType GetRoomType()
    {
        return type;
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

    public List<DungeonRoom> GetConnectedRooms()
    {
        return adjacentRooms;
    }

    public bool HasAdjacentRoom()
    {
        if (adjacentRooms.Count > 0) return true;
        return false;
    } 

    public void Enter()
    {
        activeRoom = this;
        //TODO: add behaviour
    }

    public void GenerateDoors(DungeonData data)
    {
        adjacentRooms.ForEach(room =>
        {     
            if(room.doors.Count != 0 && !room.HasAccessToRoom(this)
            || room.doors.Count == 0) 
            {               
                var addedDoor = DungeonDoor.Create(gameObject, this, room, data);
                doors.Add(addedDoor);                
            } else if (room.HasAccessToRoom(this))
            {
                room.doors.Where(door => door.GetSharedRooms().Contains(this))
                .ToList().ForEach(door => this.doors.Add(door));
            }                 
        });
        BindWalls(walls);
    }

    private bool HasAccessToRoom(DungeonRoom room)
    {   
        foreach(var door in doors)
        {
            if (door.GetSharedRooms().Contains(room))
                return true;
        }
        return false;
    }

    public static DungeonRoom CreateRandomRoom(DungeonData data, Vector3 position, Dictionary<Vector3, Vector2Int> gridMap,
        string layout, RoomTypes.RoomType type)
    {
        //This is where the room gets instantiated, change the primitive and pass a prefab instead for the room
        //Eg: var roomObj = DungeonDrawer.DrawSingleObject(position, prefab, data.GetMonoInstance(), scale) as GameObject;
        var roomObj = DungeonDrawer.DrawRandomRoom(position, type, data);
        roomObj.AddComponent<DungeonRoom>();
        var newRoom = roomObj.GetComponent<DungeonRoom>();
        newRoom.Initialize(gridMap, position, layout, type);
        data.AddRoom(newRoom);
        newRoom.walls = DungeonDrawer.DrawSingleObject(newRoom.GetPosition(), data.GetWallPrefab(), newRoom.gameObject);
        return newRoom;
    }

    private void BindWalls(GameObject walls)
    {
        Debug.Log(walls == null);
        var southWestCornerWall = walls.transform.Find("Corner").GetComponentInChildren<OccludableWall>();
        var southEastCotnerWall = walls.transform.Find("Corner4").GetComponentInChildren<OccludableWall>();
        var middleWallNoDoor = walls.transform.Find("PlainWall1").GetComponentInChildren<OccludableWall>();
        var middleWallDoor = walls.transform.Find("DoorWall1").GetComponentInChildren<OccludableWall>();
        if (occludableWalls == null) occludableWalls = new List<OccludableWall>();
        occludableWalls.Add(southWestCornerWall);
        occludableWalls.Add(southEastCotnerWall);
        occludableWalls.Add(middleWallDoor);
        occludableWalls.Add(middleWallNoDoor);
        GetBottomRoomOccludableWalls();
    }

    private void FindNorthWalls()
    {
        var topEastCornerWall = walls.transform.Find("Corner2").GetComponentInChildren<OccludableWall>();
        var topWestCornerWall = walls.transform.Find("Corner3").GetComponentInChildren<OccludableWall>();
        var topMiddleWallDoor = walls.transform.Find("PlainWall2").GetComponentInChildren<OccludableWall>();
        var topMiddleWallNoDoor = walls.transform.Find("DoorWall2").GetComponentInChildren<OccludableWall>();
        if (northWalls == null) northWalls = new List<OccludableWall>();
        northWalls.Add(topEastCornerWall);
        northWalls.Add(topWestCornerWall);
        northWalls.Add(topMiddleWallNoDoor);
        northWalls.Add(topMiddleWallDoor);
    }

    public List<OccludableWall> GetOccludableWalls()
    {
        return occludableWalls;
    }

    public static DungeonRoom CreateRoomFromData(RoomData roomData, DungeonData dungeonData, Dictionary<Vector3, Vector2Int> gridMap, string layout)
    {
        var roomObj = DungeonDrawer.DrawRoomFromData(roomData, dungeonData);
        roomObj.AddComponent<DungeonRoom>();
        var loadedRoom = roomObj.GetComponent<DungeonRoom>();
        loadedRoom.Initialize(gridMap, roomData.GetPosition(), layout, roomData.GetRoomType());
        dungeonData.AddRoom(loadedRoom);
        loadedRoom.walls = DungeonDrawer.DrawSingleObject(loadedRoom.GetPosition(), dungeonData.GetWallPrefab(), loadedRoom.gameObject);
        return loadedRoom;
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
