using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New dungeon", menuName = "Dungeon asset")]
public class DungeonData :DataContainer
{
    [SerializeField]
    private ScriptableObject activeLayout;
    [SerializeField]
    private ScriptableObject dungeonGrid;
    [SerializeField]
    [Range(0.1f, 1f)]
    private float roomDensity = 0.5f;
    private List<DungeonRoom> rooms = new List<DungeonRoom>();
    private DungeonRoom startingRoom;
    private MapManager mapM; 
    [SerializeField][Range(1, 50)]
    private int minRoomCount = 20;
    [SerializeField]
    private GameObject roomPrefab;
    [SerializeField]
    private GameObject doorPrefab;
    [SerializeField]
    private GameObject wallPrefab;

    public void GenerateDungeon()
    {
        ClearDungeon();
        GetGrid().SetMonoInstance(mono);
        GetGrid().GenerateGrid(this);
        GetGrid().GenerateRooms(this);
        SaveData();
        FindObjectOfType<MainCharacterController>().transform.position = GetActiveLayout().GetStartPosition();
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

    public GameObject GetRoomPrefab()
    {
        return roomPrefab;
    }

    public GameObject GetDoorPrefab()
    {
        return doorPrefab;
    }

    public GameObject GetWallPrefab()
    {
        return wallPrefab;
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

    public void ClearDungeon()
    {
        FindMapManager();
        mapM.ClearMap();
        rooms.Clear();
        GetGrid().ClearBuffer();    
        DungeonDrawer.EraseDungeon(mono);
        
    }

    public void SaveData()
    {
        var roomsPositions = new List<Vector3>();
        rooms.ForEach(room =>
        {
            roomsPositions.Add(room.transform.position);
        });
        GetActiveLayout().SaveStartPosition(startingRoom.transform.position);
        GetActiveLayout().SaveFloorData(roomsPositions);
    }

    public void LoadData()
    {
        ClearDungeon();
        GetGrid().LoadRooms(GetActiveLayout().GetFloorData(), this); 
        startingRoom = rooms.Where(room => room.transform.position == GetActiveLayout().GetStartPosition()).First();
        GetGrid().ReloadMiniMap(this);
    }
}
