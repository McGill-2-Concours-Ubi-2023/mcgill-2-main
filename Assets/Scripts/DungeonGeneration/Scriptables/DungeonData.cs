using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "New dungeon", menuName = "Dungeon asset")]
public class DungeonData : ScriptableObject, DungeonRoomPrefabsContainer
{
    [SerializeField]
    public GameObject specialRoomPrefab;
    private GameObject mono;
    [SerializeField]
    private ScriptableObject activeLayout;
    [SerializeField]
    private ScriptableObject dungeonGrid;
    [SerializeField]
    [Range(0.1f, 1f)]
    private float roomDensity = 0.5f;
    [SerializeField]
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
        Cleanup();
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

    public void Cleanup()
    {
       foreach(DungeonRoom room in rooms)
       {
            room.RemovePlaceholders();
            room.RemoveUnusedDoorWalls(); 
       }
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

    //Reload the whole dungeon
    public void LoadData()
    {
        ClearDungeon();
        GetGrid().LoadRooms(GetActiveLayout().GetRoomsData(), this);
        FindStartingRoom();
        FindObjectOfType<MainCharacterController>().transform.position = startingRoom.transform.position;
        GetGrid().ReloadMiniMap(this);
        Cleanup();
        InitializeNavMesh();
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
