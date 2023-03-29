using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;


[System.Serializable]
public class DungeonRoom : MonoBehaviour
{
    public static DungeonGenerator dungeonGenerator;
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
    [SerializeField]
    private List<OccludableWall> northWalls;
    [SerializeField][HideInInspector]
    private GameObject walls;
    private bool isIsolated;
    [SerializeField]
    private List<EnemyAI> enemies;
    private bool areEnemiesPresent;

    internal void OccludeRooms()
    {
        var allRooms = dungeonGenerator.data.AllRooms();
        foreach (var room in allRooms)
        {
            if (!adjacentRooms.Contains(room) && room != this && room !=  null)
            {
                room.gameObject.SetActive(false);
            }
            else room.gameObject.SetActive(true);
        }
    }

    public static DungeonRoom GetActiveRoom()
    {
        return activeRoom;
    }

    public GameObject GetWalls()
    {
        return walls;
    }

    private void Awake()
    {
        dungeonGenerator = FindObjectOfType<DungeonGenerator>();
    }

    public void SpawnEnemies()
    {
        enemies = new List<EnemyAI>();
        GetComponent<EnemySpawn1>().enabled = true;
        areEnemiesPresent = true;
        Vector3 detectionRange = transform.Find("RoomCollider").GetComponent<BoxCollider>().size;
        StartCoroutine(CheckForAI(detectionRange));
        Isolate();
    }

    public void UpdateWalls()
    {      
       foreach(OccludableWall wall in occludableWalls)
        {
            if (wall != null)
            {
                wall.Hide();
                wall.ChangeRenderQueue(3001);
            }
            
        }

        if (northWalls == null) FindNorthWalls();
        foreach(OccludableWall wall in northWalls)
        {
            if (wall != null) wall.ChangeRenderQueue(2998);
        }    
    }

