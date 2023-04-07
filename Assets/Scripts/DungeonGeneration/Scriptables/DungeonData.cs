using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;
using Debug = UnityEngine.Debug;

[CreateAssetMenu(fileName = "New dungeon", menuName = "Dungeon asset")]
public class DungeonData : ScriptableObject, DungeonRoomPrefabsContainer
{
    [SerializeField]
    public GameObject specialRoomPrefab;
    private MonoBehaviour mono;
    [SerializeField]
    private ScriptableObject activeLayout;
    [SerializeField]
    private ScriptableObject dungeonGrid;
    [SerializeField]
    [Range(0.1f, 1f)]
    private float roomDensity = 0.5f;
    [SerializeField][HideInInspector]
    private List<DungeonRoom> rooms;
    [SerializeField][HideInInspector]
    private DungeonRoom startingRoom;
    [SerializeField][HideInInspector]
    private MapManager mapM; 
    [SerializeField][Range(1, 50)]
    private int minRoomCount = 20;
    [SerializeField]
    private GameObject[] StartRoomPrefabs;
    [SerializeField]
    private GameObject[] NormalRoomPrefabs;
    [SerializeField]
    private GameObject[] TreasureRoomPrefabs;
    [SerializeField]
    private GameObject[] SpecialRoomPrefabs;
    [SerializeField]
    private GameObject doorPrefab;
    [SerializeField]
    private GameObject wallsPrefab;
    [SerializeField][Header("Replace rooms")]
    private GameObject[] roomOverrides;
    [SerializeField]
    private GameObject portalPrefab;

    public async Task GenerateDungeon()
    {
        GameManager manager = GameManager.Instance;
        manager.ReportProgress(0.5f, 0.6f);
        using Stopwatch sw = new Stopwatch(nameof(GenerateDungeon));
        await ClearDungeon();
        manager.ReportProgress(0.6f, 0.7f);
        await GetGrid().GenerateGrid(this);
        manager.ReportProgress(0.7f, 0.8f);
        await GetGrid().GenerateRooms(this);
        manager.ReportProgress(0.8f, 0.9f);
        await SaveData();
        manager.ReportProgress(0.9f, 1.0f);
        FindObjectOfType<MainCharacterController>().transform.position = GetActiveLayout().GetStartPosition();
        await InitializeNavMesh();
        await Cleanup();
        await GetGrid().ReloadMiniMap(this);
        manager.ReportProgress(1.0f, 1.0f);
    }

    public GameObject GetPortalPrefab()
    {
        return portalPrefab;
    }

    public void AddRoom(DungeonRoom room)
    {
        rooms.Add(room);
    }
    public static void SafeDestroy(GameObject obj)
    {
    #if UNITY_EDITOR
        // Check if we're in the Unity editor
        if (Application.isPlaying)
        {
            // If in play mode, destroy the object immediately
            GameObject.Destroy(obj);
        }
        else
        {
            // If in editor mode, schedule the object for destruction in the editor
            GameObject.DestroyImmediate(obj);
        }
    #else
        // If not in editor mode, destroy the object immediately
        Object.Destroy(obj);
    #endif
    }


    public DungeonRoom GetActiveRoom()
    {
        return DungeonRoom.GetActiveRoom();
    }

    public int RoomCount()
    {
        return minRoomCount;
    }

    public void PlaceMerchant()
    {
        GetGrid().PlaceMerchant();
    }

    public void SetStartingRoom(DungeonRoom room)
    {
        startingRoom = room;
    }

    public DungeonRoom GetStartingRoom()
    {
        return startingRoom;
    }

    public float RoomDensity()
    {
        return roomDensity;
    }

    public void RemoveRoom(DungeonRoom room) 
    {
        rooms.Remove(room);
    }

    public GameObject GetDoorPrefab()
    {
        return doorPrefab;
    }

    public GameObject GetWallPrefab()
    {
        return wallsPrefab;
    }

    public DungeonLayout GetActiveLayout()
    {
        return activeLayout as DungeonLayout;
    }

    public DungeonGrid GetGrid()
    {
        return dungeonGrid as DungeonGrid;
    }

    public List<DungeonRoom> AllRooms()
    {
        return rooms;
    }

    public MapManager GetMapManager()
    {
        return mapM;
    }

    public void FindMapManager()
    {
        mapM = FindObjectOfType<MapManager>();
    }

    public GameObject GetMonoInstance()
    {
        if(mono != null)
        return mono.gameObject;
        return null;
    }

    public async Task Cleanup()
    { 
        using Stopwatch sw = new Stopwatch(nameof(Cleanup));
        foreach(DungeonRoom room in rooms)
        {
             room.RemovePlaceholders();
             room.RemoveUnusedDoorWalls(); 
             await Task.Yield();
        }
    }

    public async Task ClearDungeon()
    {
        using Stopwatch sw = new Stopwatch(nameof(ClearDungeon));
        if (rooms == null) rooms = new List<DungeonRoom>();
        FindMapManager();
        await Task.Yield();
        GetGrid().SetMonoInstance(mono);
        mapM.ClearMap();
        await Task.Yield();
        rooms.Clear(); 
        GetGrid().ClearData();   
        await Task.Yield();
        await DungeonDrawer.EraseDungeon(mono.gameObject);       
    }

    public void SetMonoInstance(MonoBehaviour mono)
    {
        this.mono = mono;
    }

    public async Task SaveData()
    {
        using Stopwatch sw = new Stopwatch(nameof(SaveData));
        var roomsPositions = new List<Vector3>();
        foreach (DungeonRoom room in rooms)
        {
            roomsPositions.Add(room.transform.position);
            await Task.Yield();
        }
        GetActiveLayout().SaveRoomsData(GetGrid().GetCustomMapBuffer());
    }

    public void AddRoomData(RoomData data)
    {
        GetGrid().AddData(data);
    }

    private async Task InitializeNavMesh()
    {
        NavMeshSurface navMeshSurface = mono.GetComponentInChildren<NavMeshSurface>();
        Debug.Assert(navMeshSurface);
        await Task.Yield();
        navMeshSurface.BuildNavMesh();
        await Task.Yield();
    }

    //Reload the whole dungeon
    public async Task LoadData()
    {
        await ClearDungeon();
        await GetGrid().LoadRooms(this);
        FindStartingRoom();
        FindObjectOfType<MainCharacterController>().transform.position = startingRoom.transform.position;
        await GetGrid().ReloadMiniMap(this);
        await Cleanup();
        await InitializeNavMesh();
    }

    public void PlaceMerchantRoom()
    {
        int randIndex = UnityEngine.Random.Range(0, AllRooms().Count);

    }

    private void FindStartingRoom()
    {
        var startRoomdata = GetActiveLayout()
                    .GetRoomsData()
                    .Where(roomData => roomData.GetRoomType() == RoomTypes.RoomType.Start).First();
        startingRoom = rooms.Where(room => room.GetRoomType() == startRoomdata.GetRoomType()).First();
    }

    public GameObject[] GetNormalRoomPrefabs()
    {
        return NormalRoomPrefabs;
    }

    public GameObject[] GetStartRoomPrefabs()
    {
        return StartRoomPrefabs;
    }

    public GameObject[] GetSpecialRoomPrefabs()
    {
        return SpecialRoomPrefabs;
    }

    public GameObject[] GetTreasureRoomPrefabs()
    {
        return TreasureRoomPrefabs;
    }

    public GameObject[] GetRoomOverrides()
    {
        return roomOverrides;
    }


}
