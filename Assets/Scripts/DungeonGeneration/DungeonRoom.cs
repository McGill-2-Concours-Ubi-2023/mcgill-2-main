using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

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
    private static bool spawnPortal;
    public RoomFog fog;
    private bool hasSpawned = false;

    internal void AddEnemy(Enemy enemy)
    {
        if (enemy != null && !enemies.Contains(enemy)) 
            enemies.Add(enemy);
    }

    private void OnDistanceRender()
    {
        if (roomsBuffer == null) roomsBuffer = new List<DungeonRoom>();
        allRooms = dungeonGenerator.data.AllRooms();
        roomsBuffer.Clear();
        roomsBuffer.Add(this);
        foreach(DungeonRoom room in adjacentRooms)
        {
            roomsBuffer.Add(room);
            room.UpdateLights(transform.position);
        }
        List<DungeonRoom> inter = allRooms.Except(roomsBuffer).ToList();
        foreach(DungeonRoom room in inter)
        {
            if (room.GetRoomType() != RoomTypes.RoomType.Boss)
            room.transform.Find("RoomRoot").gameObject.SetActive(false);
            room.DeactivateEnemies();
        }
        foreach (DungeonRoom room in roomsBuffer)
        {
            room.transform.Find("RoomRoot").gameObject.SetActive(true);
            room.RetrieveDoors();
        }
    }
    

    private void Update()
    {
        if(clearedRoomsCount >= 7 && spawnPortal == false)
        {
            spawnPortal = true;
           StartCoroutine(PlaceBossRoom());
        }
    }

    IEnumerator PlaceBossRoom()
    {
        bool isPlaced = false;
        while (!isPlaced)
        {
            int randInt = UnityEngine.Random.Range(Mathf.FloorToInt(allRooms.Count / 2), allRooms.Count - 1);
            DungeonRoom choosenRoom = allRooms[randInt];
            foreach (DungeonRoom room in activeRoom.adjacentRooms)
            {
                //if the length from this room to the 
                int shortesPathLength = DungeonRoom.ShortestPath(activeRoom, choosenRoom);
                if ( shortesPathLength > 2 && shortesPathLength < 5)
                {
                    Debug.Log("Shortest path to portal: " + shortesPathLength);
                    DungeonDrawer.ReplaceRoom(choosenRoom, dungeonGenerator.data,
                    dungeonGenerator.data.GetPortalPrefab(), RoomTypes.RoomType.Boss, false);
                    isPlaced = true;
                }
                yield return new WaitForEndOfFrame();              
            }
            yield return new WaitForEndOfFrame();
        }
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
        spawnPortal = false;
        clearedRoomsCount = 0;
    }

    public void SpawnEnemies()
    {
        if (!hasSpawned)
        {
            hasSpawned = true;
            if (enemies == null)
                enemies = new List<Enemy>();
            bool isValidRoom = type != RoomTypes.RoomType.Special && type != RoomTypes.RoomType.Start
                && type != RoomTypes.RoomType.Boss;
            if (isValidRoom && !cleared)
            {
                GetComponent<EnemySpawn1>().SpawnEnemies(this);
                foreach(Enemy enemy in enemies)
                {
                    if (enemy != null) enemy.FreezeOnCurrentState();
                }
                areEnemiesPresent = true;
            }
        }          
    }

    private void OpenBossPortal()
    {
        VisualEffect portalVFX = FindObjectOfType<Portal>()
            .GetComponent<VisualEffect>();
        portalVFX.SendEvent("OnPortalAppear");
    }

    public async Task TryUpdateFog()
    {
        while(fog == null)
        {
           fog = transform.Find(DungeonDrawer.persistentFogPath)
                .GetComponent<RoomFog>();
            await Task.Yield();
        }
        DissipateFog();
    }

    private async void DissipateFog()
    {
        while (!fog.isDissipated)
        {
            fog.DissipateAmbientFog();
            await Task.Delay(500);
        }
    }

    public async Task UpdateRoomsLayout()
    {
        if (type == RoomTypes.RoomType.Start) await TryUpdateFog();
        adjacentRooms.ForEach(room => room.SpawnEnemies());
        if (type == RoomTypes.RoomType.Boss)
        {
            OpenBossPortal();
        }    
        OnDistanceRender();
        await Task.Yield();
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
        await Task.Yield();
        bool condition = type != RoomTypes.RoomType.Special
               && type != RoomTypes.RoomType.Boss
               && type != RoomTypes.RoomType.Start;
        if (condition)
        {
            Isolate();
        }
    }

    IEnumerator CheckForEnemies()
    {
        while (areEnemiesPresent)
        {
            yield return new WaitForSeconds(1.0f);
            enemies.RemoveAll(enemy => enemy == null || enemy.isDying);
            areEnemiesPresent = enemies.Count > 0;
        }
        // room cleared
        try
        {
            GameObject.FindWithTag("Player").Trigger<IMainCharacterTriggers>(nameof(IMainCharacterTriggers.OnRoomCleared));
        }
        catch (InventoryFullException<SimpleCollectible> e)
        {
            // ignored
        }
        OpenUp();
    }

    public void DeactivateEnemies()
    {
        if(enemies != null)
        {
            foreach(Enemy enemy in enemies)
            {
                if (enemy != null)
                {
                    enemy.FreezeOnCurrentState();
                    enemy.gameObject.SetActive(false);
                }           
            }
        }
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

    public void ActivateEnemies()
    {
        foreach(Enemy enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.gameObject.SetActive(true);
                if (type == RoomTypes.RoomType.Boss)
                {
                    enemies.Remove(enemy);
                    Destroy(enemy.gameObject);
                }
            }
        }
    }

    public async void Isolate()
    {
        try
        {
            if (!cleared)
            {
                DungeonRoom.lastEnteredDoor.CloseDoor();
                enemies.ForEach(enemy => enemy.UnFreeze());
                await TryUpdateFog();
                StartCoroutine(CheckForEnemies());
                foreach (DungeonDoor door in doors)
                {
                    var lights = door.GetLights();
                    foreach (DoorLight light in lights)
                    {
                        light.TurnRed();
                        light.UpdateLight(transform.position);
                    }
                    door.Block();
                }
            }
        }
        catch
        {
            //ignore
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

    public static async Task<DungeonRoom> CreateRandomRoom(DungeonData data, Vector3 position, Dictionary<Vector3, Vector2Int> gridMap,
        string layout, RoomTypes.RoomType type) 
    {
        //This is where the room gets instantiated, change the primitive and pass a prefab instead for the room
        //Eg: var roomObj = DungeonDrawer.DrawSingleObject(position, prefab, data.GetMonoInstance(), scale) as GameObject;
        var roomObj = DungeonDrawer.DrawRandomRoom(position, type, data);
        await Task.Yield();
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

    public static int ShortestPath(DungeonRoom startRoom, DungeonRoom endRoom)
    {
        Dictionary<DungeonRoom, int> distances = new Dictionary<DungeonRoom, int>();
        HashSet<DungeonRoom> visitedRooms = new HashSet<DungeonRoom>();

        foreach (DungeonRoom room in allRooms)
        {
            distances[room] = int.MaxValue;
        }
        distances[startRoom] = 0;

        DungeonRoom.DFS(startRoom, null, endRoom, visitedRooms, distances);

        return distances[endRoom];
    }

    public static void DFS(DungeonRoom currentRoom, DungeonRoom excludedRoom, DungeonRoom endRoom, HashSet<DungeonRoom> visitedRooms, Dictionary<DungeonRoom, int> distances)
    {
        if (currentRoom == endRoom)
        {
            return; // We've reached the end room, so we can stop searching
        }

        visitedRooms.Add(currentRoom);

        foreach (DungeonRoom adjacentRoom in currentRoom.adjacentRooms)
        {
            if (!visitedRooms.Contains(adjacentRoom) && adjacentRoom != excludedRoom)
            {
                int newDistance = distances[currentRoom] + 1; // Each edge has a weight of 1
                if (newDistance < distances[adjacentRoom])
                {
                    distances[adjacentRoom] = newDistance;
                    DFS(adjacentRoom, currentRoom, endRoom, visitedRooms, distances);
                }
            }
        }
    }
}