    IEnumerator CheckForAI(Vector3 detectionRange)
    {
        while (areEnemiesPresent)
        {
            //check every one second for enemies in the room
            yield return new WaitForSeconds(1.0f);
            Collider[] colliders = Physics.OverlapBox(transform.position, detectionRange);
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Enemy"))
                {
                    var enemy = collider.GetComponent<EnemyAI>();
                    if (!enemies.Contains(enemy) && enemy != null) 
                    {
                        enemies.Add(enemy);
                        enemy.AttachRoom(this);
                    }
                }
                areEnemiesPresent = enemies.Count > 0;
            }
        }
        OpenUp();
    }

    public void StopSpawnEnemies()
    {
        GetComponent<EnemySpawn1>().enabled = false;
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
        DungeonData.SafeDestroy(this.gameObject);
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

    public void Isolate()
    {
        foreach(DungeonDoor door in doors)
        {
            door.Block();
        }
    }

    //FBI ???
    public void OpenUp()
    {
        foreach (DungeonDoor door in doors)
        {
            door.Unlock();
        }
    }

    public bool IsIsolated()
    {
        return isIsolated;
    }

    public void ReassignRoom(DungeonRoom existingRoom, RoomTypes.RoomType type)
    {
        this.adjacentRooms = existingRoom.adjacentRooms;
        this.doors = existingRoom.doors;
        this.layout = existingRoom.layout;
        this.type = type;
        this.occludableWalls = existingRoom.occludableWalls;
        this.northWalls = existingRoom.northWalls;
        this.walls = existingRoom.walls;
        existingRoom.walls.transform.parent = this.transform;      
        foreach(DungeonDoor door in existingRoom.doors)
        {
            door.transform.parent = this.transform;
            var sharedRooms = door.GetSharedRooms();
            foreach(DungeonRoom room in sharedRooms)
            {
                if (adjacentRooms.Contains(room))
                    door.SetRooms(this, room);
            }
        }       
    }

    internal void RemoveEnemy(EnemyAI enemyAI)
    {
        if (enemies.Contains(enemyAI)) enemies.Remove(enemyAI);
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

    public void RemovePlaceholders()
    {
        var placeholders = walls.GetComponentsInChildren<DungeonDoorPlaceholder>();
        foreach(var door in doors)
        {
            foreach(var placeholder in placeholders)
            {
                if(placeholder != null && door != null)
                {
                    Vector3 diff = placeholder.transform.position - door.transform.position;
                    if (diff.magnitude < 0.5f) DungeonData.SafeDestroy(placeholder.gameObject);
                }              
            }
        }
    }

    public void RemoveUnusedDoorWalls()
    {       
        StartCoroutine(OnNextFrameCleanup());
    }

    private IEnumerator OnNextFrameCleanup() //Cleanup all rooms on next frame
    {
        yield return null;
        var composites = walls.GetComponentsInChildren<WallComposite>();
        foreach (var composite in composites)
        {
            var plainWall = composite.plainWall;
            if (plainWall != null) DungeonData.SafeDestroy(composite.doorWall.gameObject);
        }
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
        Vector3 offset = new Vector3(0, newRoom.transform.localScale.y / 2, 0);
        newRoom.walls = DungeonDrawer.DrawSingleObject(newRoom.GetPosition() + offset, data.GetWallPrefab(), newRoom.gameObject);
        return newRoom;
    }

    public void BindWalls(GameObject walls)
    {
        //Debug.Log(walls == null);
        var southWestCornerWall = walls.transform.Find("Corner");
        var southEastCotnerWall = walls.transform.Find("Corner4");
        var middleWallNoDoor = walls.transform.Find("PlainWall1");
        var middleWallDoor = walls.transform.Find("DoorWall1");
        if (occludableWalls == null) occludableWalls = new List<OccludableWall>();
        if(southWestCornerWall != null)
        occludableWalls.Add(southWestCornerWall.GetComponentInChildren<OccludableWall>());
        if(southEastCotnerWall != null)
        occludableWalls.Add(southEastCotnerWall.GetComponentInChildren<OccludableWall>());
        if(middleWallDoor != null)
        occludableWalls.Add(middleWallDoor.GetComponentInChildren<OccludableWall>());
        if(middleWallNoDoor != null)
        occludableWalls.Add(middleWallNoDoor.GetComponentInChildren<OccludableWall>());
        GetBottomRoomOccludableWalls();
    }

    private void FindNorthWalls()
    {
        var topEastCornerWall = walls.transform.Find("Corner2");
        var topWestCornerWall = walls.transform.Find("Corner3");
        var topMiddleWallDoor = walls.transform.Find("PlainWall2");
        var topMiddleWallNoDoor = walls.transform.Find("DoorWall2");
        if (northWalls == null) northWalls = new List<OccludableWall>();
        if(topEastCornerWall != null)
        northWalls.Add(topEastCornerWall.GetComponentInChildren<OccludableWall>());
        if(topWestCornerWall != null)
        northWalls.Add(topWestCornerWall.GetComponentInChildren<OccludableWall>());
        if(topMiddleWallNoDoor)
        northWalls.Add(topMiddleWallNoDoor.GetComponentInChildren<OccludableWall>());
        if(topMiddleWallDoor != null)
        northWalls.Add(topMiddleWallDoor.GetComponentInChildren<OccludableWall>());
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
        Vector3 offset = new Vector3(0, loadedRoom.transform.localScale.y / 2, 0);
        loadedRoom.walls = DungeonDrawer.DrawSingleObject(loadedRoom.GetPosition() + offset, dungeonData.GetWallPrefab(), loadedRoom.gameObject);
        loadedRoom.isIsolated = roomData.IsIsolated();
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
    public static (bool, int) PathExists(DungeonRoom excludedRoom, DungeonRoom startRoom, DungeonRoom endRoom)
    {
        HashSet<DungeonRoom> visitedRooms = new HashSet<DungeonRoom>();
        bool path = DFS(excludedRoom, startRoom, endRoom, visitedRooms);
        return (path, visitedRooms.Count());
    }
}
