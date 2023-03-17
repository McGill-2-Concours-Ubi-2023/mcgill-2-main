using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "New dungeon", menuName = "Dungeon asset")]
public class DungeonData : ScriptableObject, DungeonRoomPrefabsContainer
{
    private GameObject mono;
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
    [SerializeField][Header("Optional")]
    private NavMeshData navMeshData;

    public void GenerateDungeon()
    {
        ClearDungeon();
        GetGrid().GenerateGrid(this);
        GetGrid().GenerateRooms(this);
        SaveData();
        FindObjectOfType<MainCharacterController>().transform.position = GetActiveLayout().GetStartPosition();
        InitializeNavMesh();
    }

    public void AddRoom(DungeonRoom room)
    {
        rooms.Add(room);
    }

    public DungeonRoom GetActiveRoom()
    {
        return DungeonRoom.GetActiveRoom();
    }

    public int RoomCount()
    {
        return minRoomCount;
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
        return mono;
    }

    public void ClearDungeon()
    {
        if (rooms == null) rooms = new List<DungeonRoom>();
        FindMapManager();
        GetGrid().SetMonoInstance(mono);
        mapM.ClearMap();
        rooms.Clear();
        GetGrid().ClearData();    
        DungeonDrawer.EraseDungeon(mono);       
    }

    public void SetMonoInstance(GameObject mono)
    {
        this.mono = mono;
    }

    public void SaveData()
    {
        var roomsPositions = new List<Vector3>();
        rooms.ForEach(room =>
        {
            roomsPositions.Add(room.transform.position);
        });
        GetActiveLayout().SaveRoomsData(GetGrid().GetCustomMapBuffer());
    }

    public void AddRoomData(RoomData data)
    {
        GetGrid().AddData(data);
    }

    private void InitializeNavMesh()
    {
        NavMeshSurface navMeshSurface = mono.GetComponentInChildren<NavMeshSurface>();
        Debug.Assert(navMeshSurface);
        navMeshSurface.BuildNavMesh();
    }

    public void TryQuickLoad()
    {
        bool onQuickLoadSuccess = false;
        bool matchLayout;
        rooms = FindObjectsOfType<DungeonRoom>().ToList();
        //if it matches the layout data, it means we already generated the dungeon for that layout
        if (rooms != null)
        {
            matchLayout = rooms.Where(room => room.GetLayout() != GetActiveLayout().GetName()).ToList().Count() == 0
                && rooms.Count() == GetActiveLayout().GetRoomsData().Count();
            if(matchLayout)
            {
                FindStartingRoom();
                FindObjectOfType<MainCharacterController>().transform.position = startingRoom.transform.position;
                FindMapManager();
                onQuickLoadSuccess = true;
            }         
        }
        //if does not succeed, regenerate the whole dungeon
        InitializeNavMesh();
        if (!onQuickLoadSuccess) LoadData();
    }

    //Reload the whole dungeon
    public void LoadData()
    {
        ClearDungeon();
        GetGrid().LoadRooms(GetActiveLayout().GetRoomsData(), this);
        FindStartingRoom();
        FindObjectOfType<MainCharacterController>().transform.position = startingRoom.transform.position;
        GetGrid().ReloadMiniMap(this);
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
}
