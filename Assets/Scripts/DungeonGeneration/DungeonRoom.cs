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
    [SerializeField]
    private List<OccludableWall> southWalls;
    [SerializeField]
    private GameObject walls;
    private bool isIsolated;
    [SerializeField]
    private List<Enemy> enemies;
    private bool areEnemiesPresent;
    private static List<DungeonRoom> allRooms;
    private List<DungeonRoom> roomsBuffer;
    private bool cleared = false;
    public static int clearedRoomsCount;
    private List<DungeonLight> cachedLights;

    private void OnDistanceRender()
    {
        if (roomsBuffer == null) roomsBuffer = new List<DungeonRoom>();
        allRooms = dungeonGenerator.data.AllRooms();
        roomsBuffer.Clear();
        roomsBuffer.Add(this);
        adjacentRooms.ForEach(room => 
        {
            roomsBuffer.Add(room);
            room.UpdateLights(transform.position);
        });
        allRooms.Except(roomsBuffer)
            .ToList()
            .ForEach(room => room.transform.Find("RoomRoot").gameObject.SetActive(false));
        roomsBuffer.ForEach(room => 
        {
            room.transform.Find("RoomRoot").gameObject.SetActive(true);
            room.RetrieveDoors();
        });
    }

    private void UpdateLights(Vector3 position)
    {
        if (cachedLights == null) 
        {
            cachedLights = new List<DungeonLight>();
            doors.ForEach(door => cachedLights.AddRange(door.GetLights()));
            walls.GetComponent<DungeonWallAggregate>().neons
                .ToList()
                .ForEach(neon => cachedLights.Add(neon));          
        }
        cachedLights.ForEach(light => light.UpdateLight(position));
    }

    private void RetrieveDoors()
    {
        doors.ForEach(door => door.transform.parent = transform.Find("RoomRoot"));
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
        if(enemies == null)
        enemies = new List<Enemy>();
        if(type != RoomTypes.RoomType.Special && !cleared)
        {
            GetComponent<EnemySpawn1>().enabled = true;
            areEnemiesPresent = true;
            Vector3 detectionRange = transform.Find("RoomCollider").GetComponent<BoxCollider>().size;
            StartCoroutine(CheckForEnemies(detectionRange));
            Isolate();
        }   
    }

    public void UpdateRoomsLayout()
    {
        OnDistanceRender();
        foreach (OccludableWall wall in occludableWalls)
        {
            if (wall != null && wall.gameObject.activeInHierarchy)
            {
                wall.Hide();
                wall.ChangeRenderQueue(3001);
            }
        }

        if (northWalls == null) FindNorthWalls();
        foreach (OccludableWall wall in northWalls)
        {
            if (wall != null && wall.gameObject.activeInHierarchy) wall.ChangeRenderQueue(2998);
        }
    }

    public void TryRemoveEnemy(Enemy enemy)
    {
        if (enemies.Contains(enemy)) enemies.Remove(enemy);
    }

    IEnumerator CheckForEnemies(Vector3 detectionRange)
    {
        Collider[] colliders;
        while (areEnemiesPresent)
        {
            yield return new WaitForSeconds(2.0f);
            colliders = Physics.OverlapBox(transform.position, detectionRange);
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Enemy"))
                {
                    var enemy = collider.GetComponent<Enemy>();
                    if (!enemies.Contains(enemy) && enemy != null)
                    {
                        enemies.Add(enemy);
                        enemy.attachedRoom = this;
                    }
                }
            }
            areEnemiesPresent = enemies.Count > 0;
            //check every one second for enemies in the room
        }
        // room cleared
        GameObject.FindWithTag("Player").Trigger<IMainCharacterTriggers>(nameof(IMainCharacterTriggers.OnRoomCleared));
        OpenUp();
    }

    public void StopSpawnEnemies()
    {
        GetComponent<EnemySpawn1>().enabled = false;
    }

    public void FindBottomRoomNorthWalls()
    {
        foreach(DungeonRoom room in adjacentRooms)
        {
            if ((room.transform.position - transform.position).normalized == -Vector3.forward)
            {
                occludableWalls.AddRange(room.FindNorthWalls());
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

    public void GenerateDoors(DungeonData data)
    {
        adjacentRooms.ForEach(room =>
        {     
            if(room.doors.Count != 0 && !room.HasAccessToRoom(this)
            || room.doors.Count == 0) 
            {               
                var addedDoor = DungeonDoor.Create(transform.Find("RoomRoot").gameObject, this, room, data);
                doors.Add(addedDoor);                
            } else if (room.HasAccessToRoom(this))
            {
                room.doors.Where(door => door.GetSharedRooms().Contains(this))
                .ToList().ForEach(door => 
                {
                    this.doors.Add(door);
                });
            }                 
        });
        BindWalls(walls);
    }

    public void Isolate()
    {
        foreach(DungeonDoor door in doors)
        {
            var lights = door.GetLights();
            foreach(DoorLight light in lights)
            {
                light.TurnRed();
                light.UpdateLight(transform.position);
            }
            door.Block();
        }
    }

    //FBI ???
    public void OpenUp()
    {
        cleared = true;
        clearedRoomsCount++;
        foreach (DungeonDoor door in doors)
        {
            var lights = door.GetLights();
            foreach (DoorLight light in lights)
            {
                light.ResetColor();
                light.UpdateLight(transform.position);
            }
            door.Unlock();
        }
    }

    public bool IsIsolated()
    {
        return isIsolated;
    }

    public List<DungeonDoor> GetDoors()
    {
        return doors;
    }

    public void MoveDoorsUp()
    {
        foreach (var door in doors)
        {
            door.transform.parent = transform; //move doors upwards
        }
    }

    public void ReassignRoom(RoomTypes.RoomType type)
    {
        this.type = type;      
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
        newRoom.walls = DungeonDrawer.DrawSingleObject(newRoom.GetPosition() + offset, data.GetWallPrefab(),
            newRoom.transform.Find("RoomRoot").gameObject);
        return newRoom;
    }

    public void BindWalls(GameObject walls)
    {
        if (occludableWalls == null) occludableWalls = new List<OccludableWall>();
        FindSouthWalls();
        FindBottomRoomNorthWalls();
    }

    private List<OccludableWall> FindSouthWalls()
    {
        if (southWalls == null) southWalls = new List<OccludableWall>();
        southWalls = walls.GetComponent<DungeonWallAggregate>().southWalls.ToList();
        occludableWalls.AddRange(southWalls);
        return southWalls;
    }
    private List<OccludableWall> FindNorthWalls()
    {
        if (northWalls == null) northWalls = new List<OccludableWall>();
        northWalls = walls.GetComponent<DungeonWallAggregate>().northWalls.ToList();
        return northWalls;
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
        loadedRoom.walls = DungeonDrawer.DrawSingleObject(loadedRoom.GetPosition() + offset, dungeonData.GetWallPrefab(),
            loadedRoom.transform.Find("RoomRoot").gameObject);
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
